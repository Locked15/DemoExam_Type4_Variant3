using SteeringWheelApp.Models.Entities;
using System;
using System.Windows;
using Microsoft.Office.Interop.Word;
using WordDocument = Microsoft.Office.Interop.Word.Document;
using WordApplication = Microsoft.Office.Interop.Word.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SteeringWheelApp.Controllers.Document
{
    public class BulletinController
    {
        private const string TemplateFileName = "BulletinTemplate.docx";

        private readonly string _targetPath;

        private readonly Order _order;

        public BulletinController(string targetPath, Order order)
        {
            _targetPath = targetPath;
            _order = order;
        }

        public bool GenerateBulletin()
        {
            bool exceptionThrown = false;
            try
            {
                var app = new WordApplication();
                var templateDocument = app.Documents.Open(GetTemplateFileFullPath());

                try
                {
                    ReplacePlaceholderWithValue(templateDocument, "{0}", _order.OrderDate.ToString("dd.MM.yyyy!"));
                    ReplacePlaceholderWithValue(templateDocument, "{1}", _order.DeliveryDate.ToString("dd.MM.yyyy!"));
                    ReplacePlaceholderWithValue(templateDocument, "{2}", _order.Id.ToString());
                    ReplacePlaceholderWithValue(templateDocument, "{3}", string.Join(", ", _order.OrderProducts.Select(op => op.Product.Name)));
                    ReplacePlaceholderWithValue(templateDocument, "{4}", $"{_order.FinalOrderCost:0,00}Р");
                    ReplacePlaceholderWithValue(templateDocument, "{5}", $"{_order.FinalOrderDiscount:0,00}Р");
                    ReplacePlaceholderWithValue(templateDocument, "{6}", _order.PickupPoint.ToString());
                    ReplacePlaceholderWithValue(templateDocument, "{7}", DemoExamDataContext.Instance.OrderStatuses.FirstOrDefault(status => status.Id == _order.StatusId)?.Name ?? "Недоступно");
                    ReplacePlaceholderWithValue(templateDocument, "{8}", _order.TakeCode.ToString());

                    templateDocument.SaveAs2(_targetPath, WdSaveFormat.wdFormatPDF);
                    try
                    {
                        Process.Start(_targetPath);
                    }
                    catch
                    {
                        // Ничего не делаем. Эта ошибка происходит, если на ПК нет приложения для просмотра PDF, установленного по умолчанию.
                    }
                }
                catch (Exception ex)
                {
                    exceptionThrown = true;
                    MessageBox.Show($"В процессе создания талона произошла ошибка.\n\nДетали:\n{ex.Message}.", "Ошибка!",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    templateDocument.Close();
                    app.Quit();
                }
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                MessageBox.Show($"Не удалось открыть требуемый файл.\nВозможно, установка повреждена.\n\nДетали:\n{ex.Message}.", "Ошибка!",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return !exceptionThrown;
        }

        private static string GetTemplateFileFullPath() =>
                Path.Combine(Environment.CurrentDirectory, "Assets", TemplateFileName);

        private static void ReplacePlaceholderWithValue(WordDocument document, string placeholderText, string value) =>
                document.Content.Find.Execute(FindText: placeholderText, ReplaceWith: value, Replace: WdReplace.wdReplaceAll);
    }
}
