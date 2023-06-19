using Microsoft.Win32;
using SteeringWheelApp.Controllers.Document;
using SteeringWheelApp.Models.Entities;
using System.Linq;
using System.Windows;

namespace SteeringWheelApp.Views.Dialogs
{
    /// <summary>
    /// Содержит логику взаимодействия для диалогового окна BulletinDialogWindow.
    /// Здесь выводится талон для сформированного заказа.
    /// </summary>
    public partial class BulletinDialogWindow : Window
    {
        private Order _currentOrder;

        public BulletinDialogWindow(Order order)
        {
            _currentOrder = order;
            InitializeComponent();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            detailsOrderDate.Text = _currentOrder.OrderDate.ToString("dd.MM.yyyy");
            detailsOrderId.Text = _currentOrder.Id.ToString();
            detailsOrderProducts.Text = string.Join(", ", _currentOrder.OrderProducts.Select(op => op.Product.Name));
            detailsOrderCost.Text = $"{_currentOrder.FinalOrderCost:0,00}Р";
            detailsOrderDiscount.Text = $"{_currentOrder.FinalOrderDiscount:0,00}Р";
            detailsOrderPickupPoint.Text = _currentOrder.PickupPoint.ToString();
            detailsOrderTakeCode.Text = _currentOrder.TakeCode.ToString();
            detailsOrderStatus.Text = DemoExamDataContext.Instance.OrderStatuses.FirstOrDefault(status => status.Id == _currentOrder.StatusId)?.Name ?? "Недоступно";
        }

        private void OnGeneratePdfFileButtonClick(object sender, RoutedEventArgs e) => GenerateBulletinFile();

        private void GenerateBulletinFile()
        {
            var bulletinSaveDialog = new SaveFileDialog
            {
                Title = "Сохранение талона",
                Filter = "Файлы PDF (*.pdf)|*.pdf",
            };
            if (bulletinSaveDialog.ShowDialog() == true)
                new BulletinController(bulletinSaveDialog.FileName, _currentOrder).GenerateBulletin();
            else
                MessageBox.Show("Сохранение талона отменено.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
