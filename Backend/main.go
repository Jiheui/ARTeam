/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-03-28 18:52:31
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-04-12 21:45:50
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
	restful.DefaultContainer.Add(u.WebService())
	restful.DefaultContainer.Add(p.WebService())

	log.Printf("start listening on localhost:8080")
	log.Fatal(http.ListenAndServe(":8080", nil))
}