/*
 * @Author: Yutao Ge
 * @Date: 2019-04-13 00:06:38
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-08-22 03:17:15
 * @Description: This file is created "favourite poster" function
 */
package Models

import (
	"net/http"
	"strconv"
	"time"

	//log "github.com/Sirupsen/logrus"
	"github.com/emicklei/go-restful"
	//restfulspec "github.com/emicklei/go-restful-openapi"
)

type Favourite struct {
	UserId   int    `json:"userid" xorm:"userid"`
	TargetId string `json:"targetid" xorm:"targetid"`
	Time     string `json:"time" xorm:"time"`
}

type FavouriteResponse struct {
	Error   string `json:"error"`
	IsExist bool   `json:"isexist"`
	Success bool   `json:"success"`

	Favourites []Favourite `json:"favourites"`
}

type FavouritePosterResource struct {
}

func (f FavouritePosterResource) WebService() *restful.WebService {
	ws := new(restful.WebService)
	ws.
		Path("/favourites").
		Consumes(restful.MIME_XML, restful.MIME_JSON).
		Produces(restful.MIME_JSON, restful.MIME_XML) // you can specify this per route as well

	ws.Route(ws.GET("/{user-id}").To(f.Get)).
		Param(ws.PathParameter("user-id", "identifier of the user").DataType("integer").DefaultValue("0")).
		Doc("get favourite posters by user")

	ws.Route(ws.POST("").To(f.Post).
		Doc("save favourite poster info")) // from the request

	ws.Route(ws.DELETE("/{user-id}/{target-id}").To(f.Delete)).
		Param(ws.PathParameter("user-id", "identifier of the user").DataType("integer").DefaultValue("0")).
		Param(ws.PathParameter("target-id", "identifier of the poster").DataType("string").DefaultValue(""))
	return ws
}

func (f FavouritePosterResource) Get(request *restful.Request, response *restful.Response) {
	fs := []Favourite{}

	uid := request.PathParameter("user-id")

	if err := db.Engine.Table("favourite").Where("userid = ?", uid).Desc("time").Find(&fs); err != nil {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, FavouriteResponse{Error: err.Error()})
	} else {
		response.WriteHeaderAndEntity(http.StatusOK, FavouriteResponse{Favourites: fs})
	}
}

func (p *FavouritePosterResource) Post(request *restful.Request, response *restful.Response) {
	f := Favourite{}
	err := request.ReadEntity(&f)
	if err == nil {
		db.WLock()
		defer db.WUnlock() //unlock when exit this method

		if _, err := db.Engine.Insert(&f); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, FavouriteResponse{Error: err.Error()})
		} else {
			response.WriteHeaderAndEntity(http.StatusCreated, FavouriteResponse{Success: true})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, FavouriteResponse{Error: err.Error()})
	}
}

func (f FavouritePosterResource) Delete(request *restful.Request, response *restful.Response) {
	uid, _ := strconv.Atoi(request.PathParameter("user-id"))
	targetId := request.PathParameter("target-id")

	fs := Favourite{UserId: uid, TargetId: targetId}

	if _, err := db.Engine.Table("favourite").Delete(fs); err != nil {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, FavouriteResponse{Error: err.Error()})
	} else {
		response.WriteHeaderAndEntity(http.StatusOK, FavouriteResponse{Success: true})
	}
}

func (f *Favourite) BeforeInsert() {
	f.Time = time.Now().Format("2006-01-02 15:04:05")
}
