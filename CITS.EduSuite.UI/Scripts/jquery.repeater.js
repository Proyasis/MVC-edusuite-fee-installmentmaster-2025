// jquery.repeater version 0.1.3
// https://github.com/DubFriend/jquery.repeater
// (MIT) 13-08-2014
// Brian Detering <BDeterin@gmail.com> (http://www.briandetering.net/)
(function ($) {
    'use strict';
    var data = [];
    var identity = function (x) {
        return x;
    };

    var isArray = function (value) {
        return $.isArray(value);
    };

    var isObject = function (value) {
        return !isArray(value) && (value instanceof Object);
    };

    var isFunction = function (value) {
        return value instanceof Function;
    };

    var indexOf = function (object, value) {
        return $.inArray(value, object);
    };

    var inArray = function (array, value) {
        return indexOf(array, value) !== -1;
    };

    var foreach = function (collection, callback) {
        for (var i in collection) {
            if (collection.hasOwnProperty(i)) {
                callback(collection[i], i, collection);
            }
        }
    };

    var last = function (array) {
        return array[array.length - 1];
    };

    var mapToArray = function (collection, callback) {
        var mapped = [];
        foreach(collection, function (value, key, coll) {
            mapped.push(callback(value, key, coll));
        });
        return mapped;
    };

    var mapToObject = function (collection, callback, keyCallback) {
        var mapped = {};
        foreach(collection, function (value, key, coll) {
            key = keyCallback ? keyCallback(key, value) : key;
            mapped[key] = callback(value, key, coll);
        });
        return mapped;
    };

    var map = function (collection, callback, keyCallback) {
        return isArray(collection) ?
            mapToArray(collection, callback) :
            mapToObject(collection, callback, keyCallback);
    };

    var filter = function (collection, callback) {
        var filtered;

        if (isArray(collection)) {
            filtered = [];
            foreach(collection, function (val, key, coll) {
                if (callback(val, key, coll)) {
                    filtered.push(val);
                }
            });
        }
        else {
            filtered = {};
            foreach(collection, function (val, key, coll) {
                if (callback(val, key, coll)) {
                    filtered[key] = val;
                }
            });
        }

        return filtered;
    };


    var call = function (collection, functionName, args) {
        return map(collection, function (object, name) {
            return object[functionName].apply(object, args || []);
        });
    };

    var mixinPubSub = function (object) {
        object = object || {};
        var topics = {};

        object.publish = function (topic, data) {
            foreach(topics[topic], function (callback) {
                callback(data);
            });
        };

        object.subscribe = function (topic, callback) {
            topics[topic] = topics[topic] || [];
            topics[topic].push(callback);
        };

        object.unsubscribe = function (callback) {
            foreach(topics, function (subscribers) {
                var index = indexOf(subscribers, callback);
                if (index !== -1) {
                    subscribers.splice(index, 1);
                }
            });
        };

        return object;
    };
    // jquery.input version 0.0.0
    // https://github.com/DubFriend/jquery.input
    // (MIT) 09-04-2014
    // Brian Detering <BDeterin@gmail.com> (http://www.briandetering.net/)
    (function ($) {
        'use strict';

        var createBaseInput = function (fig, my) {
            var self = mixinPubSub(),
                $self = fig.$;

            self.getType = function () {
                throw 'implement me (return type. "text", "radio", etc.)';
            };

            self.$ = function (selector) {
                return selector ? $self.find(selector) : $self;
            };

            self.disable = function () {
                self.$().prop('disabled', true);
                self.publish('isEnabled', false);
            };

            self.enable = function () {
                self.$().prop('disabled', false);
                self.publish('isEnabled', true);
            };

            my.equalTo = function (a, b) {
                return a === b;
            };

            my.publishChange = (function () {
                var oldValue;
                return function (e, domElement) {
                    var newValue = self.get();
                    if (!my.equalTo(newValue, oldValue)) {
                        self.publish('change', { e: e, domElement: domElement });
                    }
                    oldValue = newValue;
                };
            }());

            return self;
        };


        var createInput = function (fig, my) {
            var self = createBaseInput(fig, my);

            self.get = function () {
                return self.$().val();
            };

            self.set = function (newValue) {
                self.$().val(newValue);
            };

            self.clear = function () {
                self.set('');
            };

            my.buildSetter = function (callback) {
                return function (newValue) {
                    callback.call(self, newValue);
                };
            };

            return self;
        };

        var inputEqualToArray = function (a, b) {
            a = isArray(a) ? a : [a];
            b = isArray(b) ? b : [b];

            var isEqual = true;
            if (a.length !== b.length) {
                isEqual = false;
            }
            else {
                foreach(a, function (value) {
                    if (!inArray(b, value)) {
                        isEqual = false;
                    }
                });
            }

            return isEqual;
        };

        var createInputButton = function (fig) {
            var my = {},
                self = createInput(fig, my);

            self.getType = function () {
                return 'button';
            };

            self.$().on('change', function (e) {
                my.publishChange(e, this);
            });

            return self;
        };

        var createInputCheckbox = function (fig) {
            var my = {},
                self = createInput(fig, my);

            self.getType = function () {
                return 'checkbox';
            };

            self.get = function () {
                var values = [];
                self.$().filter(':checked').each(function () {
                    values.push($(this).val());
                });
                return values;
            };

            self.set = function (newValues) {
                newValues = isArray(newValues) ? newValues : [newValues];

                self.$().each(function () {
                    $(this).prop('checked', false);
                });

                foreach(newValues, function (value) {
                    self.$().filter('[value="' + value + '"]')
                        .prop('checked', true);
                });
            };

            my.equalTo = inputEqualToArray;

            self.$().change(function (e) {
                my.publishChange(e, this);
            });

            return self;
        };

        var createInputEmail = function (fig) {
            var my = {},
                self = createInputText(fig, my);

            self.getType = function () {
                return 'email';
            };

            return self;
        };

        var createInputFile = function (fig) {
            var my = {},
                self = createBaseInput(fig, my);

            self.getType = function () {
                return 'file';
            };

            self.get = function () {
                return last(self.$().val().split('\\'));
            };

            self.clear = function () {
                // http://stackoverflow.com/questions/1043957/clearing-input-type-file-using-jquery
                this.$().each(function () {
                    $(this).wrap('<form>').closest('form').get(0).reset();
                    $(this).unwrap();
                });
            };

            self.$().change(function (e) {
                my.publishChange(e, this);
                // self.publish('change', self);
            });

            return self;
        };

        var createInputHidden = function (fig) {
            var my = {},
                self = createInput(fig, my);

            self.getType = function () {
                return 'hidden';
            };

            self.$().change(function (e) {
                my.publishChange(e, this);
            });

            return self;
        };
        var createInputMultipleFile = function (fig) {
            var my = {},
                self = createBaseInput(fig, my);

            self.getType = function () {
                return 'file[multiple]';
            };

            self.get = function () {
                // http://stackoverflow.com/questions/14035530/how-to-get-value-of-html-5-multiple-file-upload-variable-using-jquery
                var fileListObject = self.$().get(0).files || [],
                    names = [], i;

                for (i = 0; i < (fileListObject.length || 0); i += 1) {
                    names.push(fileListObject[i].name);
                }

                return names;
            };

            self.clear = function () {
                // http://stackoverflow.com/questions/1043957/clearing-input-type-file-using-jquery
                this.$().each(function () {
                    $(this).wrap('<form>').closest('form').get(0).reset();
                    $(this).unwrap();
                });
            };

            self.$().change(function (e) {
                my.publishChange(e, this);
            });

            return self;
        };

        var createInputMultipleSelect = function (fig) {
            var my = {},
                self = createInput(fig, my);

            self.getType = function () {
                return 'select[multiple]';
            };

            self.get = function () {
                return self.$().val() || [];
            };

            self.set = function (newValues) {
                self.$().val(
                    newValues === '' ? [] : isArray(newValues) ? newValues : [newValues]
                );
            };

            my.equalTo = inputEqualToArray;

            self.$().change(function (e) {
                my.publishChange(e, this);
            });

            return self;
        };

        var createInputPassword = function (fig) {
            var my = {},
                self = createInputText(fig, my);

            self.getType = function () {
                return 'password';
            };

            return self;
        };

        var createInputRadio = function (fig) {
            var my = {},
                self = createInput(fig, my);

            self.getType = function () {
                return 'radio';
            };

            self.get = function () {
                return self.$().filter(':checked').val() || null;
            };

            self.set = function (newValue) {
                if (!newValue) {
                    self.$().each(function () {
                        $(this).prop('checked', false);
                    });
                }
                else {
                    self.$().filter('[value="' + newValue + '"]').prop('checked', true);
                }
            };

            self.$().change(function (e) {
                my.publishChange(e, this);
            });

            return self;
        };

        var createInputRange = function (fig) {
            var my = {},
                self = createInput(fig, my);

            self.getType = function () {
                return 'range';
            };

            self.$().change(function (e) {
                my.publishChange(e, this);
            });

            return self;
        };

        var createInputSelect = function (fig) {
            var my = {},
                self = createInput(fig, my);

            self.getType = function () {
                return 'select';
            };

            self.$().change(function (e) {
                my.publishChange(e, this);
            });

            return self;
        };

        var createInputText = function (fig) {
            var my = {},
                self = createInput(fig, my);

            self.getType = function () {
                return 'text';
            };

            self.$().on('change keyup keydown', function (e) {
                my.publishChange(e, this);
            });

            return self;
        };

        var createInputTextarea = function (fig) {
            var my = {},
                self = createInput(fig, my);

            self.getType = function () {
                return 'textarea';
            };

            self.$().on('change keyup keydown', function (e) {
                my.publishChange(e, this);
            });

            return self;
        };

        var createInputURL = function (fig) {
            var my = {},
                self = createInputText(fig, my);

            self.getType = function () {
                return 'url';
            };

            return self;
        };

        var buildFormInputs = function (fig) {
            var inputs = {},
                $self = fig.$;

            var constructor = fig.constructorOverride || {
                //button: createInputButton,
                text: createInputText,
                url: createInputURL,
                email: createInputEmail,
                password: createInputPassword,
                range: createInputRange,
                textarea: createInputTextarea,
                select: createInputSelect,
                'select[multiple]': createInputMultipleSelect,
                radio: createInputRadio,
                checkbox: createInputCheckbox,
                file: createInputFile,
                'file[multiple]': createInputMultipleFile,
                hidden: createInputHidden
            };

            var addInputsBasic = function (type, selector) {
                var $input = isObject(selector) ? selector : $self.find(selector);

                $input.each(function () {
                    var name = $(this).attr('name');
                    inputs[name] = constructor[type]({
                        $: $(this)
                    });
                });
            };

            var addInputsGroup = function (type, selector) {
                var names = [],
                    $input = isObject(selector) ? selector : $self.find(selector);

                if (isObject(selector)) {
                    inputs[$input.attr('name')] = constructor[type]({
                        $: $input
                    });
                }
                else {
                    // group by name attribute
                    $input.each(function () {
                        if (indexOf(names, $(this).attr('name')) === -1) {
                            names.push($(this).attr('name'));
                        }
                    });

                    foreach(names, function (name) {
                        inputs[name] = constructor[type]({
                            $: $self.find('input[name="' + name + '"]')
                        });
                    });
                }
            };


            if ($self.is('input, select, textarea')) {
                if ($self.is('input[type="button"], button, input[type="submit"]')) {
                    addInputsBasic('button', $self);
                }
                else if ($self.is('textarea')) {
                    addInputsBasic('textarea', $self);
                }
                else if ($self.is('input[type="text"]')) {
                    addInputsBasic('text', $self);
                }
                else if ($self.is('input[type="password"]')) {
                    addInputsBasic('password', $self);
                }
                else if ($self.is('input[type="email"]')) {
                    addInputsBasic('email', $self);
                }
                else if ($self.is('input[type="url"]')) {
                    addInputsBasic('url', $self);
                }
                else if ($self.is('input[type="range"]')) {
                    addInputsBasic('range', $self);
                }
                else if ($self.is('select')) {
                    if ($self.is('[multiple]')) {
                        addInputsBasic('select[multiple]', $self);
                    }
                    else {
                        addInputsBasic('select', $self);
                    }
                }
                else if ($self.is('input[type="file"]')) {
                    if ($self.is('[multiple]')) {
                        addInputsBasic('file[multiple]', $self);
                    }
                    else {
                        addInputsBasic('file', $self);
                    }
                }
                else if ($self.is('input[type="hidden"]')) {
                    addInputsBasic('hidden', $self);
                }
                else if ($self.is('input[type="radio"]')) {
                    addInputsGroup('radio', $self);
                }
                else if ($self.is('input[type="checkbox"]')) {
                    addInputsGroup('checkbox', $self);
                }
                else {
                    throw 'invalid input type';
                }
            }
            else {
                //addInputsBasic('button', 'input[type="button"], button, input[type="submit"]');
                addInputsBasic('text', 'input[type="text"]');
                addInputsBasic('password', 'input[type="password"]');
                addInputsBasic('email', 'input[type="email"]');
                addInputsBasic('url', 'input[type="url"]');
                addInputsBasic('range', 'input[type="range"]');
                addInputsBasic('textarea', 'textarea');
                addInputsBasic('select', 'select:not([multiple])');
                addInputsBasic('select[multiple]', 'select[multiple]');
                addInputsBasic('file', 'input[type="file"]:not([multiple])');
                addInputsBasic('file[multiple]', 'input[type="file"][multiple]');
                addInputsBasic('hidden', 'input[type="hidden"]');
                addInputsGroup('radio', 'input[type="radio"]');
                addInputsGroup('checkbox', 'input[type="checkbox"]');
            }

            return inputs;
        };

        var initInputs = function (input, type, value) {

            if (type != "file") {
                if (type == "select")
                    value = "";
                else {
                    if (typeof value == "number" || value == "0")
                        input.set(0);
                    else
                        input.set(null);
                }

            }
        }

        $.fn.inputVal = function (newValue, subModel) {
            var $self = $(this);

            var inputs = buildFormInputs({ $: $self });

            if ($self.is('input, textarea, select')) {
                if (typeof newValue === 'undefined') {
                    return inputs[$self.attr('name')].get();
                }
                else {
                    inputs[$self.attr('name')].set(newValue);
                    return $self;
                }
            }
            else {
                if (typeof newValue === 'undefined') {
                    return call(inputs, 'get');
                }
                else {
                    foreach(newValue[0], function (value, inputName) {

                        for (var key in inputs) break;
                        if (key) {
                            key = key.match(/\[([0-9]*)\]/)[1];
                            var newName = subModel + "[" + key + "]." + inputName;
                            if (inputs[newName] != undefined) {
                                var type = inputs[newName].getType();

                                initInputs(inputs[newName], type, value);
                            }
                        }
                    });
                    return $self;
                }
            }
        };

        $.fn.inputOnChange = function (callback) {
            var $self = $(this);
            var inputs = buildFormInputs({ $: $self });
            foreach(inputs, function (input) {
                input.subscribe('change', function (data) {
                    callback.call(data.domElement, data.e);
                });
            });
            return $self;
        };

        $.fn.inputDisable = function () {
            var $self = $(this);
            call(buildFormInputs({ $: $self }), 'disable');
            return $self;
        };

        $.fn.inputEnable = function () {
            var $self = $(this);
            call(buildFormInputs({ $: $self }), 'enable');
            return $self;
        };

        $.fn.inputClear = function () {
            var $self = $(this);
            call(buildFormInputs({ $: $self }), 'clear');
            return $self;
        };

    }(jQuery));

    $.fn.repeater = function (fig) {
        fig = fig || {};

        var $self = this;

        var show = fig.show || function () {
            $(this).show();
        };

        var hide = fig.hide || function (removeElement) {
            removeElement();
        };

        var rebind = fig.rebind || function () {
            $(".section-content").mLoading("destroy");
        };

        var $list = $self.find('[data-repeater-list]');

        data = fig.data;
        var listAttr = fig.listAttr || "[data-repeater-list]";
        var itemAttr = fig.itemAttr || "[data-repeater-item]";
        var createAttr = fig.createAttr || "[data-repeater-create]";
        var deleteAttr = fig.deleteAttr || "[data-repeater-delete]";
        var repeatlist = fig.repeatlist || "";
        var submitButton = fig.submitButton;
        var $list = $self.find(listAttr);
        $("select", $list.find(itemAttr)
            .first()).selectpicker("destroy");
        var $itemTemplate = $list.find(itemAttr)
            .first().clone().hide();
       
        var groupName = $list.data('repeater-list');
        var hasFile = fig.hasFile || false;
        var Async = fig.Async || false;


        var setIndexes = function () {
            $list.find(itemAttr).each(function (index) {
                $(this).find('[name]').each(function () {
                    var names = $(this).attr('name').match(/\[([0-9]*)\]/g)
                    var oldIndex = names && names.length > 0 ? names[names.length - 1] : null;
                    if (oldIndex) {
                        var newName = $(this).attr('name');
                        newName = newName.replace(repeatlist + oldIndex, repeatlist + "[" + index + "]")
                        $(this).attr('name', newName);
                    }
                });
            });

            $list.find('input[name][checked]')
                .removeAttr('checked')
                .prop('checked', true);
        };

        setIndexes();

        var setItemsValues = function ($item, values) {
            var index;
            index = $item.find('[name]').first()
                .attr('name').match(/\[([0-9]*)\]/)[1];

            $item.inputVal(map(values, identity, function (name) {
                var tempName = name.match(/^[^\]}),]*/) ? name.match(/^[^\]}),]*/)[0] : name;
                var arr = tempName.split('[');
                if (arr.length == 2) {
                    var nameIfNotCheckbox = values[arr[1]] + '[' + index + '][' + name + ']';

                    return $item.find('[name="' + nameIfNotCheckbox + '"]').length ?
                        nameIfNotCheckbox : nameIfNotCheckbox;
                }


                // if($item.find('[name="' + nameIfNotCheckbox + '"]').length) {
                //     return nameIfNotCheckbox;
                // }
                // else
                // if($item.find(''))
                // return groupName + '[' + index + '][' + name + ']';
            }), repeatlist);
        };

        var appendItem = (function () {
            var setupTemplate = function ($item) {
                var defaultValues = fig.defaultValues;

                $item.find('[name]').each(function () {
                    $(this).inputClear();
                });

                if (defaultValues) {
                    setItemsValues($item, (defaultValues[repeatlist] ? defaultValues[repeatlist] : defaultValues));
                }
            };

            return function ($item) {
                $list.append($item);
                setIndexes();
                setupTemplate($item);

            };
        }());

        var changeValidateItem = (function () {
            var applyValidation = function () {
                $(':not([type=hidden])[data-val=true]', $list).each(function () {

                    var span = $($(this).next('span.form-control-error-text')[0]).find('span')[0];
                    if (span == undefined) {
                        span = $($(this).closest("div.form-col").find('span.form-control-error-text')).find('span')[0];
                    }
                    var attr = $(span).attr("data-valmsg-for");
                    $(this).attr("id", $(this).attr("name"));
                    $(span).attr("data-valmsg-for", $(this).attr("name"));
                    $(this).parent().find('label').attr("for", $(this).attr("name"));
                })

            };


            return function () {
                applyValidation()
            };
        }());
   
        changeValidateItem();

        var applyData = function () {
            $(itemAttr, $list).each(function (i) {
                $(this).find("select").each(function () {
                    var matches = $(this).attr('name').match(/].(.*)/)[0].replace('].', '');
                    var modelName = matches.replace('][', '').replace(']', '');
                    if (modelName != undefined && data) {
                        var jsonObjectArray = (data[repeatlist] ? data[repeatlist] : data);
                        var jsonObject = jsonObjectArray[i];
                        if (jsonObject != undefined) {
                            $(this).val(jsonObject[modelName]);
                        }
                    }
                });

            });

        };
        if (data) {
            applyData();
        }
        $("select", $list.find(itemAttr).first()).selectpicker("refresh");
        var formSubmit = (function () {

            var applyValidation = function () {
                var $form = $list.closest("form");
                $(submitButton).click(function () {
                    var validator = $($form).validate();

                    var JsonData = [];

                    if ($form.valid()) {
                        $(".section-content").mLoading();// Commented on 02062018 because of error 

                        $("[disabled=disabled]", $form).removeAttr("disabled");
                        var formData = $form.serializeArray();
                        objectifyForm(formData, data);
                        var dataurl = $form.attr("action");
                        var response = [];


                        if (hasFile) {

                            if (Async) {
                                
                                response = AjaxHelper.ajaxWithFileAsync("model", "POST", dataurl,
                                    {
                                        model: data
                                    });





                            }
                            else {
                                response = AjaxHelper.ajaxWithFile("model", "POST", dataurl,
                                    {
                                        model: data
                                    });
                            }

                        }
                        else {
                            response = AjaxHelper.ajax("POST", dataurl,
                                {
                                    model: data
                                });
                        }
                        fig.rebind(response);


                        $(".section-content").mLoading("destroy");

                    }
                    else {
                        validator.focusInvalid();
                    }

                })

            };

            return function () {
                applyValidation()
            };
        }());

        formSubmit();

        function objectifyForm(formArray, returnArray) {//serialize data function

            for (var i = 1; i < formArray.length; i++) {

                var subModelName = '', index = 0, keyName = ''
                var name = formArray[i]['name'];
                var tempName = name.match(/^[^\]}),]*/) ? name.match(/^[^\]}),]*/)[0] : name;
                keyName = name.match(/\].*/) ? name.match(/\].*/)[0].replace("].", "") : name;
                var arr = tempName.split('[');
                if (arr.length == 2) {
                    subModelName = arr[0];
                    index = arr[1];
                }
                else {
                    keyName = arr[0];
                }
                if (subModelName == "") {
                    var regex = new RegExp(/^\[/)
                    if (regex.test(name) && Array.isArray(returnArray)) {
                        if (!returnArray[index]) {
                            returnArray.push($.extend(true, {}, returnArray[0]));
                        }
                        if ($("input[name='" + name + "']")[0] && $($("input[name='" + name + "']")[0]).is(':checkbox')) {
                            returnArray[index][keyName] = $($("input[name='" + name + "']")[0]).prop('checked');
                        }
                        else {
                            returnArray[index][keyName] = formArray[i]['value'];
                        }
                    }
                    else {


                        if ($("input[name='" + name + "']")[0] && $($("input[name='" + name + "']")[0]).is(':checkbox')) {
                            returnArray[name] = $($("input[name='" + name + "']")[0]).prop('checked');
                        }
                        else if ($("input[name='" + name + "']").data("input-type") == "time") {
                            if (formArray[i]['value'])
                                returnArray[name] = AppCommon.FormatAMPMTo24Hrs(formArray[i]['value']);
                        }
                        else {
                            returnArray[name] = formArray[i]['value'];
                        }
                    }
                }
                else {

                    if (!returnArray[subModelName]) {
                        returnArray[subModelName] = [];
                    }
                    if (!returnArray[subModelName][index]) {
                        returnArray[subModelName][index] = $.extend(true, {}, data[subModelName][0]) || {};
                    }
                    if ($("input[name='" + name + "']")[0] && $($("input[name='" + name + "']")[0]).is(':checkbox')) {
                        returnArray[subModelName][index][keyName] = $($("input[name='" + name + "']")[0]).prop('checked');
                    }
                    else if ($("input[name='" + name + "']").data("input-type") == "time") {
                        if (formArray[i]['value'])
                            returnArray[name] = AppCommon.FormatAMPMTo24Hrs(formArray[i]['value']);
                    }
                    else {
                        returnArray[subModelName][index][keyName] = formArray[i]['value'];
                    }
                }
            }
            var $form = $list.closest("form");
            var files = $form.find(":file");
            for (var j = 0; j < files.length; j++) {
                var subModelName = '', index = 0, keyName = ''
                var name = $(files[j]).attr('name');
                var tempName = name.match(/^[^\]}),]*/) ? name.match(/^[^\]}),]*/)[0] : name;
                keyName = name.match(/\].*/) ? name.match(/\].*/)[0].replace("].", "") : name;
                var arr = tempName.split('[');
                if (arr.length == 2) {
                    subModelName = arr[0];
                    index = arr[1];
                }
                else {
                    keyName = arr[0];
                }
                if (subModelName == "") {
                    returnArray[name] = formArray[i]['value'];
                }
                else {
                    if (!returnArray[subModelName]) {
                        returnArray[subModelName] = {};
                    }
                    if (!returnArray[subModelName][index]) {
                        returnArray[subModelName][index] = [];
                    }
                    returnArray[subModelName][index][keyName] = files[j].files.length > 0 ? files[j].files[0] : null;
                }
            }

        }
        var triggerData = function (json) {
            data = json;
        }

        function CreateRow() {
            var $item = $itemTemplate.clone();
            var $lastItem = $list.children(itemAttr).last()
            var $form = $list.closest("form");
            var valid = true;

            var validator = $form.validate({


            });
            $('input,select,textarea', $lastItem).each(function (i, v) {
                if ($(v).is(":visible"))
                    valid = validator.element(v) && validator.element(v) && valid;


            });
            if (valid) {
                appendItem($item);
                $("form").removeData("validator");
                $("form").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("form");
                changeValidateItem();
                show.call($item.get(0));
                $("select", $item).selectpicker();

            }

        }

        var Create = $self.find(createAttr).last();
        $self.find(createAttr).last().on("click", function () {

            CreateRow();
        });

        $list.on('click', deleteAttr, function () {
            var self = $(this).closest(itemAttr).get(0);
            hide.call(self, function () {
                $(self).remove();
                var allItem = $(itemAttr, $list);
                if (allItem.length == 0) {
                    CreateRow();
                }
                setIndexes();
                changeValidateItem();
            });
        });

        return this;
    };

    $.fn.repeater.triggerData = function (json) {
        data = json;
    }
}(jQuery));




function toFormData2(obj, form, nameSpace) {
    var fd = form || new FormData();
    var formKey;

    for (var property in obj) {
        if (obj.hasOwnProperty(property) && obj[property]) {
            if (nameSpace) {
                formKey = nameSpace + '[' + property + ']';
            } else {
                formKey = property;
            }

            // if the property is an object, but not a File, use recursivity.
            if (obj[property] instanceof Date) {
                fd.append(formKey, obj[property].toISOString());
            }
            else if (typeof obj[property] === 'object' && !(obj[property] instanceof File)) {
                toFormData2(obj[property], fd, formKey);
            } else { // if it's a string or a File object
                fd.append(formKey, obj[property]);

            }

        }

    }

    return fd;
}
