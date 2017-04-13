using System.Linq;
namespace RazorPowerTools.ControllerActionGenerator.GenerationTemplates
{
    public class JsAjaxLoadGenerationTemplate : HtmlActionGenerationTemplate
    {
        public override string Name => "Razor Javascript load Request";

        public override string Generate(ControllerAction action)
        {
            string result = "";
            result += $"$('#divid').load('{base.Generate(action)}',function (data){{\n\n}});";         
            return result;

        }

        public override string ToString()
        {
            return Name;
        }

    }
}