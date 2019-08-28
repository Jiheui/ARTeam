/*
 * @Author: Yutao Ge
 * @Date: 2019-03-31 19:00:29
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-08-29 00:08:24
 * @Description: This file is created for user related functions
 */
package Models

import (
	"net/http"
	"strconv"

	"Tools"

	//log "github.com/Sirupsen/logrus"
	log "github.com/Sirupsen/logrus"
	"github.com/emicklei/go-restful"
	restfulspec "github.com/emicklei/go-restful-openapi"
	"github.com/gorilla/sessions"
)

var (
	// key must be 16, 24 or 32 bytes long (AES-128, AES-192 or AES-256)
	// Todo: store key in local file
	key   = []byte("super-secret-key")
	Store = sessions.NewCookieStore(key)
)

type User struct {
	ID       int    `json:"id" description:"identifier of the user" xorm:"id pk autoincr"`
	Name     string `json:"name" description:"name of the user" xorm:"name"`
	DoB      string `json:"dob" description:"age of the user" xorm:"dob"`
	NickName string `json:"nickname" xorm:"nickname"`

	Email    string `json:"email" xorm:"email"`
	Username string `json:"username" description:"login info" xorm:"username"`
	Password string `json:"password" description:"login info" xorm:"password"`

	Facebook string `json:"facebook" description:"facebook credential" xorm:"facebook"`
	Google   string `json:"google" description:"google credential" xorm:"google"`

	Activated int `json:"activated" xorm:"activated"`
}

type UsersResponse struct {
	Error   string `json:"error"`
	IsExist bool   `json:"isexist"`
	Success bool   `json:"success"`

	User User `json:"user"`
}

type UserResource struct {
}

// WebService creates a new service that can handle REST requests for User resources.
func (u UserResource) WebService() *restful.WebService {
	ws := new(restful.WebService)
	ws.
		Path("/users").
		Consumes(restful.MIME_XML, restful.MIME_JSON).
		Produces(restful.MIME_JSON, restful.MIME_XML) // you can specify this per route as well

	tags := []string{"users"}

	ws.Route(ws.POST("/login").To(u.login)).
		Doc("check login info")

	ws.Route(ws.POST("/checkExist").To(u.checkExist)).
		Doc("check login key info existence")

	ws.Route(ws.GET("/{user-id}").To(u.findUser).
		Doc("get a user").
		Param(ws.PathParameter("user-id", "identifier of the user").DataType("integer").DefaultValue("0")). //make sure if no user-id pass in, it will return a not found
		Metadata(restfulspec.KeyOpenAPITags, tags).
		Writes(User{}). // on the response
		Returns(200, "OK", User{}).
		Returns(404, "Not Found", nil))

	ws.Route(ws.PATCH("").To(u.updateUser).
		Doc("update a user").
		Metadata(restfulspec.KeyOpenAPITags, tags).
		Reads(User{})) // from the request

	ws.Route(ws.GET("/confirm/{confirm-token:*}").To(u.confirm).
		Doc("confirm activation").
		Metadata(restfulspec.KeyOpenAPITags, tags).
		Reads(User{})) // from the request

	ws.Route(ws.POST("").To(u.createUser).
		Doc("create a user").
		Metadata(restfulspec.KeyOpenAPITags, tags).
		Reads(User{})) // from the request

	ws.Route(ws.POST("/reset/password").To(u.resetPassword).
		Doc("reset password").
		Reads(User{}))

	ws.Route(ws.GET("/temp/password/{email}").To(u.sendTempPassword)).
		Doc("reset password")

	return ws
}

// GET http://localhost:8080/users/1
// Query public info of a user with given id
func (u UserResource) findUser(request *restful.Request, response *restful.Response) {
	id, _ := strconv.Atoi(request.PathParameter("user-id"))
	usr := User{ID: id}

	if has, err := db.Engine.Get(&usr); err != nil {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
	} else if !has {
		response.WriteHeaderAndEntity(http.StatusNotFound, UsersResponse{Error: "User could not be found."})
	} else {
		response.WriteEntity(UsersResponse{IsExist: true, User: usr})
	}
}

