var QuickJsonModel = [];
var EmployeeSalary = (function () {
    //Employee Salary
    var getEmployeeSalaries = function () {
        $(".section-content").mLoading();


        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetEmployeeSalaryList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                salaryMonthKey: function () {
                    var MonthDate = AppCommon.ParseMMMYYYYDate($("input#SalaryMonth").val());
                    var SalaryMonth = MonthDate.getMonth() + 1;
                    return SalaryMonth;

                },
                salaryYearKey: function () {
                    var MonthDate = AppCommon.ParseMMMYYYYDate($("input#SalaryMonth").val());
                    var SalaryYearKey = MonthDate.getFullYear();
                    return SalaryYearKey;
                },
                EmployeeKey: function () {
                    return $('#EmployeeKey').val()
                },
                BranchKey: function () {
                    return $('#BranchKey').val()
                }
            },
            // colNames: [Resources.RowKey, Resources.AddressType, Resources.Address, Resources.City, Resources.Country, Resources.Province, Resources.District, Resources.PostalCode, Resources.PhoneNumber, Resources.MobileNumber, Resources.EmailAddress, Resources.Action],
            colNames: [Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace,
                Resources.BlankSpace, "Pay Slip No", Resources.Employee + Resources.BlankSpace + Resources.Name, Resources.SalaryMonth, Resources.MonthlySalary,
            Resources.TotalSalary, Resources.AmountPaid, Resources.BalanceAmountToBePay,// Resources.Status,
            Resources.Action],
            colModel: [
                //{ key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, hidden: true, name: 'EmployeeKey', index: 'EmployeeKey', editable: true },
                { key: false, hidden: true, name: 'SalaryStatusKey', index: 'SalaryStatusKey', editable: true },
                { key: false, hidden: true, name: 'SalaryTypeKey', index: 'SalaryTypeKey', editable: true },
                { key: false, hidden: true, name: 'SalaryMonthKey', index: 'SalaryMonthKey', editable: true },
                { key: false, hidden: true, name: 'SalaryYearKey', index: 'SalaryYearKey', editable: true },
                { key: true, hidden: true, name: 'SalaryMasterKey', index: 'SalaryMasterKey', editable: true },
                { key: false, name: 'VoucherNumber', index: 'VoucherNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'SalaryMonth', index: 'SalaryMonth', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'M Y' } },
                { key: false, name: 'MonthlySalary', index: 'MonthlySalary', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TotalSalary', index: 'TotalSalary', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'PaidAmount', index: 'PaidAmount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BalanceAmount', index: 'BalanceAmount', formatter: formatBalanceAmount, editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'SalaryStatusName', index: 'SalaryStatusName', formatter: formatStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 200 },
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
            sortname: 'SalaryMasterKey',
            sortorder: 'desc',
            altclass: 'jqgrid-altrow',
            altRows: true,
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $("#grid a[data-payment-modal='']").each(function () {
                    AppCommon.PaymentPopupWindow($(this), $("#hdnAddEditEmployeeSalaryPayment").val(), Resources.Add + Resources.BlankSpace + Resources.Salary + Resources.BlankSpace + Resources.Payment, PaymentWindow.FormPaymentRebind);
                })

            },
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 40,
            subGridRowExpanded: showPaymentDetailsChildGrid,
            //subGridBeforeExpand: function (subgridDivId, rowid) {

            //    var rowIds = $("#grid").getDataIDs();
            //    $.each(rowIds, function (index, rowId) {
            //        $("#grid").collapseSubGridRow(rowId);
            //    });

            //},// javascript function that will take care of showing the child grid
            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },
        })

        $("#grid").jqGrid("setLabel", "FullName", "", "thFullName");
        $(".section-content").mLoading("destroy");
    }

    function showPaymentDetailsChildGrid(parentRowID, parentRowKey) {
        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";
        var childGridURL = parentRowKey + ".json";
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: $("#hdnGetEmployeePaymentDetails").val() + "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { key: false, hidden: true, name: 'PaymentKey', index: 'PaymentKey', editable: true },
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },

                { label: Resources.VoucherNumber, name: 'ReceiptNumber' },
                { label: Resources.PaymentMode, name: 'PaymentModeName' },
                { label: Resources.TotalPaid, name: 'PaidAmount' },
                { label: Resources.BalanceAmount, name: 'BalanceAmount' },
                { label: Resources.SalaryPaymentDate, name: 'PaymentDate', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { label: Resources.Purpose, name: 'Purpose' },
                { label: Resources.ReceivedBy, name: 'ReceivedBy' },
                { label: Resources.PaidBy, name: 'PaidBy' },
                { label: Resources.AuthorizedBy, name: 'AuthorizedBy' },
                { label: Resources.Remarks, name: 'Remarks' },

                { label: Resources.Blankspace, name: 'Action', search: false, index: 'PaymentKey', sortable: false, formatter: editSubGridLink, resizable: false, width: 250 },
            ],
            loadonce: true,
            width: 500,
            height: '100%',
            pager: false,
            footer: false
        });

    }


    function editSubGridLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.PaymentKey + "'";

        var obj = {};
        obj.id = rowdata.PaymentKey;
        var printtemp = "'" + rowdata.BranchKey + "'" + ',' + "'" + rowdata.ReceiptNumber + "'" + ',' + "'" + Resources.InvoicePrintTypeOrderReceipt + "'";

        url = $("#hdnAddEditEmployeeSalaryPaymentEdit").val() + "?" + $.param(obj);

        var html = '<a class="btn btn-primary btn-sm mx-1"  onclick="EmployeeSalary.EditPaymentPopup(' + rowdata.PaymentKey + ',' + true + ');return false;" ><i class="fa fa-pencil pointer" aria-hidden="true"></i></a>'
            + '<a class="btn btn-danger btn-sm mx-1"  title="' + Resources.Delete + '" onclick="javascript:deleteEmployeeSalaryPayments(' + rowdata.PaymentKey + ');return false;" ><i class="fa fa-trash pointer" aria-hidden="true"></i></a>'
            + '<a class="btn btn-outline-success btn-sm mx-1 btnPrint" onclick="PrintInvoice.PrintReceipt(null, ' + printtemp + ')"><i class="fa fa-print" aria-hidden="true"></i></a>'


        // var html = html + '<a class="btn btn-warning btn-sm mx-1"  title="' + Resources.Payment + '"  data-payment-modal="" data-href="' + $("#hdnAddEditEmployeeSalaryPaymentEdit").val() + '/' + obj.id + '" ><i class="fa fa-credit-card" aria-hidden="true"></i></a>';
        return html;
    }


    var getEmployeeSalaryDetails = function (json) {
        //var output = PivotJSON(json["EmployeeSalaryDetails"], "SalaryHeadName", "SalaryHeadKey", "Amount");
        //EmployeeSalary.SetExcelSheelValue(output);
        var MonthlySalary = $("#DetailTotal").html().trim();
        $("input#MonthlySalary").val(parseFloat(parseFloat(MonthlySalary).toFixed(2)).toString());
        EmployeeSalary.CalculateTotalSalary();
    }

    var setExcelSheelValue = function (JsonData) {
        $('#Excel').jexcel({
            data: JsonData,
            // colHeaders: JsonHead,
            columnSorting: false,
            colWidths: [0, 300],
            columns: [
                { type: 'text' },
                { type: 'text' },
                { type: 'number' },
                { type: 'number' },
                { type: 'number' },
                { type: 'number' }
            ]
        });
        var val0 = "", val2 = "", valueTotal = "";
        $('#Excel').jexcel('updateSettings', {
            cells: function (cell, col, row) {

                //  $(cell).addClass('readonly');

                if (col == 0) {
                    val0 = $("#Excel").jexcel('getValue', $(cell)).trim()

                    $(cell).hide();

                }
                else if (col == 2) {
                    val2 = $("#Excel").jexcel('getValue', $(cell)).trim();
                    if (val0 == "0" && val2 == "0")
                        $(cell).html("-");
                }
                if ((val0 == "0") && col == 2) {
                    $(cell).closest("tr").addClass("sub-title");
                }
                if ((val0 == "*") && col == 2) {
                    $(cell).closest("tr").addClass("main-footer");
                }
                if ((val0 == "0" || val0 == "-") && col == 2) {
                    if (val2 != "0" && val2 != "-") {
                        $(cell).closest("tr").addClass("sub-footer");
                    }


                }
                if (val0 == "*")
                    valueTotal = val0;

            }
        });

        $(".jexcel_label").hide();
    }

    var formSubmit = function (data) {

        $form = $("form");
        if ($($form).valid()) {
            $(".section-content").mLoading();
            var dataurl = $form.attr("action");
            AjaxHelper.ajaxAsync("POST", dataurl,
                {
                    model: $form.serializeToJSON({
                        associativeArrays: false
                    })
                }, function () {
                    var response = this;
                    if (response.IsSuccessful == true) {
                        window.location.href = $("#hdnEmployeeSalaryList").val() + "/" + data["EmployeeKey"];
                    }
                    else {
                        $("[data-valmsg-for=error_msg]").html(response.Message);
                    }
                    $(".section-content").mLoading('destroy');
                });


        }
    }

    var getBalance = function (paymentMode, PurchaseOrderPaymentRowKey, BankAccountKey, branchKey) {
        var url = $("#hdnGetBalance").val()
        var response = AjaxHelper.ajax("GET", $("#hdnGetBalance").val() + "?PaymentModeKey=" + paymentMode + "&PurchaseOrderPaymentRowKey=" + PurchaseOrderPaymentRowKey + "&BankAccountKey=" + BankAccountKey + "&branchKey=" + branchKey)

        $("#CashFlowBankBalance").val(response);
        if ($("#AccountMethord").val() != 2) {
            $("#CashFlowBankBalance").val($("#Amount").val());
        }
    }

    var deductAdvance = function () {
        var advanceAmount = 0;
        var totalAdvance = 0;
        var totalBalance = 0;
        $("[advanceRepeater]").each(function () {
            var item = $(this).find("[id*=IsDeduct]")
            var advanceBalanceControl = $("[advanceBalance]", $(this))
            var paidControl = $("[id*=PaidAmount]", $(this))
            var beforechangePaid = $("[id*=BeforeTakenAdvance]", $(this)).val()
            beforechangePaid = parseFloat(beforechangePaid) ? parseFloat(beforechangePaid) : 0
            if (item[0].checked) {
                var amount = $(this).find("[id*=PaidAmount]").val()
                amount = amount != "" ? parseFloat(amount) : 0;
                advanceAmount = advanceAmount + amount;
            }
            else {
                $(advanceBalanceControl).html(beforechangePaid)
                $(paidControl).val(0)
                $(paidControl).attr('readonly', true)
            }
            var paid = $(paidControl).val()
            var advanceBalance = $(advanceBalanceControl).html()
            advanceBalance = parseFloat(advanceBalance) ? parseFloat(advanceBalance) : 0
            paid = parseFloat(paid) ? parseFloat(paid) : 0
            totalAdvance = totalAdvance + paid;
            totalBalance = totalBalance + advanceBalance;
        })
        $("input#SalaryAdvance").val(advanceAmount);
        $("[totaladvance]").html(totalAdvance);
        $("[totalbalance]").html(totalBalance);
    }

    var calculateRowSum = function () {
        var OverTimeAmountPerAHour = $("#OvertimePerAHour").val();
        OverTimeAmountPerAHour = parseFloat(OverTimeAmountPerAHour) ? parseFloat(OverTimeAmountPerAHour) : 0;
        var AdditionalDayAmount = $("#AdditionalDayAmount").val();
        AdditionalDayAmount = parseFloat(AdditionalDayAmount) ? parseFloat(AdditionalDayAmount) : 0;
        var AdditionalDayCount = $("#AdditionalDayCount").val();
        AdditionalDayCount = parseFloat(AdditionalDayCount) ? parseFloat(AdditionalDayCount) : 0;
        var AdditionalDayTotalAmount = AdditionalDayAmount * AdditionalDayCount;
        AdditionalDayTotalAmount = parseFloat(AdditionalDayTotalAmount) ? parseFloat(AdditionalDayTotalAmount) : 0;
        $("#AdditionalDayTotalAmount").val(parseFloat(AdditionalDayTotalAmount.toFixed(2))).toString();
        var OverTimeHours = $("input#OverTimeHours").val();
        OverTimeHours = OverTimeHours != "" ? parseFloat(OverTimeHours) : 0;
        var OverTimeMinutes = $("input#OverTimeMinutes").val();
        OverTimeMinutes = OverTimeMinutes != "" ? parseFloat(OverTimeMinutes) : 0;
        OverTimeHours = OverTimeHours + (OverTimeMinutes / 60);
        var OverTimeAmount = OverTimeHours * OverTimeAmountPerAHour;
        OverTimeAmount = OverTimeAmount + AdditionalDayTotalAmount;
        $("#OverTimeTotalAmount").val(parseFloat(OverTimeAmount.toFixed(2))).toString();
        $("li[data-code]").each(function () {
            AppCommon.CalculateRowAmount(this);
        });
        var TotalEarnings = $("[data-earnings]").map(function () {
            return this.value
        }).get().reduce(function (sum, n) {
            return sum += (parseFloat(n) ? parseFloat(n) : 0)
        }, 0)
        var TotalDeductions = $("[data-deductions]").map(function () {
            return this.value
        }).get().reduce(function (sum, n) {
            return sum += (parseFloat(n) ? parseFloat(n) : 0)
        }, 0)
        TotalEarnings = TotalEarnings ? TotalEarnings : 0;
        TotalDeductions = TotalDeductions ? TotalDeductions : 0;
        $("#GrossSalary").val(parseFloat(TotalEarnings.toFixed(Resources.RoundToDecimalPostion)).toString());
        $("#TotalEarnings").val(parseFloat(TotalEarnings.toFixed(Resources.RoundToDecimalPostion)).toString())
        $("#TotalDeductions").val(parseFloat(TotalDeductions.toFixed(Resources.RoundToDecimalPostion)).toString())
    }

    var calculateLOP = function () {

        var $form = $("form");
        var SalaryTypeKey = $("#SalaryTypeKey").val();
        var BasicSalary = $("input#MonthlySalary").val();
        BasicSalary = BasicSalary != "" ? parseFloat(BasicSalary) : 0;
        var NoOfWorkingDays = $("input#NoOfDaysWorked").val();
        NoOfWorkingDays = NoOfWorkingDays != "" ? parseFloat(NoOfWorkingDays) : 0;

        var UnpaidLeaves = $("[name*=LeaveTypeCount]").toArray().reduce(function (sum, item) {
            var value = 0;
            var li = $(item).closest("li");
            var LeaveCount = $(item).val();
            LeaveCount = parseInt(LeaveCount) ? parseInt(LeaveCount) : 0;

            var LeaveAvailableCount = $("[name*=LeaveAvailableCount]", li).val();
            LeaveAvailableCount = parseInt(LeaveAvailableCount) ? parseInt(LeaveAvailableCount) : 0;

            var SalaryDeductionForAdditional = $("[name*=SalaryDeductionForAdditional]", li).val();
            SalaryDeductionForAdditional = SalaryDeductionForAdditional ? JSON.parse(SalaryDeductionForAdditional.toLowerCase()) : false;
            value = SalaryDeductionForAdditional ? (LeaveAvailableCount < LeaveCount ? LeaveAvailableCount : LeaveCount) : LeaveCount;
            return sum + value
        }, 0);
        NoOfWorkingDays = NoOfWorkingDays + UnpaidLeaves;

        var month = AppCommon.ParseMMMYYYYDate($("input#SalaryMonth").val());
        var MonthKey = moment(month).month() + 1;
        var YearKey = moment(month).year();
        var totalDays = $("input#TotalWorkingDays").val();
        var baseWorkingDays = $("input#BaseWorkingDays").val();
        baseWorkingDays = parseInt(baseWorkingDays) ? parseInt(baseWorkingDays) : 0
        totalDays = parseInt(totalDays) ? parseInt(totalDays) : 1;
        baseWorkingDays = baseWorkingDays ? baseWorkingDays : totalDays;

        var SalaryAdvance = $("input#SalaryAdvance").val();
        SalaryAdvance = parseFloat(SalaryAdvance) ? parseFloat(SalaryAdvance) : 0;
        var PerDayAmount = $("#PerDayAmount").val();
        PerDayAmount = parseFloat(PerDayAmount) ? parseFloat(PerDayAmount) : 0;
        var LOP = (totalDays - NoOfWorkingDays) * PerDayAmount;


        LOP = parseFloat(LOP) ? parseFloat(LOP) : 0;



        if (SalaryTypeKey == Resources.SalaryTypeMonthly) {
            //$("#EmployeeLOP").val(parseFloat(LOP.toFixed(2)).toString())
            $("#LOP").val(parseFloat(LOP.toFixed(2)).toString())
        }
        //$("#GrossSalary").val(parseFloat(grossSalary.toFixed(2)).toString());


    }

    var calculateGrossSalary = function () {
        
        var $form = $("form");
        var SalaryTypeKey = $("#SalaryTypeKey").val();
        var BasicSalary = $("input#MonthlySalary").val();
        BasicSalary = BasicSalary != "" ? parseFloat(BasicSalary) : 0;
        //Comented by khaleefa 08 Feb 2023
        //var month = AppCommon.ParseMMMYYYYDate($("input#SalaryMonth").val());
        //var MonthKey = moment(month).month() + 1;
        //var YearKey = moment(month).year();
        var NoOfWorkingDays = $("input#NoOfDaysWorked").val();
        NoOfWorkingDays = NoOfWorkingDays != "" ? parseFloat(NoOfWorkingDays) : 0;
        var totalDays = $("input#TotalWorkingDays").val();
        totalDays = parseInt(totalDays) && parseInt(totalDays) != 0 ? parseInt(totalDays) : 1
        //var SalaryAdvance = $("input#SalaryAdvance").val();
        //SalaryAdvance = SalaryAdvance != "" ? parseFloat(SalaryAdvance) : 0;

        var AbsentDays = $("input#AbsentDays").val();
        AbsentDays = AbsentDays != "" ? parseFloat(AbsentDays) : 0;
        NoOfWorkingDays = totalDays - AbsentDays;
        NoOfWorkingDays = parseInt(NoOfWorkingDays) > 0 ? parseInt(NoOfWorkingDays) : 0;
        var BaseWorkingDays = $("input#BaseWorkingDays").val();
        BaseWorkingDays = parseInt(BaseWorkingDays) && parseInt(BaseWorkingDays) != 0 ? parseInt(BaseWorkingDays) : 1

        $("#NoOfDaysWorked").val(NoOfWorkingDays);

        if (SalaryTypeKey == Resources.SalaryTypeMonthly) {
            var grossSalary = (BasicSalary);
            var perdaysalary = (BasicSalary / BaseWorkingDays);
            perdaysalary = parseFloat(perdaysalary) ? parseFloat(perdaysalary) : 0;
            var absentsalary = perdaysalary * AbsentDays;
            absentsalary = parseFloat(absentsalary) ? parseFloat(absentsalary) : 0;

            var LOP = absentsalary;
            grossSalary = grossSalary;
        }
        else if (SalaryTypeKey == Resources.SalaryTypeDaily) {
            var grossSalary = (BasicSalary * NoOfWorkingDays);
        }
        else if (SalaryTypeKey == Resources.SalaryTypeHourly) {
            var grossSalary = (BasicSalary * NoOfWorkingDays);
        }

        LOP = parseFloat(LOP) ? parseFloat(LOP) : 0;
        grossSalary = parseFloat(grossSalary) ? parseFloat(grossSalary) : 0;



        if (SalaryTypeKey == Resources.SalaryTypeMonthly) {
            //$("#EmployeeLOP").val(parseFloat(LOP.toFixed(2)).toString())
            $("#LOP").val(parseFloat(LOP.toFixed(2)).toString())
        }

        $("#GrossSalary").val(parseFloat(grossSalary.toFixed(2)).toString());


    }

    var getSalaryComponentsByMonth = function (data, item) {
        var obj = {};
        obj.employeeKey = data.EmployeeKey;
        obj.branchKey = data.BranchKey;
        obj.salaryMonthKey = data.SalaryMonthKey;
        obj.salaryYearKey = data.SalaryYearKey;
        window.location.href = $("#hdnAddEditEmployeeSalary").val() + "?" + $.param(obj);
        var response = AjaxHelper.ajax("POST", $("#hdnGetSalaryComponentsByMonth").val(),
            {
                model: data
            });
        var i = 0;
        //if (response.SalaryMasterKey != 0) {
        //    var obj = {};
        //    obj.employeeKey = response.EmployeeKey;
        //    obj.branchKey = response.BranchKey;
        //    obj.salaryMonthKey = response.SalaryMonthKey;
        //    obj.salaryYearKey = response.SalaryYearKey;
        //    window.location.href = $("#hdnAddEditEmployeeSalary").val() + "?" + $.param(obj);
        //}
        //else {
        //    var $fieldSet = $(item).clone();
        //    $("div[salary-component-item],#btnPaymentWindow,#btnPayslip").remove();
        //    var currentLength = $("#fsSalaryComponent div[salary-component-item].ComponentItemNew").length;

        //    $.each(response.SalaryComponents, function (i, JsonObject) {
        //        if (JsonObject["SalaryComponentTypeName"] != "") {

        //            $fieldSet.removeAttr("id").removeAttr("class").removeAttr("style").attr("class", "ComponentItemNew  row bottomLine");
        //            $fieldSet.find('input, select, textarea').each(function () {
        //                var matches = $(this).attr('name').match(/].(.*)/)[0].replace('].', '');
        //                var name = matches.replace('][', '').replace(']', '');
        //                var newId = "SalaryComponets" + "[" + i.toString() + "]." + name;
        //                $(this).attr('name', newId).attr('id', newId);
        //                $(this).val(JsonObject[name]);
        //                if (JsonObject["OperationType"] == "A") {
        //                    $(this).removeClass("text-red").addClass('text-green');
        //                }
        //                else {
        //                    $(this).addClass('text-red');
        //                }
        //            });
        //            var SalaryComponentTypeName;
        //            SalaryComponentTypeName = JsonObject["SalaryComponentTypeName"];
        //            if (JsonObject["OperationType"] == "A") {
        //                $($fieldSet.find("label#SalaryComponentTypeName")[0]).removeClass("text-red").addClass('text-green');
        //            }
        //            else {
        //                $($fieldSet.find("label#SalaryComponentTypeName")[0]).addClass('text-red');
        //            }
        //            $($fieldSet.find("label#SalaryComponentTypeName")[0]).html(SalaryComponentTypeName);
        //            $("#fsSalaryComponent").append($fieldSet)
        //            $fieldSet = $(item).clone();
        //        }
        //    })
        //    if (response.SalaryComponents.length > 0 && (response.SalaryComponents.length > 1 || response.SalaryComponents[0].SalaryComponentTypeName != "")) {
        //        $("#fsSalaryComponent").show();
        //    }
        //    else {
        //        $("#fsSalaryComponent").hide();
        //    }

        //    $("#SalaryMasterKey").val(response.SalaryMasterKey);
        //    $("#dvPaymentWindow").html("");

        //    EmployeeSalary.GetEmployeeSalaryDetails(response);
        //    if (response.SalaryMasterKey != 0) {
        //        var dvPayment = $("#dvPaymentWindow")[0];
        //        if (!dvPayment) {
        //            dvPayment = $("<div/>").attr("id", "dvPaymentWindow").addClass("payment-window");
        //            $("#frmEmployeeSalaryPayment").append(dvPayment);
        //            var height = (parseInt($("#dvSalaryDetails").height()) + 113) + "px";
        //            $("#dvPaymentWindow").css("top", height);
        //        }
        //        $("#dvPaymentWindow").load($("#hdnAddEditEmployeeSalaryPayment").val() + "/" + response.SalaryMasterKey, function () {
        //            AppCommon.FormatDateInput();
        //            $("#frmEmployeeSalaryPayment").removeData("validator");
        //            $("#frmEmployeeSalaryPayment").removeData("unobtrusiveValidation");
        //            $.validator.unobtrusive.parse("#frmEmployeeSalaryPayment");
        //        })
        //    }
        //    else {
        //        $("#dvPaymentWindow").remove();
        //    }
        //    if (response["LoanAmount"] == null) {
        //        $("#LoanAmount").val("");
        //        $("#dvLoan").hide();
        //    }
        //    else {
        //        $("#LoanAmount").val(parseFloat(response["LoanAmount"]).toFixed(2).replace(/\.0+$/, ''));
        //        $("#dvLoan").show();
        //    }
        //    $("#OtherAmount").val(response["OtherAmount"] ? parseFloat(response["OtherAmount"]).toFixed(2).replace(/\.0+$/, '') : "");
        //    $("#NoOfDaysWorked").val(response["NoOfDaysWorked"])
        //    EmployeeSalary.calculateLOP();
        //    //$("#fsSalaryComponent").remove();
        //}
    }

    var getOperationByAdditionalComponent = function (value) {
        value = value != "" ? parseInt(value) : 0;

        $.ajax({
            url: $("#hdnGetOperationByAdditionalComponent").val(),
            type: "GET",
            dataType: "JSON",
            data: { Id: value },
            success: function (result) {
                var $row = $(this).closest("div.row")[0];
                $("[id*=OperationType]", $row).val(result);
                EmployeeSalary.CalculateGrossSalary();
            }
        });
    }

    var changeOtherAmountStyle = function (chk) {
        if (chk.checked) {
            $("#lblOtherAmount,#OtherAmount").removeClass("text-red").addClass("text-green"),
                $("#OtherAmount").removeClass("border-red").addClass("border-green");
        }
        else {
            $("#lblOtherAmount,#OtherAmount").removeClass("text-green").addClass("text-red");
            $("#OtherAmount").removeClass("border-green").addClass("border-red");
        }
        EmployeeSalary.CalculateGrossSalary();
    }

    var getEmployeesByBranchId = function (Id, ddl) {
        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetEmployeesByBranchId").val(), ddl, Resources.Employee);

    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.SalaryMasterKey + "'";
        var tempPayKey = "'" + rowdata.SalaryPaymentKey + "'";

        var obj = {};
        obj.id = rowdata.SalaryMasterKey;
        obj.employeeKey = rowdata.EmployeeKey;
        obj.branchKey = rowdata.BranchKey;
        obj.salaryMonthKey = rowdata.SalaryMonthKey;
        obj.salaryYearKey = rowdata.SalaryYearKey;
        url = $("#hdnAddEditEmployeeSalary").val() + "?" + $.param(obj);
        var html = '<div class="divEditDelete">';
         html = '<a class="btn btn-outline-primary btn-sm mx-1" title="' + Resources.Edit + '" href="' + url + '" ><i class="fa fa-pencil" aria-hidden="true"></i></a>'
            + '<a class="btn btn-outline-danger btn-sm mx-1"  title="' + Resources.Delete + '" onclick="javascript:deleteEmployeeSalary(' + temp + ');return false;" ><i class="fa fa-trash" aria-hidden="true"></i></a>'
        if (rowdata.SalaryStatusKey == Resources.ProcessStatusApproved) {
            html = html + '<a class="btn btn-outline-warning btn-sm mx-1" title="' + Resources.Payment + '"  onclick="EmployeeSalary.EditPaymentPopup(' + rowdata.SalaryMasterKey + ',' + false + ');return false;"><i class="fa fa-credit-card" aria-hidden="true"></i></a>';

            if (rowdata.SalaryTypeKey == Resources.SalaryTypeMonthly) {
                html = html + '<a class="btn btn-outline-success btn-sm mx-1 btnPrint"   title="' + Resources.PaySlip + '" onclick="EmployeeSalary.SinglePayslipClickEvent(' + rowdata.SalaryMasterKey + ',' + false + ');return false;"  ><i class="fa fa-file" aria-hidden="true"></i></a>';

            }
        }
        html = html + '</div>';
        return html;
    }

    function formatStatus(cellValue, options, rowdata, action) {
        var html = "";
        switch (rowdata.SalaryStatusKey) {
            case Resources.ProcessStatusApproved:
                html = '<span class="label label-success">' + cellValue + '</span>';
                break;
            case Resources.ProcessStatusRejected:
                html = '<span class="label label-danger">' + cellValue + '</span>';
                break;
            default:
                html = '<span class="label label-warning">' + cellValue + '</span>';

        }
        return html;
    }
    function formatBalanceAmount(cellValue, options, rowdata, action) {

        var BalanceAmount = rowdata.TotalSalary - rowdata.PaidAmount;
        BalanceAmount = parseFloat(BalanceAmount.toFixed(Resources.RoundToDecimalPostion));
        return BalanceAmount;
    }
    //Quick Salary

    var getQuickEmployeesSalary = function (json, realod) {
        $(".section-content").mLoading();
        setTimeout(function () {
            var response = AjaxHelper.ajax("POST", $("#hdnGetQuickEmployeesSalary").val(),
                {
                    model: json
                });
            QuickJsonModel = $.extend(true, [], json);
            var colHeaders = [], colModels = [], groupParam = {};
            groupParam.payStart = ""; groupParam.dedStart = ""; groupParam.payLength = 0; groupParam.dedLength = 0;
            var totalDays = (new Date(json["SalaryYearKey"], json["SalaryMonthKey"], 0)).getDate();
            $("#gridQuick").jqGrid('clearGridData')
            var resultData = EmployeesSalaryData(response.rows, totalDays, colModels, colHeaders, groupParam);

            $("#gridQuick").jqGrid('setGridParam', { datatype: 'local', data: resultData }).trigger("reloadGrid");
            $("#gridQuick").jqGrid({
                datatype: 'local',
                pager: jQuery('#pagerQuick'),
                rowNum: 10,
                rowList: [Resources.PagingRowNum, 10, 20, 50, 100].unique(),
                colNames: colHeaders,
                colModel: colModels,
                autowidth: true,
                height: '100%',
                viewrecords: true,
                emptyrecords: Resources.NoRecordsToDisplay,
                data: resultData,
                multiselect: true,
                loadonce: true,
                ignoreCase: true,
                altRows: true,
                edit: true,
                cellEdit: true,
                cellsubmit: 'clientArray',
                editurl: 'clientArray',
                altclass: 'jqgrid-altrow',
                loadComplete: function (data) {
                    $("#gridQuick a[data-payment-modal='']").each(function () {
                        AppCommon.PaymentPopupWindow($(this), $("#hdnAddEditEmployeeSalaryPayment").val(), Resources.Add + Resources.BlankSpace + Resources.Salary + Resources.BlankSpace + Resources.Payment, PaymentWindow.FormPaymentRebind);

                    })
                    $("#gridQuick tr").each(function () {
                        if ($(this).attr("id") == "-1") {
                            $(this).hide();
                        }
                    });

                }
            })
            if (!realod) {
                $("#gridQuick").jqGrid('setGroupHeaders', {
                    useColSpanStyle: false,
                    groupHeaders: [
                        { startColumnName: groupParam.payStart, numberOfColumns: groupParam.payLength, titleText: 'Payments' },
                        { startColumnName: groupParam.dedStart, numberOfColumns: groupParam.dedLength, titleText: 'Deductions' }
                    ]
                });
            }
            $("#gridQuick").jqGrid("setLabel", "FullName", "", "thFullName");
            $(".section-content").mLoading("destroy");
        }, 500)
    }

    var singleProcessClickEvent = function (id) {
        var jsonData = [];
        var rowData = $("#gridQuick").getRowData(id);
        $("#gridQuick tr[id=" + id + "] a.loading").show();
        jsonData.push(rowData);
        var lastRowData = $("#gridQuick").getRowData("-1");
        formSubmitMultipleData(QuickJsonModel, jsonData, lastRowData)
    }

    var multipleProcessClickEvent = function (json) {
        var jsonData = [];
        var rowIds = $("#gridQuick").jqGrid("getGridParam", "selarrrow");

        rowIds.forEach(function (item) {
            var rowData = $("#gridQuick").getRowData(item);
            $("#gridQuick tr[id=" + item + "] a.loading").show();
            if (rowData.EmployeeKey != "-1")
                jsonData.push(rowData);
        })
        var lastRowData = $("#gridQuick").getRowData("-1");
        formSubmitMultipleData(json, jsonData, lastRowData)
    }

    var singlePayslipClickEvent = function (Id) {
        //var Ids = [], htmls = [];
        //Ids.push(Id);
        //var url = $("#hdnSalaryPaySlip").val() + "?Ids=" + Ids.join(",");
        //window.open(url, "", "_blank");

        window.open($("#hdnSalaryPaySlip").val() + "/" + Id, '_blank');


    }

    var multiplePayslipClickEvent = function () {
        var Ids = [], htmls = [];
        var rowIds = $("#gridQuick").jqGrid("getGridParam", "selarrrow");
        if (rowIds.length > 0) {
            rowIds.forEach(function (item) {
                var rowData = $("#gridQuick").getRowData(item);
                if (rowData.SalaryMasterKey && rowData.SalaryMasterKey != "" && rowData.SalaryMasterKey != "0") {
                    var salaryMasterKey = parseFloat(rowData.SalaryMasterKey) ? parseFloat(rowData.SalaryMasterKey) : 0;
                    Ids.push(salaryMasterKey);
                }
            })
            if (Ids.length > 0) {
                var url = $("#hdnSalaryPaySlip").val() + AppCommon.EncodeQueryString("Ids=" + Ids.join(","));
                window.open(url, "_blank");
                //var response = AjaxHelper.ajax("POST", $("#hdnSalaryPaySlip").val(),
                //                { Id: Ids }
                //              );
                //$.each(response, function () {
                //    var html = generatePayslip($(this));
                //    htmls.push(html);
                //});
                //generatePayslipPDF(htmls);
            }
        }
    }

    //Payslip
    var modifyPayslip = function (Data) {
        $.each(Data, function (i) {
            var data = Data[i];
            var PaymentSum = 0, DeductionSum = 0, componentSum = 0;
            var BasicSalary = data["SalaryPayments"][0].Amount;
            BasicSalary = BasicSalary ? parseFloat(BasicSalary) : 0;
            $.each(data["SalaryPayments"], function (i) {

                var Amount = data["SalaryPayments"][i].Amount;
                if (Amount != "" && Amount != "0") {
                    PaymentSum = PaymentSum + Amount
                }
                else {
                    delete data["SalaryPayments"][i];
                }
            })
            var overTime = data["OvertimeAmount"];
            overTime = overTime ? overTime : 0
            PaymentSum = PaymentSum + overTime
            $.each(data["SalaryDeductions"], function (j) {
                var Amount = data["SalaryDeductions"][j].Amount;
                if (Amount != "" && Amount != "0") {
                    DeductionSum = DeductionSum + Amount
                }
                else {
                    delete data["SalaryDeductions"][j];
                }

            })
            var TotalSumWthoutAdv = (PaymentSum - DeductionSum);
            $("#dvFixedGross_" + i.toString()).html(parseFloat((TotalSumWthoutAdv).toFixed(2)).toString())
            $.each(data["SalaryAdvances"], function (j) {
                var Amount = data["SalaryAdvances"][j].Amount;
                if (Amount != "" && Amount != "0") {
                    DeductionSum = DeductionSum + Amount
                }
                else {
                    delete data["SalaryAdvances"][j];
                }

            })

            var TotalSum = (PaymentSum - DeductionSum);
            $.each(data["SalaryComponents"], function (k) {
                var Amount = data["SalaryComponents"][k].AmountUnit;
                var Operation = data["SalaryComponents"][k].OperationType;
                if (Amount != "" && Amount != "0") {
                    if (Operation == "A") {
                        componentSum = componentSum + Amount
                        PaymentSum = PaymentSum + Amount;
                    }
                    else {
                        componentSum = componentSum - Amount
                        DeductionSum = DeductionSum + Amount;
                    }
                }
                else {
                    delete data["SalaryComponents"][k];
                }

            })



            var month = data["SalaryMonthKey"];


            var salaryMonth = data["SalaryMonthKey"];
            var salaryYear = data["SalaryYearKey"];
            var totalDays = data["DaysInMonth"];
            var daysWorked = parseFloat(data["NoOfDaysWorked"]) ? parseFloat(data["NoOfDaysWorked"]) : 0;
            var netPay = (BasicSalary * daysWorked / totalDays);
            //var lopAmount = BasicSalary - netPay;
            //netPay = PaymentSum - (DeductionSum + lopAmount)
            netPay = PaymentSum - (DeductionSum)

            //$("#dvLOP_" + i.toString()).html(lopAmount.toFixed(2));
            $("#SalaryPaymentSum_" + i.toString()).html(PaymentSum.toFixed(2));
            //$("#SalaryDeductionSum_" + i.toString()).html((DeductionSum + lopAmount).toFixed(2));
            $("#SalaryDeductionSum_" + i.toString()).html((DeductionSum).toFixed(2));
            if (netPay < 0) {
                netPay = 0
            }
            $("#TotalSalary_" + i.toString()).html(netPay.toFixed(2));
            $("#SalaryInWords_" + i.toString()).html(amounToWords(netPay.toFixed(2)));
        })

    }

    var daysValidation = function () {
        var SalaryTypeKey = $("#SalaryTypeKey").val();
        var daysInMonth = $("#DaysInMonth").val()
        daysInMonth = parseInt(daysInMonth) ? parseInt(daysInMonth) : 0
        var $workedDays = $("#NoOfDaysWorked")
        var $AbsentDays = $("#AbsentDays")
        var $workingDays = $("#TotalWorkingDays")
        var $BaseWorkingDays = $("#BaseWorkingDays")
        var workingDaysVal = $workingDays.val()
        workingDaysVal = parseInt(workingDaysVal) ? parseInt(workingDaysVal) : 0
        var workedDaysVal = $workedDays.val()
        workedDaysVal = parseInt(workedDaysVal) ? parseInt(workedDaysVal) : 0

        var absentDaysVal = $AbsentDays.val()
        absentDaysVal = parseInt(absentDaysVal) ? parseInt(absentDaysVal) : 0

        var baseWorkingDays = $BaseWorkingDays.val()
        baseWorkingDays = parseInt(baseWorkingDays) ? parseInt(baseWorkingDays) : 0

        if (SalaryTypeKey == Resources.SalaryTypeMonthly) {
            //if (workingDaysVal > daysInMonth) {
            //    $workingDays.val(daysInMonth)
            //    if (workedDaysVal > workingDaysVal) {
            //        $workedDays.val(workingDaysVal)
            //    }
            //}
            //else {
            //    if (workedDaysVal > workingDaysVal) {
            //        $workedDays.val(workingDaysVal)
            //    }
            //}
            if (absentDaysVal > baseWorkingDays) {
                $AbsentDays.val(baseWorkingDays)
            }
        }
        else {
            if (workedDaysVal > daysInMonth) {
                $workedDays.val(daysInMonth)
            }
            else {
                $workedDays.val(workedDaysVal)
            }
        }
    }


    var calculateOverTimeDayAmount = function () {
        debugger;

        var OverTimeDays = $("#AdditionalDayAmount").val();
        OverTimeDays = parseFloat(OverTimeDays) ? parseFloat(OverTimeDays) : 0;

        var OverTimeAmount = $("input#AdditionalDayCount").val();
        OverTimeAmount = parseFloat(OverTimeAmount) ? parseFloat(OverTimeAmount) : 0;


        var TotalOvertimeAmount = OverTimeAmount * OverTimeDays;
        $("#AdditionalDayTotalAmount").val(parseFloat(TotalOvertimeAmount.toFixed(Resources.RoundToDecimalPostion)).toString());
        CalculateSalary();
    }

    var calculateNetSalary = function () {
        var GrossSalary = $("#GrossSalary").val();
        GrossSalary = parseFloat(GrossSalary) ? parseFloat(GrossSalary) : 0;
        var TotalExcludeEarnings = $("#tblSalaryDetails tbody li:not([data-include=true])").find("[data-earnings]").toArray().reduce(function (sum, element) {
            var amount = $(element).val();
            if (isNaN(sum)) sum = 0;
            return sum + Number(amount);
        }, 0);
        var TotalExcludeDeductions = $("#tblSalaryDetails tbody li:not([data-include=true])").find("[data-deductions]").toArray().reduce(function (sum, element) {
            var amount = $(element).val();
            if (isNaN(sum)) sum = 0;
            return sum + Number(amount);
        }, 0);

        var TotalEarnings = $("#TotalEarnings").val();
        var TotalDeductions = $("#TotalDeductions").val();
        TotalEarnings = parseFloat(TotalEarnings) ? parseFloat(TotalEarnings) : 0;
        TotalDeductions = parseFloat(TotalDeductions) ? parseFloat(TotalDeductions) : 0;

        var IsFixed = $("#IsFixed").val();
        IsFixed = IsFixed ? JSON.parse(IsFixed.toLowerCase()) : true;
        if (!IsFixed) {
            TotalSalary = GrossSalary + TotalExcludeEarnings - TotalExcludeDeductions;
        }
        else {
            TotalSalary = TotalEarnings - TotalDeductions;
        }
        $("#TotalSalary").val(parseFloat(TotalSalary.toFixed(2)).toString());
    }

    var getOtherAmounts = function (json) {
        $("#repeaterEarning").repeater(
            {
                show: function () {
                    $(this).slideDown();
                    $("[id*=RowKey]", $(this)).val("0")
                    $("[id*=IsAddition]", $(this)).val(true)
                    EmployeeSalary.CalculateGrossSalary();
                    EmployeeSalary.CalculateRowSum();
                    EmployeeSalary.CalculateNetSalary();
                },

                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden][id*=RowKey]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deletePurchaseOrderDetailsItem($(hidden).val(), $(this), remove, json["PurchaseOrderDetails"]);
                    }
                    else {
                        $(this).slideUp(remove);

                    }
                    setTimeout(function () {
                        EmployeeSalary.CalculateGrossSalary();
                        EmployeeSalary.CalculateRowSum();
                        EmployeeSalary.CalculateNetSalary();
                    }, 500);
                },
                data: json,
                repeatlist: 'EmployeeSalaryOtherEarnings',
                submitButton: '',
            });
        $("#repeaterDeduction").repeater(
            {
                show: function () {
                    $(this).slideDown();
                    $("[id*=RowKey]", $(this)).val("0")
                    $("[id*=IsAddition]", $(this)).val(false)
                    //EmployeeSalary.CalculateLOP();
                    EmployeeSalary.CalculateRowSum();
                    EmployeeSalary.CalculateNetSalary();
                },

                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden][id*=RowKey]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deletePurchaseOrderDetailsItem($(hidden).val(), $(this), remove, json["PurchaseOrderDetails"]);
                    }
                    else {
                        $(this).slideUp(remove);

                    }
                    setTimeout(function () {
                        EmployeeSalary.CalculateGrossSalary();
                        EmployeeSalary.CalculateRowSum();
                        EmployeeSalary.CalculateNetSalary();
                    }, 500);
                },
                data: json,
                repeatlist: 'EmployeeSalaryOtherDeductions',
                submitButton: '',
            });
    }

    var calculateOverTimeAmount = function () {
        var month = AppCommon.ParseMMMYYYYDate($("input#SalaryMonth").val());
        var MonthKey = moment(month).month() + 1;
        var YearKey = moment(month).year();
        var OvertimePerAHour = $("#MonthlySalary").val();
        OvertimePerAHour = parseFloat(OvertimePerAHour) ? parseFloat(OvertimePerAHour) : 0;
        var SalaryTypeKey = $("#SalaryTypeKey").val();
        SalaryTypeKey = parseFloat(SalaryTypeKey) ? parseFloat(SalaryTypeKey) : 0;
        var BaseWorkingDays = $("#BaseWorkingDays").val();
        BaseWorkingDays = parseFloat(BaseWorkingDays) ? parseFloat(BaseWorkingDays) : 0;
        var BaseWorkingHours = $("#BaseWorkingHours").val();
        BaseWorkingHours = parseFloat(BaseWorkingHours) ? parseFloat(BaseWorkingHours) : 0;

        var OverTimeHours = $("input#OverTimeHours").val();
        OverTimeHours = OverTimeHours != "" ? parseFloat(OverTimeHours) : 0;

        if (SalaryTypeKey == Resources.SalaryTypeMonthly)
            OvertimePerAHour = (OvertimePerAHour / (BaseWorkingDays == 0 ? 1 : BaseWorkingDays));
        if (SalaryTypeKey == Resources.SalaryTypeMonthly || SalaryTypeKey == Resources.SalaryTypeDaily)
            OvertimePerAHour = (OvertimePerAHour / (BaseWorkingHours == 0 ? 1 : BaseWorkingHours));

        OvertimePerAHour = OvertimePerAHour ? OvertimePerAHour : 0;
        $("#OvertimePerAHour").val(parseFloat(OvertimePerAHour.toFixed(Resources.RoundToDecimalPostion)).toString());

    }



    var editPaymentPopup = function (_this, IsEdit) {
        var id = _this;

        var obj = {};
        obj.id = _this;
        if (IsEdit) {
            url = $("#hdnAddEditEmployeeSalaryPaymentEdit").val() + "?" + $.param(obj);
        }
        else {
            url = $("#hdnAddEditEmployeeSalaryPayment").val() + "/" + id;
        }

        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            formAction: $("#hdnAddEditEmployeeSalaryPayment").val(),
            load: function () {

            },
            rebind: function (result) {
                window.location.reload();
            }
        }, url);
    }

    return {
        GetEmployeeSalaries: getEmployeeSalaries,
        GetEmployeeSalaryDetails: getEmployeeSalaryDetails,
        GetQuickEmployeesSalary: getQuickEmployeesSalary,
        FormSubmit: formSubmit,
        //CalculateLOP: calculateLOP,
        GetSalaryComponentsByMonth: getSalaryComponentsByMonth,
        GetOperationByAdditionalComponent: getOperationByAdditionalComponent,
        SetExcelSheelValue: setExcelSheelValue,
        ChangeOtherAmountStyle: changeOtherAmountStyle,
        GeneratePayslip: generatePayslip,
        GetEmployeesByBranchId: getEmployeesByBranchId,
        SingleProcessClickEvent: singleProcessClickEvent,
        MultipleProcessClickEvent: multipleProcessClickEvent,
        SinglePayslipClickEvent: singlePayslipClickEvent,
        MultiplePayslipClickEvent: multiplePayslipClickEvent,
        ModifyPayslip: modifyPayslip,
        GetBalance: getBalance,
        DeductAdvance: deductAdvance,
        DaysValidation: daysValidation,
        CalculateRowSum: calculateRowSum,
        CalculateNetSalary: calculateNetSalary,
        GetOtherAmounts: getOtherAmounts,
        CalculateOverTimeDayAmount: calculateOverTimeDayAmount,
        CalculateGrossSalary: calculateGrossSalary,
        CalculateOverTimeAmount: calculateOverTimeAmount,
        EditPaymentPopup: editPaymentPopup
    }

}());

