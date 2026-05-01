using System;
using System.Windows.Forms;

namespace SecurIT_Memory
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ScoreRepository.Initialiser();
            Application.Run(new FormMenu());
        }
    }
}