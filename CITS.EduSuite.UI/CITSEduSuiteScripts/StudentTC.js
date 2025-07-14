var StudentTC = (function () {

    var getApplication = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetApplications").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {

                ApplicantName: function () {
                    return $('#txtSearchApplicantName').val()
                },
                MobileNumber: function () {
                    return $('#txtSearchMobileNumber').val()
                },
                BranchKey: function () {
                    return $('#BranchKey').val()
                },
                BatchKey: function () {
                    return $('#BatchKey').val()
                },
                CourseKey: function () {
                    return $('#CourseKey').val()
                },
                UniversityKey: function () {
                    return $('#UniversityKey').val()
                }

            },
            colNames: [Resources.RowKey, Resources.RowKey, Resources.Branch, Resources.AdmissionNo,
            Resources.Name, Resources.MobileNo, Resources.Course,
            Resources.AcademicTerm, Resources.Batch,
            Resources.Generate, Resources.Issued, Resources.GenerateDate, Resources.IssueDate, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'TCMasterKey', index: 'TCMasterKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudentName', index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudentMobile', index: 'StudentMobile', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatCourseUniversityYear, width: 250 },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsGenerate', index: 'IsGenerate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatYesorNO },
                { key: false, name: 'IsIssued', index: 'IsIssued', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatYesorNO },
                { key: false, name: 'GeneratedDate', index: 'GeneratedDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'IssuedDate', index: 'IssuedDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 250 },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 50, 100, 250, 500, 1000],
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
            loadonce: false,
            ignoreCase: true,
            sortname: 'RowKey',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow',
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
            },
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
            }
        });

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }

    function formatYesorNO(cellValue, option, rowdata, action) {

        if (cellValue == true) {
            return '<i  class="fa fa-check" aria-hidden="true"></i>';
        }
        else {
            return '<i  class="fa fa-times" aria-hidden="true"></i>';
        }
        return cellValue;
    }

    var loadData = function (json) {
        var model = json;

        model.ApplicationKey = $("#ApplicationKey").val();
        model.ApplicationKey = parseInt(model.ApplicationKey) ? parseInt(model.ApplicationKey) : 0;
        model.RowKey = $("#RowKey").val();
        $.ajax({
            type: "POST",
            url: $("#hdnGetTCColumnDetails").val(),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(model),
            success: function (result) {
                if (result.IsSuccessful == false) {
                    $("[data-valmsg-for=error_msg]").html(result.Message);

                }
                $("#dvStudentTCDetails").html("")
                $("#dvStudentTCDetails").html(result);
            },
            error: function (request, status, error) {

            }
        });
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        //return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm" data-href="' + $("#hdnAddEditUniversityCertificate").val() + '/' + rowdata.RowKey + '"><i class="fa fa-plus-circle" aria-hidden="true"></i>' + Resources.AddCertificate + '</a></div>';

        if (!rowdata.TCMasterKey) {
            return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="' + $("#hdnAddEditStudentTC").val() + '/' + rowdata.RowKey + '"><i class="fa fa-file-text" aria-hidden="true"></i>' + Resources.Edit + '</a> </div>';

        }
        else {
            return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mb-1" href="' + $("#hdnAddEditStudentTC").val() + '/' + rowdata.RowKey + '"><i class="fa fa-file-text" aria-hidden="true"></i>' + Resources.Edit + '</a>' + '<a  class="btn btn-outline-warning btn-sm ml-3" onClick="StudentTC.StudentTCPopup(' + rowdata.RowKey + ')"><i class="fa fa-share" aria-hidden="true"></i>' + Resources.Issue + '</a>' + '<a class="btn btn-outline-danger btn-sm"  onclick="javascript:deleteStudentTC(' + rowdata.TCMasterKey + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i>' + Resources.Delete + '</a>' + '<a  class="btn btn-outline-primary btn-sm ml-3" onClick="StudentTC.PrintTC(' + rowdata.RowKey + ')"><i class="fa fa-print" aria-hidden="true"></i>' + Resources.Print + '</a> </div>';

        }

    }

    function formatCourseUniversityYear(cellValue, options, rowdata, action) {
        var Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName //+ " - " + yeartext
        return Coursetext;
    }

    function formatCurrentYear(cellValue, options, rowdata, action) {

        return yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)
    }

    var resetTCDetails = function (rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Enquiry,
            actionUrl: $("#hdnDeleteStuentTCDetails").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                StudentTC.LoadData(jsonData);
            }
        });
    }

    function formSubmit() {

        var $form = $("#frmStudentTC")

        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        var id = formData['ApplicationKey'];
        if ($form.valid()) {
            var dataurl = $form.attr("action");
            var response = [];

            response = AjaxHelper.ajax("POST", dataurl,
                {
                    model: formData
                });
            if (typeof response == "string") {
                $("[data-valmsg-for=error_msg]").html(response);
            }
            else if (!response.IsIssued) {
                var successmsg = "TC Generation is Successfull,Want to Issue ?";
                $.alert({
                    type: 'green',
                    title: Resources.Success,
                    content: successmsg,
                    icon: 'fa fa-check-circle-o-',
                    buttons: {
                        Ok: {
                            text: Resources.Yes,
                            btnClass: 'btn-success',
                            action: function () {
                                StudentTC.StudentTCPopup(id);
                            }
                        },
                        Cancel: {
                            text: Resources.No,
                            btnClass: 'btn-danger',
                            action: function () {
                                window.location.href = $("#hdnStudentTCList").val();
                            }
                        }
                    }
                })

            }
            else {
                window.location.href = $("#hdnStudentTCList").val();
            }

        }
    }

    var studentTCPopup = function (Id) {
        var url = $("#hdnIssueStudentTC").val() + "/" + Id;
        $.customPopupform.CustomPopup({
            //modalsize: "modal-lg mw-100 w-75",
            modalsize: "modal-md",
            load: function () {
                setTimeout(function () {

                }, 500)
            },
            rebind: function (result) {
                //if (result.IsSuccessful) {
                //    $('#myModal').modal('hide');
                //    window.location.reload();
                //}
            }
        }, url);
    }

    var fetchStudentTC = function () {
        var obj = {};
        obj.ApplicationKey = $("#ApplicationKey").val();
        $("#dvFetchStudentTC").mLoading();

        $.ajax({
            type: "Get",
            url: $("#hdnFetchTCDetails").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata()[0];
                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: $("#hdnStudentTCPath").val() + "?no-cache=" + new Date().getTime(),
                    success: function (response) {
                        //Array.prototype.forEach.call(document.styleSheets, function (element) {
                        //    try {
                        //        element.disabled = true;
                        //    } catch (err) { }
                        //});

                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        var html = template(data);
                        $('#dvFetchStudentTC').html(html);
                    },
                    error: function (xhr) {

                    },
                    complete: function () {
                        $("#dvFetchStudentTC").mLoading("destroy");
                    }
                })


            }
        })

    }

    function formIssueSubmit() {

        var $form = $("#frmIssueStudentTC")

        var formData = $form.serializeToJSON({
            associativeArrays: false
        });

        if ($form.valid()) {
            var dataurl = $form.attr("action");
            var response = [];

            response = AjaxHelper.ajax("POST", dataurl,
                {
                    model: formData
                });
            if (typeof response == "string") {
                $("[data-valmsg-for=error_msg]").html(response);
            }
            else if (response.IsSuccessful) {
                var successmsg = "Success";
                $.alert({
                    type: 'green',
                    title: Resources.Success,
                    content: successmsg,
                    icon: 'fa fa-check-circle-o-',
                    buttons: {
                        Ok: {
                            text: Resources.Ok,
                            btnClass: 'btn-success',
                            action: function () {
                                window.location.href = $("#hdnStudentTCList").val();
                            }
                        }
                    }
                })

            }

        }



    }

    var printTC = function (Id, rebind) {
        var obj = {};
        obj.ApplicationKey = Id;
        if (obj.ApplicationKey) {
            $(".page-content").mLoading();
            $.ajax({
                type: "Get",
                url: $("#hdnFetchTCDetails").val(),
                datatype: "json",
                data: obj,
                success: function (result) {

                    result = result.formatdata()[0];
                    var dob = new Date(result.StudentDOB);
                    var DOBWords = AppCommon.DOBToWords(dob);
                    result.DOBWords = DOBWords;
                    result.StudentDOB = moment(result.StudentDOB).format('DD/MM/YYYY');



                    var url = $("#hdnStudentTCDefaultPath").val() + "?no-cache=" + new Date().getTime();

                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: url,
                        success: function (response) {


                            var template = Handlebars.compile(response);
                            AppCommon.HandleBarHelpers();
                            html = template(result);
                            $(".page-content").mLoading("destroy");
                            var obj = {}
                            obj.PaperSize = "A4";
                            obj.PaperOrientation = 1;
                            obj.CopiesPerPaper = 1;
                            $("").printArea({
                                html: html,
                                load: function (body) {
                                    obj.printContainer = $("#printTC", body)
                                    ChangeSizeOrientation(obj)
                                },
                                rebind: rebind,
                                paperSize: obj.PaperSize,
                                paperOrientation: obj.PaperOrientation
                            });


                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            //$(_this).removeAttr("disabled")
                        }
                    })

                }


            });
        }
    }


    return {
        GetApplication: getApplication,
        LoadData: loadData,
        ResetTCDetails: resetTCDetails,
        FormSubmit: formSubmit,
        FetchStudentTC: fetchStudentTC,
        StudentTCPopup: studentTCPopup,
        FormIssueSubmit: formIssueSubmit,
        PrintTC: printTC

    }
}());



