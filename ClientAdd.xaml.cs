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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Practice
{
    public partial class ClientAdd : Window
    {
        private readonly Regex letters = new Regex(@"^[А-я-]$");
        private readonly Regex phoneNumber = new Regex(@"^[0-9+]");
        public Model1 model = new Model1();
        public ClientAdd()
        {
            InitializeComponent();
            CoursesList.ItemsSource = model.Services.Select(s => s.Service_Name).ToList();
        }

        private void OnlyLetters_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!letters.IsMatch(e.Text))
                e.Handled = true;
        }

        private void NumsOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!phoneNumber.IsMatch(e.Text) || e.Text == " ")
                e.Handled = true;
        }

        private void RefreshComboBox_Click(object sender, RoutedEventArgs e)
        {
            CoursesList.ItemsSource = model.Services.Select(s => s.Service_Name).ToList();
        }

        private void clientAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!thereAreNoErrors())
                return;
            Clients cl = new Clients()
            {
                LastName = LastNameClient.Text,
                FirstName = FirstNameClient.Text,
                Patronymic = PatronymicClient.Text,
                Phone = PhoneClient.Text,
                Course = model.Services.First(s => s.Service_Name == CoursesList.SelectedItem.ToString()).ID
            };
            if (cl.Course == 0)
            {
                MessageBox.Show($"Ошибка записи клиента:\nКурс не существует", null, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                model.Clients.Add(cl);
                model.SaveChanges();
                MessageBox.Show("Клиент успешно записан!", "Запись создана", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка записи клиента:\n{ex.Message}", null, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #region Поиск ошибок
        private bool thereAreNoErrors()
        {
            if (LastNameClient.Text.Length < 1)
            {
                showError(1);
                LastNameClient.Focus();
                return false;
            }
            if (LastNameClient.Text.Contains("-"))
            {
                int hyphens = LastNameClient.Text.Count(c => c == '-');
                if (hyphens > 1)
                {
                    showError(-1);
                    LastNameClient.Focus();
                    return false;
                }
                else
                {
                    string text = LastNameClient.Text.Trim('-');
                    if (!String.IsNullOrEmpty(text))
                    {
                        showError(1);
                        LastNameClient.Focus();
                        return false;
                    }
                }
            }
            if (FirstNameClient.Text.Length < 1)
            {
                showError(2);
                FirstNameClient.Focus();
                return false;
            }
            if (FirstNameClient.Text.Contains("-"))
            {
                int hyphens = FirstNameClient.Text.Count(c => c == '-');
                if (hyphens > 1)
                {
                    showError(0);
                    FirstNameClient.Focus();
                    return false;
                }
                else
                {
                    string text = FirstNameClient.Text.Trim('-');
                    if (!String.IsNullOrEmpty(text))
                    {
                        showError(2);
                        FirstNameClient.Focus();
                        return false;
                    }
                }
            }
            if (PatronymicClient.Text.Length < 1)
            {
                showError(3);
                PatronymicClient.Focus();
                return false;
            }
            if (PhoneClient.Text.Length != 18)
            {
                showError(4);
                PhoneClient.Focus();
                return false;
            }
            if (CoursesList.SelectedIndex == -1)
            {
                showError(5);
                CoursesList.Focus();
                return false;
            }
            try
            {
                Clients cl = model.Clients.First(s => s.Phone == PhoneClient.Text);
                showError(6);
                PhoneClient.Focus();
                return false;
            }
            catch { }
            return true;
        }

        private void showError(int ID)
        {
            string errorDesc = "";
            switch (ID)
            {
                case -1:
                    errorDesc = "Дефисов в фамилии не может быть более одной штуки";
                    break;
                case 0:
                    errorDesc = "Дефисов в имени не может быть более одной штуки";
                    break;
                case 1:
                    errorDesc = "Поле фамилии клиента не может быть пустым";
                    break;
                case 2:
                    errorDesc = "Поле имени клиента не может быть пустым";
                    break;
                case 3:
                    errorDesc = "Поле отчества клиента не может быть пустым";
                    break;
                case 4:
                    errorDesc = "Номер телефона не совпадает с форматом.\nФормат: +79009009090";
                    break;
                case 5:
                    errorDesc = "Не выбран курс для записи клиента";
                    break;
                case 6:
                    errorDesc = "Клиент с таким телефоном уже существует";
                    break;
            }
            MessageBox.Show(errorDesc, "Ошибка" ,MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        private bool formattedPhone()
        {
            if (PhoneClient.Text.Contains("(") && PhoneClient.Text.Contains(")")
                && PhoneClient.Text.Contains(" ") && PhoneClient.Text.Contains("-")
                && PhoneClient.Text.Length == 18)
            {
                int spaces = PhoneClient.Text.Count(c => c == ' ');
                int hyphens = PhoneClient.Text.Count(c => c == '-');
                if (spaces == 1 && hyphens == 3)
                    return true;
            }
            return false;
        }
        #region LostFocuses
        private void PhoneClient_LostFocus(object sender, RoutedEventArgs e)
        {
            if (formattedPhone())
                return;
            if (String.IsNullOrEmpty(PhoneClient.Text) || !PhoneClient.Text.Contains("+7") || PhoneClient.Text.Length != 12 || PhoneClient.Text.Contains(" "))
            {
                errorTB((TextBox)sender);
                return;
            }
            string formattingPhone = $"+7 ({PhoneClient.Text.Substring(2, 3)})-{PhoneClient.Text.Substring(5, 3)}-{PhoneClient.Text.Substring(8, 2)}-{PhoneClient.Text.Substring(10, 2)}";
            PhoneClient.Text = formattingPhone;
            correctTB((TextBox)sender);
        }
        private void LastNameClient_LostFocus(object sender, RoutedEventArgs e)
        {
            if (LastNameClient.Text.Length < 1)
            {
                errorTB((TextBox)sender);
                return;
            }
            if (LastNameClient.Text.Contains("-"))
            {
                int hyphens = LastNameClient.Text.Count(c => c == '-');
                if (hyphens > 1)
                {
                    errorTB((TextBox)sender);
                    return;
                }
                else
                {
                    string text = LastNameClient.Text.Trim('-');
                    if (String.IsNullOrEmpty(text))
                    {
                        errorTB((TextBox)sender);
                        return;
                    }
                }
            }
            LastNameClient.Text = FormattingPersonNames(LastNameClient);
            correctTB((TextBox)sender);
        }

        private void FirstNameClient_LostFocus(object sender, RoutedEventArgs e)
        {
            if (FirstNameClient.Text.Length < 1)
            {
                errorTB((TextBox)sender);
                return;
            }
            if (FirstNameClient.Text.Contains("-"))
            {
                int hyphens = FirstNameClient.Text.Count(c => c == '-');
                if (hyphens > 1)
                {
                    errorTB((TextBox)sender);
                    return;
                }
                else
                {
                    string text = FirstNameClient.Text.Trim('-');
                    if (String.IsNullOrEmpty(text))
                    {
                        errorTB((TextBox)sender);
                        return;
                    }
                }
            }
            
            FirstNameClient.Text = FormattingPersonNames(FirstNameClient);
            correctTB((TextBox)sender);
        }

        private void PatronymicClient_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PatronymicClient.Text.Length < 1)
            {
                errorTB((TextBox)sender);
                return;
            }
            PatronymicClient.Text = FormattingPersonNames(PatronymicClient);
            correctTB((TextBox)sender);
        }

        private string FormattingPersonNames(TextBox tb)
        {
            if (tb.Text.EndsWith("-"))
                tb.Text = tb.Text.Remove(tb.Text.Length - 1, 1);
            char[] chars = tb.Text.ToCharArray();
            chars[0] = char.ToUpper(chars[0]);
            if (chars.Contains('-'))
            {
                for (int i = 1; i < chars.Length; i++)
                {
                    if (chars[i] == '-')
                    {
                        chars[i + 1] = char.ToUpper(chars[i + 1]);
                        i++;
                        continue;
                    }
                    chars[i] = char.ToLower(chars[i]);
                }
            }
            else
            {
                for (int i = 1; i < chars.Length; i++)
                    chars[i] = char.ToLower(chars[i]);
            }
            return new string(chars);
        }
        private void correctTB(TextBox tb) => tb.Background = new SolidColorBrush(Color.FromRgb(229, 255, 204));
        private void errorTB(TextBox tb) => tb.Background = new SolidColorBrush(Color.FromRgb(255, 204, 204));
        #endregion

        private void SpaceNotAllowed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
