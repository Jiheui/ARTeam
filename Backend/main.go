/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-03-28 18:52:31
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-04-03 20:47:34
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
	restful.DefaultContainer.Add(u.WebService())

	log.Printf("start listening on localhost:8080")
	log.Fatal(http.ListenAndServe(":8080", nil))
}
