using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorPowerTools.VSIX
{
    public class ClassesMetadata
    {



        public static IEnumerable<CodeClass> GetClasses()
        {
            var service = (DTE)Package.GetGlobalService(typeof(SDTE));
            var projects = service.Solution.Projects;
            List<ProjectItem> allClasses = new List<ProjectItem>();
            foreach (Project item in projects)
            {
                allClasses.AddRange(GetProjectItems(item.ProjectItems).Where(v => v.Name.Contains(".cs")).ToList());
            }

            // check for .cs extension on each

            foreach (var c in allClasses)
            {
                var eles = c.FileCodeModel;
                if (eles == null)
                    continue;
                foreach (var ele in eles.CodeElements)
                {
                    if (ele is EnvDTE.CodeNamespace)
                    {
                        var ns = ele as EnvDTE.CodeNamespace;
                        // run through classes
                        foreach (var property in ns.Members)
                        {
                            var member = property as CodeClass;
                            if (member == null || member.Access != vsCMAccess.vsCMAccessPublic)
                                continue;

                            foreach (CodeType item in member.Bases)
                            {
                                if (item.Name.Contains("Controller"))
                                {
                                    yield return member;
                                    break;
                                }
                            }


                        }
                    }
                }
            }
        }


    

        /// <summary>
        /// Recursively gets all the ProjectItem objects in a list of projectitems from a Project
        /// </summary>
        /// <param name="projectItems">The project items.</param>
        /// <returns></returns>
        public static IEnumerable<ProjectItem> GetProjectItems(EnvDTE.ProjectItems projectItems)
        {
            if (projectItems != null)
            {


                foreach (EnvDTE.ProjectItem item in projectItems)
                {
                    yield return item;

                    if (item.SubProject != null)
                    {
                        foreach (EnvDTE.ProjectItem childItem in GetProjectItems(item.SubProject.ProjectItems))
                            yield return childItem;
                    }
                    else
                    {
                        foreach (EnvDTE.ProjectItem childItem in GetProjectItems(item.ProjectItems))
                            yield return childItem;
                    }
                }
            }

        }
    }
}
