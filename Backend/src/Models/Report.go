/*
 * @Author: Yutao Ge
 * @Date: 2019-09-07 12:52:00
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-09-07 14:13:15
 * @Description:
 */

package Models

import (
	"net/http"

	"github.com/emicklei/go-restful"
)

type Report struct {
	ReportId int    `json:"reportid" xorm:"reportid"`
	Detail   string `json:"detail" xorm:"detail"`

	UserId   int    `json:"userid" xorm:"userid"`
	TargetId string `json:"targetid" xorm:"targetid"`
}

type ReportResponse struct {
	Error   string `json:"error"`
	IsExist bool   `json:"isexist"`
	Success bool   `json:"success"`

	Reports []Report `json:"Report"`
}

type ReportResource struct {
}

func (r ReportResource) WebService() *restful.WebService {
	ws := new(restful.WebService)
	ws.
		Path("/reports").
		Consumes(restful.MIME_XML, restful.MIME_JSON).
		Produces(restful.MIME_JSON, restful.MIME_XML) // you can specify this per route as well

	ws.Route(ws.GET("/").To(r.GetAll)).
		Doc("get all reports")

	ws.Route(ws.GET("/{user-id}").To(r.GetByUserId)).
		Param(ws.PathParameter("user-id", "").DataType("integer").DefaultValue("0")).
		Doc("get reports by userid")

	ws.Route(ws.POST("").To(r.Post).
		Doc("save report")) // from the request

	return ws
}

func (r ReportResource) GetAll(request *restful.Request, response *restful.Response) {
	rp := []Report{}

	if err := db.Engine.Table("report").Find(&rp); err != nil {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, ReportResponse{Error: err.Error()})
	} else {
		response.WriteHeaderAndEntity(http.StatusOK, ReportResponse{Reports: rp})
	}
}

func (r ReportResource) GetByUserId(request *restful.Request, response *restful.Response) {
	rp := []Report{}
	uid := request.PathParameter("user-id")

	if err := db.Engine.Table("report").Where("userid = ?", uid).Find(&rp); err != nil {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, ReportResponse{Error: err.Error()})
	} else {
		response.WriteHeaderAndEntity(http.StatusOK, ReportResponse{Reports: rp})
	}
}

func (r ReportResource) Post(request *restful.Request, response *restful.Response) {
	rp := Report{}
	err := request.ReadEntity(&rp)
	if err == nil {
		db.WLock()
		defer db.WUnlock() //unlock when exit this method

		if _, err := db.Engine.Insert(&rp); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, ReportResponse{Error: err.Error()})
		} else {
			response.WriteHeaderAndEntity(http.StatusCreated, ReportResponse{Success: true})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, ReportResponse{Error: err.Error()})
	}
}