// PATCH http://localhost:8080/users/
// Partially update a user
func (u *UserResource) updateUser(request *restful.Request, response *restful.Response) {
	usr := new(User)
	err := request.ReadEntity(&usr)
	if err == nil {
		db.WLock()
		defer db.WUnlock() //unlock when exit this method

		if _, err = db.Engine.Id(usr.ID).Update(usr); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
		} else {
			response.WriteEntity(UsersResponse{Success: true})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
	}
}

// PATCH http://localhost:8080/users/confirm
//
func (u *UserResource) confirm(request *restful.Request, response *restful.Response) {
	token := request.PathParameter("confirm-token") // currently using email as the token
	if err == nil {
		db.WLock()
		defer db.WUnlock() //unlock when exit this method

		if _, err := db.Engine.Exec("update user set activated=1 where email=\"" + token + "\";"); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
		} else {
			response.WriteEntity(UsersResponse{Success: true})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
	}
}

// PUT http://localhost:8080/users/
//
func (u *UserResource) createUser(request *restful.Request, response *restful.Response) {
	usr := User{}
	err := request.ReadEntity(&usr)
	if err == nil {
		db.WLock()
		defer db.WUnlock() //unlock when exit this method

		if _, err := db.Engine.Insert(&usr); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
		} else {
			// send confirm link
			if usr.Email != "" && usr.Facebook == "" && usr.Google == "" {
				usr.Activated = 0
				Tools.SendConfirmLink(usr.Email)
			}
			response.WriteHeaderAndEntity(http.StatusCreated, UsersResponse{Success: true})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
	}
}

// GET http://localhost:8080/users/login
//
func (u UserResource) login(request *restful.Request, response *restful.Response) {
	//var sess *sessions.Session
	usr := User{}
	err := request.ReadEntity(&usr)
	has := false

	if err == nil {
		if usr.Facebook != "" {
			has, err = db.Engine.Table("user").Where("facebook = ?", usr.Facebook).Get(&usr)
		} else if usr.Google != "" {
			has, err = db.Engine.Table("user").Where("google = ?", usr.Google).Get(&usr)
		} else if usr.Email != "" {
			has, err = db.Engine.Table("user").Where("email = ?", usr.Email).And("password = ?", usr.Password).Get(&usr)
		} else {
			has, err = db.Engine.Table("user").Where("username = ?", usr.Username).And("password = ?", usr.Password).Get(&usr)
		}
		if err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
		} else {
			response.WriteEntity(UsersResponse{Success: has, User: usr})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
	}
}

// GET http://localhost:8080/users/checkExists
//
func (u UserResource) checkExist(request *restful.Request, response *restful.Response) {
	usr := User{}
	err := request.ReadEntity(&usr)
	has := false
	tmp := User{}

	if err == nil {
		if usr.Facebook != "" {
			has, err = db.Engine.Table("user").Where("facebook = ?", usr.Facebook).Get(&tmp)
		} else if usr.Google != "" {
			has, err = db.Engine.Table("user").Where("google = ?", usr.Google).Get(&tmp)
		} else if usr.Email != "" {
			has, err = db.Engine.Table("user").Where("email = ?", usr.Email).Get(&tmp)
		} else {
			has, err = db.Engine.Table("user").Where("username = ?", usr.Username).Get(&tmp)
		}
		if err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
		} else {
			response.WriteEntity(UsersResponse{Success: has})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
	}
}

// Reset password
// send a random password to user
func (u UserResource) sendTempPassword(request *restful.Request, response *restful.Response) {
	email := request.PathParameter("email")
	usr := User{Email: email}
	new_password := Tools.RandString(8)

	if has, err := db.Engine.Table("user").Where("email=?", email).Get(&usr); err != nil {
		log.Error(err)
	} else if has {
		usr.Password = Tools.EncodePassword(new_password)
		if _, err = db.Engine.Id(usr.ID).Update(usr); err != nil {
			log.Error(err)
		}
	}
	Tools.SendPassword(usr.Email, new_password)
}

func (u UserResource) resetPassword(request *restful.Request, response *restful.Response) {
	usr := User{}
	err := request.ReadEntity(&usr)

	if usr.ID == 0 || usr.Password == "" {
		response.WriteHeaderAndEntity(http.StatusBadRequest, UsersResponse{Error: "Data provided not enough."})
		return
	}

	if err == nil {
		if _, err = db.Engine.Id(usr.ID).Update(usr); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
		} else {
			response.WriteEntity(UsersResponse{Success: true})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: "Cannot read data from request."})
	}
}
