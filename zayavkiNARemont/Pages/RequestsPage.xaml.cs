using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using zayavkiNARemont.Conn;

namespace zayavkiNARemont.Pages
{
    public partial class RequestsPage : Page
    {
        private users _currentUser;

        public RequestsPage(users user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadRequests();
            LoadStatistics();
        }

        private void LoadRequests()
        {
            var requests = DB.equipmentRepairDBEntities.repair_requests
                .Include("equipment_types")
                .Include("fault_types")
                .Include("users")
                .Include("users1")
                .OrderByDescending(x => x.created_date)
                .ToList();

            lvRequests.ItemsSource = requests;
        }

        private void LoadStatistics()
        {
            var requests = DB.equipmentRepairDBEntities.repair_requests.ToList();

            int total = requests.Count;
            int completed = requests.Count(x => x.status == "completed");

            tbTotal.Text = total.ToString();
            tbCompleted.Text = completed.ToString();

            var completedRequests = requests.Where(x => x.status == "completed"
                && x.actual_completion_date.HasValue && x.created_date.HasValue);

            if (completedRequests.Any())
            {
                double avgHours = completedRequests.Average(x =>
                    (x.actual_completion_date.Value - x.created_date.Value).TotalHours);
                tbAvgTime.Text = Math.Round(avgHours, 1).ToString();
            }
            else
            {
                tbAvgTime.Text = "0";
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string search = tbSearch.Text.Trim().ToLower();
            string status = (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            var requests = DB.equipmentRepairDBEntities.repair_requests
                .Include("equipment_types")
                .Include("fault_types")
                .Include("users")
                .Include("users1")
                .ToList();

            if (!string.IsNullOrEmpty(search))
            {
                requests = requests.Where(x => x.request_number.ToLower().Contains(search) ||
                                               x.problem_description.ToLower().Contains(search)).ToList();
            }

            if (status != "Все")
            {
                requests = requests.Where(x => x.status == status).ToList();
            }

            lvRequests.ItemsSource = requests;
        }

        private void LvRequests_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lvRequests.SelectedItem is repair_requests selected)
            {
                NavigationService.Navigate(new RequestCardPage(_currentUser, selected));
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NavigationPage(_currentUser));
        }
    }
}