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
using HaloDevelopmentExtender;
using HaloReach3d;

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

        private bool getValueFromXbox;

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
            _OffsetCollection.Add(new Offset(offset, type, getValueFromXbox));
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            XboxDebugCommunicator Xbox_Debug_Communicator = new XboxDebugCommunicator(new XboxManager().DefaultConsole);
            //Connect
            if (Xbox_Debug_Communicator.Connected == false)
            {
                try
                {
                    Xbox_Debug_Communicator.Connect();
                    getValueFromXbox = true;
                }
                catch
                {
                    getValueFromXbox = false;
                }
            }
            else
            {
                getValueFromXbox = true;
            }

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
                            AddCheckBox(offset, type);
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
        private int currentBatch;
        private int offsetsPerBatch;

        private int currentValue;
        private int totalValues;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private List<List<Offset>> offsetBatches = new List<List<Offset>>();
        private bool useBatchPoking = false;

        private void StartIntervalPoking()
        {
            totalValues = valuesBox.Text.Split(',').Count();
            
            //prevent checking/unchecking once process has started
            if (batchTestingCheck.IsChecked == true)
            {
                useBatchPoking = true;
                offsetsPerBatch = Convert.ToInt32(batchesBox.Text);
                for (int i = 0; i < OffsetCollection.Count; i += Convert.ToInt32(batchesBox.Text))
                {
                    List<Offset> singleBatch = new List<Offset>();
                    for (int j = 0; j < Convert.ToInt32(batchesBox.Text); j++)
                    {
                        try
                        {
                            singleBatch.Add(OffsetCollection[i + j]);
                        }
                        catch
                        {

                        }
                    }
                    offsetBatches.Add(singleBatch);
                }
            }

            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
            remainingTime = totalTime;
            if (useBatchPoking)
            {
                foreach (Offset offsetSinglet in offsetBatches[0])
                {
                    Offset OffsetSinglet = offsetSinglet;
                    OffsetSinglet.Value = valuesBox.Text.Split(',')[0];
                    rte.PokeXbox(OffsetSinglet);
                }
            }
            else
            {
                rte.PokeXbox(new Offset(OffsetCollection[0].Address, "float", valuesBox.Text.Split(',')[0]));
            }
            currentValue++;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                if (remainingTime-- == 0)
                {
                    if (useBatchPoking == true)
                    {
                        if (currentBatch <= offsetBatches.Count)
                        {
                            remainingTime = totalTime;
                            offsetsList.SelectedIndex = currentBatch * offsetsPerBatch;
                            string offset = OffsetCollection[currentOffset].Address;
                            string type = OffsetCollection[currentOffset].Type;
                            string value = valuesBox.Text.Split(',')[currentValue];
                            value = value.Replace("DEFAULT", OffsetCollection[currentOffset].Value);

                            if (value != "No Console Detected" && value != "Not Connected")
                            {
                                foreach(Offset offsetSinglet in offsetBatches[currentBatch])
                                {
                                    Offset OffsetSinglet = offsetSinglet;
                                    OffsetSinglet.Value = value;
                                    rte.PokeXbox(OffsetSinglet);
                                }
                            }
                            if (currentValue == totalValues - 1)
                            {
                                currentBatch++;
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
                        if (currentOffset <= offsetsList.Items.Count)
                        {
                            offsetsList.SelectedIndex = currentOffset;
                            remainingTime = totalTime;

                            string offset = OffsetCollection[currentOffset].Address;
                            string type = OffsetCollection[currentOffset].Type;
                            string value = valuesBox.Text.Split(',')[currentValue];
                            value = value.Replace("DEFAULT", OffsetCollection[currentOffset].Value);

                            if (value != "No Console Detected" && value != "Not Connected")
                            {
                                rte.PokeXbox(new Offset(offset, type, value));
                            }
                            if (currentValue == totalValues - 1)
                            {
                                currentOffset++;
                                currentValue = 0;
                            }
                            else
                            {
                                currentValue++;
                            }
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

        private void batchTestingCheck_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}