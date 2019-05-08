/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-05-06 22:43:42
* @Last Modified by:   Yutao GE
* @Last Modified time: 2019-05-08 00:20:10
 */
package Models

import (
	//"net/http"
	"text/template"

	log "github.com/Sirupsen/logrus"
	"github.com/emicklei/go-restful"
)

type Console struct {
	StaticFilePrefix string

	Username string
	Password string
}

type ConsoleResource struct {
}

func (c *ConsoleResource) WebService() *restful.WebService {
	ws := new(restful.WebService)
	ws.
		Path("/console").
		Consumes(restful.MIME_OCTET, restful.MIME_XML, restful.MIME_JSON).
		Produces(restful.MIME_OCTET, restful.MIME_XML, restful.MIME_JSON)

	ws.Route(ws.GET("/").To(c.Index))
	ws.Route(ws.GET("/login").To(c.Index))

	ws.Route(ws.GET("/main").To(c.Main))

	return ws
}

// Login page
func (c *ConsoleResource) Index(request *restful.Request, response *restful.Response) {
	p := NewConsoleWithStaticFilePrefix(request)

	t, err := template.ParseFiles("Models/Templates/login.html")
	if err != nil {
		log.Fatalf("Template gave: %s", err)
	}
	t.Execute(response.ResponseWriter, p)
}

// Main page
func (c *ConsoleResource) Main(request *restful.Request, response *restful.Response) {
	p := NewConsoleWithStaticFilePrefix(request)

	t, err := template.ParseFiles("Models/Templates/login.html")
	if err != nil {
		log.Fatalf("Template gave: %s", err)
	}
	t.Execute(response.ResponseWriter, p)
}

func NewConsoleWithStaticFilePrefix(request *restful.Request) *Console {
	host := request.Request.Host
	return &Console{StaticFilePrefix: "http://" + host + "/files/res"}
}
