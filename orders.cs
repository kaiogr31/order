using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsApp1.DB;

namespace WindowsFormsApp1
{
    public partial class orders : Form
    {
        private DB _db = new DB();
 

        public orders()
        {
            InitializeComponent();
        }


        private void LoadOrders()
        {
            try
            {
                DataTable dt = checkBoxOverdue.Checked ? _db.GetOverdueOrders() : _db.GetOverdueOrders();
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке заказов: " + ex.Message);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var f = new OrderEditForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                _db.AddOrder(f.UserId, f.RoomId, f.EntryDate, f.DepartureDate);
                LoadOrders();
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
            var row = dataGridView1.SelectedRows[0];

            var f = new OrderEditForm
            {
                OrderId = Convert.ToInt32(row.Cells["order_id"].Value),
                UserId = Convert.ToInt32(row.Cells["user_id"].Value),
                RoomId = Convert.ToInt32(row.Cells["room_id"].Value),
                EntryDate = Convert.ToDateTime(row.Cells["entry_date"].Value),
                DepartureDate = Convert.ToDateTime(row.Cells["departure_date"].Value)
            };

            if (f.ShowDialog() == DialogResult.OK)
            {
                _db.UpdateOrder(f.OrderId, f.UserId, f.RoomId, f.EntryDate, f.DepartureDate);
                LoadOrders();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
            var row = dataGridView1.SelectedRows[0];
            int orderId = Convert.ToInt32(row.Cells["order_id"].Value);

            var confirm = MessageBox.Show("Удалить заказ?", "Подтверждение", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                _db.DeleteOrder(orderId);
                LoadOrders();
            }
        }

        private void checkBoxOverdue_CheckedChanged(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void orders_Load(object sender, EventArgs e)
        {
            _db = new DB();
            LoadOrders();
        }

        private void checkBoxOverdue_CheckedChanged_1(object sender, EventArgs e)
        {

        }
    }
}
