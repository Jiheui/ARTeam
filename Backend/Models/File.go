/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-04-26 00:22:52
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-04-26 23:14:02
 */

package Models

import (
	//"fmt"
	"io"
	"net/http"
	"os"
	"path"

	//log "github.com/Sirupsen/logrus"
	"github.com/emicklei/go-restful"
	//restfulspec "github.com/emicklei/go-restful-openapi"
)

type File struct {
	Id       int    `json:"id" xorm:"id"`
	FileName string `json:"filename" xorm:"filename"`
	KeyGroup string `json:"keygroup" xorm:"keygroup"`
	KeyId    string `json:"keyid" xorm:"keyid"`
}

type FileResponse struct {
	Error   string `json:"error"`
	IsExist bool   `json:"isexist"`
	Success bool   `json:"success"`

	Files []File `json:"files"`
}

type FileResource struct {
}

var rootDir = "/var/arposter/files/"

func (f FileResource) WebService() *restful.WebService {
	ws := new(restful.WebService)
	ws.
		Path("/files").
		Consumes(restful.MIME_OCTET, restful.MIME_XML, restful.MIME_JSON).
		Produces(restful.MIME_OCTET, restful.MIME_JSON, restful.MIME_XML) // you can specify this per route as well

	ws.Route(ws.GET("/{file-name:*}").To(f.GetFile)).
		Param(ws.PathParameter("file-name", "identifier of the poster").DataType("string").DefaultValue("")).
		Doc("get file resource")

	ws.Route(ws.GET("/{key-group}/{key-id}").To(f.GetFileListByPoster)).
		Param(ws.PathParameter("key-group", "identifier of the poster").DataType("string").DefaultValue("")).
		Param(ws.PathParameter("key-id", "identifier of the poster").DataType("string").DefaultValue("")).
		Doc("get file list")

	ws.Route(ws.POST("/{file-name:*}").To(f.Store).
		Doc("store file")) // from the request

	ws.Route(ws.PATCH("/update").To(f.Update).
		Doc("update file")) // from the request

	return ws
}

func (f FileResource) GetFile(request *restful.Request, response *restful.Response) {
	actual := path.Join(rootDir, request.PathParameter("file-name"))
	http.ServeFile(response.ResponseWriter, request.Request, actual)
}

func (f FileResource) GetFileListByPoster(request *restful.Request, response *restful.Response) {
}

func (f *FileResource) Store(request *restful.Request, response *restful.Response) {
	fileName := request.PathParameter("file-name") // currently using email as the token

	file, err := os.Create(rootDir + fileName)
	defer file.Close()

	if err != nil {
		response.WriteHeaderAndEntity(http.StatusInternalServerError, FileResponse{Error: err.Error()})
		panic(err)
	} else {
		_, err := io.Copy(file, request.Request.Body)
		if err != nil {
			response.WriteHeaderAndEntity(http.StatusInternalServerError, FileResponse{Error: err.Error()})
		}
	}
}

func (f *FileResource) Update(request *restful.Request, response *restful.Response) {
}
