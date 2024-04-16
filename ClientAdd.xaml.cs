using Practice.Model;
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

namespace Practice
{
    /// <summary>
    /// Логика взаимодействия для ClientAdd.xaml
    /// </summary>
    public partial class ClientAdd : Window
    {
        Model1 model = new Model1();
        public ClientAdd()
        {
            InitializeComponent();
            calendarRegister.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Now.AddDays(-1)));

            clientComboBox.ItemsSource = model.Clients.OrderBy(s => s.ID)
                .Select(s => s.LastName + " " + s.FirstName + " " + s.Patronymic)
                .ToList();

            courseComboBox.ItemsSource = model.Services.OrderBy(s => s.ID)
                .Select(s => s.Name_s)
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
            Client client = model.Clients.First(s => s.ID == clientComboBox.SelectedIndex + 1);
            Service service = model.Services.First(s => s.Name_s == courseComboBox.SelectedItem.ToString());
            ClientService clientService = new ClientService()
            {
                Client = client,
                ClientID = client.ID,
                ServiceID = service.ID,
                StartTime = Convert.ToDateTime(calendarRegister.SelectedDate)
            };
            model.ClientServices.Add(clientService);
            model.SaveChanges();
            MessageBox.Show("Клиент записан на курс", "Успешная запись", MessageBoxButton.OK);
            this.Close();
        }
    }
}
