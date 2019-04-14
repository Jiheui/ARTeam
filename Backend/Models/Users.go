/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-03-31 19:00:29
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-04-15 02:49:29
 */
package Models

import (
	"net/http"
	"strconv"

	//log "github.com/Sirupsen/logrus"
	"github.com/emicklei/go-restful"
	restfulspec "github.com/emicklei/go-restful-openapi"
)

type User struct {
	ID   int    `json:"id" description:"identifier of the user" xorm:"id pk autoincr"`
	Name string `json:"name" description:"name of the user" xorm:"name"`
	DoB  string `json:"dob" description:"age of the user" xorm:"dob"`

	Username string `json:"username" description:"login info" xorm:"username"`
	Password string `json:"password" description:"login info" xorm:"password"`

	Facebook string `json:"facebook" description:"facebook credential" xorm:"facebook"`
	Google   string `json:"google" description:"google credential" xorm:"google"`
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

	ws.Route(ws.POST("").To(u.createUser).
		Doc("create a user").
		Metadata(restfulspec.KeyOpenAPITags, tags).
		Reads(User{})) // from the request

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
			response.WriteHeaderAndEntity(http.StatusCreated, UsersResponse{Success: true})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
	}
}

// GET http://localhost:8080/users/login
//
func (u UserResource) login(request *restful.Request, response *restful.Response) {
	usr := User{}
	err := request.ReadEntity(&usr)
	if err == nil {
		if has, err := db.Engine.Table("user").Where("username = ?", usr.Username).And("password = ?", usr.Password).Get(&usr); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
		} else {
			response.WriteEntity(UsersResponse{Success: has})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, UsersResponse{Error: err.Error()})
	}
}
