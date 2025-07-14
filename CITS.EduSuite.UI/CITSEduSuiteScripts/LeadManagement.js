var Leads = (function () {

    var getLeads = function () {
        $(".LoadError").html("");
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetLeads").val(),
            datatype: 'json',
            mtype: 'Get',
            styleUI: 'Bootstrap',
            postData: {

                SearchText: function () {
                    return $('#SearchText').val();
                },
              
                IsNewLead: function () {
                    return $(".LeadStatusFilter .active").attr("value");
                },
                SearchLeadStatusKey: function () {
                    return $('#SearchEnquiryLeadStatusKey').val(); 
                },
                SearchDateAddedFrom: function () {
                    return $('#SearchFromDate').val();
                },

                SearchDateAddedTo: function () {
                    return $('#SearchToDate').val();
                },



            },
           // colNames: [Resources.RowKey, Resources.Action, Resources.AdName, Resources.NatureOfEnquiry ,Resources.Name, Resources.MobileNumber, Resources.Email, Resources.Branch, Resources.Employee, Resources.ServiceType, Resources.Location, Resources.Status, Resources.CreatedOn],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true, label: Resources.RowKey },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 50, label: Resources.Action },
                { key: false, name: 'AdName', index: 'AdName', editable: true, cellEdit: true, sortable: true, resizable: false, label: Resources.AdName },
                { key: false, name: 'NatureOfEnquiryName', index: 'NatureOfEnquiryName', editable: true, cellEdit: true, sortable: true, resizable: false, label: Resources.NatureOfEnquiry },
                { key: false, name: 'Name', index: 'Name', editable: true, cellEdit: true, sortable: true, resizable: false, label: Resources.Name },
                { key: false, name: 'MobileNumber', index: 'MobileNumber', editable: true, cellEdit: true, sortable: false, resizable: false, label: Resources.MobileNumber },
                { key: false, name: 'EmailAddress', index: 'EmailAddress', editable: true, cellEdit: true, sortable: false, resizable: false, label: Resources.Email },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: false, resizable: false, label: Resources.Branch },              
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: false, resizable: false, label: Resources.Employee },
                { key: false, name: 'Location', index: 'Location', editable: true, cellEdit: true, sortable: false, resizable: false, label: Resources.Location },
                { key: false, name: 'EnquiryLeadStatusName', index: 'EnquiryLeadStatusName', editable: true, cellEdit: true, sortable: false, resizable: false, label: Resources.Status },
                { key: false, name: 'DateAddedTxt', index: 'DateAdded', editable: true, cellEdit: true, sortable: false, resizable: false, label:Resources.CreatedOn},
             
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
            autowidth: true,
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
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this))
                })


                selection.CheckList();

            },
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
              

                //$(function () {
                //    $(".listPreview").draggable();
                //});

            },
            loadError: function (jqXHR, textStatus, errorThrown) {
                $(".LoadError").html("Cannot Connect to Server. Please check your Internet Connection");
            },
            onCellSelect: function (rowId, iCol, content, event) {

                if (iCol != 12) {
                    $(".listPreview").hide();
                }

                //if (ids == null)
                //{
                //    ids = 0;
                //    if (jQuery("#list10_d").jqGrid('getGridParam', 'records') > 0) {
                //        jQuery("#list10_d").jqGrid('setGridParam', { url: "subgrid.php?q=1&id=" + ids, page: 1 });
                //        jQuery("#list10_d").jqGrid('setCaption', "Invoice Detail: " + ids)
                //            .trigger('reloadGrid');
                //    }
                //} else {
                //    jQuery("#list10_d").jqGrid('setGridParam', { url: "subgrid.php?q=1&id=" + ids, page: 1 });
                //    jQuery("#list10_d").jqGrid('setCaption', "Invoice Detail: " + ids)
                //        .trigger('reloadGrid');
                //}
            }


        })

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }
    //function editLink(cellValue, options, rowdata, action) {
    //    var temp = "'" + rowdata.RowKey + "'"; ""
    //    return '<div class="divEditDelete"><a class="btn btn-primary btn-sm" data-modal="" data-href="' + $("#hdnaddEditClientPages").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a>' + '<a class="btn btn-danger btn-sm" onclick="javascript:deleteClientPages(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> ' + Resources.Delete + ' </a>';
    //}



    function LeadStatus(cellValue, options, rowdata, action) {
        if (rowdata.LeadStatusKey == 1) {

            return '<span style="color:red">' + rowdata.LeadStatusName + '</span> <a class="btnReTransfer" id="' + rowdata.LeadgenId + '"  onclick="Retransfer(' + rowdata.LeadgenId + ',' + rowdata.RowKey + ')" >   Re-transfer</a> ';
        }
        else if (rowdata.LeadStatusKey == 2) {
            return '<span style="color:green">' + rowdata.LeadStatusName + '</span>';
        }
        return '';

    }

    function LeadStatusColor(cellValue, options, rowdata, action) {
        if (rowdata.LeadStatusKey == 1) {

            return '<span style="color:red">' + rowdata.LeadStatusName + '</span>  ';
        }
        else if (rowdata.LeadStatusKey == 2) {
            return '<span style="color:green">' + rowdata.LeadStatusName + '</span>';
        }
        return '';

    }

  

    function editLink(cellValue, options, rowdata, action) {

        if (rowdata.EmployeeKey == null) {
            return '  <input class="addLead" type="checkbox" selection_value="' + rowdata.RowKey + '"  text="' + rowdata.Name + ' ' + rowdata.MobileNumber + '" id="row_' + rowdata.RowKey + '" value="' + rowdata.RowKey + '" />  ';
        }

        else {
            return ' <input disabled="disabled" class="addLead" type="checkbox" selection_value="' + rowdata.RowKey + '"  text="' + rowdata.Name + ' ' + rowdata.MobileNumber + '" id="row_' + rowdata.RowKey + '" value="' + rowdata.RowKey + '" />  ';


        }
    }



    var getEmployeesByBranchId = function (Id, ddl) {
        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetEmployeesByBranchId").val(), ddl, Resources.Employee);

    }





    return {
        GetLeads: getLeads,
        GetEmployeesByBranchId: getEmployeesByBranchId
    }

}());
function deleteClientPages(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Application,
        actionUrl: $("#hdnDeleteClientPages").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}



function Retransfer(LeadgenID, RowKey) {
    $("#" + LeadgenID).html('<i class="fa fa-circle-o-notch fa-spin fa-3x fa-fw"></i> Transfering');
    var obj = {};
    obj.LeadgenId = LeadgenID;
    obj.RowKey = RowKey;
    $.ajax({
        url: $("#hdnTryReTransfer").val(),
        type: "GET",
        data: obj,

        complete: function (jqxhr, txt_status) {
            if (jqxhr.status == 200) {
                $("#" + LeadgenID).html('<i class="fa fa-check" aria-hidden="true"></i> success');
            }
            else {
                $("#" + LeadgenID).html('Re-try ');
            }

            console.log("Complete: [ " + txt_status + " ] " + jqxhr);


        }
    });

}



function RetransferAll() {

    var obj = {};
    obj.retransfer = 0;

    $.ajax({
        url: $("#hdnTryReTransferAll").val(),
        type: "GET",
        data: obj,
        complete: function (jqxhr, txt_status) {




        }
    });

}



function showAll(key) {
    $(".listPreview").hide();
    $("#data_" + key).show();
}


