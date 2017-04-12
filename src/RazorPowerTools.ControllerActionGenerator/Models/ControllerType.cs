namespace RazorPowerTools.ControllerActionGenerator
{
    public class ControllerType
    {
        public System.Collections.Generic.List<ControllerAction> Functions { get; set; }
        public string Name { get;  set; }

        public override string ToString()
        {
            return Name;
        }

    }
}