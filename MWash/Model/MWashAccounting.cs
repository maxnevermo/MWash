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

            // Якщо кількість зайнятих боксів менше двох, можна додати новий запис про надану послугу
            if (numberOfOccupiedBoxes < 2)
            {
                ServiceRecords.Add(serviceRecord);
                Console.WriteLine("Service record added successfully.");
            }
            else
            {
                MessageBox.Show("Усі службові бокси в даний момент зайняті. Неможливо додати новий запис про послугу.", "Помилка");
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

        public double CalculateDailySalary(Employee employee)
        {
            var employeeRecords = ServiceRecords.Where(record => record.Employees.Any(emp => emp.LastName.Trim() == employee.LastName.Trim())).ToList();
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

        public void GenerateReportForDay(DateTime date)
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

            // Save report to text file
            try
            {
                string filePath = $"{Environment.CurrentDirectory}\\{fileName}";
                System.IO.File.WriteAllText(filePath, reportContent.ToString());
                Console.WriteLine($"Report generated successfully. Saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while saving report: {ex.Message}");
            }
        }



        public void GenerateReportForWeek(DateTime startDate)
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

            // Display weekly summary in the report content
            foreach (var service in weeklyReport)
            {
                reportContent.AppendLine($"Service: {service.ServiceName}");
                reportContent.AppendLine($"Total Times used: {service.TotalCount}");
                reportContent.AppendLine($"Total Cost per service: {service.TotalPrice / service.TotalCount}");
                reportContent.AppendLine($"Total cost for this category for the week: {service.TotalPrice}");
                reportContent.AppendLine("--------------------------------------");
            }

            // Save report to text file
            try
            {
                string filePath = $"{Environment.CurrentDirectory}\\{fileName}";
                System.IO.File.WriteAllText(filePath, reportContent.ToString());
                Console.WriteLine($"Weekly summary report generated successfully. Saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while saving weekly summary report: {ex.Message}");
            }
        }



        public void GenerateReportForMonth(int year, int month)
        {
             
        }
    }
}