using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Xml;

namespace XEX_Assistant
{
    /// </summary>
    public partial class ParsingWindow : Elysium.Controls.Window
    {
        public ParsingWindow()
        {
            InitializeComponent();
            CheckForUpdate();
            Elysium.Manager.Apply(Application.Current, Elysium.Theme.Light, Brushes.YellowGreen, Brushes.Black);
        }

        private void parseValuesButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(valuesBox.Text).Show();
        }

        private void CheckForUpdate()
        {
            string downloadUrl = "";
            Version newVersion = null;
            string aboutUpdate = "";
            string xmlUrl = "http://www.ctnieves.com/projects/software.xml";
            XmlTextReader reader = new XmlTextReader(xmlUrl);
            XmlDocument updateDoc = new XmlDocument();
            updateDoc.Load(reader);
            XmlNode softwareNameNode = updateDoc.SelectSingleNode("ctnievesSoftware").SelectSingleNode("XEXAssistant");

            newVersion = Version.Parse(softwareNameNode.SelectSingleNode("version").InnerText);
            downloadUrl = softwareNameNode.SelectSingleNode("url").InnerText;
            aboutUpdate = softwareNameNode.SelectSingleNode("about").InnerText;

            reader.Close();
            reader.Dispose();

            Version applicationVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (applicationVersion.CompareTo(newVersion) < 0)
            {
                MessageBox.Show("New Version Available : " + newVersion.ToString() + "\n" +
                                "Url : " + downloadUrl + "\n" +
                                "About Update : " + aboutUpdate);
                Process.Start(downloadUrl);
            }
        }
    }
}
