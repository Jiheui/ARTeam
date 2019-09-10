/*
 * @Author: Yutao Ge
 * @Date: 2019-08-11 21:50:37
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-09-11 00:21:28
 * @Description: This file contains serveral some tools different usage
 */
package Tools

import (
	"bytes"
	"crypto/hmac"
	"crypto/md5"
	"crypto/sha1"
	"encoding/base64"
	"encoding/json"
	"fmt"
	"io"
	"io/ioutil"
	"math/rand"
	"mime/multipart"
	"net/http"
	"net/url"
	"os"
	"strings"
	"time"

	log "github.com/Sirupsen/logrus"
)

func ReplaceFileSuffix(filename, suffix string) string {
	suffix = strings.TrimPrefix(suffix, ".")
	s := strings.Split(filename, ".")
	s[len(s)-1] = suffix
	return strings.Join(s, ".")
}

func ParseFileNameFromURL(urlString string) string {
	s := strings.Split(urlString, "/")
	return s[len(s)-1]
}

func UploadPosterFile(url string, fileHeader *multipart.FileHeader) error {
	file, err := fileHeader.Open()
	if err != nil {
		return err
	}

	req, err := http.NewRequest("POST", url, file)
	if err != nil {
		return err
	}
	req.Header.Set("Content-Type", "application/octet-stream")

	client := &http.Client{}
	res, err := client.Do(req)
	if err != nil {
		return err
	}
	defer res.Body.Close()
	return err
}

func PostByStructURL(_struct interface{}, hostURL string) (*http.Response, error) {
	b, err := json.Marshal(_struct)
	if err != nil {
		return nil, err
	}
	body := &bytes.Buffer{}
	body.WriteString(string(b))

	req, err := http.NewRequest("POST", hostURL, body)
	if err != nil {
		return nil, err
	}

	req.Header.Set("Content-Type", "application/json")

	client := &http.Client{}
	res, err := client.Do(req)
	if err != nil {
		return nil, err
	}
	//defer res.Body.Close()
	return res, err
}

func PostByForm(url, contentType string, form url.Values) ([]byte, error) {
	body := bytes.NewBufferString(form.Encode())

	resp, err := http.Post(url, contentType, body)

	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()

	return ioutil.ReadAll(resp.Body)
}

func GetFileFromRequest(filename string, req *http.Request) ([]byte, *multipart.FileHeader, error) {
	file, handler, err := req.FormFile(filename)
	if err != nil {
		return nil, nil, err
	}
	defer func() {
		if err := file.Close(); err != nil {
			log.Error("Unable to close file reader: ", err.Error())
			return
		}
	}()

	fbytes, err := ioutil.ReadAll(file)
	if err != nil {
		return nil, nil, err
	}
	return fbytes, handler, err
}

// Convert Image to Base64 string from readers
//func EncodeImageFromReader(input ) (string, err) {
//}

// Convert Image to Base64 string from bytes
func EncodeImageFromBytes(input []byte) string {
	return base64.StdEncoding.EncodeToString(input)
}

// Base64 Encoding
func EncodeBase64(input string) string {
	b := make([]byte, base64.StdEncoding.EncodedLen(len(input)))
	base64.StdEncoding.Encode(b, []byte(input))
	return string(b)
}

// Base64 Encoding
func EncodeBase64FromBytes(b []byte) string {
	return base64.StdEncoding.EncodeToString(b)
}

// Base64 Decoding
func DecodeBase64(message string) (string, error) {
	base64Text := make([]byte, base64.StdEncoding.DecodedLen(len(message)))
	l, err := base64.StdEncoding.Decode(base64Text, []byte(message))
	return fmt.Sprintf("%s", base64Text[:l]), err
}

// HMAC-SHA1 encoding
func HMAC_SHA1(input, key string) []byte {
	h := hmac.New(sha1.New, []byte(key))
	h.Write([]byte(input))
	return h.Sum(nil)
}

// MD5, from string
func MD5ByString(input string) string {
	h := md5.New()
	io.WriteString(h, input)
	return fmt.Sprintf("%x", h.Sum(nil))
}

// MD5, from bytes
func MD5ByBytes(input []byte) string {
	return fmt.Sprintf("%x", md5.Sum(input))
}

// Encode Password
func EncodePassword(text string) string {
	hasher := md5.New()
	hasher.Write([]byte(text))
	return base64.StdEncoding.EncodeToString(hasher.Sum(nil))
}

// Random string
var letterRunes = []rune("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")

func RandString(n int) string {
	b := make([]rune, n)
	rand.Seed(time.Now().UnixNano())
	for i := range b {
		b[i] = letterRunes[rand.Intn(len(letterRunes))]
	}
	return string(b)
}

// This function is used to parse config file
func ParserConfig(_struct interface{}) error {
	path := "/var/arposter/config.json"
	file, err := os.OpenFile(path, os.O_RDONLY, 0666)
	if err != nil {
		return err
	}
	defer file.Close()

	decoder := json.NewDecoder(file)
	if err := decoder.Decode(&_struct); err != nil {
		return err
	}
	return nil
}
