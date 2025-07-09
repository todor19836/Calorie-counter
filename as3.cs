using System;
using System.Windows.Forms;

namespace calorie_counter
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Създаване на базата и таблиците
            DatabaseHelper.CreateDatabaseAndTables();

            Application.Run(new FormMain());
        }
    }
}
