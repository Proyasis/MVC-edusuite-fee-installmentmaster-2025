var EmployeeSalarySettings = (function () {

    var getEmployeeSalarySettings = function (json) {
        var output = PivotJSON(json["EmployeeSalaryDetails"], "SalaryHeadName", "SalaryHeadKey", "Amount");
        EmployeeSalarySettings.SetExcelSheelValue(output);
    }
    var setExcelSheelValue = function (JsonData) {
        $('#Excel').jexcel({
            data: JsonData,
            // colHeaders: JsonHead,
            columnSorting: false,
            colWidths: [0, 300],
            columns: [
               { type: 'text' },
               { type: 'text' },
               { type: 'number' },
               { type: 'number' },
               { type: 'number' },
               { type: 'number' }
            ]
        });
        var val0 = "", val2 = "", valueTotal = "";
        $('#Excel').jexcel('updateSettings', {
            cells: function (cell, col, row) {

                $(cell).addClass('readonly');

                if (col == 0) {
                    val0 = $("#Excel").jexcel('getValue', $(cell)).trim()

                    $(cell).hide();

                }
                else if (col == 2) {
                    val2 = $("#Excel").jexcel('getValue', $(cell)).trim();
                    if (val0 == "0" && val2 == "0")
                        $(cell).html("-");
                }
                if ((val0 == "0") && col == 2) {
                    $(cell).closest("tr").addClass("sub-title");
                }
                if ((val0 == "*") && col == 2) {
                    $(cell).closest("tr").addClass("main-footer");
                }
                if ((val0 == "0" || val0 == "-") && col == 2) {
                    if (val2 != "0" && val2 != "-") {
                        $(cell).closest("tr").addClass("sub-footer");
                    }


                }
                if (valueTotal == "*") {
                    var length = $("#Excel tbody > tr").length - 1;
                    if (col == 0) {
                        $(cell).closest("tr").hide();
                    }
                    else if (col == 2 && (val0 != "*" || length == row)) {
                        var copyCell = $(cell).closest("tr").clone(true);
                        $("#Payment>table>tbody").append($(copyCell).show());
                    }
                }


                if (val0 == "*")
                    valueTotal = val0;

            }
        });

        $(".jexcel_label").hide();
    }

    return {
        GetEmployeeSalarySettings: getEmployeeSalarySettings,
        SetExcelSheelValue: setExcelSheelValue
    }

}());

function deleteEmployeeSalarySettings(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_SalarySettings,
        actionUrl: $("#hdnDeleteEmployeeSalarySettings").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            Employee.ReloadData();
        }
    });
}

function ModifyModel(data) {
    var JsonData = [];
    $form = $("form");
    var $additionalComponents = $form.find('[data-repeater-item]');

    $additionalComponents.each(function () {
        var JsonObject = {};
        $(this).find('input, select, textarea').each(function () {
            if ($(this).is(':checkbox')) {
                var matches = $(this).attr('name').match(/].(.*)/)[0].replace('].', '');
                var name = matches.replace('][', '').replace(']', '').replace('[]', '');
                JsonObject[name] = $(this).prop('checked');
            }
            else if ($(this).val() != "" && $(this).attr("id") != undefined) {
                var matches = $(this).attr('name').match(/].(.*)/)[0].replace('].', '');
                var name = matches.replace('][', '').replace(']', '');
                JsonObject[name] = $(this).val();
            }

        })
        JsonData.push(JsonObject);

    })

    data["AdditionalSalaryComponents"] = JsonData;
    return data;
}

function PivotJSON(dataArray, rowIndex, rowIndex1, dataIndex) {
    var result = {}, ret = [];
    var newRows = [];
    for (var i = 0; i < dataArray.length; i++) {

        if (!result[dataArray[i][rowIndex]]) {
            result[dataArray[i][rowIndex]] = {};
        }

        result[dataArray[i][rowIndex]][0] = dataArray[i][dataIndex];

        if (newRows.indexOf(dataArray[i][rowIndex1]) == -1 || dataArray[i][colIndex] == "") {
            newRows.push(dataArray[i][rowIndex1].toString());
        }
    }

    var item = [];


    //Add content 
    var j = 0;
    var k = 1;
    var p = 0;
    var FirstItem = 0, LastItem = 0, colMonthly = 0, colDedusction = 0, currentHead = 0;
    var GroupHead = "";
    var letter = "C";
    var TotalSum = [];
    for (var key in result) {

        var count = Object.keys(result).length;

        if (newRows[j] != "0") {
            if (FirstItem == 0) {
                FirstItem = k;
            }
        }
        else if (j != 0) {
            LastItem = k - 1;
        }

        if (newRows[j] == "" && LastItem != 0) {
            GroupHead = key;
        }
        if (FirstItem != 0 && LastItem != 0) {
            item = [];
            item.push("-", "TOTAL " + Object.keys(result)[currentHead]);
            item.push("=SUM(" + letter + FirstItem.toString() + ":" + letter + LastItem.toString() + ")");

            ret.push(item);
            FirstItem = 0;
            LastItem = 0;
            k++;

            if (p == 0) {
                colMonthly = k - 1;
            }
            else if (p == 1) {
                colDeduction = k - 1;
            }
            if (p == 1) {

                item = [];
                item.push("*", "NET MONTHLY SALARY");
                item.push("=" + letter + colMonthly.toString() + "-" + letter + colDeduction.toString());
                k++;
                ret.push(item);
            }
            else {
                TotalSum.push(k - 1);
            }
            p++;
            currentHead = j;

        }

        item = [];
        item.push(newRows[j].toString() || "-");
        item.push(key);
        item.push(result[key][0].toString() || "-");
        ret.push(item);
        j++; k++;
    }
    if (j == count) {
        item = [];
        LastItem = k - 1;
        item.push("-", "TOTAL " + Object.keys(result)[currentHead]);
        item.push("=SUM(" + letter + FirstItem.toString() + ":" + letter + LastItem.toString() + ")");
        ret.push(item);
        TotalSum.push(k);
    }

    item = [];
    item.push("*", "TOTAL ")
    var Formula = "=";
    TotalSum.forEach(function (item) {
        Formula = Formula + letter + item + "+";
    });
    item.push(Formula.substring(0, Formula.length - 1));
    ret.push(item);

    return ret
}

