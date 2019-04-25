/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-04-26 00:22:52
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-04-26 01:10:32
 */

package Models

import (
	"net/http"

	//log "github.com/Sirupsen/logrus"
	"github.com/emicklei/go-restful"
	//restfulspec "github.com/emicklei/go-restful-openapi"
)

type File struct {
}

type PosterResponse struct {
	Error   string `json:"error"`
	IsExist bool   `json:"isexist"`
	Success bool   `json:"success"`

	File File `json:"file"`
}

type FileResource struct {
}

func (f FileResource) WebService() *restful.WebService {
	ws := new(restful.WebService)
	ws.
		Path("/files").
		Consumes(restful.MIME_OCTET, restful.MIME_XML, restful.MIME_JSON).
		Produces(restful.MIME_OCTET, restful.MIME_JSON, restful.MIME_XML) // you can specify this per route as well

	ws.Route(ws.GET("/{file-id}").To(p.GetFile)).
		Param(ws.PathParameter("file-id", "identifier of the poster").DataType("string").DefaultValue("")).
		Doc("get file resource")

	ws.Route(ws.GET("/{user-id}").To(p.GetFileListByUserId)).
		Param(ws.PathParameter("user-id", "identifier of the poster").DataType("string").DefaultValue("")).
		Doc("get file list")

	ws.Route(ws.POST("").To(p.Store).
		Doc("store file")) // from the request

	ws.Route(ws.PATCH("/update").To(p.Update).
		Doc("update file")) // from the request

	return ws
}

func (f FileResource) GetFile(request *restful.Request, response *restful.Response) {
}

func (f FileResource) GetFileListByUserId(request *restful.Request, response *restful.Response) {
}

func (f *FileResource) Store(request *restful.Request, response *restful.Response) {
}

func (f *FileResource) Update(request *restful.Request, response *restful.Response) {
}
