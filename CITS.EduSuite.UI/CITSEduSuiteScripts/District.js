var District = (function () {

    var getDistrict = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetDistrictList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()

                }
            },
            colNames: [Resources.RowKey, Resources.District + Resources.BlankSpace + Resources.Name, Resources.Country, Resources.Province, Resources.IsActive, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'Districtname', index: 'Districtname', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'DistrictnameLocal', index: 'DistrictnameLocal', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CountryName', index: 'CountryName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ProvinceName', index: 'ProvinceName', editable: true, cellEdit: true, sortable: true, resizable: false },
                 //{ key: false, name: 'DisplayOrder', index: 'DisplayOrder', editable: true, cellEdit: true, sortable: true, resizable: false },
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

        $("#grid").jqGrid("setLabel", "DistrictName", "", "thDistrictName");
    }

    var getProvinceById = function (Id) {

        var ddl = $("#ddlProvince");
        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetProvinceByCountry").val(), ddl, Resources.Province, "Provinces");
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<a class="btn btn-primary btn-sm mx-1" onclick="District.EditPopup(this)" data-href="' + $("#hdnAddEditDistrict").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>'
            + '<a class="btn btn-danger btn-sm mx-1" onclick="javascript:deleteDistrict(' + temp + ');return false;" ><i class="fa fa-trash pointer" aria-hidden="true"></i></a>';
    }
    var editPopup = function (_this) {


        var url = $(_this).attr("data-href");
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {
                setTimeout(function () {
                    DistrictPopupLoad()
                }, 500);
            },
            rebind: function (result) {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, url);
    }
    return {
        GetDistrict: getDistrict,
        GetProvinceById: getProvinceById,
        EditPopup: editPopup
    }
}());

function deleteDistrict(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_District,
        actionUrl: $("#hdnDeleteDistrict").val(),
        actionValue: rowkey,
        dataRefresh: function () {$("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');}
    });
}

function DistrictPopupLoad() {
    AppCommon.FormatInputCase();
    $("#ddlCountry").on("change", function () {
        District.GetProvinceById($(this).val());
    });
}