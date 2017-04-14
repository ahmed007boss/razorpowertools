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
using System.Windows;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RazorPowerTools.ControllerActionGenerator;
using System.IO;

namespace RazorPowerTools.VSIX.ControllerActionGeneratorCommand
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class MoveToParialViewVSCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0200;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("C4018B26-7D98-4E02-8B4B-EC4000E510EB");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerActionGeneratorVSCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private MoveToParialViewVSCommand(Package package)
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
        public static MoveToParialViewVSCommand Instance
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
            Instance = new MoveToParialViewVSCommand(package);
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
            IVsUIShell uiShell = (IVsUIShell)ServiceProvider.GetService(typeof(SVsUIShell));
            uiShell.EnableModeless(0);
            var xamlDialog = new TextDialogWindow("Razor File Name", MoveSelectionToRazorFile)
            {
                Owner = Application.Current.MainWindow
            };


            xamlDialog.HasMinimizeButton = false;
            xamlDialog.HasMaximizeButton = true;
            xamlDialog.MaxHeight = 140;
            xamlDialog.MinHeight = 140;
            xamlDialog.MaxWidth = 450;
            xamlDialog.MinWidth = 450;
            xamlDialog.Title = "Move To New Razor File";

            xamlDialog.ActionToClose = xamlDialog.Close;

            xamlDialog.ShowDialog();
            uiShell.EnableModeless(1);

        }

        internal void MoveSelectionToRazorFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return;
            }
            try
            {
                DTE2 provider = Package.GetGlobalService(typeof(SDTE)) as DTE2;
                TextSelection selection = (TextSelection)provider?.ActiveDocument?.Selection;
                var textToReplace = selection.Text;
               
             

                if (selection.Text.Count() > 0)
                {
                    EditPoint startPoint = selection.TopPoint.CreateEditPoint();
                    EditPoint endPoint = selection.BottomPoint.CreateEditPoint();
                    endPoint.ReplaceText(startPoint, $"@Html.Partial(\"{filename}\")", (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);


                    CreateNewcshtmlFile(filename, textToReplace);
                }


            }
            catch (Exception ex)
            {


            }
        }

        internal  void CreateNewcshtmlFile( string title, string fileContents)
        {
            DTE2 provider = Package.GetGlobalService(typeof(SDTE)) as DTE2;
            if (provider == null || provider.ActiveDocument == null) return;
            var x = provider.ActiveDocument.Path.ToString() + title + ".cshtml";
            
            using(var stream = new FileStream(x, FileMode.OpenOrCreate))
            {
             
            }
            var file = provider.ItemOperations.AddExistingItem(x).Open();
          
            if (file?.Document == null) return;

            file?.Document?.Activate();

            if (!String.IsNullOrEmpty(fileContents))
            {
                TextSelection selection = (TextSelection)file?.Document?.Selection;
                selection.SelectAll();
                selection.Text = "";
                selection.Insert(fileContents);
            }
        }

    }
}
