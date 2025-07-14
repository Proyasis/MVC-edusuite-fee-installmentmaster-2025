var UniversityCourse = (function () {
    var getUniversityCourse = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetUniversityCourse").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                SearchText: function () {
                    return $('#txtsearch').val()
                },
                SearchAcademicTermKey: function () {
                    return $('#SearchAcademicTermKey').val()
                },
                SearchCourseTypeKey: function () {
                    return $('#SearchCourseTypeKey').val()
                },
                SearchCourseKey: function () {
                    return $('#SearchCourseKey').val()
                },
                SearchUniversityMasterKey: function () {
                    return $('#SearchUniversityMasterKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.AffiliationsTieUps, Resources.Course , Resources.AcademicTerm, Resources.Duration, Resources.TotalFee, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'CourseDuration', index: 'CourseDuration', editable: true },
                { key: false, hidden: true, name: 'DurationCount', index: 'DurationCount', editable: true },
                { key: false, hidden: true, name: 'DurationTypeKey', index: 'DurationTypeKey', editable: true },
                { key: false, name: 'UniversityName', index: 'UniversityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AcademicTermName', index: 'AcademicTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Duration', index: 'Duration', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatDuration },
                { key: false, name: 'TotalUniversityCoursefee', index: 'TotalUniversityCoursefee', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [10, 50, 100, 250, 500],
            altRows: true,
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
            altclass: 'jqgrid-altrow',
            rownumbers: true,
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this), UniversityCoursePopupLoad)
                });

            }
        }).contextMenu({
            selector: ".jqgrow .context-menu",
            trigger: 'left',
            build: function ($trigger, e) {
                // this callback is executed every time the menu is to be shown
                // its results are destroyed every time the menu is hidden
                // e is the original contextmenu event
                var $tr = $(e.target).closest("tr.jqgrow"),
                    rowid = $tr.attr("id"),
                    item = $("#grid").jqGrid("getRowData", rowid);
                return {
                    callback: function (key, options) {

                        var href = "";
                        switch (key) {
                            case "E":
                                UniversityCourse.EditPopup(rowid);
                                break;
                            case "CF":
                                UniversityCourse.CourseFeepopup(rowid);
                                break;
                            case "C":
                                href = $("#hdnAddEditClassDetails").val() + "/" + rowid
                                window.location.href = href;
                                break;
                            //case "IF":
                            //    href = $("#hdnAddEditUniverstyCourseFeeInstallment").val() + "/" + rowid
                            //    window.location.href = href;
                            //    break;
                            case "D":
                                deleteUniversityCourse(rowid);
                                break;

                            default:
                                href = "";

                        }
                    },
                    items: {
                        E: { name: Resources.Edit, icon: "fa-edit" },
                        CF: { name: Resources.AddCourseFee, icon: "fa-inr" },
                        C: { name: Resources.AddClass, icon: "fa-graduation-cap" },
                        //IF: { name: Resources.AddInstallmentFee, icon: "fa-inr" },
                        D: { name: Resources.Delete, icon: "fa-trash" }

                    }
                }

            }
        });

        $("#grid").jqGrid("setLabel", "UniversityCourseName", "", "thUniversityCourseName");
    }


    function formatDuration(cellValue, option, rowdata, action) {
        var Duration = rowdata.CourseDuration;
        if (Duration == 0) {

            if (rowdata.DurationTypeKey == Resources.DurationTypeYear) {
                return "Short Term ( " + rowdata.DurationCount + " Years )";
            }
            else if (rowdata.DurationTypeKey == Resources.DurationTypeDays) {
                return "Short Term ( " + rowdata.DurationCount + " Days )";
            }
            else {
                return "Short Term ( " + rowdata.DurationCount + " Months )";
            }

           
        }
        else {
            return cellValue;
        }
        
    }
    var editPopup = function (rowid) {
        var URL = $("#hdnAddEditUniversityCourse").val() + "?id=" + rowid;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            load: function () {

            },
            rebind: function (result) {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, URL);
    }

    var getClassDetails = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).fadeIn();
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
                $("input[type=text]", $(this)).each(function () { $(this).val("") })
                var item = $(this);

            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];

                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteClassdetails($(hidden).val(), $(this), remove);
                }
                else {
                    $(this).fadeIn(remove);
                    setTimeout(function () {
                    }, 500)
                }

            },
            rebind: function (response) {
                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {

                    toastr.success(Resources.Success, response.Message);

                    var YearListCount = response.YearList.length;
                    if (response.StudentYear == YearListCount) {
                        window.location.href = $("#hdnUniversityCourseList").val();
                    }
                    else {
                        $("#tab-classyear ul  li a.active").parent().nextAll('li:visible').first().find('a').trigger('click');
                    }
                }

            },
            data: json,
            repeatlist: 'ClassDetailsModel',
            submitButton: '',
            defaultValues: json,
        });
    }
    var formSubmitClassDetails = function (btn) {

        var $form = $("#frmClassDetails")
        var JsonData = [];
        var StartTime = ["StartTime"]
        var EndTime = ["EndTime"]
        $("[disabled]", $form).removeAttr("disabled");
        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        if ($form.valid()) {

            formdData = formData["ClassDetailsModel"].map(function (item) {
                item.StartTime = item.StartTime != "" ? moment(item.StartTime.toUpperCase(), ["hh:mm A"]).format("HH:mm") : null;
                item.EndTime = item.EndTime != "" ? moment(item.EndTime.toUpperCase(), ["hh:mm A"]).format("HH:mm") : null;
                return item;
            })
            //formData['StartTime'] = (formData['StartTime'] != "" ? moment(formData['StartTime'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : null);
            //formData['EndTime'] = (formData['EndTime'] != "" ? moment(formData['EndTime'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : null);
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
                toastr.success(Resources.Success, response.Message);

                var YearListCount = response.YearList.length;
                if (response.StudentYear == YearListCount) {
                    window.location.href = $("#hdnUniversityCourseList").val();
                }
                else {
                    $("#tab-classyear ul  li a.active").parent().nextAll('li:visible').first().find('a').trigger('click');
                }

            }

        }

    }

    var getUniversityCourseAmount = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).fadeIn();
                AppCommon.FormatDateInput();
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
                $("input[type=text]", $(this)).each(function () { $(this).val("") })
                var item = $(this);
            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];

                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteUniversityCourseFee($(hidden).val(), $(this), remove);
                }
                else {
                    $(this).fadeIn(remove);
                    setTimeout(function () {
                    }, 500)
                }

            },
            rebind: function (response) {
                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {
                    toastr.success(Resources.Success, response.Message);
                    //$("#tab-classyear ul  li a.active").parent().nextAll('li:visible').first().find('a').trigger('click');
                    //window.location.href = $("#hdnUniversityCourseList").val();
                    $("#frmUniversityCourseAmount").closest(".modal").modal("hide")
                    $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
                    if (typeof ApplicationPersonal !== 'undefined') {
                        ApplicationPersonal.GetAdmissionFee();
                    }
                }

            },
            data: json,
            repeatlist: 'UniversityCourseFeeModel',
            submitButton: '#btnSave',
            defaultValues: json
        });
    }

    var courseFeepopup = function (rowid) {
        var URL = $("#hdnAddEditUniversityCourseAmount").val() + "?id=" + rowid;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            load: function () {
                setTimeout(function () {
                    //UniversityCourse.GetUniversityCourseAmount(jsonData);
                    AppCommon.CustomRepeaterRemoteMethod();
                    $("[data-repeater-item]", $("#frmUniversityCourseAmount")).each(function () {

                        var CenterShareAmountPer = $("[id*=CenterShareAmountPer]", this);
                        var Isuniversity = $("[id*=IsUniversity]", this).val();

                        // if value become a bool when convert to int then check
                        Isuniversity = JSON.parse(Isuniversity.toLowerCase()) ? JSON.parse(Isuniversity.toLowerCase()) : false



                        if (!Isuniversity) {
                            $(CenterShareAmountPer).attr("readonly", true)
                            $(CenterShareAmountPer).val(null)

                        }
                        else {
                            $(CenterShareAmountPer).removeAttr("readonly")
                        }
                    })
                }, 500)
            },
            rebind: function (response) {
                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {
                    toastr.success(Resources.Success, response.Message);
                    //$("#tab-classyear ul  li a.active").parent().nextAll('li:visible').first().find('a').trigger('click');
                    //window.location.href = $("#hdnUniversityCourseList").val();
                    $("#frmUniversityCourseAmount").closest(".modal").modal("hide")
                    $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
                    if (typeof ApplicationPersonal !== 'undefined') {
                        ApplicationPersonal.GetAdmissionFee();
                    } else {
                        $("#grid").jqGrid('setGridParm', { datatype: 'json' }).trigger('reloadGrid');
                    }
                }

            }
        }, URL);
    }
    var getUniversityCourseFeeInstallments = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).fadeIn();
                //$("[id*=OriginalIssuedDate]", $(this)).val(moment(new Date()).format("DD/MM/YYYY"))
                AppCommon.FormatDateInput();
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
                $("input[type=text]", $(this)).each(function () { $(this).val("") })
                var item = $(this);

                setTimeout(function () {
                    $("input[id*=InstallmentAmount]", $(item)).on("input", function () {
                        UniversityCourse.CalculateInstallmentFeeTotal();
                    })
                    $("select[id*=InstallmentMonth]", $(item)).on('change', function () {
                        monthOrYearChanged($(this))
                    });
                }, 500)


            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];

                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    // deleteApplicationFeeinstallment($(hidden).val(), $(this), remove);
                }
                else {
                    $(this).fadeIn(remove);
                    setTimeout(function () {
                        UniversityCourse.CalculateInstallmentFeeTotal();
                    }, 500)
                }

            },
            rebind: function (response) {
                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {
                    toastr.success(Resources.Success, response.Message);
                    $("#tab-feeyear ul  li a.active").parent().nextAll('li:visible').first().find('a').trigger('click');
                }

            },
            data: json,
            repeatlist: 'UniversityCourseFeeInstallments',
            submitButton: '#btnSave',
            defaultValues: json,
            hasFile: true
        });
    }

    var calculateInstallmentFeeTotal = function () {

        var TotalInstallmentFee = 0;
        $("#dvRepeaterList [data-repeater-item]").each(function (i) {
            var InstallmentAmount = $("input[id*=InstallmentAmount]", $(this)).val();
            InstallmentAmount = InstallmentAmount != "" ? parseFloat(InstallmentAmount) : 0;
            TotalInstallmentFee = TotalInstallmentFee + InstallmentAmount;
        })
        $("span#TotalInstallmentFee").html(TotalInstallmentFee);
        $("input#TotalInstallmentFee").val(TotalInstallmentFee);
        $("span#TotalInstallmentFee").css("font-size", "12px").css("text-align", "center");
    }

    var calculateUniversityfee = function () {

        var totalCourseFee = 0;

        $("#DivUniversityCourseAmount [data-repeater-item]").each(function () {
            var FeeAmount = $("input[id*=FeeAmount]", $(this)).val();
            FeeAmount = parseFloat(FeeAmount) ? parseFloat(FeeAmount) : 0;
            var IsActive = $("input[id*=IsActive]", $(this)).val();
           
            var object = $("input[type=checkbox][name*=IsActive]", $(this))[0];
            if (object.checked) {
                totalCourseFee = totalCourseFee + FeeAmount;
            }
           
            
        })
        $("#TotalUniversityCoursefee").val(totalCourseFee);
    }

    var calculateCenterShare = function () {

        var totalCenterSharePer = 0;

        $("#DivUniversityCourseAmount [data-repeater-item]").each(function () {
            var CenterShareAmountPer = $("input[id*=CenterShareAmountPer]", $(this)).val();
            CenterShareAmountPer = parseFloat(CenterShareAmountPer) ? parseFloat(CenterShareAmountPer) : 0;
            

            var object = $("input[type=checkbox][name*=IsActive]", $(this))[0];
            if (object.checked) {
                totalCenterSharePer = totalCenterSharePer + CenterShareAmountPer;
            }


        })
        $("#TotalCenterShareAmountPer").val(totalCenterSharePer);
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        //    return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="' + $("#hdnAddEditUniversityCourseAmount").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.AddClass + '</a>' + '<a class="btn btn-outline-primary btn-sm" href="' + $("#hdnAddEditClassDetails").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.AddClass + '</a>' + '<a data-modal="" class="btn btn-outline-primary btn-sm" data-href="' + $("#hdnAddEditUniversityCourse").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm" href="#"   onclick="javascript:deleteUniversityCourse(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm context-menu"><i class="fa fa-bars" />' + Resources.Action + '</a></div>'

    }

    function resetUniversityCourseFee(rowkey, _this, remove) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationDocument,
            actionUrl: $("#hdnDeleteUniversityCourseAmount").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                setTimeout(function () {
                    $("#frmUniversityCourseAmount").closest(".modal").modal("hide")
                    UniversityCourse.CourseFeepopup(rowkey);
                }, 500)
            }
        });
    }
    return {
        GetUniversityCourse: getUniversityCourse,
        GetClassDetails: getClassDetails,
        GetUniversityCourseAmount: getUniversityCourseAmount,
        EditPopup: editPopup,
        CourseFeepopup: courseFeepopup,
        GetUniversityCourseFeeInstallments: getUniversityCourseFeeInstallments,
        CalculateInstallmentFeeTotal: calculateInstallmentFeeTotal,
        ResetUniversityCourseFee: resetUniversityCourseFee,
        CalculateUniversityfee: calculateUniversityfee,
        CalculateCenterShare: calculateCenterShare,
        FormSubmitClassDetails: formSubmitClassDetails
    }
}());

