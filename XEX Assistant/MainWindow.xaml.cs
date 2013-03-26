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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Threading;
using XDevkit;
using System.Collections.ObjectModel;

namespace XEX_Assistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Elysium.Controls.Window
    {
        private string values;
        private int totalTime;
        private int remainingTime;
        private RealTimeEditing rte;

        private bool isPaused;

        public MainWindow(string Values)
        {
            values = Values;
            InitializeComponent();
            loadingBlock.Visibility = System.Windows.Visibility.Visible;
            XboxManager xmb = new XboxManager();
            rte = new RealTimeEditing();
            xdkName.Text = xmb.DefaultConsole;
            Elysium.Manager.Apply(Application.Current, Elysium.Theme.Light, Brushes.BlueViolet, Brushes.Black);
        }

        ObservableCollection<Offset> _OffsetCollection = new ObservableCollection<Offset>();

        public ObservableCollection<Offset> OffsetCollection
        { get { return _OffsetCollection; } }

        private void AddCheckBox(string offset, string type)
        {
            _OffsetCollection.Add(new Offset(offset, type));

            //CheckBox checkbox = new CheckBox();
            //checkbox.Content = offset;
            //checkbox.IsEnabled = false;
            //checkbox.Foreground = Brushes.Black;
            //offsetsList.Items.Add((object)checkbox);
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            string[] stringlst = values.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            string type = "";
            string offset = "";
            for (int i = 0; i < stringlst.Count(); i++)
            {
                try
                {
                    try
                    {
                        string[] temp = stringlst[i].Split('.');
                        type = temp[2];
                        temp = type.Split(' ');
                        type = temp[0];

                        string[] temp2 = stringlst[i].Split(':');
                        offset = temp2[1];
                        temp2 = offset.Split(' ');
                        offset = temp2[0];
                        AddCheckBox(offset, type);

                    }
                    catch
                    {
                        if (stringlst[i].Contains("bl "))
                        {
                            type = "float";

                            string[] temp2 = stringlst[i].Split(':');
                            offset = temp2[1];
                            temp2 = offset.Split(' ');
                            offset = temp2[0];
                            //addtxtbxs(o, t);
                        }
                    }
                }
                catch { }
            }
            loadingBlock.Visibility = System.Windows.Visibility.Hidden;
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (valuesBox.Text.Contains(','))
            {
                totalTime = Convert.ToInt32(timeBox.Text);
                StartIntervalPoking();
            }
            else
            {
                Elysium.Notifications.NotificationManager.BeginTryPush("Error", "Please fill in the values text box. ");
            }
        }

        private int currentOffset;
        private int totalOffsets;
        private int currentValue;
        private int totalValues;
        
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        private void StartIntervalPoking()
        {
            totalOffsets = offsetsList.Items.Count;
            totalValues = valuesBox.Text.Split(',').Count();

            timer.Enabled = true;
            timer.Interval = 1000;//(int)TimeSpan.FromSeconds(totalTime).Ticks;
            timer.Tick += new EventHandler(timer_Tick);
            remainingTime = totalTime;

            rte.PokeXbox(Convert.ToUInt32(OffsetCollection[0].Address, 0x10), "float", valuesBox.Text.Split(',')[0]);
            currentValue++;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                if (remainingTime-- == 0)
                {
                    if (currentOffset <= offsetsList.Items.Count)
                    {
                        offsetsList.SelectedIndex = currentOffset;
                        remainingTime = totalTime;
                        string offset = OffsetCollection[currentOffset].Address;
                        string value = valuesBox.Text.Split(',')[currentValue];
                        value = value.Replace("DEFAULT", OffsetCollection[currentOffset].Value);
                        if (value != "No Console Detected" && value != "Not Connected")
                        {
                            rte.PokeXbox(Convert.ToUInt32(offset, 0x10), OffsetCollection[currentOffset].Type, value);
                        }
                        if (currentValue == totalValues-1)
                        {
                            //(offsetsList.Items[currentOffset] as CheckBox).IsChecked = true;
                            currentOffset++;
                            currentValue = 0;
                        }
                        else
                        {
                            currentValue++;
                        }
                    }
                }
                else
                {
                    timeBar.Value = (((double)remainingTime / (double)totalTime) * 100);
                }
            }
        }

        private void connectToXDKButton_Click(object sender, RoutedEventArgs e)
        {
            if (!rte.isConnected)
            {
                rte = new RealTimeEditing(xdkName.Text);
                bool successfulConnection = rte.Connect();
                if (successfulConnection)
                {
                    connectToXDKButton.Content = "Disconnect";
                    connectToXDKButton.Background = Brushes.BlueViolet;
                    connectToXDKButton.BorderBrush = Brushes.BlueViolet;
                }
                else
                {
                    connectToXDKButton.Content = "Failure";
                    connectToXDKButton.Background = Brushes.Red;
                    connectToXDKButton.BorderBrush = Brushes.Red;
                }
            }
            else
            {
                rte.Disconnect();
                connectToXDKButton.Content = "Connect";
                connectToXDKButton.Background = Brushes.BlueViolet;
                connectToXDKButton.BorderBrush = Brushes.BlueViolet;
            }
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            isPaused = !isPaused;
            Image icon = new Image();
            if (isPaused)
            {
                BitmapImage playIcon = new BitmapImage();
                playIcon.BeginInit();
                playIcon.UriSource = new Uri("play.PNG", UriKind.Relative);
                playIcon.EndInit();
                icon.Source = playIcon;
            }
            else
            {
                BitmapImage pauseIcon = new BitmapImage();
                pauseIcon.BeginInit();
                pauseIcon.UriSource = new Uri("pause.PNG", UriKind.Relative);
                pauseIcon.EndInit();
                icon.Source = pauseIcon;
            }
            pauseButton.Content = icon;
        }
    }
}
