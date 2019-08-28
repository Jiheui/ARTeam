
// ------------------------------------------------------ //
// Show poster information panel
// ------------------------------------------------------ //

function showPanel() {

    var panels = document.getElementsByClassName('infoPanel');
    Array.prototype.forEach.call(panels, function (panel) {
        panel.style.display = 'block';
    });

}
