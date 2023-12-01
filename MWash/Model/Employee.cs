﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWash
{
    public class Employee
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public int Id { get; set; }

        // Конструктор класу Employee
        public Employee(string lastName, string firstName, string phoneNumber, int id)
        {
            LastName = lastName;
            FirstName = firstName;
            PhoneNumber = phoneNumber;

        }
    }
}
