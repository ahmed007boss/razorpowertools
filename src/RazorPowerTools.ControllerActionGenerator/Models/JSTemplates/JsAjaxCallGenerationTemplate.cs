using System.Linq;
namespace RazorPowerTools.ControllerActionGenerator.GenerationTemplates
{
    public class JsAjaxCallGenerationTemplate : HtmlActionGenerationTemplate
    {
        public override string Name => "Razor Javascript Ajax Request";

        public override string Generate(ControllerAction action)
        {
            string result = "";
            result += "$.ajax({\n";
            result += $"type: '{action.ActionVerb.ToUpper()}',\n";
            result += $"url: '{base.Generate(action)}',\n";
            result += "data: '',\n";
            result += "contentType: 'application /json; charset=utf-8',\n";
            result += "dataType: 'json',\n";
            result += "success: function(data, textStatus) {\n\n";
            result += "},";
            result += "error: function(xhr, textStatus, errorThrown) {\n\n";
            result += "}});";

            return result;

        }

        public override string ToString()
        {
            return Name;
        }

    }
}