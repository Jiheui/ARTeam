/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-05-06 22:43:42
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-08-15 20:56:31
 */
package Models

import (
	"../Tools"
	"bytes"
	"encoding/gob"
	"encoding/json"
	"errors"
	"io/ioutil"
	"mime/multipart"
	"net/http"
	"net/url"
	"text/template"
	"time"

	log "github.com/Sirupsen/logrus"
	"github.com/emicklei/go-restful"
)

type Console struct {
	StaticFilePrefix string
	PageName         string

	// Personal information
	Username  string
	Password  string
	AvatarUrl string

	// Dashboard
	TotalPosters   int
	TotalResources int
}

type ConsoleResource struct {
}

func (c *ConsoleResource) WebService() *restful.WebService {
	ws := new(restful.WebService)
	ws.
		Path("/console").
		Consumes("application/x-www-form-urlencoded", "multipart/form-data", restful.MIME_OCTET, restful.MIME_XML, restful.MIME_JSON).
		Produces("application/x-www-form-urlencoded", "multipart/form-data", restful.MIME_OCTET, restful.MIME_XML, restful.MIME_JSON)

	ws.Route(ws.GET("/").To(c.Index))
	ws.Route(ws.POST("/").To(c.Index))
	ws.Route(ws.GET("/login").To(c.Index))
	ws.Route(ws.POST("/login").To(c.Index))

	ws.Route(ws.GET("/dashboard").Filter(basicAuthenticate).To(c.Dashboard))
	ws.Route(ws.GET("/manage").Filter(basicAuthenticate).To(c.Manage))

	ws.Route(ws.GET("/upload").Filter(basicAuthenticate).To(c.Upload))
	ws.Route(ws.POST("/upload").Filter(basicAuthenticate).To(c.Upload))

	return ws
}

var token string

func init() {
	gob.Register(&User{})
}

/*
*
*	Routers
*
***/

// Login page
func (c *ConsoleResource) Index(request *restful.Request, response *restful.Response) {
	if request.Request.Method == "GET" {
		p := NewConsoleWithStaticFilePrefix(request)

		t, err := template.ParseFiles("Models/Templates/login.html")
		if err != nil {
			log.Fatalf("Template gave: %s", err)
		}
		t.Execute(response.ResponseWriter, p)
	} else {
		req := request.Request
		req.ParseForm()

		username := req.Form["username"][0]
		rawPassword := req.Form["password"][0]
		password := Tools.EncodePassword(rawPassword)
		hostURL := "http://" + req.Host + "/users/login"

		if usr, err := sendLoginInfo(username, password, hostURL); err != nil {
			log.Error(err)
			response.WriteHeader(http.StatusBadRequest)
			return
		} else {
			session, err := Store.Get(request.Request, "ARPosterCookie")
			if err != nil {
				log.Error(err)
				response.WriteHeader(http.StatusInternalServerError)
				return
			}

			usr.Password = ""
			log.Info(usr)
			session.Values["authenticated"] = true
			session.Values["userdata"] = usr
			log.Info(session.Save(req, response.ResponseWriter))

			http.Redirect(response.ResponseWriter,
				req,
				"/console/dashboard",
				303)
		}
	}
}

// Dashboard page
func (c *ConsoleResource) Dashboard(request *restful.Request, response *restful.Response) {
	p := NewConsoleWithStaticFilePrefix(request)
	p.PageName = "dashboard"
	p.TotalPosters = 25
	p.TotalResources = 70

	t, err := template.ParseFiles("Models/Templates/layout.tmpl",
		"Models/Templates/dashboard.tmpl")
	if err != nil {
		log.Fatalf("Template gave: %s", err)
	}

	t.Execute(response.ResponseWriter, p)
}

