//------------------------------------------------------------------------------
// <copyright file="ControllerActionGeneratorVSCommand.cs" company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RazorPowerTools.ControllerActionGenerator;

namespace RazorPowerTools.VSIX.ControllerActionGeneratorCommand
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ControllerActionGeneratorVSCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("055ce90c-fd63-416a-b404-30c12b67b804");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerActionGeneratorVSCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ControllerActionGeneratorVSCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ControllerActionGeneratorVSCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new ControllerActionGeneratorVSCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {

            try
            {
                var x = ClassesMetadata.GetClasses().ToList();
                var t = x.Select(d => new ControllerType
                {
                    Name = d.Name,
                    Functions = d.Children.OfType<CodeFunction2>().ToList()
                    .Where(func => func.Access == vsCMAccess.vsCMAccessPublic && func.FunctionKind == vsCMFunction.vsCMFunctionFunction)
                .Select(fun => new ControllerAction
                {
                    ControllerName = d.Name,
                    Name = fun.Name,
                    returnType = (fun.Type.CodeType as CodeClass2).Name,
                    ActionVerb = GetVerb(fun),
                    Parameters = fun.Parameters.OfType<CodeParameter2>().ToList()
                .Select(prm => new ControllerActionParameter { Name = prm.Name, TypeName = prm.Type.CodeType.Name })
                .ToList()
                }).ToList()
                });



                System.Windows.Window s = new ActionSelectorDialogWindow(t.ToList(), InsertText);

                s.ShowDialog();
            }
            catch (Exception ex)
            {


            }
        }

        public string GetVerb(CodeFunction2 d)
        {


            List<CodeAttribute2> codeAttributes2 = d.Attributes.OfType<CodeAttribute2>().ToList();

            var attrs = codeAttributes2.Select(attr => attr.Name).ToList();
            var result = "Get";
            if (attrs.Contains("HttpPost"))
            {
                result = "Post";
            }
            if (attrs.Contains("HttpPut"))
            {
                result = "Put";
            }

            if (attrs.Contains("HttpDelete"))
            {
                result = "Delete";
            }

            if (attrs.Contains("HttpPatch"))
            {
                result = "Patch";
            }

            if (attrs.Contains("HttpOption"))
            {
                result = "Option";
            }
            return result;
        }
        public void InsertText(string textToInsert)
        {
            var provider = ServiceProvider.GetService(typeof(DTE)) as DTE2;

            if (provider == null || provider.ActiveDocument == null) return;

            if (provider.UndoContext.IsOpen)
                provider.UndoContext.Close();

            provider.UndoContext.Open("Generate Action");

            try
            {
                TextSelection sel = (TextSelection)provider.ActiveDocument.Selection;
                EditPoint startPoint = sel.TopPoint.CreateEditPoint();
                EditPoint endPoint = sel.BottomPoint.CreateEditPoint();

                if (sel.Text.Length == 0)
                {
                    endPoint.Insert(textToInsert);
                }
                else
                {
                    endPoint.ReplaceText(startPoint, textToInsert, (int)EnvDTE.vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
                }
                startPoint.SmartFormat(endPoint);

            }
            finally
            {
                provider.UndoContext.Close();
            }
        }
    }
}