function deleteStudentTC(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Application,
        actionUrl: $("#hdnDeleteStuentTC").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}


function ChangeSizeOrientation(obj) {



    obj.PaperOrientation = parseInt(obj.PaperOrientation) ? parseInt(obj.PaperOrientation) : 0;
    obj.CopiesPerPaper = parseInt(obj.CopiesPerPaper) ? parseInt(obj.CopiesPerPaper) : 1;


    if (obj.PaperSize) {
        var objSize = AppCommon.GetPaperSizeFromName(obj.PaperSize);
        obj.width = objSize.width;
        obj.height = objSize.height;
        obj.margin = objSize.margin;
        obj.unit = objSize.unit;
        var dedMargin = 0;
        dedMargin = objSize.margin;
        dedMargin = obj.CopiesPerPaper > 1 ? dedMargin : 0;
        if (obj.PaperOrientation == Resources.PaperOrientationLandscape || obj.Rotate) {
            var height = (((parseFloat(objSize.width) - parseFloat(objSize.margin) * 2) / obj.CopiesPerPaper) - dedMargin).toString() + objSize.unit;
            var width = parseFloat(objSize.height) + objSize.unit;
            if (obj.Rotate) {
                height = (objSize.width - dedMargin) + objSize.unit
                width = (((parseFloat(objSize.height)) / obj.CopiesPerPaper)).toString() + objSize.unit;
            }
            $(obj.printContainer).height(height)
            $(obj.printContainer).width(width)
        }
        else {
            var height = ((parseFloat(objSize.height) + ((obj.CopiesPerPaper == 1 ? parseFloat(objSize.margin) * 2 : 0))) / obj.CopiesPerPaper).toString() + objSize.unit;
            var width = (objSize.width) + objSize.unit
            $(obj.printContainer).height(height)
            $(obj.printContainer).width(width)
        }
    }

}




