var Templates = {
    QuestionSectionItem: "<li class='nav-item position-relative'><a href='#{{RowKey}}' aria-controls='discover' role='tab' class='nav-link {{IsActive}}' data-toggle='tab' data-val='{{RowKey}}'>" +
    "<span class='nav-link-in'>{{TestSectionName}}</span></a> <i class='fa fa-trash text-danger export-buttons' onclick='deleteTestSection(this)'></i></li>"
};
var MultiOptions = [Resources.QuestionModuleListening, Resources.QuestionModuleReading];
var TestPaper = (function () {

    var getTestPaper = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetTestPaper").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()

                }
            },
            colNames: [Resources.RowKey, Resources.TestPaperName, Resources.ExamDate, Resources.ExamDuration, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                //{ key: false, name: 'TestPaperName', index: 'TestPaperName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TestPaperName', index: 'TestPaperName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ExamDateTime', index: 'ExamDateTime', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TestDuration', index: 'TestDuration', editable: true, cellEdit: true, sortable: true, resizable: false },
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
            altclass: 'jqgrid-altrow'
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
                return {
                    callback: function (key, options) {

                        var href = "";
                        switch (key) {
                            case "E":
                                href = $("#hdnAddEditTestPaper").val() + "/" + rowid
                                window.location.href = href;
                                break;
                            case "D":
                                deleteTestPaper(rowid);
                                break;
                            case "I":
                                window.location.href = $("#hdnAddEditTestInstruction").val() + "/" + rowid;

                                break;
                            case "A":
                                window.location.href = $("#hdnAddEditAnswerKey").val() + "/" + rowid;

                                break;

                            default:
                                href = "";

                        }
                    },
                    items: {
                        E: { name: Resources.Edit, icon: "fa-edit" },
                        D: { name: Resources.Delete, icon: "fa-trash" },
                        //I: { name: Resources.Instructions, icon: "fa-sticky-note" },
                        //A: { name: Resources.AnswerKey, icon: "fa-key" }

                    }
                }

            }
        });

        $("#grid").jqGrid("setLabel", "CountryName", "", "thCountryName");
    }
    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a  class="btn btn-primary btn-sm context-menu"><i class="fa fa-bars" />' + Resources.Action + '</a></div>'
        //return '<div class="divEditDelete"><a class="btn btn-primary btn-sm" href="AddEditApplication' + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a>' + '<a class="btn btn-danger btn-sm"  onclick="javascript:deleteApplication(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> ' + Resources.Delete + ' </a><a class="btn btn-warning btn-sm" href="' + $("#hdnViewApplication").val() + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-eye" aria-hidden="true"></i>' + Resources.View + '</a></div>';

        //var temp = "'" + rowdata.RowKey + "'"; ""
        //return '<div class="divEditDelete"><a class="btn btn-primary btn-sm mx-1" href="' + $("#hdnAddEditTestPaper").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-danger btn-sm" onclick="javascript:deleteTestPaper(' + temp + ');return false;" ><i class="fa fa-trash pointer" aria-hidden="true"></i></a></div>';
    }
    var updateQuestion = function (data) {
        var ed = tinymce.activeEditor;
        if (!data.IsUpdate) {
            var MaxValue = 0;
            var LastQuestionNumber = $("#LastQuestionNumber").val();
            var buttonLength = $("a.btn[data-question]").length;
            if (buttonLength > 0)
                MaxValue = TestPaper.GetQuestionEnd($("#testPaperQuestion"));
            LastQuestionNumber = parseInt(LastQuestionNumber) ? parseInt(LastQuestionNumber) : 0;
            MaxValue = parseInt(MaxValue) ? parseInt(MaxValue) : 0;

            MaxValue = MaxValue ? MaxValue + 1 : (MaxValue + LastQuestionNumber) + 1;
            data.QuestionNumber = MaxValue;
            data.index = buttonLength;
        };
        var OldQuestionNumber = data.QuestionNumber;
        if (data.NewQuestionNumber) {
            var OldQuestionNumber = data.QuestionNumber;
            data.QuestionNumber = data.NewQuestionNumber;
        }
        var response = Templates["QuestionTypePath" + data.QuestionTypeKey]
        var template = Handlebars.compile(response);
        var html = template(data);
        if (data.IsUpdate) {
            var old = $("[data-question=" + OldQuestionNumber + "]", ed.dom.doc);
            $(old).replaceWith($(html));

        } else {
            $('#testPaperContent').tinymce().execCommand('mceInsertContent', false, html);
        }

        data.IsEdit = true;
        var response = Templates.QuestionPath;
        var IsUpdate
        var TestQuestions = [];
        TestQuestions.push(data)
        AppCommon.HandleBarHelpers();
        var template = Handlebars.compile(response);
        var html = template({ TestQuestions: TestQuestions });
        if (!data.IsUpdate) {
            $('#testPaperQuestion').append(html);
        }
        else {
            $("[data-question=" + OldQuestionNumber + "]", $('#testPaperQuestion')).replaceWith($(html));
        }
        TestPaper.ShowHideDeleteButton();
        var ed = tinymce.activeEditor;

    }
    var getQuestionStart = function (item) {

        var arr = $(item).find("a.btn").toArray().map(function (item) {
            var dataset = item.dataset;
            var qtn = dataset.question;

            qtn = parseInt(qtn) ? parseInt(qtn) : 0;
            return qtn;
        }).filter(function (item) {

            return item;
        });
        return arr.length ? Math.min.apply(Math, arr) : 0;
    }
    var getQuestionEnd = function (item) {
        var arr = $(item).find("a.btn").toArray().map(function (item) {
            var dataset = item.dataset;
            var qtn = dataset.question;
            qtn = parseInt(qtn) ? parseInt(qtn) : 0;
            return qtn;
        }).filter(function (item) {

            return item;
        });
        return arr.length ? Math.max.apply(Math, arr) : 0;

    }
    var setQuestionNumber = function (type, value, data) {


        var legends = $("#testPaperQuestion").find("legend");
        $(legends).each(function () {
            var min = TestPaper.GetQuestionStart($(this).closest("fieldset"));
            var max = TestPaper.GetQuestionEnd($(this).closest("fieldset"));
            var qgData = {};
            qgData.QuestionStart = min;
            qgData.QuestionEnd = max;
            var qgtemplate = Handlebars.compile(Templates.QuestionGroupHead);
            var qghtml = qgtemplate(qgData);
            $(this).html(qghtml)

            var ed = tinymce.activeEditor;
            var legend = $("#" + $(this).prop("id"), ed.dom.doc);
            $(legend).html(qghtml);
            var index = $(this).closest("fieldset").index();
            $("[name*=QuestionStart]", $(this).closest("fieldset")).val(min)
            $("[name*=QuestionEnd]", $(this).closest("fieldset")).val(max)
            $("[name*=QuestionTypeKey]", $(this).closest("fieldset")).val(type)
            //$("[name*=QuestionStart]", $(this).closest("fieldset")).prop("name", "TestQuestions[" + index + "].QuestionStart")
            //$("[name*=QuestionEnd]", $(this).closest("fieldset")).prop("name", "TestQuestions[" + index + "].QuestionEnd")

            //$("[name*=RowKey]", $(this).closest("fieldset")).prop("name", "TestQuestions[" + index + "].RowKey")

        })
        TestPaper.ShowHideDeleteButton();
    }
    var optionClick = function (type, _this) {
        var ed = tinymce.activeEditor;
        var data = $.extend({}, true, jsonDetailData);

        if (_this) {
            var btn = $(_this).closest("[data-question]")
            data.RowKey = $("[name*=RowKey]", btn).val();
            data.RowKey = parseInt(data.RowKey) ? parseInt(data.RowKey) : 0;
            data.QuestionTypeKey = $("[name*=QuestionTypeKey]", btn).val();
            data.QuestionTypeKey = parseInt(data.QuestionTypeKey) ? parseInt(data.QuestionTypeKey) : 0;
            data.QuestionNumber = $("[name*=QuestionNumber]", btn).val();
            data.QuestionNumber = parseInt(data.QuestionNumber) ? parseInt(data.QuestionNumber) : 0;
            data.index = $(btn).index();
            data.QuestionOptions = $("[data-question=" + data.QuestionNumber + "]", ed.dom.doc).find("input").toArray().map(function (item) {
                var label = $(item).next();
                var value = $(item).val();
                var text = $(label).data("val");
                return { Text: text, Value: value };
            });
            if (!data.QuestionOptions.length) {
                data.QuestionOptions = [{ Text: "", Value: "" }]
            }

        } else {
            data.QuestionTypeKey = type;
            if (type == DbConstants.QuestionType.Optional) {
                data.QuestionOptions = [{ Text: "", Value: "" }]
            }
        }
        data.QuestionTypes = $(QuestionTypesData).map(function (i, item) {
            item.Selected = item.RowKey == data.QuestionTypeKey ? true : null;
            return item;
        }).toArray();
        data.IsUpdate = _this ? true : false;
        if (type == DbConstants.QuestionType.Optional || _this) {
            var url = $("#hdnQuestionOption").val();
            $.ajax({
                type: 'GET',
                crossDomain: true,
                url: url,
                success: function (response) {
                    var template = Handlebars.compile(response);
                    var newhtml = template(data);
                    $.customPopupform.CustomPopup(
                        {
                            html: newhtml,
                            modalsize: "modal-md",
                            load: function (dailog) {
                                setTimeout(function () {
                                    AppCommon.FormatInputCase();
                                    $("#qtnGroup").repeater(
                                        {
                                            show: function () {
                                                $(this).slideDown(function () {
                                                    AppCommon.FormatInputCase();
                                                });

                                            },
                                            hide: function (remove) {

                                                $(this).slideUp(remove);

                                            },
                                            repeatlist: 'QuestionOptions',
                                            submitButton: ''
                                        });
                                    var value = $("[id*=rblQuestionType]:checked").val();
                                    if (value == DbConstants.QuestionType.Optional) {
                                        $("#qtnGroup").show();
                                    }
                                    else {
                                        $("#qtnGroup").hide();
                                    }
                                    $("[id*=rblQuestionType]", dailog).on("change", function () {
                                        if (!$(this).is(":checked")) {
                                            $(this).prop("checked", true);
                                        }
                                        var value = $(this).val();
                                        if (value == DbConstants.QuestionType.Optional) {
                                            $("#qtnGroup").show();
                                        }
                                        else {
                                            $("#qtnGroup").hide();
                                        }
                                    })
                                    $("#btnSave", dailog).on("click", function () {
                                        var $form = $(this).closest("form");
                                        var Json = $form.serializeToJSON({ associativeArrays: false });
                                        data = Json;
                                        data.index = $(btn).index();
                                        data.IsUpdate = _this ? true : false;
                                        TestPaper.UpdateQuestion(data)


                                        $(this).closest(".modal").modal("hide")
                                    })

                                }, 500)

                            }


                        });
                },
                error: function (xhr) {

                },
                complete: function () {

                }
            })
        } else {
            TestPaper.UpdateQuestion(data)
        }

    }

    var questionPaperDetail = function (obj)
    {
        //$("#dvLoading").mLoading();
        //if (!obj) {
        //    obj = {};
        //    obj.id = $("#RowKey").val();
        //    obj.ModuleKey = $("#tab-questionmodule li a.active").data("val");
        //    obj.SectionKey = $("#tab-questionsection li a.active").data("val");
        //    $("#IsActive").prop("checked", true);
        //    $("#MarkGroupKey").val("").selectpicker('refresh');
        //}



        //$.ajax({
        //    url: $("#hdnQuestionPaperDetail").val(),
        //    type: "GET",
        //    dataType: "JSON",
        //    async: true,
        //    data: obj,
        //    success: function (response) {
        //        $("#RowKey").val(response.RowKey);
        //        $("#SubjectKey").val(response.SubjectKey).selectpicker("refresh");
        //        $("#TestSectionKey").val(response.TestSectionKey);
        //        $("#TestModuleKey").val(response.TestModuleKey);
        //        $("#TestPaperName").val(response.TestPaperName);
        //        $("#TestPaperName").val(response.TestPaperName);
        //        $("#SupportedFileName").val(response.SupportedFileName);
        //        $("#QuestionPaperFileName").val(response.QuestionPaperFileName);
        //        $("#LastQuestionNumber").val(response.LastQuestionNumber);
        //        $("#ExamDate").val(AppCommon.JsonDateToNormalDate2(response.ExamDate));
        //        $("#ExamTime").val(AppCommon.FormatObjectToTimeAMPM(response.ExamTime));
        //        $("#TestDuration").val(response.TestDuration);
        //        $("#MarkGroupKey").val(response.MarkGroupKey);
        //        $("#IsActive").prop("checked", response.IsActive);
        //        $("#PlanKeys").val(response.PlanKeys).selectpicker("refresh");
        //        $("#MarkGroupKey").val(response.MarkGroupKey).selectpicker('refresh');
        //        if (response.SupportedFileName) {
        //            $("#audSupportedFile")[0].src = ($("#audSupportedFile")[0].dataset.src + obj.ModuleKey + "/" + response.SupportedFileName).replace("~", GlobalConstants.FullPath) + "?no-cache=" + new Date().getTime();;
        //            $("#audSupportedFile").show();
        //        } else {
        //            $("#audSupportedFile").hide();
        //        }

        //        TestPaper.FillTestSection(response.QuestionSections, response.TestSectionKey);
        //        TestPaper.FillQuestionNumbers(response);
        //        TestPaper.FillQuestionPaper(response.QuestionPaperFileName ? (UrlConstants.TestQuestionPaperUrl + response.RowKey + "/" + obj.ModuleKey + "/" + response.QuestionPaperFileName) : null);


        //    }
        //});
    }
    var fillTestSection = function (data, SectionKey) {
        $("#addTestSection").prevAll("li.nav-item").remove();
        $(data).each(function (i, item) {
            var sctemplate = Handlebars.compile(Templates.QuestionSectionItem);
            var schtml = sctemplate(item);

            $("#addTestSection").before(schtml)
        })
        TestPaper.ShowHideDeleteButton();
        $("#tab-questionsection li a").removeClass("active");
        $("#tab-questionsection li a[data-val=" + (SectionKey ? SectionKey : 0) + "]").addClass("active");
        $('#tab-questionsection li a[data-toggle="tab"]').off('shown.bs.tab').on('shown.bs.tab', function (e) {

            TestPaper.QuestionPaperDetail();
        });
    }
    var fillQuestionNumbers = function (data) {

        var url = Templates.QuestionPath;
        $('#testPaperQuestion').html("");
        $(data.TestQuestions).each(function (i, item) {
            item.index = i;
            item.IsEdit = true;
        });
        var response = Templates.QuestionPath;
        //AppCommon.HandleBarHelpers();
        var template = Handlebars.compile(response);
        var html = template(data);
        $('#testPaperQuestion').append(html);
        TestPaper.ShowHideQuestionButton();


    }
    var fillQuestionPaper = function (htmlPath) {
        if (htmlPath) {
            htmlPath = htmlPath.replace("~", GlobalConstants.FullPath) + "?no-cache=" + new Date().getTime();
            $.ajax({
                type: 'GET',
                crossDomain: true,
                url: htmlPath,
                success: function (response) {

                    //$('#testPaperContent').tinymce().execCommand('mceSetContent', false, response)
                    TestPaper.MatchQuestionWithTemplate();
                    $("#dvLoading").mLoading("destroy");
                },
                error: function (xhr) {

                },
                complete: function () {

                }
            })
        } else {
            $("#dvLoading").mLoading("destroy");
            //$('#testPaperContent').tinymce().execCommand('mceSetContent', false, "")
        }


    }






    var showHideQuestionButton = function () {
        //var ModuleKey = $("#tab-questionmodule li a.active").data("val");
        //var Modules = [Resources.QuestionModuleListening, Resources.QuestionModuleReading]
        //if (Modules.indexOf(ModuleKey) > -1)
        //{
        //    $("#addNewQuestionGroup,#tab-questionsection").show();
        //    //$("#addNewQuestion").hide();
        //}
        //else
        //{

        //    //$("#addNewQuestionGroup,#tab-questionsection").hide();
        //    $("#tab-questionsection li").eq(0).find("a").trigger("click");
        //    //$("#addNewQuestion").show();
        //    if (ModuleKey == Resources.QuestionModuleSpeaking)
        //        $("#addNewQuestion").attr("data-questiontype", Resources.QuestionTypeSpeaking)
        //    else
        //        $("#addNewQuestion").attr("data-questiontype", Resources.QuestionTypeWriting)

        //}
        //if (Resources.QuestionModuleListening == ModuleKey) {
        //    $("#dvSupportedFilePath").show();
        //} else {
        //    $("#dvSupportedFilePath").hide();
        //}
        var QuestionLength = $("#testPaperQuestion").find("a").length;
        if (QuestionLength == 1) {
            $("#btnNewQuestion").hide();
        } else {
            $("#btnNewQuestion").show();
        }

    }
    var showHideDeleteButton = function () {
        var sectionIndex = $("#tab-questionsection li a.active").closest("li").index();
        var val = $("#tab-questionsection li a.active").data("val");
        val = parseInt(val) ? parseInt(val) : 0;
        sectionlength = $("#tab-questionsection li a").length - 2;
        if (sectionlength < 0) {
            sectionlength = 0;
        }
        $("i.fa-trash", $("#tab-questionsection li").eq(sectionlength)).show();
        if (sectionIndex == sectionlength || !val) {
            TestPaper.ShowHideQuestionButton();
        } else {
            $("i.fa-trash", $("#testPaperQuestion")).remove();

            $("#addNewQuestionGroup").hide();
        }
        $("i.fa-trash", $("#tab-questionsection li").eq(sectionlength).prevAll()).remove();

    }
    var matchQuestionWithTemplate = function () {
        var ed = tinymce.activeEditor;
        var fieldsets = $("fieldset[id*=qg-]", ed.dom.doc);
        var fieldsetIds = $("fieldset[id*=qg-]", $("#testPaperQuestion")).toArray().map(function (item) {

            return $(item).prop("id");
        });
        $(fieldsets).each(function () {
            if (fieldsetIds.indexOf($(this).prop("id")) == -1) {
                $(this).remove();
            }
        });
        var btns = $("[data-key]", ed.dom.doc);
        var btnids = $("a.btn[data-question]", $("#testPaperQuestion")).toArray().map(function (item) {

            return item.dataset.question;
        });
        $(btns).each(function () {
            if (btnids.indexOf(this.dataset.question) == -1) {
                $(this).remove();
            }
        });

    }
    var getFileTypeById = function () {
        var obj = {};
        obj.id = $("#tab-questionmodule li a.active").data("val");
        $.ajax({
            url: $("#hdnGetFileTypeById").val(),
            type: "GET",
            dataType: "JSON",
            async: true,
            data: obj,
            success: function (response) {
                if (response) {
                    $("#SupportedFilePath").attr("accept", response);
                } else {
                    $("#SupportedFilePath").removeAttr("accept");
                }
            }
        });
    }

    var loadTestInstructions = function () {
        var htmlPath = $("#TestInstructionFileName").val();
        if (htmlPath) {
            htmlPath = htmlPath.replace("~", GlobalConstants.FullPath);
            $.ajax({
                type: 'GET',
                crossDomain: true,
                url: htmlPath,
                success: function (response) {

                  

                    tinymce.init({


                        selector: '#testInstructionContent',
                        theme: "silver",
                        height: "430",
                        width: "100%",
                        verify_html: false,
                        plugins: 'print preview paste importcss searchreplace autolink autosave save directionality code visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists wordcount imagetools textpattern noneditable help charmap  emoticons preventdelete',
                        imagetools_cors_hosts: ['picsum.photos'],
                        menubar: 'edit view insert format tools table help',
                        toolbar: 'undo redo | bold italic underline strikethrough well| fontselect fontsizeselect formatselect | alignleft aligncenter alignright alignjustify | outdent indent |  numlist bullist | forecolor backcolor removeformat | pagebreak | charmap emoticons | fullscreen  preview save print | insertfile image media template link anchor codesample | ltr rtl',
                        toolbar_sticky: false,


                        setup: function (ed) {
                            ed.on('keyup', function (e) {
                            })
                            ed.on('dblclick', function (e) {
                                var target = e.target;
                                var key = $(target).data("key");
                                if (!key) {
                                    target = $(target).closest("[data-key]")
                                }
                                if (target) {
                                    TestPaper.OptionalClick(target)
                                }
                            })
                            ed.ui.registry.addButton('well', {
                                tooltip: 'Wrap',
                                icon: 'edit-block',
                                onAction: function () {
                                    var text = ed.selection.getContent({
                                        'format': 'html'
                                    });
                                    if (text && text.length > 0) {
                                        ed.execCommand('mceInsertContent', false,
                                            '<div class="well">' + text + '</div>');
                                    }
                                }
                            });
                        }

                    });



                },
                error: function (xhr) {

                },
                complete: function () {

                }
            })
        } else
        {
            $('#testInstructionContent').tinymce().execCommand('mceSetContent', false, "")
        }
    }

    var answerKeyDetail = function (obj) {
        if (!obj) {
            obj = {};
            obj.id = $("#RowKey").val();
            obj.ModuleKey = $("#tab-questionmodule li a.active").data("val");
        }
        $.ajax({
            url: $("#hdnGetAnswerKeyById").val(),
            type: "GET",
            dataType: "JSON",
            async: true,
            data: obj,
            success: function (response) {
                response.TestSections = response.QuestionSections;
                ExamTest.FillExamQuestions(response);
                TestPaper.FillAnswerKeys(response);
            }
        });
    }

    var fillAnswerKeys = function (data) {
        $('#dvExamPaper').html("")
        var calls = [];
        $(data.QuestionSections).each(function (i, item) {
            var url = item.TestSectionFileName;
            url = url.replace("~", GlobalConstants.FullPath) + "?no-cache=" + new Date().getTime()
            calls.push($.ajax({
                type: 'GET',
                crossDomain: true,
                url: url,
                success: function (response) {
                    var section = $("<atricle data-section='" + item.TestSectionName + "' class='mb-4'/>")
                    $(section).append("<h2 class='m-0'>" + item.TestSectionName + "</h2><hr class='m-0'/>")
                    $(section).append("<p >" + response + "</p>")
                    $('#dvExamPaper').append(section);

                },
                error: function (xhr) {

                },
                complete: function () {

                }
            }));
        })
        if (calls.length > 0) {
            $.when.apply($, calls).then(function () {

                var cntrls = ["input", "textarea"]
                $("[data-key]", $('#dvExamPaper')).each(function () {

                    var question = this.dataset.question;
                    var value = data.QuestionModules.filter(function (item) {
                        return item.RowKey == question
                    })[0].Text;
                    if (cntrls.indexOf(this.tagName.toLowerCase()) > -1) {
                        $(this).val(value);
                    } else {
                        $(this).find("input[type=checkbox][value=" + value + "]").prop("checked", true)
                    }

                });
                $("input[data-key]").tagsinput({ delimiter: '|', confirmKeys: [13] });
                $("[data-optionhint]").remove();
                $('#dvExamPaper').find("atricle").eq(0).show();

            });
        } else {
            $("input[data-key]").tagsinput({ delimiter: '|', confirmKeys: [13] });
        }

    }

    var formAnswerKeySubmit = function (btn) {
        var form = $(btn).closest("form")[0];
        var validator = $(form).validate();

        var url = $(form)[0].action;

        var validate = $(form).valid();
        if (validate) {
            $("[disabled]", form).removeAttr("disabled");
            $(".section-content").mLoading();

            var data = $(form).serializeToJSON({
                associativeArrays: true
            });

            data["QuestionModules"] = TestPaper.GetAnswersByQuestion();
            delete data[""];

            AjaxHelper.ajaxWithFileAsync("model", "POST", url, { model: data }, function () {
                response = this;
                if (response.IsSuccessful) {
                    toastr.success(Resources.Success, response.Message);
                    TestPaper.AnswerKeyDetail();
                }
                else {
                    $("[data-valmsg-for=error_msg_payment]").html(response.Message);
                }
                $(".section-content").mLoading("destroy");
            })

        } else {
            validator.focusInvalid();
        }
    }
    var getAnswersByQuestion = function () {
        var AnswerData = [];
        $("[data-key]", $('#dvExamPaper')).each(function () {

            var question = this.dataset.question;
            var value = $("input[type=checkbox]:checked", this).val();
            if (!value)
                value = $(this).val();

            if (value)
                $("button[data-question=" + question + "]", $('#dvQuestionNumber')).addClass("active")
            else
                $("button[data-question=" + question + "]", $('#dvQuestionNumber')).removeClass("active")

            var obj = {};
            obj.RowKey = question;
            obj.Text = value;
            AnswerData.push(obj);
        });
        return AnswerData;
    }
    var chooseFileEvent = function (e) {
        var ed = tinymce.activeEditor;
        $(".section-content").mLoading();
        var _this = e.target;
        var files = e.target.files;
        var i, f;
        var result;
        if (files.length > 0) {
            for (i = 0, f = files[i]; i != files.length; ++i) {
                var reader = new FileReader();
                var name = f.name;
                var type = f.type;
                reader.onload = function (e) {
                    var data = e.target.result;
                    var wb, arr;
                    function doit() {
                        try {
                            //if (type == "application/pdf") {
                            //    var resultHtml = pdfToHtml(data)
                            //    $('#testPaperContent').tinymce().execCommand('mceSetContent', false, resultHtml);
                            //} else {
                            mammoth.convertToHtml({ arrayBuffer: data }).then(function (resultObject) {
                                var htmlArray = [];
                                var deleteArray = [];
                                var SavedQuestions = $("[data-question]", $('#testPaperQuestion'));
                                $(SavedQuestions).each(function (i, item) {
                                    var RowKey = $("[name*=RowKey]", item).val();
                                    RowKey = parseInt(RowKey) ? parseInt(RowKey) : 0;
                                    var QuestionNumber = $("[name*=QuestionNumber]", item).val();
                                    QuestionNumber = parseInt(QuestionNumber) ? parseInt(QuestionNumber) : 0;
                                    if (!RowKey) {
                                        deleteArray.push(item);
                                    } else {
                                        var cntrl = $("[data-question=" + QuestionNumber + "]", ed.dom.doc)[0];
                                        htmlArray.push(cntrl.outerHTML);
                                    }
                                });
                                deleteQuestion(deleteArray);

                                var resultHtml = htmlArray.join('') + resultObject.value
                                $('#testPaperContent').tinymce().execCommand('mceSetContent', false, resultHtml);
                                $(".section-content").mLoading("destroy");
                            })

                            //}
                            $(_this).val('').trigger("change");
                        } catch (e) { console.log(e); toastr.error('Bad File !.', Resources.Failed); $(".section-content").mLoading("destroy"); $(_this).val('').trigger("change");; }
                    }

                    if (e.target.result.length > 1e6) opts.errors.large(e.target.result.length, function (e) { $(_this).val('').trigger("change"); $(".section-content").mLoading("destroy"); });
                    else { doit(); }
                };
                //if (type == "application/pdf") {
                //    reader.readAsDataURL(f);
                //} else {
                reader.readAsArrayBuffer(f);
                //}


            }
        }
        else {
            $(".section-content").mLoading("destroy");
        }
    }
    var chooseSupportFileEvent = function (e) {
        var ed = tinymce.activeEditor;
        $(".section-content").mLoading();
        var _this = e.target;
        var files = e.target.files;
        var i, f;
        var result;
        if (files.length > 0) {
            var sound = document.getElementById('audSupportedFile');
            sound.src = URL.createObjectURL(e.target.files[0]);
            // not really needed in this exact case, but since it is really important in other cases,
            // don't forget to revoke the blobURI when you don't need it
            sound.onend = function (e) {
                URL.revokeObjectURL(e.target.src);
                $(sound).show();
            }
            $(".section-content").mLoading("destroy");
        }
        else {
            $(".section-content").mLoading("destroy");
        }
    }

    var getExamValuation = function (index, reset) {
        var obj = {};
        obj.searchText = $("#txtsearch").val().trim();
        obj.page = index ? index : 1;
        obj.rows = 20;
        reset = reset ? reset : true;
        $.ajax({
            url: $("#hdnGetExamValuation").val(),
            type: "GET",
            dataType: "JSON",
            async: true,
            data: obj,
            success: function (data) {
                $('#dvStudentView').html("")
                var url = $("#hdnExamValuationFilePath").val() + "?no-cache=" + new Date().getTime();
                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: url,
                    success: function (response) {
                        var TotalScore = 0;
                        var resultdata = { Applications: data.rows }
                        resultdata.Modules = data.rows[0].ExamTests.reduce(function (index, obj) {
                            if (!index[obj.TestKey]) {
                                index[obj.TestKey] = obj;
                            } else {
                                for (prop in obj) {
                                    index[obj.TestKey][prop] = obj[prop];
                                }
                            }
                            return index;
                        }, []).filter(function (res, obj) {
                            return obj;
                        });
                        $(resultdata.Applications).each(function () {
                            var application = this;
                            $(application.ExamTests).each(function (i, item) {
                                item.ModuleItems = [];
                                $(resultdata.Modules).each(function (j, module) {
                                    var result = item.ExamModules.filter(function (exam) {
                                        return exam.ModuleKey == module.RowKey;
                                    })[0];
                                    if (!result) {
                                        result = {};
                                    } else {
                                        if (result.TotalScore != null && TotalScore != null)
                                            TotalScore += result.TotalScore;
                                        else
                                            TotalScore = null;
                                    }
                                    result.TotalScore = result.TotalScore ? result.TotalScore.toString() : null;
                                    item.ModuleItems.push(result)
                                })
                                if (TotalScore)
                                    item.BandScore = TotalScore / item.ExamModules.length;
                                item.BandColor = item.BandScore && item.BandScore >= Resources.IELTSPassMark ? "w3-text-green" : "w3-text-red";
                            });
                            //application.colspan = (application.ExamTests[0].ModuleItems.length + 2);
                        });
                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        var html = template(resultdata);
                        var sv = $('#dvStudentView');
                        $('#dvStudentView').html(html);
                        //if (reset)
                        //    ExamValuationPagination(data);


                    },
                    error: function (xhr) {

                    },
                    complete: function () {

                    }
                })
            }
        });



    }

    var processValuation = function (id, module, application) {
        var obj = {};
        obj.TestPaperKey = id;
        obj.ModuleKey = module;
        obj.ApplicationKey = application;
        //if (Resources.AnswerKeyModules.indexOf(parseInt(module)) > -1) {
        //    AjaxHelper.ajaxAsync("POST", $("#hdnAddEditTestValuation").val(), obj, function () {
        //        response = this;
        //        if (response.IsSuccessful) {
        //            TestPaper.GetExamValuation();
        //            toastr.success(response.Message, Resources.Success);
        //        }
        //    })
        //}
        //else {
        AjaxHelper.ajaxAsync("POST", $("#hdnAddEditTestValuation").val(), obj, function () {
            response = this;
            if (response.IsSuccessful) {
                var url = $("#hdnAddEditTestValuation").val() + "?" + $.param(obj);
                $.customPopupform.CustomPopup({
                    modalsize: "modal-lg",
                    load: function (dailog) {
                        setTimeout(function () {
                            ExamTest.FillQuestionPaper(valuationPopupData, $("#ModuleKey").val());
                            AppCommon.FormatInputCase();


                        }, 500)

                    },
                    submit: function () {
                        var form = $("#frmTestValuation");
                        var data = $(form).serializeToJSON({
                            associativeArrays: true
                        });
                        data.ExamAnswers = $("[data-key]", $('#dvExamPaper')).toArray().map(function (item) {
                            var resultItem = {};
                            var result = $(item).closest("span.result");
                            resultItem.QuestionNumber = item.dataset.question;
                            resultItem.TotalScore = $("[id*=ExamScore]", result).val();
                            return resultItem;
                        });
                        AjaxHelper.ajaxAsync("POST", $("#hdnAddEditTestValuation").val(), data, function () {
                            response = this;
                            if (response.IsSuccessful) {
                                toastr.success(response.Message, Resources.Success);
                                TestPaper.GetExamValuation()
                                $(form).closest(".modal").modal('hide')
                            } else {
                                toastr.error(response.Message, Resources.Failed);
                            }
                        })

                    }


                }, url);
            }
        })

        //}
    }

    var loadTemplates = function () {
        $(".section-content").mLoading()
        var calls = [];
        $(QuestionTypesData).each(function (i, item) {
            var url = $("#hdnQuestionTypePath").val() + "/" + item.RowKey + ".html" + "?no-cache=" + new Date().getTime();;
            calls.push($.ajax({
                type: 'GET',
                crossDomain: true,
                url: url,
                success: function (response) {
                    Templates["QuestionTypePath" + item.RowKey] = response;
                }
            }));
        });
        url = $("#hdnQuestionPath").val() + "?no-cache=" + new Date().getTime();
        calls.push($.ajax({
            type: 'GET',
            crossDomain: true,
            url: url,
            success: function (response) {
                Templates.QuestionPath = response;
            }
        }));
        if (calls.length > 0) {
            $.when.apply($, calls).then(function () {

                TestPaper.QuestionPaperDetail();
                TestPaper.ShowHideQuestionButton();
                $(".section-content").mLoading("destroy")
            });
        } else {
            $(".section-content").mLoading("destroy")
        }
    }




    var getQuestionPaper = function (json) {

        $('#Repeater1').repeater({
            show: function () {
                $(".page-content").mLoading();
              
                $(this).slideDown();

                var item = $(this);
                $("[data-option-item]:not(:first-child)", item).remove();


                var QuestionNumber = $("[data-repeater-item]").length;
                $(".QuestionMaster:last-child() .QuestionNumber").html("QUESTION " + QuestionNumber);
                $(".QuestionMaster:last-child()").attr("id", "ItemRepeater[" + QuestionNumber + "]");
                var RepeaterId = parseInt(QuestionNumber) - 1
                $(".QuestionMaster:last-child() .repeater").attr("id", "Repeater2_" + RepeaterId)
                var ClassName = "tiny_" + RepeaterId;
                $(".QuestionMaster:last-child() textarea").addClass(ClassName);
                

               
               
              
                               TestPaper.GetQuestionOption();            
                
                   
                 
                    tinymce.init({

                        // Location of TinyMCE script
                        //script_url: $("#hdnTinyMCEJS").val(),
                        selector: "." + ClassName,
                        theme: "silver",
                        height: "300",
                        width: "100%",
                        verify_html: false,
                        plugins: 'print preview paste importcss searchreplace autolink autosave save directionality code visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists wordcount imagetools textpattern noneditable help charmap quickbars emoticons preventdelete',
                        imagetools_cors_hosts: ['picsum.photos'],
                        menubar: 'edit view insert format tools table help',
                        toolbar: 'undo redo | bold italic underline strikethrough well| fontselect fontsizeselect formatselect | alignleft aligncenter alignright alignjustify | outdent indent |  numlist bullist | forecolor backcolor removeformat | pagebreak | charmap emoticons | fullscreen  preview save print | insertfile image media template link anchor codesample | ltr rtl',
                        toolbar_sticky: true,
                        setup: function (ed)
                        {
                            ed.on('init', function (args)
                            {
                                $(".page-content").mLoading("destroy");
                            });

                        }

                    });



            
                
            },
            hide: function (remove) {

                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteQuestion($(hidden).val(), this, remove);

                }
                else {
                    $(this).slideUp(remove);

             
                        ResetQuestionNumber();

             

                }

            

            },
            rebind: function (response) {

            
                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {
                    toastr.success(Resources.Success, response.Message);
                    location.reload();
                }
            },
            data: jsonData,
            repeatlist: 'TestQuestions',
            defaultValues: jsonData,
            hasFile: false,
            Async: true
        });
    }


    var setQuestionNumber = function (json)
    {
        $.each($(".QuestionNumber"), function (i, obj)
        {
            var QuuestionNumber = parseInt(i) + 1;
            $(this).html("QUESTION " + QuuestionNumber);

        });

    }


    var getQuestionOption = function (json)
    {
        $.each($("div[id*=Repeater2]"), function (i,obj)
        {
            $(obj).repeater({
                show: function () {
                    $(this).slideDown();
                    var item = $(this);
                },
                hide: function (remove) {

                    var self = $(this).closest('[data-option-item]').get(0);
                    var hidden = $(self).find('input[type=hidden]')[0];

                  


                    if ($(hidden).val() != "" && $(hidden).val() != "0")
                    {
                        deleteQuestionOption($(hidden).val(), this, remove);
                    }
                    else
                    {
                        $(this).slideUp(remove);

                    }
                    TestPaper.GetQuestionPaper();
                },
                rebind: function (response) {


                    location.reload();
                },
                listAttr: '[ data-option-list]',
                itemAttr: '[data-option-item]',
                createAttr: '[data-option-create]',
                deleteAttr:'[data-option-delete]',
                data: jsonData,
                repeatlist: 'QuestionOptions',
                defaultValues: jsonData,
                hasFile: false,
                Async: true
            });
        });

    }


    var checkQuestionOptionValues = function (_this) {

        var repeaterItem = "#" + $(_this).closest(".repeater")[0].id;

        var Array = $(repeaterItem + ' input[id*=OptionValue]input[id!="' + _this.id + '"]').map(function ()
        {
            return $(this).val();
        }).get(); // <----

        //alert(Array.indexOf(_this.value));
        if (Array.indexOf(_this.value) !== -1) {
            _this.value = "";
            alert("Option already exists");
        }
    }


    function FillDropdown(List, id,selectorId)
    {
        if (selectorId != id)
        {
          var SelectedItemValue=  $(id).val();
            $(id).html("");
            $(id).val('default').selectpicker("refresh");
            $.each(List, function (i, Data) {
                $(id).append(
                    $('<option></option>').val(Data.RowKey).html(Data.Text));
            });

            $(id).val(SelectedItemValue);
            $(id).selectpicker("refresh");
        }
    }


    var getDropDown = function (_this)
    {

            var data;
             var response = {};
        const formElem = document.querySelector('form');
        var formdata = new FormData(formElem);
         var url = $('#hdnGetFilters').val();
             $.ajax({
                 type: "POST",
                 url: url,
                 data: formdata,
                 dataType: 'json',
                 async: true,
                 processData: false,
                 contentType: false,
                 cache: false,
                 success: function (result)
                 {

                     //switch (!_this.id)
                     //{
                     //    case AcademicTermsKeys:
                     //        FillDropdown(result.CourseTypes, "#CourseTypeKeys");
                     //        FillDropdown(result.CourseYears, "#CourseYearsKeys");
                     //        break;
                     //    case CourseTypeKeys:
                     //        FillDropdown(result.Courses, "#CourseKeys");
                     //        FillDropdown(result.CourseYears, "#CourseYearsKeys");
                     //        break;
                     //    case Courses:
                     //        FillDropdown(result.UniversityMasters, "#UniversityMasterKeys");
                     //        FillDropdown(result.CourseYears, "#CourseYearsKeys");
                     //        break;
                     //    case UniversityMasterKeys:
                     //        FillDropdown(result.CourseYears, "#CourseYearsKeys");
                     //        break;                 
                     ////}
                

                     FillDropdown(result.AcademicTerms, "#AcademicTermsKeys","#"+_this.id);
                     FillDropdown(result.CourseTypes, "#CourseTypeKeys", "#" + _this.id);
                     FillDropdown(result.Courses, "#CourseKeys", "#" + _this.id);
                     FillDropdown(result.UniversityMasters, "#UniversityMasterKeys", "#" + _this.id);
                     //FillDropdown(result.Mediums, "#MeadiumKeys", "#" + _this.id);
                     //FillDropdown(result.SecondLanguages, "#SecondLanguageKeys", "#" + _this.id);
                     //FillDropdown(result.Batches, "#BatchKeys", "#" + _this.id);
                     //FillDropdown(result.Modes, "#ModeKeys", "#" + _this.id);
                     FillDropdown(result.CourseYears, "#CourseYearsKeys", "#" + _this.id);
                     FillDropdown(result.ClassModes, "#ClassModeKeys", "#" + _this.id);
                     FillDropdown(result.ClassCodes, "#ClassCodeKeys", "#" + _this.id);
                     //FillDropdown(result.Branches, "#BranchKeys", "#" + _this.id);
                 },
                 xhr: function ()
                 {
                     // get the native XmlHttpRequest object
                     var xhr = $.ajaxSettings.xhr();
                     // set the onprogress event handler
                     xhr.upload.onprogress = function (evt)
                     {
                       
                     };

                     xhr.onreadystatechange = function ()
                     {
                         if (xhr.readyState === 4)
                         {
                          
                             $(".section-content").mLoading("destroy");



                             
                         }
                     }


                     return xhr;
                 },
                 error: function (request, status, error)
                 {
                     console.log(request.responseText);
                     
                 }
             });


    }


    var formSubmit = function (btn) {





        var form = $(btn).closest("form")[0];
        var validator = $(form).validate();
        var validate = true;
        var url = $(form)[0].action;
        //var btns = $("[data-question]");
        //var fieldsets = $("fieldset", $("#testPaperQuestion")).toArray().filter(function (item)
        //{
        //    var btnLength = $("[data-question]", item).length;
        //    return !btnLength;
        //});

        //var validate = true;
        //if (btns.length == 0) {
        //    validate = false;
        //    EduSuite.AlertMessage({
        //        type: 'orange',
        //        title: Resources.Warning,
        //        content: Resources.AtleastQuestionErrorMessage,
        //        icon: 'fa fa-exclamation-triangle',
        //        btnClass: 'btn-warning'
        //    })
        //}
        //else if (fieldsets.length > 0)
        //{
        //    validate = false;
        //    EduSuite.AlertMessage({
        //        type: 'orange',
        //        title: Resources.Warning,
        //        content: Resources.EmptyQuestionGroupErrorMessage,
        //        icon: 'fa fa-exclamation-triangle',
        //        btnClass: 'btn-warning'
        //    })
        //}
        var validate = validate && $(form).valid();
        if (validate) {
            $("[disabled]", form).removeAttr("disabled");
            $(".section-content").mLoading();

            var data = $(form).serializeToJSON({
                associativeArrays: true
            });
            data.SupportedFilePath = $("#SupportedFilePath")[0].files.length > 0 ? $("#SupportedFilePath")[0].files[0] : null;



            $.each($("textarea[id*=QuestionPaper]"), function (i)
            {
                data.TestQuestions[i].QuestionPaper = $(this).tinymce().getContent();
                data.TestQuestions[i].SectionNumber = ($("#tab-questionsection li a.active").closest("li").index() + 1);
                data.TestQuestions[i].TestSectionName = "Question " + data.SectionNumber;
                data.TestQuestions[i].QuestionNumber = parseInt(i+1);
            });

            data.ExamTime = data.ExamTime ? moment(data.ExamTime.toUpperCase(), ["hh:mm A"]).format("HH:mm") : null;



            AjaxHelper.ajaxWithFileAsync("model", "POST", url, { model: data }, function () {
                response = this;
                if (response.IsSuccessful)
                {
                    toastr.success(Resources.Success, response.Message);
                    var obj = {};
                    obj.id = response.RowKey;
                    obj.ModuleKey = response.ModuleKey;
                    obj.SectionKey = response.TestSectionKey;
                    TestPaper.QuestionPaperDetail(obj);
                    $("#RowKey").val(response.RowKey);


                    window.location.href = $("#hdnAddEditTestPaper").val() + "/" + $("#RowKey").val();

                }
                else {
                    $("[data-valmsg-for=error_msg_payment]").html(response.Message);
                }
                $(".section-content").mLoading("destroy");
            })

            //var data;
            // var response = {};
            // const formElem = document.querySelector('form');
            // var formdata = new FormData(formElem);
            // var url = $('form').attr('action');
            // $.ajax({
            //     type: "POST",
            //     url: url,
            //     data: formdata,
            //     dataType: 'json',
            //     async: true,
            //     processData: false,
            //     contentType: false,
            //     cache: false,
            //     success: function (result) {
            //         if (typeof result == "string") {
            //             response.Message = result;
            //             $(".section-content").mLoading("destroy");
            //         }
            //         else
            //         {
            //             response = result;
            //             $("#RowKey").val(response.RowKey);
            //             $(".section-content").mLoading("destroy");                  
            //         }


            //     },
            //     xhr: function () {
            //         // get the native XmlHttpRequest object
            //         var xhr = $.ajaxSettings.xhr();
            //         // set the onprogress event handler
            //         xhr.upload.onprogress = function (evt) {



            //             $(".section-content").mLoading();

            //         };

            //         xhr.onreadystatechange = function ()
            //         {
            //             if (xhr.readyState === 4)
            //             {
            //                 toastr.success(Resources.Success, response.Message);
            //                 $(".section-content").mLoading("destroy");




            //                 return response;
            //             }
            //         }


            //         return xhr;
            //     },
            //     error: function (request, status, error)
            //     {
            //         console.log(request.responseText);
            //         if (request.status == 200) {
            //             response.Message = "Your session has ended and you have been logged out and should log back in.";
            //         }
            //         else
            //             response.Message = "Internal server error";
            //     }
            // });


        }
        else {
            validator.focusInvalid();
        }
    }

    var preventMultiChecking = function() {
        $(".answerKey").on("change", function() {
            var key = $(this).closest(".repeater")[0];
            var isChecked = $(this).prop("checked");
            $("#" + key.id + " .answerKey").prop("checked", false);
            
            $(this).prop("checked", isChecked );
        });
    }

    return {
        GetTestPaper: getTestPaper,
        UpdateQuestion: updateQuestion,
        GetQuestionStart: getQuestionStart,
        GetQuestionEnd: getQuestionEnd,
        SetQuestionNumber: setQuestionNumber,
        OptionClick: optionClick,
        QuestionPaperDetail: questionPaperDetail,
        FillTestSection: fillTestSection,
        FillQuestionNumbers: fillQuestionNumbers,
        FillQuestionPaper: fillQuestionPaper,
        FormSubmit: formSubmit,
        ShowHideQuestionButton: showHideQuestionButton,
        ShowHideDeleteButton: showHideDeleteButton,
        MatchQuestionWithTemplate: matchQuestionWithTemplate,
        GetFileTypeById: getFileTypeById,
        LoadTestInstructions: loadTestInstructions,
        AnswerKeyDetail: answerKeyDetail,
        FillAnswerKeys: fillAnswerKeys,
        FormAnswerKeySubmit: formAnswerKeySubmit,
        GetAnswersByQuestion: getAnswersByQuestion,
        ChooseFileEvent: chooseFileEvent,
        ChooseSupportFileEvent: chooseSupportFileEvent,
        GetExamValuation: getExamValuation,
        ProcessValuation: processValuation,
        LoadTemplates: loadTemplates,
        GetQuestionPaper: getQuestionPaper,
        GetQuestionOption: getQuestionOption,
        PreventMultiChecking: preventMultiChecking,
        SetQuestionNumber: setQuestionNumber,
        CheckQuestionOptionValues: checkQuestionOptionValues,
        GetDropDown: getDropDown
    }
}());

