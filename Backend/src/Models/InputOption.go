/*
 * @Author: Yutao Ge
 * @Date: 2019-09-20 02:21:18
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-10-09 19:11:32
 * @Description:
 */
package Models

import (
	"encoding/json"
	"net/http"

	"github.com/emicklei/go-restful"
)

type InputType struct {
	Id       int    `json:"id" xorm:"id"`
	TypeName string `json:"typename" xorm:"typename"`
}

type QList struct {
	Id       int64  `json:"id" xorm:"id"`
	Qid      int64  `json:"qid" xorm:"qid"`
	TargetId string `json:"targetid" xorm:"targetid"`
}

type Question struct {
	Id  int64 `json:"id" xorm:"'id' pk autoincr"`
	Tid int   `json:"tid" xorm:"tid"` // Input type

	Name string `json:"name" xorm:"name"`

	// Options is only used for checkbox and radio button
	OptionString string `json:"option_string" xorm:"option_string"`

	// we combine the options into a string and store it in database
	Options []string `json:"options" xorm:"-"`
}

type Answer struct {
	Id       int64  `json:"id" xorm:"id"`
	Uid      int    `json:"uid" xorm:"uid"`           // User Id
	TargetId string `json:"targetid" xorm:"targetid"` // Question Id

	Content string `json:"content" xorm:"content"`
}

type AjaxItem struct {
	Qid          int64  `json:"qid"`
	Name         string `json:"name"`
	TargetId     string `json:"targetid"`
	Type         int    `json:"type"`
	OptionString string `json:"option_string"`

	Error string `json:"error" xorm:"-"`
}

type InputOptionResponse struct {
	Error   string `json:"error"`
	IsExist bool   `json:"isexist"`
	Success bool   `json:"success"`

	Questions []Question `json:"questions"`
}

type InputOptionResource struct {
}

func (i *InputOptionResource) WebService() *restful.WebService {
	ws := new(restful.WebService)
	ws.
		Path("/inputs").
		Consumes(restful.MIME_OCTET, restful.MIME_XML, restful.MIME_JSON).
		Produces(restful.MIME_OCTET, restful.MIME_XML, restful.MIME_JSON) // you can specify this per route as well

	ws.Route(ws.GET("/{target-id}").To(i.GetQuestion)).
		Doc("get questions by target id")

	ws.Route(ws.POST("/answer").To(i.GetQuestion))

	ws.Route(ws.POST("/save").To(i.StoreQuestion))
	ws.Route(ws.POST("/update").To(i.UpdateQuestion))
	ws.Route(ws.POST("/delete").To(i.DeleteQuestion))
	return ws
}

func (i *InputOptionResource) GetQuestion(request *restful.Request, response *restful.Response) {
	targetid := request.PathParameter("target-id")
	session := db.Engine.NewSession()
	defer session.Close()

	qs := []Question{}
	err := session.Sql(`SELECT * FROM question WHERE id IN (SELECT qid FROM qlist WHERE targetid = ?)`, targetid).Find(&qs)
	if err != nil {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, InputOptionResponse{Error: err.Error()})
	} else {
		response.WriteHeaderAndEntity(http.StatusOK, InputOptionResponse{Questions: qs})
	}
}

func (i *InputOptionResource) StoreQuestion(request *restful.Request, response *restful.Response) {
	w := response.ResponseWriter
	w.WriteHeader(http.StatusOK)

	a := AjaxItem{}
	b := []byte{}
	session := db.Engine.NewSession()
	defer session.Close()

	err := request.ReadEntity(&a)
	if err != nil {
		a.Error = "Failed to read ajax data: " + err.Error()
	} else {
		err = session.Begin()
		if err != nil {
			a.Error = "Internal Error: " + err.Error()
		} else {
			q := Question{Name: a.Name, Tid: a.Type, OptionString: a.OptionString}

			if affected, err := session.Table("question").Insert(&q); err != nil {
				a.Error = "Internal Error: " + err.Error()
				session.Rollback()
			} else if affected == 0 {
				a.Error = "Internal Error: Insert failed."
				session.Rollback()
			} else {
				a.Qid = q.Id
				ql := QList{TargetId: a.TargetId, Qid: q.Id}
				if affected, err := session.Table("qlist").Insert(&ql); err != nil {
					a.Error = "Internal Error: " + err.Error()
					session.Rollback()
				} else if affected == 0 {
					a.Error = "Internal Error: Insert failed."
					session.Rollback()
				}
			}
		}
	}

	err = session.Commit()
	if err != nil {
		a.Error = "Internal Error: Insert failed."
	}

	b, _ = json.Marshal(&a)
	w.Write(b)
}

func (i *InputOptionResource) UpdateQuestion(request *restful.Request, response *restful.Response) {
	w := response.ResponseWriter
	w.WriteHeader(http.StatusOK)

	a := AjaxItem{}
	b := []byte{}
	session := db.Engine.NewSession()
	defer session.Close()

	err := request.ReadEntity(&a)
	if err != nil {
		a.Error = "Failed to read ajax data: " + err.Error()
	} else {
		q := Question{Id: a.Qid, Name: a.Name, Tid: a.Type, OptionString: a.OptionString}
		if affected, err := session.Table("question").ID(q.Id).Update(&q); err != nil {
			a.Error = "Internal Error: " + err.Error()
		} else if affected == 0 {
			a.Error = "Nothing updated."
		}
	}

	b, _ = json.Marshal(&a)
	w.Write(b)
}

func (i *InputOptionResource) DeleteQuestion(request *restful.Request, response *restful.Response) {
	w := response.ResponseWriter
	w.WriteHeader(http.StatusOK)

	a := AjaxItem{}
	b := []byte{}
	session := db.Engine.NewSession()
	defer session.Close()

	err := request.ReadEntity(&a)
	if err != nil {
		a.Error = "Failed to read ajax data: " + err.Error()
	} else {
		err = session.Begin()
		if err != nil {
			a.Error = "Internal Error: " + err.Error()
		} else {
			_, err = session.Table("qlist").Where("qid = ?", a.Qid).Delete(&QList{Qid: a.Qid})
			if err != nil {
				a.Error = "Internal Error: " + err.Error()
				session.Rollback()
			} else {
				_, err = session.Table("question").Where("id = ?", a.Qid).Delete(&Question{Id: a.Qid})
				if err != nil {
					a.Error = "Internal Error: " + err.Error()
					session.Rollback()
				}
			}
		}
	}

	err = session.Commit()
	if err != nil {
		a.Error = "Internal Error: " + err.Error()
	}

	b, _ = json.Marshal(&a)
	w.Write(b)
}

func (i *InputOptionResource) StoreAnswer(request *restful.Request, response *restful.Response) {
	a := Answer{}
	err := request.ReadEntity(&a)
	session := db.Engine.NewSession()
	defer session.Close()

	if err != nil {
		response.WriteHeaderAndEntity(http.StatusBadRequest, InputOptionResponse{Error: err.Error()})
	} else {
		if affected, err := session.Table("answer").Insert(&a); err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, InputOptionResponse{Error: err.Error()})
		} else if affected == 0 {
			response.WriteHeaderAndEntity(http.StatusNotModified, InputOptionResponse{Error: "Unable to store answers: no rows affected."})
		} else {
			response.WriteHeader(http.StatusOK)
		}
	}
}
