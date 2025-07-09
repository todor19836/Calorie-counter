using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace calorie_counter
{
    public class FormFoods : Form
    {
        private ListBox listFoods;
        private TextBox txtName;
        private TextBox txtCalories;
        private Button btnAddFood;
        private Label lblName;
        private Label lblCalories;

        public FormFoods()
        {
            this.Text = "Управление на храни";
            this.ClientSize = new System.Drawing.Size(400, 300);

            listFoods = new ListBox() { Left = 20, Top = 20, Width = 350, Height = 150 };

            lblName = new Label() { Left = 20, Top = 185, Text = "Име:" };
            txtName = new TextBox() { Left = 70, Top = 180, Width = 150 };

            lblCalories = new Label() { Left = 230, Top = 185, Text = "Калории:" };
            txtCalories = new TextBox() { Left = 300, Top = 180, Width = 70 };

            btnAddFood = new Button() { Left = 150, Top = 220, Width = 100, Text = "Добави" };
            btnAddFood.Click += BtnAddFood_Click;

            this.Controls.Add(listFoods);
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblCalories);
            this.Controls.Add(txtCalories);
            this.Controls.Add(btnAddFood);

            LoadFoods();
        }

        private void LoadFoods()
        {
            listFoods.Items.Clear();
            using (var connection = DatabaseHelper.GetConnection())
            {
                string sql = "SELECT Id, Name, Calories FROM Foods ORDER BY Name";
                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int calories = reader.GetInt32(2);
                        listFoods.Items.Add($"{id}. {name} - {calories} kcal");
                    }
                }
            }
        }

        private void BtnAddFood_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            if (!int.TryParse(txtCalories.Text.Trim(), out int calories) || calories < 0)
            {
                MessageBox.Show("Моля, въведете валидни калории.");
                return;
            }
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Моля, въведете име на храната.");
                return;
            }

            using (var connection = DatabaseHelper.GetConnection())
            {
                string sql = "INSERT INTO Foods (Name, Calories) VALUES (@name, @calories)";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@calories", calories);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadFoods();
            txtName.Clear();
            txtCalories.Clear();
        }
    }
}
