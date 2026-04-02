using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using zayavkiNARemont.Conn;
using System.Collections.Generic;

namespace zayavkiNARemont.Pages
{
    public partial class RequestsListPage : Page
    {
        private users currentUser;
        private List<repair_requests> requestsList;

        public RequestsListPage(users user)
        {
            InitializeComponent();
            currentUser = user;
            LoadRequests();
        }

        private void LoadRequests()
        {
            requestsList = DB.equipmentRepairDBEntities.repair_requests
                .Include("equipment_types")
                .Include("fault_types")
                .Include("users")
                .Include("users1")
                .OrderByDescending(x => x.created_date)
                .ToList();

            lvRequests.ItemsSource = requestsList;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string search = tbSearch.Text.Trim().ToLower();
            string status = (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            var filtered = requestsList;

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(x => x.request_number.ToLower().Contains(search) ||
                                               x.problem_description.ToLower().Contains(search)).ToList();
            }

            if (status != "Все")
            {
                filtered = filtered.Where(x => x.status == status).ToList();
            }

            lvRequests.ItemsSource = filtered;
        }

        private void LvRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvRequests.SelectedItem is repair_requests selected)
            {
                NavigationService.Navigate(new RequestCardPage(currentUser, selected));
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NavigationPage(currentUser));
        }
    }
}