//Employee Salary
function deleteEmployeeSalary(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_EmployeeSalary,
        actionUrl: $("#hdnDeleteEmployeeSalary").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function deleteEmployeeSalaryDetail(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_EmployeeSalaryDetails,
        actionUrl: $("#hdnDeleteEmployeeSalaryDetail").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            location.reload();
        }
    });
}

function deleteEmployeeSalaryPayments(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_EmployeeSalary,
        actionUrl: $("#hdnDeleteSalaryPayment").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function ModifyModel(data) {
    var SalaryComponentData = [], EmployeeSalaryDetailData = [];
    $form = $("form");
    var $additionalComponents = $form.find('[data-repeater-item]');
    var $salaryComponents = $form.find('[salary-component-item]');

    $salaryComponents.each(function () {
        var JsonObject = {};
        $(this).find('input, select, textarea').each(function () {
            if ($(this).val() != "" && $(this).attr("id") != undefined) {
                var matches = $(this).attr('name').match(/].(.*)/)[0].replace('].', '');
                var name = matches.replace('][', '').replace(']', '');
                JsonObject[name] = $(this).val();
            }

        })
        JsonObject["SalaryComponentTypeName"] = $("legend#SalaryComponentTypeName", $(this).closest("fieldset")[0]).html();
        if (JsonObject["SalaryComponentTypeName"] != "") {
            SalaryComponentData.push(JsonObject);
        }

    })

    data["SalaryComponents"] = SalaryComponentData;
    return data;
}

