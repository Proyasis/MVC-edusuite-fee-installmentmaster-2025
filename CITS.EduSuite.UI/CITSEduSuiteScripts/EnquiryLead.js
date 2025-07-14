var modelData = [], request = null, defaultTelephoneCodes = ["+91", "091", "91"];
var EnquiryLead = (function () {
    var getEnquiryReferenceList = function (json) {
        $('.repeater').repeater(
            {
                show: function () {
                    $(this).slideDown();
                    var item = $(this);
                    //$("[id*=btndwnld]", $(this)).remove();
                    AppCommon.CustomRepeaterRemoteMethod();
                    AppCommon.FormatInputCase();
                    AppCommon.FormatDateInput();

                    $("[id*=TelephoneCodeKey]", $(this)).val(20);

                },
                hide: function (remove) {
                    //var self = $(this).closest('[data-repeater-item]').get(0);
                    //var hidden = $(self).find('input[type=hidden]')[0];
                    //if ($(hidden).val() != "" && $(hidden).val() != "0")
                    //{
                    //    //deleteApplicationExperience($(hidden).val());
                    //}
                    //else
                    //{
                    $(this).slideUp(remove);
                    //}

                },
                rebind: function (response) {
                    if (typeof response == "string") {
                        $("[data-valmsg-for=error_msg]").html(response);
                    }
                    else if (response.IsSuccessful) {
                        toastr.success(Resources.Success, response.Message);
                        $("#tab-application li.active").parent().next('li').find('a').trigger('click');
                    }

                },
                data: json,
                repeatlist: 'ReferenceList',
                submitButton: '#btnSave',
                hasFile: true
            });
    }
    var getEnquiryLeads = function (json, pageIndex, resetPagination) {
        $("#dvRepeaterList").empty();
        $("#dvScheduleContainer").mLoading();
        JsonData = json;
        JsonData["SearchName"] = $("#SearchName").val();
        JsonData["SearchPhone"] = $("#SearchPhone").val();
        JsonData["SearchEmail"] = $("#SearchEmail").val();
        JsonData["SearchFromDate"] = $("#SearchFromDate").val();
        JsonData["SearchToDate"] = $("#SearchToDate").val();
        JsonData["SearchCallStatusKey"] = $("#SearchCallStatusKey").val();
        JsonData["SearchBranchKey"] = $("#SearchBranchKey").val();
        JsonData["SearchEmployeeKey"] = $("#SearchEmployeeKey").val();
        JsonData["SearchEnquiryLeadStatusKey"] = $("#SearchEnquiryLeadStatusKey").val();
        JsonData["NatureOfEnquiryKey"] = $("#NatureOfEnquiryKey").val();

        JsonData["SearchLocation"] = $("#SearchLocation").val();
        JsonData["PageIndex"] = pageIndex ? pageIndex : 1;
        JsonData["PageSize"] = 10;
        request = $.ajax({
            url: $("#hdnGetEnquiryLeads").val(),
            data: JsonData,
            datatype: "json",
            type: "post",
            contenttype: 'application/json; charset=utf-8',
            async: true,
            beforeSend: function () {
                if (request != null) {
                    request.abort();
                }
            },
            success: function (data) {
                $("#dvRepeaterList").html(data);
                $("#TotalRecords").html($("#hdnTotalRecords").val());
                if (resetPagination) {
                    EnquiryLeadPagination(JsonData);
                }
                $("#dvScheduleContainer").mLoading("destroy");
            },
            error: function (xhr) {
                console.log(xhr.responseText);
                //  $("#dvScheduleContainer").mLoading("destroy");
            }

        });

    }

    var getEnquiryLeadFeadbacks = function (_this, url) {
        $(_this).mLoading();
        setTimeout(function () {
            $.ajax({
                url: url,
                datatype: "json",
                type: "GET",
                contenttype: 'application/json; charset=utf-8',
                async: false,
                success: function (data) {
                    $(_this).html(data);
                    $(_this).mLoading("destroy");
                },
                error: function (xhr) {
                    $(_this).mLoading("destroy");
                }
            });
        }, 500);
    }

    var editLeadFeedback = function (key, LeadKey) {
        var obj = {};
        obj.id = key;
        obj.LeadKey = LeadKey;
        $("#dvAddEditLeadFeedback").load($("#hdnAddEditEnquiryLeadFeedback").val() + "?" + $.param(obj));
    }

    var formSubmitLeadFeedback = function () {
        var form = $("#frmEnquiryLeadFeedback");
        var validate = $(form).valid();
        if (validate) {
            $.ajax({
                url: $(form)[0].action,
                type: $(form)[0].method,
                data: $(form).serialize(),
                success: function (result) {
                    if (result.IsSuccessful) {
                        var collapse = $(form).closest(".collapse");
                        var item = $(form).closest("[data-repeater-item]");
                        var url = $("[data-toggle='collapse']", $(item)).data("url");
                        EnquiryLead.GetEnquiryLeadFeadbacks(collapse, url);

                    } else {

                    }
                }
            });
        }
    }

    var deleteEnquiryLead = function (rowkey, IsBulk) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_EnquiryLead,
            actionUrl: $("#hdnDeleteEnquiryLead").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                if (!IsBulk) {
                    window.location.reload();
                }
                else {
                    ValidateMultipleExcelData()
                }
            }
        });
    }

    var deleteEnquiryLeadFeedback = function (rowkey, _this) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_EnquiryLeadFeedback,
            actionUrl: $("#hdnDeleteEnquiryLeadFeedback").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                var collapse = $(_this).closest(".LeadFeedback");
                var item = $(_this).closest("[data-repeater-item]");
                var url = $("[data-toggle='collapse']", $(item)).data("url");
                EnquiryLead.GetEnquiryLeadFeadbacks(collapse, url);
            }
        });
    }

    var getEmployeesByBranchId = function (Id, ddl) {
        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetEmployeeByBranchId").val(), ddl, Resources.Employee);

    }

    var editFeedbackPopUp = function (_this) {
        var URL = $(_this).data("url");
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {
                EnquiryLeadFeedbackPopupReady();
            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    toastr.success(Resources.Success, result.Message);
                }
                else {
                    toastr.error(Resources.Failed, result.Message);
                }
                if (result.Feedback != "") {
                    if (result.IsUserBlocked == true) {
                        $.confirm({
                            title: Resources.Warning,
                            content: result.Feedback,
                            type: 'red',
                            buttons: {
                                Ok: {
                                    text: 'Ok',
                                    btnClass: 'btn-danger',
                                    action: function () {
                                        window.location.href = $("#hdnLogin").val();
                                    }
                                }
                            }
                        });
                    }

                    toastr.warning(Resources.Warning, result.Feedback);
                }



                //if (isChild) {
                // var Container = $(_this).closest(".LeadFeedback");
                // $(Container).load($("[data-target='#" + $(Container).prop("id") + "']").data("url"));
                //}
                if (typeof EnquirySchedule != 'undefined') {
                    EnquirySchedule.GetEnquirySchedules(null, 1, true, 0)
                } else {
                    EnquiryLead.GetEnquiryLeads(jsonData);
                }

            },
            formAction: $("#hdnAddEditEnquiryLeadFeedback").val()
        }, URL);

    }

    var getCallStatusByEnquiryStatus = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnGetCallStatusByEnquiryStatus").val(), ddl, Resources.CallStatus);
    }

    var checkCallStatusDuration = function (Id) {

        var response = AjaxHelper.ajax("GET", $("#hdnCheckCallStatusDuration").val() + "/" + Id);
        $("#IsDuration").val(response);
    }

    //Bulk Lead

    var getPendingLeadsById = function (json, index) {
        $(".section-content").mLoading();
        modelData = json;
        var pageSize = 50;
        json["PageIndex"] = index;
        json["PageSize"] = pageSize;
        var colHeaders = ["RowKey", "Name", "TelephoneCodeKey", "MobileNumber", "EmailAddress", "Qualification", "EmployeeCode", "Remarks", "LeadDate", "LeadFrom"];
        //var colHeaders = ["RowKey", "Name", "MobileNumber", "EmailAddress", "Qualification", "Employee", "Remarks", "LeadDate", "LeadFrom"];

        var response = AjaxHelper.ajaxAsync("POST", $("#hdnGetPendingLeadById").val(),
            { model: json },
            function () {
                response = this
                modelData["Employees"] = response["Employees"];
                modelData["TelephoneCodes"] = response["TelephoneCodes"];

                var TotalRecods = parseInt(response["TotalRecords"]);
                $("#TotalRecords").html(TotalRecods.toString())
                var modifiedData = ModifyData(response["Leads"], colHeaders);
                colHeaders[2] = "TelephoneCode";
                if (index > 1) {
                    EnquiryLead.LoadMorePendingData(modifiedData, colHeaders);
                }
                else {
                    EnquiryLead.SetExcelSheelValue(modifiedData, response["Employees"], response["TelephoneCodes"], colHeaders);
                    //EnquiryLead.SetExcelSheelValue(modifiedData, response["Employees"], colHeaders);
                }
                if (index * pageSize >= TotalRecods) {
                    $("#btnMore").hide();
                }
                else {
                    index = parseInt(index) + 1;
                    $("#btnMore").attr("data-index", index.toString());
                    $("#btnMore").show();
                }
            }
        );

    }

    var loadMorePendingData = function (json) {
        var data = $("#Excel").jexcel('getData', false);
        var lastIndex = data.length;
        data.push.apply(data, json);
        $("#Excel").jexcel('setData', data);
        //var calls = []
        //$('#Excel tbody tr:nth-child(' + lastIndex + ') td').each(function () {
        //    ValidateExcelSheet(this, $(this).html(), calls)
        //});
        //if (calls.length > 0) {
        //    $.when.apply($, calls).then(function () {
        //        $(".section-content").mLoading("destroy");
        //    });
        //} else {
        //    $(".section-content").mLoading("destroy");
        //}
    }

    var setExcelSheelValue = function (Data, Employees, TelephoneCodes, colHeaders, newData) {
        var employeesList = [], telephoneList = [];
        var obj = {};
        obj.id = "";
        obj.name = "Select";
        employeesList.push(obj)
        $(Employees).each(function () {
            var obj = {};
            obj.id = this.RowKey;
            obj.name = this.Text;
            employeesList.push(obj)
        })
        $(TelephoneCodes).each(function () {
            var obj = {};
            obj.id = this.RowKey;
            obj.name = this.Text;
            telephoneList.push(obj)
            if (defaultTelephoneCodes.indexOf(this.Text) > -1) {
                defaultTelephone = this.Text;
            }
        })
        var obj = {};
        obj.id = "";
        obj.name = "Select";

        $('#Excel').jexcel({
            data: Data,
            // colHeaders: JsonHead,
            columnSorting: false,
            allowInsertRow: true,
            // Allow row delete
            allowDeleteRow: true,
            colHeaders: colHeaders,
            colWidths: [0, 150, 50, 100, 100, 100, 80, 150, 100, 100, 100, 30],
            columns: [
                { type: 'number' },
                { type: 'text' },
                { type: 'dropdown', source: telephoneList },
                { type: 'number', editorRegx: /[^0-9 .]/g },
                { type: 'text' },
                { type: 'text' },
                { type: 'dropdown', source: employeesList },
                { type: 'text' },
                { type: 'calendar', option: { autoclose: true, format: 'dd/mm/yyyy' } },
                { type: 'text' },
                { type: 'checkbox' },

            ],
            onbeforechange: function (_this, cell, old) {
                var value = $(_this)[0].newValue;
                ValidateSingleExcelData(cell, value);
            },
            ondeleterow: function (_this, data) {
                var id = data.split(',')[0];
                if (id && id != "" && id != "0") {
                    var response = AjaxHelper.ajax("POST", $("#hdnDeleteEnquiryLead").val(),
                        {
                            id: id
                        });
                    if (response.Message) {
                        if (response.Message === Resources.Success) {
                            toastr.success(Resources.Success, response.Message);

                        }
                        else
                            toastr.error(Resources.Failed, response.Message);
                    }
                }
            },
            onafterdeleterow: function (_this, data) {
                var calls = []
                $(".section-content").mLoading()
                ValidateMultipleExcelData();
                $("#TotalRecords").html(data.length)
            }
        });
        var val0 = "", val2 = "";
        $('#Excel').jexcel('updateSettings', {
            cells: function (cell, col, row) {
                var val = $(cell).html();
                if (col == 0) {
                    var header = $(cell).closest("table").find("tr").eq(0);
                    $(cell).hide();
                    $($(header).find("td").eq(1)).hide();

                }
                else if (val == "" || val == "0") {
                    $(cell).html("")
                }
                //var cellText = $(cell)[0].innerText.trim()
                //if (col == 2 && (cellText == "" || /<(?=.*? .*?\/ ?>|br|hr|input|!--|wbr)[a-z]+.*?>|<([a-z]+).*?<\/\1>/i.test(cellText)) && (val == "" || /<(?=.*? .*?\/ ?>|br|hr|input|!--|wbr)[a-z]+.*?>|<([a-z]+).*?<\/\1>/i.test(val))) {
                //    $(cell).html(defaultTelephone)
                //}

            }
        });
        if (newData) {
            ValidateMultipleExcelData();
        }
        else {
            $(".section-content").mLoading("destroy");
        }

    }

    var downloadExcel = function () {
        var data = [];
        var ws_name = "Lead Sheet";
        data.push($("#Excel").jexcel('getHeaderData'))
        var newData = $("#Excel").jexcel('getData', false);
        //$(newData).each(function () {
        //    data.push($(this));
        //})
        delete data[0][0];
        data[0].shift();
        var wb = new Workbook(), ws = sheet_from_array_of_arrays(data);
        /* add worksheet to workbook */
        wb.SheetNames.push(ws_name);
        wb.Sheets[ws_name] = ws;
        var wbout = XLSX.write(wb, { bookType: 'xlsx', bookSST: true, type: 'binary' })
        saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), "LeadSheet.xlsx")

    }

    var fileUploadChange = function (e) {
        //Get the files from Upload control
        $(".section-content").mLoading();
        var files = e.target.files;
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
                            Headers.splice(0, 0, "RowKey");
                            Headers.splice(Headers.length, 0, "");
                            JsonData.shift();
                            JsonData = $(JsonData).filter(function (i, n) {
                                return n[0] && this.splice(0, 0, "0") && this.splice(Headers.length, 0, "0");
                            });
                            JsonData = $(JsonData).each(function () {
                                if (!this[2] || this[2] === "") {
                                    var defaultTelephoneCode = modelData["TelephoneCodes"].filter(function (n, p) {
                                        return defaultTelephoneCodes.indexOf(n.Text) > -1;
                                    })[0]
                                    this[2] = defaultTelephoneCode ? defaultTelephoneCode.RowKey : "";
                                }
                                else {
                                    var telephoneCode = this[2];
                                    var currentTelephoneCode = modelData["TelephoneCodes"].filter(function (n, p) {
                                        return n.Text == telephoneCode;
                                    })[0]
                                    if (currentTelephoneCode) {
                                        this[2] = currentTelephoneCode.RowKey;

                                    }
                                    else {
                                        currentTelephoneCode = modelData["TelephoneCodes"].filter(function (n, p) {
                                            return defaultTelephoneCodes.indexOf(n.Text) > -1;
                                        })[0]
                                        this[2] = currentTelephoneCode.RowKey;
                                    }
                                }
                                if (this[6]) {
                                    var employeeCode = this[6];
                                    var currentEmployee = modelData["Employees"].filter(function (n, p) {
                                        return n.GroupName.toLowerCase() == employeeCode.toLowerCase();
                                    })[0]
                                    this[6] = currentEmployee ? currentEmployee.RowKey : "";
                                }
                                else {
                                    this[6] = "";
                                }
                                if (this[8]) {
                                    var reg = /((([0][1-9]|[12][\d])|[3][01])[/]([0][13578]|[1][02])[/][1-9]\d\d\d)|((([0][1-9]|[12][\d])|[3][0])[/]([0][13456789]|[1][012])[/][1-9]\d\d\d)|(([0][1-9]|[12][\d])[/][0][2][/][1-9]\d([02468][048]|[13579][26]))|(([0][1-9]|[12][0-8])[/][0][2][/][1-9]\d\d\d)/;
                                    var result = reg.test(this[8]);
                                    if (!result) {
                                        this[8] = "";
                                    }

                                }

                                if (this[3] && this[3] != "") {
                                    this[3] = this[3].slice(-10)
                                }
                            });
                            EnquiryLead.SetExcelSheelValue(JsonData, modelData["Employees"], modelData["TelephoneCodes"], Headers, true)
                            $("#TotalRecords").html(JsonData.length)
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

    var updateBulkLead = function () {
        var errorLength = $('#Excel tbody tr td.invalid').length;
        if (errorLength == 0) {
            $(".section-content").mLoading();


            //setTimeout(function () {

            var form = $("form");
            var url = $(form).attr("action");
            var type = $(form).attr("method");
            var modelListData = GetExcelData(modelData);
            //var response = AjaxHelper.ajax(type, url, {
            //    modelList: modelListData
            //});
            $.ajax({
                type: type,
                url: url,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ modelList: modelListData }),

                success: function (response) {
                    if (response.IsSuccessful == true) {
                        window.location.href = $("#hdnEnquiryLeadList").val();
                    }
                    else {
                        $("#error_msg").html(response.Message);
                    }
                    $(".section-content").mLoading("destroy");
                },
                error: function (request, status, error) {

                }
            });
            //if (response.IsSuccessful == true) {
            //    window.location.href = $("#hdnEnquiryLeadList").val();
            //}
            //else {
            //    $("#error_msg").html(response.Message);
            //}
            //$(".section-content").mLoading("destroy");

            //}, 1000)
        }
        else {
            $('#Excel').jexcel('openEditor', $('#Excel tbody tr td.invalid:first'));
        }
    }

    var getLeadByText = function (object) {
        var obj = {};
        $("#LeadLink").remove();
        $("<a id='LeadLink' href='#'></a>").insertBefore(object);

        if (object.value == "") {
            $("#LeadList").remove();
        }
        else {
            obj.SearchText = object.value;
            $("#SearchEnquiryLeadStatusKey").val("");
            $.ajax({
                url: $("#hdnGetEnquiryLead").val(),
                type: "GET",
                dataType: "JSON",
                data: obj,
                contentType: "application/json; charset=utf-8",
                success: function (List) {
                    LeadDataList = List;
                    $("#LeadList").remove();
                    $("<ul class='AutoCompleteList' id='LeadList'></ul>").insertAfter(object);
                    BindLeadList(object)

                }
            });
        }
    }

    var deleteBulkEnquiryLead = function () {
        var inputs = $("#Excel input:checkbox:checked");
        var rowKeys = [];
        $(inputs).each(function (i) {
            var row = $(this).closest("tr");
            var cellFirst = $(row).find("td").eq(0);
            var Id = $(cellFirst).prop('id');
            if (Id) {
                Id = Id.split('-');
                rowKeys.push(Id[1]);

            }
        })
        if (rowKeys.length > 0) {
            $("#Excel").jexcel("deleteMultipleRow", rowKeys)
        }
    }

    function BindLeadList(object) {
        for (i = 0; i < LeadDataList.length; i++) {
            if (LeadDataList[i].EnquiryLeadStaffName != null) {
                $("#LeadList").append("<li onClick='FetchLead(" + i + ")' class='ListItem'>" + LeadDataList[i].Name + " (" + LeadDataList[i].EnquiryLeadStaffName + ") </li>");
            }
        }
    }

    var getPhoneNumberLength = function () {        

        var response = AjaxHelper.ajax("GET", $("#hdnGetPhoneNumberLength").val() + "?TelephoneCodeKey=" + $("#TelephoneCodeKey").val() + "&TelephoneCodeOptionalKey=" + $("#TelephoneCodeOptionalKey").val());
        //var response = AjaxHelper.ajax("GET", $("#hdnGetPhoneNumberLength").val() + "?MobileNumber=" + mobilenumber + "&TelephoneCodeKey=" + TelephoneCodeKey)

        $("[id*=MinPhoneLength]").val(response.MinPhoneLength);
        $("[id*=MaxPhoneLength]").val(response.MaxPhoneLength);
        $("[id*=MinPhoneLengthOptional]").val(response.MinPhoneLengthOptional);
        $("[id*=MaxPhoneLengthOptional]").val(response.MaxPhoneLengthOptional);
    }


    return {
        GetEnquiryLeads: getEnquiryLeads,
        GetEnquiryLeadFeadbacks: getEnquiryLeadFeadbacks,
        EditLeadFeedback: editLeadFeedback,
        FormSubmitLeadFeedback: formSubmitLeadFeedback,
        DeleteEnquiryLead: deleteEnquiryLead,
        DeleteEnquiryLeadFeedback: deleteEnquiryLeadFeedback,
        GetEmployeesByBranchId: getEmployeesByBranchId,
        EditFeedbackPopUp: editFeedbackPopUp,
        GetCallStatusByEnquiryStatus: getCallStatusByEnquiryStatus,
        SetExcelSheelValue: setExcelSheelValue,
        GetPendingLeadsById: getPendingLeadsById,
        DownloadExcel: downloadExcel,
        UpdateBulkLead: updateBulkLead,
        FileUploadChange: fileUploadChange,
        LoadMorePendingData: loadMorePendingData,
        CheckCallStatusDuration: checkCallStatusDuration,
        GetLeadByText: getLeadByText,
        DeleteBulkEnquiryLead: deleteBulkEnquiryLead,
        GetEnquiryReferenceList: getEnquiryReferenceList,
        GetPhoneNumberLength: getPhoneNumberLength,
    }

}());


