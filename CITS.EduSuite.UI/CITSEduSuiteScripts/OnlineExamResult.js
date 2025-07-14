var ExamResults = (function () {
    var getExamResults = function () {

        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnExamResultsList").val(),
            datatype: 'json',
            page: $("#hdnPageIndex").val(),
            mtype: 'Get',
            async: true,
            styleUI: 'Bootstrap',
            postData: {

                SearchText: function () {
                    return $('#txtsearch').val()
                },


            },
            colNames: [Resources.RowKey, Resources.ApplicationKey, Resources.TestPaperKey, Resources.Name, Resources.Subject, Resources.ExamStartTime, Resources.ExamEndTime, Resources.ExamDuration, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'ApplicationKey', index: 'ApplicationKey', editable: true },
                { key: false, hidden: true, name: 'TestPaperKey', index: 'TestPaperKey', editable: true },
                { key: false, name: 'ApplicantName', index: 'ApplicantName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'SubjectName', index: 'SubjectName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ExamStart', index: 'ExamStart', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: "date", formatoptions: { srcformat: "ISO8601Long", newformat: "d/m/Y h:i A" }, },
                { key: false, name: 'ExamEnd', index: 'ExamEnd', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: "date", formatoptions: { srcformat: "ISO8601Long", newformat: "d/m/Y h:i A" }, },
                { key: false, name: 'ExamDuration', index: 'ExamDuration', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 250 },
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
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );

                $("input[name='Visa']").removeAttr("disabled");
                var GvWidth = $(".ui-jqgrid-htable").width();

                if ($('#ShowClose').prop("checked") != true) {
                    $("#grid").hideCol("ClosedAppUserName");
                }
                else {
                    $("#grid").showCol("ClosedAppUserName");
                }

                //$('#grid').trigger('reloadGrid');
                jQuery("#grid").setGridWidth(GvWidth);



            },
            onPaging: function (pgButton) {

                var NewPageIndex = 0;
                if (pgButton == "first_pager") {
                    NewPageIndex = 1;
                }
                else if (pgButton == "last_pager") {
                    NewPageIndex = $("#grid").getGridParam("lastpage");
                }
                else if (pgButton == "next_pager") {
                    NewPageIndex = parseInt($('#grid').getGridParam('page')) + parseInt(1);
                }
                else if (pgButton == "prev_pager") {
                    NewPageIndex = parseInt($('#grid').getGridParam('page')) - parseInt(1);
                }

                $("#hdnPageIndex").val(NewPageIndex);
            },

        });

        //    .contextMenu({
        //    selector: ".jqgrow .context-menu",
        //    trigger: 'left',
        //    build: function ($trigger, e) {
        //        // this callback is executed every time the menu is to be shown
        //        // its results are destroyed every time the menu is hidden
        //        // e is the original contextmenu event
        //        var $tr = $(e.target).closest("tr.jqgrow"),
        //            rowid = $tr.attr("id"),
        //            item = $("#grid").jqGrid("getRowData", rowid);


        //        var items = {
        //            E: { name: Resources.ShowResult, icon: "fa-edit" },



        //        }



        //        return {
        //            callback: function (key, options) {

        //                var href = "";
        //                switch (key) {
        //                    case "E":
        //                        href = $("#hdnShowExamResult").val() + "/" + item.TestPaperKey+'?ApplicationKey='+item.ApplicationKey;
        //                        //window.location.href = href;
        //                        let a = document.createElement('a');
        //                        //a.target = '_blank';
        //                        a.href = href;
        //                        a.click();
        //                        break;



        //                    default:
        //                        href = "";

        //                }
        //            },
        //            items: items

        //        }

        //    }
        //});


        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");


        function editLink(cellValue, options, rowdata, action) {
            //var temp = "'" + rowdata.RowKey + "'";
            //return '<div class="divEditDelete"><a class="btn btn-primary btn-sm context-menu"><i class="fa fa-bars" />' + Resources.Action + '</a></div>'
            //return '<div class="divEditDelete"><a class="btn btn-primary btn-xs" href="AddEditApplication' + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a>' + '<a class="btn btn-danger btn-xs"  onclick="javascript:deleteApplication(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> ' + Resources.Delete + ' </a><a class="btn btn-warning btn-xs" href="' + $("#hdnViewApplication").val() + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-eye" aria-hidden="true"></i>' + Resources.View + '</a></div>';

            var obj = {};
            obj.id = rowdata.RowKey;
            obj.TestPaperKey = rowdata.TestPaperKey;
            obj.ApplicationKey = rowdata.ApplicationKey;

            return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" href="' + $("#hdnShowExamResult").val() + '?' + $.param(obj) + '"><i class="fa fa-pencil" aria-hidden="true"></i></a></div>';


        }


    }


    return {
        GetExamResults: getExamResults,


    }

}());
