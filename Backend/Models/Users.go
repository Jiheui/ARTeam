/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-03-31 19:00:29
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-04-04 10:32:10
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

	ws.Route(ws.GET("/login").To(u.login)).
		Doc("check login info")

	ws.Route(ws.GET("/{user-id}").To(u.findUser).
		Doc("get a user").
		Param(ws.PathParameter("user-id", "identifier of the user").DataType("integer").DefaultValue("1")).
		Metadata(restfulspec.KeyOpenAPITags, tags).
		Writes(User{}). // on the response
		Returns(200, "OK", User{}).
		Returns(404, "Not Found", nil))

	ws.Route(ws.PATCH("").To(u.updateUser).
		Doc("update a user").
		Metadata(restfulspec.KeyOpenAPITags, tags).
		Reads(User{})) // from the request

	ws.Route(ws.PUT("").To(u.createUser).
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
		response.WriteError(http.StatusInternalServerError, err)
	} else if !has {
		response.WriteErrorString(http.StatusNotFound, "User could not be found.")
	} else {
		response.WriteEntity(usr)
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
			response.WriteError(http.StatusInternalServerError, err)
		} else {
			response.WriteEntity(usr)
		}
	} else {
		response.WriteError(http.StatusInternalServerError, err)
	}
}

// PUT http://localhost:8080/users/1
//
func (u *UserResource) createUser(request *restful.Request, response *restful.Response) {
	usr := User{}
	err := request.ReadEntity(&usr)
	if err == nil {
		db.WLock()
		defer db.WUnlock() //unlock when exit this method

		if affected, err := db.Engine.Insert(&usr); err != nil {
			response.WriteError(http.StatusInternalServerError, err)
		} else {
			response.WriteHeaderAndEntity(http.StatusCreated, affected)
		}
	} else {
		response.WriteError(http.StatusInternalServerError, err)
	}
}

// GET http://localhost:8080/users/login
//
func (u UserResource) login(request *restful.Request, response *restful.Response) {
	usr := User{}
	err := request.ReadEntity(&usr)
	if err == nil {
		if usr.Username == "" || usr.Password == "" {
			response.WriteError(http.StatusInternalServerError, ErrNotEnoughInfo)
			return
		}

		if has, err := db.Engine.Table("user").Where("username = ?", usr.Username).And("password = ?", usr.Password).Get(&usr); err != nil {
			response.WriteError(http.StatusInternalServerError, err)
		} else {
			response.WriteEntity(has)
		}
	} else {
		response.WriteError(http.StatusInternalServerError, err)
	}
}