function EnquiryLeadPagination(jsonData) {
    $('#page-selection-up,#page-selection-down').empty();
    var totalRecords = $("#hdnTotalRecords").val();
    totalRecords = totalRecords != "" ? parseInt(totalRecords) : 0;
    var Size = jsonData["PageSize"];
    var totalPages = Math.floor(totalRecords % Size == 0 ? totalRecords / Size : (totalRecords / Size) + 1);

    $('#page-selection-up,#page-selection-down').bootpag({
        total: totalPages,
        page: 1,
        maxVisible: 10
    });

    $('#page-selection-up').on("page", function (event, num) {
        EnquiryLead.GetEnquiryLeads(JsonData, num);

    });
}

//Bulk Lead

function ModifyData(Json, colHeaders) {
    var resultData = []
    $(Json).each(function () {
        var jsonItem = $(this)[0];
        var resultItem = [];
        colHeaders.forEach(function (item) {

            if (item == "LeadDate") {
                jsonItem[item] = jsonItem[item]
            }
            resultItem.push(AppCommon.JsonDateToNormalDate2(jsonItem[item]));
        })
        resultData.push(resultItem);
    })
    return resultData;
}

function GetExcelData(jsonData) {
    var headerData = $("#Excel").jexcel('getHeaderData');
    var data = $("#Excel").jexcel('getData', false);
    var Leads = [];
    $.each(data, function (p) {
        var leadItem = this;
        var newLead = {};
        if (leadItem[3] != "") {
            headerData.forEach(function (item, i) {
                if (i <= 10) {
                    switch (i) {
                        case 0:
                            newLead["RowKey"] = leadItem[i] ? leadItem[i] : 0;
                            break;
                        case 1:
                            newLead["Name"] = leadItem[i] ? leadItem[i] : "";
                            break;
                        case 2:
                            newLead["TelephoneCodeKey"] = leadItem[i] ? leadItem[i] : 0;
                            break;
                        case 3:
                            newLead["MobileNumber"] = leadItem[i] ? leadItem[i] : "";
                            break;
                        case 4:
                            newLead["EmailAddress"] = leadItem[i] ? leadItem[i] : null;
                            break;
                        case 5:
                            newLead["Qualification"] = leadItem[i] ? leadItem[i] : null;
                            break;
                        case 6:
                            newLead["EmployeeKey"] = leadItem[i] ? leadItem[i] : null;
                            break;
                        case 7:
                            newLead["Remarks"] = leadItem[i] ? leadItem[i] : null;
                            break;
                        case 8:
                            newLead["LeadDate"] = leadItem[i] ? leadItem[i] : null;
                            break;
                        case 9:
                            newLead["LeadFrom"] = leadItem[i] ? leadItem[i] : null;
                            break;


                    }
                }
            })
        }
        newLead["IsNewLead"] = 0;
        newLead["BranchKey"] = $("#BranchKey").val();
        Leads.push(newLead);
    });
    return Leads;
}

