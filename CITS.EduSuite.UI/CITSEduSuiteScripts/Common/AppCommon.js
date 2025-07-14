var agent = navigator.userAgent;
var AppCommon = (function () {

    var formatString = function (str, col) {
        col = typeof col === 'object' ? col : Array.prototype.slice.call(arguments, 1);

        return str.replace(/\{\{|\}\}|\{(\w+)\}/g, function (m, n) {
            if (m == "{{") { return "{"; }
            if (m == "}}") { return "}"; }
            return col[n];
        });
    };

    var toString = function (data) {
        return JSON.parse(data).toString();
    }

    var emptyGuid = function () {
        return "00000000-0000-0000-0000-000000000000";
    }

    var isValidStringInput = function (e, inputData) {
        var isValid = true;
        if (inputData.length > 0) {
            var regExp = /^[0-9a-zA-Z\-\s]*$/
            if (!regExp.test(inputData)) {
                e.preventDefault();
                isValid = false;
            }
        }
        return isValid;
    }

    var addMonths = function (date, months) {
        date.setMonth(date.getMonth() + months);
        return date;
    }
    var addHours = function (date, hours) {
        date.setHours(date.getHours() + hours);
        return date;
    }


    // Read a page's GET URL variables and return them as an associative array.
    var getQueryStringParams = function (sParam) {

        var sPageURL = window.location.search.substring(1);
        var sURLVariables = sPageURL.split('&');
        for (var i = 0; i < sURLVariables.length; i++) {
            var sParameterName = sURLVariables[i].split('=');
            if (sParameterName[0] == sParam) {
                return sParameterName[1];
            }
        }
    }

    var formatTimeAMPM = function (inputTime) {
        var hours = inputTime.getHours();
        var minutes = inputTime.getMinutes();
        var ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        hours = hours.toString().length == 2 ? hours : '0' + hours;
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    }

    var formatObjectToTimeAMPM = function (inputTime) {
        var hours = inputTime.Hours;
        var minutes = inputTime.Minutes;
        var seconds = inputTime.Seconds;
        var ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        hours = hours.toString().length == 2 ? hours : '0' + hours;
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    }

    var formatAMPMTo24Hrs = function (inputTime) {
        var time = inputTime
        var hours = Number(time.match(/^(\d+)/)[1]);
        var minutes = Number(time.match(/:(\d+)/)[1]);
        var seconds = Number(time.match(/:(\d+ )/)[1]);
        var AMPM = time.match(/\s(.*)$/)[1];
        if (AMPM == "PM" && hours < 12) hours = hours + 12;
        if (AMPM == "AM" && hours == 12) hours = hours - 12;
        var sHours = hours.toString();
        var sMinutes = minutes.toString();
        var sSeconds = seconds.toString();
        if (hours < 10) sHours = "0" + sHours;
        if (minutes < 10) sMinutes = "0" + sMinutes;
        if (seconds < 10) sSeconds = "0" + sSeconds;
        return sHours + ":" + sMinutes + ":" + sSeconds;
    }

    function timeFromJavascriptDate(date) {
        var hh = date.getHours()
        var mm = date.getMinutes()
        var ss = date.getSeconds()

        return hh + ":" + mm + ":" + ss;

    }
    function timeAMPMFromJavascriptDate(date) {
        var hours = date.getHours()
        var minutes = date.getMinutes()
        var seconds = date.getSeconds()
        var ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        hours = hours.toString().length == 2 ? hours : '0' + hours;
        minutes = minutes < 10 ? '0' + minutes : minutes;
        seconds = seconds < 10 ? '0' + seconds : seconds;
        var strTime = hours + ':' + minutes + ':' + seconds + ' ' + ampm;
        return strTime;
    }

    var validateGuid = function (guid) {
        return /[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}/.test(guid);
    }


    var jsonDateToNormalDate2 = function (value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        if (results != null) {
            var dt = new Date(parseFloat(results[1]));
            return dt.getDate() + "/" + ((dt.getMonth() + 1) < 10 ? ("0" + (dt.getMonth() + 1)) : (dt.getMonth() + 1)) + "/" + dt.getFullYear();
        }
        return value;
    }

    var formatDateInput = function () {
        if (agent.toLowerCase().search("mobile") > 0) {
            $("input[data-input-type=date]").attr("type", "date");
            $("input[data-input-type=date]").each(function () {
                var d = $(this).attr("value").split("/");
                if (d.length == 3)
                    $(this).val(d[2] + "-" + d[1] + "-" + d[0])
            })
        }
        else {
            $("input[data-input-type=date],input[data-input-type=datetime]").inputmask();
            $("input[data-input-type=date]").datepicker({ autoclose: true, format: 'dd/mm/yyyy', todayHighlight: true });

           // $("input[data-input-type=date]").datepicker('setEndDate', new Date());
            $('#NextCallSchedule').datepicker('setEndDate', '');
            $('#NextCallSchedule').datepicker('setStartDate', new Date());
            $('#LeadDate').datepicker('setEndDate', '');
            $('#DurationFromDate').datepicker('setStartDate', '');
            $('#DurationFromDate').datepicker('setEndDate', '');
            $('#DurationToDate').datepicker('setStartDate', '');
            $('#DurationToDate').datepicker('setEndDate', '');
            $('#ExamDate').datepicker('setStartDate', '');
            $('#ExamDate').datepicker('setEndDate', '');
            $('#LeaveFrom').datepicker('setStartDate', '');
            $('#LeaveFrom').datepicker('setEndDate', '');
            $('#LeaveTo').datepicker('setStartDate', '');
            $('#LeaveTo').datepicker('setEndDate', '');
            


        }
        if (jQuery.validator != undefined && agent.toLowerCase().search("mobile") <= 0) {
            jQuery.validator.methods.date = function (value, element) {
                var d = value.split("/");
                return this.optional(element) || !/Invalid|NaN/.test(new Date((/chrom(e|ium)|opera|safari|firefox|msie/.test(navigator.userAgent.toLowerCase())) && d.length === 3 ? d[1] + "/" + d[0] + "/" + d[2] : value));
            };
        }
      
       
            
    }
    var formatTimeInput = function () {

        if (agent.toLowerCase().search("mobile") > 0) {
            $("input[data-input-type=time]").attr("type", "time");

        }
        else {
            $('input[data-input-type=time]').each(function () {
                $(this)
                    //.timepicker({
                    //    defaultTime: "",
                    //    secondStep: 1,
                    //    minuteStep: 1,
                    //    showSeconds:true
                    //})
                    .inputmask({
                        insertMode: false,
                        showMaskOnHover: false,
                        hourFormat: 12
                    });
            })
        }


    }

    var formatDurationInput = function () {
        $("input[data-input-type=duration]").timesetter({
            hour: {
                value: 0,
                min: 0,
                max: 24,
                step: 1,
                symbol: "hrs"
            },
            minute: {
                value: 0,
                min: 0,
                max: 60,
                step: 1,
                symbol: "mins"
            },
            second: {
                value: 0,
                min: 0,
                max: 60,
                step: 1,
                symbol: "secs"
            },
            direction: "increment", // increment or decrement
            inputHourTextbox: null, // hour textbox
            inputMinuteTextbox: null, // minutes textbox
            postfixText: "", // text to display after the input fields
            numberPaddingChar: '0' // number left padding character ex: 00052
        });
    }

    var formatYearInput = function () {
        $('input[data-input-type=year]').datepicker({
            startView: 2,
            format: 'yyyy',
            minViewMode: 2,
            autoclose: true,
        }).inputmask({
            insertMode: false,
            showMaskOnHover: false,
            mask: "y"
        });


    }
    var formatMonthInput = function () {
        $('input[data-input-type=month]').datepicker({
            format: "M yyyy",
            startView: "months",
            minViewMode: "months",
            autoclose: true,
        })


    }
    var formatDayInput = function () {
        $('input[data-input-type=day]').datepicker({
            format: "M dd",
            weekStart: 1,
            startView: 1,
            maxViewMode: 1,
            autoclose: true
        })
    }
    var formatSelect2 = function () {
        $('select[data-plugin-type=select2]').each(function () {
            $(this).select2({
                multiple: true
            });
        })

    }

    var formatMultiSelect = function (ctnrl) {
        ctnrl = ctnrl ? ctnrl : ('.multiselect');
        $(ctnrl).each(function () {
            $(this).multiselect({
                includeSelectAllOption: true,
                buttonWidth: '100%',
                buttonClass: $(this).attr("class"),
                nonSelectedText: $(this).data("placeholder")
            });
        });
    }

    var formatInputCase = function () {
        $("input[type=text]").on('input', function (evt) {

            var input = $(this);
            var start = input[0].selectionStart;
            var maxLength = $(this).data("maxValue");
            var value = $(this).val();
            $(this).val(function (_, val) {
                if (val != "") {
                    var firstLetter = val.split("")[0].toUpperCase().trim();
                    var subStr = val.substring(1);

                    if (maxLength < val.length)
                        return (firstLetter + subStr).substring(0, maxLength);
                    else
                        return firstLetter + subStr;
                }
            });
            if (maxLength < value.length) {
                evt.preventDefault();
            }
            if (start)
                input[0].selectionStart = input[0].selectionEnd = start;
        });
        $("textarea").on('input', function (evt) {
            var input = $(this);
            var start = input[0].selectionStart;
            $(this).val(function (_, val) {
                var returnStr = "";
                if (val != "") {
                    var arr = val.split(/\n/g) || [];
                    arr.forEach(function (part, index, theArray) {
                        var firstLetter = part.split("")[0].toUpperCase().trim();
                        var subStr = part.substring(1);
                        theArray[index] = firstLetter + subStr;
                    })
                }
                return arr.join('\n');
            });
            if (start)
                input[0].selectionStart = input[0].selectionEnd = start;
        });
        $("input[data-convert=uppercase] , textarea[data-convert=uppercase]").on('input', function (evt) {
            var input = $(this);
            var start = input[0].selectionStart;
            $(this).val(function (_, val) {
                return val.toUpperCase();
            });
            if (start)
                input[0].selectionStart = input[0].selectionEnd = start;
        });
        $("input[data-convert=lowercase] , textarea[data-convert=lowercase]").on('input', function (evt) {
            var input = $(this);
            var start = input[0].selectionStart;
            $(this).val(function (_, val) {
                return val.toLowerCase();
            });
            if (start)
                input[0].selectionStart = input[0].selectionEnd = start;
        });
        $("input[data-convert=none] , textarea[data-convert=none]").off('input').on('input', function (evt) {
            var input = $(this);
            var start = input[0].selectionStart;
            var maxLength = $(this).data("maxValue");
            $(this).val(function (_, val) {
                if (val != "") {
                    var firstLetter = val.split("")[0].trim();
                    var subStr = val.substring(1);

                    if (maxLength < val.length)
                        return (firstLetter + subStr).substring(0, maxLength);
                    else
                        return firstLetter + subStr;
                }
            });
            if (start)
                input[0].selectionStart = input[0].selectionEnd = start;
        });
    }

    var formatAutoComplete = function () {
        $('input[data-plugin-type=autocomplete]').each(function () {
            var _this = this;
            var dataset = _this.dataset;
            $(this).attr("autocomplete", "off")
            $(this).typeahead({
                hint: true,
                highlight: true, /* Enable substring highlighting */
                minLength: 1,/* Specify minimum characters required for showing suggestions */
                source: function (query, process) {
                    $.ajax({
                        url: $(_this).data("url"),
                        data: { query: query },
                        dataType: "json",
                        type: "GET",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            var resultList = data.map(function (item) {

                                var aItem = {
                                    val: item.Value,
                                    name: item.Name,
                                };
                                return JSON.stringify(aItem);
                            });

                            return process(resultList);
                        },
                        error: function (response) {
                            console.log(response.responseText);
                        },
                        failure: function (response) {
                            console.log(response.responseText);
                        }
                    })
                },
                matcher: function (obj) {
                    var item = JSON.parse(obj);
                    return ~item.name.toLowerCase().indexOf(this.query.toLowerCase())
                },

                sorter: function (items) {
                    var beginswith = [], caseSensitive = [], caseInsensitive = [], item;
                    while (aItem = items.shift()) {
                        var item = JSON.parse(aItem);
                        if (!item.name.toLowerCase().indexOf(this.query.toLowerCase())) beginswith.push(JSON.stringify(item));
                        else if (~item.name.indexOf(this.query)) caseSensitive.push(JSON.stringify(item));
                        else caseInsensitive.push(JSON.stringify(item));
                    }

                    return beginswith.concat(caseSensitive, caseInsensitive)

                },


                highlighter: function (obj) {
                    var item = JSON.parse(obj);
                    var query = this.query.replace(/[\-\[\]{}()*+?.,\\\^$|#\s]/g, '\\$&')
                    return item.name.replace(new RegExp('(' + query + ')', 'ig'), function ($1, match) {
                        return '<strong>' + match + '</strong>'
                    })
                },

                updater: function (obj) {
                    var item = JSON.parse(obj);
                    $(_this).val(item.val);
                    if (dataset.class)
                        window[dataset.class][dataset.function]();
                    else
                        $(_this).trigger("change")
                    return item.val

                }
            });
        })

    }

    //var customRepeaterRemoteMethod = function () {

    //    if (jQuery.validator != undefined) {
    //        jQuery.validator.methods.remote = function (value, element, param) {
    //            var data = param.data;
    //            var newData = {};
    //            var repeater = $(element).closest("div.repeater")[0];
    //            var container = $(element).closest("div[data-repeater-item]")[0];
    //            var name = $(element).attr('name').match(/\].*/) ? $(element).attr('name').match(/\].*/)[0].replace("].", "") : name;
    //            var similarCntrls = [], similarCntrlElements = [];
    //            $("input[name*=" + name + "],select[name*=" + name + "],textarea[name*=" + name + "]", repeater).each(function () {
    //                if (this.name != element.name) {
    //                    similarCntrls.push($(this).val());
    //                    similarCntrlElements.push($(this)[0]);
    //                }
    //            });

    //            var unique = $(element).data('unique');
    //            var checkmultiple = $(element).data('checkmultiple');
    //            var remoteType = $(element).data('remote-server');
    //            var notnull = $(element).data('notnull');
    //            $.each(data, function (key, value) {
    //                key = key.match(/\].*/) ? key.match(/\].*/)[0].replace("].", "") : key;
    //                var newValue = $("#" + key).val();
    //                if (newValue == undefined)
    //                    newValue = $("input[name*=" + key + "],select[name*=" + key + "],textarea[name*=" + key + "]", container).val();
    //                newData[key] = function () {
    //                    return newValue != "" ? newValue : "0";
    //                }

    //            });


    //            param.data = newData;
    //            if (this.optional(element) && !notnull) {
    //                return "dependency-mismatch";
    //            }

    //            var previous = this.previousValue(element);
    //            if (!this.settings.messages[element.name]) {
    //                this.settings.messages[element.name] = {};
    //            }
    //            previous.originalMessage = this.settings.messages[element.name].remote;
    //            this.settings.messages[element.name].remote = previous.message;

    //            param = typeof param === "string" && { url: param } || param;

    //            //if (previous.old === value) {
    //            //    return previous.valid;
    //            //}

    //            previous.old = value;
    //            var validator = this;
    //            this.startRequest(element);
    //            var data = {};
    //            data[element.name] = value;
    //            var valid = true;
    //            if (checkmultiple && similarCntrlElements.length > 0) {
    //                for (var i = 0; i < similarCntrlElements.length; i++) {
    //                    var similarElement = similarCntrlElements[i];
    //                    var item = $(similarElement).closest("div[data-repeater-item]")[0];
    //                    if (similarElement.name != element.name) {
    //                        for (var key in param.data) {
    //                            key = key.match(/\].*/) ? key.match(/\].*/)[0].replace("].", "") : key;
    //                            var newCheckValue = $("#" + key).val();
    //                            if (newCheckValue == undefined)
    //                                newCheckValue = $("input[name*=" + key + "],select[name*=" + key + "],textarea[name*=" + key + "]", container).val();
    //                            var newCheckableValue = $("#" + key).val();
    //                            if (newCheckableValue == undefined)
    //                                newCheckableValue = $("input[name*=" + key + "],select[name*=" + key + "],textarea[name*=" + key + "]", item).val();
    //                            if (newCheckValue === newCheckableValue) {
    //                                valid = false;
    //                            }
    //                            else {
    //                                valid = true;
    //                                break
    //                            }

    //                        }
    //                        if (!valid) {
    //                            break;
    //                        }
    //                    }
    //                }
    //                return valid;
    //            }
    //            else if (similarCntrls.indexOf($(element).val()) > -1) {
    //                //var errors = {};
    //                //var message = validator.defaultMessage(element, "remote");
    //                //errors[element.name] = previous.message = $.isFunction(message) ? message(value) : message;
    //                //validator.invalid[element.name] = true;
    //                //validator.showErrors(errors);
    //                valid = false;
    //                return valid;
    //                //previous.valid = valid;
    //                // validator.stopRequest(element, valid);
    //            }
    //            else if (unique) {
    //                $.ajax($.extend(true, {
    //                    url: param,
    //                    mode: "abort",
    //                    port: "validate" + element.name,
    //                    dataType: "json",
    //                    async: true,
    //                    data: data,
    //                    success: function (response) {
    //                        //validator.settings.messages[element.name].remote = previous.originalMessage;
    //                        valid = response === true || response === "true";
    //                        //return valid;
    //                        //previous.valid = valid;
    //                        //validator.stopRequest(element, valid);
    //                        return valid;
    //                    }
    //                }, param));
    //                return "pending";
    //            }
    //            else {
    //                //var submitted = validator.formSubmitted;
    //                //validator.prepareElement(element);
    //                //validator.formSubmitted = submitted;
    //                //validator.successList.push(element);
    //                //delete validator.invalid[element.name];
    //                //validator.showErrors();
    //                //return "pending";
    //                return valid;
    //            }
    //            //if (!valid) {
    //            //    var errors = {};
    //            //    var message = validator.defaultMessage(element, "remote");
    //            //    errors[element.name] = previous.message = $.isFunction(message) ? message(value) : message;
    //            //    validator.invalid[element.name] = true;
    //            //    validator.showErrors(errors);
    //            //    valid = false;
    //            //    previous.valid = valid;
    //            //    validator.stopRequest(element, valid);

    //            //}
    //            //else {
    //            //    var submitted = validator.formSubmitted;
    //            //    validator.prepareElement(element);
    //            //    validator.formSubmitted = submitted;
    //            //    validator.successList.push(element);
    //            //    delete validator.invalid[element.name];
    //            //    validator.showErrors();

    //            //}
    //            return "pending";


    //        };

    //    }
    //}
    var customRepeaterRemoteMethod = function () {

        if (jQuery.validator != undefined) {
            jQuery.validator.methods.remote = function (value, element, param) {
                var data = param.data;
                var newData = {};
                var repeater = $(element).closest(".repeater")[0];
                var container = $(element).closest("[data-repeater-item]")[0];
                var name = $(element).attr('name').match(/\].*/) ? $(element).attr('name').match(/\].*/)[0].replace("].", "") : name;
                var similarCntrls = [], similarCntrlElements = [];
                $("input[type=" + element.type + "][name*=" + name + "],select[name*=" + name + "],textarea[name*=" + name + "]", repeater).each(function () {
                    if (this.name != element.name) {
                        similarCntrls.push($(this).val());
                        similarCntrlElements.push($(this)[0]);
                    }
                });

                var unique = $(element).data('unique');
                var checkmultiple = $(element).data('checkmultiple');
                var remoteType = $(element).data('remote-server');
                var notnull = $(element).data('notnull');
                $.each(data, function (key, value) {
                    key = key.match(/\].*/) ? key.match(/\].*/)[0].replace("].", "") : key;
                    var newValue = $("#" + key).val();
                    if (newValue == undefined || container)
                        newValue = $("input[name*=" + key + "],select[name*=" + key + "],textarea[name*=" + key + "]", container).val();
                    newData[key] = function () {
                        return newValue != "" ? newValue : "0";
                    }
                });


                param.data = newData;
                if (this.optional(element) && !notnull) {
                    return "dependency-mismatch";
                }

                var previous = this.previousValue(element);
                if (!this.settings.messages[element.name]) {
                    this.settings.messages[element.name] = {};
                }
                previous.originalMessage = this.settings.messages[element.name].remote;
                this.settings.messages[element.name].remote = previous.message;

                param = typeof param === "string" && { url: param } || param;

                if (previous.old === value && unique && similarCntrlElements.length == 0) {
                    return previous.valid;
                }

                previous.old = value;
                var validator = this;
                this.startRequest(element);
                var data = {};
                data[element.name] = value;
                var valid = true;
                if (checkmultiple && similarCntrlElements.length > 0) {
                    for (var i = 0; i < similarCntrlElements.length; i++) {
                        var similarElement = similarCntrlElements[i];
                        var item = $(similarElement).closest("div[data-repeater-item]")[0];
                        if (similarElement.name != element.name) {
                            for (var key in param.data) {
                                key = key.match(/\].*/) ? key.match(/\].*/)[0].replace("].", "") : key;
                                var newCheckValue = $("#" + key).val();
                                if (newCheckValue == undefined)
                                    newCheckValue = $("input[name*=" + key + "],select[name*=" + key + "],textarea[name*=" + key + "]", container).val();
                                var newCheckableValue = $("#" + key).val();
                                if (newCheckableValue == undefined)
                                    newCheckableValue = $("input[name*=" + key + "],select[name*=" + key + "],textarea[name*=" + key + "]", item).val();
                                if (newCheckValue === newCheckableValue) {
                                    valid = false;
                                }
                                else {
                                    valid = true;
                                    break
                                }

                            }
                            if (!valid) {
                                break;
                            }
                        }
                    }
                    return valid;
                }
                else if (similarCntrls.indexOf($(element).val()) > -1) {
                    valid = false;
                    return valid;
                }
                else if (unique) {
                    $.ajax($.extend(true, {
                        url: param,
                        mode: "abort",
                        port: "validate" + element.name,
                        dataType: "json",
                        async: true,
                        data: data,
                        success: function (response) {
                            validator.settings.messages[element.name].remote = previous.originalMessage;
                            var valid = response === true || response === "true";

                            if (valid) {
                                var submitted = validator.formSubmitted;
                                validator.prepareElement(element);
                                validator.formSubmitted = submitted;
                                validator.successList.push(element);
                                delete validator.invalid[element.name];
                                validator.showErrors();
                            } else {
                                var errors = {};
                                var message = response || validator.defaultMessage(element, "remote");
                                errors[element.name] = previous.message = $.isFunction(message) ? message(value) : message;
                                validator.invalid[element.name] = true;
                                validator.showErrors(errors);
                            }
                            previous.valid = valid;
                            validator.stopRequest(element, valid);
                        }
                    }, param));
                    return "pending";
                }
                else {
                    //var submitted = validator.formSubmitted;
                    //validator.prepareElement(element);
                    //validator.formSubmitted = submitted;
                    //validator.successList.push(element);
                    //delete validator.invalid[element.name];
                    //validator.showErrors();
                    //return "pending";
                    return valid;
                }
                //if (!valid) {
                //    var errors = {};
                //    var message = validator.defaultMessage(element, "remote");
                //    errors[element.name] = previous.message = $.isFunction(message) ? message(value) : message;
                //    validator.invalid[element.name] = true;
                //    validator.showErrors(errors);
                //    valid = false;
                //    previous.valid = valid;
                //    validator.stopRequest(element, valid);

                //}
                //else {
                //    var submitted = validator.formSubmitted;
                //    validator.prepareElement(element);
                //    validator.formSubmitted = submitted;
                //    validator.successList.push(element);
                //    delete validator.invalid[element.name];
                //    validator.showErrors();

                //}
                return "pending";

            };
        }
    }

    var customRemoteMethod = function () {
        if (jQuery.validator != undefined) {
            jQuery.validator.methods.remote = function (value, element, param) {
                if (this.optional(element)) {
                    return "dependency-mismatch";
                }

                var previous = this.previousValue(element);
                if (!this.settings.messages[element.name]) {
                    this.settings.messages[element.name] = {};
                }
                previous.originalMessage = this.settings.messages[element.name].remote;
                this.settings.messages[element.name].remote = previous.message;

                param = typeof param === "string" && { url: param } || param;

                if (previous.old === value) {
                    return previous.valid;
                }

                previous.old = value;
                var validator = this;
                this.startRequest(element);
                var data = {};
                data[element.name] = value;
                $.ajax($.extend(true, {
                    url: param,
                    mode: "abort",
                    port: "validate" + element.name,
                    dataType: "json",
                    data: data,
                    success: function (response) {
                        validator.settings.messages[element.name].remote = previous.originalMessage;
                        var valid = response === true || response === "true";
                        if (valid) {
                            var submitted = validator.formSubmitted;
                            validator.prepareElement(element);
                            validator.formSubmitted = submitted;
                            validator.successList.push(element);
                            delete validator.invalid[element.name];
                            validator.showErrors();
                        } else {
                            var errors = {};
                            var message = response || validator.defaultMessage(element, "remote");
                            errors[element.name] = previous.message = $.isFunction(message) ? message(value) : message;
                            validator.invalid[element.name] = true;
                            validator.showErrors(errors);
                        }
                        previous.valid = valid;
                        validator.stopRequest(element, valid);
                    }
                }, param));
                return "pending";
            }
        }
    }


    //var jsonDateToNormalDate = function (value) {
    //    var pattern = /Date\(([^)]+)\)/;
    //    var results = pattern.exec(value);
    //    if (results != null) {
    //        var dt = new Date(parseFloat(results[1]));
    //        return ("0" + (dt.getMonth() + 1)).slice(-2) + "/" + ("0" + dt.getDate()).slice(-2) + "/" + dt.getFullYear();
    //    }
    //    return value;
    //}

    var jsonDateToNormalDate = function (value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        if (results != null) {
            var dt = new Date(parseFloat(results[1]));
            var month = (parseInt(dt.getMonth()) + 1);
            var day = (parseInt(dt.getDate()));

            return (day < 10 ? "0" + day : day) + "/" + (month < 10 ? "0" + month : month) + "/" + dt.getFullYear();
        }
        return value;
    }

    var jsonDateToServerDate = function (value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        if (results != null) {
            var dt = new Date(parseFloat(results[1]));
            return ("0" + dt.getDate()).slice(-2) + "/" + ("0" + (dt.getMonth() + 1)).slice(-2) + "/" + dt.getFullYear();
        }
        return value;
    }

    var getAccountBalanceById = function (label, Id) {
        $label = $(label);
        $label.html(""); // clear before appending new list 
        $label.removeClass("full-rndbracket")
        if (Id != "") {
            $.ajax({
                url: Resources.ServerPath + "BankAccount/GetAccountBalanceByAccount/" + Id,
                type: "GET",
                dataType: "JSON",
                data: { id: Id },
                success: function (result) {
                    $label.html(result.BankAccountBalance);
                    if (result.BankAccountBalance < 0) {
                        $label.removeClass("text-green").addClass("text-red");
                    }
                    else {
                        $label.removeClass("text-red").addClass("text-green");
                    }
                    $label.addClass("full-rndbracket")
                }
            });
        }
    }

    function objectifyForm(formArray) {//serialize data function

        var returnArray = {};
        for (var i = 1; i < formArray.length; i++) {
            var name = formArray[i]['name'];
            if ($("input[name='" + name + "']")[0] && $($("input[name='" + name + "']")[0]).is(':checkbox')) {

                returnArray[formArray[i]['name']] = $($("input[name='" + name + "']")[0]).prop('checked');
            }
            else {
                returnArray[formArray[i]['name']] = formArray[i]['value'];
            }

        }
        return returnArray;
    }
    var digitalClock = function (IsTime) {
        var today = new Date();
        //changed by khaleefa forQuick Attendance on 08 Apr 2019 start
        var date = !IsTime ? moment(today, ["DD/MM/YYYY"]).format("DD/MM/YYYY") : "";

        //changed by khaleefa forQuick Attendance on 08 Apr 2019 End

        var h = today.getHours();
        var m = today.getMinutes();
        var s = today.getSeconds();
        m = checkTime(m);
        s = checkTime(s);
        var ampm = h >= 12 ? 'PM' : 'AM';
        h = h % 12;
        h = h ? h : 12; // the hour '0' should be '12'
        h = h.toString().length == 2 ? h : '0' + h;
        m = m < 10 ? m : m;
        $("[data-digital-clock]").html(h + ":" + m + ":" + s + " " + ampm);
        //changed by khaleefa forQuick Attendance on 08 Apr 2019
        $("[data-digital-clock-date]").html(date + " " + h + ":" + m + ":" + s + " " + ampm);
        //changed by khaleefa forQuick Attendance on 08 Apr 2019 End

        var t = setTimeout(digitalClock, 500);
    }


    function setPhoneInputAddOn(_this) {
        var parentDiv = $(_this).closest("div")[0];
        var width = $(_this).html().length * 5;
        $(parentDiv).find("input").css('text-indent', (width).toString() + 'px');
    }
    var getDefaultById = function (uid) {
        userStorage = JSON.parse(localStorage.getItem('userDefault_' + uid));
        if (userStorage === null)
            userStorage = {};
        return userStorage;
    }

    var setDefaultById = function (uid, key, value) {
        userStorage = JSON.parse(localStorage.getItem('userDefault_' + uid));
        if (userStorage === null)
            userStorage = {};

        userStorage[key] = value;
        localStorage.setItem('userDefault_' + uid, JSON.stringify(userStorage));
    }

    var getUserData = function () {
        var userData = [];
        $.ajaxSetup({ async: false });
        $.get(Resources.GetUserDataURL, function (response) { userData = response });
        return userData;
    }
    var setDefaultToControls = function () {
        var userData = AppCommon.GetUserData();
        if (userData) {
            var Default = AppCommon.GetDefaultById(userData.UserKey);
            $.each(Default, function (key) {
                var existValue = $("[name=" + key + "]").val();
                if (existValue == "")
                    $("[name=" + key + "]").val(this.valueOf()).trigger('change');
            })
        }
    }
    var defaultBranchPopup = function () {
        var UserData = AppCommon.GetUserData();
        if (UserData) {
            var uid = UserData.UserKey;
            var userDefault = AppCommon.GetDefaultById(uid);
            var branch = userDefault != null ? userDefault["BranchKey"] : "";
            $.get(Resources.GetBranchesURL, function (response) {
                var span = $("<span/>").addClass("radio-largebox");
                $.each(response, function (i, Branch) {
                    $(span).append(
                        $('<input type="radio" id="branch' + i + '"  name="branch"></input>').val(Branch.RowKey).attr("checked", Branch.RowKey == branch ? true : false)).append(
                            $('<label for="branch' + i + '"></input>').html(Branch.Text));
                });
                $.confirm({
                    closeIcon: true,
                    title: Resources.SetDefaultBranch,
                    content: $(span)[0].outerHTML,
                    buttons: {
                        Default: {
                            text: Resources.SetAsDefault,
                            btnClass: 'btn-outline-primary',
                            action: function () {
                                var Id = $('[id*=branch]:checked').val();
                                AppCommon.SetDefaultById(uid, "BranchKey", Id);
                                window.location.reload();
                            }
                        }
                        ,
                        Reset: {
                            text: Resources.Reset,
                            btnClass: 'btn-warning',
                            action: function () {
                                AppCommon.SetDefaultById(uid, "BranchKey", "");
                                window.location.reload();
                            }
                        }
                        ,
                        Cancel: {
                            text: Resources.Cancel,
                            btnClass: 'btn-danger',
                            action: function () {

                            }
                        }
                    }

                });
            });
        }
    }
    var editPopupWithRebind = function (_this, rebindFun) {
        $.ajaxSetup({ cache: false });
        $(_this).popupform({
            load: function () {
                AppCommon.FormatInputCase();
            },
            rebind: function () {
                rebindFun;
            }
        });

    }

    var editGridPopup = function (_this, loadFun) {
        $.ajaxSetup({ cache: false });
        $(_this).popupform({
            load: function () {
                if (loadFun) {
                    setTimeout(function () {
                        loadFun.call()
                    }, 500)
                }
                else {
                    setTimeout(function () {
                        AppCommon.FormatInputCase();
                    }, 500)
                }

            },
            rebind: function () {
                $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
            }
        });

    }
    var drodownPopupWindow = function (_this, action, rebindFun) {
        $.ajaxSetup({ cache: false });
        $(_this).popupform({
            formAction: action,
            load: function () {
                AppCommon.FormatInputCase();
            },
            rebind: function () {
                rebindFun;
            }
        });
    }
    var paymentPopupWindow = function (_this, action, header) {
        $.ajaxSetup({ cache: false });
        $(_this).popupform({
            headerText: header,
            formAction: action,
            //submit: function () {

            //}
        });
    }
    var setInputAddOn = function (_this) {
        var parentDiv = $(_this).closest("div")[0];
        var width = $(_this).html().length * 5;
        $(parentDiv).find("input").css('text-indent', (width).toString() + 'px');
    }
    var generateRandomCode = function () {
        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        for (var i = 0; i < 5; i++)
            text += possible.charAt(Math.floor(Math.random() * possible.length));

        return text;
    }

    var amounToWords = function (amount) {
        //var options = {
        //    mainunit: "Rupees",
        //    subunit: "Paisa",
        //    units: ["", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten"],
        //    teens: ["Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen", "Twenty"],
        //    tens: ["", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"],
        //    othersIntl: ["Thousand", "Million", "Billion", "Trillion"]
        //}
        //var o = options;

        //var units = o.units;
        //var teens = o.teens;
        //var tens = o.tens;
        //var othersIntl = o.othersIntl;

        //var getBelowHundred = function (n) {
        //    if (n >= 100) {
        //        return "greater than or equal to 100";
        //    };
        //    if (n <= 10) {
        //        return units[n];
        //    };
        //    if (n <= 20) {
        //        return teens[n - 10 - 1];
        //    };
        //    var unit = Math.floor(n % 10);
        //    n /= 10;
        //    var ten = Math.floor(n % 10);
        //    var tenWord = (ten > 0 ? (tens[ten] + " ") : '');
        //    var unitWord = (unit > 0 ? units[unit] : '');
        //    return tenWord + unitWord;
        //};

        //var getBelowThousand = function (n) {
        //    if (n >= 1000) {
        //        return "greater than or equal to 1000";
        //    };
        //    var word = getBelowHundred(Math.floor(n % 100));

        //    n = Math.floor(n / 100);
        //    var hun = Math.floor(n % 10);
        //    word = (hun > 0 ? (units[hun] + " Hundred ") : '') + word;

        //    return word;
        //};
        //if (isNaN(n)) {
        //    return "Not a number";
        //};

        //var word = '';
        //var val;
        //var word2 = '';
        //var val2;
        //var b = n.split(".");
        //n = b[0];
        //d = b[1];
        //d = String(d);
        //d = d.substr(0, 2);

        //val = Math.floor(n % 1000);
        //n = Math.floor(n / 1000);

        //val2 = Math.floor(d % 1000);
        //d = Math.floor(d / 1000);

        //word = getBelowThousand(val);
        //word2 = getBelowThousand(val2);

        //othersArr = othersIntl;
        //divisor = 1000;
        //func = getBelowThousand;

        //var i = 0;
        //while (n > 0) {
        //    if (i == othersArr.length - 1) {
        //        word = this.numberToWords(n) + " " + othersArr[i] + " " + word;
        //        break;
        //    };
        //    val = Math.floor(n % divisor);
        //    n = Math.floor(n / divisor);
        //    if (val != 0) {
        //        word = func(val) + " " + othersArr[i] + " " + word;
        //    };
        //    i++;
        //};

        //var i = 0;
        //while (d > 0) {
        //    if (i == othersArr.length - 1) {
        //        word2 = this.numberToWords(d) + " " + othersArr[i] + " " + word2;
        //        break;
        //    };
        //    val2 = Math.floor(d % divisor);
        //    d = Math.floor(d / divisor);
        //    if (val2 != 0) {
        //        word2 = func(val2) + " " + othersArr[i] + " " + word2;
        //    };
        //    i++;
        //};
        //if (word != '') word = (word + ' ' + o.mainunit).toUpperCase()
        //if (word2 != '') word2 = (' AND ' + word2 + ' ' + o.subunit).toUpperCase();
        //return word + word2;

        var words = new Array();
        words[0] = '';
        words[1] = 'One';
        words[2] = 'Two';
        words[3] = 'Three';
        words[4] = 'Four';
        words[5] = 'Five';
        words[6] = 'Six';
        words[7] = 'Seven';
        words[8] = 'Eight';
        words[9] = 'Nine';
        words[10] = 'Ten';
        words[11] = 'Eleven';
        words[12] = 'Twelve';
        words[13] = 'Thirteen';
        words[14] = 'Fourteen';
        words[15] = 'Fifteen';
        words[16] = 'Sixteen';
        words[17] = 'Seventeen';
        words[18] = 'Eighteen';
        words[19] = 'Nineteen';
        words[20] = 'Twenty';
        words[30] = 'Thirty';
        words[40] = 'Forty';
        words[50] = 'Fifty';
        words[60] = 'Sixty';
        words[70] = 'Seventy';
        words[80] = 'Eighty';
        words[90] = 'Ninety';
        amount = amount.toString();
        var atemp = amount.split(".");
        var number = atemp[0].split(",").join("");
        var n_length = number.length;
        var words_string = "";
        if (n_length <= 9) {
            var n_array = new Array(0, 0, 0, 0, 0, 0, 0, 0, 0);
            var received_n_array = new Array();
            for (var i = 0; i < n_length; i++) {
                received_n_array[i] = number.substr(i, 1);
            }
            for (var i = 9 - n_length, j = 0; i < 9; i++, j++) {
                n_array[i] = received_n_array[j];
            }
            for (var i = 0, j = 1; i < 9; i++, j++) {
                if (i == 0 || i == 2 || i == 4 || i == 7) {
                    if (n_array[i] == 1) {
                        n_array[j] = 10 + parseInt(n_array[j]);
                        n_array[i] = 0;
                    }
                }
            }
            value = "";
            for (var i = 0; i < 9; i++) {
                if (i == 0 || i == 2 || i == 4 || i == 7) {
                    value = n_array[i] * 10;
                } else {
                    value = n_array[i];
                }
                if (value != 0) {
                    words_string += words[value] + " ";
                }
                if ((i == 1 && value != 0) || (i == 0 && value != 0 && n_array[i + 1] == 0)) {
                    words_string += "Crores ";
                }
                if ((i == 3 && value != 0) || (i == 2 && value != 0 && n_array[i + 1] == 0)) {
                    words_string += "Lakhs ";
                }
                if ((i == 5 && value != 0) || (i == 4 && value != 0 && n_array[i + 1] == 0)) {
                    words_string += "Thousand ";
                }
                if (i == 6 && value != 0 && (n_array[i + 1] != 0 && n_array[i + 2] != 0)) {
                    words_string += "Hundred and ";
                } else if (i == 6 && value != 0) {
                    words_string += "Hundred ";
                }
            }
            words_string = words_string.split("  ").join(" ");
        }
        return words_string;
    }

    var bindDropDownbyId = function (obj, url, dropdDownControl, labelName, ListName) {
        url = url + "?" + $.param(obj);
        var id = dropdDownControl.prop("id");

        $.ajax({
            type: "GET",
            url: url,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            beforeSend: function () {
                if (!$(dropdDownControl).hasClass("multiselect")) {
                    $(dropdDownControl).empty();
                    $(dropdDownControl).append($("<option loading></option>").val("").html("<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span>  Loading...</span>"));
                    $(dropdDownControl).selectpicker('refresh');
                }
            },
            success: function (response)
            {
                $(dropdDownControl).empty();
                response = ListName ? response[ListName] : response;

                if ($(dropdDownControl).is("select")) {
                    if (labelName)
                        $(dropdDownControl).append($("<option></option>").val("").html(Resources.Select + Resources.BlankSpace + labelName));
                    $.each(response, function () {
                        $(dropdDownControl).append($("<option></option>").val(this['RowKey']).html(this['Text']));
                    });
                }
                if (response.length == 1) {
                    $(dropdDownControl).val(response[0].RowKey);

                }
                if ($(dropdDownControl).hasClass("multiselect")) {
                    $(dropdDownControl).find("option:not([loading])").eq(0).remove();
                }


            },
            complete: function () {
                $(dropdDownControl).find("option[loading]").remove();
                $(dropdDownControl).selectpicker('refresh');
                $(dropdDownControl).trigger("change");
            }
        })
    }

    var bindDropDownbyObj = function (obj, url, dropdDownControl, labelName, type) {
        var response = AjaxHelper.ajax((type ? type : "GET"), url, obj)
        $(dropdDownControl).empty()
        $.each(response, function () {
            $(dropdDownControl).append($("<option></option>").val(this['RowKey']).html(this['Text']));

        });
        $(dropdDownControl).selectpicker('refresh');
    }

    var getAge = function (birthDate) {
        var today = new Date();
        var age = today.getFullYear() - birthDate.getFullYear();
        var m = today.getMonth() - birthDate.getMonth();
        if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
            age--;
        }
        return age;
    }

    var yearNumberToText = function (i) {
        i = i && i != "" ? parseInt(i) : 0;
        var j = i % 10,
            k = i % 100;
        if (j == 1 && k != 11) {
            return i + "st Year";
        }
        if (j == 2 && k != 12) {
            return i + "nd Year";
        }
        if (j == 3 && k != 13) {
            return i + "rd Year";
        }
        return i + "th Year";
    }
    var blobToFile = function (theBlob, fileName, fileType) {
        //A Blob() is almost a File() - it's just missing the two properties below which we will add
        theBlob.lastModifiedDate = new Date();
        theBlob = new File([theBlob], fileName, { type: fileType })
        return theBlob;
    }
    var dataURItoBlob = function (dataURI) {
        // convert base64 to raw binary data held in a string
        // doesn't handle URLEncoded DataURIs - see SO answer #6850276 for code that does this
        var byteString = atob(dataURI.split(',')[1]);

        // separate out the mime component
        var mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0];

        // write the bytes of the string to an ArrayBuffer
        var ab = new ArrayBuffer(byteString.length);
        var ia = new Uint8Array(ab);
        for (var i = 0; i < byteString.length; i++) {
            ia[i] = byteString.charCodeAt(i);
        }

        //Old Code
        //write the ArrayBuffer to a blob, and you're done
        //var bb = new BlobBuilder();
        //bb.append(ab);
        //return bb.getBlob(mimeString);

        //New Code
        return new Blob([ab], { type: mimeString });


    }

    var encodeQueryString = function (queryString) {
        $.ajaxSetup({ async: false });
        $.post(Resources.EncryptQueryStringURL, { query: queryString }, function (response) {
            queryString = response;
        });
        return queryString;
    }


    var autoHideDropdown = function () {
        $("select.autohide-select").each(function () {
            var name = $(this).attr("name");
            if (Resources[name] && Resources[name] != "") {
                var id = $(this).attr("id");
                var parent = $(this).parent()
                var hidden = $('<input>').attr('type', 'hidden').attr('id', id).attr('name', name).val(Resources[name]).appendTo($(parent));
                $(this).closest('*[class^="col"]').hide();
                //$(parent).find("span").remove();
                $(this).remove();
            }
            else if (this.childElementCount == 2) {
                if ($(this).val() == "") {
                    $(this).val(this.options[this.options.length - 1].value)
                    $(this).trigger("change");
                }
                $(this).closest('*[class^="col"]').hide();
            }
        });

        $("select.autoselect-select").each(function () {
            if (this.childElementCount == 2) {
                if ($(this).val() == "") {
                    $(this).val(this.options[this.options.length - 1].value).trigger("change");
                }
                //$(this).closest('*[class^="col"]').hide();
            }
        });
    }
    var jsonToExcel = function (data, Columns) {

        $(document).ready(function () {
            if (Columns == null) {
                $("body").excelexportjs({

                    datatype: 'json'
                    , dataset: data
                    , columns: getColumns(data)
                });
            }
            else {
                $("body").excelexportjs({

                    datatype: 'json'
                    , dataset: data
                    , columns: getColumnsArray(Columns)
                });
            }

        });
    }
    var jsonToPrint = function (data, Columns) {

        $(document).ready(function () {
            if (Columns == null) {
                $("body").excelexportjs({

                    datatype: 'printtable'
                    , dataset: data
                    , columns: getColumns(data)
                });
            }
            else {
                $("body").excelexportjs({

                    datatype: 'printtable'
                    , dataset: data
                    , columns: getColumnsArray(Columns)
                });
            }

        });

    }

    //added on 22 Jan 2019
    var parseMMMYYYYDate = function (input) {
        var map = {
            jan: 1, feb: 2, mar: 3, apr: 4, may: 5, jun: 6,
            jul: 7, aug: 8, sep: 9, oct: 10, nov: 11, dec: 12
        };
        input = input.split(" ");
        return new Date(input[1], (map[input[0].toLowerCase()] || input[0]) - 1, 01);
    }
    //added on 06 Feb 2019 for Employee Attendance
    var getDayNameFromDate = function (date, dayLen) {
        var days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
        if (dayLen) {
            return days[date.getDay()].substring(0, dayLen);
        }
        return days[date.getDay()];
    }
    var javascriptDateToServerDate = function (date) {
        var dd = date.getDate();
        var mm = date.getMonth() + 1; //January is 0!
        var yyyy = date.getFullYear();

        if (dd < 10) {
            dd = '0' + dd
        }

        if (mm < 10) {
            mm = '0' + mm
        }

        return dd + '/' + mm + '/' + yyyy;
    }

    var exportTableToExcel = function (obj) {

        if ($(obj.ContainerId)[0]) {
            //this will remove the blank-spaces from the title and replace it with an underscore
            obj.FileName = obj.FileName ? obj.FileName.replace(/ /g, '') : "";
            var uri = $("").excelexportjs({
                containerid: obj.ContainerId
                , datatype: 'table'
                , returnUri: true
                , worksheetName: obj.FileName
                , headerbg: "yellow"
                , title: obj.Title
                , subtitle: obj.SubTitle
            });
            var link = document.createElement("a");
            link.href = uri;

            //set the visibility hidden so it will not effect on your web-layout
            link.style = "visibility:hidden";
            link.download = obj.FileName + ".xls";

            //this part will append the anchor tag and remove it after automatic click
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }

    }
    var exportJSONToExcel = function (obj) {
        if (obj.ColumnNames.length > 0 && obj.JSONData.length > 0) {
            //this will remove the blank-spaces from the title and replace it with an underscore
            obj.FileName = obj.FileName ? obj.FileName.replace(/ /g, '') : "";
            var uri = $("").excelexportjs({
                datatype: 'json'
                , returnUri: true
                , dataset: obj.JSONData
                , columns: obj.ColumnNames
                , worksheetName: obj.FileName
                , headerbg: "yellow"
                , reporttitle: obj.Title
                , subtitle: obj.SubTitle
            });
            var link = document.createElement("a");
            link.href = uri;

            //set the visibility hidden so it will not effect on your web-layout
            link.style = "visibility:hidden";
            link.download = obj.FileName + ".xls";

            //this part will append the anchor tag and remove it after automatic click
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }

    }


    var exportPrintJqGrid = function (obj, type) {
        var $grid = $(obj.ContainerId);
        //obj.JSONData = $.extend([], true, $grid.jqGrid('getRowData'));
        obj.JSONData = $.extend([], true, $grid.jqGrid('getGridParam', 'data'));
        var columnModel = $.extend([], true, $grid.jqGrid('getGridParam', 'colModel'));
        var columnNames = $.extend([], true, $grid.jqGrid('getGridParam', 'colNames'));
        var remove = 0;
        var removeCols = ["subgrid", "edit"]
        for (var i = columnModel.length - 1; i >= 0; i--) {
            if ((columnModel[i].hidden || removeCols.indexOf(columnModel[i].name) > -1)) {
                columnModel.splice(i, 1);
            }
            else {
                columnModel[i]["headertext"] = columnNames[i];
            }
        }
        //columnModel.splice(columnModel.length - 1, 1);
        //var str = JSON.stringify(columnModel);
        //str = str.replace(/\"name\":/g, "\"datafield\":");
        obj.ColumnNames = columnModel;
        if (type == "P")
            AppCommon.PrintJSON(obj);
        else if (type == "E")
            AppCommon.ExportJSONToExcel(obj);
        else
            AppCommon.ExportJSONToPDF(obj);
    }
    var exportPrintAjax = function (obj, type) {
        var $grid = $(obj.ContainerId);
        $(".section-content").mLoading();
        $.ajax({
            type: obj.ajaxType,
            url: obj.ajaxUrl,
            dataType: "json",
            async: true,
            data: obj.ajaxData,
            success: function (response) {
                if (obj.beforeProcessing) {
                    //obj.beforeProcessing.call(response);

                    response.rows = $(response.rows).map(function (n, item) {
                        var obj = {};
                        $(item).each(function () {
                            obj[this.Key] = this.Value;
                        });
                        return obj;
                    });
                }
                obj.JSONData = response.rows;
                var columnModel = $.extend([], true, $grid.jqGrid('getGridParam', 'colModel'));
                var columnNames = $.extend([], true, $grid.jqGrid('getGridParam', 'colNames'));
                var remove = 0;
                var removeCols = ["subgrid", "edit", "cb"]
                for (var i = columnModel.length - 1; i >= 0; i--) {
                    if ((columnModel[i].hidden || removeCols.indexOf(columnModel[i].name) > -1)) {
                        columnModel.splice(i, 1);
                    }
                    else {
                        columnModel[i]["headertext"] = columnNames[i];
                    }

                }
                //columnModel.splice(columnModel.length - 1, 1);
                //var str = JSON.stringify(columnModel);
                //str = str.replace(/\"name\":/g, "\"datafield\":");
                obj.ColumnNames = columnModel;
                if (type == "P")
                    AppCommon.PrintJSON(obj);
                else if (type == "E")
                    AppCommon.ExportJSONToExcel(obj);
                else
                    AppCommon.ExportJSONToPDF(obj);
                $(".section-content").mLoading("destroy");
            }
        });

    }


    var exportHtmlToPDF = function (obj) {
        if ($(obj.HtmlCntrl).html().trim() != "") {
            var cntrl = $(obj.HtmlCntrl).clone();
            var styles = document.styleSheets;
            var res = $.map(styles, function (v, index) {
                return v.href;
            });
            if (obj.SubTitle) {
                $(cntrl).prepend("<h5 style='text-align:center'>" + obj.SubTitle + "<h5>");
            }
            if (obj.Title) {
                $(cntrl).prepend("<h3 style='text-align:center'>" + obj.Title + "<h3>");
            }
            var req = new XMLHttpRequest();
            req.open("POST", Resources.ExportToPDFUrl, true);
            var param = JSON.stringify({ Html: $(cntrl).html(), fileName: obj.FileName, cssList: res });
            req.setRequestHeader('Content-Type', 'application/json')
            req.responseType = "blob";
            req.onload = function (event) {
                var blob = req.response;
                var fileName = req.getResponseHeader("fileName") //if you have the fileName header available
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = obj.FileName + ".pdf";
                link.click();
            };

            req.send(param);
        }
    }

    var exportJSONToPDF = function (obj) {
        if (obj.JSONData.length > 0 && obj.ColumnNames.length > 0) {
            var html = $("").excelexportjs({
                datatype: 'json'
                , returnHtml: true
                , dataset: obj.JSONData
                , columns: obj.ColumnNames
                , worksheetName: obj.Title.replace(/ /g, '')
                , headerbg: "yellow"
                , title: obj.Title
                , subtitle: obj.SubTitle
            });
            var styles = document.styleSheets;
            var res = $.map(styles, function (v, index) {
                return v.href;
            });
            if (obj.SubTitle) {
                $(html).before("<h5 style='text-align:center'>" + obj.SubTitle + "<h5>");
            }
            if (obj.Title) {
                $(html).before("<h3 style='text-align:center'>" + obj.Title + "<h3>");
            }
            var req = new XMLHttpRequest();
            req.open("POST", Resources.ExportToPDFUrl, true);
            var param = JSON.stringify({ Html: html, fileName: obj.FileName, cssList: res });
            req.setRequestHeader('Content-Type', 'application/json')
            req.responseType = "blob";
            req.onload = function (event) {
                var blob = req.response;
                var fileName = req.getResponseHeader("fileName") //if you have the fileName header available
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = obj.FileName + ".pdf";
                link.click();
            };

            req.send(param);
        }
    }

    var printHtml = function (obj) {

        if ($(obj.HtmlCntrl).html().trim() != "") {
            var mode = 'iframe'; //popup
            //var close = mode == "popup";
            var styles = document.styleSheets;
            var res = $.map(styles, function (v, index) {
                return v.href;
            });
            var options = { mode: mode, popClose: close, extraCss: res.join(","), printTitle: obj.Title, printSubTitle: obj.SubTitle, extraHead: obj.ExtraHead };
            $(obj.HtmlCntrl).printArea(options);
        }
    }
    var printJSON = function (obj) {
        if (obj.JSONData.length > 0 && obj.ColumnNames.length > 0) {
            var html = $("").excelexportjs({
                datatype: 'json'
                , returnHtml: true
                , dataset: obj.JSONData
                , columns: obj.ColumnNames
                , worksheetName: obj.Title.replace(/ /g, '')
                , headerbg: "yellow"
                , title: obj.Title
                , subtitle: obj.SubTitle
            });
            var html = html +
                '<style type="text/css">' +
                'table {'
                + 'border-collapse: collapse;'
                + '}' +
                'table, th, td {'
                + 'border: 1px solid black;padding:3px'
                + '}' +
                '</style>';
            $("").printArea({ html: html })
        }
    }
    var enterKeyToTab = function (_this) {
        $('input,select,textarea', $(_this)).on("keypress", function (e) {
            /* ENTER PRESSED*/
            var inputs = $("input:not([readonly]):not([disabled]):not(:hidden):not([data-exclude]),select:not(readonly):not([disabled]):not([data-hidden]):not([data-exclude]),textarea:not(readonly):not([disabled]):not(:hidden):not([data-exclude])", _this);

            if (e.shiftKey && e.keyCode == 13) {
                /* FOCUS ELEMENT */
                var idx = inputs.index(this);
                if (inputs[idx + 1])
                    inputs[idx - 1].focus(); //  handles submit buttons
                inputs[idx - 1].select();

                return false;
            }
            else if (e.keyCode == 13) {
                /* FOCUS ELEMENT */
                var idx = inputs.index(this);
                if (inputs[idx + 1])
                    inputs[idx + 1].focus(); //  handles submit buttons

                return false;
            }

        });
    }
    var roundTo = function (n, digits) {
        //var negative = false;
        //if (digits === undefined) {
        //    digits = 0;
        //}
        //if (n < 0) {
        //    negative = true;
        //    n = n * -1;
        //}
        //var multiplicator = Math.pow(10, digits);
        //n = parseFloat((n * multiplicator).toFixed(11));
        //n = (Math.round(n) / multiplicator).toFixed(2);
        //if (negative) {
        //    n = (n * -1).toFixed(2);
        //}
        n = n * 1;
        return parseFloat(n.toFixed(digits)).toString();
    }
    var xml2Json = function (xml, tab) {

        tab = tab ? tab : "";
        var X = {
            toObj: function (xml) {
                var o = {};
                if (xml.nodeType == 1) {   // element node ..
                    if (xml.attributes.length)   // element with attributes  ..
                        for (var i = 0; i < xml.attributes.length; i++)
                            o["@" + xml.attributes[i].nodeName] = (xml.attributes[i].nodeValue || "").toString();
                    if (xml.firstChild) { // element has child nodes ..
                        var textChild = 0, cdataChild = 0, hasElementChild = false;
                        for (var n = xml.firstChild; n; n = n.nextSibling) {
                            if (n.nodeType == 1) hasElementChild = true;
                            else if (n.nodeType == 3 && n.nodeValue.match(/[^ \f\n\r\t\v]/)) textChild++; // non-whitespace text
                            else if (n.nodeType == 4) cdataChild++; // cdata section node
                        }
                        if (hasElementChild) {
                            if (textChild < 2 && cdataChild < 2) { // structured element with evtl. a single text or/and cdata node ..
                                X.removeWhite(xml);
                                var regex = new RegExp(/\_(.*?)\_/);
                                for (var n = xml.firstChild; n; n = n.nextSibling) {
                                    var nodeName = n.nodeName.replace(/_/g, "");
                                    if (n.nodeType == 3)  // text node
                                        o["#text"] = X.escape(n.nodeValue);
                                    else if (n.nodeType == 4)  // cdata node
                                        o["#cdata"] = X.escape(n.nodeValue);
                                    else if (o[nodeName] || regex.test(n.nodeName)) {  // multiple occurence of element ..

                                        if (o[nodeName] instanceof Array) {
                                            var value = X.toObj(n);
                                            if (value)
                                                o[nodeName][o[nodeName].length] = value;
                                        }
                                        else
                                            o[nodeName] = [o[nodeName], X.toObj(n)].filter(function (el) {
                                                return el != null;
                                            });
                                    }
                                    else  // first occurence of element..
                                    {
                                        var value = X.toObj(n);
                                        o[nodeName] = value;
                                    }
                                }
                            }
                            else { // mixed content
                                if (!xml.attributes.length)
                                    o = X.escape(X.innerXml(xml));
                                else
                                    o["#text"] = X.escape(X.innerXml(xml));
                            }
                        }
                        else if (textChild) { // pure text
                            if (!xml.attributes.length)
                                o = X.escape(X.innerXml(xml));
                            else
                                o["#text"] = X.escape(X.innerXml(xml));
                        }
                        else if (cdataChild) { // cdata
                            if (cdataChild > 1)
                                o = X.escape(X.innerXml(xml));
                            else
                                for (var n = xml.firstChild; n; n = n.nextSibling)
                                    o["#cdata"] = X.escape(n.nodeValue);
                        }
                    }
                    if (!xml.attributes.length && !xml.firstChild) o = null;
                }
                else if (xml.nodeType == 9) { // document.node
                    o = X.toObj(xml.documentElement);
                }
                else
                    alert("unhandled node type: " + xml.nodeType);
                return o;
            },
            toJson: function (o, name, ind) {
                var json = name ? ("\"" + name + "\"") : "";
                if (o instanceof Array) {
                    for (var i = 0, n = o.length; i < n; i++)
                        o[i] = X.toJson(o[i], "", ind + "\t");
                    json += (name ? ":[" : "[") + (o.length > 1 ? ("\n" + ind + "\t" + o.join(",\n" + ind + "\t") + "\n" + ind) : o.join("")) + "]";
                }
                else if (o == null)
                    json += (name && ":") + "null";
                else if (typeof (o) == "object") {
                    var arr = [];
                    for (var m in o)
                        arr[arr.length] = X.toJson(o[m], m, ind + "\t");
                    json += (name ? ":{" : "{") + (arr.length > 1 ? ("\n" + ind + "\t" + arr.join(",\n" + ind + "\t") + "\n" + ind) : arr.join("")) + "}";
                }
                else if (typeof (o) == "string")
                    json += (name && ":") + "\"" + o.toString() + "\"";
                else
                    json += (name && ":") + o.toString();
                return json;
            },
            innerXml: function (node) {
                var s = ""
                if ("innerHTML" in node)
                    s = node.innerHTML;
                else {
                    var asXml = function (n) {
                        var s = "";
                        if (n.nodeType == 1) {
                            s += "<" + n.nodeName;
                            for (var i = 0; i < n.attributes.length; i++)
                                s += " " + n.attributes[i].nodeName + "=\"" + (n.attributes[i].nodeValue || "").toString() + "\"";
                            if (n.firstChild) {
                                s += ">";
                                for (var c = n.firstChild; c; c = c.nextSibling)
                                    s += asXml(c);
                                s += "</" + n.nodeName + ">";
                            }
                            else
                                s += "/>";
                        }
                        else if (n.nodeType == 3)
                            s += n.nodeValue;
                        else if (n.nodeType == 4)
                            s += "<![CDATA[" + n.nodeValue + "]]>";
                        return s;
                    };
                    for (var c = node.firstChild; c; c = c.nextSibling)
                        s += asXml(c);
                }
                return s;
            },
            escape: function (txt) {
                return txt.replace(/[\\]/g, "\\\\")
                    .replace(/[\"]/g, '\\"')
                    .replace(/[\n]/g, '\\n')
                    .replace(/[\r]/g, '\\r');
            },
            removeWhite: function (e) {
                e.normalize();
                for (var n = e.firstChild; n;) {
                    if (n.nodeType == 3) {  // text node
                        if (!n.nodeValue.match(/[^ \f\n\r\t\v]/)) { // pure whitespace text node
                            var nxt = n.nextSibling;
                            e.removeChild(n);
                            n = nxt;
                        }
                        else
                            n = n.nextSibling;
                    }
                    else if (n.nodeType == 1) {  // element node
                        X.removeWhite(n);
                        n = n.nextSibling;
                    }
                    else                      // any other node
                        n = n.nextSibling;
                }
                return e;
            }
        };
        if (xml && xml.nodeType == 9) // document node
            xml = xml.documentElement;
        var json = X.toJson(X.toObj(X.removeWhite(xml)), xml.nodeName, "\t");
        return JSON.parse("{\n" + tab + (tab ? json.replace(/\t/g, tab) : json.replace(/\t|\n/g, "")) + "\n}");
    }
    var handleBarHelpers = function () {
        Handlebars.registerHelper('ifCond', function (v1, operator, v2, options) {

            switch (operator) {
                case '==':
                    return (v1 == v2) ? options.fn(this) : options.inverse(this);
                case '===':
                    return (v1 === v2) ? options.fn(this) : options.inverse(this);
                case '!=':
                    return (v1 != v2) ? options.fn(this) : options.inverse(this);
                case '!==':
                    return (v1 !== v2) ? options.fn(this) : options.inverse(this);
                case '<':
                    return (v1 < v2) ? options.fn(this) : options.inverse(this);
                case '<=':
                    return (v1 <= v2) ? options.fn(this) : options.inverse(this);
                case '>':
                    return (v1 > v2) ? options.fn(this) : options.inverse(this);
                case '>=':
                    return (v1 >= v2) ? options.fn(this) : options.inverse(this);
                case '&&':
                    return (v1 && v2) ? options.fn(this) : options.inverse(this);
                case '||':
                    return (v1 || v2) ? options.fn(this) : options.inverse(this);
                default:
                    return options.inverse(this);
            }
        });
        Handlebars.registerHelper('math', function (lvalue, operator, rvalue) {
            lvalue = parseFloat(lvalue) ? parseFloat(lvalue) : 0;
            rvalue = parseFloat(rvalue) ? parseFloat(rvalue) : 0;
            return {
                "+": lvalue + rvalue,
                "-": lvalue - rvalue,
                "*": lvalue * rvalue,
                "/": lvalue / rvalue,
                "%": lvalue % rvalue
            }[operator];
        });
        Handlebars.registerHelper('G29', function (value) {
            return (value && parseFloat(value) != undefined ? parseFloat(parseFloat(value).toFixed(Resources.RoundToDecimalPostion)) : value)
        });
        Handlebars.registerHelper('sum', function (item, key) {
            var total = item.reduce(function (sum, item) {
                return sum + (parseFloat(item[key]) ? parseFloat(item[key]) : 0);
            }, 0);
            return AppCommon.FormatCurrency((total ? parseFloat(parseFloat(total).toFixed(Resources.RoundToDecimalPostion)).toString() : total))
        });
        Handlebars.registerHelper('dateformat', function (item, format) {

            return moment(item).format(format);
        });
        Handlebars.registerHelper('breaklines', function (text) {
            text = Handlebars.Utils.escapeExpression(text);
            text = text.replace(/(\r\n|\n|\r)/gm, '<br>');
            return new Handlebars.SafeString(text);
        });
        Handlebars.registerHelper('url', function (url) {
            return Resources.FullPath + "/" + url;
        });
        Handlebars.registerHelper('yesno', function (value) {
            return value ? "Yes" : "No";
        });
        Handlebars.registerHelper('currency', function (value) {

            return AppCommon.FormatCurrency(value);
        });
        Handlebars.registerHelper('fromnow', function (value) {
            value = moment(value);
            return value.fromNow();
        });

    }
    var getYearDescriptionByCodeDetails = function (Duration, Year, AcademicTermKey) {
        Duration = AcademicTermKey == Resources.AcademicTermSemester ? Duration / 6 : Duration / 12;
        var YearText = AcademicTermKey == Resources.AcademicTermSemester ? " Semester" : " Year";
        var result;
        switch (parseInt(Year)) {
            case 1:
                if (Duration < 1) {
                    result = "Short Term";
                    break;
                }
                else {
                    result = Year + "st " + YearText;
                    break;
                }
            case 2:
                result = Year + "nd " + YearText;
                break;
            case 3:
                result = Year + "rd " + YearText;
                break;
            default:
                result = Year + "th " + YearText;
                break;


        }
        return result;
    }
    var setColorByBackgroundIntensity = function (color) {
        var r = 0, g = 0, b = 0, a = 1, yiq = 0;
        if (/rgba/.test(color)) {
            color = color.replace('rgba(', '').replace(')', '').split(/,/);
            r = color[0];
            g = color[1];
            b = color[2];
            a = color[3];
        } else if (/rgb/.test(color)) {
            color = color.replace('rgb(', '').replace(')', '').split(/,/);
            r = color[0];
            g = color[1];
            b = color[2];
        } else if (/#/.test(color)) {
            color = color.replace('#', '');
            if (color.length == 3) {
                var _t = '';
                _t += color[0] + color[0];
                _t += color[1] + color[1];
                _t += color[2] + color[2];
                color = _t;
            }
            r = parseInt(color.substr(0, 2), 16);
            g = parseInt(color.substr(2, 2), 16);
            b = parseInt(color.substr(4, 2), 16);
        }
        yiq = ((r * 299) + (g * 587) + (b * 114)) / 1000;
        return (yiq >= 128) ? 'black' : 'white';
    }
    var setScrollTableWidth = function (div) {
        var width = $(div).find("th[data-fixed]").toArray().reduce(function (sum, item) {
            return sum = sum + item.offsetWidth
        }, 0);

        $(div).find(".inner").css("margin-left", width);
        $(div).find("th[data-fixed]").each(function () {
            var index = $(this).index();
        })
    }

    var easyNotify = function (options) {

        var settings = $.extend({
            title: "Notification",
            options: {
                body: "",
                icon: "",
                lang: 'pt-BR',
                onClose: "",
                onClick: "",
                onError: ""
            }
        }, options);

        this.init = function () {
            var notify = this;
            if (!("Notification" in window)) {
                alert("This browser does not support desktop notification");
            } else if (Notification.permission === "granted") {

                var notification = new Notification(settings.title, settings.options);

                notification.onclose = function () {
                    if (typeof settings.options.onClose == 'function') {
                        settings.options.onClose();
                    }
                };

                notification.onclick = function () {
                    if (typeof settings.options.onClick == 'function') {
                        settings.options.onClick();
                    }
                };

                notification.onerror = function () {
                    if (typeof settings.options.onError == 'function') {
                        settings.options.onError();
                    }
                };

            } else if (Notification.permission !== 'granted') {
                Notification.requestPermission(function (permission) {
                    if (permission === "granted") {
                        notify.init();
                    }

                });
            }

        };

        this.init();
        return this;
    };
    var formatCurrency = function (n) {
        if (!parseFloat(n))
            return (n ? n : 0)
        var x = n;
        x = x.toString();
        var afterPoint = '';
        if (x.indexOf('.') > 0)
            afterPoint = x.substring(x.indexOf('.'), x.length);
        x = Math.floor(x);
        x = x.toString();
        var lastThree = x.substring(x.length - 3);
        var otherNumbers = x.substring(0, x.length - 3);
        if (otherNumbers != '')
            lastThree = ',' + lastThree;
        var res = otherNumbers.replace(/\B(?=(\d{2})+(?!\d))/g, ",") + lastThree + afterPoint;

        return res;

    }
    var getPaperSizeFromName = function (name) {
        var arr = [
            { name: "4A0", width: "1682", height: "2338", margin: "20", unit: "mm" },
            { name: "2A0", width: "1189", height: "1642", margin: "20", unit: "mm" },
            { name: "A0", width: "841", height: "1149", margin: "20", unit: "mm" },
            { name: "A1", width: "594", height: "801", margin: "20", unit: "mm" },
            { name: "A2", width: "420", height: "554", margin: "20", unit: "mm" },
            { name: "A3", width: "297", height: "380", margin: "20", unit: "mm" },
            { name: "A4", width: "210", height: "257", margin: "20", unit: "mm" },
            { name: "A5", width: "148", height: "170", margin: "20", unit: "mm" },
            { name: "A6", width: "105", height: "108", margin: "20", unit: "mm" },
            { name: "A7", width: "74", height: "65", margin: "20", unit: "mm" },
            { name: "A8", width: "52", height: "34", margin: "20", unit: "mm" },
            { name: "A9", width: "37", height: "12", margin: "20", unit: "mm" },
            { name: "A10", width: "26", height: "37", margin: "20", unit: "mm" },]
        var obj = arr.filter(function (n, p) {
            return n.name === name
        })[0]
        return obj;
    }

    function calculateRowAmount(_this, isSecond) {
        var formulaString = $("input[name*=Formula]", _this).val();
        var IsFixed = $(_this).data("fixed")
        IsFixed = IsFixed ? JSON.parse(IsFixed.toLowerCase()) : true;
        if (formulaString) {
            var formulaArray = JSON.parse(formulaString) ? JSON.parse(formulaString) : null;
            var formula = formulaArray.join("");
            var res = []
            var result = 0;
            if (formula) {
                formula = formula.replace(new RegExp("%", 'gi'), "/100");
                var obj = getAllCodesAndValue(isSecond);
                for (var key in obj) {
                    if (!isSecond && formulaArray.indexOf(key) > -1)
                        AppCommon.CalculateRowAmount($("[data-code=" + key + "]"), true)

                    var value = $("[name*=Amount],[fixedsalary]", $("[data-code=" + key + "]")).val();
                    if (!IsFixed) {
                        value = $("[name*=Amount],[dynamicsalary]", $("[data-code=" + key + "]")).val();
                    }
                    formula = formula.replace(new RegExp(key, 'gi'), value);

                }

                try {
                    var IsApplicable = true;
                    if (!IsFixed) {
                        var applicableFormulaString = $("input[name*=Applicable]", _this).val();
                        if (applicableFormulaString) {
                            formulaArray = JSON.parse(applicableFormulaString) ? JSON.parse(applicableFormulaString) : null;
                            applicableFormula = formulaArray.join("");
                            applicableFormula = applicableFormula.replace(new RegExp("%", 'gi'), "/100");
                            for (var key in obj) {
                                if (!isSecond && formulaArray.indexOf(key) > -1)
                                    AppCommon.CalculateRowAmount($("[data-code=" + key + "]"), true)

                                var value = $("[name*=Amount],[fixedsalary]", $("[data-code=" + key + "]")).val();

                                applicableFormula = applicableFormula.replace(new RegExp(key, 'gi'), value);

                            }
                            IsApplicable = eval(applicableFormula);
                        }
                    }
                    if (IsApplicable)
                        result = eval(formula);


                }
                catch (e) {
                    result = 0;
                }

            }
            $("input[name*=Amount]", _this).val(AppCommon.RoundTo(result, 0).toString());
            if (!isSecond)
                checkcolumnReferences($(_this).data("code"));
        }
    }


    var integerToWord = function (num) {

        var a = ['', 'One ', 'Two ', 'Three ', 'Four ', 'Five ', 'Six ', 'Seven ', 'Eight ', 'Nine ', 'Ten ', 'Eleven ', 'Twelve ', 'Thirteen ', 'Fourteen ', 'Fifteen ', 'Sixteen ', 'Seventeen ', 'Eighteen ', 'Nineteen '];
        var b = ['', '', 'Twenty', 'Thirty', 'Forty', 'Fifty', 'Sixty', 'Seventy', 'Eighty', 'Ninety'];


        if ((num = num.toString()).length > 9) return 'overflow';
        n = ('000000000' + num).substr(-9).match(/^(\d{2})(\d{2})(\d{2})(\d{1})(\d{2})$/);
        if (!n) return; var str = '';
        str += (n[1] != 0) ? (a[Number(n[1])] || b[n[1][0]] + ' ' + a[n[1][1]]) + 'crore ' : '';
        str += (n[2] != 0) ? (a[Number(n[2])] || b[n[2][0]] + ' ' + a[n[2][1]]) + 'lakh ' : '';
        str += (n[3] != 0) ? (a[Number(n[3])] || b[n[3][0]] + ' ' + a[n[3][1]]) + 'thousand ' : '';
        str += (n[4] != 0) ? (a[Number(n[4])] || b[n[4][0]] + ' ' + a[n[4][1]]) + 'hundred ' : '';
        str += (n[5] != 0) ? ((str != '') ? '' : '') + (a[Number(n[5])] || b[n[5][0]] + ' ' + a[n[5][1]]) : '';
        return str;

    }

    var dOBToWords = function (DOB) {
        var wDays = ['', 'First', 'Second', 'Third', 'Fourth', 'Fifth', 'Sixth', 'Seventh', 'Eighth', 'Ninth', 'Tenth', 'Eleventh', 'Twelfth', 'Thirteenth', 'Fourteenth', 'Fifteenth', 'Sixteenth', 'Seventeenth', 'Eighteenth', 'Nineteenth', 'Twentieth', 'Twenty-First', 'Twenty-Second', 'Twenty-Third', 'Twenty-Fourth', 'Twenty-Fifth', 'Twenty-Sixth', 'Twenty-Seventh', 'Twenty-Eighth', 'Twenty-Ninth', 'Thirtieth', 'Thirty-First']

        var wMonths = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December']

        var date = DOB;
        var day = parseInt(date.getDate());
        var month = parseInt(date.getMonth());
        var year = date.getUTCFullYear().toString();
        var yeardgt = date.getUTCFullYear();

        var yearword;
        if (yeardgt >= 2020 || year <= 1999) {
            var Fltr = year.substring(2, 0);
            var Fltrword = AppCommon.IntegerToWord(Fltr);

            var Sltr = year.substring(2, 4);
            var Sltrrword = AppCommon.IntegerToWord(Sltr);

            yearword = Fltrword + Sltrrword;
        }
        else {
            var yearword = AppCommon.IntegerToWord(year);
        }

        var DOBWords = wDays[day] + ' ' + wMonths[month] + ' ' + yearword;
        return DOBWords;

    }
    var IsDataIsEmpty = function (data) {
        if (data == null || data == undefined || data == 0) {
            return true;
        }
        else if (String(data).trim() == "" || String(data).trim() == "0") {
            return true;
        }
        else {
            return false;
        }
    }
    var TestRegx = function (regexString, inputString) {
        if (AppCommon.IsDataIsEmpty(inputString) == false) {
            const regex = new RegExp(regexString);
            if (!regex.test(inputString)) {
                return false;
            }
        }
        return true;
    }
    var TestRegxById = function (element, inputString) {
        if (AppCommon.IsDataIsEmpty(inputString) == false) {
            const regex = new RegExp(element.attr("data-val-regex-pattern"));
            if (!regex.test(inputString)) {
                return false;
            }
        }
        return true;
    }

    return {
        FormatString: formatString,
        toString: toString,
        emptyGuid: emptyGuid,
        IsValidStringInput: isValidStringInput,
        AddMonths: addMonths,
        AddHours: addHours,
        GetQueryStringParams: getQueryStringParams,
        FormatTimeAMPM: formatTimeAMPM,
        FormatObjectToTimeAMPM: formatObjectToTimeAMPM,
        TimeFromJavascriptDate: timeFromJavascriptDate,
        ValidateGuid: validateGuid,
        FormatDateInput: formatDateInput,
        FormatTimeInput: formatTimeInput,
        FormatSelect2: formatSelect2,
        FormatMultiSelect: formatMultiSelect,
        JsonDateToNormalDate: jsonDateToNormalDate,
        FormatYearInput: formatYearInput,
        FormatMonthInput: formatMonthInput,
        FormatDayInput: formatDayInput,
        FormatInputCase: formatInputCase,
        CustomRepeaterRemoteMethod: customRepeaterRemoteMethod,
        CustomRemoteMethod: customRemoteMethod,
        GetAccountBalanceById: getAccountBalanceById,
        ObjectifyForm: objectifyForm,
        FormatAMPMTo24Hrs: formatAMPMTo24Hrs,
        TimeAMPMFromJavascriptDate: timeAMPMFromJavascriptDate,
        DigitalClock: digitalClock,
        SetPhoneInputAddOn: setPhoneInputAddOn,
        GetDefaultById: getDefaultById,
        SetDefaultById: setDefaultById,
        GetUserData: getUserData,
        SetDefaultToControls: setDefaultToControls,
        DefaultBranchPopup: defaultBranchPopup,
        EditPopupWithRebind: editPopupWithRebind,
        EditGridPopup: editGridPopup,
        PaymentPopupWindow: paymentPopupWindow,
        DrodownPopupWindow: drodownPopupWindow,
        SetInputAddOn: setInputAddOn,
        GenerateRandomCode: generateRandomCode,
        AmounToWords: amounToWords,
        BindDropDownbyId: bindDropDownbyId,
        JsonDateToServerDate: jsonDateToServerDate,
        GetAge: getAge,
        JsonDateToNormalDate2: jsonDateToNormalDate2,
        YearNumberToText: yearNumberToText,
        BlobToFile: blobToFile,
        DataURItoBlob: dataURItoBlob,
        EncodeQueryString: encodeQueryString,
        AutoHideDropdown: autoHideDropdown,
        BindDropDownbyObj: bindDropDownbyObj,
        JsonToExcel: jsonToExcel,
        JsonToPrint: jsonToPrint,
        ParseMMMYYYYDate: parseMMMYYYYDate,
        GetDayNameFromDate: getDayNameFromDate,
        JavascriptDateToServerDate: javascriptDateToServerDate,
        FormatDurationInput: formatDurationInput,
        ExportTableToExcel: exportTableToExcel,
        ExportJSONToExcel: exportJSONToExcel,
        ExportHtmlToPDF: exportHtmlToPDF,
        ExportJSONToPDF: exportJSONToPDF,
        PrintHtml: printHtml,
        PrintJSON: printJSON,
        ExportPrintJqGrid: exportPrintJqGrid,
        ExportPrintAjax: exportPrintAjax,
        EnterKeyToTab: enterKeyToTab,
        RoundTo: roundTo,
        Xml2Json: xml2Json,
        HandleBarHelpers: handleBarHelpers,
        GetYearDescriptionByCodeDetails: getYearDescriptionByCodeDetails,
        SetColorByBackgroundIntensity: setColorByBackgroundIntensity,
        SetScrollTableWidth: setScrollTableWidth,
        EasyNotify: easyNotify,
        FormatAutoComplete: formatAutoComplete,
        FormatCurrency: formatCurrency,
        GetPaperSizeFromName: getPaperSizeFromName,
        CalculateRowAmount: calculateRowAmount,
        DOBToWords: dOBToWords,
        IntegerToWord: integerToWord,
        IsDataIsEmpty: IsDataIsEmpty,
        TestRegx: TestRegx,
        TestRegxById: TestRegxById
    };

}());

function checkTime(i) {
    if (i < 10) { i = "0" + i };  // add zero in front of numbers < 10
    return i;
}




function getAllCodesAndValue() {
    var obj = {};
    $("[data-code]").each(function () {
        var key = $(this).data("code");
        if (key)
            obj[key] = $("[name*=Amount],[monthlysalary]", this).val();
    });
    return obj;
}

function checkcolumnReferences(code) {

    $("input[name*=Formula]").each(function () {
        var formula = $(this).val();
        if (formula) {
            var formulaArray = JSON.parse(formula) ? JSON.parse(formula) : null;
            if (formulaArray.indexOf(code) > -1) {
                AppCommon.CalculateRowAmount($(this).closest("[data-code]"), true)
            }
        }
    });
}