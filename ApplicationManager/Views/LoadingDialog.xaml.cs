using System.Windows;
using System.Windows.Input;

namespace ApplicationManager.Views
{
    public partial class LoadingDialog : Window
    {
        public LoadingDialog(string message)
        {
            InitializeComponent();

            MessageTextBlock.Text = message;
        }
        
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}