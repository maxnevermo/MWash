using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWash
{
    public class ServiceRecord
    {
        public Employee Employee { get; set; }
        public Service Service { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Конструктор класу ServiceRecord
        public ServiceRecord(Employee employee, Service service, DateTime startTime, DateTime endTime)
        {
            Employee = employee;
            Service = service;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
