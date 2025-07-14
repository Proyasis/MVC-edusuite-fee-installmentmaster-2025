var Course = (function () {
    var getCourse = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetCourse").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                SearchText: function () {
                    return $('#txtsearch').val()
                },
                SearchCourseTypeKey: function () {
                    return $('#SearchCourseTypeKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.RowKey, Resources.RowKey, Resources.RowKey, Resources.CourseType, Resources.Name, Resources.Code, Resources.Duration, Resources.IsActive, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'UniversityCourseKey', index: 'UniversityCourseKey', editable: true },
                { key: false, hidden: true, name: 'DurationCount', index: 'DurationCount', editable: true },
                { key: false, hidden: true, name: 'DurationTypeKey', index: 'DurationTypeKey', editable: true },
                { key: false, name: 'CourseTypeName', index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseCode', index: 'CourseCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseDuration', index: 'CourseDuration', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatMonth },
                //{ key: false, name: 'CourseYear', index: 'CourseYear', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsActiveText', index: 'IsActiveText', editable: true, cellEdit: true, sortable: false, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
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
            altclass: 'jqgrid-altrow',
            rownumbers: false,
            //loadComplete: function (data) {
            //    $("#grid a[data-modal='']").each(function () {
            //        AppCommon.EditGridPopup($(this))
            //    })

            //}
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

                var items = {
                    E: { name: Resources.Edit, icon: "fa-edit" },
                    CF: { name: Resources.AddCourseFee, icon: "fa-inr" },
                    C: { name: Resources.AddClass, icon: "fa-graduation-cap" },
                    //IF: { name: Resources.AddInstallmentFee, icon: "fa-inr" },
                    D: { name: Resources.Delete, icon: "fa-trash" }

                }
                var IsMultipleUniversity = $("#IsMultipleUniversity").val();
                IsMultipleUniversity = IsMultipleUniversity ? JSON.parse(IsMultipleUniversity.toLowerCase()) : false;
                if (IsMultipleUniversity) {
                    delete items.C;
                    delete items.CF;
                }
                return {
                    callback: function (key, options) {

                        var href = "";
                        switch (key) {
                            case "E":
                                Course.EditPopup(rowid);
                                break;
                            case "CF":
                                UniversityCourse.CourseFeepopup(item.UniversityCourseKey);
                                break;
                            case "C":
                                href = $("#hdnAddEditClassDetails").val() + "/" + item.UniversityCourseKey
                                window.location.href = href;
                                break;
                            //case "IF":
                            //    href = $("#hdnAddEditUniverstyCourseFeeInstallment").val() + "/" + rowid
                            //    window.location.href = href;
                            //    break;
                            case "D":
                                deleteCourse(rowid, item.UniversityCourseKey);
                                break;

                            default:
                                href = "";

                        }
                    },
                    items: items
                }

            }
        });

        $("#grid").jqGrid("setLabel", "CourseName", "", "thCourseName");
    }


    function editLink(cellValue, options, rowdata, action) {
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm context-menu"><i class="fa fa-bars" />' + Resources.Action + '</a></div>'

        //    var temp = "'" + rowdata.RowKey + "'";
        //    return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditCourse").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:deleteCourse(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }
    function formatMonth(cellValue, option, rowdata, action) {
        
        if (rowdata.DurationTypeKey == Resources.DurationTypeYear) {
            return rowdata.DurationCount + " Years";
        }
        else if (rowdata.DurationTypeKey == Resources.DurationTypeDays) {
            return rowdata.DurationCount + " Days";
        }
        else {
            return rowdata.DurationCount + " Months";
        }
    }
    var editPopup = function (rowid) {

        var URL = $("#hdnAddEditCourse").val() + "?id=" + rowid;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            load: function () {
                AppCommon.FormatInputCase();

            },
            rebind: function (result) {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, URL);
    }

    var checkDurationType = function () {

        var Duration = $("#DurationCount").val();
        Duration = parseInt(Duration) ? parseInt(Duration) : null;
        var DurationTypeKey = $("#DurationTypeKey").val();

        if (DurationTypeKey == Resources.DurationTypeYear) {
            $("#lblCourseDuration").html("Course Duration ( Year )");
            if (Duration > 5) {
                $("#DurationCount").val(5);
            }
        }
        else if (DurationTypeKey == Resources.DurationTypeDays) {
            $("#lblCourseDuration").html("Course Duration ( Days )");
            if (Duration > 200) {
                $("#DurationCount").val(200);
            }
        }
        else {
            $("#lblCourseDuration").html("Course Duration ( Months )");
            if (Duration > 60) {
                $("#DurationCount").val(60);
            }
        }

    }

    return {
        GetCourse: getCourse,
        EditPopup: editPopup,
        CheckDurationType: checkDurationType
    }
}());

function deleteCourse(rowkey, UniversityCourseKey) {
    var obj = {};

    obj.id = rowkey;
    obj.UniversityCourseKey = UniversityCourseKey;

    var result = EduSuite.Confirm2({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Course,
        actionUrl: $("#hdnDeleteCourse").val(),
        parameters: obj,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}






