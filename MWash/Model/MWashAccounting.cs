using MWash;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWash
{
    public class MWashAccounting
    {
        public ObservableCollection<ServiceRecord> ServiceRecords { get; private set; }
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
                Console.WriteLine("All service boxes are currently occupied. Cannot add new service record.");
            }
        }


        public List<(string ServiceName, int TotalTime, int TotalPrice)> GenerateDailyReport(DateTime date)
        {
            var recordsForDate = ServiceRecords.Where(record => record.StartTime.Date == date.Date).ToList();
            var serviceData = new Dictionary<string, (int totalTime, int totalPrice)>();
            var serviceReport = new List<(string ServiceName, int TotalTime, int TotalPrice)>();

            foreach (var record in recordsForDate)
            {
                var serviceName = record.Service.ServiceName;
                var serviceTime = (int)(record.EndTime - record.StartTime).TotalMinutes;
                var servicePrice = record.Service.ServiceCost;

                if (serviceData.ContainsKey(serviceName))
                {
                    var currentData = serviceData[serviceName];
                    serviceData[serviceName] = (currentData.totalTime + serviceTime, currentData.totalPrice + servicePrice);
                }
                else
                {
                    serviceData.Add(serviceName, (serviceTime, servicePrice));
                }
            }

            foreach (var serviceEntry in serviceData)
            {
                serviceReport.Add((serviceEntry.Key, serviceEntry.Value.totalTime, serviceEntry.Value.totalPrice));
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
            var employeeRecords = ServiceRecords.Where(record => record.Employees.Any(emp => emp.Id == employee.Id)).ToList();
            var totalEmployeeIncome = 0;

            foreach (var record in employeeRecords)
            {
                var numberOfEmployees = record.Employees.Count; // Отримати кількість працівників, які беруть участь у послузі
                var employeeShare = record.Service.ServiceCost / numberOfEmployees; // Розподілити вартість послуги між працівниками

                if (record.Employees.Any(emp => emp.Id == employee.Id))
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

    }
}
