using System.Linq;
namespace RazorPowerTools.ControllerActionGenerator.GenerationTemplates
{
    public class AjaxActionLinkGenerationTemplate : IGenerationTemplate
    {
        public string Name => "Razor Ajax Action Link";

        public string Generate(ControllerAction action)
        {

            //@Ajax.ActionLink("", "", "", new { }, new AjaxOptions() { UpdateTargetId="DivId" ,HttpMethod="GET",InsertionMode=InsertionMode.Replace})
            if (action.Parameters.Any())
            {
                return $"@Ajax.ActionLink(\"Title Here\",\"{action.Name}\",\"{action.ControllerName}\", new {{ { string.Join(",", action.Parameters.Select(d => d.Name + " = \"\"")) } }}, new AjaxOptions() {{ UpdateTargetId = \"DivId\",HttpMethod = \"{action.ActionVerb.ToUpper()}\",InsertionMode=InsertionMode.Replace}})";
            }
            else
            {
                return $"@Ajax.ActionLink(\"Title Here\",\"{action.Name}\",\"{action.ControllerName}\",null, new AjaxOptions() {{ UpdateTargetId = \"DivId\",HttpMethod = \"{action.ActionVerb.ToUpper()}\",InsertionMode=InsertionMode.Replace}})";
            }
        }
    }
}