/*
 * jQuery Client Side Excel Export Plugin Library
 * http://techbytarun.com/
 *
 * Copyright (c) 2013 Batta Tech Private Limited
 * https://github.com/tarunbatta/ExcelExportJs/blob/master/LICENSE.txt
 *
 * March 22, 2017 - Update by Maynard for IE 11.09 up compatability
 * 
 */

(function ($) {
    var $defaults = {
        containerid: null
        , datatype: 'table'
        , dataset: null
        , columns: null
        , returnUri: false
        , returnHtml: false
        , worksheetName: "My Worksheet"
        , encoding: "utf-8"
        , headerbg: null
        , title: null
        , subtitle: null
    };

    var $settings = $defaults;

    $.fn.excelexportjs = function (options) {

        $settings = $.extend({}, $defaults, options);

        var gridData = [];
        var excelData;

        return Initialize();

        function Initialize() {
            var type = $settings.datatype.toLowerCase();

            BuildDataStructure(type);


            switch (type) {
                case 'table':
                    excelData = ConvertFromTable();
                    break;
                case 'json':
                    excelData = ConvertDataStructureToTable();
                    break;
                case 'xml':
                    excelData = ConvertDataStructureToTable();
                    break;
                case 'jqgrid':
                    excelData = ConvertDataStructureToTable();
                    break;
            }


            if ($settings.returnUri) {
                return Export(excelData);
            }
            else if ($settings.returnHtml) {
                return excelData
            }
            else {

                if (!isBrowserIE()) {
                    window.open(Export(excelData));
                }


            }
        }

        function BuildDataStructure(type) {
            switch (type) {
                case 'table':
                    break;
                case 'json':
                    gridData = $settings.dataset;
                    break;
                case 'xml':
                    $($settings.dataset).find("row").each(function (key, value) {
                        var item = {};

                        if (this.attributes != null && this.attributes.length > 0) {
                            $(this.attributes).each(function () {
                                item[this.name] = this.value;
                            });

                            gridData.push(item);
                        }
                    });
                    break;
                case 'jqgrid':
                    $($settings.dataset).find("rows > row").each(function (key, value) {
                        var item = {};

                        if (this.children != null && this.children.length > 0) {
                            $(this.children).each(function () {
                                item[this.tagName] = $(this).text();
                            });

                            gridData.push(item);
                        }
                    });
                    break;
            }
        }

        function ConvertFromTable() {
            var tableCopy = $($settings.containerid).clone();

            $(tableCopy).attr("style", "border-collapse: collapse;padding:10px;min-width:500px").attr('border', '1');;
            $(tableCopy).find("tr").eq(0).find("td,th").attr("style", "background:" + $settings.headerbg + ";font-weight:bold")
            var collength = 0
            $(tableCopy).find('tr').eq(0).find("td,th").each(function () {
                if ($(this).attr('colspan')) {
                    collength += +$(this).attr('colspan');
                } else {
                    collength++;
                }
            });
            if ($settings.subtitle) {
                $(tableCopy).find("tr").eq(0).before("<tr><td  style='background:#c7e9da;font-weight:bold;text-align:center' colspan='" + collength + "'>" + $settings.subtitle + "</td></tr>");
            }
            if ($settings.title) {
                $(tableCopy).find("tr").eq(0).before("<tr><td  style='background:#c7e9da;font-weight:bold;text-align:center' colspan='" + collength + "'>" + $settings.title + "</td></tr>");
            }
            var result = $('<div>').append(tableCopy).html();
            return result;
        }

        function ConvertDataStructureToTable() {
            var result = "<table id='tabledata' style='border-collapse: collapse;min-width:100%' border='1' cellpadding='10' >";

            result += "<thead>";
            var collength = $settings.columns.length
            if ($settings.title) {
                result += "<tr><td style='background:#c7e9da;font-weight:bold;text-align:center'  colspan='" + collength + "'>" + $settings.title + "</td></tr>";
            }
            if ($settings.subtitle) {
                result += "<tr><td style='background:#c7e9da;font-weight:bold;text-align:center'  colspan='" + collength + "'>" + $settings.subtitle + "</td></tr>";
            }
            result += "<tr>"
            $($settings.columns).each(function (key, value) {
                if (this.ishidden != true) {
                    result += "<th"
                    result += " style='";
                    if (this.width != null) {
                        result += "width: " + this.width + ";";
                    }
                    if ($settings.headerbg) {
                        result += ($settings.headerbg ? "font-weight:bold;background:" + $settings.headerbg + ";" : "");
                    }

                    result += "'>";
                    result += this.headertext;
                    result += "</th>";
                }
            });
            result += "</tr></thead>";

            result += "<tbody>";
            $(gridData).each(function (key, value) {
                result += "<tr>";
                $($settings.columns).each(function (k, v) {
                    if (value.hasOwnProperty(this.name)) {
                        if (this.ishidden != true) {
                            result += "<td";
                            if (this.width != null) {
                                result += " style='width: " + this.width + "'";
                            }
                            result += ">";
                            result += value[this.name] != null ? formatColumnValue(this, value) : "";
                            result += "</td>";
                        }
                    }
                });
                result += "</tr>";
            });
            result += "</tbody>";

            result += "</table>";

            return result;
        }

        function Export(htmltable) {

            if (isBrowserIE()) {

                exportToExcelIE(htmltable);
            }
            else {
                var excelFile = "<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel' xmlns='http://www.w3.org/TR/REC-html40'>";
                excelFile += "<head>";
                excelFile += '<meta http-equiv="Content-type" content="text/html;charset=' + $defaults.encoding + '" />';
                excelFile += "<!--[if gte mso 9]>";
                excelFile += "<xml>";
                excelFile += "<x:ExcelWorkbook>";
                excelFile += "<x:ExcelWorksheets>";
                excelFile += "<x:ExcelWorksheet>";
                excelFile += "<x:Name>";
                excelFile += "{worksheet}";
                excelFile += "</x:Name>";
                excelFile += "<x:WorksheetOptions>";
                excelFile += "<x:DisplayGridlines/>";
                excelFile += "</x:WorksheetOptions>";
                excelFile += "</x:ExcelWorksheet>";
                excelFile += "</x:ExcelWorksheets>";
                excelFile += "</x:ExcelWorkbook>";
                excelFile += "</xml>";
                excelFile += "<![endif]-->";
                excelFile += "</head>";
                excelFile += "<body>";
                excelFile += htmltable.replace(/"/g, '\'');
                excelFile += "</body>";
                excelFile += "</html>";

                var uri = "data:application/vnd.ms-excel;base64,";
                var ctx = { worksheet: $settings.worksheetName, table: htmltable };

                return (uri + base64(format(excelFile, ctx)));
            }
        }

        function base64(s) {
            return window.btoa(unescape(encodeURIComponent(s)));
        }

        function format(s, c) {
            return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; });
        }

        function isBrowserIE() {
            var msie = !!navigator.userAgent.match(/Trident/g) || !!navigator.userAgent.match(/MSIE/g);
            if (msie > 0) {  // If Internet Explorer, return true
                return true;
            }
            else {  // If another browser, return false
                return false;
            }
        }

        function exportToExcelIE(table) {


            var el = document.createElement('div');
            el.innerHTML = table;

            var tab_text = "<table border='2px'><tr bgcolor='#87AFC6'>";
            var textRange; var j = 0;
            var tab;


            if ($settings.datatype.toLowerCase() == 'table') {
                tab = $($settings.containerid)[0];  // get table              
            }
            else {
                tab = el.children[0]; // get table
            }



            for (j = 0 ; j < tab.rows.length ; j++) {
                tab_text = tab_text + tab.rows[j].innerHTML + "</tr>";
                //tab_text=tab_text+"</tr>";
            }

            tab_text = tab_text + "</table>";
            tab_text = tab_text.replace(/<A[^>]*>|<\/A>/g, "");//remove if u want links in your table
            tab_text = tab_text.replace(/<img[^>]*>/gi, ""); // remove if u want images in your table
            tab_text = tab_text.replace(/<input[^>]*>|<\/input>/gi, ""); // reomves input params

            var ua = window.navigator.userAgent;
            var msie = ua.indexOf("MSIE ");

            if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))      // If Internet Explorer
            {
                txtArea1.document.open("txt/html", "replace");
                txtArea1.document.write(tab_text);
                txtArea1.document.close();
                txtArea1.focus();
                sa = txtArea1.document.execCommand("SaveAs", true, "download");
            }
            else
                sa = window.open('data:application/vnd.ms-excel,' + encodeURIComponent(tab_text));

            return (sa);


        }

    };
})(jQuery);


//get columns
function getColumns(paramData) {

    var header = [];
    $.each(paramData[0], function (key, value) {
        //console.log(key + '==' + value);
        var obj = {}
        obj["headertext"] = key;
        obj["datatype"] = "string";
        obj["name"] = key;
        header.push(obj);
    });
    return header;

}

function formatColumnValue(obj, value) {
    if (obj.formatter) {
        if (typeof obj.formatter === 'function') {
            return obj.formatter.call(null,value[obj.name], null, value, null)
        }
        else if (obj.formatter == "date") {
            return AppCommon.JsonDateToNormalDate(value[obj.name])
        }
        else {
            return value[obj.name]
        }
    }
    else {
        return value[obj.name]
    }
}

