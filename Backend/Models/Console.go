/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-05-06 22:43:42
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-05-15 10:31:39
 */
package Models

import (
	"bytes"
	"encoding/json"
	"io"
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
		Consumes("multipart/form-data", restful.MIME_OCTET, restful.MIME_XML, restful.MIME_JSON).
		Produces("multipart/form-data", restful.MIME_OCTET, restful.MIME_XML, restful.MIME_JSON)

	ws.Route(ws.GET("/").To(c.Index))
	ws.Route(ws.GET("/login").To(c.Index))

	ws.Route(ws.GET("/dashboard").To(c.Dashboard))
	ws.Route(ws.GET("/manage").To(c.Manage))

	ws.Route(ws.GET("/upload").To(c.Upload))
	ws.Route(ws.POST("/upload").To(c.Upload))

	return ws
}

var token string

func init() {
	log.Error(parserJson(&Config))
	login_url := "https://portal.aryun.com/account/signin"

	form := url.Values{
		"account":  {Config.HiUsername},
		"password": {Config.HiPassword},
	}

	body_byte, err := sendPost(login_url, "application/x-www-form-urlencoded", form)
	if err != nil {
		panic(err)
	}

	type HiARLoginResp struct {
		Token string `json:"token"`
	}
	hiresp := &HiARLoginResp{}

	log.Error(json.Unmarshal(body_byte, hiresp))
	token = hiresp.Token

	go keepTokenAlive(token)
}

// Login page
func (c *ConsoleResource) Index(request *restful.Request, response *restful.Response) {
	p := NewConsoleWithStaticFilePrefix(request)

	t, err := template.ParseFiles("Models/Templates/login.html")
	if err != nil {
		log.Fatalf("Template gave: %s", err)
	}
	t.Execute(response.ResponseWriter, p)
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
	if request.Request.Method == "GET" {
		p := NewConsoleWithStaticFilePrefix(request)
		p.PageName = "upload"
		p.TotalPosters = 25
		p.TotalResources = 70

		t, err := template.ParseFiles("Models/Templates/layout.tmpl",
			"Models/Templates/upload.tmpl")
		if err != nil {
			log.Fatalf("Template gave: %s", err)
		}

		t.Execute(response.ResponseWriter, p)
	} else {
		req := request.Request
		req.ParseForm()

		file, handler, err := req.FormFile("fileInput")
		if err != nil {
			log.Error("FormFile: ", err.Error())
			return
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
			return
		}

		response.ResponseWriter.Write(fbytes)

		title := req.Form["title"][0]
		//filename := handler.Filename
		location := req.Form["location"][0]
		datetime := req.Form["datetime"][0]
		url := req.Form["url"][0]
		log.Info(title, "\n", location, "\n", datetime, "\n", url)

		createHiARMaterial(title+"_"+time.Now().Format("20060102_150405"), handler)
	}
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

func NewConsoleWithStaticFilePrefix(request *restful.Request) *Console {
	host := request.Request.Host
	return &Console{StaticFilePrefix: "http://" + host + "/files/res"}
}

func createHiARMaterial(name string, fileheader *multipart.FileHeader) error {
	upload_url := "https://portal.aryun.com/api/material/creation"

	body := &bytes.Buffer{}
	writer := multipart.NewWriter(body)

	file, err := fileheader.Open()
	if err != nil {
		return err
	}

	if part, err := writer.CreateFormFile("imgFile", fileheader.Filename); err != nil {
		return err
	} else {
		if _, err := io.Copy(part, file); err != nil {
			return err
		}
	}

	writer.WriteField("name", name)
	writer.WriteField("collectionid", Config.CollectionId)

	if err = writer.Close(); err != nil {
		return err
	}

	req, err := http.NewRequest("POST", upload_url, body)
	if err != nil {
		return err
	}

	req.Header.Set("Content-Type", writer.FormDataContentType())
	req.Header.Set("token", token)

	client := &http.Client{}
	resp, err := client.Do(req)
	if err != nil {
		return err
	}

	rspbody := &bytes.Buffer{}
	_, err = rspbody.ReadFrom(resp.Body)
	if err != nil {
		log.Fatal(err)
	}
	resp.Body.Close()
	//log.Println(resp.StatusCode)
	//log.Println(resp.Header)
	log.Println(rspbody)

	return err
}

func keepTokenAlive(token string) {
	for {
		<-time.After(60 * time.Second)
		keepalive_url := "https://portal.aryun.com/api/account/keepAlive"

		form := url.Values{
			"token": {token},
		}

		_, err := sendPost(keepalive_url, "application/x-www-form-urlencoded", form)
		if err != nil {
			log.Error(err)
		} else {
			log.Info("keep HiAR token alive")
		}
	}
}

func sendPost(url, contentType string, form url.Values) ([]byte, error) {
	body := bytes.NewBufferString(form.Encode())

	resp, err := http.Post(url, contentType, body)

	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()

	return ioutil.ReadAll(resp.Body)
}
