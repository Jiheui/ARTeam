/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-04-11 15:42:37
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-08-11 22:04:45
*
* @Description: This file is created for store & get poster information
*
 */
package Models

import (
	"net/http"

	//log "github.com/Sirupsen/logrus"
	"github.com/emicklei/go-restful"
	//restfulspec "github.com/emicklei/go-restful-openapi"
)

type Poster struct {
	KeyGroup    string `json:"keygroup" xorm:"keygroup"`
	KeyId       string `json:"keyid" xorm:"keyid"`
	PosTitle    string `json:"postitle" xorm:"postitle"`
	PosDate     string `json:"posdate" xorm:"posdate"`
	PosLocation string `json:"poslocation" xorm:"poslocation"`
	PosMap      string `json:"posmap" xorm:"posmap"`
	PosLink     string `json:"poslink" xorm:"poslink"`
	ResUrl      string `json:"resurl" xorm:"resurl"`
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

	ws.Route(ws.GET("/{KeyGroup}/{KeyId}").To(p.Get)).
		Param(ws.PathParameter("KeyGroup", "identifier of the poster").DataType("string").DefaultValue("")).
		Param(ws.PathParameter("KeyId", "identifier of the poster").DataType("string").DefaultValue("")).
		Doc("get poster key info")

	ws.Route(ws.POST("").To(p.Post).
		Doc("store poster info")) // from the request

	return ws
}

func (p PosterResource) Get(request *restful.Request, response *restful.Response) {
	poster := Poster{}

	keyGroup := request.PathParameter("KeyGroup")
	keyId := request.PathParameter("KeyId")

	if has, err := db.Engine.Table("poster").Where("keygroup = ?", keyGroup).And("keyid = ?", keyId).Get(&poster); err != nil {
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