function ValidateExcelSheet(cell, value, objParam) {

    var mobileColumns = ["2", "3"]

    var id = $(cell).prop("id").split("-")[0];
    var row = $(cell).closest("tr");
    var rowIndex = $(row).index();
    var cellRow = $("td", $(row)).eq(1);
    var rowKey = $(cellRow).html();
    rowKey = parseInt(rowKey) ? rowKey : 0;
    var exelDatas = [];

    value = value ? value.trim() : "";
    var telephoneCodeCell = $("td", $(row)).eq(3)
    var telephoneCode = $("#Excel").jexcel('getValue', $(telephoneCodeCell));
    telephoneCode = id == 2 ? value : telephoneCode;
    telephoneCode = parseInt(telephoneCode) ? parseInt(telephoneCode) : 0;

    var mobileNumberCell = $("td", $(row)).eq(4)
    var mobileNumber = $("#Excel").jexcel('getValue', $(mobileNumberCell));
    mobileNumber = id == 3 ? value : mobileNumber;

    var emailAddressCell = $("td", $(row)).eq(5)
    var emailAddress = $("#Excel").jexcel('getValue', $(emailAddressCell));
    emailAddress = id == 4 ? value : emailAddress;

    var qualificationCell = $("td", $(row)).eq(6)
    var qualification = $("#Excel").jexcel('getValue', $(qualificationCell));
    qualification = id == 5 ? value : qualification;

    var staffLoginIdCell = $("td", $(row)).eq(7)
    var staffLoginId = $("#Excel").jexcel('getValue', $(staffLoginIdCell));
    staffLoginId = id == 6 ? value : staffLoginId;
    staffLoginId = parseInt(staffLoginId) ? parseInt(staffLoginId) : 0;

    var leadDateCell = $("td", $(row)).eq(9)
    var leadDate = $("#Excel").jexcel('getValue', $(leadDateCell));
    leadDate = id == 8 ? value : leadDate;




    if (mobileColumns.indexOf(id) > -1) {
        $('#Excel tbody tr').each(function () {
            //add item to array
            var elementValue
            var telephoneCodeCheck = $("#Excel").jexcel('getValue', $("td", $(this)).eq(3));
            var mobileNumberCheck = $("#Excel").jexcel('getValue', $("td", $(this)).eq(4));

            if (row[0].rowIndex != $(this)[0].rowIndex) {
                if (telephoneCode == telephoneCodeCheck && mobileNumber == mobileNumberCheck) {
                    exelDatas.push(mobileNumber);
                }
            }
        });

        var hidMobileNumber = $("#MobileNumber");
        if (id == 3) {
            var lengthMax = $(hidMobileNumber).data("val-length-max");
            var regex = new RegExp($(hidMobileNumber).data("val-regex-pattern"))

            if (value == "") {
                $(cell).attr("error-msg", $(hidMobileNumber).data("val-required"))
                $(cell).addClass("invalid")
            } else if (value.length > parseInt(lengthMax)) {
                $(cell).attr("error-msg", $(hidMobileNumber).data("val-length"))
                $(cell).addClass("invalid")
            }
            else if (value != "" && !regex.test(value)) {
                $(cell).attr("error-msg", $(hidMobileNumber).data("val-regex"))
                $(cell).addClass("invalid")
            }
            else {
                $(mobileNumberCell).removeClass("invalid")
            }

        }
        if (mobileNumber != "") {
            if (!$(cell).hasClass("invalid")) {
                var obj = {};
                obj.MobileNumber = mobileNumber;
                obj.RowKey = rowKey;
                obj.TelephoneCodeKey = telephoneCode;
                if (exelDatas.length > 0) {
                    $(mobileNumberCell).attr("error-msg", $(hidMobileNumber).data("val-remote"))
                    $(mobileNumberCell).addClass("invalid")
                }
                else {
                    var obj = {};// AppCommon.ClearArray($.extend({}, modelData, true));
                    obj.MobileNumber = mobileNumber;
                    obj.RowKey = rowKey;
                    obj.TelephoneCodeKey = telephoneCode;
                    obj.PageIndex = rowIndex;
                    objParam.checkMobileExistData.push(obj);
                    objParam.mobileNumberCell = mobileNumberCell;
                    //calls.push($.ajax({
                    //    url: $(hidMobileNumber).data("val-remote-url"),
                    //    data: obj,
                    //    datatype: "json",
                    //    type: "GET",
                    //    contenttype: 'application/json; charset=utf-8',
                    //    success: function (response) {
                    //        if (!response) {
                    //            $(mobileNumberCell).attr("error-msg", $(hidMobileNumber).data("val-remote"))
                    //            $(mobileNumberCell).addClass("invalid")
                    //        }
                    //        else {
                    //            $(mobileNumberCell).removeClass("invalid")
                    //        }

                    //        $(row).removeClass("invalid-row")
                    //        if ($(row).find("td.invalid").length > 0) {
                    //            $(row).addClass("invalid-row")
                    //        }

                    //    },
                    //    error: function (xhr) {
                    //        //alert('error');
                    //    }

                    //}));
                }
            }


        }

        $(row).removeClass("invalid-row")
        if ($(row).find("td.invalid").length > 0) {
            $(row).addClass("invalid-row")
        }
        else if (id == 4) {
            exelDatas = [];
            $('#Excel tbody tr').each(function () {
                //add item to array
                var elementValue
                var emailAddressCheck = $("#Excel").jexcel('getValue', $("td", $(this)).eq(5));
                if (row[0].rowIndex != $(this)[0].rowIndex) {
                    if (emailAddress == emailAddressCheck) {
                        exelDatas.push(elementValue);
                    }
                }
            });
            var hidEmailAddress = $("#EmailAddress");
            emailAddress = emailAddress.trim();
            if (id == 4) {
                var lengthMax = $(hidEmailAddress).data("val-length-max");
                var regex = new RegExp($(hidEmailAddress).data("val-regex-pattern"))
                if (value.length > parseInt(lengthMax)) {
                    $(cell).attr("error-msg", $(hidEmailAddress).data("val-length"))
                    $(cell).addClass("invalid")
                }
                else if (value != "" && !regex.test(value)) {
                    $(cell).attr("error-msg", $(hidEmailAddress).data("val-regex"))
                    $(cell).addClass("invalid")
                }
            }
            if (emailAddress != "") {
                if (!$(cell).hasClass("invalid")) {
                    if (exelDatas.length > 0) {
                        $(emailAddressCell).attr("error-msg", $(hidEmailAddress).data("val-remote"))
                        $(emailAddressCell).addClass("invalid")
                    }
                    else {
                        obj = {} //AppCommon.ClearArray($.extend({}, modelData, true));
                        obj.EmailAddress = emailAddress;
                        obj.RowKey = rowKey;
                        obj.ServiceTypeKey = parseInt(serviceType) ? parseInt(serviceType) : 0;
                        obj.PageIndex = rowIndex;
                        objParam.checkEmailExistData.push(obj);
                        objParam.emailAddressCell = emailAddressCell;
                        //calls.push($.ajax({
                        //    url: $(hidEmailAddress).data("val-remote-url"),
                        //    data: obj,
                        //    datatype: "json",
                        //    type: "GET",
                        //    contenttype: 'application/json; charset=utf-8',
                        //    success: function (response) {
                        //        if (!response) {
                        //            $(emailAddressCell).attr("error-msg", $(hidEmailAddress).data("val-remote"))
                        //            $(emailAddressCell).addClass("invalid")
                        //        }
                        //        else {
                        //            $(emailAddressCell).removeClass("invalid")
                        //        }

                        //        $(row).removeClass("invalid-row")
                        //        if ($(row).find("td.invalid").length > 0) {
                        //            $(row).addClass("invalid-row")
                        //        }

                        //    },
                        //    error: function (xhr) {
                        //        //alert('error');
                        //    }

                    }
                }
            }
        }
        else {
            $(emailAddressCell).removeClass("invalid")
        }

    }
    else if (id == "8") {
        var DateNow = new moment();
        var Date = value != "" ? new moment(value, "DD/MM/YYYY") : null;
        if (staffLoginId != "" && value == "") {
            $(cell).attr("error-msg", $("#LeadDate").data("val-required"));
            $(cell).addClass("invalid")
        }
        else if (Date && value != "" && Date.diff(DateNow, 'days') < 0) {
            $(cell).attr("error-msg", $("#LeadDate").data("val-is"))
            $(cell).addClass("invalid")
        }
        else {
            $(cell).removeClass("invalid")
        }
    }
    else if (id == "6") {

        if (value == "" || /<(?=.*? .*?\/ ?>|br|hr|input|!--|wbr)[a-z]+.*?>|<([a-z]+).*?<\/\1>/i.test(value)) {

            $(leadDateCell).html("");
            $(leadDateCell).removeClass("invalid").addClass("readonly")
        }
        else if (value != "" && leadDate == "") {
            $(leadDateCell).attr("error-msg", $("#LeadDate").data("val-required"));
            $(leadDateCell).addClass("invalid")
            $(leadDateCell).removeClass("readonly")
        }
        else {
            $(leadDateCell).removeClass("invalid")
            $(leadDateCell).removeClass("readonly")
        }
    }
    else if (id == "5") {
        if (value != "") {
            var maxLength = $("#Qualification").data("val-length-max");
            maxLength = parseInt(maxLength) ? parseInt(maxLength) : 0;
            if (value.length > maxLength) {
                $(cell).attr("error-msg", $("#Qualification").data("val-length"));
                $(cell).addClass("invalid")

            }
            else {
                $(cell).removeClass("invalid")

            }
        }
    }
    $(row).removeClass("invalid-row")
    if ($(row).find("td.invalid").length > 0) {
        $(row).addClass("invalid-row")
    }

}

