using SteeringWheelApp.Models.Entities;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SteeringWheelApp.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для окна AuthWindow.
    /// Это первое окно, которое видит пользователь.
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void OnWindowKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Button_Click(default, default);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (var login, var password) = (loginInputBox.Text, passwordInputBox.Password);
            if (DemoExamDataContext.Instance.Users.FirstOrDefault(user => user.Login == login &&
                                                                          user.Password == password) is User user)
            {
                NavigateToProductsWindow(user);
            }
            else
            {
                MessageBox.Show("Аккаунт с указанными данными не обнаружен.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) => NavigateToProductsWindow(null);

        private void NavigateToProductsWindow(User? user)
        {
            new ProductsWindow(Order.GenerateNewOrder(user)).Show();
            Close();
        }
    }
}
