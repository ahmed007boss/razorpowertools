using System.Linq;
namespace RazorPowerTools.ControllerActionGenerator
{
    public class ControllerAction
    {
        public string ControllerName { get;  set; }
        public string Name { get;  set; }
        public System.Collections.Generic.IEnumerable<ControllerActionParameter> Parameters { get; set; }
        public string returnType { get; set; }


        public string Signature { get { return $"Public {returnType} {Name}({string.Join(",", Parameters.Select(d => d.TypeName + " " + d.Name))});"; } }

   

     
    }
}