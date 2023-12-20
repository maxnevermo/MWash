using MWash.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace MWash
{
    public partial class MainWindow : Window
    {

        System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
        private MWashAccounting accounting = new MWashAccounting(); 
        ObservableCollection<Employee> employessAtOneService = new ObservableCollection<Employee>();
        UserRepository userRepository = new UserRepository();
        ServiceRepository serviceRepository = new ServiceRepository();

        private ObservableCollection<Employee> employees = new ObservableCollection<Employee>();

        public int SelectedEmployeeId { get; set; }  // Зберігання обраного ідентифікатора працівника
        public string SelectedEmployeeFullName { get; set; }  // Зберігання повного імені обраного працівника

        public MainWindow()
        {
            InitializeComponent();

            List<Employee> allUsers = userRepository.GetUsers();
            List<ServiceRecord> allRecords = serviceRepository.GetUsers();

            DataContext = this;


            if (allUsers != null)
            {
                foreach (var user in allUsers)
                {
                    accounting.EmployeesList.Add(user);
                }
            }

            if (allRecords != null)
            {
                foreach (var record in allRecords)
                {
                    accounting.ServiceRecords.Add(record);
                }
            }
            PopulateServiceDataGrid();

            for (int i = 0; i < 24; i++)
            {
                StringBuilder sb = new StringBuilder();

                if (i < 10)
                {
                    sb.Append("0");
                }

                sb.Append(i);

                StartHourComboBox.Items.Add(sb.ToString());
                EndHourComboBox.Items.Add(sb.ToString());
            }
            for (int i = 0; i <= 60; i++)
            {
                StringBuilder sb = new StringBuilder();

                if (i < 10)
                {
                    sb.Append("0");
                }

                sb.Append(i);

                StartMinuteComboBox.Items.Add(sb.ToString());
                EndMinuteComboBox.Items.Add(sb.ToString());
            }
            FillEmployeeComboBox();
        }

        private void FillEmployeeComboBox()
        {
            EmployeeComboBox.ItemsSource = null;
            employees.Clear();
            // Отримання списку працівників з об'єкта accounting
            List<Employee> allUsers = accounting.EmployeesList;

            if (allUsers != null)
            {
                foreach (var user in allUsers)
                {
                    employees.Add(user);
                }
            }

            // Налаштування джерела даних для ComboBox
            EmployeeComboBox.ItemsSource = employees;
            EmployeeComboBox.DisplayMemberPath = "FullName"; // Поле, яке ви хочете відображати у ComboBox
            EmployeeComboBox.SelectedValuePath = "Id"; // Поле, яке ви хочете використовувати як значення в ComboBox
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

        void openReportTableButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            ReportTable.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            ReportTable.IsHitTestVisible = true;
        }

        private void exitReportTableButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            ReportTable.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            ReportTable.IsHitTestVisible = false;
        }

        private void exitReportDateButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            ReportDatePicker.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            ReportDatePicker.IsHitTestVisible = false;
        }

        private void exitReportButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            Report.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            Report.IsHitTestVisible = false;
        }

        private void openServiceButton_Click(object sender, RoutedEventArgs e)
        {
            isEditing = false;

            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            Service.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            ServiceComboBox.Text = string.Empty;
            Service.IsHitTestVisible = true;
            //EmployeeComboBox.Text = string.Empty;
            employessAtOneService.Clear();
            StartHourComboBox.Text = string.Empty;
            EndHourComboBox.Text = string.Empty;
            StartMinuteComboBox.Text = string.Empty;
            EndMinuteComboBox.Text = string.Empty;
            currentEmployeesDataGrid.ItemsSource = null;
        }

        private void addEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedEmployeeId = EmployeeComboBox.SelectedValue as int? ?? -1;

            if (selectedEmployeeId != -1)
            {
                // Знайдення працівника за обраним ідентифікатором у списку всіх працівників
                Employee selectedEmployee = accounting.EmployeesList.FirstOrDefault(emp => emp.Id == selectedEmployeeId);

                if (selectedEmployee != null)
                {
                    employessAtOneService.Add(selectedEmployee);

                    currentEmployeesDataGrid.ItemsSource = null;
                    currentEmployeesDataGrid.ItemsSource = employessAtOneService;
                }
                else
                {
                    MessageBox.Show("The selected worker was not found. Try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a worker from the list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        async private void deleteEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            Employee itemToDelete = (sender as Button).Tag as Employee;

            if (itemToDelete != null)
            {
                //Отримання рядка для видалення
                DataGridRow row = currentEmployeesDataGrid.ItemContainerGenerator.ContainerFromItem(itemToDelete) as DataGridRow;

                if (row != null)
                {
                    //Анімація видалення
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

                    //Видалити працівника

                    employessAtOneService.Remove(itemToDelete);


                    //Оновити таблицю
                    CollectionViewSource.GetDefaultView(currentEmployeesDataGrid.ItemsSource).Refresh();

                }
            }

        }

        private void addServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isEditing)
            {
                string selectedService = ServiceComboBox.Text; // Отримання вибраної користувачем послуги зі списку

                int minMinutes;
                switch (selectedService)
                {
                    case "Body only":
                        minMinutes = 15;
                        break;
                    case "Body and interior":
                        minMinutes = 25;
                        break;
                    case "Dry cleaning":
                        minMinutes = 60;
                        break;
                    default:
                        minMinutes = 30; // Дефолтне значення
                        break;
                }

                // Ініціалізація  для зберігання відповідності назв послуг та їх вартостей
                Dictionary<string, int> services = new Dictionary<string, int>
                {
                    { "Body only", 250 },
                    { "Body and interior", 350 },
                    { "Dry cleaning", 1800 }
                };


                if (services.ContainsKey(selectedService))
                {
                    int serviceCost = services[selectedService]; // Отримання вартості вибраної послуги


                    // Створення нової послуги
                    Service newService = new Service(selectedService, (int)serviceCost);

                    if (string.IsNullOrEmpty(StartHourComboBox.SelectedItem?.ToString()) || string.IsNullOrEmpty(StartMinuteComboBox.SelectedItem?.ToString()) || string.IsNullOrEmpty(EndHourComboBox.SelectedItem?.ToString()) || string.IsNullOrEmpty(EndMinuteComboBox.SelectedItem?.ToString()))
                    {
                        MessageBox.Show("Please select both start and end times.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Отримання вибраного часу (години та хвилини)
                    int selectedHour = int.Parse(StartHourComboBox.SelectedItem.ToString());
                    int selectedMinute = int.Parse(StartMinuteComboBox.SelectedItem.ToString());

                    int selectedEndHour = int.Parse(EndHourComboBox.SelectedItem.ToString());
                    int selectedEndMinute = int.Parse(EndMinuteComboBox.SelectedItem.ToString());

                    

                    DateTime startTime = DateTime.Today.AddHours(selectedHour).AddMinutes(selectedMinute);
                    DateTime endTime = DateTime.Today.AddHours(selectedEndHour).AddMinutes(selectedEndMinute);

                    if (endTime <= startTime)
                    {
                        MessageBox.Show("End time should be greater than start time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if ((endTime - startTime).TotalMinutes < minMinutes)
                    {
                        // Вивести повідомлення про помилку
                        MessageBox.Show($"Minimum duration for {selectedService} is {minMinutes} minutes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

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
                        serviceRepository.AddUser(newServiceRecord);
                        DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                        Service.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                        Service.IsHitTestVisible = false;

                    }
                    else
                    {
                        MessageBox.Show("No worker entered", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("The selected service does not exist in the list.", "Error");
                }
            }
            else
            {
                string selectedService = ServiceComboBox.Text; // Отримання вибраної користувачем послуги зі списку

                int minMinutes;
                switch (selectedService)
                {
                    case "Body only":
                        minMinutes = 15;
                        break;
                    case "Body and interior":
                        minMinutes = 25;
                        break;
                    case "Dry cleaning":
                        minMinutes = 60;
                        break;
                    default:
                        minMinutes = 30; // Дефолтне значення
                        break;
                }

                // Ініціалізація словаря для зберігання відповідності назв послуг та їх вартостей
                Dictionary<string, int> services = new Dictionary<string, int>
                {
                    { "Body only", 250 },
                    { "Body and interior", 350 },
                    { "Dry cleaning", 1800 }
                };

                if (services.ContainsKey(selectedService))
                {
                    int serviceCost = services[selectedService]; // Отримання вартості вибраної послуги

                    // Створення нової послуги
                    Service newService = new Service(selectedService, (int)serviceCost);

                    if (string.IsNullOrEmpty(StartHourComboBox.SelectedItem?.ToString()) || string.IsNullOrEmpty(StartMinuteComboBox.SelectedItem?.ToString()) || string.IsNullOrEmpty(EndHourComboBox.SelectedItem?.ToString()) || string.IsNullOrEmpty(EndMinuteComboBox.SelectedItem?.ToString()))
                    {
                        MessageBox.Show("Please select both start and end times.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }


                    // Отримання вибраного часу (години та хвилини)
                    int selectedHour = int.Parse(StartHourComboBox.SelectedItem.ToString());
                    int selectedMinute = int.Parse(StartMinuteComboBox.SelectedItem.ToString());

                    int selectedEndHour = int.Parse(EndHourComboBox.SelectedItem.ToString());
                    int selectedEndMinute = int.Parse(EndMinuteComboBox.SelectedItem.ToString());

                    

                    DateTime startTime = DateTime.Today.AddHours(selectedHour).AddMinutes(selectedMinute);
                    DateTime endTime = DateTime.Today.AddHours(selectedEndHour).AddMinutes(selectedEndMinute);

                    if (endTime <= startTime)
                    {
                        MessageBox.Show("End time should be greater than start time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if ((endTime - startTime).TotalMinutes < minMinutes)
                    {
                        // Вивести повідомлення про помилку
                        MessageBox.Show($"Minimum duration for {selectedService} is {minMinutes} minutes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (employessAtOneService.Count > 0)
                    {
                        //Створення списку усіх працівників, які були залучені до послуги

                        List<Employee> allEmplyees = new List<Employee>();

                        foreach (var employee in employessAtOneService)
                        {
                            allEmplyees.Add(employee);
                        }
                        // Створення запису про надання послуги з вибраним працівником
                        ServiceRecord newServiceRecord = accounting.ServiceRecords[editIndex - 1];

                        newServiceRecord.Service = newService;
                        newServiceRecord.Employees = allEmplyees;
                        newServiceRecord.StartTime = startTime;
                        newServiceRecord.EndTime = endTime;

                        // Додавання нового запису до ServiceRecords через об'єкт MWashAccounting (accounting)

                        // Оновлення відображення у DataGrid після додавання нового запису
                        PopulateServiceDataGrid();

                        serviceRepository.UpdateUser(newServiceRecord);

                        DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                        Service.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                        Service.IsHitTestVisible = false;

                    }
                    else
                    {
                        MessageBox.Show("No worker entered.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("The selected service does not exist in the list.", "Error");
                }
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
            int i = 1;
            foreach (var item in accounting.EmployeesList)
            {
                item.Id = i;
                i++;
            }

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
            // Отримання сьогоднішньої дати
            DateTime currentDate = DateTime.Now.Date;

            // Доступ до таблиці записів послуг
            var serviceDataList = accounting.ServiceRecords
                .Where(record => record.StartTime.Date == currentDate) // Фільтруємо записи за сьогоднішньою датою
                .Select((record, index) => new
                {
                    Number = index + 1,
                    Employee = string.Join(", ", record.Employees.Select(emp => $"{emp.FirstName} {emp.LastName}")),
                    Service = record.Service.ServiceName,
                    Price = record.Service.ServiceCost,
                    Time = $"{record.StartTime.ToString("HH:mm", culture)} - {record.EndTime.ToString("HH:mm", culture)}"
                })
                .ToList();

            // Очищення таблиці перед додаванням нової послуги
            ServiceDataGrid.ItemsSource = null;

            // Демонстрація видозміненої таблиці
            ServiceDataGrid.ItemsSource = serviceDataList;
        }


        private void AddEmployeeToTableButton_Click(object sender, RoutedEventArgs e)
        {

            if (!isEmployeeEdit)
            {

            string surname = Surname.Text;
            string name = Name.Text;
            string phone = Phone.Text;
            int id = accounting.EmployeesList.Count + 1;

            // Перевірка на введення лише букв латини для прізвища та імені
            if (!Regex.IsMatch(name, @"^[A-Za-z-]{1,60}$") || !Regex.IsMatch(surname, @"^[A-Za-z-]{1,60}$"))
            {
                MessageBox.Show("Please enter a valid name and surname (only Latin letters and hyphen, maximum 60 characters, minimum 1).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Перевірка на введення лише '+', а потім дванадцять цифр для номеру телефону
            if (!Regex.IsMatch(phone, @"^\+[0-9]{12}$"))
            {
                MessageBox.Show("Please enter a valid phone number (starts with '+', followed by twelve digits).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


                Employee employee = new Employee(surname, name, phone, id);

                if (!accounting.EmployeesList.Contains(employee))
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

                FillEmployeeComboBox();

            }
            else
            {
                string surname = Surname.Text;
                string name = Name.Text;
                string phone = Phone.Text;

                Employee employee = accounting.EmployeesList[editEmployee];

                employee.LastName = surname;
                employee.FirstName = name;
                employee.PhoneNumber = phone;


                userRepository.UpdateUser(employee);

                PopulateEmployeesDataGrid();
                DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                AddEmployee.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                AddEmployee.IsHitTestVisible = false;

                FillEmployeeComboBox();
            }
            
        }


        async private void DeleteEmployeeFromTableButton_Click(object sender, RoutedEventArgs e)
        {
            Employee itemToDelete = (sender as Button).Tag as Employee;

            if (itemToDelete != null)
            {
                //Отримання рядка для видалення
                DataGridRow row = EmployeesDataGrid.ItemContainerGenerator.ContainerFromItem(itemToDelete) as DataGridRow;

                if (row != null)
                {
                    int index = accounting.EmployeesList.IndexOf(itemToDelete);
                    //Анімація видалення
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

                    //Видалення працівника

                    accounting.EmployeesList.RemoveAt(index);
                    userRepository.DeleteUser(itemToDelete);
                    accounting.ServiceRecords = new ObservableCollection<ServiceRecord>(serviceRepository.GetUsers());

                    //Оновлення таблиці
                    PopulateEmployeesDataGrid();
                    FillEmployeeComboBox();

                }
            }
        }

        async private void DeleteServiceButton_Click(object sender, RoutedEventArgs e)
        {
            var taggedObject = (sender as Button)?.Tag;

            if (taggedObject != null)
            {
                PropertyInfo propertyInfo = taggedObject.GetType().GetProperty("Number");

                if (propertyInfo != null)
                {
                    // Отримання ID послуги
                    if (int.TryParse(propertyInfo.GetValue(taggedObject)?.ToString(), out int index))
                    {
                        // Анімація видалення
                        DataGridRow row = ServiceDataGrid.ItemContainerGenerator.ContainerFromItem(taggedObject) as DataGridRow;

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

                        if (index - 1 >= 0 && index - 1 < accounting.ServiceRecords.Count)
                        {
                            accounting.ServiceRecords.RemoveAt(index - 1);
                        }
                        else
                        {
                            // Обробка події, якщо індекс поза межами
                            Console.WriteLine("Index is out of range.");
                        }

                        ServiceDataGrid.ItemsSource = null;
                        ServiceDataGrid.ItemsSource = accounting.ServiceRecords;
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse 'id' property as an integer.");
                    }
                }
                else
                {
                    Console.WriteLine("The 'id' property does not exist.");
                }
            }
            var list = serviceRepository.GetUsers().ToList();

            foreach (var record in list)
            {
                if (accounting.ServiceRecords?.Contains(record) == false)
                {
                    serviceRepository.DeleteUser(record);
                }
            }
            accounting.ServiceRecords = new ObservableCollection<ServiceRecord>(serviceRepository.GetUsers());
            ServiceDataGrid.ItemsSource = null;
            ServiceDataGrid.ItemsSource = accounting.ServiceRecords;
            PopulateServiceDataGrid();
        }

        bool isEditing = true;
        int editIndex = -1;
        private void EditServiceRecordButton_Click(object sender, RoutedEventArgs e)
        {
            isEditing = true;
            var taggedObject = (sender as Button)?.Tag;

            if (taggedObject != null)
            {
                PropertyInfo propertyInfo = taggedObject.GetType().GetProperty("Number");

                if (propertyInfo != null)
                {
                    // Отримання ID послуги для видозміни
                    if (int.TryParse(propertyInfo.GetValue(taggedObject)?.ToString(), out int index))
                    {
                        DataGridRow row = ServiceDataGrid.ItemContainerGenerator.ContainerFromItem(taggedObject) as DataGridRow;


                        if (index - 1 >= 0 && index - 1 < accounting.ServiceRecords.Count)
                        {
                            editIndex = index;
                            ServiceRecord newServiceRecord = accounting.ServiceRecords[index - 1];

                            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                            Service.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                            StartHourComboBox.SelectedIndex = newServiceRecord.StartTime.Hour;
                            StartMinuteComboBox.SelectedIndex = newServiceRecord.StartTime.Minute;
                            ServiceComboBox.Text = newServiceRecord.Service.ServiceName;
                            Service.IsHitTestVisible = true;
                            EmployeeNameTextBox.Text = "";
                            employessAtOneService = new ObservableCollection<Employee>(newServiceRecord.Employees);
                            currentEmployeesDataGrid.ItemsSource = employessAtOneService;

                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse 'id' property as an integer.");
                    }
                }
                else
                {
                    Console.WriteLine("The 'id' property does not exist.");
                }
            }
        }


        /////////////////////
        private List<object> FormatDataForDataGrid(List<(string ServiceName, int TotalCount, double CostPerService)> data)
        {
            return data.Select((record, index) => new
            {
                Number = index + 1,
                ServiceName = record.ServiceName,
                TotalCount = record.TotalCount,
                CostPerService = record.CostPerService,
                TotalCost = record.TotalCount * record.CostPerService // Нова властивість для сумарної вартості
            }).ToList<object>();
        }
        private void PopulateDataGrid(List<(string ServiceName, int TotalCount, double CostPerService)> data, DataGrid dataGrid)
        {
            var formattedData = FormatDataForDataGrid(data);

            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = formattedData;
        }



        private void GenerateReport(object sender, RoutedEventArgs e)
        {
            var selectedItem = ReportComboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                string selectedOption = selectedItem.Content.ToString();

                if (selectedOption == "per day")
                {
                    DateTime currentDate = DateTime.Now;

                    ReportDateTextBlock.Text = currentDate.ToString("d MMM yyyy", culture);
                    List<(string ServiceName, int TotalCount, double CostPerService)> dailyReportData = accounting.GenerateDailyReportForGrid(currentDate);

                    // Отримання загальної суми вартості для усіх послуг
                    double totalCost = dailyReportData.Sum(record => record.TotalCount * record.CostPerService);

                    // Виведення сумарної вартості у TextBlock
                    TotalCostTextBlock.Text = $"Total Cost: {totalCost}";

                    // Використовуємо метод для заповнення DailyReportDataGrid
                    PopulateDataGrid(dailyReportData, DailyReportDataGrid);

                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                    ReportTable.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                    ReportTable.IsHitTestVisible = true;

                    if (CreateFileCheckBox.IsChecked == true)
                    {
                        accounting.GenerateFileReportForDay(currentDate);
                    }
                }
                else if (selectedOption == "per week")
                {
                    // Отримати дати для формування звіту за тиждень (останній тиждень)
                    DateTime endDate = DateTime.Now;
                    DateTime startDate = endDate.AddDays(-7);

                    ReportDateTextBlock.Text = $"{startDate.ToString("d MMM yyyy", culture)} - {endDate.ToString("d MMM yyyy", culture)}";


                    List<(string ServiceName, int TotalCount, double CostPerService)> weeklyReportData = accounting.GenerateWeeklyReportForGrid(endDate);

                    // Отримання загальної суми вартості для усіх послуг
                    double totalCost = weeklyReportData.Sum(record => record.TotalCount * record.CostPerService);

                    // Виведення сумарної вартості у TextBlock
                    TotalCostTextBlock.Text = $"Total Cost: {totalCost}";

                    // Використовуємо метод для заповнення DailyReportDataGrid
                    PopulateDataGrid(weeklyReportData, DailyReportDataGrid);

                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                    ReportTable.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                    ReportTable.IsHitTestVisible = true;

                    // Якщо користувач вибрав "Створити файл звіту", зберегти звіт у текстовий файл
                    if (CreateFileCheckBox.IsChecked == true)
                    {
                        accounting.GenerateFileReportForWeek(startDate);
                    }
                }


                else if (selectedOption == "for the selected period of time")
                {
                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                    ReportDatePicker.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                    ReportDatePicker.IsHitTestVisible = true;
                }
            }
        }

        bool isEmployeeEdit = false;
        int editEmployee = -1;
        private void EditEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            isEmployeeEdit = true;
            Employee itemToDelete = (sender as Button).Tag as Employee;

            if (itemToDelete != null)
            {
                DataGridRow row = EmployeesDataGrid.ItemContainerGenerator.ContainerFromItem(itemToDelete) as DataGridRow;

                if (row != null)
                {
                    int index = accounting.EmployeesList.IndexOf(itemToDelete);

                    editEmployee = index;
                    Employee newEmployee = accounting.EmployeesList[index];

                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                    AddEmployee.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                    AddEmployee.IsHitTestVisible = true;


                    Surname.Text = newEmployee.LastName;
                    Name.Text = newEmployee.FirstName;
                    Phone.Text = newEmployee.PhoneNumber;


                    PopulateEmployeesDataGrid();
                    FillEmployeeComboBox();

                }
            }
        }
        private void ConfirmDateReport(object sender, RoutedEventArgs e) {
            // Отримання обраних користувачем початкової та кінцевої дат для формування звіту
            DateTime selectedStartDate = startDatePicker.SelectedDate.GetValueOrDefault(); // Початкова дата, обрана користувачем
            DateTime selectedEndDate = endDatePicker.SelectedDate.GetValueOrDefault(); // Кінцева дата, обрана користувачем

            if (selectedStartDate > DateTime.Today || selectedEndDate > DateTime.Today)
            {
                MessageBox.Show("Please select dates equal to or less than today's date.", "Invalid Date Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (selectedStartDate > selectedEndDate)
            {
                MessageBox.Show("End date should be greater than or equal to the start date.", "Invalid Date Range", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Генерація звіту за обраним проміжком часу
            List<(string ServiceName, int TotalCount, double CostPerService)> selectedPeriodReportData = accounting.GenerateReportForSelectedPeriod(selectedStartDate, selectedEndDate);

            // Отримання загальної суми вартості для усіх послуг
            double totalCost = selectedPeriodReportData.Sum(record => record.TotalCount * record.CostPerService);

            // Виведення сумарної вартості у TextBlock
            TotalCostTextBlock.Text = $"Total Cost: {totalCost}";

            ReportDateTextBlock.Text = $"{selectedStartDate.ToString("d MMM yyyy", culture)} - {selectedEndDate.ToString("d MMM yyyy", culture)}";

            // Якщо користувач вибрав "Створити файл звіту", додайте код для збереження у текстовий файл
            if (CreateFileCheckBox.IsChecked == true)
            {
                accounting.GenerateFileReportForSelectedPeriod(selectedStartDate, selectedEndDate);
            }

            // Використання методу для заповнення DailyReportDataGrid отриманими даними
            PopulateDataGrid(selectedPeriodReportData, DailyReportDataGrid);

            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            ReportTable.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            ReportTable.IsHitTestVisible = true;
        }
    }
}
