namespace RazorPowerTools.ControllerActionGenerator.GenerationTemplates
{
    public interface IGenerationTemplate
    {
        string Name { get;  }

        string Generate(ControllerAction action);
    }
}