function ValidateMultipleExcelData() {
    var objParam = {};
    objParam.checkMobileExistData = [];
    objParam.checkEmailExistData = [];
    objParam.checkMobileOptionalExistData = [];
    var checkColumnIndexes = ["3", "4", "6"]
    $('#Excel tbody tr').each(function () {
        $(this).find("td").each(function (i) {
            var id = $(this).prop("id").split("-")[0];
            if (checkColumnIndexes.indexOf(id) > -1) {
                var value = $("#Excel").jexcel('getValue', $(this));
                ValidateExcelSheet(this, value, objParam);
            }
        })

    });
    $.ajax({
        url: $("#hdnCheckBulkLeadExists").val(),
        data: { DuplicateMobileNumbers: objParam.checkMobileExistData, DuplicateEmails: objParam.checkEmailExistData },
        datatype: "json",
        type: "POST",
        contenttype: 'application/json; charset=utf-8',
        success: function (response) {
            var hidMobileNumber = $("#MobileNumber");
            var hidEmailAddress = $("#EmailAddress");
            $(response.DuplicateMobileNumbers).each(function () {
                var row = $('#Excel tbody tr').eq(this.PageIndex);
                var MobileNumberCell = $("td", $(row)).eq(4);
                $(MobileNumberCell).attr("error-msg", $(hidMobileNumber).data("val-remote"));
                $(MobileNumberCell).addClass("invalid");

                if ($(row).find("td.invalid").length > 0) {
                    $(row).addClass("invalid-row");
                }

            });
            $(response.DuplicateMobileOptionals).each(function () {
                var row = $('#Excel tbody tr').eq(this.PageIndex);
                var MobileNumberOptionalCell = $("td", $(row)).eq(7);
                $(MobileNumberOptionalCell).attr("error-msg", $(hidMobileNumber).data("val-remote"));
                $(MobileNumberOptionalCell).addClass("invalid");

                if ($(row).find("td.invalid").length > 0) {
                    $(row).addClass("invalid-row");
                }

            });
            $(response.DuplicateEmails).each(function () {
                var row = $('#Excel tbody tr').eq(this.PageIndex);
                var EmailAddressCell = $("td", $(row)).eq(5);
                $(EmailAddressCell).attr("error-msg", $(hidEmailAddress).data("val-remote"));
                $(EmailAddressCell).addClass("invalid");
                $(row).removeClass("invalid-row");
                if ($(row).find("td.invalid").length > 0) {
                    $(row).addClass("invalid-row");
                }

            });
            $(".section-content").mLoading("destroy")
        },
        error: function (xhr) {
            $(".section-content").mLoading("destroy")
        }

    });
}

