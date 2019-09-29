/*
 * @Author: Yue Jiang
 * @Date: 2019-08-28 22:36:53
 * @Email: chris.dfo.only@gmail.com
 * @Last Modified by: Yutao Ge
 * @Last Modified time: 2019-09-30 02:49:35
 * @Description: 
 */

// ------------------------------------------------------ //
// Show poster information panel
// ------------------------------------------------------ //

function showPanel(targetId) {
    document.getElementById(targetId).classList.remove('infoPanel');
    document.getElementById("btn_" + targetId).setAttribute("onclick", "closePanel('" + targetId + "')");
}

function closePanel(targetId) {
    document.getElementById(targetId).classList.add('infoPanel');
    document.getElementById("btn_" + targetId).setAttribute("onclick", "showPanel('" + targetId + "')");
}

// ------------------------------------------------------ //
// Input option panel
// ------------------------------------------------------ //
const newTr = `
<tr>
    <td class="pt-3-half" contenteditable="false" style="display:none"></td>
    <td class="pt-3-half" contenteditable="true"></td>
    <td class="pt-3-half" contenteditable="true">
        <select>
            <option value="1" selected>Text Area</option>
			<option value="2">Radio Button</option>
			<option value="3">Check Box</option>
		</select>
    </td>
    <td class="pt-3-half" contenteditable="true"></td>
    <td>
        <span class="table-save">
            <a onclick="SaveRow(event)"><i class="far fa-save"></i></a>
        </span>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <span class="table-remove">
            <a onclick="DeleteRow(event)"><i class="far fa-trash-alt"></i></a>
        </span>
    </td>
</tr>`;

function AddNewRow(table_id){
    var $tableID = $(".table_" + table_id);

    var $clone = $tableID.find('tbody tr').last().clone(true).removeClass('hide table-line');
    var $firstChild = $clone.find('td').first();

    if ($clone.length == 0 || $firstChild.text() != "") {
        $tableID.find('tbody').append(newTr);
    }
}

function SaveRow(e) {
    var targetId = $(e.target).parents('tr').parents('tbody').find('tr').first().find('td').text().trim();
    var currentElem = $(e.target).parents('tr').find('td').first();

    if(currentElem.text().trim() == "") {
        // Insert
        currentElem = currentElem.next();
        var name = currentElem.text();

        currentElem = currentElem.next();
        var inputType = currentElem.find('select').first().val();

        currentElem = currentElem.next();
        var options_string = currentElem.text();

        insertNewRow(e, name, inputType, options_string, targetId);
    } else {
        // Update
        var qid = currentElem.text().trim();
        
        currentElem = currentElem.next();
        var name = currentElem.text().trim();

        currentElem = currentElem.next();
        var inputType = currentElem.find('select').first().val().trim();

        currentElem = currentElem.next();
        var options_string = currentElem.text().trim();

        updateRow(qid, name, inputType, options_string);
    }

}

function DeleteRow(e){
    var currentElem = $(e.target).parents('tr').find('td').first();
    
    if(currentElem.text().trim() == "") {
        $(e.target).parents('tr').detach();
        return;
    }

    var qid = parseInt(currentElem.text().trim(), 10);

    $.ajax(
        {
            headers: { 
                'Content-Type': 'application/json' 
            },
            url: "/inputs/delete",
            type: "POST",
            data: "{\"qid\":" + qid + "}",
            success: function(results) {
                var obj = JSON.parse(results);

                if(obj.error == "") {
                    $(e.target).parents('tr').detach();
                    alertify.success("Successfully delete a question.");
                } else {
                    alertify.error(obj.error);
                }
            }
        }
    )
}

function insertNewRow(e, name, inputType, options_string, targetId) {
    $.ajax(
        {
            headers: { 
                'Content-Type': 'application/json' 
            },
            url: "/inputs/save",
            type: "POST",
            data: "{\"option_string\":\"" + options_string + 
                    "\", \"type\":" + inputType + 
                    ", \"name\":\"" + name + 
                    "\", \"targetid\":\"" + targetId + 
                "\"}",
            success: function(results) {
                var obj = JSON.parse(results);

                if(obj.error == "") {
                    var currentElem = $(e.target).parents('tr').find('td').first();
                    currentElem.text(obj.qid);
                    alertify.success("Successfully save a new question.");
                } else {
                    alertify.error(obj.error);
                }
            }
        }
    )
}

function updateRow(qid, name, inputType, options_string) {
    $.ajax(
        {
            headers: { 
                'Content-Type': 'application/json' 
            },
            url: "/inputs/update",
            type: "POST",
            data: "{\"option_string\":\"" + options_string + 
                    "\", \"type\":" + inputType + 
                    ", \"qid\":" + qid + 
                    ", \"name\":\"" + name +  
                "\"}",
            success: function(results) {
                var obj = JSON.parse(results);

                if(obj.error == "") {
                    alertify.success("Successfully update a question.");
                } else {
                    alertify.error(obj.error);
                }

                console.log("{\"text_options\":\"" + text_options + 
                "\", \"type\":" + inputType + 
                ", \"qid\":" + qid + 
                ", \"name\":\"" + name +  
            "\"}");
            }
        }
    )
}