/*
 * @Author: Yutao Ge
 * @Date: 2019-05-06 22:43:42
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-10-09 19:39:49
 * @Description:
 */
package Models

import (
	. "Tools"
	"encoding/gob"
	"encoding/json"
	"errors"
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
	FilePrefix       string
	PageName         string

	// Personal information
	UserInfo  *User
	AvatarUrl string

	// Dashboard
	TotalPosters   int
	TotalResources int

	// Manage
	Posters []Poster

	// Random string
	Rand string

	// Origin host with protocol
	Origin string

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

	ws.Route(ws.GET("/register").To(c.Register))
	ws.Route(ws.POST("/register").To(c.Register))

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
			storeErrMsgAndRedirect(request, response, err.Error(), "/console/login")
			return
		} else {
			session, err := Store.Get(request.Request, "ARPosterCookie")
			if err != nil {
				storeErrMsgAndRedirect(request, response, err.Error(), "/console/login")
				return
			}

			usr.Password = ""
			session.Values["authenticated"] = true
			session.Values["userdata"] = usr

			if err := session.Save(req, response.ResponseWriter); err != nil {
				storeErrMsgAndRedirect(request, response, err.Error(), "/console/login")
				return
			}

			http.Redirect(response.ResponseWriter,
				req,
				"/console/dashboard",
				301)
		}
	}
}

