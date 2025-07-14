
var JsonData = [];

var ScholarshipExamResult = (function () {

    var getScholarshipExamResult = function () {

        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetScholarshipExamResult").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: {

                SearchName: function () {
                    return $('#SearchName').val()
                },
                SearchPhone: function () {
                    return $('#SearchPhone').val()
                },
                SearchFromDate: function () {
                    return $('#SearchFromDate').val()
                },
                SearchToDate: function () {
                    return $('#SearchToDate').val()
                },
                SearchDistrictKey: function () {
                    return $('#SearchDistrictKey').val()
                },
                SearchBranchKey: function () {
                    return $('#SearchBranchKey').val()
                },
                SearchScholarshipTypeKey: function () {
                    return $('#SearchScholarshipTypeKey').val()
                },
                SearchSubBranchKey: function () {
                    return $('#SubBranchKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace,
                Resources.Name, Resources.MobileNo, Resources.center,
                Resources.City, Resources.District, Resources.Scholarship,
                Resources.EmailAddress, Resources.RegNo, Resources.Mark, Resources.Status],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'ScholarshipExamScheduleKey', index: 'ScholarshipExamScheduleKey', editable: true },
                { key: false, name: 'ScholarShipName', index: 'ScholarShipName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MobileNumber', index: 'MobileNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BranchName', index: 'DistrictName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'LocationName', index: 'LocationName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DistrictName', index: 'UniversityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'ScholarshipDate', index: 'ScholarshipDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'ScholarshipTypeName', index: 'ScholarshipTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'EmailAddress', index: 'EmailAddress', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ExamRegNo', index: 'ExamRegNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'ExamDate', index: 'ExamDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'Mark', index: 'Mark', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ResultStatus', index: 'ResultStatus', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: checkStatus },

                //{ name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 250 },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 20, 50, 100, 250, 500],
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
            multiselect: true,
            loadonce: false,
            ignoreCase: true,
            sortname: 'RowKey',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow',

        })

        $("#grid").jqGrid("setLabel", "ScholarshipName", "", "thScholarshipName");
    }





    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return "";//'<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditScholarship").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteScholarship(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>';
    }
    function checkStatus(cellValue, options, rowdata, action) {
        
        var html = "";

        switch (rowdata.ResultStatus != null && rowdata.ResultStatus.trim()) {
            case "A":
                html = '<span class="label label-warning">' + "Absent" + '</span>';
                break;
            case "P":
                html = '<span class="label label-success">' + "Present" + '</span>';
                break;
            default:
                html = '<span class="label label-danger">' + "No Result" + '</span>';

        }
        return html;
    }

    var getScholarshipExamResultDetails = function (json) {

        //$('.repeater').repeater({
        //    show: function () {
        //        $(this).slideDown();
        //        AppCommon.FormatDateInput();
        //        AppCommon.CustomRepeaterRemoteMethod();
        //        AppCommon.FormatInputCase();
        //        Application.ItemLoad($(this));
        //        //$("[id*=ApplicationKey]", $(this)).val("").removeAttr("disabled")
        //    },
        //    hide: function (remove) {
        //        var self = $(this).closest('[data-repeater-item]').get(0);
        //        var hidden = $(self).find('input[type=hidden]')[0];
        //        if ($(hidden).val() != "" && $(hidden).val() != "0") {

        //        }
        //        else {
        //            $(this).slideUp(remove);
        //        }
        //    },
        //    rebind: function (response) {
        //        if (typeof response == "string") {
        //            $("[data-valmsg-for=error_msg]").html(response);
        //        }
        //        else if (response.IsSuccessful) {
        //            toastr.success(Resources.Success, response.Message);
        //            $("#frmAddEditScholarshipExamResult").closest(".modal").modal("hide")
        //            $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
        //        }

        //    },
        //    data: json,
        //    repeatlist: 'ScholarshipExamDetails',
        //    submitButton: '',
        //    defaultValues: json
        //});
    }

    var setExamResultStatus = function () {
        $("#DivScholarshipExamResult [data-repeater-item]").each(function (i) {

            //var item = $(_this).closest("[data-repeater-item]");
            var AbsentStatus = $("input[type=checkbox][id*=AbsentStatus]", $(this)).is(":checked");
            var Mark = $("input[id*=Mark]", $(this))[0];
            //var MinimumMark = $("input[id*=MinimumMark]", $(this)).val();
            //var MaximumMark = $("input[id*=MaximumMark]", $(this)).val();
            var ResultStatus = $("input[id*=ResultStatus]", $(this))[0];

            if (AbsentStatus == true) {
                $("#spnResultStatus", $(this)).html("Absent").attr("style", "color:orange;");

                ResultStatus.value = "A";
                Mark.value = "";
                Mark.disabled = true;
            }
            else {
                Mark.disabled = false;

                if (Mark.value == "") {
                    $("#spnResultStatus", $(this)).html("No Result").attr("style", "color:black;");

                    ResultStatus.value = "N";
                }
                else {
                    ResultStatus.value = "P";
                    $("#spnResultStatus", $(this)).html("Precent").attr("style", "color:green;");
                }

                //else if(parseFloat(Mark.value)<parseFloat(MinimumMark))
                //{
                //    if(parseFloat(Mark.value)<parseFloat(0))
                //    {
                //        Mark.value=0;
                //    }
                //    $("#spnResultStatus",$(this)).html("Fail").attr("style", "color:red;");

                //    ResultStatus.value = "F";
                //}
                //else if(parseFloat(Mark.value)>=parseFloat(MinimumMark))
                //{
                //    if(parseFloat(Mark.value)>parseFloat(MaximumMark))
                //    {
                //        Mark.value=MaximumMark;
                //    }
                //    $("#spnResultStatus",$(this)).html("Pass").attr("style", "color:green;");

                //    ResultStatus.value = "P";
                //}
            }

        });

    }

    var printExamResult = function () {
        
        var postData = {};
        var valid = true;
        postData.SearchName = $('#SearchName').val();
        postData.SearchPhone = $('#SearchPhone').val()
        postData.SearchFromDate = $('#SearchFromDate').val()
        postData.SearchToDate = $('#SearchToDate').val()
        postData.SearchDistrictKey = $('#SearchDistrictKey').val()
        postData.SearchBranchKey = $('#SearchBranchKey').val()
        postData.SearchScholarshipTypeKey = $('#SearchScholarshipTypeKey').val()
        postData.SearchSubBranchKey = $('#SubBranchKey').val()
        postData.ScheduleStatus = $("#tab-componets li a.active").data('val')
        postData.sidx = "RowKey"; postData.sord = "asc"; postData.page = 0; postData.rows = 0;
        var validator = $("form").validate();
        valid = valid && validator.element('#SearchBranchKey');
        valid = valid && validator.element('#SearchScholarshipTypeKey');
        if (valid) {
            $.ajax({
                type: "POST",
                url: $("#hdnGetScholarshipExamResult").val(),
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(postData),
                success: function (response) {
                    var obj = {};
                    //obj.Title = $('#SearchScholarshipTypeKey').find("option:selected").text();
                    //obj.SubTitle = $('#SearchBranchKey').find("option:selected").text();
                    obj.FileName = $('#SearchScholarshipTypeKey').find("option:selected").text() + "-" + $('#SearchBranchKey').find("option:selected").text();

                    obj.JSONData = response.rows;
                    var columnModel = [
                    { name: 'ScholarShipName', index: 'ScholarShipName', headertext: Resources.Name },
                    { name: 'MobileNumber', index: 'MobileNumber', headertext: Resources.MobileNo },
                    { name: 'ExamRegNo', index: 'ExamRegNo', headertext: Resources.RegNo },
                    { name: 'Mark', index: 'Mark', headertext: "Mark" }
                    ];
                    obj.JSONData = $(obj.JSONData).map(function (n, item) {
                        item.Signature = null;
                        return item
                    })
                    obj.ColumnNames = columnModel;
                    //AppCommon.PrintJSON(obj);
                    AppCommon.ExportJSONToExcel(obj);


                },
                error: function (request, status, error) {

                }
            });
        }
    }


    var fileUploadChange = function () {
        //Get the files from Upload control
        
        var _this = $('#ExcelFile')[0];
        $(".section-content").mLoading();
        var files = _this.files;
        var i, f;
        var result;
        if (files.length > 0) {
            for (i = 0, f = files[i]; i != files.length; ++i) {
                var reader = new FileReader();
                var name = f.name;
                reader.onload = function (e) {
                    var data = e.target.result;
                    var wb, arr;
                    function doit() {
                        try {
                            wb = XLSX.read(data, { type: 'binary', dateNF: 'dd/MM/yyyy' });
                            var JsonData = process_wb(wb)

                            var Headers = JsonData[0];
                            //Headers.splice(0, 0, "RowKey");
                            //Headers.splice(Headers.length, 0, "");
                            JsonData.shift();
                            WrongData = $(JsonData).filter(function (i, n) {
                                return n[3] && !parseFloat(n[3]);
                            }).map(function (n, item) {
                                return item[2]
                            }).toArray();
                            if (WrongData && WrongData.length > 0) {
                                //alert('Invalid ExcelSheet on Exam RegNo :' + WrongData.join(','));
                                EduSuite.AlertMessage({ title: Resources.Warning, content: 'Invalid ExcelSheet on Exam RegNo :' + WrongData.join(','), type: 'orange' })
                                $(".section-content").mLoading("destroy");
                                return false;
                            }

                            JsonData = $(JsonData).filter(function (i, n) {
                                return !n[3] || parseFloat(n[3]);
                            });

                            JsonData = GetExcelData(Headers, JsonData)
                            var url = $("#hdnAddBulkScholarshipExamResult").val();
                            $.ajax({
                                type: "POST",
                                data: { modelList: JsonData },
                                url: url,
                                success: function (response) {
                                    if (typeof response == "string") {
                                        $("[data-valmsg-for=error_msg]").html(response);
                                    }
                                    else if (response.IsSuccessful) {
                                        toastr.success(Resources.Success, response.Message);
                                        $("#frmScholarshipExamResult").closest(".modal").modal("hide")
                                        $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
                                    }
                                    $(".section-content").mLoading("destroy");
                                },
                                error: function () {
                                    $(".section-content").mLoading("destroy");
                                }
                            });

                            //var form = $("form");
                            //var url = $(form).attr("action");
                            //var type = $(form).attr("method");
                            //var modelListData = JsonData;
                            //var response = AjaxHelper.ajax(type, url, {
                            //    modelList: modelListData
                            //});
                        } catch (e) { console.log(e); }
                    }

                    if (e.target.result.length > 1e6) opts.errors.large(e.target.result.length, function (e) { if (e) doit(); });
                    else { doit(); }
                };
                reader.readAsBinaryString(f);

            }
        }
        else {
            $(".section-content").mLoading("destroy");
        }
    }
    function GetExcelData(headerData, data) {

        var Leads = [];
        $.each(data, function (p) {
            var leadItem = this;
            var newLead = {};
            headerData.forEach(function (item, i) {
                if (i <= 10) {
                    switch (i) {
                        case 2:
                            newLead["ExamRegNo"] = leadItem[i] ? leadItem[i] : "";
                            break;
                        case 3:
                            newLead["Mark"] = leadItem[i] ? leadItem[i] : null;
                            break;

                    }
                }
                newLead["RowKey"] = 0;
                newLead["AbsentStatus"] = false;
            })
            Leads.push(newLead);
        })


        return Leads;
    }







    return {
        GetScholarshipExamResult: getScholarshipExamResult,
        GetScholarshipExamResultDetails: getScholarshipExamResultDetails,
        SetExamResultStatus: setExamResultStatus,
        PrintExamResult: printExamResult,
        FileUploadChange: fileUploadChange

    }
}());

