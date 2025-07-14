var JsonData = [], request = null;
var examTimer;
var ExamTest = (function () {
    var questionPaperDetail = function (obj) {

        $("#ExamReview").show();
        $("#btnExamSubmit,#btnSave,#btnSaveAndMark,#MarkForReviewAndNext,#examPersonDetails,#dvExamPaper").show();



        ExamTest.GetExamTime(responseJson);


        //$("#dvTestContaner").mLoading();
        //$("#btnRead,#btnSave").hide();
        //$("#tab-questionmodule a").removeClass("disabled");
        //if (!obj) {
        //    obj = {};
        //    obj.id = $("#TestPaperKey").val();
        //    obj.ModuleKey = 1// $("#tab-questionmodule li a.active").data("val");
        //    obj.ApplicationKey = $("#ApplicationKey").val();
        //}

        //$.ajax({
        //    url: $("#hdnQuestionPaperDetail").val(),
        //    type: "GET",
        //    dataType: "JSON",
        //    async: true,
        //    data: obj,
        //    success: function (response) {
        //        $("#RowKey").val(response.RowKey);

        //        if (response.RowKey > 0)
        //        {
        //            $("#ExamResultDetails,#examPersonDetails").show();
        //            if (response.AnswerKeyStatus == true) {
        //                $("#divMessage").show();
        //            }
        //        }
        //        else {
        //            $("#ExamReview").show();



        //            $("#btnExamSubmit,#btnSave,#btnSaveAndMark,#MarkForReviewAndNext,#examPersonDetails").show();
        //        }

        //        $("#ExamTestSectionKey").val(response.ExamTestSectionKey);

        //        $("#SpnTotalQuestions").html(response.TotalQuestions);
        //        $("#SpnTotalAttempted").html(response.TotalAttept);
        //        $("#SpnCorrectAnsweres").html(response.TotalCorrectAnswers);
        //        $("#SpnIncorrectAnswers").html(response.TotalInCorrectAnswers);
        //        $("#spnScore").html(response.TotalScore);
        //        $("#spnNegativeMarks").html(response.TotalNegativeMarks);
        //        $("#SpnApplicantName").html(":" + response.ApplicantName);
        //        $("#SpnTestPaperName").html(":" + response.TestPaperName);

        //        if (response.ExamTestSectionKey && response.TotalScore != null) {
        //            $("#dvMark span").eq(0).html(response.TotalScore);
        //            $("#dvMark span").eq(1).html(response.MaximumScore);
        //            var cssClass = 5 > response.TotalScore ? "failed" : "passed";
        //            $("#dvMark").removeClass("passed").removeClass("failed").addClass(cssClass).show();

        //        }
        //        else {
        //            $("#dvMark").hide();
        //        }



        //        $("#ExamStart").val(AppCommon.JsonDateToNormalDate(response.ExamStart));

        //        $("#ExamDuration").val(response.ExamDuration)

        //        //ExamTest.FillQuestionPaper(response, obj.ModuleKey);
        //        //ExamTest.FillExamQuestions(response);
        //        //ExamTest.EmbedSupportFile(obj.ModuleKey, response.SupportedFileName);


        //        ExamTest.GetExamTime(response);

        //    }
        //});
    }
    var loadExamInstruction = function () {
        $("#btnStart").show();
        var htmlPath = $("#InstructionFileName").val();
        var extension = htmlPath ? htmlPath.replace(/^.*\./, '') : null;
        if (extension == "html") {
            htmlPath = htmlPath.replace("~", GlobalConstants.FullPath) + "?no-cache=" + new Date().getTime();
            $.ajax({
                type: 'GET',
                crossDomain: true,
                url: htmlPath,
                success: function (response) {

                    $('#ExamInstructions').append(response)


                    if (htmlPath.trim() != "" && response.trim() != "") {
                        $('#dvExamInstructions').show();
                    }
                    else {
                        //$('#dvExamInstructions').hide();
                        //$("#btnStart").trigger("click");
                        $('#dvExamInstructions').show();
                    }

                },
                error: function (xhr) {

                },
                complete: function () {

                }
            })
        }
        else {
            $("#btnStart").trigger("click");
        }


    }
    var fillQuestionPaper = function (data, module) {
        $("#dvSupportFile,#dvAudioFile").html("");
        $('#dvExamPaper').html("")
        var calls = [];
        var sections = [];
        $(data.TestSections).each(function (i, item) {

            var url = item.TestSectionFileName;
            url = url.replace("~", GlobalConstants.FullPath) + "?no-cache=" + new Date().getTime()
            calls.push($.ajax({
                type: 'GET',
                crossDomain: true,
                url: url,
                success: function (response) {
                    var style = '';
                    if (i > 0) {
                        style = "style=display:none";
                    }
                    var section = $("<atricle data-rowkey=" + item.RowKey + " data-section='" + item.TestSectionName + "' " + style + " class='mb-4'/>")
                    $(section).append("<h2 class='m-0'>" + item.TestSectionName + "</h2><hr class='m-0'/>")
                    $(section).append("<p >" + response + "</p>")
                    sections.push(section[0]);

                },
                error: function (xhr) {

                },
                complete: function () {

                }
            }));
        });

        if (calls.length > 0) {
            $.when.apply($, calls).then(function () {
                sections.sort(function (item1, item2) {
                    var a = item1.dataset.rowkey;
                    var b = item2.dataset.rowkey;
                    return a - b
                });
                $(sections).appendTo('#dvExamPaper');
                $("[data-optionhint]").remove();
                ExamTest.PreventMultiChecking();
                if (data.ExamTestSectionKey) {
                    var QuestionDetails = [];
                    $(data.TestSections).each(function (i, item) {
                        $(item.QuestionDetails).each(function (n, qitem) {
                            QuestionDetails.push(qitem);
                        })

                    });
                    ExamTest.SetAnswersToQuestion(QuestionDetails);
                    $("#dvTestContaner").mLoading("destroy");
                } else {
                    var sec = $('#dvExamPaper').find("atricle").eq(0);
                    $(sec).show();
                    $("input,textarea").on("change", function () {
                        ExamTest.GetAnswersByQuestion();

                    }).bind('copy paste cut', function (e) {
                        e.preventDefault();
                        toastr.warning('cut,copy & paste options are disabled !!', Resources.Warning);
                    }).attr("spellcheck", "false");
                    if (module == Resources.QuestionModuleSpeaking) {
                        ExamTest.RecordEvent();
                    }

                    var index = 0;
                    $("#dvTestContaner").mLoading("destroy");
                    if (module == Resources.QuestionModuleListening) {
                        var audio = $("<audio/>")[0]
                        try {
                            if (data.TestSections[index].SupportedFileName)
                                var url = data.TestSections[index].SupportedFileName;
                            url = url.replace("~", GlobalConstants.FullPath) + "?no-cache=" + new Date().getTime()
                            //var source = $("<source src=" + url + ">");
                            //$(audio).append(source)

                            audio.src = url;
                            $("#dvAudioFile").append(audio)
                            var audioPlayer = new GreenAudioPlayer('#dvAudioFile', { showTooltips: true, stopOthersOnPlay: true, outlineControls: false, showDownloadButton: false, enableKeystrokes: true });
                            audioPlayer.player.play();
                            $("[data-section='" + data.TestSections[index].TestSectionName + "']").trigger("click");
                            audioPlayer.player.addEventListener('ended', function () {
                                index++;
                                if (data.TestSections[index]) {
                                    var url = data.TestSections[index].SupportedFileName;
                                    url = url.replace("~", GlobalConstants.FullPath) + "?no-cache=" + new Date().getTime()
                                    audioPlayer.player.src = url;
                                    audioPlayer.player.play();
                                    $("[data-section='" + data.TestSections[index].TestSectionName + "']").trigger("click");

                                }
                            });
                            //audio.player.play();
                            $(".holder").remove();
                            $(".controls__slider").html("/").addClass("slash_split");
                        } catch (e) {
                            $("#dvTestContaner").mLoading("destroy");
                            toastr.error("Please check your audio device");
                        }
                    }
                    //$('#dvExamPaper').find("atricle").eq(0).fadeIn("slow");

                    autoExpand();
                }

            });

        }

    }
    var fillExamQuestions = function (data) {
        $('#dvQuestionNumber').html("")
        var url = $("#hdnExamQuestions").val() + "?no-cache=" + new Date().getTime();

        $.ajax({
            type: 'GET',
            crossDomain: true,
            url: url,
            success: function (response) {
                var template = Handlebars.compile(response);
                var html = template(data);
                $('#dvQuestionNumber').html(html);


                $("button", $('#dvQuestionNumber')).on("click", function () {
                    var ExamTestSectionKey = $("#ExamTestSectionKey").val();
                    if (!parseInt(ExamTestSectionKey)) {
                        $("atricle", $('#dvExamPaper')).hide();
                    }
                    var question = this.dataset.question;
                    if (question) {
                        $("[data-question=" + question + "]", $('#dvExamPaper')).closest("atricle").show();
                        $("[data-question=" + question + "]", $('#dvExamPaper')).focus();

                    } else {
                        var section = $(this).find("span").html();
                        $("[data-section='" + section + "']", $('#dvExamPaper')).show();

                    }

                });

                //$("#dvQuestionNumber div button:nth-child(1)").attr("data-questionStatus", 2);
                $('[data-question="1"]').trigger("click");

            },
            error: function (xhr) {

            },
            complete: function () {

            }
        })

    }
    var embedSupportFile = function (key, file) {
        file = file.replace("~", GlobalConstants.FullPath)
        $("#dvSupportFile,#dvAudioFile").html("");

        if (key == Resources.QuestionModuleListening) {


        } else if (key == Resources.QuestionModuleReading) {


            $("#supportFileModal").attr("data-url", file);
            $("#btnRead").show();
            //$.ajax({
            //    type: 'GET',
            //    crossDomain: true,
            //    url: file,
            //    success: function (response) {
            //        var para = $("<p/>")
            //        $(para).html(response);
            //        $("#dvSupportFile").append(para)
            //    },
            //    error: function (xhr) {

            //    },
            //    complete: function () {

            //    }
            //})

        }
    }
    var preventMultiChecking = function () {
        $("input[type=checkbox][name*=QuestionNumber]").on("change", function () {
            var key = $(this).closest("[data-key]")[0];
            if (key.hasAttribute("disabled")) {
                this.checked = !this.checked;
            } else {
                var checked = this.checked;
                if (checked) {
                    var group = $(this).closest(".form-group")
                    $(group).find("input").prop("checked", !this.checked)
                    this.checked = checked;
                }
            }
        });
    }



    var formSubmit = function (btn, QuestionStatusKey) {

        var form = $("form")[0];
        var validator = $(form).validate();

        var url = $(form)[0].action;
        var validate = $(form).valid();

        if (validate) {
            $("[disabled]", form).removeAttr("disabled");
            $(".section-content").mLoading();

            var data = $(form).serializeToJSON({
                associativeArrays: true
            });

            var ExamAnswered = ExamTest.GetAnswersByQuestion();

            if ($("#hdnIsShowAllQuestions").val() == "True") {
                data["ExamAnswers"] = [];
                data["ExamAnswers"] = ExamAnswered;

            }
            else {
                data["ExamAnswers"] = [];
                data["ExamAnswers"].push(ExamAnswered[parseInt($("#QuestionNumber").val()) - 1]);

            }

            data.QuestionStatusKey = QuestionStatusKey;
            delete data[""];
            data["ExamStart"] = $("#ExamStart").val();
            AjaxHelper.ajaxWithFileAsync("model", "POST", url, { model: data }, function () {
                response = this;
                if (response.IsSuccessful) {
                    //toastr.success(Resources.Success, response.Message);
                    if ($("#hdnIsShowAllQuestions").val() == "True") {
                        window.location.href = $("#hdnComplateExam").val();
                    }

                    var nextTab = $('#tab-questionmodule li a.active').closest("li").next().find("a");
                    $('#tab-questionmodule li a').removeClass("active")

                    if (QuestionStatusKey != 6) {
                        $(CurrentTargetPage).attr("data-questionStatus", QuestionStatusKey);

                    }
                    AddCount();

                    $(CurrentTargetPage).next().trigger("click");

                    $("#dvQuestionNumber button").removeClass("CurrentPage");
                    $(CurrentTargetPage).addClass("CurrentPage");

                    if (nextTab[0])
                        $(nextTab).trigger("click").addClass("active");
                    else
                        $('#tab-questionmodule li a').eq(0).trigger("click").addClass("active");
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
        var ModuleKey = $("#ModuleKey").val();
        $("[data-key]", $('#dvExamPaper')).each(function () {

            var question = this.dataset.question;
            var value = $("input[type=checkbox]:checked", this).val();

            var AnswerText = $("input[type=checkbox]:checked", this).attr("data-val");
            if (!value)
                value = $("input[type=hidden][name*=QuestionNumber]", this).val();
            if (!value)
                value = $(this).val();

            if (value) {
                $("button[data-question=" + question + "]", $('#dvQuestionNumber')).addClass("active")
            }
            else {
                $("button[data-question=" + question + "]", $('#dvQuestionNumber')).removeClass("active")
            }


            var QuestionStatusKey = $("button[data-question=" + question + "]").attr("data-questionstatus");

            if (AnswerText != undefined) {
                var obj = {};
                obj.QuestionNumber = question;
                obj.AnswerText = AnswerText;
                obj.OptionRowKey = value;
                obj.QuestionStatusKey = QuestionStatusKey;
                if (ModuleKey == Resources.QuestionModuleSpeaking) {
                    var audio = $(this).find("audio")[0];
                    if (audio != null) {
                        obj.QuestionNumber = question;
                        obj.AnswerText = obj.QuestionNumber + ".mp3";
                        var blob = AppCommon.DataURItoBlob(audio.src);
                        var file = AppCommon.BlobToFile(blob, obj.AnswerText, "audio/mpeg");
                        obj.AnswerFile = file;

                    }
                }
                AnswerData.push(obj);
            }


        });
        return AnswerData;
    }
    var setAnswersToQuestion = function (data) {
        var IsExam = $("#IsExam").val();
        IsExam = IsExam ? JSON.parse(IsExam.toLowerCase()) : false;
        var cntrls = ["input", "textarea"]
        var ModuleKey = $("#ModuleKey").val();
        $("#btnSave").hide();
        $("[data-key]", $('#dvExamPaper')).each(function (i) {
            var wrap = $("<span class='result'/>");

            var question = this.dataset.question;
            var result = data.filter(function (item) {
                return item.QuestionNumber == question
            })[0];



            if (Resources.QuestionModuleSpeaking == ModuleKey) {
                var UserKey = $("#ApplicationUserKey").val();
                if (!UserKey) {
                    UserKey = Resources.UserKey
                }
                var recordedAudio = $("<audio controls src=" + (result.AnswerText ? (Resources.ExamTestAnsweFileUrl + UserKey + "/" + $("#RowKey").val() + "/" + result.AnswerText).replace("~", GlobalConstants.FullPath) : "") + "/>")[0];
                $(this).html("").append(recordedAudio);

            }
            else {
                if (result.AnswerText) {
                    if (cntrls.indexOf(this.tagName.toLowerCase()) > -1) {
                        $(this).val(result.AnswerText);

                    } else {
                        $(this).find("input[type=checkbox][value=" + result.AnswerText + "]").prop("checked", true)
                    }
                }
            }
            if (!result.AnswerText || (result.AnswerText && result.Mark != null)) {
                if (result.AnswerText) {
                    if (result.IsCorrect) {
                        $(wrap).addClass("valid")
                    } else {
                        $(wrap).addClass("invalid")
                    }
                } else {
                    $(wrap).addClass("notanswer")
                }
                $(this).wrap(wrap);
                var AnswerKeyPop = "";
                if (result.AnswerKey) {
                    var AnswerKey = result.AnswerKey.replace('|', ',');
                    AnswerKeyPop = 'data-toggle="popover" title="Answer Key" data-content="' + AnswerKey + '"';
                }
                var icons = {
                    valid: '<i class="fa fa-check-circle text-success" aria-hidden="true"></i>',
                    invalid: '<i class="fa  fa-times-circle text-danger" aria-hidden="true" ' + AnswerKeyPop + '></i>',
                    notanswer: '<i class="fa fa-exclamation-circle text-warning" aria-hidden="true" ' + AnswerKeyPop + '></i>'
                }
                if (result.AnswerText) {
                    if (result.IsCorrect) {
                        if (cntrls.indexOf(this.tagName.toLowerCase()) > -1) {

                            $(this).after(icons.valid);

                        } else {
                            $(this).find("input[type=checkbox]:checked").after(icons.valid).addClass('valid');
                        }
                    }
                    else {
                        if (cntrls.indexOf(this.tagName.toLowerCase()) > -1) {
                            $(this).after(icons.invalid);

                        } else {
                            $(this).find("input[type=checkbox]:checked").after(icons.invalid).addClass('invalid')
                        }

                    }
                    if (result.IsCorrect) {
                        $("[data-question=" + result.QuestionNumber + "]", $("#dvQuestionNumber")).removeClass("btn-primary-outline").addClass("btn-success");
                    } else {
                        $("[data-question=" + result.QuestionNumber + "]", $("#dvQuestionNumber")).removeClass("btn-primary-outline").addClass("btn-danger");
                    }
                } else {
                    if (cntrls.indexOf(this.tagName.toLowerCase()) > -1) {
                        $(this).after(icons.notanswer);

                    } else {
                        $(this).find("input[type=checkbox]").eq(0).after(icons.notanswer).addClass('notanswer');
                    }
                    $("[data-question=" + result.QuestionNumber + "]", $("#dvQuestionNumber")).removeClass("btn-primary-outline").addClass("btn-warning");

                }
            }
            else {
                $(this).wrap(wrap);

                var ResultTest = '<input type="text" placeholder="' + result.QuestionNumber + '" id="[' + result.QuestionNumber + '].ExamScore" style="text-align:center;width: 8%;height: 30px;border: solid 1px #265729;margin-left: 2%;color:#265729;font-weight:bold" value="' + (result.Mark ? result.Mark : (result.AnswerText ? "" : "0")) + '" oninput="ExamTest.PreventTypingMaxValue(this)" onclick="this.select()" ></input>';

                if (IsExam) {
                    if (result.Mark)
                        ResultTest = '<i class="fa badge badge-primary badge-pills font-16" aria-hidden="true">' + result.Mark + '</i>';
                    else
                        ResultTest = null;
                } else {
                    $(this).css("width", "90%")
                }
                if (ResultTest) {
                    if (cntrls.indexOf(this.tagName.toLowerCase()) > -1) {
                        $(this).after(ResultTest);

                    } else {
                        var audio = $(this).find("audio")[0];
                        if (audio)
                            $(this).find("audio").eq(0).after(ResultTest);
                    }
                }
                if ($(this).is("textarea"))   //or 
                    wordCountWriting(this);
            }
            $(this).attr("disabled", true);

        });
        $('#dvExamPaper').find("atricle").fadeIn("slow");
        $('[data-toggle="popover"]').popover({
            template: '<div class="popover" role="tooltip"><div class="arrow"></div><h3 class="popover-header bg-success text-white"></h3><div class="popover-body  text-success bg-white"></div></div>',
            trigger: 'hover'
        });

        autoExpand();
    }

    var getExamTime = function (data) {

        var warnPercent = 10;
        $('#dvDuration').removeClass('[class^="w3-text-"]').removeClass('blinking').addClass("w3-text-green");
        $('#dvDuration').html("").show()
        if (examTimer) {
            clearInterval(examTimer);
        }
        if (!data.ExamStatusKey != 2) {

            $("#btnSave").show();

            if ($("#hdnIsShowAllQuestions").val() == "True") {
                $("#btnSaveAll").show();
            }
            //var dateAr = '04-07-2018 1:00:00 PM'.split('-');
            //var newDate = dateAr[1] + '/' + dateAr[0] + '/' + dateAr[2];
            var minutes = $("#ExamDuration").val();
            minutes = parseInt(minutes) ? parseInt(minutes) : 0;

            var now = new Date();
            var min = new Date().getMinutes();
            var end = new Date().setMinutes(new Date().getMinutes() + minutes);
            var initdistance = end - now;
            var interv = Math.floor(initdistance / 3);
            var initialTime = $("#EndTimeMilliSeconds").val();

            if (minutes) {


                end = new Date().setMinutes(new Date().getMinutes() + minutes);


                var _second = 1000;
                var _minute = _second * 60;
                var _hour = _minute * 60;
                var _day = _hour * 24;

                var objExam = {};

                //localStorage.removeItem("exam_" + $("TestPaperKey").val());

                var TestPaperKey = $("#TestPaperKey").val();
                var ApplicationKey = $("#ApplicationKey").val();

                objExam = JSON.parse(localStorage.getItem(data.ExamKey));

                if (data.IsExamExists == true && objExam == null) {
                    //alert("This exam is in progress in another browser");
                    //window.location.href = $("#hdnExamTestList").val();
                }


                if (objExam != null) {

                    //_minute = objExam.Minuts
                    //_hour = objExam.hour;
                    //_second = objExam.second;
                    //end = objExam.end;
                }

                if (objExam == null) {
                    objExam = {}
                }




                examTimer = setInterval(showRemaining, 1000);
                function showRemaining() {

                    now = new Date();
                    end = end - initialTime;
                    var distance = end - now;
                    if (distance < 0) {
                        clearInterval(examTimer);
                        document.getElementById('dvDuration').innerHTML = "";
                        //$("#btnSave").trigger("click");
                        var obj = {};
                        obj.id = $("#TestPaperKey").val();
                        obj.ExamStatusKey = 3;
                        ExamTest.UpdateExamStatus(obj);
                    }
                    else {
                        var days = Math.floor(distance / _day);
                        var hours = Math.floor((distance % _day) / _hour);
                        var minutes = Math.floor((distance % _hour) / _minute);
                        var seconds = Math.floor((distance % _minute) / _second);


                        //objExam.Minuts = _minute;
                        //objExam.hour = _hour;
                        //objExam.second = _second;
                        //objExam.end = end;

                        objExam.examStatus = 1;
                        localStorage.setItem(data.ExamKey, JSON.stringify(objExam));






                        document.getElementById('dvDuration').innerHTML = "";
                        //document.getElementById('DesignTime').value = days + ' days  ';
                        document.getElementById('dvDuration').innerHTML += hours + ' hrs  ';
                        document.getElementById('dvDuration').innerHTML += minutes + ' mins  ';
                        document.getElementById('dvDuration').innerHTML += seconds + ' secs';

                        var Percent = distance * 100 / interv

                        if (Percent <= warnPercent && seconds == 0) {
                            document.getElementById("audBeep").play()


                        }
                        initialTime = 0;

                        if (Percent <= warnPercent) {
                            $('#dvDuration').removeClass('[class="blinking"]').addClass("blinking");
                        }
                        if (distance <= interv) {

                            $('#dvDuration').removeClass('[class^="w3-text-"]').addClass("w3-text-red");

                        }
                        else if (distance <= interv * 2) {
                            $('#dvDuration').removeClass('[class^="w3-text-"]').addClass("w3-text-orange")
                        }

                    }
                }
            }
        }


    }
    var startExam = function () {
        $("#btnStart").hide();
        $("#dvExamInstructions").hide();

        $("#dvDuration").html("")
        $('#dvDuration').removeClass('[class^="w3-text-"]').removeClass('blinking');


        document.body.scrollTop = 0;
        document.documentElement.scrollTop = 0;
        var secs = 30;
        var end = new Date().setSeconds(new Date().getSeconds() + secs);


        var _second = 1000;
        var _minute = _second * 60;
        var _hour = _minute * 60;
        var _day = _hour * 24;

        var timer = setInterval(showRemaining, 1000);
        function showRemaining() {
            var now = new Date();
            var distance = end - now;
            if (distance <= 0) {
                clearInterval(timer);
                $("#btnSave,#dvAudioFile,#dvSupportFile").show();
                $("#btnStart,#btnSkip,#dvDuration").hide();
                $("#dvDuration").html("");
                $("#tab-questionmodule a").removeClass("disabled");
                //$('#tab-questionmodule li a').eq(0).trigger("click").addClass("active");
                $("#btnSkip").trigger("click");

                //ExamTest.QuestionPaperDetail();
            } else {
                var days = Math.floor(distance / _day);
                var hours = Math.floor((distance % _day) / _hour);
                var minutes = Math.floor((distance % _hour) / _minute);
                var seconds = Math.floor((distance % _minute) / _second);
                document.getElementById('dvDuration').innerHTML = "Exam will start after ";
                //document.getElementById('DesignTime').value = days + ' days  ';
                //document.getElementById('dvDuration').innerHTML += hours + ' hrs  ';
                //document.getElementById('dvDuration').innerHTML += minutes + ' mins  ';
                document.getElementById('dvDuration').innerHTML += seconds + ' secs';
                //$("#btnSkip").show();
                $("#btnSkip").trigger("click");
            }
        }
        $("#btnSkip").on("click", function () {
            clearInterval(timer);
            $("#btnSave,#btnBack,#btnNext,#btnClear,#dvAudioFile,#dvSupportFile,#btnSaveAndMark,#MarkForReviewAndNext,#ntVisited").show();
            $("#btnStart,#btnSkip,#dvDuration").hide();
            $("#dvDuration").html("");
            $("#tab-questionmodule a").removeClass("disabled");
            $("#dvQuestionNumber").show();
            $("#ExamReview").show();

            var obj = {};
            obj.id = $("#TestPaperKey").val();
            ExamTest.UpdateExamKey(obj);




            //$('#tab-questionmodule li a').eq(0).trigger("click").addClass("active");
        })

    }

    var examReview = function (id) {
        var obj = {};
        obj.id = id;
        $.ajax({
            url: $("#hdnExamReview").val(),
            type: "GET",
            dataType: "JSON",
            async: true,
            data: obj,
            success: function (result) {
                result = { ExamDetails: result }
                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: $("#hdnExamReviewFilePath").val(),
                    success: function (response) {
                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        var newhtml = template(result);
                        $.customPopupform.CustomPopup({
                            html: newhtml,
                            modalsize: "modal-md",
                            load: function (dailog) {


                            }


                        });
                    },
                    error: function (xhr) {

                    },
                    complete: function () {

                    }
                })
            }
        });

    }
    var recordEvent = function () {
        var btnGroup;
        var rec;

        $("[id*=btnRecord]").on("click", function () {
            $("#btnSave").hide();
            $("[id*=btnRecord]").attr("disabled", true);
            btnGroup = $(this).closest("div.btn-group");
            var StopRecord = $("[id*=btnStopRecord]", btnGroup);
            var Record = this;
            var html = $(this).html();
            $(this).html("Starting...");
            var doSomething = setTimeout(function () {
                clearTimeout(doSomething);
                $(Record).removeAttr("disabled");
                $(Record).hide();
                $(StopRecord).show();
            }, 3000);
            try {
                rec.start();
            } catch (e) {
                $("#dvTestContaner").mLoading("destroy");
                $("[id*=btnRecord]").removeAttr("disabled");
                clearTimeout(doSomething);
                $(Record).html(html);
                $(Record).show();
                $(StopRecord).hide();

                toastr.error("Please check your audio device");
            }


        });
        $("[id*=btnStopRecord]").on("click", function () {
            btnGroup = $(this).closest("div.btn-group");
            $("#btnSave").show();
            $("[id*=btnRecord]").removeAttr("disabled");
            rec.stop();
        });
        var Media = navigator.mediaDevices.getUserMedia({ audio: true });
        if (Media) {
            navigator.mediaDevices.getUserMedia({ audio: true })
                .then(function (stream) { handlerFunction(stream) })
        } else {
            $("[id*=btnRecord]").remove();
        }


        function handlerFunction(stream) {
            rec = new MediaRecorder(stream);
            rec.ondataavailable = function (e) {
                var recordedAudio = $("<audio controls/>")[0];

                if (rec.state == "inactive") {
                    var blob = AppCommon.BlobToDataURI(e.data, function (url) {
                        $(btnGroup).find("button").remove();
                        $(btnGroup).append(recordedAudio)
                        recordedAudio.src = url;

                        //var audio = new GreenAudioPlayer(btnGroup[0], { showTooltips: true, stopOthersOnPlay: true, outlineControls: false, showDownloadButton: false, enableKeystrokes: true });

                    });

                }
            }
        }
    }
    var preventTypingMaxValue = function (_this) {
        var prevInput = $(_this).prev("input,textarea")[0];
        var prevVal = "";
        if (!prevInput) {
            prevInput = $(_this).prev("audio")[0]
            prevVal = prevInput.src;
        } else {
            prevVal = $(prevInput).val()

        }
        if (prevVal) {
            var MaximumScore = $("#MaximumScore").val();
            MaximumScore = parseFloat(MaximumScore) ? parseFloat(MaximumScore) : 0;

            var value = $(_this).val();
            value = parseFloat(value) ? parseFloat(value) : 0;
            if (MaximumScore < value) {
                $(_this).val(MaximumScore);
            }
        } else {
            $(_this).val(0);
        }
    }


    var updateExamStatus = function (obj) {
        $.ajax({
            url: $("#hdnUpdateTestPaperKey").val(),
            type: "GET",
            dataType: "JSON",
            data: obj,
            success: function (result) {
                window.location.href = $("#hdnExamTestList").val();
            }
        });
    }

    var updateExamKey = function (obj) {
        $.ajax({
            url: $("#hdnUpdateExamKey").val(),
            type: "GET",
            dataType: "JSON",
            data: obj,
            success: function (result) {
                responseJson.ExamKey = result.ExamKey;
                $("#EndTimeMilliSeconds").val(result.EndTimeMilliSeconds);
                ExamTest.QuestionPaperDetail();
            }
        });
    }


    function FillDropdown(List, id, selectorId, name) {
        if (selectorId != id) {
            var SelectedItemValue = $(id).val();
            $(id).html("");
            $(id).append($("<option></option>").val("").html(Resources.Select + Resources.BlankSpace + name));
            $(id).val('default').selectpicker("refresh");
            $.each(List, function (i, Data) {
                $(id).append(
                    $('<option></option>').val(Data.RowKey).html(Data.Text));
            });

            $(id).val(SelectedItemValue);
            $(id).selectpicker("refresh");
        }
    }


    var getDropDown = function (_this) {

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
            success: function (result) {




                //FillDropdown(result.Courses, "#SearchCourseKey", "#" + _this.id, "Course");
                FillDropdown(result.Subjects, "#SearchSubjectKey", "#" + _this.id, "Subject");
                FillDropdown(result.SubjectDivisions, "#SearchSubjectDivisionKey", "#" + _this.id, "Division");
                FillDropdown(result.SubjectSubDivisions, "#SearchSubjectSubDivisionKey", "#" + _this.id, "Sub Division");



            },
            xhr: function () {
                // get the native XmlHttpRequest object
                var xhr = $.ajaxSettings.xhr();
                // set the onprogress event handler
                xhr.upload.onprogress = function (evt) {

                };

                xhr.onreadystatechange = function () {
                    if (xhr.readyState === 4) {

                        $(".section-content").mLoading("destroy");




                    }
                }


                return xhr;
            },
            error: function (request, status, error) {
                console.log(request.responseText);

            }
        });


    }


    var getExamTestDetails = function (json, resetPagination, PageNumber) {
        JsonData = json;

        var newPostData = null;
        newPostData = $.extend(true, {}, json);

        var JsonData = $("form").serializeToJSON({

        });
        $.extend(newPostData, JsonData)

        newPostData["PageSize"] = 10;
        newPostData["PageIndex"] = PageNumber;
        newPostData["PageIndex"] = PageNumber;
        newPostData["SearchExamFilterTypeKey"] = $("#SearchExamFilterTypeKey").val();

        $("#ExamTestDetails").mLoading();


        request = $.ajax({
            url: $("#hdnGetExamTestDetails").val(),
            data: newPostData,
            datatype: "json",
            type: "post",
            contenttype: 'application/json; charset=utf-8',
            async: true,
            beforeSend: function () {
                if (request != null) {
                    request.abort();
                }
            },
            success: function (data) {
                $("#ExamTestDetails").html(data);

                if (resetPagination) {
                    ExamTestPagination();
                }

                $("#ExamTestDetails").mLoading("destroy");
                //$("#StudyMaterialssList").mLoading("destroy");
            },
            error: function (error) {
                if (error.statusText != "abort") {
                    console.log(JSON.stringify(error));


                }
            }
        });
    }


    return {
        QuestionPaperDetail: questionPaperDetail,
        FillQuestionPaper: fillQuestionPaper,
        EmbedSupportFile: embedSupportFile,
        PreventMultiChecking: preventMultiChecking,
        FormSubmit: formSubmit,
        FillExamQuestions: fillExamQuestions,
        GetAnswersByQuestion: getAnswersByQuestion,
        SetAnswersToQuestion: setAnswersToQuestion,
        GetExamTime: getExamTime,
        LoadExamInstruction: loadExamInstruction,
        StartExam: startExam,
        ExamReview: examReview,
        RecordEvent: recordEvent,
        PreventTypingMaxValue: preventTypingMaxValue,
        UpdateExamStatus: updateExamStatus,
        UpdateExamKey: updateExamKey,
        GetExamTestDetails: getExamTestDetails,
        GetDropDown: getDropDown

    }
}());

