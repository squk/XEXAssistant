using System;
using System.Collections.Generic;
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

namespace XEX_Assistant
{
    /// </summary>
    public partial class ParsingWindow : Elysium.Controls.Window
    {
        public ParsingWindow()
        {
            InitializeComponent();
            Elysium.Manager.Apply(Application.Current, Elysium.Theme.Light, Brushes.YellowGreen, Brushes.Black);
        }

        private void parseValuesButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(valuesBox.Text).Show();
        }

        string GetStringFromRichTextBox(RichTextBox rtb)
        {
            var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return textRange.Text;
        }
    }
}
