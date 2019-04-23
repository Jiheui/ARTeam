/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-04-22 20:38:53
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-04-23 23:34:18
 */
package Tools

import (
	"net/smtp"
	"regexp"
	//"strings"

	log "github.com/Sirupsen/logrus"
)

func SendConfirmLink(email string) {
	if !checkEmail(email) {
		return
	}

	from := "arposter.team@gmail.com"
	pass := "arposter?t3am"
	to := email
	body := "shmily.me/users/confirm/" + email

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

func checkEmail(email string) (b bool) {
	if m, _ := regexp.MatchString("^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+(.[a-zA-Z0-9_-])+", email); !m {
		return false
	}
	return true
}
