using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Microsoft.EntityFrameworkCore;
using WarehouseITSystem.Models;
using CsvHelper;
using System.IO;
using CsvHelper.Configuration;
using System.Globalization;
using OfficeOpenXml;
using TemplateEngine.Docx;

namespace WarehouseITSystem.ViewModels
{
    internal class ExportViewModel : Screen
    {
        public ExportViewModel() : base()
        {
            LoadValues();
            DateStart = DateTime.Now;
            DateEnd = DateTime.Now;
        }
        private void LoadValues()
        {
            TableImport = new List<string> { "Номеклатуры", "Характеристики", "Покупатели", "Поставщики", "Поставки", "Склады", "Типы складов" };
            TableExport = new List<string> { "Номеклатуры", "Характеристики", "Покупатели", "Поставщики", "Поставки", "Склады", "Типы складов" };
        }

        private List<string> tableImport = null!;
        public List<string> TableImport
        {
            get => tableImport;
            set
            {
                tableImport = value;
                NotifyOfPropertyChange(() => TableImport);
            }
        }

        private string? selectedTableImport;
        public string? SelectedTableImport
        {
            get => selectedTableImport;
            set
            {
                selectedTableImport = value;
                NotifyOfPropertyChange(() => SelectedTableImport);
                NotifyOfPropertyChange(() => CanImportTable);
            }
        }

        private bool isTableImportRewrite;
        public bool IsTableImportRewrite
        {
            get => isTableImportRewrite;
            set
            {
                isTableImportRewrite = value;
                NotifyOfPropertyChange(() => IsTableImportRewrite);
            }
        }

        private List<string> tableExport = null!;
        public List<string> TableExport
        {
            get => tableExport;
            set
            {
                tableExport = value;
                NotifyOfPropertyChange(() => TableExport);
            }
        }

        private string? selectedTableExport;
        public string? SelectedTableExport
        {
            get => selectedTableExport;
            set
            {
                selectedTableExport = value;
                NotifyOfPropertyChange(() => SelectedTableExport);
                NotifyOfPropertyChange(() => CanExportTable);
            }
        }

        private bool isTableExportRewrite;
        public bool IsTableExportRewrite
        {
            get => isTableExportRewrite;
            set
            {
                isTableExportRewrite = value;
                NotifyOfPropertyChange(() => IsTableExportRewrite);
            }
        }

        private DateTime? dateStart;
        public DateTime? DateStart
        {
            get => dateStart;
            set
            {
                dateStart = value;
                NotifyOfPropertyChange(() => DateStart);
                NotifyOfPropertyChange(() => CanSalesReport);
                NotifyOfPropertyChange(() => CanPurchaseReport);
            }
        }

        private DateTime? dateEnd;
        public DateTime? DateEnd
        {
            get => dateEnd;
            set
            {
                dateEnd = value;
                NotifyOfPropertyChange(() => DateEnd);
                NotifyOfPropertyChange(() => CanSalesReport);
                NotifyOfPropertyChange(() => CanPurchaseReport);
            }
        }

        public bool CanImportTable
        {
           get => SelectedTableImport != null;
        }

