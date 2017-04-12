namespace RazorPowerTools.ControllerActionGenerator
{
    public class ControllerActionParameter
    {
        public string Name { get;  set; }
        public string TypeName { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }
}