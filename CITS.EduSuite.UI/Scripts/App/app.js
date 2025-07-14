//if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|ipad|iris|kindle|Android|Silk|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(navigator.userAgent)
// || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(navigator.userAgent.substr(0, 4))) {
var agent = navigator.userAgent;
if (navigator.userAgent.toLowerCase().search("mobile") > 0) {
    if ($.fn.selectpicker)
        $.fn.selectpicker.Constructor.DEFAULTS.mobile = true;

    $.extend(true, $.jgrid.defaults, {
        // new default values for some options
        autowidth: false, shrinkToFit: false
    });
}
else {
    $.extend(true, $.jgrid.defaults, {
        // new default values for some options
        autowidth: true, shrinkToFit: true
    });
}
$.extend(true, $.fn.fmatter, {
    currencyFmatter: function (cellvalue, options, rowdata) {
        return AppCommon.FormatCurrency(cellvalue);
    }
});
Array.prototype.contains = function (v) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] === v) return true;
    }
    return false;
};

Array.prototype.unique = function () {
    var arr = [];
    for (var i = 0; i < this.length; i++) {
        if (!arr.contains(this[i])) {
            arr.push(this[i]);
        }
    }
    return arr;
}

Array.prototype.duplicate =
    function () {
        var sorted_arr = this.slice().sort(); // You can define the comparing function here. 
        // JS by default uses a crappy string compare.
        // (we use slice to clone the array so the
        // original array won't be modified)
        var results = [];
        for (let i = 0; i < sorted_arr.length - 1; i++) {
            if (sorted_arr[i + 1] == sorted_arr[i]) {
                results.push(sorted_arr[i]);
            }
        }
        return results;
    };
