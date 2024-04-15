using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Practice
{
    public partial class Window1 : Window
    {
        private protected readonly string password = "0000";

        public Image status;
        public DispatcherTimer timer;

        private int attempt = 3;
        private int sec = 30;
        public Window1(Image adminStatus)
        {
            InitializeComponent();
            this.status = adminStatus;
            passwordBox.Focus();
        }

        private void checkPassword_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password == password)
            {
                status.Tag = "True";
                this.Close();
            }
            if (String.IsNullOrEmpty(passwordBox.Password))
            {
                MessageBox.Show("Поле пустое!", null, MessageBoxButton.OK, MessageBoxImage.Warning);
                passwordBox.Focus();
            }
            else
            {
                attempt--;
                if (attempt <= 0)
                {
                    checkPassword.IsEnabled = false;
                    TimeSpan time = TimeSpan.FromSeconds(sec);
                    timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                    {
                        attemptsBox.Text = $"Вход разблокируется через {time}";
                        if (time == TimeSpan.Zero)
                        {
                            timer.Stop();
                            attemptsBox.Text = "";
                            checkPassword.IsEnabled = true;
                        }
                        time = time.Add(TimeSpan.FromSeconds(-1));
                    }, Application.Current.Dispatcher);
                    timer.Start();
                    sec += sec / 2;
                }
                else
                {
                    attemptsBox.Text = $"Осталось {attempt} попытки/ка входа";
                }
            }
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!checkPassword.IsEnabled)
                return;
            if (e.Key == Key.Enter)
            {
                checkPassword_Click(null, null);
            }
        }
    }
}
