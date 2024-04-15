using Practice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

namespace Practice
{
    public partial class MainWindow : Window
    {
        Model1 model = new Model1();
        public bool adminStatus = false;
        public MainWindow()
        {
            InitializeComponent();
            //services.ItemsSource = model.Services.ToList();
            status.Tag = adminStatus;
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            Window1 login = new Window1(status);
            login.ShowDialog();
            login.Focus();
        }
        private void exitFromAdmin_Click(object sender, RoutedEventArgs e) => status.Tag = "False";
        private void filter_Selection(object sender, SelectionChangedEventArgs e)
        {
            if (defaultFilterComboBox.SelectedIndex == 0 || defaultFilterComboBox.SelectedIndex == -1)
                discountFilterFunc();
            else
                defaultFilter();
        }
        private void discountFilterFunc()
        {
            var discount = getTuple(filterDiscount.SelectedIndex);
            if (String.IsNullOrEmpty(findBox.Text))
                services.ItemsSource = model.Services.Where(s => s.Discount >= discount.Item1 && s.Discount < discount.Item2).ToList();
            else
                services.ItemsSource = model.Services.Where(s => (s.Discount >= discount.Item1 && s.Discount < discount.Item2) && (s.Name_s.Contains(findBox.Text))).ToList();
        }
        private void defaultFilter()
        {
            var discount = getTuple(filterDiscount.SelectedIndex);
            Func<Service, bool> whereFunc = getWhereFunc();
            Func<Service, decimal?> order = s => s.Price - (s.Price * s.Discount / 100);

            switch (defaultFilterComboBox.SelectedIndex)
            {
                case 0:
                    services.ItemsSource = model.Services.Where(whereFunc).ToList();
                    return;
                case 1:
                    services.ItemsSource = model.Services.Where(whereFunc).OrderByDescending(order).ToList();
                    return;
                case 2:
                    services.ItemsSource = model.Services.Where(whereFunc).OrderBy(order).ToList();
                    return;
                case 3:
                    services.ItemsSource = model.Services.Where(whereFunc).OrderBy(s => s.Name_s).ToList();
                    return;
            }

        }
        private Func<Service, bool> getWhereFunc()
        {
            var discount = getTuple(filterDiscount.SelectedIndex);

            if (String.IsNullOrEmpty(findBox.Text))
                return s => s.Discount >= discount.Item1 && s.Discount < discount.Item2;

            return s => (s.Discount >= discount.Item1 && s.Discount < discount.Item2) && (s.Name_s.Contains(findBox.Text));
        }
        private Tuple<int, int> getTuple(int i)
        {
            switch (i)
            {
                case 0:
                    return Tuple.Create(0, 100);
                case 1:
                    return Tuple.Create(0, 5);
                case 2:
                    return Tuple.Create(5, 15);
                case 3:
                    return Tuple.Create(15, 30);
                case 4:
                    return Tuple.Create(30, 70);
                case 5:
                    return Tuple.Create(70, 100);
            }
            return Tuple.Create(0, 100);
        }
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            long tag = Convert.ToInt64(button.Tag);
            Service service;
            try
            {
                service = model.Services.First(s => s.ID == tag);
            }
            catch
            {
                MessageBox.Show("Курс не существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                services.ItemsSource = model.Services.ToList();
                return;
            }
            List<ClientService> clientS = model.ClientServices.Where(c => c.ServiceID == service.ID).ToList();
            if (clientS.Count > 0)
            {
                MessageBox.Show("К данному курсу уже записан(ы) клиент(ы)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBoxResult result = MessageBox.Show($"Вы уверены что хотите удалить курс под названием \"{service.Name_s}\"?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:

                    model.Services.Remove(service);
                    model.SaveChanges();
                    services.ItemsSource = model.Services.ToList();
                    return;
            }
        }
        private bool haveFilters() => (filterDiscount.SelectedIndex != 0 || filterDiscount.SelectedIndex != -1) && (defaultFilterComboBox.SelectedIndex != 0 || defaultFilterComboBox.SelectedIndex != -1);
        private void findBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (haveFilters())
            {
                filter_Selection(null, null);
                return;
            }
            services.ItemsSource = model.Services.Where(s => s.Name_s.Contains(findBox.Text)).ToList();
        }
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            long id = Convert.ToInt64(button.Tag);
            Service service;
            try
            {
                service = model.Services.First(s => s.ID == id);
            }
            catch
            {
                MessageBox.Show("Курс не существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                services.ItemsSource = model.Services.ToList();
                return;
            }
            Edit edit = new Edit(services, service);
            edit.ShowDialog();
        }
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            Edit edit = new Edit(services);
            edit.ShowDialog();
        }
        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            services.ItemsSource = null;
            if (String.IsNullOrEmpty(findBox.Text) && haveFilters())
            {
                services.ItemsSource = model.Services.ToList();
            }
            else
                findBox_TextChanged(null, null);
        }
        private void addClient_Click(object sender, RoutedEventArgs e)
        {
            ClientAdd windowClient = new ClientAdd();
            windowClient.ShowDialog();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
                refreshButton_Click(null, null);
        }
    }
}
