/*
 * @Author: Yutao Ge
 * @Date: 2019-05-06 22:43:42
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-09-11 04:16:58
 * @Description:
 */
package Models

import (
	. "Tools"
	"encoding/gob"
	"encoding/json"
	"errors"
	"io/ioutil"
	"net/http"
	"os"
	"path"
	"strconv"
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

	// Random string
	Rand string

	// Error message
	ErrMsg string
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

	ws.Route(ws.GET("/logout").To(c.Logout))

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
	p := newConsoleWithStaticFilePrefix(request, response, "login")

	if request.Request.Method == "GET" {
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
		password := EncodePassword(rawPassword)
		hostURL := "http://" + req.Host + "/users/login"

		if usr, err := sendLoginInfo(username, password, hostURL); err != nil {
			storeErrMsg(request, response, err.Error())
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/login",
				301)
		} else {
			session, err := Store.Get(request.Request, "ARPosterCookie")
			if err != nil {
				storeErrMsg(request, response, err.Error())
				http.Redirect(response.ResponseWriter,
					request.Request,
					"/console/login",
					301)
			}

			usr.Password = ""
			session.Values["authenticated"] = true
			session.Values["userdata"] = usr

			if err := session.Save(req, response.ResponseWriter); err != nil {
				storeErrMsg(request, response, err.Error())
				http.Redirect(response.ResponseWriter,
					request.Request,
					"/console/login",
					301)
			}

			http.Redirect(response.ResponseWriter,
				req,
				"/console/dashboard",
				301)
		}
	}
}

func (c *ConsoleResource) Logout(request *restful.Request, response *restful.Response) {
	if request.Request.Method == "GET" {
		session, err := Store.Get(request.Request, "ARPosterCookie")
		if err != nil {
			log.Error(err.Error())
			response.WriteHeader(http.StatusInternalServerError)
			return
		}

		session.Values["authenticated"] = false
		session.Values["userdata"] = nil

		if err := session.Save(request.Request, response.ResponseWriter); err != nil {
			storeErrMsg(request, response, "Failed to clean user cache when log out.")
		}

		http.Redirect(response.ResponseWriter,
			request.Request,
			"/console/login",
			301)
	}
}

// Dashboard page
func (c *ConsoleResource) Dashboard(request *restful.Request, response *restful.Response) {
	p := newConsoleWithStaticFilePrefix(request, response, "dashboard")

	t, err := template.ParseFiles("Models/Templates/layout.tmpl",
		"Models/Templates/dashboard.tmpl")
	if err != nil {
		log.Fatalf("Template gave: %s", err)
	}

	t.Execute(response.ResponseWriter, p)
}

