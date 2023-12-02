using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace MWash
{
    public partial class MainWindow : Window
    {
        private MWashAccounting accounting; // Додайте це поле

        public MainWindow()
        {
            InitializeComponent();
            accounting = new MWashAccounting(); // Ініціалізуйте об'єкт MWashAccounting

            List<Employee> employees1 = new List<Employee>
    {
        new Employee("Doe", "John", "123456789", 1),
        new Employee("Smith", "Jane", "987654321", 2)
    };

            List<Employee> employees2 = new List<Employee>
    {
        new Employee("Brown", "Mike", "111111111", 3)
    };

            List<Employee> employees3 = new List<Employee>
    {
        new Employee("Garcia", "Carlos", "333333333", 5),
        new Employee("Chen", "Ling", "444444444", 6)
    };

            // Створення послуг
            Service service1 = new Service("Car Wash", 250); // Перша послуга
            Service service2 = new Service("Interior Detailing", 150); // Друга послуга
            Service service3 = new Service("Exterior Detailing", 200); // Третя послуга

            // Початковий та кінцевий часи надання послуг
            DateTime startTime1 = DateTime.Now;
            DateTime endTime1 = DateTime.Now.AddHours(1); // Припустимо, що послуга триває годину

            DateTime startTime2 = DateTime.Now.AddHours(2);
            DateTime endTime2 = DateTime.Now.AddHours(3); // Припустимо, що послуга триває годину

            DateTime startTime3 = DateTime.Now.AddHours(4);
            DateTime endTime3 = DateTime.Now.AddHours(5); // Припустимо, що послуга триває годину

            // Створення записів про надання послуг
            ServiceRecord serviceRecord1 = new ServiceRecord(employees1, service1, startTime1, endTime1);
            ServiceRecord serviceRecord2 = new ServiceRecord(employees2, service2, startTime2, endTime2);
            ServiceRecord serviceRecord3 = new ServiceRecord(employees3, service3, startTime3, endTime3);

            // Додавання нових записів до ServiceRecords через об'єкт MWashAccounting (accounting)
            accounting.AddServiceRecord(serviceRecord1);
            accounting.AddServiceRecord(serviceRecord2);
            accounting.AddServiceRecord(serviceRecord3);

            // Оновлення відображення у DataGrid після додавання нових записів
            PopulateServiceDataGrid();

            // ... інші ініціалізації або налаштування інтерфейсу
        }


        private void openSalaryButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            Salary.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            Salary.IsHitTestVisible = true;
            PopulateSalaryDataGrid();
        }

        private void PopulateSalaryDataGrid()
        {
            // Очищення DataGrid перед оновленням даних
            SalaryDataGrid.ItemsSource = null;

            // Отримання даних про зарплату для кожного працівника з об'єкту MWashAccounting
            var employees = accounting.GetEmployees();
            var salaryDataList = new List<EmployeeSalary>(); // Користувацький клас для зберігання даних про заробітну плату

            foreach (var employee in employees)
            {
                double dailySalary = accounting.CalculateDailySalary(employee);

                // Додавання даних про зарплату у список
                salaryDataList.Add(new EmployeeSalary
                {
                    Surname = employee.LastName,
                    Name = employee.FirstName,
                    Salary = dailySalary
                });
            }

            // Відображення даних про зарплату у DataGrid
            SalaryDataGrid.ItemsSource = salaryDataList;
        }

        private void exitSalaryButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            Salary.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            Salary.IsHitTestVisible = false;
                        
        }

        private void openReportButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            Report.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            Report.IsHitTestVisible = true;
        }

        private void exitReportButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            Report.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            Report.IsHitTestVisible = false;
        }

        private void openServiceButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            Service.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            Service.IsHitTestVisible = true;
        }

        private void addServiceButton_Click(object sender, RoutedEventArgs e)
        {
            // Отримання даних, введених користувачем у вікні "Add service window"
            string serviceName = ServiceNameTextBox.Text; // Припустимо, що ви маєте текстове поле для введення назви послуги
            double serviceCost = Convert.ToDouble(ServiceCostTextBox.Text); // Припустимо, що ви маєте текстове поле для введення вартості послуги
            string employeeName = EmployeeNameTextBox.Text; // Ім'я працівника
            string employeeLastName = EmployeeLastNameTextBox.Text; // Прізвище працівника
            string employeePhone = EmployeePhoneTextBox.Text; // Номер телефону працівника

            // Початковий та кінцевий часи надання послуг
            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now.AddHours(1); // Припустимо, що послуга триває годину

            // Створення працівника з введеними даними
            Employee newEmployee = new Employee(employeeLastName, employeeName, employeePhone, 0); // Потрібно буде вказати Id працівника

            // Створення нової послуги
            Service newService = new Service(serviceName, (int)serviceCost);

            // Створення запису про надання послуги з введеним працівником
            ServiceRecord newServiceRecord = new ServiceRecord(new List<Employee> { newEmployee }, newService, startTime, endTime);

            // Додавання нового запису до ServiceRecords через об'єкт MWashAccounting (accounting)
            accounting.AddServiceRecord(newServiceRecord);

            // Оновлення відображення у DataGrid після додавання нового запису
            PopulateServiceDataGrid();
        }


        private void exitServiceButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            Service.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            Service.IsHitTestVisible = false;
        }

        private void openEmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            Employees.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            PopulateEmployeesDataGrid();

            Employees.IsHitTestVisible = true;
        }

        private void PopulateEmployeesDataGrid()
        {
            // Отримання списку всіх працівників з об'єкту MWashAccounting
            var employees = accounting.GetEmployees();

            // Створення анонімного об'єкту для відображення даних про працівників у DataGrid
            var employeesDataList = employees.Select(emp => new
            {
                Surname = emp.LastName,
                Name = emp.FirstName,
                Phone = emp.PhoneNumber,
                Id = emp.Id
            }).ToList();

            // Очищення EmployeesDataGrid перед оновленням даних
            EmployeesDataGrid.ItemsSource = null;

            // Додавання оновлених даних до EmployeesDataGrid
            EmployeesDataGrid.ItemsSource = employeesDataList;
        }



        private void exitEmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            Employees.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            Employees.IsHitTestVisible = false;
        }
        private void openAddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            AddEmployee.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            AddEmployee.IsHitTestVisible = true;
        }

        private void exitAddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            AddEmployee.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            AddEmployee.IsHitTestVisible = false;
        }

        private void PopulateServiceDataGrid()
        {
            // Accessing the ServiceRecords collection from MWashAccounting
            var serviceDataList = accounting.ServiceRecords.Select((record, index) => new
            {
                Number = index + 1,
                Employee = string.Join(", ", record.Employees.Select(emp => $"{emp.FirstName} {emp.LastName}")),
                Service = record.Service.ServiceName,
                Price = record.Service.ServiceCost,
                Time = record.StartTime.ToString("dd MMMM, yyyy HH:mm:ss")
            }).ToList();

            // Clearing ServiceDataGrid before adding new data
            ServiceDataGrid.ItemsSource = null;

            // Adding the updated data to ServiceDataGrid
            ServiceDataGrid.ItemsSource = serviceDataList;
        }


    }
}
