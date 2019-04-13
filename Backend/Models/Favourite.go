/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-04-13 00:06:38
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-04-13 02:03:02
 */
package Models

import (
//"net/http"

//log "github.com/Sirupsen/logrus"
//"github.com/emicklei/go-restful"
//restfulspec "github.com/emicklei/go-restful-openapi"
)

type Favourite struct {
	Id     int `json:"id" xorm:"id"`
	UserId int `json:"userid" xorm:"userid"`
}
