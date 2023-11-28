using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace prodaja_kompov2
{
    public partial class Form1 : Form
    {
        private const string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\jenya\\source\\repos\\prodaja_kompov2\\prodaja_kompov2\\books.mdf;Integrated Security=True;Connect Timeout=30";
        public Form1()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT ComputerID, Brand, ProcessorType, ProcessorClockRate, RAMSize, VideoMemorySize, Price, Amount FROM Computers", connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "booksDataSet.Computers". При необходимости она может быть перемещена или удалена.
            this.computersTableAdapter.Fill(this.booksDataSet.Computers);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO Computers (Brand, ProcessorType, ProcessorClockRate, RAMSize, VideoMemorySize, Price, Amount) " +
                               "VALUES (@Brand, @ProcessorType, @ProcessorClockRate, @RAMSize, @VideoMemorySize, @Price, @Amount)";
                SqlCommand command = new SqlCommand(query, connection);

                // Здесь нужно установить значения параметров из TextBox и ComboBox
                command.Parameters.AddWithValue("@Brand", textBox1.Text);
                command.Parameters.AddWithValue("@ProcessorType", comboBox1.Text);
                command.Parameters.AddWithValue("@ProcessorClockRate", Convert.ToDouble(textBox2.Text));
                command.Parameters.AddWithValue("@RAMSize", Convert.ToInt32(textBox3.Text));
                command.Parameters.AddWithValue("@VideoMemorySize", Convert.ToInt32(textBox4.Text));
                command.Parameters.AddWithValue("@Price", Convert.ToDouble(textBox5.Text));
                command.Parameters.AddWithValue("@Amount", Convert.ToInt32(textBox6.Text));

                command.ExecuteNonQuery();
            }
            LoadData(); // Перезагрузка данных после добавления записи
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Получаем ComputerID из TextBox
                if (int.TryParse(textBox10.Text, out int computerID))
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Computers WHERE ComputerID = @ComputerID";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ComputerID", computerID);
                        command.ExecuteNonQuery();
                    }

                    LoadData(); // Перезагрузка данных после удаления записи
                }
                else
                {
                    MessageBox.Show("Пожалуйста, введите корректный ComputerID для удаления.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при удалении записи: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Computers", connection);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                DataTable changes = ((DataTable)dataGridView1.DataSource).GetChanges();
                if (changes != null)
                {
                    adapter.Update(changes);
                    ((DataTable)dataGridView1.DataSource).AcceptChanges();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Поиск данных по типу процессора и объему оперативной памяти
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Computers WHERE ProcessorType = @ProcessorType AND RAMSize = @RAMSize";
                SqlCommand command = new SqlCommand(query, connection);

                // Установка значений параметров из TextBox и ComboBox для поиска
                command.Parameters.AddWithValue("@ProcessorType", comboBox3.Text);
                command.Parameters.AddWithValue("@RAMSize", Convert.ToInt32(textBox9.Text));

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                adapter.Fill(dataTable);
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Фильтрация данных по полям Марка компьютера, Цена компьютера, Тип процессора
            DataView dataView = new DataView((DataTable)dataGridView1.DataSource);

            if (!string.IsNullOrEmpty(textBox7.Text))
                dataView.RowFilter = $"Brand LIKE '{textBox7.Text}%'";

            if (!string.IsNullOrEmpty(textBox8.Text))
                dataView.RowFilter += $" AND Price <= {Convert.ToDouble(textBox8.Text)}";

            if (!string.IsNullOrEmpty(comboBox2.Text))
                dataView.RowFilter += $" AND ProcessorType = '{comboBox2.Text}'";

            dataGridView1.DataSource = dataView.ToTable();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
