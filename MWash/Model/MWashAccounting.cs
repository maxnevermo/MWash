using MWash.Model;
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
        // Колекція для зберігання записів про надані послуги
        public ObservableCollection<ServiceRecord> ServiceRecords { get; private set; }

        public MWashAccounting()
        {
            ServiceRecords = new ObservableCollection<ServiceRecord>();
        }

        // Метод для додавання нового запису про надану послугу
        public void AddServiceRecord(ServiceRecord serviceRecord)
        {
            ServiceRecords.Add(serviceRecord);
        }

        // Інші методи для реалізації функціональних вимог

        // Наприклад, методи для керування працівниками, додавання та видалення послуг тощо

        // Наприклад:
        // public void AddEmployee(Employee employee) { ... }
        // public void RemoveEmployee(Employee employee) { ... }
        // public void AddService(Service service) { ... }
        // public void GenerateDailyReport(DateTime date) { ... }
        // і т.д.
    }
}
