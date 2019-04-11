/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-04-11 15:42:37
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-04-12 02:24:10
 */
package Models

import (
	"net/http"

	//log "github.com/Sirupsen/logrus"
	"github.com/emicklei/go-restful"
	//restfulspec "github.com/emicklei/go-restful-openapi"
)

type Poster struct {
	KeyGroup string `json:"keygroup" xorm:"keygroup"`
	KeyId    string `json:"keyid" xorm:"keyid"`
	Detail   string `json:"detail" xorm:"detail"`
	Url      string `json:"url" xorm:"url"`
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

	ws.Route(ws.GET("/").To(p.Get)).
		Doc("check login info")

	ws.Route(ws.PUT("").To(p.Put).
		Doc("store poster info")) // from the request

	return ws
}

func (p PosterResource) Get(request *restful.Request, response *restful.Response) {
	poster := Poster{}
	err := request.ReadEntity(&poster)
	if err == nil {
		if has, err := db.Engine.Table("poster").Where("keygroup = ?", poster.KeyGroup).And("keyid = ?", poster.KeyId).Get(&poster); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, PosterResponse{Error: err.Error()})
		} else {
			response.WriteEntity(PosterResponse{Poster: poster, IsExist: has})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, PosterResponse{Error: err.Error()})
	}
}

func (p *PosterResource) Put(request *restful.Request, response *restful.Response) {
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
