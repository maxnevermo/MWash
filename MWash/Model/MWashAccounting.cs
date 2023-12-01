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

        // Метод для додавання нового працівника
        public void AddEmployee(Employee employee)
        {
            // Логіка для додавання працівника
        }

        // Метод для видалення працівника
        public void RemoveEmployee(Employee employee)
        {
            // Логіка для видалення працівника
        }

        // Метод для додавання нової послуги
        public void AddService(Service service)
        {
            // Логіка для додавання нової послуги
        }

        // Метод для генерації щоденного звіту
        public void GenerateDailyReport(DateTime date)
        {
            // Логіка для генерації щоденного звіту
        }

        // Метод для розрахунку щоденної зарплати для працівника
        public void CalculateDailySalary(Employee employee)
        {
            // Логіка для розрахунку щоденної зарплати працівника
            // Наприклад, можна використовувати інформацію з ServiceRecords для розрахунку зарплати працівника за певний день
        }

        // Можливо, інші методи для реалізації функціональних вимог
        // ...
    }
}
