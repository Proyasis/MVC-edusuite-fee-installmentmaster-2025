var JsonData = [], request = null;
var ViewVideoUpload = (function () {

    var getVideosCategory = function (json) {

        JsonData = json;
        $("#VideosCategoryList").empty();
     
        request = $.ajax({
            url: $("#hdnGetVideoCategory").val(),
            data: JsonData,
            datatype: "json",
            type: "post",
            contenttype: 'application/json; charset=utf-8',
            async: false,
            beforeSend: function () {
                if (request != null) {
                    request.abort();
                }
            },
            success: function (data) {
                $("#VideosCategoryList").html(data);
               

               
                $("#VideosCategoryList").mLoading("destroy");
            },
            error: function (error) {
                if (error.statusText != "abort") {
                    console.log(JSON.stringify(error));
                    $("#VideosCategoryList").mLoading("destroy");
                }
            }
        });

    }

    var getVideosList = function (json, pageIndex, resetPagination,SubjectKey)
    {
        JsonData = json;
        $("#VideossList").empty();

        if (resetPagination)
        {
            JsonData["SubjectKey"] = SubjectKey;
            $("#hdnVideoCategoryKey").val(SubjectKey);
        }
        else
        {
            JsonData["SubjectKey"] = $("#SubjectKey").val();

        }
        $("#VideossList").mLoading();

     
        JsonData["PageIndex"] = pageIndex ? pageIndex : 1;
        JsonData["PageSize"] = 10;
        request = $.ajax({
            url: $("#hdnGetVideos").val(),
            data: JsonData,
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
                $("#VideossList").html(data);

                if (resetPagination) {
                    VideosPagination();
                }
                $("#TotalRecords").html($("#hdnVideosTotalRecords").val());
                $("#VideossList").mLoading("destroy");
            },
            error: function (error) {
                if (error.statusText != "abort") {
                    console.log(JSON.stringify(error));
                    $("#VideossList").mLoading("destroy");
                }
            }
        });
    }



    var getVideosDetailsList = function (json, pageIndex, resetPagination, VideoKey) {
        JsonData = json;
        $("#VideossList").empty();

       
        $("#VideossList").mLoading();

        JsonData["RowKey"] = VideoKey;
 
        request = $.ajax({
            url: $("#hdnGetVideosDetails").val(),
            data: JsonData,
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
                $("#VideossList").html(data);

                //if (resetPagination) {
                //    VideosPagination();
                //}
                //$("#TotalRecords").html($("#hdnVideosTotalRecords").val());
                $("#VideossList").mLoading("destroy");
            },
            error: function (error) {
                if (error.statusText != "abort") {
                    console.log(JSON.stringify(error));
                    $("#VideossList").mLoading("destroy");
                }
            }
        });
    }

    return {
        GetVideosCategory: getVideosCategory,
        GetVideosList: getVideosList,
        GetVideosDetailsList: getVideosDetailsList

    }
}());




function VideosPagination() {

    $('#page-selection-up,#page-selection-down').empty();
    var totalRecords = $("#hdnVideosTotalRecords").val();
    totalRecords = totalRecords != "" ? parseInt(totalRecords) : 0;
    var Size = jsonData["PageSize"];
    var totalPages = Math.floor(totalRecords % Size == 0 ? totalRecords / Size : (totalRecords / Size) + 1);
    var page = jsonData["PageIndex"];
    page = parseInt(page) ? (parseInt(page) <= totalPages ? parseInt(page) : totalPages) : 1;
    $('#page-selection-up,#page-selection-down').bootpag({
        total: totalPages,
        page: page,
        maxVisible: 10
    })

    $('#page-selection-up,#page-selection-down').off("page").on("page", function (event, num) {
        $("#PageIndex").val(num);
        $('#page-selection-up,#page-selection-down').bootpag({ page: num })
        ViewVideoUpload.GetVideosList(JsonData, num, false, null);


    });

};