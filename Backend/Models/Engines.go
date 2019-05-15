/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-04-02 19:00:20
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-05-15 10:24:42
 */
package Models

import (
	"encoding/json"
	"os"
	"sync"

	log "github.com/Sirupsen/logrus"
	_ "github.com/go-sql-driver/mysql"
	"github.com/go-xorm/xorm"
)

var Config struct {
	// mysql connection info
	Username string `json:"username"`
	Password string `json:"password"`

	// HiAR
	HiUsername   string `json:"hiusername"`
	HiPassword   string `json:"hipassword"`
	CollectionId string `json:"collectionid"`
}

type DBClient struct {
	Engine *xorm.Engine
	Lock   sync.RWMutex
}

var db DBClient
var err error

func init() {
	parserJson(&Config)
	db.Engine, err = xorm.NewEngine("mysql", Config.Username+":"+Config.Password+"@/ARPoster?charset=utf8")
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

func parserJson(_struct interface{}) error {
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