function deleteScholarship(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Scholarship,
        actionUrl: $("#hdnDeleteScholarship").val(),
        actionValue: rowkey,
        dataRefresh: function () { $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); }
    });
}

function ScholarshipPopupLoad() {
    $(".selectpicker").selectpicker()
    $("#DistrictKey").on("change", function () {

        var obj = {};
        obj.id = $(this).val() != "" ? $(this).val() : 0;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetBranchByDistrict").val(), $("#BranchKey"), Resources.center, "Branches");

    });
}

function ScholarshipExamResultPopupLoad() {

    ScholarshipExamResult.SetExamResultStatus();
    $("input[id*=AbsentStatus]", $("#DivScholarshipExamResult")).change(function () {

        ScholarshipExamResult.SetExamResultStatus();
    });

    $("input[id*=Mark]", $("#DivScholarshipExamResult")).keydown(function (e) {
        
        if (e.keyCode == '40') {
            var tr = $(this).closest(tr).next();
            $("input[id*=Mark]", $(tr)).focus();
        }
        if (e.keyCode == '38') {
            var tr = $(this).closest(tr).prev();
            $("input[id*=Mark]", $(tr)).focus();
        }

    });

    $("input[id*=Mark]", $("#DivScholarshipExamResult")).on('input', function (e) {
        ScholarshipExamResult.SetExamResultStatus();
    })
}



