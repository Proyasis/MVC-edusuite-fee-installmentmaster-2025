/**
 * Author: Sandun Angelo Perera
 * Date: 2016-11-26
 * Description: jquery-timesetter is a jQuery plugin which generates a UI component which is useful to take user inputs or 
 * to display time values with hour and minutes in a friendly format. UI provide intutive behaviours for better user experience 
 * such as validations in realtime and keyboard arrow key support.
 * Dependency: 
 *              jQuery-2.2.4.min.js
 *              bootstrap css: https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css
 * 
 * https://github.com/sandunangelo/jquery-timesetter
 */

(function ($)
{
    var $self;
    var wrapper;
    /**
	 * Support function to construct string with padded with a given character to the left side.
	 */
    function padLeft(value, l, c)
    {
        return Array(l - value.toString().length + 1).join(c || " ") + value.toString();
    };

    /**
     * Initialize all the time setter controls in the document.
     */
    $.fn.timesetter = function (options)
    {
        $self = this;

        var id = $(this).prop("id");
        wrapper = $("<div id='dvDuration_" + id + "'/>");
        $(this).attr("style", "position: absolute;z-index: -1;height:0;width:0;visibility:hidden");
        $(this).after(wrapper)

        if (wrapper.find(".divTimeSetterwrapper").length !== 1)
        {
            wrapper.html(htmlTemplate);
        }
        saveOptions(wrapper, options);

        var btnUp = wrapper.find('#btnUp');
        var btnDown = wrapper.find('#btnDown');

        // binding events
        btnUp.unbind('click').bind('click', function (event) { updateTimeValue(this, event); });
        btnDown.unbind('click').bind('click', function (event) { updateTimeValue(this, event); });

        var txtHours = wrapper.find('#txtHours');
        var txtMinutes = wrapper.find('#txtMinutes');
        var txtSeconds = wrapper.find('#txtSeconds');



        txtHours.unbind('focusin').bind('focusin', function (event) { $(this).select(); unitChanged(this, event); });
        txtMinutes.unbind('focusin').bind('focusin', function (event) { $(this).select(); unitChanged(this, event); });
        txtSeconds.unbind('focusin').bind('focusin', function (event) { $(this).select(); unitChanged(this, event); });

        txtHours.unbind('keydown').bind('keydown', function (event) { updateTimeValueByArrowKeys(this, event); });
        txtMinutes.unbind('keydown').bind('keydown', function (event) { updateTimeValueByArrowKeys(this, event); });
        txtSeconds.unbind('keydown').bind('keydown', function (event) { updateTimeValueByArrowKeys(this, event); });

        // apply formatting for input fields
        $(wrapper).find("input[type=text]").each(function ()
        {
            $(this).change(function (e)
            {
                formatInput(e);
                $.fn.settings.inputHourTextbox = txtHours;
                $.fn.settings.inputMinuteTextbox = txtMinutes;
                $.fn.settings.inputSecondTextbox = txtSeconds;
                if ($.fn.settings.inputHourTextbox.val() || $.fn.settings.inputMinuteTextbox.val() || $.fn.settings.inputSecondTextbox.val())
                {
                    $self.val((parseInt($.fn.settings.inputHourTextbox.val()) ? $.fn.settings.inputHourTextbox.val() : "00") + ":" + (parseInt($.fn.settings.inputMinuteTextbox.val()) ? $.fn.settings.inputMinuteTextbox.val() : "00") + ":" +(parseInt($.fn.settings.inputSecondTextbox.val()) ? $.fn.settings.inputSecondTextbox.val() : "00"));
                }
            });
        });

        // set default values
        if (txtHours.val().length === 0)
        {
            txtHours.val(padLeft($.fn.settings.hour.min.toString(), getMaxLength($.fn.settings.hour), $.fn.settings.numberPaddingChar));
        }

        if (txtMinutes.val().length === 0)
        {
            txtMinutes.val(padLeft($.fn.settings.minute.min.toString(), getMaxLength($.fn.settings.minute), $.fn.settings.numberPaddingChar));
        }
        if (txtSeconds.val().length === 0)
        {
            txtSeconds.val(padLeft($.fn.settings.second.min.toString(), getMaxLength($.fn.settings.second), $.fn.settings.numberPaddingChar));
        }

        var hourSymbolSpan = txtHours.siblings("span.hourSymbol:first");
        hourSymbolSpan.text($.fn.settings.hour.symbol);
        var minuteSymbolSpan = txtMinutes.siblings("span.minuteSymbol:first");
        minuteSymbolSpan.text($.fn.settings.minute.symbol);
        var secondSymbolSpan = txtSeconds.siblings("span.secondSymbol:first");
        secondSymbolSpan.text($.fn.settings.second.symbol);

        var postfixLabel = wrapper.find(".postfix-position");
        postfixLabel.text($.fn.settings.postfixText);

        var currValue = $self.val();
        currValue = currValue.split(':');
        if (currValue.length == 3)
        {
            $.fn.setHour(currValue[0])
            $.fn.setMinute(currValue[1])
            $.fn.setSecond(currValue[2])
        }
        return this;
    };

    /**
     * Capture the time unit which is about to update from events.
     */
    function unitChanged(sender)
    {
        var wrapper = $(sender).parents(".divTimeSetterwrapper");
        loadOptions(wrapper);

        unit = $(sender).data("unit");

        $.fn.settings.inputHourTextbox = wrapper.find('#txtHours');
        $.fn.settings.inputMinuteTextbox = wrapper.find('#txtMinutes');
        $.fn.settings.inputSecondTextbox = wrapper.find('#txtSeconds');

        saveOptions(wrapper, $.fn.settings);
    };

    /**
     * Change the time setter values from UI events.
     */
    function updateTimeValue(sender)
    {
        var wrapper = $(sender).parents(".divTimeSetterwrapper");
        loadOptions(wrapper);

        $.fn.settings.inputHourTextbox = wrapper.find('#txtHours');
        $.fn.settings.inputMinuteTextbox = wrapper.find('#txtMinutes');
        $.fn.settings.inputSecondTextbox = wrapper.find('#txtSeconds');

        $.fn.settings.hour.value = parseInt($.fn.settings.inputHourTextbox.val());
        $.fn.settings.minute.value = parseInt($.fn.settings.inputMinuteTextbox.val());
        $.fn.settings.second.value = parseInt($.fn.settings.inputSecondTextbox.val());

        $.fn.settings.direction = $(sender).data("direction");

        // validate hour and minute values
        if (isNaN($.fn.settings.hour.value))
        {
            $.fn.settings.hour.value = $.fn.settings.hour.min;
        }

        if (isNaN($.fn.settings.minute.value))
        {
            $.fn.settings.minute.value = $.fn.settings.minute.min;
        }
        if (isNaN($.fn.settings.second.value))
        {
            $.fn.settings.second.value = $.fn.settings.second.min;
        }

        // update time setter by changing hour value
        if (unit === "hours")
        {
            var oldHourValue = parseInt($($.fn.settings.inputHourTextbox).val().trim());
            var newHourValue = 0;

            if ($.fn.settings.direction === "decrement")
            {
                newHourValue = oldHourValue - $.fn.settings.hour.step;

                // tolerate the wrong step number and move to a valid step
                if ((newHourValue % $.fn.settings.hour.step) > 0)
                {
                    newHourValue = (newHourValue - (newHourValue % $.fn.settings.hour.step)); // set to the previous adjacent step
                }

                if (newHourValue <= $.fn.settings.hour.min)
                {
                    newHourValue = $.fn.settings.hour.min;
                }
            }
            else if ($.fn.settings.direction === "increment")
            {
                newHourValue = oldHourValue + $.fn.settings.hour.step;

                // tolerate the wrong step number and move to a valid step
                if ((newHourValue % $.fn.settings.hour.step) > 0)
                {
                    newHourValue = (newHourValue - (newHourValue % $.fn.settings.hour.step)); // set to the previous adjacent step
                }

                if (newHourValue >= $.fn.settings.hour.max)
                {
                    newHourValue = $.fn.settings.hour.max - $.fn.settings.hour.step;
                }
            }

            $($.fn.settings.inputHourTextbox).val(padLeft(newHourValue.toString(), getMaxLength($.fn.settings.hour), $.fn.settings.numberPaddingChar));
            $(wrapper).attr("data-hourvalue", newHourValue);
            $(wrapper).attr("data-minutevalue", newMinuteValue);
            $($.fn.settings.inputHourTextbox).trigger("change").select();
        }
        else if (unit === "minutes") // update time setter by changing minute value
        {
            var oldHourValue = $.fn.settings.hour.value;
            var newHourValue = oldHourValue;

            var oldMinuteValue = $.fn.settings.minute.value;
            var newMinuteValue = oldMinuteValue;

            if ($.fn.settings.direction === "decrement")
            {
                newMinuteValue = oldMinuteValue - $.fn.settings.minute.step;

                // tolerate the wrong step number and move to a valid step
                if ((newMinuteValue % $.fn.settings.minute.step) > 0)
                {
                    newMinuteValue = (newMinuteValue - (newMinuteValue % $.fn.settings.minute.step)); // set to the previuos adjacent step
                }

                if (newHourValue <= $.fn.settings.hour.min &&
                    oldMinuteValue <= $.fn.settings.minute.min)
                {
                    newHourValue = $.fn.settings.hour.min;
                    newMinuteValue = $.fn.settings.minute.min;
                }
            }
            else if ($.fn.settings.direction === "increment")
            {
                newMinuteValue = oldMinuteValue + $.fn.settings.minute.step;

                // tolerate the wrong step number and move to a valid step
                if ((newMinuteValue % $.fn.settings.minute.step) > 0)
                {
                    newMinuteValue = (newMinuteValue - (newMinuteValue % $.fn.settings.minute.step)); // set to the previous adjacent step
                }

                if (newHourValue >= ($.fn.settings.hour.max - $.fn.settings.hour.step) &&
                    oldMinuteValue >= ($.fn.settings.minute.max - $.fn.settings.minute.step))
                {
                    newHourValue = $.fn.settings.hour.max - $.fn.settings.hour.step;
                    newMinuteValue = $.fn.settings.minute.max - $.fn.settings.minute.step;
                }
            }

            // change the hour value when the minute value exceed its limits
            if (newMinuteValue >= $.fn.settings.minute.max && newHourValue != $.fn.settings.hour.max && newMinuteValue)
            {
                newMinuteValue = $.fn.settings.minute.min;
                newHourValue = oldHourValue + $.fn.settings.hour.step;
            }
            else if (newMinuteValue < $.fn.settings.minute.min && oldHourValue >= $.fn.settings.hour.step)
            {
                newMinuteValue = $.fn.settings.minute.max - $.fn.settings.minute.step;
                newHourValue = oldHourValue - $.fn.settings.hour.step;
            }
            else if (newMinuteValue < $.fn.settings.minute.min && oldHourValue < $.fn.settings.hour.step)
            {
                newMinuteValue = $.fn.settings.minute.min;
                newHourValue = $.fn.settings.hour.min;
            }

            $($.fn.settings.inputHourTextbox).val(padLeft(newHourValue.toString(), getMaxLength($.fn.settings.hour), $.fn.settings.numberPaddingChar));
            $($.fn.settings.inputMinuteTextbox).val(padLeft(newMinuteValue.toString(), getMaxLength($.fn.settings.minute), $.fn.settings.numberPaddingChar));
            $(wrapper).attr("data-hourvalue", newHourValue);
            $(wrapper).attr("data-minutevalue", newMinuteValue);

            $($.fn.settings.inputMinuteTextbox).trigger("change").select();

            saveOptions(wrapper, $.fn.settings);
        }
        else if (unit === "seconds") // update time setter by changing minute value
        {
            var oldHourValue = $.fn.settings.hour.value;
            var newHourValue = oldHourValue;

            var oldMinuteValue = $.fn.settings.minute.value;
            var newMinuteValue = oldMinuteValue;
            var oldSecondValue = $.fn.settings.second.value;
            var newSecondValue = oldSecondValue;

            if ($.fn.settings.direction === "decrement")
            {
                oldSecondValue = oldSecondValue - $.fn.settings.second.step;

                // tolerate the wrong step number and move to a valid step
                if ((newSecondValue % $.fn.settings.second.step) > 0)
                {
                    newSecondValue = (newSecondValue - (newSecondValue % $.fn.settings.second.step)); // set to the previuos adjacent step
                }

                if (newHourValue <= $.fn.settings.hour.min &&
                    oldMinuteValue <= $.fn.settings.minute.min &&
                    oldMinuteValue <= $.fn.settings.second.min)
                {
                    newHourValue = $.fn.settings.hour.min;
                    newMinuteValue = $.fn.settings.minute.min;
                    newSecondValue = $.fn.settings.second.min;
                }
            }
            else if ($.fn.settings.direction === "increment")
            {
                oldSecondValue = oldSecondValue + $.fn.settings.second.step;

                // tolerate the wrong step number and move to a valid step
                if ((newSecondValue % $.fn.settings.minute.step) > 0)
                {
                    newSecondValue = (newSecondValue - (newSecondValue % $.fn.settings.second.step)); // set to the previous adjacent step
                }

                if (newHourValue >= ($.fn.settings.hour.max - $.fn.settings.hour.step) &&
                    oldMinuteValue >= ($.fn.settings.minute.max - $.fn.settings.minute.step) &&
                    oldSecondValue >= ($.fn.settings.second.max - $.fn.settings.second.step))
                {
                    newHourValue = $.fn.settings.hour.max - $.fn.settings.hour.step;
                    newMinuteValue = $.fn.settings.minute.max - $.fn.settings.minute.step;
                    newSecondValue = $.fn.settings.second.max - $.fn.settings.second.step;
                }
            }

            // change the hour value when the minute value exceed its limits
            if (newSecondValue >= $.fn.settings.second.max && newMinuteValue != $.fn.settings.minute.max && newMinuteValue)
            {
                newSecondValue = $.fn.settings.second.min;
                newMinuteValue = oldMinuteValue + $.fn.settings.minute.step;
            }
            else if (newSecondValue < $.fn.settings.second.min && oldMinuteValue >= $.fn.settings.minute.step)
            {
                newSecondValue = $.fn.settings.second.max - $.fn.settings.second.step;
                newMinuteValue = oldMinuteValue - $.fn.settings.minute.step;
            }
            else if (newSecondValue < $.fn.settings.second.min && oldMinuteValue < $.fn.settings.minute.step)
            {
                newSecondValue = $.fn.settings.second.min;
                oldMinuteValue = $.fn.settings.minute.min;
            }

            $($.fn.settings.inputHourTextbox).val(padLeft(newHourValue.toString(), getMaxLength($.fn.settings.hour), $.fn.settings.numberPaddingChar));
            $($.fn.settings.inputMinuteTextbox).val(padLeft(newMinuteValue.toString(), getMaxLength($.fn.settings.minute), $.fn.settings.numberPaddingChar));
            $($.fn.settings.inputSecondTextbox).val(padLeft(newSecondValue.toString(), getMaxLength($.fn.settings.second), $.fn.settings.numberPaddingChar));
            $(wrapper).attr("data-hourvalue", newHourValue);
            $(wrapper).attr("data-minutevalue", newMinuteValue);
            $(wrapper).attr("data-secondvalue", newSecondValue);
            $($.fn.settings.inputSecondTextbox).trigger("change").select();

            saveOptions(wrapper, $.fn.settings);
        }
    };

    /**
     * Change the time setter values from arrow up/down key events
     */
    function updateTimeValueByArrowKeys(sender, event)
    {
        var wrapper = $(sender).parents(".divTimeSetterwrapper");
        loadOptions(wrapper);

        var senderUpBtn = $(wrapper).find("#btnUp");
        var senderDownBtn = $(wrapper).find("#btnDown");
        switch (event.which)
        {
            case 13: // return
                break;

            case 37: // left
                break;

            case 38: // up
                senderUpBtn.click();
                break;

            case 39: // right
                break;

            case 40: // down
                senderDownBtn.click();
                break;

            default: return; // exit this handler for other keys
        }
        event.preventDefault(); // prevent the default action (scroll / move caret)            
        saveOptions(wrapper, $.fn.settings);

        $(sender).select();
    };

    /**
     * apply sanitization to the input value and apply corrections.
     */
    function formatInput(e)
    {
        var element = $(e.target);
        var status = true;
        var wrapper = $(element).parents(".divTimeSetterwrapper");
        loadOptions(wrapper);

        var unitSettings;

        if (unit === "hours")
        {
            unitSettings = $.fn.settings.hour;
        }
        else if (unit === "minutes")
        {
            unitSettings = $.fn.settings.minute;
        }
        else if (unit === "seconds")
        {
            unitSettings = $.fn.settings.second;
        }
        if (status && !$.isNumeric(element.val()))
        {
            $(element).val(padLeft(unitSettings.min.toString(), getMaxLength(unitSettings), $.fn.settings.numberPaddingChar));
            status = false;
        }

        var value = parseInt(parseFloat(element.val()));

        // tolerate the wrong step number and move to a valid step
        // ex: user enter 20 while step is 15, auto correct to 15
        if (status && value >= unitSettings.max)
        {
            value = unitSettings.max - unitSettings.step;
            $(element).val(padLeft(value.toString(), getMaxLength(unitSettings), $.fn.settings.numberPaddingChar));
            status = false;
        }
        else if (status && value <= unitSettings.min)
        {
            $(element).val(padLeft(unitSettings.min.toString(), getMaxLength(unitSettings), $.fn.settings.numberPaddingChar));
            return false;
        }
        else if (status && padLeft(value.toString(), getMaxLength(unitSettings), $.fn.settings.numberPaddingChar) !== $(element).val())
        {
            $(element).val(padLeft(value.toString(), getMaxLength(unitSettings), $.fn.settings.numberPaddingChar));
            status = false;
        }
        else if (status && (value % unitSettings.step) > 0)
        {
            value = (value - (value % unitSettings.step)); // set to the previous adjacent step
            $(element).val(padLeft(value.toString(), getMaxLength(unitSettings), $.fn.settings.numberPaddingChar));
            status = false;
        }

        //if the letter is not digit then display error and don't type anything
        if (status && (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)))
        {
            //display error message
            status = false;
        }

        if (status && value >= Math.pow(10, getMaxLength(unitSettings)))
        {
            $(element).val(padLeft((Math.pow(10, getMaxLength(unitSettings)) - 1).toString(), getMaxLength(unitSettings), $.fn.settings.numberPaddingChar));
            status = false;
        }


    };

    /**
     * get the hour value from the control.
     */
    $.fn.getHoursValue = function ()
    {

        var txtHour = $(wrapper).find("#txtHours");
        if ($.isNumeric(txtHour.val()))
        {
            return parseInt(txtHour.val());
        }
        return $.fn.settings.hour.min;
    };

    /**
     * get the minute value from the control.
     */
    $.fn.getMinutesValue = function ()
    {

        var txtMinute = $(wrapper).find("#txtMinutes");
        if ($.isNumeric(txtMinute.val()))
        {
            return parseInt(txtMinute.val());
        }
        return $.fn.settings.minute.min;
    };

    /**
    * get the second value from the control.
    */
    $.fn.getSecondsValue = function ()
    {

        var txtSecond = $(wrapper).find("#txtSeconds");
        if ($.isNumeric(txtSecond.val()))
        {
            return parseInt(txtSecond.val());
        }
        return $.fn.settings.second.min;
    };

    /**
     * get the total number of minutes from the control.
     */
    $.fn.getTotalMinutes = function ()
    {

        var txtHour = $(wrapper).find("#txtHours");
        var txtMinute = $(wrapper).find("#txtMinutes");
        var txtSecond = $(wrapper).find("#txtSeconds");
        var hourValue = 0;
        var minuteValue = 0;
        var secondValue = 0;
        if ($.isNumeric(txtHour.val()) && $.isNumeric(txtMinute.val()) && $.isNumeric(txtSecond.val()))
        {
            hourValue = parseInt(txtHour.val());
            minuteValue = parseInt(txtMinute.val());
            minuteValue = parseInt(txtSecond.val());
        }
        return ((hourValue * 60) + minuteValue + secondValue / 60);
    };

    /**
     * get the postfix display text.
     */
    $.fn.getPostfixText = function ()
    {

        return wrapper.find(".postfix-position").text();
    };

    /**
     * set the hour value to the control.
     */
    $.fn.setHour = function (hourValue)
    {

        loadOptions(wrapper);

        var txtHours = $(wrapper).find("#txtHours");
        if ($.isNumeric(hourValue))
        {
            txtHours.val(hourValue);
        }
        else
        {
            txtHours.val(padLeft($.fn.settings.hour.min.toString(), getMaxLength($.fn.settings.hour), $.fn.settings.numberPaddingChar));
        }
        unit = "hours"
        saveOptions(wrapper, $.fn.settings);
        txtHours.change();
        return this;
    };

    /**
     * set the minute value to the control.
     */
    $.fn.setMinute = function (minuteValue)
    {

        loadOptions(wrapper);

        var txtMinute = $(wrapper).find("#txtMinutes");
        if ($.isNumeric(minuteValue))
        {
            txtMinute.val(minuteValue);
        }
        else
        {
            txtMinute.val(padLeft($.fn.settings.minute.min.toString(), getMaxLength($.fn.settings.minute), $.fn.settings.numberPaddingChar));
        }
        unit = "minutes"
        saveOptions(wrapper, $.fn.settings);
        txtMinute.change();
        return this;
    };

    /**
   * set the minute value to the control.
   */
    $.fn.setSecond = function (secondValue)
    {

        loadOptions(wrapper);

        var txtSecond = $(wrapper).find("#txtSeconds");
        if ($.isNumeric(secondValue))
        {
            txtSecond.val(secondValue);
        }
        else
        {
            txtSecond.val(padLeft($.fn.settings.second.min.toString(), getMaxLength($.fn.settings.second), $.fn.settings.numberPaddingChar));
        }
        unit = "seconds"
        saveOptions(wrapper, $.fn.settings);
        txtSecond.change();
        return this;
    };

    /**
     * set the values by calculating based on total number of minutes by caller.
     */
    $.fn.setValuesByTotalMinutes = function (totalMinutes)
    {

        loadOptions(wrapper);

        var txtHour = $(wrapper).find("#txtHours");
        var txtMinute = $(wrapper).find("#txtMinutes");
        var txtSecond = $(wrapper).find("#txtSeconds");
        var hourValue = 0;
        var minuteValue = 0;
        var secondValue = 0;

        // total minutes must be less than total minutes per day
        if (totalMinutes && totalMinutes > 0 && totalMinutes < (24 * 60))
        {
            minuteValue = (totalMinutes % 60);
            hourValue = ((totalMinutes - minuteValue) / 60);
        }

        txtHour.val(padLeft(hourValue.toString(), getMaxLength($.fn.settings.hour), $.fn.settings.numberPaddingChar));
        txtMinute.val(padLeft(minuteValue.toString(), getMaxLength($.fn.settings.minute), $.fn.settings.numberPaddingChar));

        // trigger formattings
        unit = "minutes"
        saveOptions(wrapper, $.fn.settings);
        txtMinute.change(); // one event is enough to do formatting one time for all the input fields
        return this;
    };

    /**
     * set the postfix display text.
     */
    $.fn.setPostfixText = function (textValue)
    {

        wrapper.find(".postfix-position").text(textValue);
        return this;
    };

    /**
     * plugin default options for the element
     */
    $.fn.getDefaultSettings = function ()
    {
        return {
            hour: {
                value: 0,
                min: 0,
                max: 24,
                step: 1,
                symbol: "hours"
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
            direction: null, // increment or decrement
            inputHourTextbox: null, // hour textbox
            inputMinuteTextbox: null, // minutes textbox
            inputMinuteSecondbox: null, // seconds textbox
            postfixText: "", // text to display after the input fields
            numberPaddingChar: '0' // number left padding character ex: 00052
        };
    };

    /**
     * plugin options for the element
     */
    $.fn.settings = $.fn.getDefaultSettings();

    /**
     * unit is taken out from $.fn.settings to make it globally affect as currently user is concern about which unit to change.
     */
    var unit = "minutes"; /* minutes or hours */

    /**
     * get max length based on input field options max value.
     */
    function getMaxLength(unitSettings)
    {
        return unitSettings.max.toString().length;
    };

    /**
     * save the element options' values as a data value within the element.
     */
    function saveOptions(wrapper, options)
    {
        if (options)
        {
            $.fn.settings = $.extend($.fn.settings, options);
        }
        else
        {
            $.fn.settings = $.fn.getDefaultSettings();
        }
        $(wrapper).data('options', $.fn.settings);
        return $.fn.settings;
    };

    /**
     * load the element's option values saved as data values.
     */
    function loadOptions(wrapper)
    {
        var savedOptions = $(wrapper).data('options');
        if (savedOptions)
        {
            $.fn.settings = $.extend($.fn.settings, $(wrapper).data('options'));
        }
        else
        {
            $.fn.settings = $.fn.getDefaultSettings();
        }
        return $.fn.settings;
    }

    /**
     * plugin UI html template
     */
    var htmlTemplate =
	'<div class="divTimeSetterContainer input-control form-control">' +
		'<div class="timeValueBorder">' +
			'<input id="txtHours" type="text" class="timePart hours" data-unit="hours" autocomplete="off" />' +
			'<span class="hourSymbol"></span>' +
			'<span class="timeDelimiter">:</span>' +
			'<input id="txtMinutes" type="text" class="timePart minutes" data-unit="minutes" autocomplete="off" />' +
			'<span class="minuteSymbol"></span>' +
            '<span class="timeDelimiter">:</span>' +
            '<input id="txtSeconds" type="text" class="timePart seconds" data-unit="seconds" autocomplete="off" />' +
			'<span class="secondSymbol"></span>' +

		'</div>' +
        '<label class="postfix-position"></label>' +
	'</div>';

}(jQuery));




