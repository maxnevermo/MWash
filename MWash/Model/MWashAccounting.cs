using Microsoft.Win32;
using MWash;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MWash
{
    public class MWashAccounting
    {
        public ObservableCollection<ServiceRecord> ServiceRecords { get; set; }
        public List<Employee> EmployeesList { get; private set; }

        public MWashAccounting()
        {
            ServiceRecords = new ObservableCollection<ServiceRecord>();
            EmployeesList = new List<Employee>();   
        }

        // Метод для додавання нового запису про надану послугу
        public void AddServiceRecord(ServiceRecord serviceRecord)
        {
            // Перевірка наявності вільних боксів перед додаванням нового запису про надану послугу
            var numberOfOccupiedBoxes = ServiceRecords
                .Count(record => record.StartTime <= serviceRecord.EndTime && record.EndTime >= serviceRecord.StartTime);

            if (numberOfOccupiedBoxes < 2)
            {
                // Перевірка, чи працівники не зайняті на іншій послузі у цей час
                foreach (var employee in serviceRecord.Employees)
                {
                    bool isEmployeeOccupied = ServiceRecords
                        .Any(record => record.Employees.Any(emp => emp.Id == employee.Id &&
                                                                  record.StartTime <= serviceRecord.EndTime &&
                                                                  record.EndTime >= serviceRecord.StartTime));

                    if (isEmployeeOccupied)
                    {
                        MessageBox.Show($"{employee.FirstName} {employee.LastName} is already occupied at this time.", "Error");
                        return;
                    }
                }

                ServiceRecords.Add(serviceRecord);
                Console.WriteLine("Service record added successfully.");
            }
            else
            {
                MessageBox.Show("All service boxes are currently occupied. Cannot add a new service record.", "Error");
            }
        }



        public List<(string ServiceName, int TotalTime, int TotalCount, int TotalPrice)> GenerateDailyReport(DateTime date)
        {
            var recordsForDate = ServiceRecords.Where(record => record.StartTime.Date == date.Date).ToList();
            var serviceData = new Dictionary<string, (int totalTime, int totalCount, int totalPrice)>();
            var serviceReport = new List<(string ServiceName, int TotalTime, int TotalCount, int TotalPrice)>();

            foreach (var record in recordsForDate)
            {
                var serviceName = record.Service.ServiceName;
                var serviceTime = (int)(record.EndTime - record.StartTime).TotalMinutes; // Час надання послуги в хвилинах
                var serviceCount = 1; // Поле для кількості наданих послуг
                var servicePrice = record.Service.ServiceCost;

                if (serviceData.ContainsKey(serviceName))
                {
                    var currentData = serviceData[serviceName];
                    serviceData[serviceName] = (currentData.totalTime + serviceTime, currentData.totalCount + serviceCount, currentData.totalPrice + servicePrice);
                }
                else
                {
                    // Оновлений рядок з передачею даних у кортежі
                    serviceData.Add(serviceName, (serviceTime, serviceCount, servicePrice));
                }
            }

            foreach (var serviceEntry in serviceData)
            {
                serviceReport.Add((serviceEntry.Key, serviceEntry.Value.totalTime, serviceEntry.Value.totalCount, serviceEntry.Value.totalPrice));
            }

            return serviceReport;
        }




        public List<(string EmployeeName, string ServiceName, DateTime StartTime, DateTime EndTime)> GenerateDailyReportForEmployees(DateTime date)
        {
            var recordsForDate = ServiceRecords.Where(record => record.StartTime.Date == date.Date).ToList();
            var employeeReport = new List<(string EmployeeName, string ServiceName, DateTime StartTime, DateTime EndTime)>();

            foreach (var record in recordsForDate)
            {
                foreach (var employee in record.Employees)
                {
                    employeeReport.Add(($"{employee.FirstName} {employee.LastName}", record.Service.ServiceName, record.StartTime, record.EndTime));
                }
            }

            return employeeReport;
        }

        //Функція для обрахунку денної зарплати працівників
        public double CalculateDailySalary(Employee employee)
        {
            DateTime currentDate = DateTime.Now.Date;

            var employeeRecords = ServiceRecords
                .Where(record =>
                    record.Employees.Any(emp => emp.LastName.Trim() == employee.LastName.Trim()) &&
                    record.StartTime.Date == currentDate) // Фільтруємо записи за сьогоднішньою датою
                .ToList();

            var totalEmployeeIncome = 0;

            foreach (var record in employeeRecords)
            {
                var numberOfEmployees = record.Employees.Count; // Отримати кількість працівників, які беруть участь у послузі
                var employeeShare = record.Service.ServiceCost / numberOfEmployees; // Розподілити вартість послуги між працівниками

                if (record.Employees.Any(emp => emp.LastName.Trim() == employee.LastName.Trim()))
                {
                    totalEmployeeIncome += employeeShare; // Додати до загального доходу працівника його частку вартості послуги
                }
            }

            var employeeDailySalary = totalEmployeeIncome * 0.5; // Розрахувати зарплату працівника

            return employeeDailySalary;
        }

        //Вивід даних про послугу у таблицю
        public List<(string Employee, string Service, int Price, DateTime Time)> GetServiceDataForGrid(DateTime date)
        {
            var recordsForDate = ServiceRecords.Where(record => record.StartTime.Date == date.Date).ToList();
            var serviceDataForGrid = new List<(string Employee, string Service, int Price, DateTime Time)>();

            foreach (var record in recordsForDate)
            {
                foreach (var employee in record.Employees)
                {
                    serviceDataForGrid.Add(($"{employee.FirstName} {employee.LastName}", record.Service.ServiceName, record.Service.ServiceCost, record.StartTime));
                }
            }

            return serviceDataForGrid;
        }

        //Функція для створення файлу зі звітністю за день
        public void GenerateFileReportForDay(DateTime date)
        {
            string fileName = $"{date:dd-MM-yyyy}.txt";
            List<(string ServiceName, int TotalTime, int TotalCount, int TotalPrice)> dailyReport = GenerateDailyReport(date);

            StringBuilder reportContent = new StringBuilder();
            reportContent.AppendLine($"Report for {date:dd-MM-yyyy}");

            foreach (var service in dailyReport)
            {
                reportContent.AppendLine($"Service: {service.ServiceName}");
                reportContent.AppendLine($"Times used: {service.TotalCount}");
                reportContent.AppendLine($"Cost per service: {service.TotalPrice / service.TotalCount}");
                reportContent.AppendLine($"Total cost for this category: {service.TotalPrice}");
                reportContent.AppendLine();
            }

            // Calculate total earnings for the day
            double totalEarnings = dailyReport.Sum(service => service.TotalPrice);
            reportContent.AppendLine($"Total earnings for the day: {totalEarnings}");


            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = fileName;
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            // Save report to text file
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {                    
                    //string filePath = $"{Environment.CurrentDirectory}\\{fileName}";
                    System.IO.File.WriteAllText(filePath, reportContent.ToString());
                    Console.WriteLine($"Report generated successfully. Saved to {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while saving report: {ex.Message}");
                }
            }
        }


        //Функція для створення файлу зі звітністю за тиждень
        public void GenerateFileReportForWeek(DateTime startDate)
        {
            DateTime endDate = startDate.AddDays(7); // Calculate the end date (a week from the start date)
            string fileName = $"{startDate:dd-MM-yyyy}_to_{endDate:dd-MM-yyyy}.txt";

            StringBuilder reportContent = new StringBuilder();
            reportContent.AppendLine($"Summary Report for the week: {startDate:dd-MM-yyyy} to {endDate:dd-MM-yyyy}");

            List<(string ServiceName, int TotalTime, int TotalCount, int TotalPrice)> weeklyReport = new List<(string ServiceName, int TotalTime, int TotalCount, int TotalPrice)>();

            // Gather data for the entire week
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                List<(string ServiceName, int TotalTime, int TotalCount, int TotalPrice)> dailyReport = GenerateDailyReport(date);

                // Aggregate data for the week
                foreach (var service in dailyReport)
                {
                    var existingService = weeklyReport.FirstOrDefault(s => s.ServiceName == service.ServiceName);

                    if (existingService.Equals(default))
                    {
                        weeklyReport.Add(service);
                    }
                    else
                    {
                        var index = weeklyReport.IndexOf(existingService);
                        weeklyReport[index] = (service.ServiceName, existingService.TotalTime + service.TotalTime,
                                               existingService.TotalCount + service.TotalCount,
                                               existingService.TotalPrice + service.TotalPrice);
                    }
                }
            }

            //Вивід тижневого заробітку
            foreach (var service in weeklyReport)
            {
                reportContent.AppendLine($"Service: {service.ServiceName}");
                reportContent.AppendLine($"Total Times used: {service.TotalCount}");
                reportContent.AppendLine($"Total Cost per service: {service.TotalPrice / service.TotalCount}");
                reportContent.AppendLine($"Total cost for this category for the week: {service.TotalPrice}");
                reportContent.AppendLine("--------------------------------------");
            }

            //Розрахунок та вивід усього заробітку
            double totalEarnings = weeklyReport.Sum(service => service.TotalPrice);
            reportContent.AppendLine($"Total earnings for the period: {totalEarnings}");


            // Save report to text file
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = fileName;
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            // Збереження звіту до файлу
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    System.IO.File.WriteAllText(filePath, reportContent.ToString());
                    Console.WriteLine($"Report generated successfully. Saved to {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while saving report: {ex.Message}");
                }
            }
        }
        //Функція для створення файлу зі звітністю за певний проміжок часу
        public void GenerateFileReportForSelectedPeriod(DateTime startDate, DateTime endDate)
        {
            string fileName = $"{startDate:dd-MM-yyyy}_to_{endDate:dd-MM-yyyy}.txt";

            StringBuilder reportContent = new StringBuilder();
            reportContent.AppendLine($"Summary Report for the period: {startDate:dd-MM-yyyy} to {endDate:dd-MM-yyyy}");

            List<(string ServiceName, int TotalTime, int TotalCount, int TotalPrice)> periodReport = new List<(string ServiceName, int TotalTime, int TotalCount, int TotalPrice)>();

            // Збір даних для певного періоду
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                List<(string ServiceName, int TotalTime, int TotalCount, int TotalPrice)> dailyReport = GenerateDailyReport(date);

                // Узагальнення даних
                foreach (var service in dailyReport)
                {
                    var existingService = periodReport.FirstOrDefault(s => s.ServiceName == service.ServiceName);

                    if (existingService.Equals(default))
                    {
                        periodReport.Add(service);
                    }
                    else
                    {
                        var index = periodReport.IndexOf(existingService);
                        periodReport[index] = (service.ServiceName, existingService.TotalTime + service.TotalTime,
                                               existingService.TotalCount + service.TotalCount,
                                               existingService.TotalPrice + service.TotalPrice);
                    }
                }
            }

            // Вивід заробітку для вказаного періоду
            foreach (var service in periodReport)
            {
                reportContent.AppendLine($"Service: {service.ServiceName}");
                reportContent.AppendLine($"Total Times used: {service.TotalCount}");
                reportContent.AppendLine($"Total Cost per service: {service.TotalPrice / service.TotalCount}");
                reportContent.AppendLine($"Total cost for this category for the period: {service.TotalPrice}");
                reportContent.AppendLine("--------------------------------------");
            }

            //Весь заробіток за деякий період
            double totalEarnings = periodReport.Sum(service => service.TotalPrice);
            reportContent.AppendLine($"Total earnings for the period: {totalEarnings}");


            // Вибір файлу для збереження звіту
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = fileName;
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            // Збереження звіту у файл
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    System.IO.File.WriteAllText(filePath, reportContent.ToString());
                    Console.WriteLine($"Report generated successfully. Saved to {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while saving report: {ex.Message}");
                }
            }
        }


        //Створення денного звіту таблицею
        public List<(string ServiceName, int TotalCount, double CostPerService)> GenerateDailyReportForGrid(DateTime date)
        {
            var recordsForDate = ServiceRecords.Where(record => record.StartTime.Date == date.Date).ToList();
            var serviceDataForGrid = new List<(string ServiceName, int TotalCount, double CostPerService)>();

            foreach (var record in recordsForDate)
            {
                var serviceName = record.Service.ServiceName;
                var totalCount = 1; // Підсумовуємо кількість виконаних послуг
                var costPerService = record.Service.ServiceCost; // Ціна за послуги

                // Перевіряємо, чи дана послуга вже є у таблиці
                var existingService = serviceDataForGrid.FirstOrDefault(s => s.ServiceName == serviceName);
                if (existingService.Equals(default))
                {
                    serviceDataForGrid.Add((serviceName, totalCount, costPerService));
                }
                else
                {
                    // Змінюємо кількість послуг
                    var modifiedService = existingService;

                    // Оновлюємо кількість послуг
                    var index = serviceDataForGrid.IndexOf(existingService);
                    modifiedService = (modifiedService.ServiceName, modifiedService.TotalCount + totalCount, modifiedService.CostPerService + costPerService);

                    serviceDataForGrid[index] = modifiedService;
                }
            }

            List<(string ServiceName, int TotalCount, double CostPerService)> updatedServiceDataForGrid = new List<(string ServiceName, int TotalCount, double CostPerService)>();

            foreach (var service in serviceDataForGrid)
            {
                // Розрахунок вартості послуг
                double costPerService = service.CostPerService / service.TotalCount;
                updatedServiceDataForGrid.Add((service.ServiceName, service.TotalCount, costPerService));
            }

            // Заміна попередньої колекції послуг на нову
            serviceDataForGrid = updatedServiceDataForGrid;

            return serviceDataForGrid;
        }

        public List<(string ServiceName, int TotalCount, double CostPerService)> GenerateWeeklyReportForGrid(DateTime endDate)
        {
            // Визначення початку тижня на основі кінцевої дати
            DateTime startDate = endDate.AddDays(-7);

            // Фільтрація записів за останній тиждень
            var recordsForWeek = ServiceRecords
                .Where(record => record.StartTime.Date >= startDate.Date && record.StartTime.Date <= endDate.Date)
                .ToList();

            var serviceDataForGrid = new List<(string ServiceName, int TotalCount, double CostPerService)>();

            foreach (var record in recordsForWeek)
            {
                var serviceName = record.Service.ServiceName;
                var totalCount = 1; // Припускаємо, що кожен запис представляє одне використання послуги
                var costPerService = record.Service.ServiceCost; // Вартість послуги

                // Перевірка, чи ім'я послуги вже існує в списку
                var existingService = serviceDataForGrid.FirstOrDefault(s => s.ServiceName == serviceName);
                if (existingService.Equals(default))
                {
                    serviceDataForGrid.Add((serviceName, totalCount, costPerService));
                }
                else
                {
                    // Оновлення існуючого запису про послугу в списку
                    var modifiedService = existingService;
                    var index = serviceDataForGrid.IndexOf(existingService);
                    modifiedService = (modifiedService.ServiceName, modifiedService.TotalCount + totalCount, modifiedService.CostPerService + costPerService);
                    serviceDataForGrid[index] = modifiedService;
                }
            }

            List<(string ServiceName, int TotalCount, double CostPerService)> updatedServiceDataForGrid = new List<(string ServiceName, int TotalCount, double CostPerService)>();

            foreach (var service in serviceDataForGrid)
            {
                // Розрахунок вартості послуги на одне використання та додавання до нової колекції
                double costPerService = service.CostPerService / service.TotalCount;
                updatedServiceDataForGrid.Add((service.ServiceName, service.TotalCount, costPerService));
            }

            // Заміна оригінальної колекції оновленою
            serviceDataForGrid = updatedServiceDataForGrid;

            return serviceDataForGrid;
        }

        public List<(string ServiceName, int TotalCount, double CostPerService)> GenerateReportForSelectedPeriod(DateTime selectedStartDate, DateTime selectedEndDate)
        {
            // Фільтрація записів за обраний проміжок часу
            var recordsForSelectedPeriod = ServiceRecords
                .Where(record => record.StartTime.Date >= selectedStartDate.Date && record.StartTime.Date <= selectedEndDate.Date)
                .ToList();

            var serviceDataForGrid = new List<(string ServiceName, int TotalCount, double CostPerService)>();

            foreach (var record in recordsForSelectedPeriod)
            {
                var serviceName = record.Service.ServiceName;
                var totalCount = 1; // Припускаємо, що кожен запис представляє одне використання послуги
                var costPerService = record.Service.ServiceCost; // Вартість послуги

                // Перевірка, чи ім'я послуги вже існує в списку
                var existingService = serviceDataForGrid.FirstOrDefault(s => s.ServiceName == serviceName);
                if (existingService.Equals(default))
                {
                    serviceDataForGrid.Add((serviceName, totalCount, costPerService));
                }
                else
                {
                    // Оновлення існуючого запису про послугу в списку
                    var modifiedService = existingService;
                    var index = serviceDataForGrid.IndexOf(existingService);
                    modifiedService = (modifiedService.ServiceName, modifiedService.TotalCount + totalCount, modifiedService.CostPerService + costPerService);
                    serviceDataForGrid[index] = modifiedService;
                }
            }

            List<(string ServiceName, int TotalCount, double CostPerService)> updatedServiceDataForGrid = new List<(string ServiceName, int TotalCount, double CostPerService)>();

            foreach (var service in serviceDataForGrid)
            {
                // Розрахунок вартості послуги на одне використання та додавання до нової колекції
                double costPerService = service.CostPerService / service.TotalCount;
                updatedServiceDataForGrid.Add((service.ServiceName, service.TotalCount, costPerService));
            }

            // Заміна оригінальної колекції оновленою
            serviceDataForGrid = updatedServiceDataForGrid;

            return serviceDataForGrid;
        }
    }
}