using System.Linq;
namespace RazorPowerTools.ControllerActionGenerator.GenerationTemplates
{
    public class HtmlActionGenerationTemplate : IGenerationTemplate
    {
        public virtual string Name => "Razor Html Action";

        public virtual string Generate(ControllerAction action)
        {
            if (action.Parameters.Any())
            {
                return $"@Html.Action(\"{action.Name}\",\"{action.ControllerName}\", new {{ { string.Join(",", action.Parameters.Select(d => d.Name + " = \"\"")) } }})";
            }
            else
            {
                return $"@Html.Action(\"{action.Name}\",\"{action.ControllerName}\")";
            }
        }

        public  override string ToString()
        {
            return Name;
        }

    }
}