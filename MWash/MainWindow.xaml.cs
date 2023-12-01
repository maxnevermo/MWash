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
            PopulateServiceDataGrid();

            // ... інші ініціалізації або налаштування інтерфейсу
        }

        private void openSalaryButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            Salary.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            Salary.IsHitTestVisible = true;
                        
        }

        private void PopulateServiceDataGrid()
        {
            // Захардкоджена інформація для відображення в таблиці
            var hardcodedData = new List<(string Employee, string Service, int Price, DateTime Time)>
        {
        ("John Doe", "Car Wash", 50, new DateTime(2023, 12, 1, 10, 30, 0)),
        ("Jane Smith", "Interior Cleaning", 80, new DateTime(2023, 12, 1, 11, 0, 0)),
        ("Alice Johnson", "Wheel Maintenance", 30, new DateTime(2023, 12, 1, 12, 15, 0)),
        // Додайте інші дані за необхідністю
        };

            // Створення нового списку об'єктів для відображення в DataGrid
            var serviceDataList = hardcodedData.Select((data, index) => new
            {
                Number = index + 1,
                data.Employee,
                data.Service,
                data.Price,
                Time = data.Time.ToString("dd MMMM, yyyy")
            }).ToList();

            // Очистити ServiceDataGrid перед додаванням нових даних
            ServiceDataGrid.ItemsSource = null;

            // Додавання нових даних до ServiceDataGrid
            ServiceDataGrid.ItemsSource = serviceDataList;
        }


        private void exitSalaryButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            Salary.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            Salary.IsHitTestVisible = false;
        }
    }
}
