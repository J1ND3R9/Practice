using Practice.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
    public partial class Edit : Window
    {
        private static readonly Regex priceRegex = new Regex(@"[\d,]");
        private static readonly Regex discountRegex = new Regex(@"\d");
        private static readonly Regex timeRegex = new Regex(@"[\dчсм]");

        private Services service;
        private ListBox lb;
        public Edit(ListBox lb, Services service = null)
        {
            InitializeComponent();
            this.service = service;
            this.lb = lb;
            if (service != null)
            {
                ServiceName.Text = service.Service_Name;
                ServiceDesc.Text = service.Description;
                priceInput.Text = service.Price.ToString();
                discountInput.Text = service.Discount.ToString();
            }
            else
            {
                this.Title = "Добавление курса";
                ServiceName.Focus();
            }
        }

        private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!priceRegex.IsMatch(e.Text))
                e.Handled = true;
        }

        private void discountInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!discountRegex.IsMatch(e.Text))
                e.Handled = true;
        }

        private void ServiceTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!timeRegex.IsMatch(e.Text))
                e.Handled = true;
            if (ServiceTime.Text.Contains("ч") || ServiceTime.Text.Contains("м") || ServiceTime.Text.Contains("с"))
                e.Handled = true;
        }
        private void saveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceName.Text.Length == 0 || ServiceName.Text.Length > 100)
            {
                messageErrorByID(1);
                return;
            }

            if (ServiceDesc.Text.Length > 300)
            {
                messageErrorByID(2);
                return;
            }

            decimal price;
            try
            {
                price = Convert.ToDecimal(priceInput.Text);
            }
            catch
            {
                messageErrorByID(3);
                return;
            }

            if (price <= 0)
            {
                messageErrorByID(4);
                return;
            }

            byte discount;
            try
            {
                discount = Convert.ToByte(discountInput.Text);
            }
            catch
            {
                messageErrorByID(5);
                return;
            }
            if (discount < 0 || discount >= 100)
            {
                messageErrorByID(6);
                return;
            }

            int time;
            try
            {
                string timeText = ServiceTime.Text;
                //string timeFormat = timeText.Substring(timeText.Length - 1);
                timeText = timeText.Remove(timeText.Length - 1);
                time = Convert.ToInt32(timeText);
            }
            catch
            {
                messageErrorByID(0);
                return;
            }
            editCourse();
        }

        private int getTime()
        {
            string timeText = ServiceTime.Text;
            string timeFormat = timeText.Substring(timeText.Length - 1);
            timeText = timeText.Remove(timeText.Length - 1);
            int timeInt = Convert.ToInt32(timeText);
            switch (timeFormat)
            {
                case "ч":
                    return timeInt * 60 * 60;
                case "м":
                    return timeInt * 60;
            }
            return timeInt;
        }
        private void editCourse()
        {
            Model1 model = new Model1();

            if (service == null)
                service = new Services();

            service.Service_Name = ServiceName.Text;
            service.Description = ServiceDesc.Text;
            service.Price = Convert.ToDecimal(priceInput.Text);
            service.Discount = Convert.ToByte(discountInput.Text);
            service.Time = getTime();
            model.Services.AddOrUpdate(service);
            model.SaveChanges();
            lb.ItemsSource = model.Services.ToList();
            this.Close();
        }
        private void messageErrorByID(int ID)
        {
            string descError = "";
            switch (ID)
            {
                case 0:
                    descError = "Ошибка преобразования времени курса, проверьте введённые данные";
                    ServiceTime.Focus();
                    break;
                case 1:
                    descError = "Наименование курса не должно быть пустое или превышать более 100 символов";
                    ServiceName.Focus();
                    break;
                case 2:
                    descError = "Описание курса не должно превышать более 300 символов";
                    ServiceDesc.Focus();
                    break;

                case 3:
                    descError = "Ошибка преобразования стоимости курса, проверьте введённые данные";
                    priceInput.Focus();
                    break;

                case 4:
                    descError = "Стоимость курса не должна быть меньше 1 рубля";
                    priceInput.Focus();
                    break;

                case 5:
                    descError = "Ошибка преобразования скидки, проверьте введённые данные";
                    discountInput.Focus();
                    break;

                case 6:
                    descError = "Скидка не должна быть меньше 0% или больше 99%";
                    discountInput.Focus();
                    break;
            }
            MessageBox.Show(descError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
