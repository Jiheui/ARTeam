/*
 * @Author: Yutao Ge
 * @Date: 2019-04-11 15:42:37
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-09-29 00:11:00
 * @Description: This file is created for store & get poster information
 */
package Models

import (
	"net/http"

	//log "github.com/Sirupsen/logrus"
	"github.com/emicklei/go-restful"
	//restfulspec "github.com/emicklei/go-restful-openapi"
)

type Publish struct {
	UserId   int    `json:"userid" xorm:"userid"`
	TargetId string `json:"targetid" xorm:"targetid"`
}

type Poster struct {
	Id           int64  `json:"-" xorm:"id"`
	TargetId     string `json:"targetid" xorm:"targetid"`
	PosTitle     string `json:"postitle" xorm:"postitle"`
	PosDate      string `json:"posdate" xorm:"posdate"`
	PosLocation  string `json:"poslocation" xorm:"poslocation"`
	PosMap       string `json:"posmap" xorm:"posmap"`
	PosLink      string `json:"poslink" xorm:"poslink"`
	ResUrl       string `json:"resurl" xorm:"resurl"`
	Model        string `json:"model" xorm:"model"`
	Thumbnail    string `json:"thumbnail" xorm:"thumbnail"`
	Relevantinfo string `json:"relevantinfo" xorm:"relevantinfo"`
	Type         int    `json:"type" xorm:"type"`

	Questions []Question `json:"questions" xorm:"-"`
}

type PosterResponse struct {
	Error   string `json:"error"`
	IsExist bool   `json:"isexist"`
	Success bool   `json:"success"`

	Poster Poster `json:"poster"`
}

type PosterResource struct {
}

func (p PosterResource) WebService() *restful.WebService {
	ws := new(restful.WebService)
	ws.
		Path("/posters").
		Consumes(restful.MIME_XML, restful.MIME_JSON).
		Produces(restful.MIME_JSON, restful.MIME_XML) // you can specify this per route as well

	ws.Route(ws.GET("/{target-id}").To(p.Get)).
		Param(ws.PathParameter("target-id", "identifier of the poster").DataType("string").DefaultValue("")).
		Doc("get poster key info")

	ws.Route(ws.POST("").To(p.Post).
		Doc("store poster info")) // from the request

	ws.Route(ws.POST("/publish").To(p.StorePublish).
		Reads(Publish{}))

	ws.Route(ws.POST("/update").To(p.Update).
		Reads(Poster{}))

	return ws
}

func (p PosterResource) Get(request *restful.Request, response *restful.Response) {
	poster := Poster{}
	targetId := request.PathParameter("target-id")

	if has, err := db.Engine.Table("poster").Where("targetid = ?", targetId).Get(&poster); err != nil {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, PosterResponse{Error: err.Error()})
	} else {
		response.WriteHeaderAndEntity(http.StatusOK, PosterResponse{Poster: poster, IsExist: has})
	}
}

func (p *PosterResource) Post(request *restful.Request, response *restful.Response) {
	poster := Poster{}
	err := request.ReadEntity(&poster)
	if err == nil {
		db.WLock()
		defer db.WUnlock() //unlock when exit this method

		if _, err := db.Engine.Insert(&poster); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, PosterResponse{Error: err.Error()})
		} else {
			response.WriteHeaderAndEntity(http.StatusCreated, PosterResponse{Success: true})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, PosterResponse{Error: err.Error()})
	}
}

func (p *PosterResource) StorePublish(request *restful.Request, response *restful.Response) {
	pub := Publish{}
	err := request.ReadEntity(&pub)
	if err == nil {
		if _, err := db.Engine.Insert(&pub); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, PosterResponse{Error: err.Error()})
		} else {
			response.WriteHeaderAndEntity(http.StatusCreated, PosterResponse{Success: true})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, PosterResponse{Error: err.Error()})
	}
}

func (p *PosterResource) Update(request *restful.Request, response *restful.Response) {
	poster := Poster{}
	err := request.ReadEntity(&poster)
	if err == nil {
		if _, err := db.Engine.Where("targetid=?", poster.TargetId).Update(&poster); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, PosterResponse{Error: err.Error()})
		} else {
			response.WriteHeaderAndEntity(http.StatusCreated, PosterResponse{Success: true})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, PosterResponse{Error: err.Error()})
	}
}
