/*
 * @Author: Yutao Ge
 * @Date: 2019-08-11 21:42:07
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-08-16 22:47:48
 * @Description:  This file is created for backend to connect to Vuforia server via REST api.
 *
 * @About Vuforia: Vuforia Engine is a software platform for creating Augmented Reality apps.
 *				Developers can easily add advanced computer vision functionality to any application,
 *				allowing it to recognize images and objects, and interact with spaces in the real world.
 *				link: https://developer.vuforia.com/
 */
package Tools

import (
	"net/http"
	"time"
)

type VuforiaConfig struct {
	ServerAccessKey string `json:"server_access_key"`
	ServerSecretKey string `json:"server_secret_key"`
}

type VuforiaManger struct {
	config    VuforiaConfig
	Signature string

	TargetId string
	Url      string
}

var VuMaster VuforiaManger

var (
	VuforiaPostTargetRequestPath = "https://vws.vuforia.com/targets"
)

func init() {
	ParserConfig(&VuMaster.config)
	VuMaster.Url = "https://vws.vuforia.com"
}

// Post a new vuforia target
// Image should be encoded in Base64
func (v *VuforiaManger) PostNewTarget(req *http.Request, name, image, metaData string) {
	j := "{\"name\":\"" + name + "\", \"width\":1, \"image\":\"" + image + "\", \"active_flag\":1, " + " \"application_metadata\":\"" + metaData + "\"}"
	contentMD5 := MD5ByString(j)
	v.setHeaders(req, http.MethodPost, contentMD5, "application/json", VuforiaPostTargetRequestPath)
}

// set request header
func (v *VuforiaManger) setHeaders(req *http.Request, httpVerb, contentMD5, contentType, requestPath string) error {
	location, err := time.LoadLocation("GMT")
	if err != nil {
		return err
	}
	date := time.Now().In(location).Format("Mon, 02 Jan 2006 15:04:05 GMT")

	v.signatureBuilder(httpVerb, contentMD5, contentType, date, requestPath)
	req.Header.Set("Date", date)
	req.Header.Set("Content-Type", contentType)
	req.Header.Set("Authorization", "VWS "+v.config.ServerAccessKey+":"+v.Signature)
	return nil
}

// Vuforia signature builder
// Used when set authorization header field
func (v *VuforiaManger) signatureBuilder(HttpVerb, ContentMD5, ContentType, Date, RequestPath string) {
	stringToSign := HttpVerb + "\n" +
		ContentMD5 + "\n" +
		ContentType + "\n" +
		Date + "\n" +
		RequestPath

	v.Signature = EncodeBase64(HMAC_SHA1(v.config.ServerSecretKey, stringToSign))
}

// Create a new Vuforia manager
func (v *VuforiaManger) Copy() VuforiaManger {
	tmp := VuforiaManger{
		config: VuforiaConfig{
			ServerAccessKey: v.config.ServerAccessKey,
			ServerSecretKey: v.config.ServerSecretKey,
		},
		Url: v.Url,
	}
	return tmp
}

func NewVuforiaManager() VuforiaManger {
	return VuMaster.Copy()
}