function changeQuestionNumber(item, qtn, qtype) {
    $(item).attr("data-question", qtn);
    $(item).attr("data-questiontype", qtype);
    var questions = $("[name*=QuestionNumber]", $(item));
    if (questions.length > 0) {
        $(questions).prop("name", "[" + qtn + "].QuestionNumber");
        $("span#QuestionNumber", $(item).closest("div[data-groupkey]")).html(qtn + ".");
        var inputs = $(item).find("input");
        $(inputs).each(function (i) {
            var id = qtn + "_" + i + "_QuestionNumber";
            $(this).prop("id", id);
            $(this).next("span").prop("for", id);
        });

    }
    else {
        $(item).prop("name", "[" + qtn + "].QuestionNumber");
        $(item).prop("placeholder", qtn);

    }

}

function deleteTestPaper(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        //content: Resources.Delete_Confirm_Country,
        actionUrl: $("#hdnDeleteTestPaper").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function deleteTestSection(_this) {
    var rowkey = $(_this).prev("a")[0].dataset.val;
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        //content: Resources.Delete_Confirm_Country,
        actionUrl: $("#hdnDeleteTestSection").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $(_this).closest("li").remove();
            var sectionlength = $("#tab-questionsection li a").length - 2;
            if (sectionlength < 0) {
                sectionlength = 0;
            }

            $("#tab-questionsection li a").removeClass("active");
            $("#tab-questionsection li").eq(sectionlength).find("a").addClass("active");
            TestPaper.ShowHideQuestionButton();
            var obj = {};
            obj.id = $("#RowKey").val();
            obj.ModuleKey = $("#tab-questionmodule li a.active").data("val");
            obj.SectionKey = null;
            TestPaper.QuestionPaperDetail(obj);

        }
    });
}
function deleteQuestionGroup(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        //content: Resources.Delete_Confirm_Country,
        actionUrl: $("#hdnDeleteQuestionGroup").val(),
        actionValue: rowkey,
        dataRefresh: function () { $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); }
    });
}
function deleteQuestion(rowkey,row, remove)
{
  
        if (rowkey)
        {
            var result = EduSuite.Confirm({
                title: Resources.Confirmation,
                //content: Resources.Delete_Confirm_Country,
                actionUrl: $("#hdnDeleteQuestion").val(),
                actionValue: rowkey,
                dataRefresh: function () {
                    $(row).slideUp(remove);   
                    window.location.href = $("#frmQuestionPaper").attr("action") + "/" + $("#RowKey").val();
                 
                }
            });
        } else {
            removeButton(btn)
        }   
}

