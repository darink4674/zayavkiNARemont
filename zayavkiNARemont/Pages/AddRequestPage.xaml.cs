using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using zayavkiNARemont.Conn;

namespace zayavkiNARemont.Pages
{
    public partial class AddRequestPage : Page
    {
        private users _currentUser;

        public AddRequestPage(users user)
        {
            InitializeComponent();
            _currentUser = user;

            // Загрузка справочников
            cbEquipment.ItemsSource = DB.equipmentRepairDBEntities.equipment_types.ToList();
            cbFaultType.ItemsSource = DB.equipmentRepairDBEntities.fault_types.ToList();

            // Исполнители (для менеджера и админа)
            var executors = DB.equipmentRepairDBEntities.users
                .Where(x => x.roles.name == "executor" && x.is_active == true)
                .ToList();
            cbExecutor.ItemsSource = executors;

            // Если клиент - скрываем выбор исполнителя
            if (user.roles.name == "client")
            {
                tbExecutorLabel.Visibility = Visibility.Collapsed;
                cbExecutor.Visibility = Visibility.Collapsed;
            }

            dpDeadline.SelectedDate = DateTime.Now.AddDays(3);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbEquipment.SelectedItem == null)
            {
                MessageBox.Show("Выберите оборудование!");
                return;
            }

            if (cbFaultType.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип неисправности!");
                return;
            }

            if (string.IsNullOrWhiteSpace(tbDescription.Text))
            {
                MessageBox.Show("Введите описание проблемы!");
                return;
            }

            try
            {
                // Генерация номера
                int maxNum = 0;
                var last = DB.equipmentRepairDBEntities.repair_requests
                    .OrderByDescending(x => x.id)
                    .FirstOrDefault();

                if (last != null && last.request_number.StartsWith("REQ-"))
                {
                    string numPart = last.request_number.Substring(4);
                    int.TryParse(numPart, out maxNum);
                }

                string newNumber = $"REQ-{(maxNum + 1):D4}";

                repair_requests request = new repair_requests();
                request.request_number = newNumber;
                request.created_date = DateTime.Now;
                request.equipment_type_id = (int)cbEquipment.SelectedValue;
                request.equipment_serial = tbSerial.Text;
                request.fault_type_id = (int)cbFaultType.SelectedValue;
                request.problem_description = tbDescription.Text;
                request.client_id = _currentUser.id;
                request.status = "waiting";
                request.priority = (cbPriority.SelectedItem as ComboBoxItem)?.Content.ToString();
                request.created_by_user_id = _currentUser.id;

                if (dpDeadline.SelectedDate.HasValue)
                    request.planned_deadline = dpDeadline.SelectedDate.Value;

                if (cbExecutor.SelectedValue != null && cbExecutor.Visibility == Visibility.Visible)
                    request.assigned_executor_id = (int)cbExecutor.SelectedValue;

                DB.equipmentRepairDBEntities.repair_requests.Add(request);
                DB.equipmentRepairDBEntities.SaveChanges();

                MessageBox.Show($"Заявка {newNumber} создана!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}