function ValidateSingleExcelData(cell, value) {
    var objParam = {};
    objParam.checkMobileExistData = [];
    objParam.checkEmailExistData = [];
    var hidMobileNumber = $("#MobileNumber");
    var hidEmailAddress = $("#EmailAddress");
    ValidateExcelSheet(cell, value, objParam);
    var url;
    var PostData;
    var CurrColumn;
    if (objParam.emailAddress) {
        CurrColumn = objParam.emailAddress;
        urlCntrl = $(hidEmailAddress);
        PostData = objParam.checkEmailExistData[0];
    }
    else {
        if (objParam.mobileNumberCell) {
            CurrColumn = objParam.mobileNumberCell;
            PostData = objParam.checkMobileExistData[0];
        }
        else if (objParam.mobileNumberOptionalCell) {
            CurrColumn = objParam.mobileNumberOptionalCell;
            PostData = objParam.checkMobileOptionalExistData[0];
        }

        urlCntrl = $(hidMobileNumber);

    }

    if (PostData) {
        $.ajax({
            url: $(urlCntrl).data("val-remote-url"),
            data: PostData,
            datatype: "json",
            type: "GET",
            contenttype: 'application/json; charset=utf-8',
            success: function (response) {
                if (!response) {
                    $(CurrColumn).attr("error-msg", $(urlCntrl).data("val-remote"));
                    $(CurrColumn).addClass("invalid");
                }
                else {
                    $(CurrColumn).removeClass("invalid");
                }
                var row = $(CurrColumn).closest("tr");
                $(row).removeClass("invalid-row");
                if ($(row).find("td.invalid").length > 0) {
                    $(row).addClass("invalid-row");
                }
                $(".section-content").mLoading("destroy")

            },
            error: function (xhr) {
                $(".section-content").mLoading("destroy")
                //alert('error');
            }

        });
    }

}

