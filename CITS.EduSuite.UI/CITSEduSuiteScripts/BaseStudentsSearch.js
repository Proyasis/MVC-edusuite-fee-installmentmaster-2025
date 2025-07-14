var BaseStudentsSearch = (function () {

    var getCourseTypeByAcademicTerm = function (ddl) {
        $(ddl).html("");
        $(ddl).val('default').selectpicker("refresh");
        $.ajax({
            url: $("#hdnGetCourseTypeByAcademicTerm").val(),
            type: "POST",
            data: $("form").serializeArray(),
            success: function (result) {
                $.each(result.CourseTypes, function (i, CourseType) {
                    $(ddl).append(
                        $('<option></option>').val(CourseType.RowKey).html(CourseType.Text));
                });
                $(ddl).selectpicker("refresh");
            },
            error: function (xhr) {
                conso.log(xhr.responseText);
            }
        });
    }

    var getCourseByCourseType = function (ddl) {
        $(ddl).html(""); $(ddl).val('default').selectpicker("refresh");


        $.ajax(
            {
                url: $("#hdnGetCourseByCourseType").val(),
                type: "POST",
                data: $("form").serializeArray(),
                success: function (result) {
                    $.each(result.Courses, function (i, Course) {
                        $(ddl).append(
                            $('<option></option>').val(Course.RowKey).html(Course.Text));
                    });


                    $(ddl).selectpicker("refresh");

                }
            });
    }

    var getUniversityByCourse = function (ddl) {
        $(ddl).html(""); $(ddl).val('default').selectpicker("refresh");

        $.ajax({
            url: $("#hdnGetUniversityByCourse").val(),
            type: "POST",
            
            data: $("form").serializeArray(),
            
            success: function (result) {
                $.each(result.UniversityMasters, function (i, University) {
                    $(ddl).append(
                        $('<option></option>').val(University.RowKey).html(University.Text));
                });
                $(ddl).selectpicker("refresh");
            }
        });

    }

    var getYearsByAcademicTerm = function (ddl) {
        $(ddl).html(""); $(ddl).val('default').selectpicker("refresh");

        $.ajax({
            url: $("#hdnGetYearsByAcademicTermKey").val(),
            type: "POST",
            
            data: $("form").serializeArray(),
            
            success: function (result) {
                $.each(result.CourseYears, function (i, CourseYears) {
                    $(ddl).append($('<option></option>').val(CourseYears.RowKey).html(CourseYears.Text));
                });
                $(ddl).selectpicker("refresh");
            }
        });
    }
    var getEmployeesByBranches = function (ddl) {
        $(ddl).html(""); $(ddl).val('default').selectpicker("refresh");


        $.ajax(
            {
                url: $("#hdnGetEmployeesByBranchKeys").val(),
                type: "POST",
                data: $("form").serializeArray(),
                success: function (result) {
                    $.each(result, function (i, result) {
                        $(ddl).append(
                            $('<option></option>').val(result.RowKey).html(result.Text));
                    });


                    $(ddl).selectpicker("refresh");

                }
            });
    }


    return {
        GetCourseTypeByAcademicTerm: getCourseTypeByAcademicTerm,
        GetCourseByCourseType: getCourseByCourseType,
        GetUniversityByCourse: getUniversityByCourse,
        GetYearsByAcademicTerm: getYearsByAcademicTerm,
        GetEmployeesByBranches: getEmployeesByBranches
    }

}());




function GenerateListBoxString(obj) {
    var Data = "";
    $(obj).each(function () {
        var selected = $("option:selected", this).map(function () {
            Data = Data + $(this).val() + ",0";
        })
    });
    return Data;
}