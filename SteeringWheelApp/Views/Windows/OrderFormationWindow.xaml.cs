using SteeringWheelApp.Models.Entities;
using SteeringWheelApp.Views.Dialogs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SteeringWheelApp.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для OrderFormationWindow.
    /// В этом окне происходит формирование заказа и его сохранение в БД.
    /// </summary>
    public partial class OrderFormationWindow : Window
    {
        public Order CurrentOrder { get; init; }

        public OrderFormationWindow(Order currentOrder)
        {
            CurrentOrder = currentOrder;

            InitializeComponent();
            DataContext = CurrentOrder;
        }

        /// <summary>
        /// Это событие возникает после загрузки окна.
        /// Оно нужно для первичной инициализации полей.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            pickupPointSelector.ItemsSource = DemoExamDataContext.Instance.PickupPoints.ToList();
            UpdateWindow();
        }

        private void UpdateWindow()
        {
            if (!CurrentOrder.OrderProducts.Any())
                Close();

            productsInOrderListView.SelectedIndex = -1;
            productsInOrderListView.ItemsSource = null;
            productsInOrderListView.ItemsSource = CurrentOrder.OrderProducts;

            UpdateFields();
        }

        private void UpdateFields()
        {
            var finalCost = CurrentOrder.FinalOrderCost;
            var finalDiscount = CurrentOrder.FinalOrderDiscount;

            finalDiscountText.Text = $"{finalDiscount:0,00}Р";
            finalCostText.Text = $"{finalCost:0,00}Р";
        }

        private void OnProductsInOrderListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productsInOrderListView.SelectedItem is OrderProduct orderProduct)
                productCountInputBox.Text = orderProduct.Count.ToString();
        }

        private void OnAddToOrderMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (productsInOrderListView.SelectedItem is OrderProduct orderProduct)
            {
                orderProduct.Count++;
                UpdateFields();

                productsInOrderListView.SelectedIndex = -1;
            }
        }

        private void OnProductCountInputBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (productsInOrderListView.SelectedItem is OrderProduct orderProduct)
            {
                var newCountRawText = productCountInputBox.Text;
                if (int.TryParse(newCountRawText, out var newCount))
                {
                    if (newCount <= 0)
                    {
                        var confirm = MessageBox.Show("Такое количество приведёт к удаление товара из заказа.\n\nВы уверены?", "Подтверждение",
                                                      MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (confirm == MessageBoxResult.Yes)
                        {
                            CurrentOrder.SetProductCount(orderProduct.Product, newCount);
                            UpdateWindow();
                        }
                        else
                        {
                            productCountInputBox.Text = orderProduct.Count.ToString();
                        }
                    }
                    else
                    {
                        CurrentOrder.SetProductCount(orderProduct.Product, newCount);
                        UpdateFields();
                    }
                }
                else
                {
                    MessageBox.Show("Введено некорректное значение.\n\nСброс...", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
                    productCountInputBox.Text = orderProduct.Count.ToString();
                }
            }
        }

        private void OnGenerateBulletinButtonClick(object sender, RoutedEventArgs e)
        {
            if (CurrentOrder.Id == default)
                MessageBox.Show("Заказ ещё не сформирован. Создание талона недоступно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                new BulletinDialogWindow(CurrentOrder).ShowDialog();
        }

        private void OnRemoveProductFromOrderButtonClick(object sender, RoutedEventArgs e)
        {
            if (productsInOrderListView.SelectedItem is OrderProduct orderProduct)
            {
                var confirm = MessageBox.Show("Вы точно хотите удалить товар из заказа?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm == MessageBoxResult.Yes)
                {
                    CurrentOrder.OrderProducts.Remove(orderProduct);
                    UpdateWindow();
                }
            }
        }

        private void OnSaveOrderButtonClick(object sender, RoutedEventArgs e)
        {
            if (CurrentOrder.PickupPoint != null)
            {
                PrepareOrderToSave();
                try
                {
                    if (CurrentOrder.Id == default)
                    {
                        DemoExamDataContext.Instance.Add(CurrentOrder);
                        DemoExamDataContext.Instance.AddRange(CurrentOrder.OrderProducts);

                        DemoExamDataContext.Instance.SaveChanges();
                        var confirm = MessageBox.Show("Заказ добавлен в систему.\nПросмотреть талон?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (confirm == MessageBoxResult.Yes)
                            OnGenerateBulletinButtonClick(default, default);
                    }
                    else
                    {
                        DemoExamDataContext.Instance.SaveChanges();
                        var confirm = MessageBox.Show("Заказ обновлён.\nПросмотреть новый талон?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (confirm == MessageBoxResult.Yes)
                            OnGenerateBulletinButtonClick(default, default);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка сохранения данных.\n\nДетали:\n{ex.Message}.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Обнаружены ошибки:\nНеобходимо указать точку выдачи.", "Ошибки!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrepareOrderToSave()
        {
            var isAllProductsArePresented = CurrentOrder.OrderProducts.All(op => op.Product.Amount > 3);
            CurrentOrder.DeliveryDate = CurrentOrder.OrderDate.AddDays(isAllProductsArePresented ? 3 : 6);
        }

        /// <summary>
        /// Если заказ был сохранён в системе, то необходимо вернуть 'TRUE', чтобы система создана новый заказ.
        /// В ином случае работа со старым заказом продолжается.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) =>
                DialogResult = CurrentOrder.Id != default;
    }
}
