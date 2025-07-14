var Party = (function () {
    var getParty = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetPartyList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                },
                PartyTypeKey: function () {
                    return $('#gridPartyType').val()
                },
                branchKey: function () {
                    return $('#BranchKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Party, Resources.ContactNumbers, Resources.Address, Resources.GSTINNo, Resources.PartyType, Resources.Location, Resources.Status, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'PartyName', index: 'PartyName', editable: true, cellEdit: true, sortable: true, resizable: false, width: 100 },
                { key: false, name: 'MobileNumber1', index: 'MobileNumber1', editable: true, cellEdit: true, sortable: true, resizable: false, width: 100 },
                { key: false, name: 'Address', index: 'Address', editable: true, cellEdit: true, sortable: true, resizable: false, width: 100 },
                { key: false, name: 'GSTINNumber', index: 'GSTINNumber', editable: true, cellEdit: true, sortable: true, resizable: false, width: 70 },
                { key: false, name: 'PartyTypeName', index: 'PartyTypeName', editable: true, cellEdit: true, sortable: true, resizable: false, width: 50 },
                { key: false, name: 'LocationName', index: 'LocationName', editable: true, cellEdit: true, sortable: true, resizable: false, width: 100 },
                { key: false, name: 'StatusName', index: 'StatusName', editable: true, cellEdit: true, sortable: true, resizable: false, width: 50 },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 80 },
            ],
            pager: jQuery('#pager'),
            rowNum: Resources.PagingRowNum,
            rowList: [10, 20, 50, 100],
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
            altRows: true,
            sortname: 'RowKey',
            sortorder: 'desc',
            altclass: 'jqgrid-altrow',
        })

        $("#grid").jqGrid("setLabel", "PartyName", "", "thPartyName");
    }

    var editPopup = function (_this) {
        var validator = null
        var url = $(_this).data("href");
        $.customPopupform.CustomPopup({

            load: function () {
                setTimeout(function () {
                    AppCommon.FormatInputCase();

                }, 500)
            },
            rebind: function () {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, url);
    }

    var getLocation = function (stateKey, Value) {

        $ddl = $("#LocationKey");
        $.ajax({
            url: $("#hdnFillLocation").val(),
            type: "GET",
            dataType: "JSON",
            data: { StateKey: stateKey },
            success: function (result) {
                $ddl.html(""); // clear before appending new list 
                $ddl.append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.Location));
                $.each(result.Locations, function (i, Location) {
                    $ddl.append(
                        $('<option></option>').val(Location.RowKey).html(Location.Text));
                });
                if (Value) {
                    $ddl.val(Value)
                }
            }
        });
    }

    var locationPopup = function () {
        var obj = {};
        obj.id = 0;
        obj.stateKey = $("#StateKey").val()
        $.customPopupform.CustomPopup({
            load: function () {
                AppCommon.FormatInputCase();
            },
            rebind: function (response) {
                Party.GetLocation($("select#StateKey", $("#frmParty")).val(), response.RowKey);

            }
        }, $("#hdnAddEditLocation").val() + '?' + $.param(obj));
    }

    var fillOtherPartyTypes = function () {
        partyType = $("#PartyTypeKey").val()
        $ddl = $("#OtherPartyTypeKeys");
        $.ajax({
            url: $("#hdnFillPartyTypeById").val(),
            type: "GET",
            dataType: "JSON",
            data: { partyType: partyType },
            success: function (result) {
                $ddl.html(""); // clear before appending new list 
                //$ddl.append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.Location));
                $.each(result.OtherPartyTypes, function (i, PartTypes) {
                    $ddl.append(
                        $('<option></option>').val(PartTypes.RowKey).html(PartTypes.Text));
                });
                $('#OtherPartyTypeKeys').multiselect('rebuild');
            }
        });
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-primary btn-xs" onclick="Party.EditPopup(this)" data-href="' + $("#hdnAddEditParty").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a>' + '</a><a  class="btn btn-danger btn-xs" href="#"  onclick="javascript:deleteParty(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i>' + Resources.Delete + '</a> </div>';
    }
    return {
        GetParty: getParty,
        EditPopup: editPopup,
        GetLocation: getLocation,
        FillOtherPartyTypes: fillOtherPartyTypes,
        LocationPopup: locationPopup
    }
}());

function deleteParty(rowkey) {
    var result = confirm(Resources.Delete_Confirm_Party);
    if (result == true) {
        var response = AjaxHelper.ajax("POST", $("#hdnDeleteParty").val(),
                    {
                        id: rowkey
                    });

        if (response.Message === Resources.Success) {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
        else
            alert(response.Message);
        event.preventDefault();
    }
}

function bindPartyForm(dialog) {
    $('#frmParty', dialog).submit(function () {
        var validate = $('#frmParty').valid();
        var form = this;
        if (validate) {
            $("[disabled=disabled]").removeAttr("disabled");
            $(form).mLoading();
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    if (result.IsSuccessful) {
                        $('#myModal').modal('hide');
                        window.location.reload();
                    } else {
                        $('#myModalContent').html(result);
                        bindPartyForm(dialog)
                    }
                    $(form).mLoading("destroy");
                }
            });

        }

        return false;

    });
}