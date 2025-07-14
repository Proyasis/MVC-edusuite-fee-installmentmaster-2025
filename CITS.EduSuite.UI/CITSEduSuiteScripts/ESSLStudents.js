var request = null;
var ESSLStudents = (function () {

    var getESSLStudents = function () {

        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');

        $("#grid").jqGrid({
            url: $("#hdnGetESSLStudent").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {

                EmployeeCode: function () {
                    return $('#txtSearchApplicantName').val()
                },               
                IsConnected: function () {
                    return $("input[type=checkbox][name*=IsConnected]").prop("checked")
                }
            },
            colNames: [Resources.RowKey, Resources.RowKey, Resources.Name, Resources.Code,
                Resources.Gender, Resources.Status, Resources.EnquiryStatus, Resources.AdmissionNo//, Resources.Action
            ],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'IsConnected', index: 'IsConnected', editable: true },
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'EmployeeCode', index: 'EmployeeCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Gender', index: 'Gender', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Status', index: 'Status', editable: true, cellEdit: true, sortable: true, resizable: false, width: 250 },
                { key: false, name: 'number', index: 'number', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatFormStatus },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
              
               /* { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 100, align: 'center' },*/

            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 50, 100, 250, 500, 1000],
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
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );

            }
        })

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }

    
    function formatFormStatus(cellValue, option, rowdata, action) {
        cellValue = cellValue ? cellValue : 0;
        if (rowdata.IsConnected) {
            return '<span  >' + "Fetched Application" + '</span>';
        }
        else {
            return '<span  > ' + "Pending" + '</span>';
        }
        return cellValue;
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        //return '<div class="divEditDelete"><button type="button" class="btn btn-outline-primary btn-sm context-menu"><i class="fa fa-bars" />' + Resources.Action + '</button></div>'
        //return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="AddEditApplication' + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm"  onclick="javascript:deleteApplication(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a><a class="btn btn-warning btn-sm" href="' + $("#hdnViewApplication").val() + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-eye" aria-hidden="true"></i>' + Resources.View + '</a></div>';
        return "";
    }

    
    return {
        GetESSLStudents: getESSLStudents,
      
    }

}());

