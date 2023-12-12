using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MWash.MainWindow;

namespace MWash
{
        public class ServiceRecord
        {
            public List<Employee> Employees { get; set; }
            public Service Service { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }

            // Конструктор класу ServiceRecord
            public ServiceRecord(List<Employee> employees, Service service, DateTime startTime, DateTime endTime)
            {
                Employees = employees;
                Service = service;
                StartTime = startTime;
                EndTime = endTime;
            }
    }
}
