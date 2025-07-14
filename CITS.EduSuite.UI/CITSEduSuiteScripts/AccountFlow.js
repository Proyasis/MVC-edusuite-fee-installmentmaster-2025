var AccountFlow = (function () {

    var getDayBook = function (fromDate, toDate, BranchKey) {

        var obj = {};
        obj.fromDate = fromDate;
        obj.toDate = toDate;
        obj.BranchKey = BranchKey;

        $('#dayBookList').html("");
        $(".section-content").mLoading()
        $.ajax({
            type: "POST",
            url: $("#hdnGetDayBook").val(),
            data: obj,
            success: function (data) {
                var resultData = {};
                data = data.map(function (item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
                resultData.DayBook = data;
                var dates = resultData.DayBook.map(function (item, i) {
                    return moment(item.TransactionDate).format("YYYY-MM-DD");
                }).unique();
                resultData.DayBookMaster = dates.map(function (item, i) {
                    var obj = {};
                    obj.DayBookDetails = resultData.DayBook.filter(function (filteritem) {
                        var date = moment(item);
                        var filterdate = moment(filteritem.TransactionDate);
                        return moment(item).date() == moment(filteritem.TransactionDate).date() && moment(item).month() == moment(filteritem.TransactionDate).month() && moment(item).year() == moment(filteritem.TransactionDate).year()
                    });
                    obj.DayBookDetails = obj.DayBookDetails.map(function (subitem, j) {
                        subitem.DebitAmount = (subitem.CashFlowTypeKey == 1 || subitem.CashFlowTypeKey == 3) ? subitem.Amount : null;
                        subitem.CreditAmount = (subitem.CashFlowTypeKey == 2 || subitem.CashFlowTypeKey == 4) ? subitem.Amount : null;
                        if (j == 0)
                            subitem.RowSpan = obj.DayBookDetails.length;
                        else
                            subitem.TransactionDate = null;
                        return subitem;
                    });
                    return obj;
                });
                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: $("#hdnDayBookPath").val() + "?no-cache=" + new Date().getTime(),
                    success: function (response) {
                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        var html = template(resultData);
                        $('#dayBookList').html(html);
                        $(".section-content").mLoading("destroy")
                    },
                    error: function (xhr) {

                    },
                    complete: function () {
                        $(".section-content").mLoading("destroy")
                    }
                })
            }
        });



    }

    return {
        GetDayBook: getDayBook
    }
}())