using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace WindowsFormsApp1
{
    public class DB
    {
        private readonly NpgsqlConnection _connection;

        // Параметры подключения (можно вынести в конфиг)
        private const string Host = "172.20.7.53";
        private const int Port = 5432;
        private const string Database = "db3996_13";
        private const string Username = "st3996";
        private const string Password = "pwd3996";
        private const string Schema = "hotel";

        public DB()
        {
            var connectionString = $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};SearchPath={Schema};SslMode=Disable";
            _connection = new NpgsqlConnection(connectionString);
        }


        public void OpenConnection()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
                using (var cmd = new NpgsqlCommand($"SET search_path TO {Schema}", _connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void closecon()
        {
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        private DataTable ExecuteQuery(string query, NpgsqlParameter[] parameters = null)
        {
            var dt = new DataTable();

            try
            {
                OpenConnection();
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (var da = new NpgsqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка выполнения запроса: " + ex.Message);
            }

            return dt;
        }

        public DataTable GetOverdueOrders()
        {
            var dt = new DataTable();

            string query = @"
        SELECT 
            o.order_id,
            u.user_id,
            o.entry_date,
            o.departure_date,
            o.room_id,
            (o.departure_date - o.entry_date) AS nights_count
        FROM hotel.orders o
        JOIN hotel.users u ON o.user_id = u.user_id
        JOIN hotel.rooms r ON o.room_id = r.room_id
        WHERE o.departure_date < CURRENT_DATE
        ORDER BY o.order_id;
    ";

            try
            {
                OpenConnection();
                using (var cmd = new NpgsqlCommand(query, _connection))
                using (var da = new NpgsqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке просроченных заказов: " + ex.Message);
            }

            return dt;
        }

        // Удалить заказ
        public void DeleteOrder(int orderId)
        {
            try
            {
                OpenConnection();
                using (var cmd = new NpgsqlCommand("DELETE FROM hotel.orders WHERE order_id = @id", _connection))
                {
                    cmd.Parameters.AddWithValue("@id", orderId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления заказа: " + ex.Message);
            }
        }

        // Добавить заказ
        public void AddOrder(int userId, int roomId, DateTime entry, DateTime departure)
        {
            try
            {
                OpenConnection();
                string sql = "INSERT INTO hotel.orders (user_id, room_id, entry_date, departure_date) VALUES (@u, @r, @e, @d)";
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand(sql, _connection);
                using (var cmd = npgsqlCommand)
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.Parameters.AddWithValue("@r", roomId);
                    cmd.Parameters.AddWithValue("@e", entry);
                    cmd.Parameters.AddWithValue("@d", departure);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        // Обновить заказ
        public void UpdateOrder(int orderId, int userId, int roomId, DateTime entry, DateTime departure)
        {
            try
            {
                OpenConnection();
                string sql = @"UPDATE hotel.orders 
                       SET user_id = @u, room_id = @r, entry_date = @e, departure_date = @d 
                       WHERE order_id = @id";
                using (var cmd = new NpgsqlCommand(sql, _connection))
                {
                    cmd.Parameters.AddWithValue("@id", orderId);
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.Parameters.AddWithValue("@r", roomId);
                    cmd.Parameters.AddWithValue("@e", entry);
                    cmd.Parameters.AddWithValue("@d", departure);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления: " + ex.Message);
            }
        }

        public DataTable GetOrders()
        {
            string query = @"SELECT * FROM hotel.orders ORDER BY entry_date DESC";
            return ExecuteQuery(query);
        }


        public DataTable GetUserList()
        {
            string query = @"SELECT user_id, name || ' ' || last_name AS full_name 
                        FROM hotel.users ORDER BY last_name";
            return ExecuteQuery(query);
        }

        public DataTable GetRoomTypes()
        {
            string query = "SELECT information_id, type_room FROM hotel.informations ORDER BY type_room";
            return ExecuteQuery(query);
        }

    }
}

    