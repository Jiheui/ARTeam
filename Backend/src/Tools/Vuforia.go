/*
 * @Author: Yutao Ge
 * @Date: 2019-08-11 21:42:07
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-08-22 03:00:57
 * @Description:  This file is created for backend to connect to Vuforia server via REST api.
 *
 * @About Vuforia: Vuforia Engine is a software platform for creating Augmented Reality apps.
 *				Developers can easily add advanced computer vision functionality to any application,
 *				allowing it to recognize images and objects, and interact with spaces in the real world.
 *				link: https://developer.vuforia.com/
 */
package Tools

import (
	"github.com/Fox-0390/Vuforia-Web-Services/vuforia"
)

type VuforiaConfig struct {
	ServerAccessKey string `json:"server_access_key"`
	ServerSecretKey string `json:"server_secret_key"`
	Host            string `json:"-"`
}

type VuforiaManger struct {
	client *vuforia.VuforiaClient
}

var vuConfig VuforiaConfig

var (
	VuforiaPostTargetRequestPath = "https://vws.vuforia.com/targets"
)

func init() {
	ParserConfig(&vuConfig)
	vuConfig.Host = "https://vws.vuforia.com"
}

func NewVuforiaManager() *VuforiaManger {
	v := &VuforiaManger{}
	v.client = &vuforia.VuforiaClient{}
	v.client.AccessKey = vuConfig.ServerAccessKey
	v.client.SecretKey = vuConfig.ServerSecretKey
	v.client.Host = vuConfig.Host
	return v
}

func (v *VuforiaManger) AddItem(name string, width float32, image string, activeFlag bool, metabase64 string) (targetId string, isSuccess bool, err error) {
	return v.client.AddItem(name, width, image, activeFlag, metabase64)
}

func (v *VuforiaManger) GetAccessKey() string {
	return v.client.AccessKey
}
