using MWash.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
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


                if (allUsers != null )
                {
                    foreach (var user in allUsers)
                    {
                        accounting.EmployeesList.Add(user);
                    }
                }

                if(allRecords != null)
                {
                    foreach(var record in allRecords)
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

                    HourComboBox.Items.Add(sb.ToString());
                }
                for (int i = 0; i <= 60; i++)
                {
                    StringBuilder sb = new StringBuilder();

                    if (i < 10)
                    {
                        sb.Append("0");
                    }

                    sb.Append(i);

                    MinuteComboBox.Items.Add(sb.ToString());
                }
                FillEmployeeComboBox();
        }

        private void FillEmployeeComboBox()
        {
            // Отримання списку працівників з об'єкта accounting
            List<Employee> allUsers = userRepository.GetUsers();

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
            //EmployeeNameTextBox.Text = "";
            employessAtOneService.Clear();
            HourComboBox.SelectedIndex = 0;
            MinuteComboBox.SelectedIndex = 0;
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
                    MessageBox.Show("Обраний працівник не знайдений. Спробуйте знову.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть працівника зі списку.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        /*private void addServiceButton_Click(object sender, RoutedEventArgs e)
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

                // Отримання вибраного працівника з ComboBox
                Employee selectedEmployee = EmployeeComboBox.SelectedItem as Employee;

                if (selectedEmployee != null)
                {
                    // Створення нової послуги
                    Service newService = new Service(selectedService, serviceCost);

                    // Отримання вибраного часу (години та хвилини)
                    int selectedHour = int.Parse(((ComboBoxItem)HourComboBox.SelectedItem).Content.ToString());
                    int selectedMinute = int.Parse(((ComboBoxItem)MinuteComboBox.SelectedItem).Content.ToString());

                    DateTime startTime = DateTime.Today.AddHours(selectedHour).AddMinutes(selectedMinute);
                    DateTime endTime = startTime.AddMinutes(30);

                    // Створення запису про надання послуги з вибраним працівником
                    ServiceRecord newServiceRecord = new ServiceRecord(new List<Employee> { selectedEmployee }, newService, startTime, endTime);

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
                    MessageBox.Show("Помилка: Оберіть працівника зі списку.", "Помилка");
                }
            }
            else
            {
                MessageBox.Show("Помилка: Вибрана послуга не існує у списку.", "Помилка");
            }
        }*/



        private void addServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isEditing)
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


                    // Початковий та кінцевий часи надання послуг
                    //DateTime startTime = DateTime.Now;
                    //DateTime endTime = DateTime.Now.AddHours(1); // Припустимо, що послуга триває годину

                    // Створення нової послуги
                    Service newService = new Service(selectedService, (int)serviceCost);

                    // Отримання вибраного часу (години та хвилини)
                    int selectedHour = int.Parse(HourComboBox.SelectedItem.ToString());
                    int selectedMinute = int.Parse(MinuteComboBox.SelectedItem.ToString());


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
                        serviceRepository.AddUser(newServiceRecord);
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
            else
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
                    int selectedHour = int.Parse(HourComboBox.SelectedItem.ToString());
                    int selectedMinute = int.Parse(MinuteComboBox.SelectedItem.ToString());

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
                        ServiceRecord newServiceRecord = accounting.ServiceRecords[editIndex-1];

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
                        MessageBox.Show("Не введено жодного працівника", "Помилка");
                    }
                }
                else
                {
                    MessageBox.Show("Вибрана послуга не існує у списку.", "Помилка");
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
            foreach(var item in accounting.EmployeesList)
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
                    accounting.ServiceRecords = new ObservableCollection<ServiceRecord>(serviceRepository.GetUsers());

                    //refresh table
                    PopulateEmployeesDataGrid();

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
                    // Get the value of the 'id' property and try parsing it as an integer
                    if (int.TryParse(propertyInfo.GetValue(taggedObject)?.ToString(), out int index))
                    {
                        // Animation for delete
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

                        // Wait for the animation to complete
                        await Task.Delay(300);

                        // Delete the item from the underlying data source
                        if (index-1 >= 0 && index-1 < accounting.ServiceRecords.Count)
                        {
                            accounting.ServiceRecords.RemoveAt(index-1);
                        }
                        else
                        {
                            // Handle the case where the index is out of range
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
                    // Get the value of the 'id' property and try parsing it as an integer
                    if (int.TryParse(propertyInfo.GetValue(taggedObject)?.ToString(), out int index))
                    {
                        DataGridRow row = ServiceDataGrid.ItemContainerGenerator.ContainerFromItem(taggedObject) as DataGridRow;


                        if (index - 1 >= 0 && index - 1 < accounting.ServiceRecords.Count)
                        {
                            editIndex = index;
                            ServiceRecord newServiceRecord = accounting.ServiceRecords[index-1];

                            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                            Service.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                            HourComboBox.SelectedIndex = newServiceRecord.StartTime.Hour;
                            MinuteComboBox.SelectedIndex = newServiceRecord.StartTime.Minute;
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


        private void GenerateReport(object sender, RoutedEventArgs e)
        {
            var selectedItem = ReportComboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                string selectedOption = selectedItem.Content.ToString();

                if (selectedOption == "за день")
                {
                    // Отримати поточну дату для формування звіту за день
                    DateTime currentDate = DateTime.Now;
                    accounting.GenerateReportForDay(currentDate);
                }
                else if (selectedOption == "за тиждень")
                {
                    // Отримати дати для формування звіту за тиждень (останній тиждень)
                    DateTime endDate = DateTime.Now;
                    DateTime startDate = endDate.AddDays(-7);
                    accounting.GenerateReportForWeek(startDate);

                    // Якщо користувач вибрав "Створити файл звіту", зберегти звіт у текстовий файл
                    if (CreateFileCheckBox.IsChecked == true)
                    {
                        // Зберегти звіт у текстовий файл
                        // Вам необхідно додати відповідний код для збереження у файлі звіту за тиждень
                        // Наприклад:
                        // accounting.SaveReportToFile("Звіт_за_тиждень.txt", reportContent);
                    }
                }
                else if (selectedOption == "за обраний проміжок часу")
                {
                    // Отримати обраний проміжок часу для формування звіту
                    // DateTime selectedStartDate = ...; // введіть обрану користувачем початкову дату
                    // DateTime selectedEndDate = ...;   // введіть обрану користувачем кінцеву дату
                    // Ваш код для формування звіту за обраний проміжок часу
                    // accounting.GenerateReportForSelectedPeriod(selectedStartDate, selectedEndDate);

                    // Якщо користувач вибрав "Створити файл звіту", додайте код для збереження у текстовий файл
                    if (CreateFileCheckBox.IsChecked == true)
                    {
                        // Зберегти звіт у текстовий файл
                        // Вам необхідно додати відповідний код для збереження у файлі звіту за обраний проміжок часу
                    }
                }
            }
        }




    }
}