function PivotJSON(dataArray, rowIndex, rowIndex1, dataIndex) {
    var result = {}, ret = [];
    var newRows = [];
    for (var i = 0; i < dataArray.length; i++) {

        if (!result[dataArray[i][rowIndex]]) {
            result[dataArray[i][rowIndex]] = {};
        }

        result[dataArray[i][rowIndex]][0] = dataArray[i][dataIndex];

        if (newRows.indexOf(dataArray[i][rowIndex1]) == -1 || dataArray[i][colIndex] == "") {
            newRows.push(dataArray[i][rowIndex1] ? dataArray[i][rowIndex1].toString() : "");
        }
    }

    var item = [];


    //Add content 
    var j = 0;
    var k = 1;
    var p = 0;
    var FirstItem = 0, LastItem = 0, colMonthly = 0, colDedusction = 0, currentHead = 0;
    var GroupHead = "";
    var letter = "C";
    var TotalSum = [];
    for (var key in result) {

        var count = Object.keys(result).length;

        if (newRows[j] != "0") {
            if (FirstItem == 0) {
                FirstItem = k;
            }
        }
        else if (j != 0) {
            LastItem = k - 1;
        }

        if (newRows[j] == "" && LastItem != 0) {
            GroupHead = key;
        }
        if (FirstItem != 0 && LastItem != 0) {
            item = [];
            item.push("-", "TOTAL " + Object.keys(result)[currentHead]);
            item.push("=SUM(" + letter + FirstItem.toString() + ":" + letter + LastItem.toString() + ")");

            ret.push(item);
            FirstItem = 0;
            LastItem = 0;
            k++;

            if (p == 0) {
                colMonthly = k - 1;
            }
            else if (p == 1) {
                colDeduction = k - 1;
            }
            if (p == 1) {

                item = [];
                item.push("*", "NET MONTHLY SALARY");
                item.push("=" + letter + colMonthly.toString() + "-" + letter + colDeduction.toString());
                k++;
                ret.push(item);
            }
            else {
                TotalSum.push(k - 1);
            }
            p++;
            currentHead = j;

        }

        item = [];
        item.push(newRows[j].toString() || "-");
        item.push(key);
        item.push(result[key][0].toString() || "-");
        ret.push(item);
        j++; k++;
    }
    if (j == count) {
        item = [];
        LastItem = k - 1;
        item.push("-", "TOTAL " + Object.keys(result)[currentHead]);
        item.push("=SUM(" + letter + FirstItem.toString() + ":" + letter + LastItem.toString() + ")");
        ret.push(item);
        TotalSum.push(k);
    }

    item = [];
    item.push("*", "TOTAL ")
    var Formula = "=";
    TotalSum.forEach(function (item) {
        Formula = Formula + letter + item + "-";
    });
    item.push(Formula.substring(0, Formula.length - 1));
    ret.push(item);

    return ret
}