// Upload page
func (c *ConsoleResource) Upload(request *restful.Request, response *restful.Response) {
	p := newConsoleWithStaticFilePrefix(request, response, "upload")

	if request.Request.Method == "GET" {
		t, err := template.ParseFiles("Models/Templates/layout.tmpl",
			"Models/Templates/upload.tmpl")
		if err != nil {
			log.Fatalf("Template gave: %s", err)
		}

		t.Execute(response.ResponseWriter, p)
	} else {
		req := request.Request
		req.ParseMultipartForm(1048576000)

		f, thumbnail, err := GetFileFromRequest("thumbnail", req)
		if err != nil {
			storeErrMsg(request, response, err.Error())
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/upload",
				301)
		}
		_, posterModel, err := GetFileFromRequest("armodel", req)
		if err != nil {
			storeErrMsg(request, response, err.Error())
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/upload",
				301)
		}

		_type, err := strconv.Atoi(req.Form["type"][0])
		if err != nil {
			storeErrMsg(request, response, err.Error())
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/upload",
				301)
		}

		new_poster_info := &Poster{
			PosTitle:     req.Form["title"][0],
			PosDate:      req.Form["datetime"][0],
			PosLocation:  req.Form["location"][0],
			PosMap:       req.Form["mapurl"][0],
			PosLink:      req.Form["url"][0],
			Relevantinfo: req.Form["rvntinfo"][0],
			Type:         _type,
		}
		upload_url := "http://" + req.Host + "/files/upload/"
		file_url_prefix := "http://" + req.Host + "/files/"
		fileSuffix := path.Ext(path.Base(thumbnail.Filename))

		thumbnail_filename := "thumbnail_" + p.UserInfo.Username + "_" + new_poster_info.PosTitle + "_" + time.Now().Format("Mon-02-Jan-2006-15-04") + fileSuffix
		new_poster_info.Thumbnail = file_url_prefix + thumbnail_filename

		fileSuffix = path.Ext(path.Base(posterModel.Filename))
		model_filename := "model_" + p.UserInfo.Username + "_" + new_poster_info.PosTitle + "_" + time.Now().Format("Mon-02-Jan-2006-15-04") + fileSuffix
		new_poster_info.Model = file_url_prefix + model_filename

		metaDataBytes, err := json.Marshal(new_poster_info)
		if err != nil {
			storeErrMsg(request, response, "Unable to pack poster info.")
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/upload",
				301)
		}

		image := EncodeImageFromBytes(f)
		vu := NewVuforiaManager()
		name := p.UserInfo.Username + "_" + new_poster_info.PosTitle + "_" + time.Now().Format("Mon-02-Jan-2006-15-04")
		if targetId, ok, err := vu.AddItem(name, 32.0, image, true, EncodeBase64FromBytes(metaDataBytes)); err != nil {
			storeErrMsg(request, response, "Unable to upload poster to remote server due to some error.")
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/upload",
				301)
		} else if !ok {
			storeErrMsg(request, response, "Unable to upload poster to remote server.")
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/upload",
				301)

		} else {
			new_poster_info.TargetId = targetId

			if err := UploadPosterFile(upload_url+thumbnail_filename, thumbnail); err != nil {
				storeErrMsg(request, response, "Failed to upload thumnail.")
				http.Redirect(response.ResponseWriter,
					request.Request,
					"/console/upload",
					301)
			}

			if err := UploadPosterFile(upload_url+model_filename, posterModel); err != nil {
				storeErrMsg(request, response, "Failed to upload poster AR model.")
				http.Redirect(response.ResponseWriter,
					request.Request,
					"/console/upload",
					301)
			}
		}

		hostURL := "http://" + req.Host + "/posters"
		if _, err := PostByStructURL(new_poster_info, hostURL); err != nil {
			storeErrMsg(request, response, "Failed to store poster info.")
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/upload",
				301)
		}

		pub := &Publish{UserId: p.UserInfo.ID, TargetId: new_poster_info.TargetId}
		hostURL = "http://" + req.Host + "/posters/publish"
		if _, err := PostByStructURL(pub, hostURL); err != nil {
			storeErrMsg(request, response, "Failed to store publish info.")
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/upload",
				301)
		}

		http.Redirect(response.ResponseWriter,
			request.Request,
			"/console/manage",
			301)
	}
}