        public async void ImportTable()
        {
            Microsoft.Win32.OpenFileDialog dlg = new();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV Files (*.csv)|*.csv";
            if (dlg.ShowDialog() == true)
            {
                string filename = dlg.FileName;
                if (Path.GetExtension(filename) != ".csv")
                {
                    MessageBox.Show("Неверный формат файла");
                    return;
                }
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.ToLower().Replace("_", ""),
                };
                using var reader = new StreamReader(filename);
                using var csv = new CsvReader(reader, config);
                using var db = new WarehouseContext();
               
                try
                {
                    switch (SelectedTableImport!)
                    {
                        case "Номеклатуры":
                            if (IsTableImportRewrite)
                            {
                                try
                                {
                                    db.Database.ExecuteSqlRaw("TRUNCATE nomenclatures RESTART IDENTITY;");
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                    return;
                                }
                            }
                            var recordsNomenclature = csv.GetRecords<Nomenclature>().ToList();
                            db.Nomenclatures.AddRange(recordsNomenclature);
                            await db.SaveChangesAsync();
                            db.Database.ExecuteSqlRaw("SELECT setval('nomenclatures_id_seq', (SELECT MAX(id) from nomenclatures));");
                            await db.SaveChangesAsync();
                            MessageBox.Show("Импорт успешно завершен");
                            break;

                        case "Характеристики":
                            if (IsTableImportRewrite)
                            {
                                try
                                {
                                    db.Database.ExecuteSqlRaw("TRUNCATE properties RESTART IDENTITY;");
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                    return;
                                }
                            }
                            var recordsProperty = csv.GetRecords<Property>().ToList();
                            db.Properties.AddRange(recordsProperty);
                            await db.SaveChangesAsync();
                            db.Database.ExecuteSqlRaw("SELECT setval('properties_id_seq', (SELECT MAX(id) from properties));");
                            await db.SaveChangesAsync();
                            MessageBox.Show("Импорт успешно завершен");
                            break;

                        case "Покупатели":                            
                            if (IsTableImportRewrite)
                            {
                                try
                                {
                                    db.Database.ExecuteSqlRaw("TRUNCATE customers RESTART IDENTITY;");
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                    return;
                                }
                            }
                            var recordsCustomer = csv.GetRecords<Customer>().ToList();
                            db.Customers.AddRange(recordsCustomer);
                            await db.SaveChangesAsync();
                            db.Database.ExecuteSqlRaw("SELECT setval('customers_id_seq', (SELECT MAX(id) from customers));");
                            await db.SaveChangesAsync();
                            MessageBox.Show("Импорт успешно завершен");
                            break;

                        case "Поставщики":
                            if (IsTableImportRewrite)
                            {
                                try
                                {
                                    db.Database.ExecuteSqlRaw("TRUNCATE suppliers RESTART IDENTITY;");
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                    return;
                                }
                            }
                            var recordsSupplier = csv.GetRecords<Supplier>().ToList();
                            db.Suppliers.AddRange(recordsSupplier);
                            await db.SaveChangesAsync();
                            db.Database.ExecuteSqlRaw("SELECT setval('suppliers_id_seq', (SELECT MAX(id) from suppliers));");
                            await db.SaveChangesAsync();
                            MessageBox.Show("Импорт успешно завершен");
                            break;

                        case "Поставки":
                            if (IsTableImportRewrite)
                            {
                                try
                                {
                                    db.Database.ExecuteSqlRaw("TRUNCATE deliveries RESTART IDENTITY;");
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                    return;
                                }
                            }
                            var recordsDelivery = csv.GetRecords<Delivery>().ToList();
                            db.Deliveries.AddRange(recordsDelivery);
                            await db.SaveChangesAsync();
                            db.Database.ExecuteSqlRaw("SELECT setval('deliveries_id_seq', (SELECT MAX(id) from deliveries));");
                            await db.SaveChangesAsync();
                            MessageBox.Show("Импорт успешно завершен");
                            break;

                        case "Склады":
                            if (IsTableImportRewrite)
                            {
                                try
                                {
                                    db.Database.ExecuteSqlRaw("TRUNCATE warehouses RESTART IDENTITY;");
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                    return;
                                }
                            }
                            var recordsWarehouse = csv.GetRecords<Warehouse>().ToList();
                            db.Warehouses.AddRange(recordsWarehouse);
                            await db.SaveChangesAsync();
                            db.Database.ExecuteSqlRaw("SELECT setval('warehouses_id_seq', (SELECT MAX(id) from warehouses));");
                            await db.SaveChangesAsync();
                            MessageBox.Show("Импорт успешно завершен");
                            break;

                        case "Типы складов":
                            if (IsTableImportRewrite)
                            {
                                try
                                {
                                    db.Database.ExecuteSqlRaw("TRUNCATE warehouse_types RESTART IDENTITY;");
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                    return;
                                }
                            }
                            var recordsWarehouseType = csv.GetRecords<WarehouseType>().ToList();
                            db.WarehouseTypes.AddRange(recordsWarehouseType);
                            await db.SaveChangesAsync();
                            db.Database.ExecuteSqlRaw("SELECT setval('warehouse_types_id_seq', (SELECT MAX(id) from warehouse_types));");
                            await db.SaveChangesAsync();
                            MessageBox.Show("Импорт успешно завершен");
                            break;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Неверный формат данных в файле, импорт не удался");
                }
            }
        }


