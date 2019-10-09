/*
 * @Author: Yutao Ge
 * @Date: 2019-09-08 21:22:34
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-10-09 14:02:52
 * @Description:
 */
package Models

import (
	"net/http"

	"github.com/emicklei/go-restful"
)

type Option struct {
	OptionId int64  `json:"optionid" xorm:"optionid"`
	TargetId string `json:"targetid" xorm:"targetid"`

	Key   string `json:"key" xorm:"key"`
	Value int    `json:"value" xorm:"value"`
}

type OptionResponse struct {
	Error   string `json:"error"`
	IsExist bool   `json:"isexist"`
	Success bool   `json:"success"`

	Options []Option `json:"options"`
}

type OptionResource struct {
}

func (o OptionResource) WebService() *restful.WebService {
	ws := new(restful.WebService)
	ws.
		Path("/options").
		Consumes(restful.MIME_XML, restful.MIME_JSON).
		Produces(restful.MIME_JSON, restful.MIME_XML) // you can specify this per route as well

	ws.Route(ws.GET("/{target-id}").To(o.GetAll)).
		Doc("get Options by target id")

	ws.Route(ws.POST("/incr").To(o.Incr).
		Doc("increase option value by 1"))

	ws.Route(ws.POST("").To(o.AddNew).
		Doc("save option"))

	return ws
}

func (o OptionResource) GetAll(request *restful.Request, response *restful.Response) {
	op := []Option{}
	targetid := request.PathParameter("target-id")

	if err := db.Engine.Table("option").Where("targetid=?", targetid).Find(&op); err != nil {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, OptionResponse{Error: err.Error()})
	} else {
		response.WriteHeaderAndEntity(http.StatusOK, OptionResponse{Options: op})
	}
}

func (o OptionResource) Incr(request *restful.Request, response *restful.Response) {
	op := Option{}
	err := request.ReadEntity(&op)

	if err == nil {
		if has, err := db.Engine.Table("option").Where("targetid=?", op.TargetId).And("key = ?", op.Key).Get(&op); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, OptionResponse{Error: err.Error()})
		} else if !has {
			op.Value = 1
			if _, err := db.Engine.Insert(&op); err != nil {
				response.WriteHeaderAndEntity(http.StatusInternalServerError, OptionResponse{Error: err.Error()})
			} else {
				response.WriteHeaderAndEntity(http.StatusCreated, OptionResponse{Success: true})
			}
		} else {
			if _, err := db.Engine.Table("option").Where("optionid=?", op.OptionId).Incr("value").Update(&op); err != nil {
				response.WriteHeaderAndEntity(http.StatusInternalServerError, OptionResponse{Error: err.Error()})
			} else {
				response.WriteHeaderAndEntity(http.StatusCreated, OptionResponse{Success: true})
			}
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, OptionResponse{Error: err.Error()})
	}
}

func (o OptionResource) AddNew(request *restful.Request, response *restful.Response) {
	op := Option{}
	err := request.ReadEntity(&op)
	if err == nil {
		db.WLock()
		defer db.WUnlock() //unlock when exit this method

		if _, err := db.Engine.Insert(&op); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, OptionResponse{Error: err.Error()})
		} else {
			response.WriteHeaderAndEntity(http.StatusCreated, OptionResponse{Success: true})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, OptionResponse{Error: err.Error()})
	}
}
