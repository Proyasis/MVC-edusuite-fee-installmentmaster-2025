var DesignationGrade = (function () {
    var gr = 0;
    var TotalItem = 0;
    var getDesignationGrades = function () {
        $(".section-content").mLoading();
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetDesignationGradeList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()

                }
            },
            colNames: [Resources.RowKey, Resources.Designation, Resources.Designation, Resources.Grade, Resources.MonthlySalary, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'DesignationKey', index: 'DesignationKey', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DesignationName', index: 'DesignationName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DesignationGradeName', index: 'DesignationGradeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MonthlySalary', index: 'MonthlySalary', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            hidegrid: false,
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader: {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: true,
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow'
        })

        $("#grid").jqGrid("setLabel", "DesignationName", "", "thDesignationName");
        $(".section-content").mLoading("destroy");
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        var Name = "'" + rowdata.DesignationGradeName + "'";
       // return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" onclick="DesignationGrade.UpdateGradeName(' + temp + ',' + Name + ')"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm" onclick="javascript:deleteDesignationGradeDetail(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>';
        //return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" onclick="DesignationGrade.UpdateGradeName(' + temp + ',' + Name + ')"><i class="fa fa-pencil" aria-hidden="true"></i></a></div>';



        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" href="AddEditDesignationGrade/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm" onclick="javascript:deleteDesignationGradeDetail(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>';
        //return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" onclick="DesignationGrade.UpdateGradeName(' + temp + ',' + Name + ')"><i class="fa fa-pencil" aria-hidden="true"></i></a></div>';

    }


    var getDesignationGradeById = function () {
        var DesignationKey = $("#DesignationKey").val() != "" ? $("#DesignationKey").val() : "0";
        var response = AjaxHelper.ajax("GET", $("#hdnGetDesignationGradeDetailsById").val() + "/" + DesignationKey
        );
        var options = {
            row: "SalaryHeadCode",
            row1: "SalaryHeadName",
            column: "DesignationGradeName",
            value: "Formula"
        };
        var checkResponse = response.filter(filterJSONResult)
        if (checkResponse.length > 0) {
            $.confirm({
                title: Resources.AddNewGrade,
                content: '<input type="text" id="txtGradeName" class="form-control" onkeyup="ValidateGradeInput(this)"/>',
                buttons: {
                    Add: {
                        text: 'Add',
                        btnClass: 'btn-outline-primary',
                        action: function () {
                            var input = this.$content.find('input#txtGradeName');
                            var errorText = this.$content.find('.text-danger');
                            if (!input.val().trim()) {
                                $.alert({
                                    content: Resources.GradeNameRequired,
                                    type: 'red'
                                });
                                return false;
                            }
                            else {
                                $.each(response, function (i) {
                                    if (response[i]["DesignationGradeName"] == null) {
                                        response[i]["DesignationGradeName"] = input.val().trim();
                                    }

                                });
                                var output = PivotJSON(response, "SalaryHeadName", "SalaryHeadCode", "DesignationGradeName", "Formula");
                                DesignationGrade.SetExcelSheelValue(output);
                            }
                        },

                    },
                    Cancel: {
                        text: 'Cancel',
                        btnClass: 'btn-danger',
                        action: function () {
                        }
                    }
                }
            });
            setTimeout(function () {
                $("input#txtGradeName").focus();
            }, 1000)
        }
        else {
            var output = PivotJSON(response, "SalaryHeadName", "SalaryHeadCode", "DesignationGradeName", "Formula");
            //DesignationGrade.SetExcelSheelValue(output);

        }

    }

    var setExcelSheelValue = function (JsonData) {
        $('#Excel').jexcel({
            data: JsonData,
            // colHeaders: JsonHead,
            columnSorting: false,
            colWidths: [30, 300],
            columns: [
                { type: 'text' },
                { type: 'text' },
                { type: 'number', editorRegx: /[^0-9 .]/g },
                { type: 'number', editorRegx: /[^0-9 .]/g },
                { type: 'number', editorRegx: /[^0-9 .]/g },
                { type: 'number', editorRegx: /[^0-9 .]/g }
            ],
            allowInsertColumn: true
        });
        var val0 = "", val2 = "";
        $('#Excel').jexcel('updateSettings', {
            cells: function (cell, col, row) {
                // If the column is number 4 or 5
                var value = $("#Excel").jexcel('getValue', $(cell)).trim()
                value = '' + value.replace(',', '');
                value = value != "" ? value : "0"
                if (value == "") {
                    $("#Excel").jexcel('setValue', $(cell), "0").trim()
                }
                if (row == 0 || col == 0 || col == 1) {
                    $(cell).addClass('readonly');
                }

                if (col == 0) {
                    val0 = $("#Excel").jexcel('getValue', $(cell)).trim()

                }
                else if (col == 2) {
                    val2 = $("#Excel").jexcel('getValue', $(cell)).trim();
                }
                if ((val0 == "-") && col == 2) {
                    $(cell).closest("tr").addClass("sub-title");

                }
                if ((val0 == "*") && col == 2) {
                    $(cell).closest("tr").addClass("main-footer");
                }
                if ((val0 == "-") && col == 2) {
                    if ((val2 != "-" && val2 != "")) {
                        $(cell).closest("tr").addClass("sub-footer");
                    }
                    else {
                        $(cell).closest("tr").find("td").each(function (i) {
                            if (i != 0 && i != 2)
                                $(this).html("-").addClass('readonly');
                        })
                    }

                }

            }
        });

        $("#jexcel_contextmenu").remove();
    }



    var updateDesignationGrade = function (jsonData) {

        $(".section-content").mLoading();


        setTimeout(function () {
            var form = $("form");
            var url = $(form).attr("action");
            var type = $(form).attr("method");
            var modelListData = GetExcelData(jsonData);
            var response = AjaxHelper.ajax(type, url, {
                modelList: modelListData
            });
            if (response.IsSuccessful == true) {
                window.location.href = $("#hdnDesignationGradeList").val();
            }
            else {
                $("#error_msg").html(response.Message);
            }
            $(".section-content").mLoading("destroy");

        }, 1000)
    }

    var downloadExcel = function () {
        var data = [];
        data = GetFormatedExcelData();
        var ws_name = "Wage structure";
        var wb = new Workbook(), ws = sheet_from_array_of_arrays(data);
        /* add worksheet to workbook */
        wb.SheetNames.push(ws_name);
        wb.Sheets[ws_name] = ws;
        var wbout = XLSX.write(wb, { bookType: 'xlsx', bookSST: true, type: 'binary' })
        saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), "Wage structure.xlsx")

    }

    var handleFile = function (e) {
        //Get the files from Upload control
        var files = e.target.files;
        var i, f;
        var result;

        for (i = 0, f = files[i]; i != files.length; ++i) {
            var reader = new FileReader();
            var name = f.name;
            reader.onload = function (e) {
                var data = e.target.result;
                var wb, arr;
                function doit() {
                    try {
                        wb = XLSX.read(data, { type: 'binary' });
                        var JsonData = process_wb(wb)
                        DesignationGrade.SetExcelSheelValue(JsonData)
                    } catch (e) { console.log(e); }
                }

                if (e.target.result.length > 1e6) opts.errors.large(e.target.result.length, function (e) { if (e) doit(); });
                else { doit(); }
            };
            reader.readAsBinaryString(f);

        }
    }

    var addNewGrade = function () {
        $.confirm({
            title: Resources.AddNewGrade,
            content: '<input type="text" id="txtGradeName" class="form-control" onkeyup="ValidateGradeInput(this)"/>',
            buttons: {
                Add: {
                    text: 'Add',
                    btnClass: 'btn-outline-primary',
                    action: function () {
                        var input = this.$content.find('input#txtGradeName');
                        var errorText = this.$content.find('.text-danger');
                        var data = $("#Excel").jexcel('getData', false);
                        data = data[0].splice(2, data[0].length - 1);
                        if (!input.val().trim()) {
                            $.alert({
                                title: Resources.Warning,
                                content: Resources.GradeNameRequired,
                                type: 'red'
                            });

                            return false;
                        }
                        else if (data.indexOf(input.val().trim()) > -1) {
                            $.alert({
                                title: Resources.Warning,
                                content: "'" + input.val() + "' is Already Exists",
                                type: 'orange'
                            });
                            return false;
                        }
                        else {

                            $('#Excel').jexcel('insertColumn', 1, { header: input.val().trim() });
                        }
                    }
                },
                Cancel: function () {
                    // do nothing.
                }
            }

        });
        setTimeout(function () {
            $("input#txtGradeName").focus();
        }, 1000)
    }


    var updateGradeName = function (Id, Name) {
        $.confirm({
            title: Resources.EditGrade,
            content: '<input type="hidden" id="hdnDesignationGradeKey" value="' + Id + '"/><input type="text" id="txtGradeName" class="form-control text-uppercase" onkeyup="ValidateGradeInput(this)" value="' + Name + '"/>',
            buttons: {
                Add: {
                    text: Resources.Add,
                    btnClass: 'btn-outline-primary',
                    action: function () {
                        var input = this.$content.find('input#txtGradeName');
                        var errorText = this.$content.find('.text-danger');
                        if (!input.val().trim()) {
                            $.alert({
                                content: Resources.GradeNameRequired,
                                type: 'red'
                            });
                            return false;
                        } else {
                            var response = AjaxHelper.ajax("POST", $("#hdnUpdateDesignationGradeName").val(), {
                                Id: $("#hdnDesignationGradeKey").val(),
                                DesignationGradeName: $("#txtGradeName").val().trim()
                            });
                            if (response.IsSuccessful == true) {
                                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                            }
                            else {
                                $.alert({
                                    content: response.Message,
                                    type: 'red'
                                });
                            }
                        }
                    }
                },
                Cancel: function () {
                    // do nothing.
                }
            }

        });
        setTimeout(function () {
            var value = $("input#txtGradeName").val()
            $("input#txtGradeName").focus().val('').val(value);
        }, 1000)
    }

    return {
        GetDesignationGrades: getDesignationGrades,
        GetDesignationGradeById: getDesignationGradeById,
        SetExcelSheelValue: setExcelSheelValue,
        HandleFile: handleFile,
        AddNewGrade: addNewGrade,
        UpdateGradeName: updateGradeName,
        UpdateDesignationGrade: updateDesignationGrade,
        DownloadExcel: downloadExcel
    }
})();