function printDiv(div) {
    var mode = 'iframe'; //popup
    var close = mode == "popup";
    var options = { mode: mode, popClose: close };
    $(div).printArea(options);
}

//Quick Salary

function ModifyMultipleModel(Json, Data, DataKeys) {
    var multiJson = [];
    var KeyHeadNames = $.extend(true, [], Json["EmployeeSalaryDetails"]);

    $.each(Data, function (i) {
        var newJson = $.extend(true, {}, Json);
        newJson["EmployeeKey"] = parseInt(Data[i]["EmployeeKey"]) ? parseInt(Data[i]["EmployeTotaleKey"]) : 0;
        newJson["SalaryMasterKey"] = parseInt(Data[i]["SalaryMasterKey"]) ? parseInt(Data[i]["SalaryMasterKey"]) : 0;

        newJson["EmployeeSalaryDetails"] = [];
        KeyHeadNames.forEach(function (arrItem) {
            var item = {}, SalaryHead = arrItem["SalaryHeadName"];
            item = $.extend(true, {}, arrItem);
            item["SalaryHeadKey"] = parseInt(DataKeys[SalaryHead]) ? parseInt(DataKeys[SalaryHead]) : 0;
            item["Amount"] = parseFloat(DataKeys[SalaryHead]) ? parseFloat(Data[i][SalaryHead]) : 0;
            newJson["EmployeeSalaryDetails"].push(item);

        })
        newJson["LoanAmount"] = parseFloat(Data[i]["LoanAmount"]) ? parseFloat(Data[i]["LoanAmount"]) : 0;
        var Overtime = Data[i]["Overtime"] && Data[i]["Overtime"] != "" ? Data[i]["Overtime"].split("/")[0] : 0;
        newJson["OvertimeAmount"] = parseFloat(Overtime) ? parseFloat(Overtime) : 0;
        newJson["OtherAmount"] = parseFloat(Data[i]["OtherAmount"]) ? Math.abs(Data[i]["OtherAmount"]) : 0;
        newJson["OtherAmountType"] = parseFloat(Data[i]["OtherAmount"]) < 0 ? false : true;
        newJson["DateTimeNow"] = AppCommon.JsonDateToNormalDate(newJson["DateTimeNow"]);
        newJson["SalaryMonth"] = AppCommon.JsonDateToNormalDate(newJson["SalaryMonth"]);
        newJson["SalaryMonthKey"] = parseInt(newJson["SalaryMonthKey"]);
        newJson["SalaryYearKey"] = parseInt(newJson["SalaryYearKey"]);
        newJson["MonthlySalary"] = parseFloat(Data[i]["MonthlySalary"]) ? parseFloat(Data[i]["MonthlySalary"]) : 0;
        newJson["TotalSalary"] = parseFloat(Data[i]["TotalSalary"]) ? parseFloat(Data[i]["TotalSalary"]) : 0;
        newJson["NoOfDaysWorked"] = parseFloat(Data[i]["NoOfDaysWorked"]) ? parseFloat(Data[i]["NoOfDaysWorked"]) : 0;
        multiJson.push(newJson);
    });
    return multiJson;
}

