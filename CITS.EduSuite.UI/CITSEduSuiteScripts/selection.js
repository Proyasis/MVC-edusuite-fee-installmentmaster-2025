

var selection = (function ()
{
    var NewSelection = function (value,text)
    {
        var html = '<li class="selection"   selection_value=' + value + '>' + text + ' <span onclick="selection.RemoveSelection(' + value+')" class="rmvItem"><i class="fa fa-remove"></i></span> </li>';
        $("#selectionList").append(html);
        
        if ($("li[selection_value]").length > 0) {
            $("#selection").show();
        }
    }

    var RemoveSelection = function (value)
    {
        $("li[selection_value='" + value + "']").remove();
        $("input[selection_value='" + value + "']").prop("checked", false);

        if ($("li[selection_value]").length == 0) {
            $("#selection").hide();
        }

    }

    var CheckList = function ()
    {
        $("#selectionList li").each(function ()
        {
            $("input[selection_value='" + $(this).attr("selection_value") + "']").prop("checked", true);
        });
    }

    var AppendList = function (id) {
        $("#selectionList li").each(function ()
        {
            var value =$(this).attr("selection_value");
            $(id).append('<option value="' + value + '" selected="true">' + value+'</option>');
        });
    }
 

    return {
        NewSelection: NewSelection,
        RemoveSelection: RemoveSelection,
        CheckList: CheckList,
        AppendList: AppendList
    }

}());