request = null, removeCols = ["subgrid", "edit"], ApplicationJsonData = [];
var FeePaidorUnPaidSummary = (function () {


    var getFeePaidorUnPaidSummary = function () {

        var JsonData = $("form").serializeToJSON({

        });

        JsonData["DateAddedFrom"] = $("#DateAddedFrom").val();
        JsonData["DateAddedTo"] = $("#DateAddedTo").val();
        var PaidUnpaidKey = $("#PaidUnPaid").val();
        PaidUnpaidKey = parseInt(PaidUnpaidKey) ? parseInt(PaidUnpaidKey) : 0;

        if (JsonData["BranchKey"] == "") {
            JsonData["BranchKey"] = 0;
        }

        $('#dvAttendanceReport').html("");
        $('#dvAttendanceReport').mLoading();
        $.ajax({
            type: "POST",
            url: $("#hdnGetStudentFeePaymentSummaryByDate").val() + "?no-cache=" + new Date().getTime(),
            data: JsonData,
            success: function (data) {
                var resultData = {};
                var xmlDoc = $.parseXML(data);
                data = AppCommon.Xml2Json(xmlDoc)
                resultData = data.Fees;
                if (resultData) {
                    resultData.FullPath = Resources.FullPath;
                    resultData.ApplicationUrl = Resources.ApplicationUrl.replace("~", Resources.FullPath);

                    resultData.FeeDateHead = resultData.Students[0].FeeDetails.map(function (item) {
                        var obj = {};
                        obj.DayNumber = moment(item.FeeDate).date();
                        obj.DayName = moment(item.FeeDate).format('ddd');
                        obj.MonthName = moment(item.FeeDate).format('MMM');
                        obj.Year = moment(item.FeeDate).format('YYYY');
                        return obj;
                    });

                    $(resultData.Students).each(function (i, item) {
                        if (item.FeeDetails) {
                            var TotalPaidFee = item.FeeDetails.reduce(function (sum, item) {
                                return sum + parseFloat(item.FeeAmount);
                            }, 0);
                            TotalPaidFee = parseFloat(TotalPaidFee) ? parseFloat(TotalPaidFee) : 0;
                            item.TotalPaidFee = TotalPaidFee;
                        }

                    });

                    //$(resultData.Students).each(function (i, item) {                      
                       
                    //    this.splice(0, 0, item.FeeDetails)
                    //});

                    if (PaidUnpaidKey == 1) {
                        var PaidList = resultData.Students.filter(function (item) {
                            return parseFloat(item.TotalPaidFee) != 0;
                        });
                        resultData.Students = PaidList;
                    }
                    else if (PaidUnpaidKey == 2) {
                        var UnPaidList = resultData.Students.filter(function (item) {
                            return parseFloat(item.TotalPaidFee) == 0;
                        });
                        resultData.Students = UnPaidList;
                    }




                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnFeePaidSummaryPath").val(),
                        success: function (response) {

                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(resultData);
                            $('#dvAttendanceReport').html(html);
                            $("#tblAttendance").tableHeadFixer({ "left": 6 });
                            $(".btn-attendance").each(function () {
                                $(this).css("color", AppCommon.SetColorByBackgroundIntensity($(this).css("background-color")));
                            })



                        },
                        error: function (xhr) {

                        },
                        complete: function () {

                        }
                    })
                } else {
                    $('#dvAttendanceReport').mLoading("destroy");
                }
            }
        });



    }




    return {
        GetFeePaidorUnPaidSummary: getFeePaidorUnPaidSummary
    }

}());