        public bool CanExportTable
        {
            get => SelectedTableExport != null;
        }
        public void ExportTable()
        {
            Microsoft.Win32.SaveFileDialog dlg = new();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV Files (*.csv)|*.csv";
            if (dlg.ShowDialog() == true)
            {
                string filename = dlg.FileName;
                if (Path.GetExtension(filename) != ".csv")
                {
                    MessageBox.Show("Неверный формат файла");
                    return;
                }
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.ToLower().Replace("_", ""),
                    HasHeaderRecord = IsTableExportRewrite
                };
                using var writer = new StreamWriter(filename, !IsTableExportRewrite);
                using var csv = new CsvWriter(writer, config);
                using var db = new WarehouseContext();
                csv.WriteRecords(db.Customers);
            }
        }

        public static void ExcelReport()
        {
            using WarehouseContext db = new();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Отчет по продуктам");

            sheet.Cells["B2"].Value = "Отчет по продуктам";
            sheet.Cells["B2"].Style.Font.Bold = true;
            sheet.Cells["B3"].Value = "Дата = ";
            sheet.Cells["B3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells["C3"].Value = DateOnly.FromDateTime(DateTime.Now);

            sheet.Cells["B4:I4"].LoadFromArrays(new object[][] { new[] { "ID продукта", "Наименование", "Статус", "Адрес на складе", "ИНН поставщика", "Дата поставки", "Адрес склада", "Стоимость" } });

            var list = db.Products.Include(p => p.Nomenclature).Include(p => p.Delivery).Include(p => p.Delivery!.Supplier).Include(p => p.Warehouse).Include(p => p.Nomenclature!.ProductWorths);
            var row = 5;
            var column = 2;
            foreach (var item in list)
            {
                sheet.Cells[row, column].Value = item.Id;
                sheet.Cells[row, column + 1].Value = item.Nomenclature!.Name;
                sheet.Cells[row, column + 2].Value = item.Status;
                sheet.Cells[row, column + 3].Value = item.CellAddress;
                sheet.Cells[row, column + 4].Value = item.Delivery!.Supplier!.Tin;
                sheet.Cells[row, column + 5].Value = item.Delivery.Date;
                sheet.Cells[row, column + 6].Value = item.Warehouse!.Address;
                sheet.Cells[row, column + 7].Value = item.Nomenclature.ProductWorths.OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).FirstOrDefault()!.Worth;
                row++;
            }

            sheet.Cells[1, 1, row, column + 9].AutoFitColumns();
            sheet.Cells[4, 2, row, column + 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

            sheet.Cells[row, column + 6].Value = "Итого:";
            sheet.Cells[5, column + 7, row - 1, column + 7].Style.Numberformat.Format = "#"; 
            sheet.Cells[row, column + 7].Formula = $"SUM({(char)('A' + column + 6)}{5}:{(char)('A' + column + 6)}{row - 1})";
            sheet.Cells[row, column + 7].Calculate();
            sheet.Cells[row, column + 8].Value = "Кол-во:";
            sheet.Cells[row, column + 9].Value = db.Products.Count();
            sheet.Cells[row, column + 6, row, column + 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

            sheet.Protection.IsProtected = true;

            try
            {
                File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\productReport.xlsx", package.GetAsByteArray());
            }
            catch (Exception)
            {
                MessageBox.Show("При записи отчета в файл произошла ошибка. Убедитесь, что у вас закрыт прошлый сформированный отчет");
                return;
            }

            MessageBox.Show("Отчет успешно сформирован. Файл отчета располагается на рабочем столе");
        }

        public bool CanSalesReport
        {
            get => DateStart != null && DateEnd != null;
        }

        public void SalesReport()
        {
            var sampleReportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\salesReport.docx";
            var reportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\Отчет о продажах (от {DateOnly.FromDateTime(DateTime.Now)}).docx";
            
            try { File.Delete(reportPath); }
            catch { }

            File.Copy(sampleReportPath, reportPath);

            using var outputDocument = new TemplateProcessor(reportPath).SetRemoveContentControls(true);
            using var db = new WarehouseContext();

            db.ProductSellings.Load();
            db.Products.Load();
            db.Sellings.Load();
            db.Nomenclatures.Load();
            db.ProductWorths.Load();

            var valuesToFillNew = new Content();
            var valuesToFill = valuesToFillNew.Append(new FieldContent("Date", DateOnly.FromDateTime(DateTime.Now).ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("DateStart", DateOnly.FromDateTime(DateStart!.Value).ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("DateEnd", DateOnly.FromDateTime(DateEnd!.Value).ToString()));

            var querySellings = db.ProductSellings.Where(ps => ps.Selling!.Date >= DateOnly.FromDateTime(DateStart!.Value) && ps.Selling!.Date <= DateOnly.FromDateTime(DateEnd!.Value));
            decimal sumWorth = 0;

            List<TableRowContent> rows = new();
            foreach (var productSelling in querySellings)
            {
                var worth = productSelling.Product!.Nomenclature!.ProductWorths!.Where(pw => pw.Date <= DateOnly.FromDateTime(DateEnd!.Value)).OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).FirstOrDefault()!.Worth;
                sumWorth += worth * (decimal)(1 - 1.0 * productSelling.Selling!.PersonalDiscount! / 100);
                rows.Add(new TableRowContent(new FieldContent("ProductId", productSelling.Product!.Id.ToString()),
                new FieldContent("NomenclatureName", productSelling.Product!.Nomenclature!.Name),
                new FieldContent("NomenclatureWorth", worth.ToString()),
                new FieldContent("NomenclatureWorthDiscounted", (worth * (decimal)(1 - 1.0 * productSelling.Selling!.PersonalDiscount! / 100)).ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("SoldProducts", rows));

            var productTypeSellings = querySellings.GroupBy(ps => ps.Product!.Nomenclature!.Type!.Name).Select(g => new
            {
                g.Key,
                Sum = g.Sum(s => s.Product!.Nomenclature!.ProductWorths!.Where(pw => pw.Date <= DateOnly.FromDateTime(DateEnd!.Value)).OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).Select(t => t.Worth * (decimal)(1 - 1.0 * s.Selling!.PersonalDiscount! / 100)).Sum())
            }); ;
            rows = new();
            foreach (var productSelling in productTypeSellings)
            {
                rows.Add(new TableRowContent(new FieldContent("NomenclatureType", productSelling.Key),
                new FieldContent("NomenclatureTypeWorth", productSelling.Sum.ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("ProductTypeSellings", rows));

            valuesToFill = valuesToFill.Append(new FieldContent("ProductCount", querySellings.Count().ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("ProductSumWorth", sumWorth.ToString()));
            outputDocument.FillContent(new Content(valuesToFill.ToArray()));
            outputDocument.SaveChanges();
            

            MessageBox.Show("Отчет успешно сформирован. Файл отчета располагается на рабочем столе");
        }

        public bool CanPurchaseReport
        {
            get => DateStart != null && DateEnd != null;
        }
        public void PurchaseReport()
        {
            var sampleReportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\purchaseReport.docx";
            var reportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\Отчет о покупках (от {DateOnly.FromDateTime(DateTime.Now)}).docx";

            try { File.Delete(reportPath); }
            catch { }

            File.Copy(sampleReportPath, reportPath);

            using var outputDocument = new TemplateProcessor(reportPath).SetRemoveContentControls(true);
            using var db = new WarehouseContext();

            db.Deliveries.Load();
            db.Products.Load();
            db.Suppliers.Load();
            db.Nomenclatures.Load();
            db.ProductWorths.Load();

            var valuesToFillNew = new Content();
            var valuesToFill = valuesToFillNew.Append(new FieldContent("Date", DateOnly.FromDateTime(DateTime.Now).ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("DateStart", DateOnly.FromDateTime(DateStart!.Value).ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("DateEnd", DateOnly.FromDateTime(DateEnd!.Value).ToString()));

            var queryDeliveries = db.Products.Where(ps => ps.Delivery!.Date >= DateOnly.FromDateTime(DateStart!.Value) && ps.Delivery!.Date <= DateOnly.FromDateTime(DateEnd!.Value));
            decimal sumWorth = 0;

            List<TableRowContent> rows = new();
            foreach (var product in queryDeliveries)
            {
                var worth = product!.Nomenclature!.ProductWorths!.Where(pw => pw.Date <= DateOnly.FromDateTime(DateEnd!.Value)).OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).FirstOrDefault()!.Worth;
                sumWorth += worth;
                rows.Add(new TableRowContent(new FieldContent("ProductId", product.Id.ToString()),
                new FieldContent("NomenclatureName", product.Nomenclature!.Name),
                new FieldContent("NomenclatureWorth", worth.ToString()),
                new FieldContent("DeliveryId", product.DeliveryId.ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("ProductBuyings", rows));

            valuesToFill = valuesToFill.Append(new FieldContent("ProductCount", queryDeliveries.Count().ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("ProductSumWorth", sumWorth.ToString()));

            rows = new();
            var queryDeliveriesGroup = db.Deliveries.Where(d => d.Date >= DateOnly.FromDateTime(DateStart!.Value) && d.Date <= DateOnly.FromDateTime(DateEnd!.Value) && d.Products.Count > 0);
            foreach (var delivery in queryDeliveriesGroup)
            {
                rows.Add(new TableRowContent(new FieldContent("DeliveryIdGroup", delivery.Id.ToString()),
                new FieldContent("DeliveriesSumCost", delivery.Products.Sum(p => p.Nomenclature!.ProductWorths!.Where(pw => pw.Date <= DateOnly.FromDateTime(DateEnd!.Value)).OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).FirstOrDefault()!.Worth).ToString()),
                new FieldContent("DeliveriesSumCount", delivery.Products.Count.ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("ProductsDeliveries", rows));

            rows = new();
            var querySuppliersGroup = db.Suppliers.Where(s => s.Deliveries.Where(d => d.Date >= DateOnly.FromDateTime(DateStart!.Value) && d.Date <= DateOnly.FromDateTime(DateEnd!.Value)).Any());
            foreach (var supplier in querySuppliersGroup)
            {
                rows.Add(new TableRowContent(new FieldContent("SupplierName", supplier.Name ?? "-"),
                new FieldContent("SupplierTin", supplier.Tin.ToString()),
                new FieldContent("SupplierSumWorth", supplier.Deliveries.Where(d => d.Date >= DateOnly.FromDateTime(DateStart!.Value) && d.Date <= DateOnly.FromDateTime(DateEnd!.Value)).Sum(d => d.Products.Sum(p => p.Nomenclature!.ProductWorths!.Where(pw => pw.Date <= DateOnly.FromDateTime(DateEnd!.Value)).OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).FirstOrDefault()!.Worth)).ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("SuppliersProducts", rows));

            outputDocument.FillContent(new Content(valuesToFill.ToArray()));
            outputDocument.SaveChanges();

            MessageBox.Show("Отчет успешно сформирован. Файл отчета располагается на рабочем столе");
        }

        public static void WarehouseReport()
        {
            var sampleReportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\warehouseReport.docx";
            var reportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\Отчет о заполненности складов (от {DateOnly.FromDateTime(DateTime.Now)}).docx";

            try { File.Delete(reportPath); }
            catch { }

            File.Copy(sampleReportPath, reportPath);

            using var outputDocument = new TemplateProcessor(reportPath).SetRemoveContentControls(true);
            using var db = new WarehouseContext();

            db.Warehouses.Load();
            db.WarehouseTypes.Load();
            db.Products.Load();
            db.Nomenclatures.Load();
            db.ProductWorths.Load();

            var valuesToFillNew = new Content();
            var valuesToFill = valuesToFillNew.Append(new FieldContent("Date", DateOnly.FromDateTime(DateTime.Now).ToString()));

            var queryWarehouses = db.Warehouses;
            List<TableRowContent> rows = new();
            var sumProducts = queryWarehouses.Sum(w => w.Products.Count);
            foreach (var warehouse in queryWarehouses)
            {               
                rows.Add(new TableRowContent(new FieldContent("WarehouseId", warehouse.Id.ToString()),
                new FieldContent("WarehouseAddress", warehouse.Address),
                new FieldContent("WarehouseProductCount", warehouse.Products.Count.ToString()),
                new FieldContent("WarehouseProductPercentage", (1.0 * warehouse.Products.Count / sumProducts * 100).ToString()),
                new FieldContent("WarehouseProductMaxWorth", warehouse.Products.Max(p => p.Nomenclature!.ProductWorths.OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).FirstOrDefault()!.Worth).ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("WarehouseProducts", rows));


            rows = new();
            var queryBrokenProducts = db.Products.Where(p => p.Status == Product.ProductStatus.Списан);
            foreach (var product in queryBrokenProducts)
            {
                rows.Add(new TableRowContent(new FieldContent("BrokenProductId", product.Id.ToString()),
                new FieldContent("BrokenProductName", product.Nomenclature!.Name)));
            }
            valuesToFill = valuesToFill.Append(new TableContent("BrokenProducts", rows));


            outputDocument.FillContent(new Content(valuesToFill.ToArray()));
            outputDocument.SaveChanges();

            MessageBox.Show("Отчет успешно сформирован. Файл отчета располагается на рабочем столе");
        }
    }
}