function UpdateExcelData(json) {
    delete json[0];
    var newJson = [];

    $.each(json, function (i) {
        var newJsonItem = [];
        $.each(json[i], function (j) {

            newJsonItem.push(json[i][j])
        })
        if (newJsonItem.length > 0)
            newJson.push(newJsonItem);
    })
    return newJson;
}

function ValidateGradeInput(_this) {
    var start = _this.selectionStart
    $(_this).val($(_this).val().toUpperCase())
    var test = /^[\s]/;
    if (test.test($(_this).val())) {
        var newVal = $(_this).val().replace(/[\s *0-]/g, "");
        $(_this).val(newVal);
    }
    _this.selectionStart = _this.selectionEnd = start;
}



function GetExcelData(jsonData) {
    var obj = {};
    obj.checkCols = [];


    var data = $("#Excel").jexcel('getData', false);
    var formula = $("#Excel").jexcel('getFormulaData', false);
    var Grades = [];
    obj.Keys = data[0]
    var DataList = []
    var i = 0;
    obj.Keys.forEach(function (item) {
        if (i > 1) {
            Grades.push(item)
        }
        i++;
    })
    changeColumnNameToCode(obj)
    $.each(Grades, function (p) {

        var NewData = jQuery.extend(true, {}, jsonData);
        NewData.DesignationKey = $("#DesignationKey").val()
        var checkList = [];
        $.each(data, function (j) {
            var item = $(this);
            if (j == 0 && Grades[p] == $(this)[p + 2]) {
                var ColumnLetter = $("#Excel").jexcel('getColumnName', p + 2);
                NewData["DesignationGradeName"] = $(this)[p + 2];
                NewData["ColumnLetter"] = ColumnLetter;

                checkList = obj.checkCols.filter(function (n, m) {
                    return n.grade === item[p + 2];
                });
            }
            else {
                if (!item[p + 2]) {
                    $.alert({
                        type: 'red',
                        title: Resources.Failed,
                        content: Resources.AnErrorFoundInExcelSheet,
                        icon: 'fa fa-check-circle-o-',
                        buttons: {
                            Ok: {
                                text: Resources.Ok,
                                btnClass: 'btn-danger',
                                action: function () {

                                }
                            }
                        }
                    })
                    return false;
                }
                if (item[0] != "-" && item[0] != "*") {
                    var formulaString = formula[j][p + 2].substr(0, 1) == '=' ? formula[j][p + 2] : "";
                    formulaString = excelFormulaUtilities.formula2JavaScript(formulaString)

                    $(checkList).each(function () {
                        formulaString = formulaString.replace(new RegExp(this.column, 'gi'), "{{" + this.code + "}}");
                    })

                    NewData.DesignationGradeDetails.push(
                        {
                            RowKey: 0,
                            DesignationGradeKey: 0,
                            DesignationGradeName: "",
                            SalaryHeadKey: 0,
                            SalaryHeadCode: item[0],
                            SalaryHeadName: "",
                            AmountUnit: item[p + 2],
                            Formula: formulaString
                        }
                    );
                }

            }
        })
        DataList.push(NewData);

    });
    return DataList;
}