Array.prototype.formatdata = function () {
        var results = $(this).map(function (n, item) {
            var obj = {};
            $(item).each(function () {
                obj[this.Key] = this.Value;
            });
            return obj;
        }).toArray();
        return results;
};
moment.tz.add("Asia/Calcutta|HMT BURT IST IST|-5R.k -6u -5u -6u|01232|-18LFR.k 1unn.k HB0 7zX0");
moment.tz.link("Asia/Calcutta|Asia/Kolkata");
$(document).ready(function () {
    $(document).on('change', '.custom-file-input', function (event) {

        $(this).next('.custom-file-label').html(event.target.files.length > 0 ? event.target.files[0].name : "Choose File");
    })
    var menuid = localStorage.getItem("MenuId");
    var $menu = "";
    var $menuItem = "";
    $menu = !menuid || menuid == "0" ? $(".side-menu-list li.with-sub").eq(0) : $(".side-menu-list").find("#" + menuid).closest("li.with-sub")
    if (menuid != "0") {
        $menuItem = $(".side-menu-list").find("#" + menuid).closest("li")
        $menuItem.addClass("active");
    }
    $menu.addClass("opened");

    $("[menuItems]").on("click", function () {
        localStorage.setItem("MenuId", $(this).attr("id"));
    });
    //Jqgrid Style
    var replacement =
    {
        'ui-icon-seek-first': 'ace-icon fa fa-angle-double-left bigger-140',
        'ui-icon-seek-prev': 'ace-icon fa fa-angle-left bigger-140',
        'ui-icon-seek-next': 'ace-icon fa fa-angle-right bigger-140',
        'ui-icon-seek-end': 'ace-icon fa fa-angle-double-right bigger-140',
        'ui-grid-ico-sort ui-icon-asc': 'ace-icon fa fa-sort-desc',
        'ui-grid-ico-sort ui-icon-desc': 'ace-icon fa fa-sort-desc'
    };
    $('.ui-pg-table:not(.navtable) > tbody > tr > .ui-pg-button > .ui-icon').each(function () {
        var icon = $(this);
        var $class = $.trim(icon.attr('class').replace('ui-icon', ''));

        if ($class in replacement) icon.attr('class', 'ui-icon ' + replacement[$class]);
    })
    $("ul.dropdown-menu [data-toggle='dropdown']").on("click", function (event) {
        event.preventDefault();
        event.stopPropagation();

        $(this).siblings().toggleClass("show");


        if (!$(this).next().hasClass('show')) {
            $(this).parents('.dropdown-menu').first().find('.show').removeClass("show");
        }
        $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function (e) {
            $('.dropdown-submenu .show').removeClass("show");
        });

    });
    $("#fullscreen-button").on("click", function toggleFullScreen() {
        if ((document.fullScreenElement !== undefined && document.fullScreenElement === null) || (document.msFullscreenElement !== undefined && document.msFullscreenElement === null) || (document.mozFullScreen !== undefined && !document.mozFullScreen) || (document.webkitIsFullScreen !== undefined && !document.webkitIsFullScreen)) {
            if (document.documentElement.requestFullScreen) {
                document.documentElement.requestFullScreen();
            } else if (document.documentElement.mozRequestFullScreen) {
                document.documentElement.mozRequestFullScreen();
            } else if (document.documentElement.webkitRequestFullScreen) {
                document.documentElement.webkitRequestFullScreen(Element.ALLOW_KEYBOARD_INPUT);
            } else if (document.documentElement.msRequestFullscreen) {
                document.documentElement.msRequestFullscreen();
            }
        } else {
            if (document.cancelFullScreen) {
                document.cancelFullScreen();
            } else if (document.mozCancelFullScreen) {
                document.mozCancelFullScreen();
            } else if (document.webkitCancelFullScreen) {
                document.webkitCancelFullScreen();
            } else if (document.msExitFullscreen) {
                document.msExitFullscreen();
            }
        }
    })

    /* ==========================================================================
        Scroll
        ========================================================================== */

    if (!("ontouchstart" in document.documentElement)) {

        document.documentElement.className += " no-touch";

        var jScrollOptions = {
            autoReinitialise: true,
            autoReinitialiseDelay: 100,
            contentWidth: '0px'
        };

        $('.scrollable .box-typical-body').jScrollPane(jScrollOptions);
        $('.side-menu').jScrollPane(jScrollOptions);
        $('.side-menu-addl').jScrollPane(jScrollOptions);
        $('.scrollable-block').jScrollPane(jScrollOptions);
    }

    /* ==========================================================================
        Header search
        ========================================================================== */

    $('.site-header .site-header-search').each(function () {
        var parent = $(this),
            overlay = parent.find('.overlay');

        overlay.click(function () {
            parent.removeClass('closed');
        });

        parent.clickoutside(function () {
            if (!parent.hasClass('closed')) {
                parent.addClass('closed');
            }
        });
    });

    /* ==========================================================================
        Header mobile menu
        ========================================================================== */

    // Dropdowns
    $('.site-header-collapsed .dropdown').each(function () {
        var parent = $(this),
            btn = parent.find('.dropdown-toggle');

        btn.click(function () {
            if (parent.hasClass('mobile-opened')) {
                parent.removeClass('mobile-opened');
            } else {
                parent.addClass('mobile-opened');
            }
        });
    });

    $('.dropdown-more').each(function () {
        var parent = $(this),
            more = parent.find('.dropdown-more-caption'),
            classOpen = 'opened';

        more.click(function () {
            if (parent.hasClass(classOpen)) {
                parent.removeClass(classOpen);
            } else {
                parent.addClass(classOpen);
            }
        });
    });

    // Left mobile menu
    $('.hamburger').click(function () {
        if ($('body').hasClass('menu-left-opened')) {
            $(this).removeClass('is-active');
            $('body').removeClass('menu-left-opened');
            $('html').css('overflow', 'auto');
        } else {
            $(this).addClass('is-active');
            $('body').addClass('menu-left-opened');
            $('html').css('overflow', 'hidden');
            $('.side-menu').addClass("overflow-auto");
            $('.side-menu').css("height", "100%");
        }
    });

    $('.mobile-menu-left-overlay').click(function () {
        $('.hamburger').removeClass('is-active');
        $('body').removeClass('menu-left-opened');
        $('html').css('overflow', 'auto');
    });

    // Right mobile menu
    $('.site-header .burger-right').click(function () {
        if ($('body').hasClass('menu-right-opened')) {
            $('body').removeClass('menu-right-opened');
            $('html').css('overflow', 'auto');
        } else {
            $('.hamburger').removeClass('is-active');
            $('body').removeClass('menu-left-opened');
            $('body').addClass('menu-right-opened');
            $('html').css('overflow', 'hidden');
        }
    });

    $('.mobile-menu-right-overlay').click(function () {
        $('body').removeClass('menu-right-opened');
        $('html').css('overflow', 'auto');
    });

    /* ==========================================================================
        Header help
        ========================================================================== */

    $('.help-dropdown').each(function () {
        var parent = $(this),
            btn = parent.find('>button'),
            popup = parent.find('.help-dropdown-popup'),
            jscroll
            ;

        btn.click(function () {
            if (parent.hasClass('opened')) {
                parent.removeClass('opened');
                jscroll.destroy();
            } else {
                parent.addClass('opened');

                if (!("ontouchstart" in document.documentElement)) {
                    setTimeout(function () {
                        jscroll = parent.find('.jscroll').jScrollPane(jScrollOptions).data().jsp;
                    }, 0);
                }
                else {
                    $('.side-menu').addClass("overflow-auto");
                    $('.side-menu').css("height", "100%");
                }
            }
        });

        $('html').click(function (event) {
            if (
                !$(event.target).closest('.help-dropdown-popup').length
                &&
                !$(event.target).closest('.help-dropdown>button').length
                &&
                !$(event.target).is('.help-dropdown-popup')
                &&
                !$(event.target).is('.help-dropdown>button')
            ) {
                if (parent.hasClass('opened')) {
                    parent.removeClass('opened');
                    jscroll.destroy();
                }
            }
        });
    });

    /* ==========================================================================
        Side menu list
        ========================================================================== */


    $('.side-menu-list li.with-sub').each(function () {
        var parent = $(this),
            clickLink = parent.find('>span'),
            subMenu = parent.find('>ul');

        clickLink.click(function () {
            if (parent.hasClass('opened')) {

                subMenu.slideUp(function () {
                    parent.removeClass('opened');
                    subMenu.find('.opened').removeClass('opened');
                });

            } else {
                if (clickLink.parents('.with-sub').length == 1) {
                    $('.side-menu-list .opened').removeClass('opened').find('ul').slideUp();
                }

                subMenu.slideDown(function () {
                    parent.addClass('opened');
                });
            }
        });
    });

    /*menu hide show content mouse hover*/

    $(".horizontal-menu .bottom-navbar .page-navigation > .nav-item").on("click", function () {
        $(".submenu").hide();
        var div = $(this).find(".submenu");
        div.show();
    });
    $(document).on("click", function (e) {
        if ($(e.target).closest(".page-navigation").length <= 0) {
            $(".submenu").hide();
        };


    });


    /* ==========================================================================
        Dashboard
        ========================================================================== */

    $(window).resize(function () {
        $('body').trigger('click');
    });

    // Collapse box
    $('.box-typical-dashboard').each(function () {
        var parent = $(this),
            btnCollapse = parent.find('.action-btn-collapse');

        btnCollapse.click(function () {
            if (parent.hasClass('box-typical-collapsed')) {
                parent.removeClass('box-typical-collapsed');
            } else {
                parent.addClass('box-typical-collapsed');
            }
        });
    });

    // Full screen box
    $('.box-typical-dashboard').each(function () {
        var parent = $(this),
            btnExpand = parent.find('.action-btn-expand'),
            classExpand = 'box-typical-full-screen';

        btnExpand.click(function () {
            if (parent.hasClass(classExpand)) {
                parent.removeClass(classExpand);
                $('html').css('overflow', 'auto');
            } else {
                parent.addClass(classExpand);
                $('html').css('overflow', 'hidden');
            }
        });
    });

    /* ==========================================================================
        Select
        ========================================================================== */

    if ($('.bootstrap-select').length) {
        // Bootstrap-select
        $('.bootstrap-select').selectpicker({
            style: '',
            width: '100%',
            size: 8
        });
    }

    if ($('.select2').length) {
        // Select2
        //$.fn.select2.defaults.set("minimumResultsForSearch", "Infinity");

        $('.select2').not('.manual').select2();

        $(".select2-icon").not('.manual').select2({
            templateSelection: select2Icons,
            templateResult: select2Icons
        });

        $(".select2-arrow").not('.manual').select2({
            theme: "arrow"
        });

        $('.select2-no-search-arrow').select2({
            minimumResultsForSearch: "Infinity",
            theme: "arrow"
        });

        $('.select2-no-search-default').select2({
            minimumResultsForSearch: "Infinity"
        });

        $(".select2-white").not('.manual').select2({
            theme: "white"
        });

        $(".select2-photo").not('.manual').select2({
            templateSelection: select2Photos,
            templateResult: select2Photos
        });
    }

    function select2Icons(state) {
        if (!state.id) { return state.text; }
        var $state = $(
            '<span class="font-icon ' + state.element.getAttribute('data-icon') + '"></span><span>' + state.text + '</span>'
        );
        return $state;
    }

    function select2Photos(state) {
        if (!state.id) { return state.text; }
        var $state = $(
            '<span class="user-item"><img src="' + state.element.getAttribute('data-photo') + '"/>' + state.text + '</span>'
        );
        return $state;
    }

    /* ==========================================================================
        Tooltips
        ========================================================================== */

    // Tooltip
    $('[data-toggle="tooltip"]').tooltip({
        html: true
    });

    // Popovers
    $('[data-toggle="popover"]').popover({
        trigger: 'focus'
    });

    /* ==========================================================================
        Full height box
        ========================================================================== */

    function boxFullHeight() {
        var sectionHeader = $('.section-header');
        var sectionHeaderHeight = 0;

        if (sectionHeader.length) {
            sectionHeaderHeight = parseInt(sectionHeader.height()) + parseInt(sectionHeader.css('padding-bottom'));
        }

        $('.box-typical-full-height').css('min-height',
            $(window).height() -
            parseInt($('.page-content').css('padding-top')) -
            parseInt($('.page-content').css('padding-bottom')) -
            sectionHeaderHeight -
            parseInt($('.box-typical-full-height').css('margin-bottom')) - 2
        );
        $('.box-typical-full-height>.tbl, .box-typical-full-height>.box-typical-center').height(parseInt($('.box-typical-full-height').css('min-height')));
    }

    boxFullHeight();

    $(window).resize(function () {
        boxFullHeight();
    });

    /* ==========================================================================
        Chat
        ========================================================================== */

    function chatHeights() {
        $('.chat-dialog-area').height(
            $(window).height() -
            parseInt($('.page-content').css('padding-top')) -
            parseInt($('.page-content').css('padding-bottom')) -
            parseInt($('.chat-container').css('margin-bottom')) - 2 -
            $('.chat-area-header').outerHeight() -
            $('.chat-area-bottom').outerHeight()
        );
        $('.chat-list-in')
            .height(
                $(window).height() -
                parseInt($('.page-content').css('padding-top')) -
                parseInt($('.page-content').css('padding-bottom')) -
                parseInt($('.chat-container').css('margin-bottom')) - 2 -
                $('.chat-area-header').outerHeight()
            )
            .css('min-height', parseInt($('.chat-dialog-area').css('min-height')) + $('.chat-area-bottom').outerHeight());
    }

    chatHeights();

    $(window).resize(function () {
        chatHeights();
    });

    /* ==========================================================================
        Box typical full height with header
        ========================================================================== */

    function boxWithHeaderFullHeight() {
        /*$('.box-typical-full-height-with-header').each(function(){
			var box = $(this),
				boxHeader = box.find('.box-typical-header'),
				boxBody = box.find('.box-typical-body');

			boxBody.height(
				$(window).height() -
				parseInt($('.page-content').css('padding-top')) -
				parseInt($('.page-content').css('padding-bottom')) -
				parseInt(box.css('margin-bottom')) - 2 -
				boxHeader.outerHeight()
			);
		});*/
    }

    boxWithHeaderFullHeight();

    $(window).resize(function () {
        boxWithHeaderFullHeight();
    });

    /* ==========================================================================
        File manager
        ========================================================================== */

    function fileManagerHeight() {
        $('.files-manager').each(function () {
            var box = $(this),
                boxColLeft = box.find('.files-manager-side'),
                boxSubHeader = box.find('.files-manager-header'),
                boxCont = box.find('.files-manager-content-in'),
                boxColRight = box.find('.files-manager-aside');

            var paddings = parseInt($('.page-content').css('padding-top')) +
                parseInt($('.page-content').css('padding-bottom')) +
                parseInt(box.css('margin-bottom')) + 2;

            boxColLeft.height('auto');
            boxCont.height('auto');
            boxColRight.height('auto');

            if (boxColLeft.height() <= ($(window).height() - paddings)) {
                boxColLeft.height(
                    $(window).height() - paddings
                );
            }

            if (boxColRight.height() <= ($(window).height() - paddings - boxSubHeader.outerHeight())) {
                boxColRight.height(
                    $(window).height() -
                    paddings -
                    boxSubHeader.outerHeight()
                );
            }

            boxCont.height(
                boxColRight.height()
            );
        });
    }

    fileManagerHeight();

    $(window).resize(function () {
        fileManagerHeight();
    });

    /* ==========================================================================
        Mail
        ========================================================================== */

    function mailBoxHeight() {
        $('.mail-box').each(function () {
            var box = $(this),
                boxHeader = box.find('.mail-box-header'),
                boxColLeft = box.find('.mail-box-list'),
                boxSubHeader = box.find('.mail-box-work-area-header'),
                boxColRight = box.find('.mail-box-work-area-cont');

            boxColLeft.height(
                $(window).height() -
                parseInt($('.page-content').css('padding-top')) -
                parseInt($('.page-content').css('padding-bottom')) -
                parseInt(box.css('margin-bottom')) - 2 -
                boxHeader.outerHeight()
            );

            boxColRight.height(
                $(window).height() -
                parseInt($('.page-content').css('padding-top')) -
                parseInt($('.page-content').css('padding-bottom')) -
                parseInt(box.css('margin-bottom')) - 2 -
                boxHeader.outerHeight() -
                boxSubHeader.outerHeight()
            );
        });
    }

    mailBoxHeight();

    $(window).resize(function () {
        mailBoxHeight();
    });


    /* ==========================================================================
        Steps progress
        ========================================================================== */

    function stepsProgresMarkup() {
        $('.steps-icon-progress').each(function () {
            var parent = $(this),
                cont = parent.find('ul'),
                padding = 0,
                padLeft = (parent.find('li:first-child').width() - parent.find('li:first-child .caption').width()) / 2,
                padRight = (parent.find('li:last-child').width() - parent.find('li:last-child .caption').width()) / 2;

            padding = padLeft;

            if (padLeft > padRight) padding = padRight;

            cont.css({
                marginLeft: -padding,
                marginRight: -padding
            });
        });
    }

    stepsProgresMarkup();

    $(window).resize(function () {
        stepsProgresMarkup();
    });

    /* ========================================================================== */

    $('.control-panel-toggle').on('click', function () {
        var self = $(this);

        if (self.hasClass('open')) {
            self.removeClass('open');
            $('.control-panel').removeClass('open');
        } else {
            self.addClass('open');
            $('.control-panel').addClass('open');
        }
    });

    $('.control-item-header .icon-toggle, .control-item-header .text').on('click', function () {
        var content = $(this).closest('li').find('.control-item-content');

        if (content.hasClass('open')) {
            content.removeClass('open');
        } else {
            $('.control-item-content.open').removeClass('open');
            content.addClass('open');
        }
    });

    $.browser = {};
    $.browser.chrome = /chrome/.test(navigator.userAgent.toLowerCase());
    $.browser.msie = /msie/.test(navigator.userAgent.toLowerCase());
    $.browser.mozilla = /firefox/.test(navigator.userAgent.toLowerCase());

    if ($.browser.chrome) {
        $('body').addClass('chrome-browser');
    } else if ($.browser.msie) {
        $('body').addClass('msie-browser');
    } else if ($.browser.mozilla) {
        $('body').addClass('mozilla-browser');
    }

    $('#show-hide-sidebar-toggle').on('click', function () {
        if (!$('body').hasClass('sidebar-hidden')) {
            $('body').addClass('sidebar-hidden');
        } else {
            $('body').removeClass('sidebar-hidden');
        }
    });


});

