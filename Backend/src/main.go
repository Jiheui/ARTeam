/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-03-28 18:52:31
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-08-15 20:57:29
 */
package main

import (
	"log"
	"net/http"

	model "./Models"

	"github.com/emicklei/go-restful"
)

func main() {
	u := model.UserResource{}
	p := model.PosterResource{}
	f := model.FavouritePosterResource{}
	fb := model.FeedbackResource{}
	file := model.FileResource{}
	c := model.ConsoleResource{}
	restful.DefaultContainer.Add(u.WebService())
	restful.DefaultContainer.Add(p.WebService())
	restful.DefaultContainer.Add(f.WebService())
	restful.DefaultContainer.Add(fb.WebService())
	restful.DefaultContainer.Add(file.WebService())
	restful.DefaultContainer.Add(c.WebService())

	log.Printf("start listening on localhost:8080")
	log.Fatal(http.ListenAndServe(":8080", nil))
}