function deleteQuestionOption(rowkey, row, remove) {

    if (rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            //content: Resources.Delete_Confirm_Country,
            actionUrl: $("#hdnDeleteQuestionOption").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                $(row).slideUp(remove);

            }
        });
    } else {
        removeButton(btn)
    }
}




function removeButton(_this) {

    var ed = tinymce.activeEditor;
    if ($.isArray(_this)) {
        $(_this).each(function () {
            var btn = $(this).closest("[data-question]");
            var qtn = $("[name*=QuestionNumber]", btn).val();
            var cntrl = $("[data-question=" + qtn + "]", ed.dom.doc);
            $(cntrl).remove();
            $(btn).remove();
        })

    } else {
        var btn = $(_this).closest("[data-question]");
        var qtn = $("[name*=QuestionNumber]", btn).val();

        var cntrl = $("[data-question=" + qtn + "]", ed.dom.doc);
        $(cntrl).remove();
        $(btn).remove();
    }

    var btns = $("#testPaperQuestion").find("a.btn").filter(function (n, item) {
        var dataset = $(item)[0].dataset;
        var text = dataset.question;
        text = parseInt(text) ? parseInt(text) : 0;
        return text && text > qtn;
    });
    $(btns).each(function () {
        var data = $.extend({}, true, jsonDetailData);
        data.RowKey = $("[name*=RowKey]", this).val();
        data.RowKey = parseInt(data.RowKey) ? parseInt(data.RowKey) : 0;
        data.QuestionTypeKey = $("[name*=QuestionTypeKey]", this).val();
        data.QuestionTypeKey = parseInt(data.QuestionTypeKey) ? parseInt(data.QuestionTypeKey) : 0;
        data.QuestionNumber = $("[name*=QuestionNumber]", this).val();
        data.QuestionNumber = parseInt(data.QuestionNumber) ? parseInt(data.QuestionNumber) : 0;
        data.IsUpdate = true;
        data.index = $(this).index();
        data.IsEdit = true;
        data.QuestionOptions = $("[data-question=" + data.QuestionNumber + "]", ed.dom.doc).find("input").toArray().map(function (item) {
            var label = $(item).next();
            var value = $(item).val();
            var text = $(label).data("val");
            return { Text: text, Value: value };
        });
        data.NewQuestionNumber = data.QuestionNumber - 1;
        TestPaper.UpdateQuestion(data)
    })

    TestPaper.ShowHideQuestionButton();
}

