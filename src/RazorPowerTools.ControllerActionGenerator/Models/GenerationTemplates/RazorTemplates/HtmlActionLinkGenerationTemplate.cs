using System.Linq;
namespace RazorPowerTools.ControllerActionGenerator.GenerationTemplates
{
    public class HtmlActionLinkGenerationTemplate : IGenerationTemplate
    {
        public virtual string Name => "Razor Html Action link";

        public virtual string Generate(ControllerAction action)
        {
          
            if (action.Parameters.Any())
            {
                return $"@Html.ActionLink(\"Title Here\",\"{action.Name}\",\"{action.ControllerName}\", new {{ { string.Join(",", action.Parameters.Select(d => d.Name + " = \"\"")) } }})";
            }
            else
            {
                return $"@Html.ActionLink(\"Title Here\",\"{action.Name}\",\"{action.ControllerName}\")";
            }
        }

        public override string ToString()
        {
            return Name;
        }

    }
}