function deleteUniversityCourse(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_UniversityCourse,
        actionUrl: $("#hdnDeleteUniversityCourse").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function deleteClassdetails(rowkey, _this, remove) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_ApplicationDocument,
        actionUrl: $("#hdnDeleteClassDetails").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $(_this).fadeIn(remove);
            setTimeout(function () {
                //ApplicationFeeInstallment.CalculateInstallmentFeeTotal();
            }, 500)
        }
    });
}

function deleteUniversityCourseFee(rowkey, _this, remove) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_ApplicationDocument,
        actionUrl: $("#hdnDeleteUniversityCourseAmount").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $(_this).fadeIn(remove);
            setTimeout(function () {
                //ApplicationFeeInstallment.CalculateInstallmentFeeTotal();
            }, 500)
        }
    });
}

function UniversityCoursePopupLoad() {
    $("#CourseTypeKey").on("change", function () {
        var obj = {};
        obj.key = $(this).val() != "" ? $(this).val() : 0;
        AppCommon.BindDropDownbyId(obj, $("#hdnFillCourse").val(), $("#CourseKey"), Resources.Course);
    });
    $("#CourseKey").on("change", function () {
        var obj = {};
        obj.key = $(this).val() != "" ? $(this).val() : 0;
        AppCommon.BindDropDownbyId(obj, $("#hdnFillAcademicTerm").val(), $("#AcademicTermKey"), Resources.AcademicTerm);
        //var ddl= $("#AcademicTermKey");
        //$(ddl).html("");
        //$.ajax(
        //    {
        //        url: $("#hdnFillAcademicTerm").val(),
        //        type: "GET",
        //        dataType: "JSON",
        //        data: obj,
        //        //contentType: "application/json; charset=utf-8",
        //        success: function (result) {
        //            
        //            $.each(result.AcademicTerm, function (i, AcademicTerm) {
        //                $(ddl).append(
        //                    $('<option></option>').val(AcademicTerm.RowKey).html(AcademicTerm.Text));
        //            });


        //            $(ddl).selectpicker('refresh');

        //        }
        //    });
    });
}
//function deleteApplicationFeeinstallment(rowkey, _this, remove)
//{
//    var result = EduSuite.Confirm({
//        title: Resources.Confirmation,
//        content: Resources.Delete_Confirm_ApplicationDocument,
//        actionUrl: $("#hdnDeleteUniversityCourseFeeInstallment").val(),
//        actionValue: rowkey,
//        dataRefresh: function ()
//        {
//            $(_this).fadeIn(remove);
//            setTimeout(function ()
//            {
//                ApplicationFeeInstallment.CalculateInstallmentFeeTotal();
//            }, 500)
//        }
//    });
//}






