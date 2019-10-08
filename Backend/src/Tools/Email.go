/*
 * @Author: Yutao Ge
 * @Date: 2019-04-22 20:38:53
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-10-09 06:21:52
 * @Description: This file is created for handling email
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
	body := "ar.shmily.tk:8080/users/confirm/" + email
	subject := "Subject: Please click the link below to complete registration\n\n"
	mailBase(email, subject, body)
}

func SendPassword(email, password string) {
	body := "Your new password is set to: " + password + ". You can use this password to login."
	subject := "Password Reset\n\n"
	mailBase(email, subject, body)
}

func mailBase(email, subject, body string) {
	from := "arposter.team@gmail.com"
	pass := "arposter?t3am"
	to := email

	msg := "From: " + from + "\n" +
		"To: " + to + "\n" +
		subject +
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
