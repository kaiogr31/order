using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class OrderEditForm : Form
    {
        private DB _db = new DB();

        // Свойства для доступа к данным формы
        public int OrderId { get; set; }
        public int UserId
        {
            get => Convert.ToInt32(comboBoxUser.SelectedValue);
            set => comboBoxUser.SelectedValue = value;
        }
        public int RoomId
        {
            get => Convert.ToInt32(comboBoxRoom.SelectedValue);
            set => comboBoxRoom.SelectedValue = value;
        }
        public DateTime EntryDate
        {
            get => dateTimePickerEntry.Value;
            set => dateTimePickerEntry.Value = value;
        }
        public DateTime DepartureDate
        {
            get => dateTimePickerDeparture.Value;
            set => dateTimePickerDeparture.Value = value;
        }

        public OrderEditForm()
        {
            InitializeComponent();
        }

        private void OrderEditForm_Load(object sender, EventArgs e)
        {
            // Загрузка списка пользователей
            comboBoxUser.DataSource = _db.GetUserList();
            comboBoxUser.DisplayMember = "name";
            comboBoxUser.ValueMember = "user_id";

            // Загрузка списка комнат
            comboBoxRoom.DataSource = _db.GetRoomTypes();
            comboBoxRoom.DisplayMember = "type_room";     // Что отображается пользователю
            comboBoxRoom.ValueMember = "information_id";  // Что используется в коде

            // Установка минимальных дат
            dateTimePickerEntry.MinDate = DateTime.Today;
            dateTimePickerDeparture.MinDate = DateTime.Today.AddDays(1);
        }

        private void dateTimePickerEntry_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerDeparture.MinDate = dateTimePickerEntry.Value.AddDays(1);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (dateTimePickerDeparture.Value <= dateTimePickerEntry.Value)
            {
                MessageBox.Show("Дата выезда должна быть позже даты заезда", "Ошибка");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
