﻿//------------------------------------------------------------------------------
// <copyright file="ControllerActionGeneratorVSCommand.cs" company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Windows;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.PlatformUI;
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
                var controllersClasses = ClassesMetadata.GetClasses().ToList();

                List<ControllerType> controllers = new List<ControllerType>();

                foreach (var cnrtclass in controllersClasses)
                {
                    try
                    {
                        var newCnt = new ControllerType();


                        newCnt.Name = cnrtclass.Name;
                        newCnt.Functions = new List<ControllerAction>();
                        #region Functions
                        var funcClass = cnrtclass.Children.OfType<CodeFunction2>().Where(func => func.Access == vsCMAccess.vsCMAccessPublic && func.FunctionKind == vsCMFunction.vsCMFunctionFunction).ToList();
                        foreach (var fun in funcClass)
                        {
                            try
                            {
                                var newCntaction = new ControllerAction();
                                newCntaction.Name = fun.Name;
                                newCntaction.ControllerName = newCnt.Name;
                                newCntaction.Name = fun.Name;
                                newCntaction.returnType = (fun.Type?.CodeType as CodeClass2)?.Name ?? "void";
                                newCntaction.ActionVerb = GetVerb(fun);
                                newCntaction.Parameters = new List<ControllerActionParameter>();
                                var funParams = fun.Parameters.OfType<CodeParameter2>()?.ToList();
                                foreach (var prm in funParams)
                                {
                                    try
                                    {
                                        ControllerActionParameter p = new ControllerActionParameter()
                                        {
                                            Name = prm.Name,
                                            TypeName = prm?.Type?.CodeType?.Name
                                        };
                                        newCntaction.Parameters.Add(p);


                                    }
                                    catch (Exception e1)
                                    {


                                    };


                                }

                                newCnt.Functions.Add(newCntaction);
                            }
                            catch (Exception e2)
                            {


                            }
                        }
                        #endregion

                        controllers.Add(newCnt);
                    }
                    catch (Exception e3)
                    {


                    }
                }


              

                IVsUIShell uiShell = (IVsUIShell)ServiceProvider.GetService(typeof(SVsUIShell));
                uiShell.EnableModeless(0);
                var xamlDialog = new ActionSelectorDialogWindow(controllers, InsertText)
                {
                    Owner = Application.Current.MainWindow
                };


                xamlDialog.HasMinimizeButton = false;
                xamlDialog.HasMaximizeButton = true;
                xamlDialog.MaxHeight = 440;
                xamlDialog.MinHeight = 340;
                xamlDialog.MaxWidth = 800;
                xamlDialog.MinWidth = 300;
                xamlDialog.Title = "Generate From Action";

                xamlDialog.ActionToClose = xamlDialog.Close;

                xamlDialog.ShowDialog();
                uiShell.EnableModeless(1);

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
            var provider = ServiceProvider.GetService(typeof(DTE)) as DTE;

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
                    endPoint.ReplaceText(startPoint, textToInsert, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
                }
                //  startPoint.SmartFormat(endPoint);

            }
            finally
            {
                provider.UndoContext.Close();
            }
        }
    }
}