function ExamValuationPagination(data) {
    $('#page-selection').empty();
    var totalRecords = $("#hdnTotalRecords").val();
    totalRecords = totalRecords != "" ? parseInt(totalRecords) : 0;
    var Size = jsonData["PageSize"];
    var totalPages = data.total;
    var page = data.page;
    $('#page-selection').bootpag({
        total: totalPages,
        page: page,
        maxVisible: 10
    }).on("page", function (event, num) {
        $("#PageIndex").val(num);
        $('#page-selection-up,#page-selection-down').bootpag({ page: num })
        TestPaper.GetEnquirySchedules(num, false);


    });


}

function pdfToHtml(file) {
    var currPage = 1; //Pages are 1-based not 0-based
    var numPages = 0;
    var thePDF = null;

    //This is where you start
    pdfjsLib.getDocument(file).then(function (pdf) {

        //Set PDFJS global object (so we can easily access in our page functions
        thePDF = pdf;

        //How many pages it has
        numPages = pdf.numPages;

        //Start with first page
        pdf.getPage(1).then(handlePages);
    });



    function handlePages(page) {
        //This gives us the page's dimensions at full scale
        var viewport = page.getViewport(1);

        //We'll create a canvas for each page to draw it on
        var canvas = document.createElement("canvas");
        canvas.style.display = "block";
        var context = canvas.getContext('2d');
        canvas.height = viewport.height;
        canvas.width = viewport.width;

        //Draw it on the canvas
        page.render({ canvasContext: context, viewport: viewport });

        //Add it to the web page
        document.body.appendChild(canvas);

        //Move to next page
        currPage++;
        if (thePDF !== null && currPage <= numPages) {
            thePDF.getPage(currPage).then(handlePages);
        }
    }
}


async function ResetQuestionNumber()
{
    var QuestionIndex = 1;

    setTimeout(function () {
    

  
    $.each($(".QuestionMaster .QuestionNumber"), function (i) {
        $(this).html("QUESTION " + QuestionIndex);
        QuestionIndex = QuestionIndex + 1;

        });

    }, 500);

}