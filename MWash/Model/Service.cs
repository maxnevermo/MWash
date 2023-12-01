using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWash
{
    public class Service
    {
        public string ServiceName { get; set; }
        public int ServiceCost { get; set; }

        // Конструктор класу Service
        public Service(string serviceName, int serviceCost)
        {
            ServiceName = serviceName;
            ServiceCost = serviceCost;
        }
    }
}
