var JSONTemplate;
var ImportType;
var Import = (function () {
    var getTemplateData = function (type) {
        $.getJSON($("#hdnImportJSONPath").val() + "?no-cache=" + new Date().getTime(), function (result) {
            JSONTemplate = result[type];
            JSONTemplate = JSONTemplate.map(function (item) {
                if (item.editorRegx) {
                    var pattern = new RegExp(item.editorRegx, 'g');
                    item.editorRegx = pattern
                }
                if ($("#" + item.key)[0]) {
                    var element = $("#" + item.key).clone(true)
                    $(element).removeAttr("data-val-required").removeAttr("data-val-remote")
                    item.element = $(element)[0]
                }
                return item;
            });
            ImportType = type;
        });

    }
    var downloadExcel = function () {
        var data = [];
        var headers = JSONTemplate.filter(function (item) {
            return item.type != "hidden"
        }).map(function (item) {
            return item.title;
        });
        data.push(headers);
        var wb = new Workbook(), ws = sheet_from_array_of_arrays(data);
        /* add worksheet to workbook */
        wb.SheetNames.push(ImportType);

        wb.Sheets[ImportType] = ws;
        $(JSONTemplate).each(function () {

            if (this.source && this.source.length) {
                wb.SheetNames.push(this.listname);
                var listData = [];
                var listHeader = ["Code", "Name"];
                listData.push(listHeader);
                this.source.forEach(function (item) {
                    var objItem = [];
                    objItem.push(item.code);
                    objItem.push(item.name);
                    listData.push(objItem);
                    return objItem
                });
                ws = sheet_from_array_of_arrays(listData);
                wb.Sheets[this.listname] = ws;
            }
        });



        var wbout = XLSX.write(wb, { bookType: 'xlsx', bookSST: true, type: 'binary' })
        saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), ImportType + ".xlsx")

    }
    var fileUploadChange = function (e, type) {
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
                            JsonData.shift();
                            var hiddenCols = JSONTemplate.filter(function (item) {
                                return item.type == "hidden"
                            });
                            JsonData = JsonData.map(function (item) {
                                var newItem = {};

                                newItem = item.map(function (subitem, i) {
                                    if (JSONTemplate[hiddenCols.length + i].source && JSONTemplate[hiddenCols.length + i].source.length)
                                        subitem = JSONTemplate[hiddenCols.length + i].source.filter(function (sourceitem) {
                                            return sourceitem.code == subitem || sourceitem.name == subitem
                                        }).map(function (sourceitem) {
                                            return sourceitem.id;
                                        })[0]
                                    return subitem;
                                });
                                for (var j = 0; j < hiddenCols.length; j++) {
                                    newItem.unshift("");
                                }
                                return newItem
                            })
                            var obj = {};
                            obj.data = JsonData;
                            obj.columns = JSONTemplate;

                            Import.SetExcelSheelValue(obj)
                            $(".section-content").mLoading("destroy");
                            //// create workbook & add worksheet
                            //var workbook = new ExcelJS.Workbook();
                            //var worksheet = workbook.addWorksheet('Discography');

                            //// add column headers
                            //worksheet.columns = [
                            //    { header: 'Album', key: 'album' },
                            //    { header: 'Year', key: 'year' }
                            //];

                            //// add row using keys
                            //worksheet.addRow({ album: "Taylor Swift", year: 2006 });

                            //// add rows the dumb way
                            //worksheet.addRow(["Fearless", 2008]);

                            //// add an array of rows
                            //var rows = [
                            //    ["Speak Now", 2010],
                            //    { album: "Red", year: 2012 }
                            //];
                            //worksheet.addRows(rows);

                            //// edit cells directly
                            //worksheet.getCell('A' + (1)).dataValidation = {
                            //    type: 'list',
                            //    allowBlank: false,
                            //    formulae: ['"Selected,Rejected,On-hold"']
                            //};
                            //function s2ab(s) {
                            //    var buf = new ArrayBuffer(s.length);
                            //    var view = new Uint8Array(buf);
                            //    for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
                            //    return buf;
                            //}
                            //// save workbook to disk
                            //workbook.xlsx.writeBuffer().then(function (data) {
                            //    var blob = new Blob([data], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
                            //    saveAs(blob, "fileName.xlsx");
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
    var setExcelSheelValue = function (obj, IsLoad) {
        setExcelSheetWidth(obj);
        if (!$('#Excel').hasClass('jexcel_container')) {
            $('#Excel').jexcel({
                data: obj.data,
                // colHeaders: JsonHead,
                columnSorting: false,
                allowInsertRow: true,
                // Allow row delete
                allowDeleteRow: true,
                columns: obj.columns,
                autoIncrement: false,
                minSpareRows: 1,
                includeHeadersOnDownload: true,
                onbeforepaste: function (el, records) {

                },
                //onbeforedeleterow: function (el, rowNum, records) {

                //},
                onload: function (el, obj, data) {
                    //var cols = $("td[data-x][data-y]", el);
                    //$(cols).each(function () {
                    //    if ($(this).is(':visible') && this.innerHTML) {
                    //        obj.openEditor(this);
                    //        obj.closeEditor(this, true);
                    //    }
                    //})

                }
            });

        } else {
            $('#Excel').jexcel('setData', obj.data, false);
        }



    }
    var getExcelSheetValue = function () {
        var postData = [];
        var data = $('#Excel').jexcel('getData');
        $(data).each(function (i, dataitem) {
            var obj = {};
            var remove = true;
            $(JSONTemplate).each(function (j, item) {
                obj[item.key] = dataitem[j]
                if (!obj[item.key] && !item.required)
                    obj[item.key] = null

                remove = remove && ((item.required && (dataitem[j] != null && dataitem[j] != undefined && dataitem[j] != "")) || !item.required)
            });
            obj.PageIndex = i;
            if (remove)
                postData.push(obj);
        });

        return postData;

    }
    var fillCourseTypesById = function () {
        var ddl = $("#CourseTypeKey")
        var obj = {};
        obj.AcademicTermKey = $("#AcademicTermKey").val();
        obj.AcademicTermKey = obj.AcademicTermKey ? parseInt(obj.AcademicTermKey) : 0;
        AppCommon.BindDropDownbyId(obj, $("#hdnFillCourseTypesById").val(), ddl, Resources.CourseType);

    }
    var fillCoursesById = function () {
        var ddl = $("#CourseKey")
        var obj = {};
        obj.CourseTypeKey = $("#CourseTypeKey").val();
        AppCommon.BindDropDownbyId(obj, $("#hdnFillCoursesById").val(), ddl, Resources.Course);
    }
    var fillUniversitiesById = function () {
        var ddl = $("#UniversityKey")
        var obj = {};
        obj.CourseKey = $("#CourseKey").val();
        AppCommon.BindDropDownbyId(obj, $("#hdnFillUniversitiesById").val(), ddl, Resources.Course);

    }
    var fillYearsById = function () {
        var ddl = $("#CurrentYear")
        var obj = {};
        obj.AcademicTermKey = $("#AcademicTermKey").val();
        obj.CourseKey = $("#CourseKey").val();
        AppCommon.BindDropDownbyId(obj, $("#hdnFillYearsById").val(), ddl, Resources.Year);
    }
    var fillClassDetailsById = function () {
        var ddl = $("#ClassDetailsKey")
        var obj = {};
        obj.BatchKey = $("#BatchKey").val();
        obj.BranchKey = $("#BranchKey").val();
        obj.AcademicTermKey = $("#AcademicTermKey").val();
        obj.CourseKey = $("#CourseKey").val();
        obj.UniversityKey = $("#UniversityKey").val();
        obj.InternalExamTermKey = $("#InternalExamTermKey").val();
        if ($("#CurrentYear").val())
            obj.StudentYearKey = $("#CurrentYear").val();
        AppCommon.BindDropDownbyId(obj, $("#hdnFillClassDetails").val(), ddl, Resources.ClassCode);
    }
    var fillExcelSheetDropdowns = function (url, modifyFilter, IsLoad) {
        $(".section-content").mLoading();
        var obj = {
            BatchKey: $("#BatchKey").val(),
            BranchKey: $("#BranchKey").val(),
            AcademicTermKey: $("#AcademicTermKey").val(),
            CourseKey: $("#CourseKey").val(),
            UniversityKey: $("#UniversityKey").val(),
            ClassDetailsKey: $("#ClassDetailsKey").val(),
            InternalExamTermKey: $("#InternalExamTermKey").val(),
            IsValid: true
        };

        if ($("#CurrentYear").val())
            obj.StudentYearKey = $("#CurrentYear").val();

        if (modifyFilter) {
            modifyFilter.call(obj);
        }

        if (obj.IsValid) {
            obj.IsValid = null;

            $.ajax({
                url: url,
                type: "GET",
                dataType: "JSON",
                data: obj,
                success: function (result) {

                    $(JSONTemplate).each(function () {
                        this.source = result[this.listname] ? result[this.listname].map(function (item) {
                            return {
                                id: item.RowKey,
                                code: item.ValueText,
                                text: (item.ValueText ? item.ValueText + " - " : "") + item.Text,
                                name: item.Text
                            };
                        }) : [];
                    });

                    var objGrid = {};
                    objGrid.data = result.data ? Import.ModifyExistingData(result.data) : [[]];
                    objGrid.columns = JSONTemplate;

                    if (result.TotalRecords)
                        $("#spnTotalRecords").html(result.TotalRecords);

                    Import.ShowHideExcel(objGrid, IsLoad);
                    $(".section-content").mLoading("destroy");
                }
            });
        } else {
            $(".dvExcelSheet").hide();
            $(".section-content").mLoading("destroy");
        }
    };
  var updateAvailableStudyMaterials = function () {
        var data = Import.GetExcelSheetValue();
        data = data.map(function (item) {
            item.RowKey = 0;
            item.IsAvailable = true;
            return item;
        });
        if (data.length) {
            $(".section-content").mLoading();


            //setTimeout(function () {

            var form = $("form");
            var url = $(form).attr("action");
            var type = $(form).attr("method");
            var model = { StudyMaterialList: data }
            $.ajax({
                type: type,
                url: url,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ model: model }),

                success: function (response) {
                    if (response.IsSuccessful == true) {

                        $.alert({
                            type: 'green',
                            title: Resources.Success,
                            content: response.Message,
                            icon: 'fa fa-check-circle-o-',
                            buttons: {
                                Ok: {
                                    text: Resources.Ok,
                                    btnClass: 'btn-success',
                                    action: function () {
                                        window.location.href = $("#hdnStudyMaterialList").val();
                                    }
                                }
                            }
                        })

                    }
                    else {
                        $("#error_msg").html(response.Message);
                    }
                    $(".section-content").mLoading("destroy");
                },
                error: function (request, status, error) {

                }
            });

        }
        else {
            EduSuite.AlertMessage({ title: Resources.Warning, content: "Please add atleast one Record !.", type: 'orange' })
            return false;
        }
    }
    var updateAvailableCertificates = function () {
        var data = Import.GetExcelSheetValue();
        data = data.map(function (item) {
            item.RowKey = 0;
            item.IsReceived = true;
            item.ReceivedDate = $("#ReceivedDate").val();
            return item;
        });
        if (data.length) {
            $(".section-content").mLoading();


            //setTimeout(function () {

            var form = $("form");
            var url = $(form).attr("action");
            var type = $(form).attr("method");
            var model = { UniversityCertificateDetails: data }
            $.ajax({
                type: type,
                url: url,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ model: model }),

                success: function (response) {
                    if (response.IsSuccessful == true) {
                        window.location.href = $("#hdnUniversityCertificateList").val();
                    }
                    else {
                        $("#error_msg").html(response.Message);
                    }
                    $(".section-content").mLoading("destroy");
                },
                error: function (request, status, error) {

                }
            });

        }
    }
    var updateExamResults = function () {
        var data = Import.GetExcelSheetValue();
        data = data.filter(function (item) {
            return item.Mark || item.ResultStatus;
        }).map(function (item) {
            item.RowKey = 0;
            item.ResultStatus = item.ResultStatus ? 'A' : null;
            item.Mark = item.ResultStatus ? null : item.Mark;
            return item;
        });
        if (data.length) {
            $(".section-content").mLoading();


            //setTimeout(function () {

            var form = $("form");
            var url = $(form).attr("action");
            var type = $(form).attr("method");
            var model = { ExamResultDetail: data }
            $.ajax({
                type: type,
                url: url,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ model: model }),

                success: function (response) {
                    if (response.IsSuccessful == true) {
                        window.location.href = $("#hdnExamResultList").val();
                    }
                    else {
                        $("#error_msg").html(response.Message);
                    }
                    $(".section-content").mLoading("destroy");
                },
                error: function (request, status, error) {

                }
            });

        }
    }
    var updateInternalExamResults = function () {
        var data = Import.GetExcelSheetValue();
        data = data.filter(function (item) {
            return item.Mark || item.ResultStatus;
        }).map(function (item) {
            item.RowKey = 0;
            item.ResultStatus = item.ResultStatus ? 'A' : null;
            item.Mark = item.ResultStatus ? null : item.Mark;
            return item;
        });

        if (data.length) {
            $(".section-content").mLoading();


            //setTimeout(function () {

            var form = $("form");
            var url = $(form).attr("action");
            var type = $(form).attr("method");
            var model = { InternalExamResultDetails: data }
            model.BranchKey = $("#BranchKey").val();
            model.BatchKey = $("#BatchKey").val();
            model.CourseKey = $("#CourseKey").val();
            model.AcademicTermKey = $("#AcademicTermKey").val();
            model.UniversityMasterKey = $("#UniversityKey").val();
            model.ExamYear = $("#CurrentYear").val();
            model.InternalExamTermKey = $("#InternalExamTermKey").val();

            $.ajax({
                type: type,
                url: url,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ model: model }),

                success: function (response) {
                    if (response.IsSuccessful == true) {
                        window.location.href = $("#hdnInternalExamResultList").val();
                    }
                    else {
                        $("#error_msg").html(response.Message);
                    }
                    $(".section-content").mLoading("destroy");
                },
                error: function (request, status, error) {

                }
            });

        }
    }
    var updateEnquiryLeads = function () {
        var data = Import.GetExcelSheetValue();
        var IsValidationSuccess = ValidateExcelData();
        data = data.map(function (item) {
            item.BranchKey = $("#BranchKey").val();
            item.RowKey = parseInt(item.RowKey) ? parseInt(item.RowKey) : 0;
            return item
        })
        var form = $("#frmBulkEnquiryLead")
        var validator = form.validate({


        });
        if (data.length && IsValidationSuccess == true) {

            var url = $(form).attr("action");
            var type = $(form).attr("method");
            var validate = $(form).valid();
            if (validate) {

                var duplicates = data.filter(function (el, i, x) {
                    return x.some(function (obj, j) {
                        return obj.MobileNumber === el.MobileNumber && (x = j);
                    }) && i != x;
                });

                var DuplicateMobileNumbers = data.filter(function (item) {
                    return item.MobileNumber
                });
                var InnerDuplicateMobileNumbers = DuplicateMobileNumbers.map(function (item) {
                    return (item.TelephoneCodeKey ? item.TelephoneCodeKey : "") + "|" + item.MobileNumber
                }).duplicate();
                DuplicateMobileNumbers = data.filter(function (item) {
                    return InnerDuplicateMobileNumbers.indexOf((item.TelephoneCodeKey ? item.TelephoneCodeKey : "") + "|" + item.MobileNumber) == -1
                });

                var DuplicateEmails = data.filter(function (item) {
                    return item.EmailAddress
                });
                var InnerDuplicateEmails = DuplicateEmails.map(function (item) {
                    return item.EmailAddress
                }).duplicate();
                DuplicateEmails = data.filter(function (item) {
                    return InnerDuplicateEmails.indexOf(item.EmailAddress) == -1
                });


                if (InnerDuplicateMobileNumbers.length || InnerDuplicateEmails.length) {
                    var objExists = {};
                    objExists.Mobiles = InnerDuplicateMobileNumbers.map(function (item) {
                        return item.split('|')[1]
                    });
                    objExists.Emails = InnerDuplicateEmails;
                    objExists.title = "Duplicates Found !."
                    //Import.ExistsLeadAlert(objExists)

                    $(duplicates).each(function () {
                        var row = $('#Excel tbody tr').eq(this.PageIndex);
                        var index = JSONTemplate.findIndex(function (item) {
                            return item.key == 'MobileNumber';
                        });
                        var MobileNumberCell = $("td[data-x][data-y]", $(row)).eq(index);
                        $(MobileNumberCell).attr("data-error", $("#MobileNumber").data("val-remote"));
                    })

                }
                else {
                    $(".section-content").mLoading();
                    $.ajax({
                        url: $("#hdnCheckBulkLeadExists").val(),
                        data: { DuplicateMobileNumbers: DuplicateMobileNumbers, DuplicateEmails: DuplicateEmails },
                        datatype: "json",
                        type: "POST",
                        contenttype: 'application/json; charset=utf-8',
                        success: function (response) {
                            if (response.DuplicateMobileNumbers.length || response.DuplicateEmails.length) {
                                $(response.DuplicateMobileNumbers).each(function () {
                                    var row = $('#Excel tbody tr').eq(this.PageIndex);
                                    var index = JSONTemplate.findIndex(function (item) {
                                        return item.key == 'MobileNumber';
                                    });
                                    var MobileNumberCell = $("td[data-x][data-y]", $(row)).eq(index);
                                    $(MobileNumberCell).attr("data-error", $("#MobileNumber").data("val-remote"));

                                });
                                $(response.DuplicateEmails).each(function () {
                                    var row = $('#Excel tbody tr').eq(this.PageIndex);
                                    var index = JSONTemplate.findIndex(function (item) {
                                        return item.key == 'EmailAddress';
                                    });
                                    var EmailAddressCell = $("td[data-x][data-y]", $(row)).eq(index);
                                    $(EmailAddressCell).attr("data-error", $("#MobileNumber").data("val-remote"));

                                });
                                $(".section-content").mLoading("destroy");
                            } else {
                                $("td", $('#Excel')).removeAttr("data-error")
                                $.ajax({
                                    type: type,
                                    url: url,
                                    contentType: "application/json; charset=utf-8",
                                    data: JSON.stringify({ modelList: data }),

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
                            }
                        },
                        error: function (xhr) {
                            $(".section-content").mLoading("destroy")
                        }

                    });

                }

            }
            else {
                validator.focusInvalid();
            }
        }
    }
    var showHideExcel = function (obj, IsLoad) {
        var RequiredLength = JSONTemplate.filter(function (item) {
            return !item.source.length && item.listname && item.required;
        }).length;
        if (RequiredLength) {
            $(".dvExcelSheet").hide();

        } else {
            $(".dvExcelSheet").show();
            Import.SetExcelSheelValue(obj, IsLoad);
        }

    }
    var existsLeadAlert = function (obj) {
        var message = obj.Mobiles.join(", ") + (obj.Emails.length ? "<hr class='my-2'/>" + obj.Emails.join(", ") : "");
        EduSuite.AlertMessage({
            title: obj.title,
            content: message,
            type: 'orange',
            icon: 'fa fa-exclamation-circle',
            buttons: {

                Ok:
                {
                    text: Resources.Ok,
                    btnClass: 'btn-warning'
                }
            }
        })

    }
    var modifyExistingData = function (data) {
        var result = [];
        $(data).each(function (i, item) {
            var resultitem = [];
            $(JSONTemplate).each(function () {
                resultitem.push(item[this.key]);

            });
            result.push(resultitem);
        });
        return result;
    }
    var updateEmployeeAttendance = function () {
        var data = Import.GetExcelSheetValue();
        data = data.map(function (item) {
            item.BranchKey = $("#BranchKey").val();
            item.RowKey = parseInt(item.RowKey) ? parseInt(item.RowKey) : 0;
            item.InDateTime = null;
            item.OutDateTime = null;
            item.InTime = ("09:30");
            item.OutTime = ("18:30");

            return item
        });
        if (data.length) {
            $(".section-content").mLoading();


            //setTimeout(function () {

            var form = $("#frmBulkEmployeeAttendance")
            var url = $(form).attr("action");
            var type = $(form).attr("method");

            $.ajax({
                type: type,
                url: url,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ modelList: data }),

                success: function (response) {
                    if (response.IsSuccessful == true) {

                        $.alert({
                            type: 'green',
                            title: Resources.Success,
                            content: response.Message,
                            icon: 'fa fa-check-circle-o-',
                            buttons: {
                                Ok: {
                                    text: Resources.Ok,
                                    btnClass: 'btn-success',
                                    action: function () {
                                        window.location.href = $("#hdnEmployeeAttendanceList").val();
                                    }
                                }
                            }
                        })

                    }
                    else {
                        $("#error_msg").html(response.Message);
                    }
                    $(".section-content").mLoading("destroy");
                },
                error: function (request, status, error) {

                }
            });

        }
        else {
            EduSuite.AlertMessage({ title: Resources.Warning, content: "Please add atleast one Record !.", type: 'orange' })
            return false;
        }
    }
    return {
        GetTemplateData: getTemplateData,
        DownloadExcel: downloadExcel,
        FileUploadChange: fileUploadChange,
        SetExcelSheelValue: setExcelSheelValue,
        GetExcelSheetValue: getExcelSheetValue,
        FillCourseTypesById: fillCourseTypesById,
        FillCoursesById: fillCoursesById,
        FillUniversitiesById: fillUniversitiesById,
        FillYearsById: fillYearsById,
        FillExcelSheetDropdowns: fillExcelSheetDropdowns,
        UpdateAvailableStudyMaterials: updateAvailableStudyMaterials,
        UpdateAvailableCertificates: updateAvailableCertificates,
        UpdateExamResults: updateExamResults,
        UpdateInternalExamResults: updateInternalExamResults,
        UpdateEnquiryLeads: updateEnquiryLeads,
        ShowHideExcel: showHideExcel,
        ExistsLeadAlert: existsLeadAlert,
        ModifyExistingData: modifyExistingData,
        FillClassDetailsById: fillClassDetailsById,
        UpdateEmployeeAttendance: updateEmployeeAttendance
    }

}());