function GetFormatedExcelData() {
    var formula = $("#Excel").jexcel('getFormulaData', false);
    var DataList = []
    $.each(formula, function () {
        var items = $(this);
        var newItem = [];
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            if (parseFloat(item) == item) {
                newItem.push(parseFloat(item));
            }

            else {
                newItem.push(item);
            }
        }
        DataList.push(newItem);
    })

    return DataList;
}

function PivotJSON(dataArray, rowIndex, rowIndex1, colIndex, dataIndex) {
    var result = {}, ret = [];
    var newCols = [], newRows = [];
    for (var i = 0; i < dataArray.length; i++) {

        if (!result[dataArray[i][rowIndex]]) {
            result[dataArray[i][rowIndex]] = {};
        }
        result[dataArray[i][rowIndex]][dataArray[i][colIndex]] = dataArray[i][dataIndex];
        result[dataArray[i][rowIndex]][dataArray[i][rowIndex1]] = dataArray[i][dataIndex];

        //To get column names
        if (newCols.indexOf(dataArray[i][colIndex]) == -1 && dataArray[i][colIndex] != "") {
            newCols.push(dataArray[i][colIndex]);
        }
        if (newRows.indexOf(dataArray[i][rowIndex1]) == -1 || dataArray[i][colIndex] == "") {
            newRows.push(dataArray[i][rowIndex1]);
        }
    }

    newCols.sort();
    var item = [];

    //Add Header Row
    item.push("Code");
    item.push("Description");
    item.push.apply(item, newCols);
    ret.push(item);

    //Add content 
    var j = 0;
    var k = 2;
    var p = 0;
    var FirstItem = 0, LastItem = 0, colMonthly = 0, colDedusction = 0, currentHead = 0;
    var GroupHead = "";
    var TotalSum = [];
    for (var key in result) {

        var count = Object.keys(result).length;

        if (newRows[j] != "") {
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

            for (var i = 0; i < newCols.length; i++) {
                var letter = String.fromCharCode(67 + i);
                item.push("=SUM(" + letter + FirstItem.toString() + ":" + letter + LastItem.toString() + ")");

            }
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

                for (var i = 0; i < newCols.length; i++) {
                    var letter = String.fromCharCode(67 + i);
                    item.push("=" + letter + colMonthly.toString() + "-" + letter + colDeduction.toString());
                }
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
        item.push(newRows[j] || "-");
        item.push(key);
        for (var i = 0; i < newCols.length; i++) {
            item.push(result[key][newCols[i]] || (newRows[j] ? "0" : "-"));
        }
        ret.push(item);
        j++; k++;
    }
    if (j == count) {
        item = [];
        LastItem = k - 1;
        item.push("-", "TOTAL " + Object.keys(result)[currentHead]);

        for (var i = 0; i < newCols.length; i++) {
            var letter = String.fromCharCode(67 + i);
            item.push("=SUM(" + letter + FirstItem.toString() + ":" + letter + LastItem.toString() + ")");

        }
        ret.push(item);
        TotalSum.push(k);
    }

    item = [];
    item.push("*", "TOTAL ")
    for (var i = 0; i < newCols.length; i++) {
        var letter = String.fromCharCode(67 + i);
        var Formula = "=";
        TotalSum.forEach(function (item) {
            Formula = Formula + letter + item + "-";
        });
        item.push(Formula.substring(0, Formula.length - 1));

    }
    ret.push(item);
    //if (!newCols[0]) {
    //    $.each(ret, function (i) {
    //        this.forEach(function (item, j) {
    //            if (j > 1) {
    //                ret[i].splice(ret[i].length - 1, 1);
    //            }
    //        });
    //    })
    //}
    return ret
}

function deleteDesignationGrade(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_DesignationGrade,
        actionUrl: $("#hdnDeleteDesignationGrade").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function deleteDesignationGradeDetail(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_DesignationGradeDetail,
        actionUrl: $("#hdnDeleteDesignationGradeDetail").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function filterJSONResult(grade) {
    return grade.DesignationGradeName == null;
}

function changeColumnNameToCode(obj) {

    var codeCols = $("#Excel").find('.jexcel tbody tr td:nth-child(2)').not('.jexcel_label').map(function (k, v) {
        return v.innerHTML
    });
    obj.Keys.forEach(function (item, i) {
        if (i > 1) {
            var dataCols = $("#Excel").find('.jexcel tbody tr td:nth-child(' + (i + 2) + ')').not('.jexcel_label').map(function (k, v) {
                return $("#Excel").jexcel('getColumnNameFromId', $(v).prop('id'));
            });
            for (var index in codeCols) {
                if (parseInt(index)) {
                    obj.checkCols.push({ code: codeCols[index], column: dataCols[index], grade: item })
                }
            }
        }

    })
}
