/*
* @Author: Yutao Ge
* @E-mail: u6283016@anu.edu.au
* @Date:   2019-04-03 19:11:14
* @Last Modified by:   Yutao Ge
* @Last Modified time: 2019-08-11 22:01:04
*
* @Description: This file contains error types
*
 */
package Models

import (
	"errors"
)

var (
	ErrNotEnoughInfo = errors.New("Please provide necessary information.")
)
