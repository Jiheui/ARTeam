/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-08-11 21:50:37
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-08-15 20:40:18
*
* @Description: This file contains serveral some tools different usage
*
 */
package Tools

import (
	"crypto/hmac"
	"crypto/md5"
	"crypto/sha1"
	"encoding/base64"
	"encoding/json"
	"fmt"
	"io"
	"os"
)

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

// Base64 Decoding
func DecodeBase64(message string) (string, error) {
	base64Text := make([]byte, base64.StdEncoding.DecodedLen(len(message)))
	l, err := base64.StdEncoding.Decode(base64Text, []byte(message))
	return fmt.Sprintf("%s", base64Text[:l]), err
}

// HMAC-SHA1 encoding
func HMAC_SHA1(input, key string) string {
	h := hmac.New(sha1.New, []byte(key))
	h.Write([]byte(input))
	return fmt.Sprintf("%x", h.Sum(nil))
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