func (c *ConsoleResource) Register(request *restful.Request, response *restful.Response) {
	p := newConsoleWithStaticFilePrefix(request, response, "register")

	if request.Request.Method == "GET" {
		t, err := template.ParseFiles("Models/Templates/register.html")
		if err != nil {
			log.Fatalf("Template gave: %s", err)
		}
		t.Execute(response.ResponseWriter, p)
	} else {
		req := request.Request
		req.ParseForm()

		u := User{}
		if CheckEmail(req.Form["username"][0]) {
			u.Email = req.Form["username"][0]
		} else {
			u.Username = req.Form["username"][0]
		}

		rawPassword := req.Form["password"][0]
		confirm_rawPassword := req.Form["confirm-password"][0]
		if rawPassword != confirm_rawPassword {
			storeErrMsgAndRedirect(request, response, "Two passwords not match.", "/console/register")
			return
		}
		u.Password = EncodePassword(rawPassword)

		// check existence
		hostURL := "http://" + req.Host + "/users/checkExist"
		resp, err := PostByStructURL(u, hostURL)
		if err != nil {
			storeErrMsgAndRedirect(request, response, err.Error(), "/console/register")
			return
		}

		ur := UsersResponse{}
		if err := ExtractStructDataFromResponse(&ur, resp.Body); err != nil {
			storeErrMsgAndRedirect(request, response, err.Error(), "/console/register")
			return
		}
		if ur.Success {
			storeErrMsgAndRedirect(request, response, "Username/Email already exists.", "/console/register")
			return
		} else {
			hostURL = "http://" + req.Host + "/users"
			if _, err := PostByStructURL(u, hostURL); err != nil {
				storeErrMsgAndRedirect(request, response, err.Error(), "/console/register")
				return
			}
		}

		http.Redirect(response.ResponseWriter,
			request.Request,
			"/console/login",
			301)
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
			storeErrMsgAndRedirect(request, response, "Failed to clean user cache when log out.", "")
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

		f, physical, err := GetFileFromRequest("physicalposter", req)
		if err != nil {
			storeErrMsgAndRedirect(request, response, err.Error(), "/console/upload")
			return
		}

		_, thumbnail, err := GetFileFromRequest("thumbnail", req)
		if err != nil {
			storeErrMsgAndRedirect(request, response, err.Error(), "/console/upload")
			return
		}
		_, posterModel, err := GetFileFromRequest("armodel", req)
		if err != nil {
			storeErrMsgAndRedirect(request, response, err.Error(), "/console/upload")
			return
		}

		_type, err := strconv.Atoi(req.Form["type"][0])
		if err != nil {
			storeErrMsgAndRedirect(request, response, err.Error(), "/console/upload")
			return
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

		fileSuffix := path.Ext(path.Base(physical.Filename))
		physical_filename := "physical_" + p.UserInfo.Username + "_" + new_poster_info.PosTitle + "_" + time.Now().Format("Mon-02-Jan-2006-15-04") + fileSuffix
		new_poster_info.Physical = file_url_prefix + physical_filename

		fileSuffix = path.Ext(path.Base(thumbnail.Filename))
		thumbnail_filename := "thumbnail_" + p.UserInfo.Username + "_" + new_poster_info.PosTitle + "_" + time.Now().Format("Mon-02-Jan-2006-15-04") + fileSuffix
		new_poster_info.Thumbnail = file_url_prefix + thumbnail_filename

		fileSuffix = path.Ext(path.Base(posterModel.Filename))
		model_filename := "model_" + p.UserInfo.Username + "_" + new_poster_info.PosTitle + "_" + time.Now().Format("Mon-02-Jan-2006-15-04") + fileSuffix
		new_poster_info.Model = file_url_prefix + model_filename

		metaDataBytes, err := json.Marshal(new_poster_info)
		if err != nil {
			storeErrMsgAndRedirect(request, response, "Unable to pack poster info.", "/console/upload")
			return
		}

		image := EncodeImageFromBytes(f)
		vu := NewVuforiaManager()
		name := p.UserInfo.Username + "_" + new_poster_info.PosTitle + "_" + time.Now().Format("Mon-02-Jan-2006-15-04")
		if targetId, ok, err := vu.AddItem(name, 32.0, image, true, EncodeBase64FromBytes(metaDataBytes)); err != nil {
			storeErrMsgAndRedirect(request, response, "Unable to upload poster to remote server due to some error.", "/console/upload")
			return
		} else if !ok {
			storeErrMsgAndRedirect(request, response, "Unable to upload poster to remote server.", "/console/upload")
			return
		} else if targetId == "" {
			storeErrMsgAndRedirect(request, response, "Bad Image: please check image JPG or PNG(RGB).", "/console/upload")
			return
		} else {
			new_poster_info.TargetId = targetId

			if err := UploadPosterFile(upload_url+physical_filename, physical); err != nil {
				storeErrMsgAndRedirect(request, response, "Failed to upload physical poster.", "/console/upload")
				return
			}

			if err := UploadPosterFile(upload_url+thumbnail_filename, thumbnail); err != nil {
				storeErrMsgAndRedirect(request, response, "Failed to upload thumbnail.", "/console/upload")
				return
			}

			if err := UploadPosterFile(upload_url+model_filename, posterModel); err != nil {
				storeErrMsgAndRedirect(request, response, "Failed to upload poster AR model.", "/console/upload")
				return
			}
		}

		hostURL := "http://" + req.Host + "/posters"
		if _, err := PostByStructURL(new_poster_info, hostURL); err != nil {
			storeErrMsgAndRedirect(request, response, "Failed to store poster info.", "/console/upload")
			return
		}

		pub := &Publish{UserId: p.UserInfo.ID, TargetId: new_poster_info.TargetId}
		hostURL = "http://" + req.Host + "/posters/publish"
		if _, err := PostByStructURL(pub, hostURL); err != nil {
			storeErrMsgAndRedirect(request, response, "Failed to store publish info.", "/console/upload")
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
	p := newConsoleWithStaticFilePrefix(request, response, "manage")
	session := db.Engine.NewSession()
	defer session.Close()

	if request.Request.Method == "GET" {
		targetIds := []string{}

		if err := session.Table("publish").Where("userid=?", p.UserInfo.ID).Select("targetid").Find(&targetIds); err != nil {
			storeErrMsgAndRedirect(request, response, "Failed to retrieve publish info.", "/console/dashboard")
			return
		}

		tmp := []Poster{}
		if err := session.Table("poster").In("targetid", targetIds).Find(&tmp); err != nil {
			storeErrMsgAndRedirect(request, response, "Failed to retrieve poster(s).", "/console/dashboard")
			return
		}

		for i, tp := range tmp {
			q := []Question{}
			err := session.Sql(`SELECT * FROM question WHERE id IN (SELECT qid FROM qlist WHERE targetid = ?)`, tp.TargetId).Find(&q)
			if err != nil {
				storeErrMsgAndRedirect(request, response, "Failed to retrieve question(s).", "/console/dashboard")
				return
			}
			tmp[i].Questions = q
		}

		p.Posters = tmp

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
		if has, err := session.Table("poster").Where("targetid = ?", targetId).Get(poster_info); err != nil {
			storeErrMsgAndRedirect(request, response, "Failed to retrieve original poster info.", "/console/manage")
			return
		} else if !has {
			storeErrMsgAndRedirect(request, response, "Target poster not exist.", "/console/manage")
			return
		} else {
			_type, err := strconv.Atoi(req.Form["type"][0])
			if err != nil {
				storeErrMsgAndRedirect(request, response, err.Error(), "/console/manage")
				return
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

		f, physical, err := GetFileFromRequest("physical", req)
		if err != nil {
			if err == http.ErrMissingFile {
			} else {
				storeErrMsgAndRedirect(request, response, err.Error(), "/console/manage")
				return
			}
		} else if len(f) != 0 {
			physical_filename := ParseFileNameFromURL(poster_info.Physical)
			os.Remove("/var/arposter/files/" + physical_filename)
			// delete original file
			physical_filename = ReplaceFileSuffix(physical_filename, path.Ext(path.Base(physical.Filename)))
			poster_info.Physical = file_url_prefix + physical_filename
			// store new file
			if err := UploadPosterFile(upload_url+physical_filename, physical); err != nil {
				storeErrMsgAndRedirect(request, response, "Failed to update new physical poster.", "")
			}
		}

		f_thumbnail, thumbnail, err := GetFileFromRequest("thumbnail", req)
		if err != nil {
			if err == http.ErrMissingFile {
			} else {
				storeErrMsgAndRedirect(request, response, err.Error(), "/console/manage")
				return
			}
		} else if len(f_thumbnail) != 0 {
			thumbnail_filename := ParseFileNameFromURL(poster_info.Thumbnail)
			os.Remove("/var/arposter/files/" + thumbnail_filename)
			// delete original file
			thumbnail_filename = ReplaceFileSuffix(thumbnail_filename, path.Ext(path.Base(thumbnail.Filename)))
			poster_info.Thumbnail = file_url_prefix + thumbnail_filename
			// store new file
			if err := UploadPosterFile(upload_url+thumbnail_filename, thumbnail); err != nil {
				storeErrMsgAndRedirect(request, response, "Failed to upload new thumbnail.", "")
			}
		}

		f_model, posterModel, err := GetFileFromRequest("armodel", req)
		if err != nil {
			if err == http.ErrMissingFile {
			} else {
				storeErrMsgAndRedirect(request, response, err.Error(), "/console/manage")
				return
			}
		} else if len(f_model) != 0 {
			model_filename := ParseFileNameFromURL(poster_info.Model)
			os.Remove("/var/arposter/files/" + model_filename)
			// delete original file
			model_filename = ReplaceFileSuffix(model_filename, path.Ext(path.Base(posterModel.Filename)))
			poster_info.Model = file_url_prefix + model_filename
			// store new file
			if err := UploadPosterFile(upload_url+model_filename, posterModel); err != nil {
				storeErrMsgAndRedirect(request, response, "Failed to upload new poster AR model.", "")
			}
		}

		metaDataBytes, err := json.Marshal(poster_info)
		if err != nil {
			storeErrMsgAndRedirect(request, response, "Unable to pack poster info.", "/console/upload")
			return
		}

		if len(f) != 0 {
			image := EncodeImageFromBytes(f)
			vu := NewVuforiaManager()
			name := p.UserInfo.Username + "_" + poster_info.PosTitle + "_" + time.Now().Format("Mon-02-Jan-2006-15-04")
			if ok, err := vu.UpdateItem(poster_info.TargetId, name, 32.0, image, true, EncodeBase64FromBytes(metaDataBytes)); err != nil {
				storeErrMsgAndRedirect(request, response, "Failed to update vuforia metafile: "+err.Error(), "")
			} else if !ok {
				storeErrMsgAndRedirect(request, response, "Failed to update vuforia metafile.", "")
			}
		}

		hostURL := "http://" + req.Host + "/posters/update"
		if _, err := PostByStructURL(poster_info, hostURL); err != nil {
			storeErrMsgAndRedirect(request, response, "Failed to update poster info.", "/console/manage")
			return
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

func storeErrMsgAndRedirect(request *restful.Request, response *restful.Response, errMsg, redirectTarget string) {
	session, _ := Store.Get(request.Request, "ARPosterCookie")
	session.Values["errMsg"] = errMsg

	if err := session.Save(request.Request, response.ResponseWriter); err != nil {
		log.Error("Unable to save session: ", err.Error())
	}

	if redirectTarget != "" {
		http.Redirect(response.ResponseWriter,
			request.Request,
			redirectTarget,
			301)
	}
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

	ur := UsersResponse{}
	if err := ExtractStructDataFromResponse(&ur, res.Body); err != nil {
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
		Origin:           "http://" + host + "/console",
		FilePrefix:       "http://" + host + "/files",
		StaticFilePrefix: "http://" + host + "/files/res",
		PageName:         pageName,
		Rand:             RandString(8),
	}

	if pageName != "login" && pageName != "register" && pageName != "confirmed" {
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
		storeErrMsgAndRedirect(request, response, "", "")
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
