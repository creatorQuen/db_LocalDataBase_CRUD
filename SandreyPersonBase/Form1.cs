using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;      // Добавляем пространство имен.



namespace SandreyPersonBase
{
    public partial class Form1 : Form
    {

        // Нужные поля для работы.

        private SqlConnection sqlConnection = null;

        private SqlCommandBuilder sqlBuilder = null;

        private SqlDataAdapter sqlDataAdapter = null;

        private DataSet dataSet = null;

        private bool newRowAdding = false;



        public Form1()
        {
            InitializeComponent();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        // Создаем метод для загрузкми данных.
        private void LoadData()
        {
            try
            {
                // Созданим экземпляр класса sqlApapter. В качестве первого параметра передадим SQL запрос, второй - экземпляр класса sqlConnection.
                // SELECT * выбираем все колонки и сущности из таблицы, 7 колонка таблицы это кнопки для управления insert или update.
                // Это для таблицы "Users".
                // Command - название ячейки.
                sqlDataAdapter = new SqlDataAdapter("SELECT *, 'Delete' AS [Команда] FROM Users", sqlConnection);

                // Создаем экземпляр класса sqlCommanBuilder
                sqlBuilder = new SqlCommandBuilder(sqlDataAdapter);

                // Создаем команды для insert, update и delete для sqlBuilder
                sqlBuilder.GetInsertCommand();
                sqlBuilder.GetUpdateCommand();
                sqlBuilder.GetDeleteCommand();

                // Создаем поле dataSet
                dataSet = new DataSet();

                // Заполняем dataSet и заполним при помощи sqlDataAdapter. + добавляем имя таблицы
                sqlDataAdapter.Fill(dataSet, "Users");

                // Устанавливаем 
                dataGridView1.DataSource = dataSet.Tables["Users"];

                // В 7 колонке содержится linkLabel(insert,delete,update) и это у меня кнопки которые должны работать, а не быть просто текстом. 
                // Поэтому в цикле переопределим 7 колонку.
                // Проходимся по количеству всех строк.
                    for (int i = 0; i < dataGridView1.Rows.Count; i++) 
                    {
                        DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                        // Обращаемся к ячейка как к элементу в двумерном массиве.
                        // 8 это девятая колонка , а row index i.
                        dataGridView1[8, i] = linkCell;

                    }


            }
            // Вслучае ошибки выводим.
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Создаем метод для перезаписывания данных.
        private void ReloadData()
        {
            try
            {

                // Очищение таблицы.
                dataSet.Tables["Users"].Clear();

                // Заполняем dataSet и заполним при помощи sqlDataAdapter. + добавляем имя таблицы.
                sqlDataAdapter.Fill(dataSet, "Users");

                // Устанавливаем.
                dataGridView1.DataSource = dataSet.Tables["Users"];

                // В 7 колонке содержится linkLabel(insert,delete,update) и это у меня кнопки которые должны работать, а не быть просто текстом .
                // Поэтому в цикле переопределим 7 колонку.
                for (int i = 0; i < dataGridView1.Rows.Count; i++) // проходимся по количеству всех строк.
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    // Обращаемся к ячейка как к элементу в двумерном массиве.
                    // 8 это девятая колонка , а row index i.
                    dataGridView1[8, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                // Вслучае ошибки выводим.
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Создаем поле для подключения базы данных (см. св-во БД). Пишем через @"".
            sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\User\source\repos\SandreyPersonBase\SandreyPersonBase\DatabaseMain.mdf;Integrated Security=True");

            // Открываем соединение с БД.
            // В этом же методе будет происходит загрузка данных в таблицу GridNew.
            sqlConnection.Open();

            LoadData();
        }

        // Это кнопка обновить
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadData();
        }

        // sell contant click и user add the draw.
        // Обрабатывается событие нажатия на ячейку и событые добавление новой строки.
        // Тут узнаем при помощи 7 колонки действие котороеб будет updadte, delete или insert.
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Определяем на какую ячейку было совершено нажатие.
                if (e.ColumnIndex == 8)
                {
                    // Получаем текст из libkLabel.
                    string task = dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString();

                    // определяем какю команду хотел выполнить пользователь.
                    if (task == "Delete")
                    {
                        // Если yes то удаляем.
                        if (MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes) 
                        {
                            int rowIndex = e.RowIndex;

                            // Вызываем метод remove at на объЕкт.
                            dataGridView1.Rows.RemoveAt(rowIndex);

                            // Удаляем строку из dataSet т.к. удали только с gridView.
                            dataSet.Tables["Users"].Rows[rowIndex].Delete();

                            // Обновляем БД.
                            sqlDataAdapter.Update(dataSet, "Users");
                        }
                    }
                    else if (task == "Insert")
                    {
                        // Новая переменная с индексом строки.
                        int rowIndex = dataGridView1.Rows.Count - 2; // - 2

                        // Создаем ссылку на новую строку  которую создадим в dataSet в таблице Users.
                        DataRow row = dataSet.Tables["Users"].NewRow();

                        row["Имя"] = dataGridView1.Rows[rowIndex].Cells["Имя"].Value;
                        row["Фамилия"] = dataGridView1.Rows[rowIndex].Cells["Фамилия"].Value;
                        row["Отчество"] = dataGridView1.Rows[rowIndex].Cells["Отчество"].Value;
                        row["День_Рождения"] = dataGridView1.Rows[rowIndex].Cells["День_Рождения"].Value;
                        row["Адресс"] = dataGridView1.Rows[rowIndex].Cells["Адресс"].Value;
                        row["Отдел"] = dataGridView1.Rows[rowIndex].Cells["Отдел"].Value;
                        row["О_себе"] = dataGridView1.Rows[rowIndex].Cells["О_себе"].Value;

                        // Выведем стрку которою только что заполнили.
                        dataSet.Tables["Users"].Rows.Add(row);
                        
                        dataSet.Tables["Users"].Rows.RemoveAt(dataSet.Tables["Users"].Rows.Count - 1); // - 1

                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2); // - 2

                        // Добовляем delete чтобы можно было использовать.
                        dataGridView1.Rows[e.RowIndex].Cells[8].Value = "Delete";

                        sqlDataAdapter.Update(dataSet, "Users");

                        newRowAdding = false;
                    }
                    else if (task == "Update")
                    {
                        // Индекс выделенной строки.
                        int r = e.RowIndex;

                        // Обновляем все данные в dataSet.
                        // Ячейкам, нужной нам строки по индексу в dataSet в талице Users присваеваем значение из ячеек редактируемой строки которая нужна.
                        dataSet.Tables["Users"].Rows[r]["Имя"] = dataGridView1.Rows[r].Cells["Имя"].Value;
                        dataSet.Tables["Users"].Rows[r]["Фамилия"] = dataGridView1.Rows[r].Cells["Фамилия"].Value;
                        dataSet.Tables["Users"].Rows[r]["Отчество"] = dataGridView1.Rows[r].Cells["Отчество"].Value;
                        dataSet.Tables["Users"].Rows[r]["День_Рождения"] = dataGridView1.Rows[r].Cells["День_Рождения"].Value;
                        dataSet.Tables["Users"].Rows[r]["Адресс"] = dataGridView1.Rows[r].Cells["Адресс"].Value;
                        dataSet.Tables["Users"].Rows[r]["Отдел"] = dataGridView1.Rows[r].Cells["Отдел"].Value;
                        dataSet.Tables["Users"].Rows[r]["О_себе"] = dataGridView1.Rows[r].Cells["О_себе"].Value;

                        sqlDataAdapter.Update(dataSet, "Users");

                        // Меняем текст последней колонки.
                        dataGridView1.Rows[e.RowIndex].Cells[8].Value = "Delete";
                    }
                }

                ReloadData();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if(newRowAdding == false)
                {
                    // Делаем так потому что строка добовляется.
                    newRowAdding = true;

                    // Чтобы строка изменялаять на insert или delete в последней строке.
                    int lastRow = dataGridView1.Rows.Count - 2; // -2

                    // Используя индекс последней строки в которую мы добавляем новую ячейку создаем класс.
                    DataGridViewRow row = dataGridView1.Rows[lastRow];

                    // Загружаем форму.
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[8, lastRow] = linkCell;

                    // Меняем кнопку вместо delete на insert.
                    row.Cells["Команда"].Value = "Insert";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Переминуем последнюю яйчейку на update.
            try
            {
                // Нужен false чтобы было понятно что строка не добавляется, а изменяется.
                if (newRowAdding == false)
                {
                    // Выводим индЕкс строки выделенной ячейки.
                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;

                    // Создаем экземпляр класса dataGridviewRow.
                    DataGridViewRow editingRow = dataGridView1.Rows[rowIndex];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[8, rowIndex] = linkCell;

                    // Меняем кнопку вместо delete на update.
                    editingRow.Cells["Команда"].Value = "Update";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошикбка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        DataTable dt = new DataTable("Users");
        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)13)
                {
                    DataView dv = dt.DefaultView;
                    dv.RowFilter = string.Format("Фамилия like '%{0}%',", txtSearch.Text);
                    dataGridView1.DataSource = dv.ToTable();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошикбка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void FindButton_Click(object sender, EventArgs e)
        {

        }
    }
}
