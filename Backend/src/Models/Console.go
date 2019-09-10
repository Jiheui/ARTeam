/*
 * @Author: Yutao Ge
 * @Date: 2019-05-06 22:43:42
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-09-10 00:52:22
 * @Description:
 */
package Models

import (
	"Tools"
	"encoding/gob"
	"encoding/json"
	"errors"
	"io/ioutil"
	"mime/multipart"
	"net/http"
	"os"
	"path"
	"strconv"
	"strings"
	"text/template"
	"time"

	log "github.com/Sirupsen/logrus"
	"github.com/emicklei/go-restful"
)

type Console struct {
	StaticFilePrefix string
	PageName         string

	// Personal information
	UserInfo  *User
	AvatarUrl string

	// Dashboard
	TotalPosters   int
	TotalResources int

	// Manage
	Pos Poster
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
	ws.Route(ws.POST("/manage").Filter(basicAuthenticate).To(c.Manage))

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

// Index used as login page
func (c *ConsoleResource) Index(request *restful.Request, response *restful.Response) {
	if request.Request.Method == "GET" {
		p := newConsoleWithStaticFilePrefix(request)

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
	p := newConsoleWithStaticFilePrefix(request)
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
	session, _ := Store.Get(request.Request, "ARPosterCookie")
	usr, ok := session.Values["userdata"].(*User)
	if !ok {
		log.Error("Load user data from session failed.")
	}

	if request.Request.Method == "GET" {
		p := newConsoleWithStaticFilePrefix(request)
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
		req.ParseMultipartForm(1048576000)

		f, thumbnail, err := getFileFromRequest("thumbnail", req)
		if err != nil {
			log.Error(err)
			response.WriteHeader(http.StatusBadRequest)
			return
		}
		_, posterModel, err := getFileFromRequest("armodel", req)
		if err != nil {
			log.Error(err)
			response.WriteHeader(http.StatusBadRequest)
			return
		}

		t, _ := strconv.Atoi(req.Form["type"][0])
		new_poster_info := &Poster{
			PosTitle:     req.Form["title"][0],
			PosDate:      req.Form["datetime"][0],
			PosLocation:  req.Form["location"][0],
			PosMap:       req.Form["mapurl"][0],
			PosLink:      req.Form["url"][0],
			Relevantinfo: req.Form["rvntinfo"][0],
			Type:         t,
		}
		upload_url := "http://" + req.Host + "/files/upload/"
		file_url_prefix := "http://" + req.Host + "/files/"
		fileSuffix := path.Ext(path.Base(thumbnail.Filename))

		thumbnail_filename := "thumbnail_" + usr.Username + "_" + new_poster_info.PosTitle + "_" + time.Now().Format("Mon-02-Jan-2006-15-04") + fileSuffix
		new_poster_info.Thumbnail = file_url_prefix + thumbnail_filename

		fileSuffix = path.Ext(path.Base(posterModel.Filename))
		model_filename := "model_" + usr.Username + "_" + new_poster_info.PosTitle + "_" + time.Now().Format("Mon-02-Jan-2006-15-04") + fileSuffix
		new_poster_info.Model = file_url_prefix + model_filename

		metaDataBytes, err := json.Marshal(new_poster_info)
		if err != nil {
			log.Error(err)
			return
		}

		image := Tools.EncodeImageFromBytes(f)
		vu := Tools.NewVuforiaManager()
		name := usr.Username + "_" + new_poster_info.PosTitle + "_" + time.Now().Format("Mon-02-Jan-2006-15-04")
		if targetId, ok, err := vu.AddItem(name, 32.0, image, true, Tools.EncodeBase64FromBytes(metaDataBytes)); err != nil {
			log.Error(err)
		} else if !ok {
			log.Error("sad")
		} else {
			new_poster_info.TargetId = targetId

			if err := Tools.UploadPosterFile(upload_url+thumbnail_filename, thumbnail); err != nil {
				log.Error(err)
				response.WriteHeader(http.StatusBadRequest)
				return
			}

			if err := Tools.UploadPosterFile(upload_url+model_filename, posterModel); err != nil {
				log.Error(err)
				response.WriteHeader(http.StatusBadRequest)
				return
			}
		}

		hostURL := "http://" + req.Host + "/posters"
		if _, err := Tools.PostByStructURL(new_poster_info, hostURL); err != nil {
			log.Error(err)
			response.WriteHeader(http.StatusBadRequest)
			return
		}

		pub := &Publish{UserId: usr.ID, TargetId: new_poster_info.TargetId}
		hostURL = "http://" + req.Host + "/posters/publish"
		if _, err := Tools.PostByStructURL(pub, hostURL); err != nil {
			log.Error(err)
			response.WriteHeader(http.StatusBadRequest)
			return
		}

		http.Redirect(response.ResponseWriter,
			request.Request,
			"/console/manage",
			301)
	}
}

// Manage page
func (c *ConsoleResource) Manage(request *restful.Request, response *restful.Response) {
	session, _ := Store.Get(request.Request, "ARPosterCookie")
	usr, ok := session.Values["userdata"].(*User)
	if !ok {
		log.Error("Load user data from session failed.")
	}

	if request.Request.Method == "GET" {
		p := newConsoleWithStaticFilePrefix(request)
		p.PageName = "manage"
		p.TotalPosters = 25
		p.TotalResources = 70

		pub := Publish{}
		if _, err := db.Engine.Table("publish").Where("userid=?", usr.ID).Get(&pub); err != nil {
			log.Error(1)
			log.Error(err)
			response.WriteHeader(http.StatusInternalServerError)
			return
		}

		if _, err := db.Engine.Table("poster").Where("targetid=?", pub.TargetId).Get(&p.Pos); err != nil {
			log.Error(2)
			log.Error(err)
			response.WriteHeader(http.StatusInternalServerError)
			return
		}

		t, err := template.ParseFiles("Models/Templates/layout.tmpl",
			"Models/Templates/manage.tmpl")
		if err != nil {
			log.Fatalf("Template gave: %s", err)
		}

		t.Execute(response.ResponseWriter, p)
	} else {
		req := request.Request
		req.ParseMultipartForm(1048576000)

		targetId := req.Form["targetid"][0]
		poster_info := &Poster{}
		if has, err := db.Engine.Table("poster").Where("targetid = ?", targetId).Get(poster_info); err != nil {
		} else if !has {
		} else {
			poster_info.PosTitle = req.Form["title"][0]
			poster_info.PosDate = req.Form["datetime"][0]
			poster_info.PosLocation = req.Form["location"][0]
			poster_info.PosMap = req.Form["mapurl"][0]
			poster_info.PosLink = req.Form["url"][0]
			poster_info.Relevantinfo = req.Form["rvntinfo"][0]
		}

		upload_url := "http://" + req.Host + "/files/upload/"
		file_url_prefix := "http://" + req.Host + "/files/"
		f, thumbnail, err := getFileFromRequest("thumbnail", req)
		if err != nil {
			log.Error(err)
			response.WriteHeader(http.StatusBadRequest)
			return
		} else if f != nil {
			thumbnail_filename := parseFileNameFromURL(poster_info.Thumbnail)
			os.Remove("/var/arposter/files/" + thumbnail_filename)
			// delete original file
			thumbnail_filename = replaceSuffix(thumbnail_filename, path.Ext(path.Base(thumbnail.Filename)))
			poster_info.Thumbnail = file_url_prefix + thumbnail_filename
			// store new file
			if err := Tools.UploadPosterFile(upload_url+thumbnail_filename, thumbnail); err != nil {
				log.Error(err)
				response.WriteHeader(http.StatusBadRequest)
				return
			}
		}

		f, posterModel, err := getFileFromRequest("armodel", req)
		if err != nil {
			log.Error(err)
			response.WriteHeader(http.StatusBadRequest)
			return
		} else if f != nil {
			model_filename := parseFileNameFromURL(poster_info.Model)
			os.Remove("/var/arposter/files/" + model_filename)
			// delete original file
			model_filename = replaceSuffix(model_filename, path.Ext(path.Base(posterModel.Filename)))
			poster_info.Model = file_url_prefix + model_filename
			// store new file
			if err := Tools.UploadPosterFile(upload_url+model_filename, posterModel); err != nil {
				log.Error(err)
				response.WriteHeader(http.StatusBadRequest)
				return
			}
		}

		hostURL := "http://" + req.Host + "/posters/update"
		if _, err := Tools.PostByStructURL(poster_info, hostURL); err != nil {
			log.Error(err)
			response.WriteHeader(http.StatusBadRequest)
			return
		}
	}
}

func replaceSuffix(filename, suffix string) string {
	suffix = strings.TrimPrefix(suffix, ".")
	s := strings.Split(filename, ".")
	s[len(s)-1] = suffix
	return strings.Join(s, ".")
}

func parseFileNameFromURL(urlString string) string {
	s := strings.Split(urlString, "/")
	return s[len(s)-1]
}

func sendLoginInfo(username, password, hostURL string) (User, error) {
	usr := &User{}
	if Tools.CheckEmail(username) {
		usr.Email = username
	} else {
		usr.Username = username
	}
	usr.Password = password

	res, err := Tools.PostByStructURL(usr, hostURL)
	if err != nil {
		return User{}, err
	}
	defer res.Body.Close()

	rbytes, err := ioutil.ReadAll(res.Body)
	if err != nil {
		return User{}, err
	}

	ur := &UsersResponse{}
	if err := json.Unmarshal(rbytes, ur); err != nil {
		return User{}, err
	}

	if ur.Success {
		return ur.User, nil
	} else {
		return User{}, errors.New("login failed: " + username + " " + password)
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

func newConsoleWithStaticFilePrefix(request *restful.Request) *Console {
	host := request.Request.Host

	session, _ := Store.Get(request.Request, "ARPosterCookie")
	usr, ok := session.Values["userdata"].(*User)
	if !ok {
		log.Error("Load user data from session failed.")
	}
	usr.Password = ""

	return &Console{StaticFilePrefix: "http://" + host + "/files/res", UserInfo: usr}
}

func getFileFromRequest(filename string, req *http.Request) ([]byte, *multipart.FileHeader, error) {
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