function autoExpand() {
    var autoExpand = function (field) {

        // Reset field height
        field.style.height = 'inherit';

        // Get the computed styles for the element
        var computed = window.getComputedStyle(field);

        // Calculate the height
        var height = parseInt(computed.getPropertyValue('border-top-width'), 10)
            + parseInt(computed.getPropertyValue('padding-top'), 10)
            + field.scrollHeight
            + parseInt(computed.getPropertyValue('padding-bottom'), 10)
            + parseInt(computed.getPropertyValue('border-bottom-width'), 10);

        field.style.height = height + 'px';

    };
    $("textarea").on('input', function () {
        autoExpand(this);
    });
    $("textarea").each(function () {
        autoExpand(this);
    });
}

function wordCountWriting(txt) {


    var number = 0;
    var text = $(txt).val();


    text = this.removeUncountablePartsFromText(text)

    var words, matches;
    var text_without_space = text.replace(/\s+/g, '');
    matches = text_without_space.match(/([^\x00-\x7F\u2013\u2014])+/gi);
    var latin_only = (matches === null);
    if (latin_only == false) {
        words = text.match(/\S+/g);
    } else {
        words = text.replace(/[;!:—\/]/g, ' ').replace(/\.\s+/g, ' ').replace(/[^a-zA-Z\d\s&:,]/g, '').replace(/,([^0-9])/g, ' $1').match(/\S+/g);
    }
    number = words.length;
    $(txt).closest("span.result").attr("data-wordcount", number + ' word' + (number != 1 ? 's' : ''));
}