function EmployeesSalaryData(data, totalDays, colModels, colHeaders, groupParam) {
    var ret = [], hiddenitem = [];
    colModels.push(createGridColum("SalaryMasterKey", null, null, null, true))
    colModels.push(createGridColum("EmployeeKey", null, null, true, true))
    colModels.push(createGridColum("SalaryStatusKey", null, null, null, true))
    colModels.push(createGridColum("EmployeeCode", 50))
    colModels.push(createGridColum("EmployeeName"))
    colModels.push(createGridColum("NoOfDaysWorked", 50))
    colHeaders.push("Row Key")
    colHeaders.push("Emp Key")
    colHeaders.push("Stat Key")
    colHeaders.push("Emp Code")
    colHeaders.push("Emp Name")
    colHeaders.push("Days Present")
    groupParam.payLength = 0, groupParam.dedLength = 0;
    for (var i = 0; i < data.length; i++) {
        var dataItem = data[i], item = [], grossPay = 0, grossDed = 0, paySum = 0, dedSum = 0; totSalary = 0, netPay = 0;

        item["SalaryMasterKey"] = dataItem["SalaryMasterKey"];
        hiddenitem["EmployeeKey"] = "-1";
        item["EmployeeKey"] = dataItem["EmployeeKey"];
        item["SalaryStatusKey"] = dataItem["SalaryStatusKey"];
        item["EmployeeCode"] = dataItem["EmployeeCode"];
        item["EmployeeName"] = dataItem["EmployeeName"];
        item["NoOfDaysWorked"] = dataItem["NoOfDaysWorked"];
        var paymentModel = $.extend(true, [], dataItem["EmployeeSalaryDetails"]);
        var dedutionModel = $.extend(true, [], dataItem["EmployeeSalaryDetails"]);
        paymentModel = paymentModel.filter(function (modelItem) {
            return modelItem.SalaryHeadTypeKey == Resources.SalaryHeadTypeMonthlyPayments;
        });
        dedutionModel = dedutionModel.filter(function (modelItem) {
            return modelItem.SalaryHeadTypeKey != Resources.SalaryHeadTypeMonthlyPayments;
        });
        for (var j = 0; j < paymentModel.length; j++) {
            item[paymentModel[j]["SalaryHeadName"]] = paymentModel[j]["Amount"];
            var amount = paymentModel[j]["Amount"] != "" ? parseFloat(paymentModel[j]["Amount"]) : 0;
            paySum = paySum + amount;
            grossPay = grossPay + amount;
            if (i == 0) {
                hiddenitem[paymentModel[j]["SalaryHeadName"]] = paymentModel[j]["SalaryHeadKey"];
                colModels.push(createGridColum(paymentModel[j]["SalaryHeadName"], 60));
                colHeaders.push(paymentModel[j]["SalaryHeadName"]);
                if (groupParam.payStart == "") {
                    groupParam.payStart = paymentModel[j]["SalaryHeadName"]
                }
                groupParam.payLength++;

            }
        }
        var overTimeAmount = dataItem["OvertimeAmount"] ? dataItem["OvertimeAmount"] : 0;
        var overtimeHours = dataItem["OverTimeHours"] ? dataItem["OverTimeHours"] : 0;
        item["Overtime"] = parseFloat(overTimeAmount.toFixed(2)).toString() + "/" + overtimeHours.toString();
        paySum = paySum + overTimeAmount;
        if (i == 0) {
            colModels.push(createGridColum("Overtime", 80))
            colHeaders.push("Overtime");
            groupParam.payLength++;
        }



        for (var k = 0; k < dedutionModel.length; k++) {
            item[dedutionModel[k]["SalaryHeadName"]] = dedutionModel[k]["Amount"];
            var amount = dedutionModel[k]["Amount"] != "" ? parseFloat(dedutionModel[k]["Amount"]) : 0;
            dedSum = dedSum + amount;
            grossDed = grossDed + amount;
            if (i == 0) {
                hiddenitem[dedutionModel[k]["SalaryHeadName"]] = dedutionModel[k]["SalaryHeadKey"];
                colModels.push(createGridColum(dedutionModel[k]["SalaryHeadName"], 60));
                colHeaders.push(dedutionModel[k]["SalaryHeadName"]);
                if (groupParam.dedStart == "") {
                    groupParam.dedStart = dedutionModel[k]["SalaryHeadName"]
                }
                groupParam.dedLength++;

            }
        }
        item["LoanAmount"] = dataItem["LoanAmount"] ? dataItem["LoanAmount"] : 0;
        if (i == 0) {
            colModels.push(createGridColum("LoanAmount", 50))
            colHeaders.push("LoanAmount");
            groupParam.dedLength++;
        }
        dedSum = dedSum + item["LoanAmount"];


        var fixedGross = grossPay - grossDed
        item["FixedGross"] = parseFloat(fixedGross.toFixed(2)).toString()

        var daysWorked = dataItem["NoOfDaysWorked"], loanAmount = dataItem["LoanAmount"], otherAmount = dataItem["OtherAmount"];

        daysWorked = daysWorked && daysWorked != "" ? parseInt(daysWorked) : 0;
        loanAmount = loanAmount && loanAmount != "" ? parseFloat(loanAmount) : 0;
        otherAmount = otherAmount && otherAmount != "" ? parseFloat(otherAmount) : 0;
        otherAmount = dataItem["OtherAmountType"] ? otherAmount : -otherAmount;
        totalDays = totalDays != "" && totalDays != "0" ? parseInt(totalDays) : 1;
        netPay = fixedGross * daysWorked / totalDays;
        var lopAmount = fixedGross - netPay;
        netPay = parseFloat((fixedGross * daysWorked / totalDays).toFixed(2)).toString()

        item["LOP"] = parseFloat(lopAmount.toFixed(2)).toString();
        if (i == 0) {
            colModels.push(createGridColum("LOP", 80))
            colHeaders.push("LOP");
            groupParam.dedLength++;
        }
        dedSum = dedSum + lopAmount;


        totSalary = paySum - dedSum;

        item["TotalPayment"] = parseFloat(paySum.toFixed(2)).toString();
        item["TotalDeduction"] = parseFloat(dedSum.toFixed(2)).toString();
        item["MonthlySalary"] = parseFloat(totSalary.toFixed(2)).toString();
        item["OtherAmountType"] = dataItem["OtherAmountType"];
        item["OtherAmount"] = dataItem["OtherAmount"];
        item["TotalSalary"] = parseFloat((netPay - loanAmount + otherAmount + overTimeAmount).toFixed(2)).toString();
        ret.push(item);

    }
    ret.push(hiddenitem);

    colModels.push(createGridColum("FixedGross", 80))
    colModels.push(createGridColum("TotalPayment", 80))
    colModels.push(createGridColum("TotalDeduction", 80))
    colModels.push(createGridColum("MonthlySalary", 80))
    colModels.push(createGridColum("OtherAmountType", 60, null, null, true))
    colModels.push(CreateOtherAmountColumn("OtherAmount", 60))
    colModels.push(createGridColum("TotalSalary", 80))
    colModels.push(createGridColum("Action", 150, quickSalaryAction))

    colHeaders.push("Fixed");
    colHeaders.push("Pay Sum")
    colHeaders.push("Ded Sum")
    colHeaders.push("Total Amt")
    colHeaders.push("Other");
    colHeaders.push("Other");
    colHeaders.push("Net Pay")
    colHeaders.push("Action")
    return ret

}

