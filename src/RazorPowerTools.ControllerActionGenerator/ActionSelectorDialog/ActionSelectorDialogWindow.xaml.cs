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
    public partial class ActionSelectorDialogWindow : DialogWindow
    {
        public Action<string> ActionToDo { get; set; }
        public Action ActionToClose { get; set; }
        public ObservableCollection<IGenerationTemplate> Templates { get; set; }
        public ObservableCollection<ControllerType> Controllers { get; set; } = new ObservableCollection<ControllerType>();
        public ActionSelectorDialogWindow(List<ControllerType> controllers, Action<string> actionToDo) : base("Microsoft.VisualStudio.PlatformUI.DialogWindow")
        {
            try
            {
                foreach (var item in controllers)
                {
                    Controllers.Add(item);
                }
                ActionToDo = actionToDo;
                Templates = new ObservableCollection<IGenerationTemplate>();

                Templates.Add(new UrlActionGenerationTemplate());
                Templates.Add(new HtmlActionGenerationTemplate());
                Templates.Add(new HtmlActionLinkGenerationTemplate());
                Templates.Add(new AjaxActionLinkGenerationTemplate());
                Templates.Add(new HtmlFormGenerationTemplate());
                Templates.Add(new JsAjaxCallGenerationTemplate());
                Templates.Add(new JsAjaxLoadGenerationTemplate());
                Templates.Add(new UrlStringGenerationTemplate());
            }
            catch (Exception e)
            {


            }
            InitializeComponent();



        }

        string Generate()
        {
            var controllerAction = comboBox_actions.SelectedItem as ControllerAction;
            var generationTemplate = comboBox_templates.SelectedItem as IGenerationTemplate;
            if (controllerAction != null && generationTemplate != null)
            {
                return generationTemplate.Generate(controllerAction);
            }
            return "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ActionToClose?.Invoke();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            ActionToClose?.Invoke();
            ActionToDo?.Invoke(TextBox_result.Text);
        }

        private void comboBox_templatesOractions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TextBox_result != null)
            {
                TextBox_result.Text = Generate();
            }

        }


    }
}
