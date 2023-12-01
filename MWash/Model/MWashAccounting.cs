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

        public MWashAccounting()
        {
            ServiceRecords = new ObservableCollection<ServiceRecord>();
        }

        public void AddServiceRecord(ServiceRecord serviceRecord)
        {
            ServiceRecords.Add(serviceRecord);
        }

        // Метод для генерації щоденного звіту
        public void GenerateDailyReport(DateTime date)
        {
            // Фільтрація записів за вказану дату
            var recordsForDate = ServiceRecords.Where(record => record.StartTime.Date == date.Date).ToList();

            // Формування звіту на основі отриманих записів
            foreach (var record in recordsForDate)
            {
                // Опрацювання кожного запису та формування звіту
                // Наприклад, виведення інформації про надані послуги та їх кількість за день
                Console.WriteLine($"Employee: {record.Employee.FirstName} {record.Employee.LastName}, Service: {record.Service.ServiceName}, Start Time: {record.StartTime}, End Time: {record.EndTime}");
            }

            // Розрахунок і виведення загальної суми заробленої за день
            var totalDailyIncome = recordsForDate.Sum(record => record.Service.ServiceCost);
            Console.WriteLine($"Total income for {date.Date}: {totalDailyIncome} UAH");
        }

        // Метод для розрахунку щоденної зарплати для працівника
        public void CalculateDailySalary(Employee employee)
        {
            // Фільтрація записів за працівником
            var employeeRecords = ServiceRecords.Where(record => record.Employee.Id == employee.Id).ToList();

            // Розрахунок зарплати на основі вартості послуг працівника за день
            var employeeDailyIncome = employeeRecords.Sum(record => record.Service.ServiceCost);
            var employeeDailySalary = employeeDailyIncome * 0.5; // Заробітня плата становить 50% від вартості наданих послуг

            Console.WriteLine($"Daily salary for {employee.FirstName} {employee.LastName}: {employeeDailySalary} UAH");
        }

        // Можливо, інші методи для реалізації функціональних вимог
        // ...
    }
}