// Upload page
func (c *ConsoleResource) Upload(request *restful.Request, response *restful.Response) {
	// if request.Request.Method == "GET" {
	// 	p := NewConsoleWithStaticFilePrefix(request)
	// 	p.PageName = "upload"
	// 	p.TotalPosters = 25
	// 	p.TotalResources = 70

	// 	t, err := template.ParseFiles("Models/Templates/layout.tmpl",
	// 		"Models/Templates/upload.tmpl")
	// 	if err != nil {
	// 		log.Fatalf("Template gave: %s", err)
	// 	}

	// 	t.Execute(response.ResponseWriter, p)
	// } else {
	// 	req := request.Request
	// 	req.ParseForm()

	// 	_, fileHeader, err := GetFileFromRequest("fileInput", req)
	// 	if err != nil {
	// 		log.Error(err)
	// 		response.WriteHeader(http.StatusBadRequest)
	// 		return
	// 	}

	// 	title := req.Form["title"][0]
	// 	//filename := handler.Filename
	// 	location := req.Form["location"][0]
	// 	datetime := req.Form["datetime"][0]
	// 	mapurl := req.Form["mapurl"][0]
	// 	link := req.Form["url"][0]
	//log.Info(title, "\n", location, "\n", datetime, "\n", link)

	// if targetId, err := createHiARMaterial(title+"_"+time.Now().Format("20060102_150405"), fileHeader); err != nil {
	// 	log.Error(err)
	// 	response.WriteHeader(http.StatusInternalServerError)
	// 	return
	// } else {
	// 	_, fileHeader, err := GetFileFromRequest("thumbnail", req)
	// 	if err != nil {
	// 		log.Error(err)
	// 		response.WriteHeader(http.StatusBadRequest)
	// 		return
	// 	}

	// 	upload_url := "http://" + req.Host + "/files/upload/"
	// 	fileSuffix := path.Ext(path.Base(fileHeader.Filename))
	// 	resURL := upload_url + Config.KeyGroup + "_" + targetId + fileSuffix
	// 	if err := uploadThumbnail(targetId, resURL, fileHeader); err != nil {
	// 		log.Error(err)
	// 		response.WriteHeader(http.StatusBadRequest)
	// 		return
	// 	}

	// 	hostURL := "http://" + req.Host + "/posters"
	// 	resURL = "http://" + req.Host + "/files/" + Config.KeyGroup + "_" + targetId + fileSuffix
	// 	if err := storePublishInformation(targetId, title, datetime, location, mapurl, link, resURL, hostURL); err != nil {
	// 		log.Error(err)
	// 		response.WriteHeader(http.StatusBadRequest)
	// 		return
	// 	}
	// }

	// 	http.Redirect(response.ResponseWriter,
	// 		request.Request,
	// 		"/console/manage",
	// 		http.StatusTemporaryRedirect)
	// }
}

// Manage page
func (c *ConsoleResource) Manage(request *restful.Request, response *restful.Response) {
	p := NewConsoleWithStaticFilePrefix(request)
	p.PageName = "manage"
	p.TotalPosters = 25
	p.TotalResources = 70

	t, err := template.ParseFiles("Models/Templates/layout.tmpl",
		"Models/Templates/manage.tmpl")
	if err != nil {
		log.Fatalf("Template gave: %s", err)
	}

	t.Execute(response.ResponseWriter, p)
}

/*
*
*	Tools
*
***/
func uploadThumbnail(targetId, url string, fileHeader *multipart.FileHeader) error {
	file, err := fileHeader.Open()
	if err != nil {
		return err
	}

	req, err := http.NewRequest("POST", url, file)
	if err != nil {
		return err
	}
	req.Header.Set("Content-Type", "application/octet-stream")

	client := &http.Client{}
	res, err := client.Do(req)
	if err != nil {
		return err
	}
	defer res.Body.Close()
	return err
}