var LeadDataList;

function FetchLead(index) {
    $("#LeadList").remove();
    $("#SearchName").val(LeadDataList[index].Name);
    $("#SearchEnquiryLeadStatusKey").val(LeadDataList[index].EnquiryLeadStatusKey);
    $("#SearchBranchKey").val(LeadDataList[index].BranchKey);
    $("#SearchEmployeeKey").val(LeadDataList[index].SearchEmployeeKey);
    EnquiryLead.GetEnquiryLeads(jsonData);
    $("#SearchEnquiryLeadStatusKey").val("");
}

$(document).on('click', 'body', function (e) {
    if (e.target.id != "SearchName") {
        $("#LeadList").remove();
    }


})

function EnquiryLeadFeedbackPopupReady() {

    $("#EnquiryLeadStatusKey").change(function () {

        if (this.value == Resources.EnquiryStatusClosed) {
            $("#NextCallSchedule").val("");
            $("#NextCallSchedule").attr("disabled", true)
        }
        else {
            $("#NextCallSchedule").removeAttr("disabled");
        }
        var validator = $($(this).closest("form")).validate();
        validator.element("#NextCallSchedule");
        var obj = {};
        obj.EnquiryStatusKey = $("#EnquiryLeadStatusKey").val();
        EnquiryLead.GetCallStatusByEnquiryStatus(obj, $("#EnquiryLeadCallStatusKey"));
        EnquiryLead.CheckCallStatusDuration($(this).val());
    });


    //$("#EnquiryLeadStatusKey").on("change", function ()
    //{
    //    
    //    var obj = {};
    //    obj.EnquiryStatusKey = $("#EnquiryLeadStatusKey").val();
    //    EnquiryLead.GetCallStatusByEnquiryStatus(obj, $("#EnquiryLeadCallStatusKey"));
    //});
    $("#EnquiryLeadCallStatusKey").on("change", function () {

        EnquiryLead.CheckCallStatusDuration($(this).val());
    });

}