using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MWash.Model
{

    public class ServiceRepository
    {
        private List<ServiceRecord> services = new List<ServiceRecord>();
        private string filePath = "services.json"; // Adjust the file path as needed

        public void AddUser(ServiceRecord servicerecord)
        {
            services.Add(servicerecord);
            SaveUsers();
        }
        public void UpdateUser(ServiceRecord updatedServiceRecord)
        {
            // Find the existing record based on some criteria, for example, ID
            ServiceRecord existingServiceRecord = services.FirstOrDefault(sr =>
                            sr.Employees.SequenceEqual(updatedServiceRecord.Employees) &&
                            sr.StartTime == updatedServiceRecord.StartTime);

            if (existingServiceRecord != null)
            {
                existingServiceRecord.Employees = updatedServiceRecord.Employees;
                existingServiceRecord.Service = updatedServiceRecord.Service;
                existingServiceRecord.StartTime = updatedServiceRecord.StartTime;
                existingServiceRecord.EndTime = updatedServiceRecord.EndTime;

                SaveUsers();
            }
            else
            {
                Console.WriteLine("Service record not found for update.");
            }
        }
        public void DeleteUser(ServiceRecord servicerecord)
        {
            services.Remove(servicerecord);
            SaveUsers();
        }

        public List<ServiceRecord> GetUsers()
        {
            return services;
        }

        private void SaveUsers()
        {
            string json = JsonConvert.SerializeObject(services, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        private void LoadUsers()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                services = JsonConvert.DeserializeObject<List<ServiceRecord>>(json);
            }
        }

        public ServiceRepository()
        {
            LoadUsers();
        }
    }

}
