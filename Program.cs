using System;
using System.Windows.Forms;

namespace StrategyGame
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new RaceSelectionForm());
        }
    }
}
