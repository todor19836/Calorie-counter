using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace calorie_counter
{
    public class FormAddMeal : Form
    {
        private ComboBox comboBoxFoods;
        private TextBox txtQuantity;
        private Label lblFood;
        private Label lblQuantity;
        private DateTimePicker dateTimePickerMealDate;
        private Label lblDate;
        private Button btnAddMeal;

        public FormAddMeal()
        {
            InitializeMyComponents();
            LoadFoods();
        }

        private void InitializeMyComponents()
        {
            this.Text = "Добавяне на хранене";
            this.ClientSize = new System.Drawing.Size(350, 200);

            lblFood = new Label() { Left = 20, Top = 23, Text = "Храна:", AutoSize = true };
            comboBoxFoods = new ComboBox() { Left = 100, Top = 20, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            lblQuantity = new Label() { Left = 20, Top = 63, Text = "Количество:", AutoSize = true };
            txtQuantity = new TextBox() { Left = 100, Top = 60, Width = 100, Text = "1" };

            lblDate = new Label() { Left = 20, Top = 103, Text = "Дата:", AutoSize = true };
            dateTimePickerMealDate = new DateTimePicker() { Left = 100, Top = 100, Width = 200, Format = DateTimePickerFormat.Short };

            btnAddMeal = new Button() { Left = 100, Top = 140, Width = 100, Text = "Добави" };
            btnAddMeal.Click += BtnAddMeal_Click;

            this.Controls.Add(lblFood);
            this.Controls.Add(comboBoxFoods);
            this.Controls.Add(lblQuantity);
            this.Controls.Add(txtQuantity);
            this.Controls.Add(lblDate);
            this.Controls.Add(dateTimePickerMealDate);
            this.Controls.Add(btnAddMeal);
        }

        private void LoadFoods()
        {
            comboBoxFoods.Items.Clear();

            string connectionString = "Data Source=calorie_counter.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT Id, Name FROM Foods ORDER BY Name";

                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Запазваме в comboBox обект с ID и име
                        comboBoxFoods.Items.Add(new FoodItem
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }

            if (comboBoxFoods.Items.Count > 0)
                comboBoxFoods.SelectedIndex = 0;
        }

        private void BtnAddMeal_Click(object sender, EventArgs e)
        {
            if (comboBoxFoods.SelectedItem == null)
            {
                MessageBox.Show("Моля, изберете храна.");
                return;
            }

            if (!double.TryParse(txtQuantity.Text, out double quantity) || quantity <= 0)
            {
                MessageBox.Show("Моля, въведете валидно количество.");
                return;
            }

            var selectedFood = (FoodItem)comboBoxFoods.SelectedItem;
            DateTime date = dateTimePickerMealDate.Value.Date;

            string connectionString = "Data Source=calorie_counter.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Взимаме калориите на храната
                string sqlGetCalories = "SELECT Calories FROM Foods WHERE Id = @foodId";
                int caloriesPerUnit = 0;
                using (var cmd = new SQLiteCommand(sqlGetCalories, connection))
                {
                    cmd.Parameters.AddWithValue("@foodId", selectedFood.Id);
                    caloriesPerUnit = Convert.ToInt32(cmd.ExecuteScalar());
                }

                int totalCalories = (int)(caloriesPerUnit * quantity);

                // Добавяме ястието
                string sqlInsertMeal = @"INSERT INTO Meals (FoodId, Date, Quantity, Calories)
                                         VALUES (@foodId, @date, @quantity, @calories)";
                using (var cmd = new SQLiteCommand(sqlInsertMeal, connection))
                {
                    cmd.Parameters.AddWithValue("@foodId", selectedFood.Id);
                    cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@calories", totalCalories);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Храненето е добавено успешно!");
            this.Close();
        }

        // Клас за елементите на ComboBox (за да пазим Id и Name)
        private class FoodItem
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
