var BonafiedCertificate = (function () {

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
            Resources.Issued, Resources.IssueDate, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'BonafiedCertificateKey', index: 'BonafiedCertificateKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudentName', index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudentMobile', index: 'StudentMobile', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatCourseUniversityYear, width: 250 },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsIssued', index: 'IsIssued', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatYesorNO },
                { key: false, name: 'IssuedDate', index: 'IssuedDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 250 },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
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

   
    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        //return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm" data-href="' + $("#hdnAddEditUniversityCertificate").val() + '/' + rowdata.RowKey + '"><i class="fa fa-plus-circle" aria-hidden="true"></i>' + Resources.AddCertificate + '</a></div>';

        if (!rowdata.BonafiedCertificateKey) {
            return '<div class="divEditDelete"><a  class="btn btn-outline-warning btn-sm ml-3" onClick="BonafiedCertificate.BonafiedCertificatePopup(' + rowdata.BonafiedCertificateKey + ',' + rowdata.RowKey+')"><i class="fa fa-share" aria-hidden="true"></i>' + Resources.Issue + '</a> </div>';

        }
        else {
            return '<div class="divEditDelete"><a  class="btn btn-outline-warning btn-sm ml-3" onClick="BonafiedCertificate.BonafiedCertificatePopup(' + rowdata.BonafiedCertificateKey + ',' + rowdata.RowKey + ')"><i class="fa fa-share" aria-hidden="true"></i>' + Resources.Issue + '</a>' + '<a class="btn btn-outline-danger btn-sm ml-2"  onclick="javascript:deleteBonafiedCertificate(' + rowdata.BonafiedCertificateKey + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>' + '<a  class="btn btn-outline-primary btn-sm ml-2" onClick="BonafiedCertificate.PrintBonafiedCertificate(' + rowdata.RowKey + ')"><i class="fa fa-print" aria-hidden="true"></i></a> </div>';

        }

    }

    function formatCourseUniversityYear(cellValue, options, rowdata, action) {
        var Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName //+ " - " + yeartext
        return Coursetext;
    }

    function formatCurrentYear(cellValue, options, rowdata, action) {

        return yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)
    }

   
   
    var BonafiedCertificatePopup = function (Id,ApplicationKey) {
        var obj = {};
        obj.ApplicationKey = ApplicationKey;
        obj.Id = Id;
        var url = $("#hdnAddEditBonafiedCertificate").val() + "?" + $.param(obj);
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

    var fetchBonafiedCertificate = function () {
        var obj = {};
        obj.ApplicationKey = $("#ApplicationKey").val();
        $("#dvFetchBonafiedCertificate").mLoading();

        $.ajax({
            type: "Get",
            url: $("#hdnFetchBonafiedCertificateDetails").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata()[0];
                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: $("#hdnBonafiedCertificatePath").val() + "?no-cache=" + new Date().getTime(),
                    success: function (response) {
                        //Array.prototype.forEach.call(document.styleSheets, function (element) {
                        //    try {
                        //        element.disabled = true;
                        //    } catch (err) { }
                        //});

                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        var html = template(data);
                        $('#dvFetchBonafiedCertificate').html(html);
                    },
                    error: function (xhr) {

                    },
                    complete: function () {
                        $("#dvFetchBonafiedCertificate").mLoading("destroy");
                    }
                })


            }
        })

    }

    function formSubmit() {

        var $form = $("#frmAddEditBonafiedCertificate")

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
                var successmsg = "Bonafied Certificate Issued Successfully";
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
                                window.location.href = $("#hdnBonafiedCertificateList").val();
                            }
                        }
                    }
                })

            }

        }



    }

    var printBonafiedCertificate = function (Id, rebind) {
        var obj = {};
        obj.ApplicationKey = Id;
        if (obj.ApplicationKey) {
            $(".page-content").mLoading();
            $.ajax({
                type: "Get",
                url: $("#hdnFetchBonafiedCertificateDetails").val(),
                datatype: "json",
                data: obj,
                success: function (result) {

                    result = result.formatdata()[0];
                 
                    if (result.StudentGender == 1) {
                        result.Gender = "his"
                    }
                    else {
                        result.Gender = "her"
                    }
                   
                    var url = $("#hdnBonafiedCertificateDefaultPath").val() + "?no-cache=" + new Date().getTime();

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
        FetchBonafiedCertificate: fetchBonafiedCertificate,
        BonafiedCertificatePopup: BonafiedCertificatePopup,
        FormSubmit: formSubmit,
        PrintBonafiedCertificate: printBonafiedCertificate

    }
}());



function deleteBonafiedCertificate(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Application,
        actionUrl: $("#hdnDeleteBonafiedCertificate").val(),
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