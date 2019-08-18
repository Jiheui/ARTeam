/*
 * @Author: Yutao Ge
 * @Date: 2019-04-03 19:11:14
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-08-16 01:08:11
 * @Description: This file contains error types
 */
package Models

import (
	"errors"
)

var (
	ErrNotEnoughInfo = errors.New("Please provide necessary information.")
)
