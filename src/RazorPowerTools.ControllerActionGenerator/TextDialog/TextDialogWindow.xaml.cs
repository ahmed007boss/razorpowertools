using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.PlatformUI;
using RazorPowerTools.ControllerActionGenerator.GenerationTemplates;

namespace RazorPowerTools.ControllerActionGenerator
{
    /// <summary>
    /// Interaction logic for ActionSelectorDialogWindow.xaml
    /// </summary>
    public partial class TextDialogWindow : DialogWindow
    {
        public Action<string> ActionToDo { get; set; }
        public Action ActionToClose { get; set; }

        public TextDialogWindow(string labelName, Action<string> actionToDo) : base("Microsoft.VisualStudio.PlatformUI.DialogWindow")
        {

            InitializeComponent();

            Label_TextBox_Label.Content = labelName;
            ActionToDo = actionToDo;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ActionToClose?.Invoke();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBox_result.Text))
            {
                ActionToClose?.Invoke();
                ActionToDo?.Invoke(TextBox_result.Text);
            }

        }



    }
}
