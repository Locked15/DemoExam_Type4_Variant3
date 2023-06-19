using SteeringWheelApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SteeringWheelApp.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProductsWindow.
    /// В этом окне отображаются все товары.
    /// </summary>
    public partial class ProductsWindow : Window
    {
        /// <summary>
        /// Это поле автоматически инициализируется после полной загрузки окна.
        /// </summary>
        private List<Product> _selectedProducts = null!;

        public Order CurrentOrder { get; private set; }

        public ProductsWindow(Order currentOrder)
        {
            InitializeComponent();
            CurrentOrder = currentOrder;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Установив обработчик таким образом автоматически будет вызвана вся цепочка, приводящая к заполнению списка товаров.
            searchInputBox.TextChanged += OnSearchInputBoxTextChanged;
            searchInputBox.Text = string.Empty;

            UpdateOrderNavigationVisibility();
        }

        private void UpdateOrderNavigationVisibility() => goToOrderButton.Visibility = CurrentOrder.OrderProducts.Any() ? Visibility.Visible : Visibility.Hidden;

        /// <summary>
        /// Событие происходит при обновлении поискового запроса.
        /// Поиск производится по названию и описанию товара.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void OnSearchInputBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            // Для упрощения работы был использован приём Pattern Matching (в данной ситуации запись в переменную прямо в условном блоке).
            _selectedProducts = DemoExamDataContext.Instance.Products.ToList();
            if (searchInputBox.Text is string search && !string.IsNullOrWhiteSpace(search))
            {
                _selectedProducts = _selectedProducts.Where(product => product.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                                                       product.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) == true
                                                           ).ToList();
            }

            // Вывод сообщения производится уже после обновления списка, для наглядности.
            productsListView.ItemsSource = _selectedProducts;
            if (!_selectedProducts.Any())
                MessageBox.Show("По поисковому запросу ничего не найдено.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void OnProductsListViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (productsListView.SelectedItem is Product product)
            {
                CurrentOrder.TryToAddNewProduct(product);
                UpdateOrderNavigationVisibility();

                MessageBox.Show($"К товару добавлено: {product.Name}.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnAddToOrderMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (productsListView.SelectedItem is Product product)
            {
                CurrentOrder.TryToAddNewProduct(product);
                UpdateOrderNavigationVisibility();
            }
        }

        private void OnGoToOrderButtonClick(object sender, RoutedEventArgs e)
        {
            // Результат диалога является Null-совместимым типом, так что его нужно напрямую сравнивать с 'True'.
            var newWindow = new OrderFormationWindow(CurrentOrder);
            if (newWindow.ShowDialog() == true)
            {
                CurrentOrder = Order.GenerateNewOrder(CurrentOrder.User);
                MessageBox.Show("Начато формирование нового заказа.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Так как в окне формирования заказа можно менять сам заказ.
                CurrentOrder = newWindow.CurrentOrder;
            }

            UpdateOrderNavigationVisibility();
        }

        private void OnExitButtonClick(object sender, RoutedEventArgs e) => Close();

        /// <summary>
        /// При закрытии окна автоматически открывается форма авторизации.
        /// Это предотвращает "случайное" завершение работы приложения.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => new AuthWindow().Show();
    }
}