function formatSalaryHeadColumn(cellValue, options, rowdata, action) {
    return "<span>" + cellValue + "</span><input type='hidden' value='" + rowdata.SalaryHeadKey + "'>"
}

function CreateOtherAmountColumn(name, width) {
    var obj = {};
    if (name) {
        obj["name"] = name;
        obj["index"] = name;
    }
    if (width)
        obj["width"] = width;

    obj["editable"] = true;
    obj["cellEdit"] = true;
    obj["editoptions"] = {
        dataEvents: [
            {
                type: 'input',
                fn: function (e) {
                    OtherAmountChangeEvent($(this))
                }
            },
            {
                type: 'blur',
                fn: function (e) {
                    var row = $(this).closest("tr");
                    var col = $(row).find("td")[0];
                    $(col).trigger("click")
                }
            }
        ]
    }
    return obj;
}

function createGridColum(name, width, formatter, key, hidden) {
    var obj = {};
    obj["name"] = name;
    if (width)
        obj["width"] = width;
    if (formatter)
        obj["formatter"] = formatter;
    if (key)
        obj["key"] = key;
    if (hidden)
        obj["hidden"] = hidden;

    return obj;
}


function quickSalaryAction(cellValue, options, rowdata, action) {
    var temp = "'" + rowdata.SalaryMasterKey + "'";
    var tempPayKey = "'" + rowdata.SalaryPaymentKey + "'";
    var payslipUrl = $("#hdnSalaryPaySlip").val() + AppCommon.EncodeQueryString("Ids=" + rowdata.SalaryMasterKey);
    var html = '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" style="min-width:0px" title="' + Resources.Process + '" onclick="EmployeeSalary.SingleProcessClickEvent(' + rowdata.EmployeeKey + ')" ><i class="fa fa-floppy-o" aria-hidden="true"></i></a>'
    if (rowdata.SalaryStatusKey == Resources.ProcessStatusApproved) {
        html = html + '<a class="btn btn-warning btn-sm" data-inputtype="submit" style="min-width:0px" data-payment-modal="" title="' + Resources.Payment + '" data-href="' + $("#hdnAddEditEmployeeSalaryPayment").val() + '/' + rowdata.SalaryMasterKey + '" ><i class="fa fa-credit-card" aria-hidden="true"></i></a>';
        html = html + '<a class="btn btn-info btn-sm" style="min-width:0px" title="' + Resources.PaySlip + '" href="' + payslipUrl + '") target="_blank"><i class="fa fa-file" aria-hidden="true"></i></a>';

    }



    html = html + "</div>"

    return html;
}