function numberToWords(number) {  
        var digit = ['zero', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine'];  
        var elevenSeries = ['ten', 'eleven', 'twelve', 'thirteen', 'fourteen', 'fifteen', 'sixteen', 'seventeen', 'eighteen', 'nineteen'];  
        var countingByTens = ['twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];  
        var shortScale = ['', 'thousand', 'million', 'billion', 'trillion'];  
  
        number = number.toString(); number = number.replace(/[\, ]/g, ''); if (number != parseFloat(number)) return 'not a number'; var x = number.indexOf('.'); if (x == -1) x = number.length; if (x > 15) return 'too big'; var n = number.split(''); var str = ''; var sk = 0; for (var i = 0; i < x; i++) { if ((x - i) % 3 == 2) { if (n[i] == '1') { str += elevenSeries[Number(n[i + 1])] + ' '; i++; sk = 1; } else if (n[i] != 0) { str += countingByTens[n[i] - 2] + ' '; sk = 1; } } else if (n[i] != 0) { str += digit[n[i]] + ' '; if ((x - i) % 3 == 0) str += 'hundred '; sk = 1; } if ((x - i) % 3 == 1) { if (sk) str += shortScale[(x - i - 1) / 3] + ' '; sk = 0; } } if (x != number.length) { var y = number.length; str += 'point '; for (var i = x + 1; i < y; i++) str += digit[n[i]] + ' '; } str = str.replace(/\number+/g, ' '); return str.trim() + ".";  
  
    } 