function setExcelSheetWidth(obj) {
    var width = $("#Excel").closest(".card-body")[0].offsetWidth - 65;
    var singleWidth;
    var totalCount = obj.columns.length;
    var widthLength = obj.columns.filter(function (item) {
        return (parseFloat(item.width) ? parseFloat(item.width) : 0);
    }).length;
    totalCount -= widthLength;

    $(obj.columns).each(function (i, item) {
        var itemWidth = (parseFloat(item.width) ? parseFloat(item.width) : 0)
        if (itemWidth)
            width -= itemWidth;

    });
    singleWidth = width / totalCount;
    $(obj.columns).each(function (i, item) {
        var itemWidth = (parseFloat(item.width) ? parseFloat(item.width) : 0)
        if (!itemWidth)
            item.width = singleWidth;
    });



}

var ValidateExcelData = function () {
    var IsValidationSuccess = true;
    var IsTelephoneCodeKeyValid = true;
    var postData = [];
    var data = $('#Excel').jexcel('getData');
    $(data).each(function (i, dataitem) {
        var obj = {};
        var remove = true;
        $(JSONTemplate).each(function (j, item) {
            if (AppCommon.IsDataIsEmpty(dataitem[j]) == false) {
                remove = false;
            }
        });
        if (remove == false) {
            $(JSONTemplate).each(function (j, item) {
                obj[item.key] = dataitem[j]
                if (!obj[item.key] && !item.required) {
                    obj[item.key] = null
                }
                switch (item.key) {
                    case "MobileNumber":
                        var index = JSONTemplate.findIndex(function (Myitem) {
                            return Myitem.key == 'MobileNumber';
                        });
                        var row = $('#Excel tbody tr').eq(i);
                        var MobileNumberCell = $("td[data-x][data-y]", $(row)).eq(index);
                        $(MobileNumberCell).removeAttr("data-error");
                        if (AppCommon.IsDataIsEmpty(dataitem[j]) && item.required) {
                            $(MobileNumberCell).attr("data-error", "Required");
                            IsValidationSuccess = false;
                        }
                        if (AppCommon.TestRegxById($("#MobileNumber"), dataitem[j]) == false) {
                            MobileNumberCell = $("td[data-x][data-y]", $(row)).eq(index);
                            $(MobileNumberCell).attr("data-error", "Invalid Mobile Number");
                            IsValidationSuccess = false;
                        }


                        break;
                    case "EmailID":
                        var index = JSONTemplate.findIndex(function (Myitem) {
                            return Myitem.key == 'EmailID';
                        });
                        var row = $('#Excel tbody tr').eq(i);
                        var EmailIDCell = $("td[data-x][data-y]", $(row)).eq(index);
                        $(EmailIDCell).removeAttr("data-error");
                        if (AppCommon.TestRegxById($("#EmailID"), dataitem[j]) == false) {
                            EmailIDCell = $("td[data-x][data-y]", $(row)).eq(index);
                            $(EmailIDCell).attr("data-error", "Invalid Email");
                            IsValidationSuccess = false;
                        }
                        break;
                    case "TelephoneCodeKey":
                        var index = JSONTemplate.findIndex(function (Myitem) {
                            return Myitem.key == 'TelephoneCodeKey';
                        });
                        var row = $('#Excel tbody tr').eq(i);
                        var TelephoneCodeKeyCell = $("td[data-x][data-y]", $(row)).eq(index);
                        $(TelephoneCodeKeyCell).removeAttr("data-error");
                        if (AppCommon.IsDataIsEmpty(dataitem[j]) && item.required) {
                            $(TelephoneCodeKeyCell).attr("data-error", "Required");
                            IsValidationSuccess = false;
                        }
                        break;
                    case "LeadDate":
                        var index = JSONTemplate.findIndex(function (Myitem) {
                            return Myitem.key == 'LeadDate';
                        });
                        var row = $('#Excel tbody tr').eq(i);
                        var LeadDateCell = $("td[data-x][data-y]", $(row)).eq(index);
                        $(LeadDateCell).removeAttr("data-error");
                        if (AppCommon.IsDataIsEmpty(dataitem[j]) && item.required) {
                            LeadDateCell = $("td[data-x][data-y]", $(row)).eq(index);
                            $(LeadDateCell).attr("data-error", "Required");
                            IsValidationSuccess = false;
                        }
                        break;
                    case "LeadSourceKey":
                        var index = JSONTemplate.findIndex(function (Myitem) {
                            return Myitem.key == 'LeadSourceKey';
                        });
                        var row = $('#Excel tbody tr').eq(i);
                        var LeadSourceKeyCell = $("td[data-x][data-y]", $(row)).eq(index);
                        $(LeadSourceKeyCell).removeAttr("data-error");
                        if (AppCommon.IsDataIsEmpty(dataitem[j]) && item.required) {
                            LeadSourceKeyCell = $("td[data-x][data-y]", $(row)).eq(index);
                            $(LeadSourceKeyCell).attr("data-error", "Required");
                            IsValidationSuccess = false;
                        }
                        break;
                }
            });
        }
        obj.StartIndex = i;
    });
    return IsValidationSuccess
}
