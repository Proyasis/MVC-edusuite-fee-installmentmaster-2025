var Country = (function () {

    var getCountry = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetCountryList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()

                }
            },
            colNames: [Resources.RowKey, Resources.Country + Resources.BlankSpace + Resources.Name, Resources.Short + Resources.BlankSpace + Resources.Name, Resources.Nationality, Resources.Language, Resources.Telephone + Resources.BlankSpace + Resources.Code, Resources.Display + Resources.BlankSpace + Resources.Order, Resources.IsActive, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'CountryName', index: 'CountryName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'CountryNameLocal', index: 'CountryNameLocal', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CountryShortName', index: 'CountryShortName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NationalityName', index: 'NationalityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'LanguageName', index: 'LanguageName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'CapitalCityName', index: 'CapitalCityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TelephoneCode', index: 'TelephoneCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DisplayOrder', index: 'DisplayOrder', editable: true, cellEdit: true, sortable: true, resizable: false },
                 { key: false, name: 'IsActiveText', index: 'IsActiveText', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
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
            loadonce: true,
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this));
                })
            }
        })

        $("#grid").jqGrid("setLabel", "CountryName", "", "thCountryName");
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";""
        return '<div class="divEditDelete"><a class="btn btn-primary btn-sm" data-modal="" data-href="' + $("#hdnAddEditCountry").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a>' + '<a class="btn btn-danger btn-sm" onclick="javascript:deleteCountry(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> ' + Resources.Delete + ' </a>';
    }
    return {
        GetCountry: getCountry
    }
}());

function deleteCountry(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Country,
        actionUrl: $("#hdnDeleteCountry").val(),
        actionValue: rowkey,
        dataRefresh: function () {$("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');}
    });
}
