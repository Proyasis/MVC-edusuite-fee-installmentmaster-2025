

function ResponsiveMenu() {

    $(document).ready(function () {
        $(".Toogle").hide();

        if ($("body").width() <= 1109) {
            $(".Toogle").addClass("toogleMenu");
            $(".Toogle").addClass("toogleLeft");



            $(".iconToogle").click(function () {
                if ($(".Toogle").hasClass("toogleLeft")) {
                    $(".Toogle").removeClass("toogleLeft");
                }
                else {

                    $(".Toogle").addClass("toogleLeft");
                }

            });
        }
    });
}

    $(document).ready(function () {

        if ($("body").width() <= 1109) {
            $(".Toogle").addClass("toogleLeft");

           ResponsiveMenu();
        }
        else {
            $(".Toogle").removeClass("toogleLeft");
        }


        if ($("body").width() <= 798) {


           /* ResponsiveTable();*/
        }
    });


    $(document).ready(function () {
        $(window).resize(function () {
            ResponsiveMenu();

        });
    });


    function ResponsiveTable() {
        $(document).ready(function () {
            $("#grid").append($(".ui-jqgrid-htable").html());

        });
    }


    function ResponsiveMenu() {

        $(document).ready(function () {
            $(".Toogle").addClass("toogleMenu");


            $(".iconToogle").click(function () {
                if ($(".Toogle").hasClass("toogleLeft")) {
                    $(".Toogle").removeClass("toogleLeft");
                }
                else if (!$(".Toogle").hasClass("toogleLeft")) {

                    $(".Toogle").addClass("toogleLeft");
                }

            });

        });
    }




//Script to Remove space on Input in textbox

    $(document).ready(function () {

        $('input[CharSpace="RemoveSpace"]').on("input", function () {

            var start = this.selectionStart
            $(this).val($(this).val().toUpperCase())
            var test = /^[\s]/;
            if (test.test($(this).val())) {
                var newVal = $(this).val().replace(/[\s *0-]/g, "");
                $(this).val(newVal);
            }
            document.getElementById(this.id).value = document.getElementById(this.id).value.trim();
            this.selectionStart = this.selectionEnd = start;

        });

    });




//Script to trim all textboxes on button click -- 
$(document).ready(function () {
    setTimeout(function () {
        $('select:not(.ui-pg-selbox)').selectpicker();
    }, 500)
    $('input,textarea,select').click(function (e) {
        $('input[type="text"],textarea').each(function () {
            $(this).val($(this).val().trim())
        });
    });
});
