using System.Linq;
namespace RazorPowerTools.ControllerActionGenerator.GenerationTemplates
{
    public class UrlStringGenerationTemplate : IGenerationTemplate
    {
        public string Name => "URL";

        public string Generate(ControllerAction action)
        {

            //@Ajax.ActionLink("", "", "", new { }, new AjaxOptions() { UpdateTargetId="DivId" ,HttpMethod="GET",InsertionMode=InsertionMode.Replace})
            if (action.Parameters.Any())
            {
                return $"/{action.ControllerName}/{action.Name}?{string.Join("&", action.Parameters.Select(d => d.Name + "=VAL"))}";
            }
            else
            {
                return $"/{action.ControllerName}/{action.Name}";
            }
        }

        public override string ToString()
        {
            return Name;
        }

    }
}