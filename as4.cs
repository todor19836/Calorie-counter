using System;
using System.Data.SQLite;

namespace calorie_counter
{
    public static class DatabaseHelper
    {
        private static string connectionString = "Data Source=calorie_counter.db;Version=3;";

        public static void CreateDatabaseAndTables()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createFoods = @"
                    CREATE TABLE IF NOT EXISTS Foods (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Calories INTEGER NOT NULL
                    );";

                string createMeals = @"
                    CREATE TABLE IF NOT EXISTS Meals (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FoodId INTEGER,
                        Date TEXT NOT NULL,
                        Quantity REAL NOT NULL,
                        Calories INTEGER NOT NULL,
                        FOREIGN KEY(FoodId) REFERENCES Foods(Id)
                    );";

                using (var cmd = new SQLiteCommand(createFoods, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SQLiteCommand(createMeals, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static SQLiteConnection GetConnection()
        {
            var connection = new SQLiteConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
