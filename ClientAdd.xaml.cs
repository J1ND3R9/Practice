using Practice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Practice
{
    public partial class ClientAdd : Window
    {
        private static readonly Regex timeRegex = new Regex(@"^[0-9]");

        Model1 model = new Model1();
        public ClientAdd()
        {
            InitializeComponent();
            calendarRegister.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Now.AddDays(-1)));

            clientComboBox.ItemsSource = model.Client.OrderBy(s => s.ID)
                .Select(s => s.LastName + " " + s.FirstName + " " + s.Patronymic)
                .ToList();

            courseComboBox.ItemsSource = model.Service.OrderBy(s => s.ID)
                .Select(s => s.Title)
                .ToList();
        }

        private void clientAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (clientComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не выбрали клиента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (courseComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не выбрали курс", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (calendarRegister.SelectedDate == null)
            {
                MessageBox.Show("Вы не выбрали дату записи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (timeTextBox.Text.StartsWith("0"))
            {
                int h = Convert.ToInt32(timeTextBox.Text[1].ToString());
                if (h < 6 || h >= 22)
                {
                    MessageBox.Show("Время приема клиентов начинается с 7:00 до 22:00", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            Client client = model.Client.First(s => s.ID == clientComboBox.SelectedIndex + 1);
            if (haveSignedOnTime(client))
            {
                MessageBox.Show("Клиент уже записан на курсы в этот промежуток времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Service service = model.Service.First(s => s.Title == courseComboBox.SelectedItem.ToString());
            int hours = Convert.ToInt32(timeTextBox.Text);
            int minutes = Convert.ToInt32(timeMinTextBox.Text);
            ClientService clientService = new ClientService()
            {
                Client = client,
                ClientID = client.ID,
                ServiceID = service.ID,
                StartTime = Convert.ToDateTime(calendarRegister.SelectedDate).AddHours(hours).AddMinutes(minutes)
            };
            model.ClientService.Add(clientService);
            model.SaveChanges();
            MessageBox.Show("Клиент записан на курс", "Успешная запись", MessageBoxButton.OK);
            this.Close();
        }

        private bool haveSignedOnTime(Client client)
        {
            ClientService cs;
            try
            {
                cs = model.ClientService.OrderByDescending(c => c.StartTime).First(c => client.ID == c.ClientID);
            }
            catch
            {
                return false;
            }
            Service service = model.Service.First(s => s.ID ==  cs.ServiceID);
            DateTime startTime = cs.StartTime;
            DateTime endTime = startTime.AddSeconds(service.DurationInSeconds);
            if (calendarRegister.SelectedDate == startTime.Date)
            {
                int hours = Convert.ToInt32(timeTextBox.Text);
                int minutes = Convert.ToInt32(timeMinTextBox.Text);
                DateTime selectedTime = Convert.ToDateTime(calendarRegister.SelectedDate).AddHours(hours).AddMinutes(minutes);
                if (selectedTime >= startTime && selectedTime <= endTime)
                    return true;
                selectedTime = selectedTime.AddSeconds(service.DurationInSeconds);
                if (selectedTime >= startTime && selectedTime <= endTime)
                    return true;
            }
            return false;
        }

        private void timeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.Length < 1)
                tb.Text = "00";
            if (tb.Text.Length < 2)
            {
                string num = tb.Text;
                tb.Text = "0" + num;
            }
        }

        private void timeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!timeRegex.IsMatch(e.Text))
                e.Handled = true;
        }
        private void SpaceNotAllowed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
