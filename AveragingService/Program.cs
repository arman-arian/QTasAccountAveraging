using System;
using System.Linq;
using AveragingService.Domain;

namespace AveragingService
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            //Application.Run(new Form1());

            var averagingService = new NewAveragingService();
            var x = averagingService.CalcAverage(13940401, 13940415, 14, 69, 1, 100, 2);
        }
    }
}
