using CROFFLE_WPF.WPF_xamls.Windows;
using CroffleDataManager.SQLiteDB;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace CROFFLE_WPF.WPF_xamls
{
    internal class AlarmManager
    {
        SQLiteDB db;
        DispatcherTimer timer_15m;
        DispatcherTimer timer_1h;

        public AlarmManager()
        {
            db = new SQLiteDB();
        }

        public void Update()
        {
            if(timer_15m != null && timer_15m.IsEnabled) timer_15m.Stop();
            if(timer_1h != null && timer_1h.IsEnabled) timer_1h.Stop();

            Console.WriteLine("[AlarmManager] Update");

            SetAlarm(15);
            SetAlarm(60);
        }

        public void SetAlarm(int minutes)
        {
            db.GetAlarmProperty(DateTime.Now.AddMinutes(minutes + 1), DateTime.Now.AddHours(2), 0, false, out var alarm);

            if (alarm.Rows.Count == 0) { Console.WriteLine("> [AlarmManager] Next Alarm Not Found"); return; }

            Console.WriteLine("> [AlarmManager] Next Alarm Found");
            var nextAlarm = alarm.Rows[0];
            DateTime.TryParse(nextAlarm["ctime"].ToString(), out var time);

            var timer = minutes == 15 ? timer_15m : timer_1h;
            timer = new DispatcherTimer();
            var interval = time - DateTime.Now.AddMinutes(minutes);
            timer.Interval = interval;

            timer.Tick += (s, e) =>
            {
                ((DispatcherTimer)s).Stop();
                var alarmWindow = new AlarmWindow(nextAlarm["contentsID"].ToString(), minutes);
                alarmWindow.Show();
            };


            Console.WriteLine($@"> [AlarmManager] Timer Start: {timer.Interval}");
            timer.Start();
        }
    }
}
