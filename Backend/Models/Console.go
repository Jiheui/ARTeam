/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-05-06 22:43:42
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-05-07 00:12:10
 */
package Models

import (
	"log"
	//"net/http"
	"text/template"

	"github.com/emicklei/go-restful"
)

type Console struct {
	Text string
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

	return ws
}

func (c *ConsoleResource) Index(request *restful.Request, response *restful.Response) {
	p := &Console{"restful-html-template demo"}
	// you might want to cache compiled templates
	t, err := template.ParseFiles("Models/Templates/console.html")
	if err != nil {
		log.Fatalf("Template gave: %s", err)
	}
	t.Execute(response.ResponseWriter, p)
}
