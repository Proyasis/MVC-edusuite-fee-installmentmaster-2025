var ajaxRequest = null;
var MemberPlanDetails = (function () {

    var getMemberPlanDetails = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetMemberPlanDetailsList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                },
                BranchKey: function () {
                    return $('#BranchKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.Member, Resources.CardId, Resources.ApplicationType, Resources.MemberType, Resources.BorrowerType + Resources.BlankSpace + Resources.Name, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'ApplicationTypeKey', index: 'ApplicationTypeKey', editable: true },
                { key: false, name: 'MemberFullName', index: 'MemberFullName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CardId', index: 'CardId', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ApplicationTypeName', index: 'ApplicationTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MemberTypeName', index: 'MemberTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BorrowerTypeName', index: 'BorrowerTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },

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
            loadonce: true,
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this))
                })

            }
        })

        $("#grid").jqGrid("setLabel", "MemberPlanDetailsName", "", "thMemberPlanDetailsName");
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        if (rowdata.ApplicationTypeKey == Resources.ApplicationTypeOther) {
            return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditMemberPlanDetails").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a></div>';
        }
        else {
            return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditMemberPlanDetails").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteMemberPlanDetails(' + temp + ');return false;" ><i class="fa fa-trash pointer" aria-hidden="true"></i></a></div>';
        }
    }

    var loadData = function (pageIndex, resetPagination) {


        var model = {};
        $("#dynamicRepeater").mLoading();

        model.CourseKey = $("#CourseKey").val();
        model.UniversityMasterKey = $("#UniversityMasterKey").val();
        model.BatchKey = $("#BatchKey").val();
        model.BranchKey = $("#BranchKey").val();
        model.ApplicationTypeKey = $("#tab-ApplicationType li a.active").data('val');
        model.SearchText = $("#txtsearch").val();
        model["PageIndex"] = pageIndex ? pageIndex : 1;
        model["PageSize"] = 10;
        ajaxRequest = $.ajax({
            type: "POST",
            url: $("#hdnGetMemberDetails").val(),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(model),
            beforeSend: function () {
                if (ajaxRequest != null) {
                    ajaxRequest.abort();
                }
            },
            success: function (result) {
                if (result.IsSuccessful == false) {
                    $("[data-valmsg-for=error_msg]").html(result.Message);

                }
                $("form").removeData("validator");
                $("form").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("form");

                $("#dvStudentsList").html("")
                $("#dvStudentsList").html(result);
                $("#TotalRecords").html($("#hdnTotalRecords").val());

                if (resetPagination) {
                    MemberPagination();
                }
            },
            error: function (request, status, error) {

            }
        });


    }


    function formSubmitMemberPlan() {

        var $form = $("#frmAddEditMemberPlans")
        var JsonData = [];

        var checkdata = $("[name*=CheckStatus]:checked")

        var formData = $form.serializeToJSON({ associativeArrays: false });
        formData = formData[""].filter(function (item) {
            return item.CheckStatus;
        });;



        if ($form.valid()) {
            if (checkdata.length > 0) {
                $form.mLoading();
                var dataurl = $form.attr("action");
                var response = [];

                AjaxHelper.ajaxAsync("POST", dataurl,
                    {
                        model: formData
                    }, function () {
                        response = this;
                        if (typeof response == "string") {
                            $("[data-valmsg-for=error_msg]").html(response);
                        }
                        else if (response.IsSuccessful) {
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
                                            window.location.href = $("#hdnMemberPlanDetailsList").val();
                                        }
                                    }
                                }
                            })

                        }
                        $form.mLoading("destroy");
                    });

            }
            else {
                $.alert({
                    type: 'orange',
                    title: Resources.Warning,
                    content: 'Please Select atleast One !',
                    icon: 'fa fa-exclamation-circle',
                    buttons: {
                        Ok: {
                            text: Resources.Ok,
                            btnClass: 'btn-warning',

                        }
                    }
                })

            }

        }
    }


    return {
        GetMemberPlanDetails: getMemberPlanDetails,
        LoadData: loadData,
        FormSubmitMemberPlan: formSubmitMemberPlan
    }

}());

function deleteMemberPlanDetails(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_MemberPlanDetails,
        actionUrl: $("#hdnDeleteMemberPlanDetails").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}


function MemberPagination() {
    $('#page-selection-up,#page-selection-down').empty();
    var totalRecords = $("#hdnTotalRecords").val();
    totalRecords = totalRecords != "" ? parseInt(totalRecords) : 0;
    var Size = 10;
    var totalPages = Math.floor(totalRecords % Size == 0 ? totalRecords / Size : (totalRecords / Size) + 1);

    $('#page-selection-up,#page-selection-down').bootpag({
        total: totalPages,
        page: 1,
        maxVisible: 10
    });
    $('#page-selection-up').on("page", function (event, num) {
        MemberPlanDetails.LoadData(num, false);
    });

}