using System.Linq;
namespace RazorPowerTools.ControllerActionGenerator.GenerationTemplates
{
    public class UrlActionGenerationTemplate : IGenerationTemplate
    {
        public string Name => "Razor Action URL";

        public string Generate(ControllerAction action)
        {
            if (action.Parameters.Any())
            {
                return $"@Url.Action(\"{action.Name}\",\"{action.ControllerName}\", new {{ { string.Join(",", action.Parameters.Select(d => d.Name + " = \"\"")) } }})";
            }
            else
            {
                return $"@Url.Action(\"{action.Name}\",\"{action.ControllerName}\")";
            }
        }
    }
}