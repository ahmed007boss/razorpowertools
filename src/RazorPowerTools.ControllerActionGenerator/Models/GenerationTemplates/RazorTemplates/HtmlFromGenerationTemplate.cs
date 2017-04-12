using System.Linq;
namespace RazorPowerTools.ControllerActionGenerator.GenerationTemplates
{
    public class HtmlFormGenerationTemplate : IGenerationTemplate
    {
        public string Name => "Razor Html Begin Form";

        public string Generate(ControllerAction action)
        {
            string result = "";
            result += "@using(var form =";
            if (action.Parameters.Any())
            {
                result += $"Html.BeginForm(\"{action.Name}\",\"{action.ControllerName}\", new {{ { string.Join(",", action.Parameters.Select(d => d.Name + " = \"\"")) } }}, FormMethod.{action.ActionVerb})";
            }
            else
            {
                result += $"Html.BeginForm(\"{action.Name}\",\"{action.ControllerName}\",null, FormMethod.{action.ActionVerb})";
            }

            result += ")\n{\n\n}";


            return result;

        }
    }
}