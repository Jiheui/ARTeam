/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-04-22 20:38:53
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-08-15 20:55:01
*
* @Description: This file is created for handling email
*
 */
package Tools

import (
	"net/smtp"
	"regexp"
	//"strings"

	log "github.com/Sirupsen/logrus"
)

func SendConfirmLink(email string) {
	if !CheckEmail(email) {
		return
	}

	from := "arposter.team@gmail.com"
	pass := "arposter?t3am"
	to := email
	body := "shmily.me:8080/users/confirm/" + email

	msg := "From: " + from + "\n" +
		"To: " + to + "\n" +
		"Subject: Please click the link below to complete registration\n\n" +
		body

	err := smtp.SendMail("smtp.gmail.com:587",
		smtp.PlainAuth("", from, pass, "smtp.gmail.com"),
		from, []string{to}, []byte(msg))

	if err != nil {
		log.Errorf("smtp error: %s", err)
		return
	}
}

func CheckEmail(email string) (b bool) {
	re := regexp.MustCompile("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")
	return re.MatchString(email)
}
