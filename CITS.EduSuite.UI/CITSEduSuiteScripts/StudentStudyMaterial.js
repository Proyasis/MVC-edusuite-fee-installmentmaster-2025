var JsonData = [], request = null;
var ViewStudyMaterialUpload = (function () {

    var getStudyMaterialsCategory = function (json) {

        JsonData = json;
        $("#StudyMaterialsCategoryList").empty();

        request = $.ajax({
            url: $("#hdnGetStudyMaterialCategory").val(),
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
                $("#StudyMaterialsCategoryList").html(data);



                $("#StudyMaterialsCategoryList").mLoading("destroy");
            },
            error: function (error) {
                if (error.statusText != "abort") {
                    console.log(JSON.stringify(error));
                    $("#StudyMaterialsCategoryList").mLoading("destroy");
                }
            }
        });

    }

    var getStudyMaterialsList = function (json, pageIndex, resetPagination, SubjectKey) {
        JsonData = json;
        $("#StudyMaterialssList").empty();

        if (resetPagination) {
            JsonData["SubjectKey"] = SubjectKey;
            $("#hdnStudyMaterialCategoryKey").val(SubjectKey);
        }
        else {
            JsonData["SubjectKey"] = $("#hdnStudyMaterialCategoryKey").val();

        }
        $("#StudyMaterialssList").mLoading();


        JsonData["PageIndex"] = pageIndex ? pageIndex : 1;
        JsonData["PageSize"] = 10;
        request = $.ajax({
            url: $("#hdnGetStudyMaterials").val(),
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
                $("#StudyMaterialssList").html(data);

                if (resetPagination) {
                    StudyMaterialsPagination();
                }
                $("#TotalRecords").html($("#hdnStudyMaterialsTotalRecords").val());
                $("#StudyMaterialssList").mLoading("destroy");
            },
            error: function (error) {
                if (error.statusText != "abort") {
                    console.log(JSON.stringify(error));
                    $("#StudyMaterialssList").mLoading("destroy");
                }
            }
        });
    }



    var getStudyMaterialsDetailsList = function (json, pageIndex, resetPagination, StudyMaterialKey) {
        JsonData = json;
        $("#StudyMaterialssList").empty();


        $("#StudyMaterialssList").mLoading();

        JsonData["RowKey"] = StudyMaterialKey;

        request = $.ajax({
            url: $("#hdnGetStudyMaterialsDetails").val(),
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
                $("#StudyMaterialssList").html(data);

                //if (resetPagination) {
                //    StudyMaterialsPagination();
                //}
                //$("#TotalRecords").html($("#hdnStudyMaterialsTotalRecords").val());
                $("#StudyMaterialssList").mLoading("destroy");
            },
            error: function (error) {
                if (error.statusText != "abort") {
                    console.log(JSON.stringify(error));
                    $("#StudyMaterialssList").mLoading("destroy");
                }
            }
        });
    }

    return {
        GetStudyMaterialsCategory: getStudyMaterialsCategory,
        GetStudyMaterialsList: getStudyMaterialsList,
        GetStudyMaterialsDetailsList: getStudyMaterialsDetailsList

    }
}());




function StudyMaterialsPagination() {

    $('#page-selection-up,#page-selection-down').empty();
    var totalRecords = $("#hdnStudyMaterialsTotalRecords").val();
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
        ViewStudyMaterialUpload.GetStudyMaterialsList(JsonData, num, false, null);


    });

};