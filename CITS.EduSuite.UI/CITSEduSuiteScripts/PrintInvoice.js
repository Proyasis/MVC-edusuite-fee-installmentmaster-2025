
var PrintInvoice = (function () {

    var printReceipt = function (_this, branchKey, ReceiptNumber, type, rebind) {
        
        $(_this).attr("disabled", true)
        var obj = {};
        obj.ReceiptNumber = ReceiptNumber;
        obj.BranchKey = branchKey;
        obj.PrintType = type;
        if (obj.PrintType) {
            $.ajax({
                type: "Get",
                url: $("#hdnPrintReciept").val(),
                datatype: "json",
                data: obj,
                success: function (result) {
                    var xmlDoc = $.parseXML(result);
                    result = AppCommon.Xml2Json(xmlDoc)
                    result = result.Receipt;
                    result.CompanyImageUrl = getRootWebSitePath() + result.CompanyImageUrl
                    result.AmountInWords = AppCommon.AmounToWords(result.AmountToPay.toString())
                    result.BalanceAmount = (parseFloat(result.AccountAmount) - parseFloat(result.PaidAmount)).toFixed(2)
                    var url = $("#hdnReceiptPath").val();
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: url,
                        success: function (response) {

                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            html = template(result);

                            var obj = {}

                            obj.PaperSize = result.PaperSize;
                            obj.PaperOrientation = result.PaperOrientation;
                            obj.CopiesPerPaper = result.CopiesPerPaper;
                            var PaperOrientation = obj.PaperOrientation == Resources.PaperOrientationLandscape ? "landscape" : "portrait";
                            $("").printArea({
                                html: html,
                                load: function (body) {
                                    obj.printContainer = $("#dvPrintContainer", body)
                                    obj.Rotate = $(obj.printContainer).data("rotate");
                                    ChangeSizeOrientation(obj)
                                },
                                rebind: rebind,
                                paperSize: obj.PaperSize,
                                paperOrientation: PaperOrientation
                            });

                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            $(_this).removeAttr("disabled")
                        }
                    })

                }


            });
        }
    }
    var printApplicationForm = function (_this, Id, rebind) {
        var obj = {};
        obj.id = Id;
        if (obj.id) {
            $(".page-content").mLoading();
            $.ajax({
                type: "Get",
                url: $("#hdnPrintApplication").val(),
                datatype: "json",
                data: obj,
                success: function (result) {

                    $.extend(result, result.Application, result.Branch);
                    $.extend(result, result, result.PersonalDetails);
                    var url = $("#hdnApplicationFormPath").val() + "?no-cache=" + new Date().getTime();
                   // result.StudentPhotoPath = "/UploadFiles/Application/" + result.AdmissionNo + "/" + result.StudentPhotoPath;
                    /*result.ApplicantPhoto = "/UploadFiles/Application/" + result.AdmissionNo + "/" + result.StudentPhotoPath;*/
                    result.ApplicantPhoto = result.ApplicantPhoto.substring(2);
                    result.CompanyLogoPath = result.CompanyLogoPath.substring(2);
                    result.YearNow = new Date().getFullYear();
                    result.Gender = result.Gender==1 ? "Male" : "Female";
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: url,
                        success: function (response) {


                            var template = Handlebars.compile(response);
                            AppCommon.HandleBarHelpers();
                            html = template(result);
                            $(".page-content").mLoading("destroy");
                            var obj = {}
                            obj.PaperSize = "A4";
                            obj.PaperOrientation = 1;
                            obj.CopiesPerPaper = 1;
                            $("").printArea({
                                html: html,
                                load: function (body) {
                                    obj.printContainer = $("#dvPrintContainer", body)
                                    ChangeSizeOrientation(obj)
                                },
                                rebind: rebind
                            });


                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            $(_this).removeAttr("disabled")
                        }
                    })

                }


            });
        }
    }


    return {
        PrintReceipt: printReceipt,
        PrintApplicationForm: printApplicationForm
    }
}());

function groupBySum(array, baseKey, f, g) {
    var baseValues = array.map(function (a) { return a[baseKey] }).unique();
    var groups = {};
    baseValues.forEach(function (o) {
        var newObject = array.filter(function (n, p) {
            return n[baseKey] == o;
        })[0];
        var groupValues = g(array);
        var groupObject = jsonConcat(f(newObject), g(newObject));
        for (var key in groupValues) {
            if (!groupObject[key])
                groupObject[key] = array.filter(function (n, p) {
                    return n[baseKey] == o;
                }).reduce(function (sum, item) {
                    return sum + (parseFloat(item[key]) ? parseFloat(item[key]) : 0);
                }, 0).toFixed(Resources.RoundToDecimalPostion);
        }

        groups[o] = groups[o] || [];
        groups[o].push(groupObject);
    });
    return Object.keys(groups).map(function (group) {
        return groups[group][0];
    })
}
function jsonConcat(o1, o2) {
    for (var key in o2) {
        o1[key] = o2[key];
    }
    return o1;
}

function ChangeSizeOrientation(obj) {



    obj.PaperOrientation = parseInt(obj.PaperOrientation) ? parseInt(obj.PaperOrientation) : 0;
    obj.CopiesPerPaper = parseInt(obj.CopiesPerPaper) ? parseInt(obj.CopiesPerPaper) : 1;


    if (obj.PaperSize) {
        var objSize = AppCommon.GetPaperSizeFromName(obj.PaperSize);
        obj.width = objSize.width;
        obj.height = objSize.height;
        obj.margin = objSize.margin;
        obj.unit = objSize.unit;
        var dedMargin = 0;
        dedMargin = objSize.margin;
        dedMargin = obj.CopiesPerPaper > 1 ? dedMargin : 0;
        if (obj.PaperOrientation == Resources.PaperOrientationLandscape || obj.Rotate) {
            var height = (((parseFloat(objSize.width) - parseFloat(objSize.margin) * 2) / obj.CopiesPerPaper) - dedMargin).toString() + objSize.unit;
            var width = parseFloat(objSize.height) + objSize.unit;
            if (obj.Rotate) {
                height = (objSize.width - dedMargin) + objSize.unit
                width = (((parseFloat(objSize.height)) / obj.CopiesPerPaper)).toString() + objSize.unit;
            }
            $(obj.printContainer).height(height)
            $(obj.printContainer).width(width)
        }
        else {
            //var height = ((parseFloat(objSize.height) + ((obj.CopiesPerPaper == 1 ? (parseFloat(objSize.margin) * 2) : 0))) / obj.CopiesPerPaper).toString() + objSize.unit;
            var height = ((parseFloat(objSize.height)) / obj.CopiesPerPaper).toString() + objSize.unit;
            var width = (objSize.width) + objSize.unit
            /*var height = "257mm";*/
            $(obj.printContainer).height(height)
            $(obj.printContainer).width(width)
        }
    }

}
function getRootWebSitePath() {
    return Resources.FullPath + "/UploadedFiles/CompanyLogo/";
}