function OtherAmountChangeEvent(_this) {
    var row = $(_this).closest("tr");
    var month = AppCommon.ParseMMMYYYYDate($("input#SalaryMonth").val());
    var salaryMonth = moment(month).month() + 1;
    var salaryYear = moment(month).year();
    var totalDays = (new Date(salaryYear, salaryMonth, 0)).getDate();
    var totalSalary = $("td[aria-describedby=gridQuick_MonthlySalary]", $(row)).html();
    var noOfDaysWorked = $("td[aria-describedby=gridQuick_NoOfDaysWorked]", $(row)).html();
    totalDays = totalDays && totalDays != "" ? parseInt(totalDays) : 1;
    noOfDaysWorked = noOfDaysWorked != "" ? parseFloat(noOfDaysWorked) : 0;
    $("td[aria-describedby=gridQuick_TotalSalary]", $(row)).html("");
    var otherAmount = $(_this).val();
    otherAmount = otherAmount != "" && otherAmount != "-" ? parseFloat(otherAmount) : 0;
    $(_this).attr("value", $(_this).val())
    totalSalary = totalSalary != "" ? parseFloat(totalSalary) : 0;
    var netPay = totalSalary * noOfDaysWorked / totalDays;
    var loanAmount = $("td[aria-describedby=gridQuick_LoanAmount]", $(row)).html();
    loanAmount = loanAmount && loanAmount != "" ? parseFloat(loanAmount) : 0;
    var overtimeAmount = $("td[aria-describedby=gridQuick_Overtime]", $(row)).html();
    overtimeAmount = overtimeAmount != "" ? overtimeAmount.split("/")[0] : 0;
    overtimeAmount = overtimeAmount && overtimeAmount != "" ? parseFloat(overtimeAmount) : 0;


    var totalAmount = netPay - loanAmount + otherAmount + overtimeAmount;
    $("td[aria-describedby=gridQuick_TotalSalary]", $(row)).html(parseFloat(totalAmount.toFixed(2)).toString());
}