func sendLoginInfo(username, password, hostURL string) (*User, error) {
	usr := &User{}
	if Tools.CheckEmail(username) {
		usr.Email = username
	} else {
		usr.Username = username
	}
	usr.Password = password

	b, err := json.Marshal(usr)
	if err != nil {
		return nil, err
	}
	body := &bytes.Buffer{}
	body.WriteString(string(b))

	req, err := http.NewRequest("POST", hostURL, body)
	if err != nil {
		return nil, err
	}

	req.Header.Set("Content-Type", "application/json")

	client := &http.Client{}
	res, err := client.Do(req)
	if err != nil {
		return nil, err
	}
	defer res.Body.Close()

	rbytes, err := ioutil.ReadAll(res.Body)
	if err != nil {
		return nil, err
	}

	ur := &UsersResponse{}
	if err := json.Unmarshal(rbytes, ur); err != nil {
		return nil, err
	}

	if ur.Success {
		return &ur.User, nil
	} else {
		return nil, errors.New("login failed: " + username + " " + password)
	}
}

func basicAuthenticate(req *restful.Request, resp *restful.Response, chain *restful.FilterChain) {
	session, _ := Store.Get(req.Request, "ARPosterCookie")

	if auth, ok := session.Values["authenticated"].(bool); !ok || !auth {
		log.Info(ok, auth)
		http.Redirect(resp.ResponseWriter, req.Request, "/console/login", http.StatusTemporaryRedirect)
		return
	}

	chain.ProcessFilter(req, resp)
}

func keepTokenAlive(token string) {
	for {
		<-time.After(60 * time.Second)
		keepalive_url := "https://api.hiar.io/v1/account/keepAlive"

		form := url.Values{
			"token": {token},
		}

		b, err := sendSimplePost(keepalive_url, "application/x-www-form-urlencoded", form)
		if err != nil {
			log.Error(err)
		} else {
			log.Info("keep HiAR token alive: ", string(b))
		}
	}
}

func sendSimplePost(url, contentType string, form url.Values) ([]byte, error) {
	body := bytes.NewBufferString(form.Encode())

	resp, err := http.Post(url, contentType, body)

	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()

	return ioutil.ReadAll(resp.Body)
}

func sendPostWithToken(url string, writer *multipart.Writer, body *bytes.Buffer) (string, error) {
	req, err := http.NewRequest("POST", url, body)
	if err != nil {
		return "", err
	}

	req.Header.Set("Content-Type", writer.FormDataContentType())
	req.Header.Set("token", token)

	client := &http.Client{}
	resp, err := client.Do(req)
	if err != nil {
		return "", err
	}
	defer resp.Body.Close()

	rspbody := &bytes.Buffer{}
	_, err = rspbody.ReadFrom(resp.Body)
	if err != nil {
		return "", err
	}
	//log.Println(resp.StatusCode)
	//log.Println(resp.Header)
	//log.Println(rspbody)

	type HiARResp struct {
		TargetId string `json:"targetid"`
		RetCode  int    `json:"retCode"`
		Comment  string `json:"comment"`
	}
	hiresp := &HiARResp{}

	if err := json.Unmarshal(rspbody.Bytes(), hiresp); err != nil {
		return "", err
	} else if hiresp.RetCode != 0 {
		return "", errors.New(hiresp.Comment)
	} else {
		return hiresp.TargetId, err
	}
}

func NewConsoleWithStaticFilePrefix(request *restful.Request) *Console {
	host := request.Request.Host
	return &Console{StaticFilePrefix: "http://" + host + "/files/res"}
}

func GetFileFromRequest(filename string, req *http.Request) ([]byte, *multipart.FileHeader, error) {
	file, handler, err := req.FormFile(filename)
	if err != nil {
		return nil, nil, err
	}
	defer func() {
		if err := file.Close(); err != nil {
			log.Error("Close: ", err.Error())
			return
		}
	}()

	fbytes, err := ioutil.ReadAll(file)
	if err != nil {
		log.Error("ReadAll: ", err.Error())
		return nil, nil, err
	}
	return fbytes, handler, err
}
