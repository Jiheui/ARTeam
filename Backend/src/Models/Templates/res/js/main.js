/*
 * @Author: Yue Jiang
 * @Date: 2019-08-28 22:36:53
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-09-11 18:45:35
 * @Description: 
 */

// ------------------------------------------------------ //
// Show poster information panel
// ------------------------------------------------------ //

function showPanel(targetId) {
    document.getElementById(targetId).classList.remove('infoPanel');
    document.getElementById("btn_" + targetId).innerHTML = "Hide";
    document.getElementById("btn_" + targetId).setAttribute("onclick",  "closePanel('" + targetId + "')");
}

function closePanel(targetId) {
    document.getElementById(targetId).classList.add('infoPanel');
    document.getElementById("btn_" + targetId).innerHTML = "Show";
    document.getElementById("btn_" + targetId).setAttribute("onclick",  "showPanel('" + targetId + "')");
}
