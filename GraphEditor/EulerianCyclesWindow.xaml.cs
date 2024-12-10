using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GraphEditor
{
    public partial class EulerianCyclesWindow : Window
    {
        public EulerianCyclesWindow(List<List<Node>> cycles)
        {
            InitializeComponent();
            DisplayCycles(cycles);
        }

        private void DisplayCycles(List<List<Node>> cycles)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < cycles.Count; i++)
            {
                sb.AppendLine($"Цикл {i + 1}:");
                for (int j = 0; j < cycles[i].Count; j++)
                {
                    sb.Append(cycles[i][j].EllipseName);
                    if (j < cycles[i].Count - 1)
                        sb.Append(" -> ");
                }
                sb.AppendLine();
                sb.AppendLine();
            }

            CyclesTextBox.Text = sb.ToString();
        }
    }
}