function removeUncountablePartsFromText(text) {
    var filteredText = text;
    var openIndex = 0
        , closeIndex = 0
        , index = 0;
    while (true) {
        if (filteredText.indexOf('~~~') !== -1) {
            openIndex = filteredText.indexOf('~~~');
            index = openIndex + 3;
            if (filteredText.indexOf('~~~', index) !== -1) {
                closeIndex = filteredText.indexOf('~~~', index) + 3;
                filteredText = filteredText.slice(0, openIndex) + filteredText.slice(closeIndex);
            } else {
                break;
            }
        } else {
            break;
        }
    }
    return filteredText;
}




function ExamTestPagination() {

    $('#page-selection-down').empty();
    var totalRecords = $("#hdnTotalRecords").val();
    totalRecords = totalRecords != "" ? parseInt(totalRecords) : 0;
    var Size = 10;
    var totalPages = Math.floor(totalRecords % Size == 0 ? totalRecords / Size : (totalRecords / Size) + 1);
    var page = jsonData["PageIndex"];
    page = parseInt(page) ? (parseInt(page) <= totalPages ? parseInt(page) : totalPages) : 1;

    $('#page-selection-down').bootpag({
        total: totalPages,
        page: page,
        maxVisible: 10
    })

    $('#page-selection-down').off("page").on("page", function (event, num) {
        $("#PageIndex").val(num);
        $('#page-selection-down').bootpag({ page: num })

        ExamTest.GetExamTestDetails(jsonData, false, num);

    });

};
