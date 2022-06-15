using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace launcher_manager
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : Window
    {
        public ControlPanel()
        {
            InitializeComponent();

        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            labelUser.Content = Messages.AuthenticationJson["user_name"].ToString();
            labelToken.Content = Messages.AuthenticationJson["authorization_token"].ToString().Substring(0, 8) + "**** ...";
            DateTime d = DateTime.Parse(Messages.AuthenticationJson["exp"].ToString()).ToLocalTime();
            labelExp.Content = d.ToLongTimeString();

            System.Windows.Threading.DispatcherTimer updateDevConsole = new System.Windows.Threading.DispatcherTimer();
            updateDevConsole.Tick += updateDevConsole_tick;
            updateDevConsole.Interval = new TimeSpan(0, 0, 2);
            updateDevConsole.Start();

            System.Windows.Threading.DispatcherTimer heartbeatTimer = new System.Windows.Threading.DispatcherTimer();
            heartbeatTimer.Tick += heartbeat_tick;
            heartbeatTimer.Interval = new TimeSpan(0, 0, 45);
            heartbeatTimer.Start();
        }

        private async void heartbeat_tick(object sender, EventArgs e)
        {
            if (Messages.LoggedIn)
            {
                string token = Messages.AuthenticationJson["authorization_token"].ToString();
                string b = await Messages.Heartbeat(token);
                if (Messages.Expiration.Length > 0)
                {
                    DateTime d = DateTime.Parse(Messages.Expiration.ToString()).ToLocalTime();
                    labelExp.Content = d.ToLongTimeString();
                }
            }
        }

        private void updateDevConsole_tick(object sender, EventArgs e)
        {
            textBlock.Text = "";
            for (int i=Messages.DevConsole.Count-10;i<Messages.DevConsole.Count;i++)
            {
                if (i >= 0)
                    textBlock.Text += Messages.DevConsole[i] + "\n";
            }
        }
    }
}