function setQuickSalaryParam(jsonData) {
    var month = AppCommon.ParseMMMYYYYDate($("input#SalaryMonth").val());
    jsonData["SalaryMonthKey"] = moment(month).month() + 1;
    jsonData["SalaryYearKey"] = moment(month).year();
    jsonData["BranchKey"] = $('#BranchKey').val() != "" ? parseInt($('#BranchKey').val()) : 0;
}

function formSubmitMultipleData(json, data, lastRowData) {
    $(".section-content").mLoading();
    setTimeout(function () {
        $form = $("form");
        if ($($form).valid()) {

            var result = ModifyMultipleModel(json, data, lastRowData);

            var dataurl = $($form)[0].action;
            var response = AjaxHelper.ajax("POST", dataurl,
                {
                    model: result
                });
            if (response.IsSuccessful == true) {

                toastr.success(Resources.Success, response.Message);
                setQuickSalaryParam(response);
                EmployeeSalary.GetQuickEmployeesSalary(response, true);

            }
            else {
                $("[data-valmsg-for=error_msg]").html(response.Message);
            }

        }
        $(".section-content").mLoading("destroy");
    }, 1000);
}

//Common for Salary

function ModifyPayslipData(Json) {
    var PayslipData = [];
    $.each(Json, function (i) {
        var data = Json[i];
        var PaymentSum = 0, DeductionSum = 0, ComponentSum = 0;
        $.each(data["SalaryPayments"], function (i) {

            var Amount = data["SalaryPayments"][i].Amount;
            if (Amount != "" && Amount != "0") {
                PaymentSum = PaymentSum + Amount
            }
            else {
                delete data["SalaryPayments"][i];
            }
        })
        $.each(data["SalaryDeductions"], function (j) {
            var Amount = data["SalaryDeductions"][j].Amount;
            if (Amount != "" && Amount != "0") {
                DeductionSum = DeductionSum + Amount
            }
            else {
                delete data["SalaryDeductions"][j];
            }

        })
        $.each(data["SalaryAdvances"], function (j) {
            var Amount = data["SalaryAdvances"][j].Amount;
            if (Amount != "" && Amount != "0") {
                DeductionSum = DeductionSum + Amount
            }
            else {
                delete data["SalaryAdvances"][j];
            }

        })
        $.each(data["SalaryComponents"], function (k) {
            var Amount = data["SalaryComponents"][k].AmountUnit;
            var Operation = data["SalaryComponents"][k].OperationType;
            if (Amount != "" && Amount != "0") {
                if (Operation == "A") {
                    ComponentSum = ComponentSum + Amount
                }
                else {
                    ComponentSum = ComponentSum - Amount
                }
            }
            else {
                delete data["SalaryComponents"][k];
            }

        })

        var TotalSum = (PaymentSum - DeductionSum);
        var month = AppCommon.ParseMMMYYYYDate($("input#SalaryMonth").val());
        var salaryMonth = moment(month).month() + 1;
        var salaryYear = moment(month).year();
        var totalDays = (new Date(salaryYear, salaryMonth, 0)).getDate();
        var daysWorked = parseFloat(data["NoOfDaysWorked"]) ? parseFloat(data["NoOfDaysWorked"]) : 0;
        var netPay = (TotalSum * daysWorked / totalDays);
        netPay = parseFloat((netPay + ComponentSum).toFixed(2)).toString()
        //var PaidAmount = data["PaidAmount"];
        //var BalanceAmount = data["BalanceAmount"];
        //PaidAmount = PaidAmount != "" ? parseFloat(PaidAmount) : 0;
        data["SalaryPaymentTotal"] = PaymentSum.toFixed(2)
        data["SalaryDeductionTotal"] = DeductionSum.toFixed(2)
        data["TotalSalary"] = netPay;
        data["SalaryInWords"] = AppCommon.AmounToWords(netPay)
        //data["BalanceAmount"] = Math.abs(BalanceAmount);
        //data["BalanceText"] = BalanceAmount < 0 ? "Balance" : "Advance";
        //data["BalanceColor"] = BalanceAmount < 0 ? "text-red" : "text-green";
        //data["PaymentDate"] = AppCommon.JsonDateToNormalDate(data["PaymentDate"]);
        data["DateOfJoining"] = AppCommon.JsonDateToNormalDate(data["DateOfJoining"]);
        PayslipData.push(data);
    })

    return PayslipData;
}

function generatePayslip(Json) {
    var filepath = Resources.ServerPath + "Templates/SalaryPayslip.html"
    var html = "";
    var payslipData = ModifyPayslipData(Json);
    $.ajaxSetup({ async: false });
    $.get(filepath, function (data) {
        html = data;
        Mustache.Formatters = {
            "multiply": function (value, multiplier) {
                return value * multiplier;
            }
        }
        html = Mustache.to_html(html, { PaySlipData: payslipData });
        html = html.replace("script", "div").replace("script", "div").replace('type="text/html"', "");

    });
    return html;
}

function generatePayslipPDF(htmls, item, IsUpload) {
    var div = $("<div/>");
    $(div).attr("id", "dvPayslipPdf").css("background", "#fff");
    $("body").append(div);
    var htmlLength = htmls.length, i = 0;
    var doc = new jsPDF("p", "mm", "a4");
    htmls.forEach(function (html) {
        $("#dvPayslipPdf").html("").html(html);
        var divHeight = $(div).height();
        var divWidth = $(div).width();
        var ratio = divHeight / divWidth;
        html2canvas($("#dvPayslipPdf"), {
            onrendered: function (canvas) {
                try {
                    var ctx = canvas.getContext('2d');

                    ctx.webkitImageSmoothingEnabled = false;
                    ctx.mozImageSmoothingEnabled = false;
                    ctx.imageSmoothingEnabled = false;
                    ctx.font = "14px san-serif";

                    var imgData = canvas.toDataURL("image/png");

                    var width = doc.internal.pageSize.width;
                    var height = doc.internal.pageSize.height;
                    height = ratio * width;
                    if (i > 0)
                        doc.addPage();

                    doc.addImage(imgData, 'PNG', 10, 10, width - 20, height - 20);

                    //doc.save('sample-file.pdf');
                    if (IsUpload) {
                        var pdfFile = AppCommon.BlobToFile(doc.output('blob'), item.toString(), 'application/pdf'); //returns raw body of resulting PDF returned as a string as per the plugin documentation.
                        var pdfFiles = [];
                        pdfFiles["file"] = pdfFile;
                        var response = AjaxHelper.ajaxWithFile("file", "POST", $("#hdnUploadPdfPayslip").val(),
                            { file: pdfFiles }
                        );
                    }
                    if (htmlLength - 1 == i) {
                        if (!IsUpload) {
                            doc.save($("#SalaryMonth").val() + ".pdf");
                        }
                        $("#dvPayslipPdf").remove();
                    }
                    i++;
                }
                catch (e) {

                }

            }
        });
    })
}

function amounToWords(n) {
    var options = {
        mainunit: "Rupees",
        subunit: "Paisa",
        units: ["", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten"],
        teens: ["Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen", "Twenty"],
        tens: ["", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"],
        othersIntl: ["Thousand", "Million", "Billion", "Trillion"]
    }
    var o = options;

    var units = o.units;
    var teens = o.teens;
    var tens = o.tens;
    var othersIntl = o.othersIntl;

    var getBelowHundred = function (n) {
        if (n >= 100) {
            return "greater than or equal to 100";
        };
        if (n <= 10) {
            return units[n];
        };
        if (n <= 20) {
            return teens[n - 10 - 1];
        };
        var unit = Math.floor(n % 10);
        n /= 10;
        var ten = Math.floor(n % 10);
        var tenWord = (ten > 0 ? (tens[ten] + " ") : '');
        var unitWord = (unit > 0 ? units[unit] : '');
        return tenWord + unitWord;
    };

    var getBelowThousand = function (n) {
        if (n >= 1000) {
            return "greater than or equal to 1000";
        };
        var word = getBelowHundred(Math.floor(n % 100));

        n = Math.floor(n / 100);
        var hun = Math.floor(n % 10);
        word = (hun > 0 ? (units[hun] + " Hundred ") : '') + word;

        return word;
    };
    if (isNaN(n)) {
        return "Not a number";
    };

    var word = '';
    var val;
    var word2 = '';
    var val2;
    var b = n.split(".");
    n = b[0];
    d = b[1];
    d = String(d);
    d = d.substr(0, 2);

    val = Math.floor(n % 1000);
    n = Math.floor(n / 1000);

    val2 = Math.floor(d % 1000);
    d = Math.floor(d / 1000);

    word = getBelowThousand(val);
    word2 = getBelowThousand(val2);

    othersArr = othersIntl;
    divisor = 1000;
    func = getBelowThousand;

    var i = 0;
    while (n > 0) {
        if (i == othersArr.length - 1) {
            word = this.numberToWords(n) + " " + othersArr[i] + " " + word;
            break;
        };
        val = Math.floor(n % divisor);
        n = Math.floor(n / divisor);
        if (val != 0) {
            word = func(val) + " " + othersArr[i] + " " + word;
        };
        i++;
    };

    var i = 0;
    while (d > 0) {
        if (i == othersArr.length - 1) {
            word2 = this.numberToWords(d) + " " + othersArr[i] + " " + word2;
            break;
        };
        val2 = Math.floor(d % divisor);
        d = Math.floor(d / divisor);
        if (val2 != 0) {
            word2 = func(val2) + " " + othersArr[i] + " " + word2;
        };
        i++;
    };
    if (word != '') word = (word + ' ' + o.mainunit).toUpperCase()
    if (word2 != '') word2 = (' AND ' + word2 + ' ' + o.subunit).toUpperCase();
    return word + word2;

}
