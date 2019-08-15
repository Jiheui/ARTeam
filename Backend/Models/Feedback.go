/*
 * @Author: Yutao Ge
 * @Date: 2019-04-17 19:05:19
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-08-16 01:08:06
 * @Description: This file is created for feedback function
 */
package Models

import (
	"net/http"
	"strconv"
	"time"

	"github.com/emicklei/go-restful"
)

type Feedback struct {
	Id      int    `json:"id" xorm:"id"`
	Content string `json:"content" xorm:"content"`
	Email   string `json:"email" xorm:"email"`
	Time    string `json:"time" xorm:"time"`

	UserId   int    `json:"userid" xorm:"userid"`
	Username string `json:"username" xorm:"username"`

	IsDeleted int `json:"-" xorm:"isdeleted"`
}

type FeedbackResponse struct {
	Error   string `json:"error"`
	IsExist bool   `json:"isexist"`
	Success bool   `json:"success"`

	Feedbacks []Feedback `json:"Feedback"`
}

type FeedbackResource struct {
}

func (f FeedbackResource) WebService() *restful.WebService {
	ws := new(restful.WebService)
	ws.
		Path("/feedbacks").
		Consumes(restful.MIME_XML, restful.MIME_JSON).
		Produces(restful.MIME_JSON, restful.MIME_XML) // you can specify this per route as well

	ws.Route(ws.GET("/{user-id}").To(f.Get)).
		Param(ws.PathParameter("user-id", "").DataType("integer").DefaultValue("0")).
		Doc("get feedbacks")

	ws.Route(ws.GET("/feedback/{feedback-id}").To(f.GetOne)).
		Param(ws.PathParameter("feedback-id", "").DataType("integer").DefaultValue("0")).
		Doc("get one feedback")

	ws.Route(ws.POST("").To(f.Post).
		Doc("save feedback")) // from the request

	ws.Route(ws.DELETE("/{feedback-id}").To(f.Get)).
		Param(ws.PathParameter("feedback-id", "identifier of the feedback").DataType("integer").DefaultValue("0"))
	return ws
}

func (f FeedbackResource) Get(request *restful.Request, response *restful.Response) {
	fb := []Feedback{}
	uid := request.PathParameter("user-id")

	if err := db.Engine.Table("feedback").Where("userid = ?", uid).Desc("time").Find(&fb); err != nil {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, FeedbackResponse{Error: err.Error()})
	} else {
		response.WriteHeaderAndEntity(http.StatusOK, FeedbackResponse{Feedbacks: fb})
	}
}

func (f FeedbackResource) GetOne(request *restful.Request, response *restful.Response) {
	fb := []Feedback{}
	id := request.PathParameter("fedback-id")

	if err := db.Engine.Table("feedback").Where("id = ?", id).Desc("time").Find(&fb); err != nil {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, FeedbackResponse{Error: err.Error()})
	} else {
		response.WriteHeaderAndEntity(http.StatusOK, FeedbackResponse{Feedbacks: fb})
	}
}

func (f *FeedbackResource) Post(request *restful.Request, response *restful.Response) {
	fb := Feedback{}
	err := request.ReadEntity(&fb)
	if err == nil {
		db.WLock()
		defer db.WUnlock() //unlock when exit this method

		if _, err := db.Engine.Insert(&fb); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, FeedbackResponse{Error: err.Error()})
		} else {
			response.WriteHeaderAndEntity(http.StatusCreated, FeedbackResponse{Success: true})
		}
	} else {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, FeedbackResponse{Error: err.Error()})
	}
}

func (f FeedbackResource) Delete(request *restful.Request, response *restful.Response) {
	fid, _ := strconv.Atoi(request.PathParameter("feedback-id"))

	fs := Feedback{Id: fid}

	if _, err := db.Engine.Where("id = ?", fid).Delete(fs); err != nil {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, FeedbackResponse{Error: err.Error()})
	} else {
		response.WriteHeaderAndEntity(http.StatusOK, FeedbackResponse{Success: true})
	}
}

func (f *Feedback) BeforeInsert() {
	f.Time = time.Now().Format("2006-01-02 15:04:05")
}
