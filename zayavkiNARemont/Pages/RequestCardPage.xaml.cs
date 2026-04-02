using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using zayavkiNARemont.Conn;

namespace zayavkiNARemont.Pages
{
    public partial class RequestCardPage : Page
    {
        private users _currentUser;
        private repair_requests _request;

        public RequestCardPage(users user, repair_requests request)
        {
            InitializeComponent();
            _currentUser = user;
            _request = request;
            LoadData();
            SetupAccess();
        }

        private void LoadData()
        {
            _request = DB.equipmentRepairDBEntities.repair_requests
                .Include("equipment_types")
                .Include("fault_types")
                .Include("users")
                .Include("users1")
                .FirstOrDefault(x => x.id == _request.id);

            tbNumber.Text = _request.request_number;
            tbDate.Text = _request.created_date?.ToString("dd.MM.yyyy HH:mm");
            tbEquipment.Text = _request.equipment_types?.name;
            tbSerial.Text = _request.equipment_serial;
            tbDescription.Text = _request.problem_description;

            // Загрузка справочников
            cbFaultType.ItemsSource = DB.equipmentRepairDBEntities.fault_types.ToList();
            cbFaultType.SelectedValue = _request.fault_type_id;

            cbExecutor.ItemsSource = DB.equipmentRepairDBEntities.users
                .Where(x => x.roles.name == "executor" && x.is_active == true)
                .ToList();
            cbExecutor.SelectedValue = _request.assigned_executor_id;

            // Статус
            foreach (ComboBoxItem item in cbStatus.Items)
            {
                if (item.Content.ToString() == _request.status)
                {
                    cbStatus.SelectedItem = item;
                    break;
                }
            }

            // Приоритет
            foreach (ComboBoxItem item in cbPriority.Items)
            {
                if (item.Content.ToString() == _request.priority)
                {
                    cbPriority.SelectedItem = item;
                    break;
                }
            }

            if (_request.planned_deadline.HasValue)
            {
                dpDeadline.SelectedDate = _request.planned_deadline.Value;
            }

            LoadComments();
        }

        private void LoadComments()
        {
            var comments = DB.equipmentRepairDBEntities.request_comments
                .Include("users")
                .Where(x => x.request_id == _request.id)
                .OrderByDescending(x => x.created_at)
                .ToList();
            lvComments.ItemsSource = comments;
        }

        private void SetupAccess()
        {
            string role = _currentUser.roles.name;

            if (role == "client")
            {
                // Клиент только смотрит
                tbSerial.IsEnabled = false;
                tbDescription.IsEnabled = false;
                cbFaultType.IsEnabled = false;
                cbStatus.IsEnabled = false;
                cbPriority.IsEnabled = false;
                cbExecutor.IsEnabled = false;
                dpDeadline.IsEnabled = false;
                btnSave.Visibility = Visibility.Collapsed;
                btnAddComment.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var req = DB.equipmentRepairDBEntities.repair_requests
                    .FirstOrDefault(x => x.id == _request.id);

                if (req != null)
                {
                    req.equipment_serial = tbSerial.Text;
                    req.problem_description = tbDescription.Text;
                    req.fault_type_id = (int)cbFaultType.SelectedValue;
                    req.status = (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
                    req.priority = (cbPriority.SelectedItem as ComboBoxItem)?.Content.ToString();

                    if (cbExecutor.SelectedValue != null)
                        req.assigned_executor_id = (int)cbExecutor.SelectedValue;

                    if (dpDeadline.SelectedDate.HasValue)
                        req.planned_deadline = dpDeadline.SelectedDate.Value;

                    if (req.status == "completed" && req.actual_completion_date == null)
                        req.actual_completion_date = DateTime.Now;

                    DB.equipmentRepairDBEntities.SaveChanges();
                    MessageBox.Show("Сохранено!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void BtnAddComment_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbNewComment.Text))
            {
                MessageBox.Show("Введите комментарий!");
                return;
            }

            request_comments comment = new request_comments();
            comment.request_id = _request.id;
            comment.user_id = _currentUser.id;
            comment.comment_text = tbNewComment.Text;
            comment.created_at = DateTime.Now;

            DB.equipmentRepairDBEntities.request_comments.Add(comment);
            DB.equipmentRepairDBEntities.SaveChanges();

            tbNewComment.Text = "";
            LoadComments();
            MessageBox.Show("Комментарий добавлен!");
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}