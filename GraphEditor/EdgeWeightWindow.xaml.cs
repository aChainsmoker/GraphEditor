using System.Windows;
using System.Windows.Input;

namespace GraphEditor;

public partial class EdgeWeightWindow : Window
{
    private int weight;
    public int Weight{ get => weight; }
    
    public EdgeWeightWindow()
    {
        InitializeComponent();
    }
    
    private void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(weightInputTextbox.Text, out weight))
        {
            MessageBox.Show("Значение должно быть числом", "Ошибка", MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }
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