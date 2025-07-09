using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace calorie_counter
{
    public class FormDailySummary : Form
    {
        private DateTimePicker dateTimePicker;
        private Label lblDate;
        private Label lblTotalCalories;
        private Button btnShow;

        public FormDailySummary()
        {
            this.Text = "Дневен отчет";
            this.ClientSize = new System.Drawing.Size(300, 150);

            lblDate = new Label() { Left = 20, Top = 20, Text = "Избери дата:", AutoSize = true };
            dateTimePicker = new DateTimePicker() { Left = 100, Top = 15, Width = 150, Format = DateTimePickerFormat.Short };

            btnShow = new Button() { Left = 100, Top = 50, Width = 100, Text = "Покажи" };
            btnShow.Click += BtnShow_Click;

            lblTotalCalories = new Label() { Left = 20, Top = 90, Width = 250, Text = "Общо калории: 0", AutoSize = true };

            this.Controls.Add(lblDate);
            this.Controls.Add(dateTimePicker);
            this.Controls.Add(btnShow);
            this.Controls.Add(lblTotalCalories);
        }

        private void BtnShow_Click(object sender, EventArgs e)
        {
            string selectedDate = dateTimePicker.Value.ToString("yyyy-MM-dd");

            using (var connection = DatabaseHelper.GetConnection())
            {
                string sql = "SELECT SUM(Calories) FROM Meals WHERE Date = @date";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@date", selectedDate);
                    object result = cmd.ExecuteScalar();
                    int totalCalories = 0;

                    if (result != DBNull.Value && result != null)
                    {
                        totalCalories = Convert.ToInt32(result);
                    }

                    lblTotalCalories.Text = $"Общо калории: {totalCalories}";
                }
            }
        }
    }
}
