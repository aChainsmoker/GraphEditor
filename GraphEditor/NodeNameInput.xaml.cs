using System.Windows;
using System.Windows.Input;

namespace GraphEditor
{
    public partial class NodeNameInput : Window
    {
        public string NodeName { get; private set; }

        public NodeNameInput()
        {
            InitializeComponent();
            nameInputTextBox.Focus();
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            NodeName = nameInputTextBox.Text;
            this.DialogResult = true;
        }
        private void OnKeyPressedTextBox(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                OnOkButtonClick(sender, e);
            }
        }

    }
}



