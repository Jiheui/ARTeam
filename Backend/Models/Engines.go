/*
 * @Author: Yutao Ge
 * @Date: 2019-05-06 22:43:42
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-08-16 01:08:13
 * @Description: This file is created for MySQL connection
 */
package Models

import (
	"sync"

	"../Tools"

	log "github.com/Sirupsen/logrus"
	_ "github.com/go-sql-driver/mysql"
	"github.com/go-xorm/xorm"
)

var MysqlConfig struct {
	Username string `json:"username"`
	Password string `json:"password"`
}

type DBClient struct {
	Engine *xorm.Engine
	Lock   sync.RWMutex
}

var db DBClient
var err error

func init() {
	Tools.ParserConfig(&MysqlConfig)
	db.Engine, err = xorm.NewEngine("mysql", MysqlConfig.Username+":"+MysqlConfig.Password+"@/ARPoster?charset=utf8")
	db.Engine.ShowSQL(true)
	if err != nil {
		log.Info(err)
		return
	}
	//连接测试
	if err := db.Engine.Ping(); err != nil {
		log.Info(err)
		return
	}
}

// Write lock
func (d *DBClient) WLock() {
	d.Lock.Lock()
}

func (d *DBClient) WUnlock() {
	d.Lock.Unlock()
}
