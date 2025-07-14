var permissionData;
var DashBoardpermissionData;
var EmployeeUserPermission = (function () {
    var getEmployeeUserPermissionById = function () {
        var response = AjaxHelper.ajax("Get", $("#hdnGetEmployeeUserPermissionById").val(),
            {

            });

        permissionData = response;
        EmployeeUserPermission.BindPerissionTree(true);
    }

    var formSubmit = function (btn, jsonData) {
        //var permissionList = $('#dvUserPermission').jstree(true).get_json('#', { flat: true })
        var countryAccessList = $('#dvCountryAccess').jstree(true).get_json('#', { flat: true })
        var branchAccessList = $('#dvBranchAccess').jstree(true).get_json('#', { flat: true })

        var data = $.extend(true, {}, jsonData || {});
        var existingList = $('#dvUserPermission').jstree(true).get_json('#', { flat: true });

        mergePermissionData(existingList);

        var DashBoardPermissionexistingList = $('#dvDashBoardPermission').jstree(true).get_json('#', { flat: true });

        mergeDashBoardPermissionData(DashBoardPermissionexistingList);
        var childrens = [];
        var DashBoardchildrens = [];
        $(permissionData["UserPermission"]).each(function () {
            $(this.children).each(function () {
                childrens.push(this);
            });

        });
        $(childrens).each(function () {
            var permission = this.data;
            var permissionState = this.state;

            if (permission && permission.mid) {
                dataItem = {};
                dataItem.RowKey = permission.key;
                dataItem.MenuKey = permission.mid;
                dataItem.ActionKey = permission.aid
                dataItem.IsActive = permissionState.selected;
                data["UserPermissions"].push(dataItem);
            }
        });


        var postDashBoardPermission = [];
        $(permissionData["DashBoardPermission"]).each(function () {
            $(this.children).each(function () {
                DashBoardchildrens.push(this);
            });

        });
        $(DashBoardchildrens).each(function () {
            var permission = this.data;
            var permissionState = this.state;

            if (permission && permission.mid) {
                dataItem = {};
                dataItem.RowKey = permission.key;
                dataItem.DashBoardTypeKey = permission.mid;
                dataItem.DashBoardContentKey = permission.aid
                dataItem.IsActive = permissionState.selected;
                postDashBoardPermission.push(dataItem);
            }
        });

        data["DashBoardPermission"] = postDashBoardPermission;

        var postCountryAccess = [];
        $(countryAccessList).each(function () {
            var countryAccess = this.data;
            var countryAccessState = this.state;
            if (countryAccess && countryAccess.mid && countryAccessState.selected) {
                postCountryAccess.push(countryAccess.mid);
            }
        })
        data["CountryAccess"] = postCountryAccess.join(",");

        var postBranchAccess = [];
        $(branchAccessList).each(function () {
            var branchAccess = this.data;
            var branchAccessState = this.state;

            if (branchAccess && branchAccess.mid && branchAccessState.selected) {
                postBranchAccess.push(branchAccess.mid);
            }
        })
        data["BranchAccess"] = postBranchAccess.join(",");
        var form = $(btn).closest("form");
        var validate = $(form).valid();
        if (validate) {
            $(".section-content").mLoading();
            $('#btnSave').hide();
            setTimeout(function () {

                response = AjaxHelper.ajax($(form)[0].method, $(form).attr("action"),
                    {
                        model: data
                    });
                if (response.IsSuccessful == true) {
                    $('#btnSave').show();
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
                                    window.location.href = $("#hdnEmployeeList").val();
                                }
                            }
                        }
                    })
                }
                $('#btnSave').show();
                $(".section-content").mLoading("destroy");
            }, 500);

        }
    }
    var bindPerissionTree = function (IsLoad) {
        if (!IsLoad) {
            var existingList = $('#dvUserPermission').jstree(true).get_json('#', { flat: true });
            $('#dvUserPermission').jstree('destroy');
            mergePermissionData(existingList);


            var DashBoardPermissionexistingList = $('#dvDashBoardPermission').jstree(true).get_json('#', { flat: true });
            $('#dvDashBoardPermission').jstree('destroy');
            mergeDashBoardPermissionData(DashBoardPermissionexistingList);

        }
        var menutypekey = $("#MenuTypeKey").val();
        var searchtext = $("#txtsearch").val();
        var UserPermission = $.extend([], true, permissionData["UserPermission"]);
        var DashBoardPermission = $.extend([], true, permissionData["DashBoardPermission"]);
        if (menutypekey)
            UserPermission = UserPermission.filter(function (item) {
                return item.type == menutypekey;
            });
        if (searchtext)
            UserPermission = UserPermission.filter(function (item) {
                return item.text.toLowerCase().indexOf(searchtext.toLowerCase()) > -1;
            });


        $("#dvUserPermission").bind("loaded.jstree", function (event, data) {
            data.instance.open_all();
        }).jstree({
            core: {
                data: UserPermission,
                "themes": {
                    "icons": false,
                    "dots": false
                },
                "dblclick_toggle": false,
            },

            "plugins": ["checkbox"]

        });

        $("#dvDashBoardPermission").bind("loaded.jstree", function (event, data) {
            data.instance.open_all();
        }).jstree({
            core: {
                data: DashBoardPermission,
                "themes": {
                    "icons": false,
                    "dots": false
                },
                "dblclick_toggle": false,
            },

            "plugins": ["checkbox"]

        });


        $("#dvCountryAccess").bind("loaded.jstree", function (event, data) {
            data.instance.open_all();
        }).jstree({
            core: {
                data: permissionData["CountryAccess"],
                "themes": {
                    "icons": false,
                    "dots": false
                },
                "dblclick_toggle": false,
            },

            "plugins": ["checkbox"]

        });
        $("#dvBranchAccess").bind("loaded.jstree", function (event, data) {
            data.instance.open_all();
        }).jstree({
            core: {
                data: permissionData["BranchAccess"],
                "themes": {
                    "icons": false,
                    "dots": false
                },
                "dblclick_toggle": false,
            },

            "plugins": ["checkbox"]

        });

    }
    return {
        GetEmployeeUserPermissionById: getEmployeeUserPermissionById,
        FormSubmit: formSubmit,
        BindPerissionTree: bindPerissionTree
    }
}());

function mergePermissionData(existingList) {
    $(permissionData["UserPermission"]).each(function () {
        var permission = this;
        var state = existingList.filter(function (item) {
            return item.id == permission.id;
        });
        if (state && state.length)
            $.extend(this.state, this.state, state[0].state);
        $(this.children).each(function () {
            permission = this;
            var state = existingList.filter(function (item) {
                return item.id == permission.id;
            });
            if (state && state.length)
                $.extend(this.state, this.state, state[0].state);
        });
    })

}

function mergeDashBoardPermissionData(existingList) {
    $(permissionData["DashBoardPermission"]).each(function () {
        var permission = this;
        var state = existingList.filter(function (item) {
            return item.id == permission.id;
        });
        if (state && state.length)
            $.extend(this.state, this.state, state[0].state);
        $(this.children).each(function () {
            permission = this;
            var state = existingList.filter(function (item) {
                return item.id == permission.id;
            });
            if (state && state.length)
                $.extend(this.state, this.state, state[0].state);
        });
    })

}
