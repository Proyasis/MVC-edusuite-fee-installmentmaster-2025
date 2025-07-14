using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentScheduler;
using CITS.EduSuite.Business.Interfaces;


namespace CITS.EduSuite.UI.Helper
{

    public class ScheduledJobRegistry : Registry
    {
        public ScheduledJobRegistry(IInstallmentDateExtendService objinstallmentDateExtendService)
        {
            //Schedule<MyJob>()
            //        .NonReentrant() // Only one instance of the job can run at a time
            //        .ToRunOnceAt(DateTime.Now.AddSeconds(3))    // Delay startup for a while
            //        .AndEvery(2).Seconds();     // Interval

            // TODO... Add more schedules here

            // Schedule an IJob to run at an interval
            Schedule(() => new MyJob(objinstallmentDateExtendService)).ToRunNow().AndEvery(1).Days().At(23, 36);

            // Schedule an IJob to run once, delayed by a specific time interval
            //Schedule<MyJob>().ToRunOnceIn(5).Seconds();

            //// Schedule a simple job to run at a specific time
            //Schedule(() => Console.WriteLine("It's 9:15 PM now.")).ToRunEvery(1).Days().At(21, 15);

            //// Schedule a more complex action to run immediately and on an monthly interval
            //Schedule<MyJob>().ToRunNow().AndEvery(1).Months().OnTheFirst(DayOfWeek.Monday).At(3, 0);

            //// Schedule a job using a factory method and pass parameters to the constructor.
            //Schedule(() => new MyJob("Foo", DateTime.Now)).ToRunNow().AndEvery(2).Seconds();

            //// Schedule multiple jobs to be run in a single schedule
            //Schedule<MyJob>().AndThen<MyJob>().ToRunNow().AndEvery(5).Minutes();
        }
    }

    public class MyJob : IJob
    {
        private IInstallmentDateExtendService installmentDateExtendService;

        public MyJob(IInstallmentDateExtendService objinstallmentDateExtendService)
        {
            this.installmentDateExtendService = objinstallmentDateExtendService;
        }
        public void Execute()
        {
            // Execute your scheduled task here
            //Console.WriteLine("The time is {0:HH:mm:ss}", DateTime.Now);
           installmentDateExtendService.GetAllInstallmentSMSDateForSMS();

        }

    }
}