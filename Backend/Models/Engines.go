/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-04-02 19:00:20
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-04-04 10:20:27
 */
package Models

import (
	"sync"

	log "github.com/Sirupsen/logrus"
	_ "github.com/go-sql-driver/mysql"
	"github.com/go-xorm/xorm"
)

type DBClient struct {
	Engine *xorm.Engine
	Lock   sync.RWMutex
}

var db DBClient
var err error

func init() {

	db.Engine, err = xorm.NewEngine("mysql", "root:jiggy111@/ARPoster?charset=utf8")
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
