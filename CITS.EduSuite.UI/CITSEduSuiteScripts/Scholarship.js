
var JsonData = [];

var Scholarship = (function () {

    var getScholarship = function () {
        
        //json["DateOfBirth"] = null;
        //JsonData = json;
        //JsonData["SearchName"] = $("#SearchName").val() == "" ? "" : $("#SearchName").val();
        //JsonData["SearchPhone"] = $("#SearchPhone").val() == "" ? "" : $("#SearchPhone").val();
        //JsonData["SearchFromDate"] = $("#SearchFromDate").val() == "" ? null : $("#SearchFromDate").val();
        //JsonData["SearchToDate"] = $("#SearchToDate").val() == "" ? null : $("#SearchToDate").val();
        //JsonData["SearchDistrictKey"] = $("#SearchDistrictKey").val() == "" ? null : $("#SearchDistrictKey").val();
        //JsonData["SearchBranchKey"] = $("#SearchBranchKey").val() == "" ? null : $("#SearchBranchKey").val();
        //JsonData["SearchScholarshipTypeKey"] = $("#SearchScholarshipTypeKey").val() == "" ? null : $("#SearchScholarshipTypeKey").val();


        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetScholarship").val(),
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
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.Name, Resources.MobileNo, Resources.center, Resources.City, Resources.District, Resources.Scholarship + Resources.BlankSpace + Resources.Date, Resources.Scholarship + Resources.BlankSpace + Resources.Type, Resources.EmailAddress, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'EnquiryKey', index: 'EnquiryKey', editable: true },
                { key: false, name: 'ScholarShipName', index: 'ScholarShipName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MobileNumber', index: 'MobileNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BranchName', index: 'DistrictName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'LocationName', index: 'LocationName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DistrictName', index: 'UniversityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ScholarshipDate', index: 'ScholarshipDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'ScholarshipTypeName', index: 'ScholarshipTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                 { key: false, name: 'EmailAddress', index: 'EmailAddress', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 250 },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 20, 50, 100, 250, 500, 1000],
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
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this), ScholarshipPopupLoad);
                })
                for (i = 0, count = data.rows.length; i < count; i += 1) {
                    if (data.rows[i].EnquiryKey != 0) {
                        //$("#grid").jqGrid('setSelection', data.rows[i].RowKey, false);
                        // $("input#jqg_grid_" + data.rows[i].RowKey).prop("disabled", true)
                        $("input#jqg_grid_" + data.rows[i].RowKey).remove()
                    }
                }
            },
            beforeSelectRow: function (rowid, e) {
                
                //var rowData = $("#grid").jqGrid('getRowData', rowid);
                //var selectIds = $("#grid").jqGrid("getGridParam", "selarrrow")
                //if (selectIds[0])
                //    var rowFirstData = $("#grid").jqGrid('getRowData', selectIds[0]);
                //var Isvalid = !rowFirstData || rowData.EnquiryKey == 0 ? true : false;
                //if ($("#jqg_grid_" + rowid).attr("disabled") || !Isvalid) {

                //    e.stopImmediatePropagation()
                //    $("#jqg_grid_" + rowid)[0].checked = false;
                //    return false;
                //}
                //return true;

                var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", grid);
                if (cbsdis.length === 0) {
                    return true;    // allow select the row
                } else {
                    return false;   // not allow select the row
                }

            },
            onSelectAll: function (aRowids, status) {
                
                if (status) {
                    // uncheck "protected" rows
                    var cbs = $("tr.jqgrow > td > input.cbox:disabled", grid[0]);
                    cbs.removeAttr("checked");
                }
                else
                {
                    var cbs = $("tr.jqgrow > td > input.cbox:disabled", grid[0]);
                    cbs.attr("checked");

                }
            },
            //onSelectRow: function (id) {
            //    if ($("input[id*=jqg_grid_]:checked").length == 0) {
            //        //SalesOrderMasterKey = 0;
            //    }
            //}
        })

        $("#grid").jqGrid("setLabel", "ScholarshipName", "", "thScholarshipName");
    }





    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditScholarship").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteScholarship(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>';
    }


    return {
        GetScholarship: getScholarship,


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


