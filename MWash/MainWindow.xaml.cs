using MWash.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MWash
{
    public partial class MainWindow : Window
    {
            private MWashAccounting accounting = new MWashAccounting(); // Додайте це поле
            //for managing at one service table
            ObservableCollection<Employee> employessAtOneService = new ObservableCollection<Employee>();
            UserRepository userRepository = new UserRepository();

            public MainWindow()
            {
                InitializeComponent();
                List<Employee> allUsers = userRepository.GetUsers();

                if (allUsers != null )
                {
                    foreach (var user in allUsers)
                    {
                        accounting.EmployeesList.Add(user);
                    }
                }
                
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
            var employees = accounting.EmployeesList;
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

            ServiceComboBox.Text = string.Empty;
            Service.IsHitTestVisible = true;
            EmployeeNameTextBox.Text = "";
            employessAtOneService.Clear();
            currentEmployeesDataGrid.ItemsSource = null;
        }
        private void addEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            string surname_name_of_employee = EmployeeNameTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(surname_name_of_employee))
            {
                string[] nameParts = Regex.Split(surname_name_of_employee, @"\s+");
                //check if two words are entered
                if (nameParts.Length == 2 ) {
                    string name = nameParts[1];
                    string surname = nameParts[0];

                    // Add employees to the employee at one service table
                    Employee newEmployee = new Employee(surname, name, "", 0);

                    // Find the matching employee
                    Employee toFind = accounting.EmployeesList.FirstOrDefault(e => e.LastName == newEmployee.LastName && e.FirstName == newEmployee.FirstName);

                    if (toFind != null)
                    {
                        employessAtOneService.Add(toFind);

                        EmployeeNameTextBox.Text = "";

                        currentEmployeesDataGrid.ItemsSource = null;
                        currentEmployeesDataGrid.ItemsSource = employessAtOneService;
                    }

                    else
                    {
                        MessageBox.Show("Current employee doesn’t exists. Add it to the data first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    currentEmployeesDataGrid.ItemsSource = null; // Clear the ItemsSource
                    currentEmployeesDataGrid.ItemsSource = employessAtOneService; // Reassign the ItemsSource
                }
                else
                {
                    MessageBox.Show("Enter exactly two words (surname and name)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Enter employee surname and name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        async private void deleteEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            Employee itemToDelete = (sender as Button).Tag as Employee;

            if (itemToDelete != null)
            {
                //get row to delete
                DataGridRow row = currentEmployeesDataGrid.ItemContainerGenerator.ContainerFromItem(itemToDelete) as DataGridRow;

                if (row != null)
                {
                    //animation for delete
                    var storyboard = new Storyboard();

                    var opacityAnimation = new DoubleAnimation
                    {
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.3),
                    };

                    Storyboard.SetTarget(opacityAnimation, row);
                    Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));
                    storyboard.Children.Add(opacityAnimation);

                    storyboard.Begin();

                    await Task.Delay(300);

                    //delete elemenet

                    employessAtOneService.Remove(itemToDelete);

                    //refresh table
                    CollectionViewSource.GetDefaultView(currentEmployeesDataGrid.ItemsSource).Refresh();

                }
            }

        }

        private void addServiceButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedService = ServiceComboBox.Text; // Отримання вибраної користувачем послуги зі списку

            // Ініціалізація словаря для зберігання відповідності назв послуг та їх вартостей
            Dictionary<string, int> services = new Dictionary<string, int>
                {
                    { "Лише кузов", 250 },
                    { "Кузов та салон", 350 },
                    { "Хімчистка", 1800 }
                };

            if (services.ContainsKey(selectedService))
            {
                int serviceCost = services[selectedService]; // Отримання вартості вибраної послуги

                // Перевірка наявності працівника з введеними ім'ям та прізвищем у вашій системі
                // Якщо працівника немає, ви маєте додати логіку для його створення або вибору зі списку наявних працівників

                // Початковий та кінцевий часи надання послуг
                //DateTime startTime = DateTime.Now;
                //DateTime endTime = DateTime.Now.AddHours(1); // Припустимо, що послуга триває годину

                // Створення нової послуги
                Service newService = new Service(selectedService, (int)serviceCost);

                // Отримання вибраного часу (години та хвилини)
                int selectedHour = int.Parse(((ComboBoxItem)HourComboBox.SelectedItem).Content.ToString());
                int selectedMinute = int.Parse(((ComboBoxItem)MinuteComboBox.SelectedItem).Content.ToString());

                DateTime startTime = DateTime.Today.AddHours(selectedHour).AddMinutes(selectedMinute);
                DateTime endTime = startTime.AddMinutes(30);

                //check if employees were entered
                if (employessAtOneService.Count > 0)
                {
                    //create list with all employees that were added to atOneService table

                    List<Employee> allEmplyees = new List<Employee>();

                    foreach (var employee in employessAtOneService)
                    {
                        allEmplyees.Add(employee);
                    }
                    // Створення запису про надання послуги з вибраним працівником
                    ServiceRecord newServiceRecord = new ServiceRecord(allEmplyees, newService, startTime, endTime);

                    // Додавання нового запису до ServiceRecords через об'єкт MWashAccounting (accounting)
                    accounting.AddServiceRecord(newServiceRecord);

                    // Оновлення відображення у DataGrid після додавання нового запису
                    PopulateServiceDataGrid();
                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                    Service.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                    Service.IsHitTestVisible = false;
                }
                else
                {
                    MessageBox.Show("Не введено жодного працівника", "Помилка");
                }
            }
            else
            {
                MessageBox.Show("Вибрана послуга не існує у списку.", "Помилка");
            }
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

            // Очищення EmployeesDataGrid перед оновленням даних
            EmployeesDataGrid.ItemsSource = null;

            // Додавання оновлених даних до EmployeesDataGrid
            EmployeesDataGrid.ItemsSource = accounting.EmployeesList;
        }



        private void exitEmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            Employees.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            Employees.IsHitTestVisible = false;
        }
        private void openAddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            Surname.Text = "";
            Name.Text = "";
            Phone.Text = "";

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

        private void AddEmployeeToTableButton_Click(object sender, RoutedEventArgs e)
        {
            string surname = Surname.Text;
            string name = Name.Text;
            string phone = Phone.Text;
            int id = accounting.EmployeesList.Count+1;

            Employee employee = new Employee(surname, name, phone, id);

            if(!accounting.EmployeesList.Contains(employee))
            {
                accounting.EmployeesList.Add(employee);
                userRepository.AddUser(employee);
            }
            else
            {
                MessageBox.Show("Employee already exists", "Duplicate Employee", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            PopulateEmployeesDataGrid();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            AddEmployee.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            AddEmployee.IsHitTestVisible = false;

        }

        async private void DeleteEmployeeFromTableButton_Click(object sender, RoutedEventArgs e)
        {
            Employee itemToDelete = (sender as Button).Tag as Employee;

            if (itemToDelete != null)
            {
                //get row to delete
                DataGridRow row = EmployeesDataGrid.ItemContainerGenerator.ContainerFromItem(itemToDelete) as DataGridRow;

                if (row != null)
                {
                    int index = accounting.EmployeesList.IndexOf(itemToDelete);
                    //animation for delete
                    var storyboard = new Storyboard();

                    var opacityAnimation = new DoubleAnimation
                    {
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.3),
                    };

                    Storyboard.SetTarget(opacityAnimation, row);
                    Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));
                    storyboard.Children.Add(opacityAnimation);

                    storyboard.Begin();

                    await Task.Delay(300);

                    //delete elemenet

                    accounting.EmployeesList.RemoveAt(index);
                    userRepository.DeleteUser(itemToDelete);

                    //refresh table
                    CollectionViewSource.GetDefaultView(EmployeesDataGrid.ItemsSource).Refresh();

                }
            }
        }
    }
}