// Manage page
func (c *ConsoleResource) Manage(request *restful.Request, response *restful.Response) {
	p := newConsoleWithStaticFilePrefix(request, response, "manage")

	if request.Request.Method == "GET" {
		pub := Publish{}
		if _, err := db.Engine.Table("publish").Where("userid=?", p.UserInfo.ID).Get(&pub); err != nil {
			storeErrMsg(request, response, "Failed to retrieve publish info.")
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/dashboard",
				301)
		}

		if _, err := db.Engine.Table("poster").Where("targetid=?", pub.TargetId).Get(&p.Pos); err != nil {
			storeErrMsg(request, response, "Failed to retrieve poster(s).")
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/dashboard",
				301)
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
			storeErrMsg(request, response, "Failed to retrieve original poster info.")
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/manage",
				301)
		} else if !has {
			storeErrMsg(request, response, "Target poster not exist.")
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/manage",
				301)
		} else {
			_type, err := strconv.Atoi(req.Form["type"][0])
			if err != nil {
				storeErrMsg(request, response, err.Error())
				http.Redirect(response.ResponseWriter,
					request.Request,
					"/console/manage",
					301)
			}
			poster_info.PosTitle = req.Form["title"][0]
			poster_info.PosDate = req.Form["datetime"][0]
			poster_info.PosLocation = req.Form["location"][0]
			poster_info.PosMap = req.Form["mapurl"][0]
			poster_info.PosLink = req.Form["url"][0]
			poster_info.Relevantinfo = req.Form["rvntinfo"][0]
			poster_info.Type = _type
		}

		upload_url := "http://" + req.Host + "/files/upload/"
		file_url_prefix := "http://" + req.Host + "/files/"
		_, thumbnail, err := GetFileFromRequest("thumbnail", req)
		if err != nil {
			if err == http.ErrMissingFile {
			} else {
				storeErrMsg(request, response, err.Error())
				http.Redirect(response.ResponseWriter,
					request.Request,
					"/console/manage",
					301)
			}
		} else {
			thumbnail_filename := ParseFileNameFromURL(poster_info.Thumbnail)
			os.Remove("/var/arposter/files/" + thumbnail_filename)
			// delete original file
			thumbnail_filename = ReplaceFileSuffix(thumbnail_filename, path.Ext(path.Base(thumbnail.Filename)))
			poster_info.Thumbnail = file_url_prefix + thumbnail_filename
			// store new file
			if err := UploadPosterFile(upload_url+thumbnail_filename, thumbnail); err != nil {
				storeErrMsg(request, response, "Failed to upload new thumbnail.")
				http.Redirect(response.ResponseWriter,
					request.Request,
					"/console/manage",
					301)
			}
		}

		_, posterModel, err := GetFileFromRequest("armodel", req)
		if err != nil {
			if err == http.ErrMissingFile {
			} else {
				storeErrMsg(request, response, err.Error())
				http.Redirect(response.ResponseWriter,
					request.Request,
					"/console/manage",
					301)
			}
		} else {
			model_filename := ParseFileNameFromURL(poster_info.Model)
			os.Remove("/var/arposter/files/" + model_filename)
			// delete original file
			model_filename = ReplaceFileSuffix(model_filename, path.Ext(path.Base(posterModel.Filename)))
			poster_info.Model = file_url_prefix + model_filename
			// store new file
			if err := UploadPosterFile(upload_url+model_filename, posterModel); err != nil {
				storeErrMsg(request, response, "Failed to upload new poster AR model.")
				http.Redirect(response.ResponseWriter,
					request.Request,
					"/console/manage",
					301)
			}
		}

		hostURL := "http://" + req.Host + "/posters/update"
		if _, err := PostByStructURL(poster_info, hostURL); err != nil {
			storeErrMsg(request, response, "Failed to update poster info.")
			http.Redirect(response.ResponseWriter,
				request.Request,
				"/console/manage",
				301)
		}

		http.Redirect(response.ResponseWriter,
			request.Request,
			"/console/manage",
			301)
	}
}

func basicAuthenticate(req *restful.Request, resp *restful.Response, chain *restful.FilterChain) {
	session, _ := Store.Get(req.Request, "ARPosterCookie")
	if auth, ok := session.Values["authenticated"].(bool); !ok || !auth {
		http.Redirect(resp.ResponseWriter, req.Request, "/console/login", http.StatusTemporaryRedirect)
		return
	}
	chain.ProcessFilter(req, resp)
}

func storeErrMsg(request *restful.Request, response *restful.Response, errMsg string) {
	session, _ := Store.Get(request.Request, "ARPosterCookie")
	session.Values["errmsg"] = errMsg

	log.Info(session.Save(request.Request, response.ResponseWriter))
}

func sendLoginInfo(username, password, hostURL string) (User, error) {
	usr := &User{}
	if CheckEmail(username) {
		usr.Email = username
	} else {
		usr.Username = username
	}
	usr.Password = password

	res, err := PostByStructURL(usr, hostURL)
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

func newConsoleWithStaticFilePrefix(request *restful.Request, response *restful.Response, pageName string) *Console {
	session, _ := Store.Get(request.Request, "ARPosterCookie")

	host := request.Request.Host
	c := &Console{
		StaticFilePrefix: "http://" + host + "/files/res",
		PageName:         pageName,
		Rand:             RandString(8),
	}

	if pageName != "login" {
		usr, ok := session.Values["userdata"].(*User)
		if !ok {
			c.ErrMsg = "Load user data from session failed."
		}
		usr.Password = ""
		c.UserInfo = usr
	}

	if errMsg, ok := session.Values["errMsg"].(string); !ok {
	} else {
		c.ErrMsg = errMsg
		session.Values["errMsg"] = nil
		session.Save(request.Request, response.ResponseWriter)
	}

	if pageName == "dashboard" {
		if cnt, err := db.Engine.Table("publish").Count(&Publish{UserId: c.UserInfo.ID}); err != nil {
			c.ErrMsg = err.Error()
		} else {
			c.TotalPosters = int(cnt)
			c.TotalResources = int(cnt) * 2
		}
	}

	return c
}
