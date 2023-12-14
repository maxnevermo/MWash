using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MWash.Model
{
    public class UserRepository
    {
        private List<Employee> employees = new List<Employee>();
        private string filePath = "users.json"; // Adjust the file path as needed

        public void AddUser(Employee employee)
        {
            employees.Add(employee);
            SaveUsers();
        }

        public void DeleteUser(Employee employee)
        {
            employees.Remove(employee);
            SaveUsers();
        }

        public void UpdateUser(Employee updatedEmployee)
        {
            // Find the existing employee based on their ID
            Employee existingEmployee = employees.FirstOrDefault(e => e.Id == updatedEmployee.Id);

            if (existingEmployee != null)
            {
                // Update the existing employee's properties
                existingEmployee.LastName = updatedEmployee.LastName;
                existingEmployee.FirstName = updatedEmployee.FirstName;
                existingEmployee.PhoneNumber = updatedEmployee.PhoneNumber;

                SaveUsers();
            }
            else
            {
                Console.WriteLine("Employee not found for update.");
            }
        }

        public List<Employee> GetUsers()
        {
            return employees;
        }

        private void SaveUsers()
        {
            string json = JsonConvert.SerializeObject(employees, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        private void LoadUsers()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                employees = JsonConvert.DeserializeObject<List<Employee>>(json);
            }
        }

        public UserRepository()
        {
            LoadUsers();
        }
    }
}
