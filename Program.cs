using System;
using System.Text;
using System.Timers;

namespace weight
{
    class Program
    {
        private const double Factor = 2.0 / (86400 * 7);
        private const string Ruler = "----+---190---+---200---+---210---+---220";
        private const double TargetWeight = 180;
        private static Timer _timer;
        private static DateTime _startTime;
        private static DateTime _endDate;
        private static DateTime _now = DateTime.Now;
        private static double _startWeight;
        private static string _displayProgress = "";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("weight <start time> <start weight>");
                    return;
                }
                Console.CursorVisible = false;
                _startTime = DateTime.Parse(args[0]);
                _startWeight = double.Parse(args[1]);
                _endDate = _startTime.AddSeconds((_startWeight - TargetWeight) / Factor);
                Console.Clear();
                Console.WriteLine(Ruler);
                using (_timer = new Timer(100))
                {
                    _timer.Elapsed += new ElapsedEventHandler(OnTimer);
                    _timer.AutoReset = false;
                    _timer.Enabled = true;
                    Console.ReadLine();
                    _timer.Enabled = false;
                    _timer.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.CursorVisible = true;
            }
        }

        private static int RoundUp(double t)
        {
            return (int)Math.Ceiling(t);
        }

        private static int Compare(string l, string r)
        {
            return string.Compare(l, r, StringComparison.OrdinalIgnoreCase);
        }

        private static string DisplayTime(double seconds)
        {
            //var days = (seconds / 86400);
            //seconds -= (days * 86400);
            //var hours = (seconds / 3600);
            //seconds -= (hours * 3600);
            //var minutes = (seconds / 60);
            //seconds -= (minutes * 60);
            //return string.Format("{0,0}.{1,2:00}:{2,2:00}:{3,2:00}", days, hours, minutes, seconds);
            return string.Format("{0:0.00000}", seconds/86400.0);
        }

        private static void OnTimer(object sender, ElapsedEventArgs args)
        {
            try
            {
                _timer.Enabled = false;
                _now = DateTime.Now;
                //_now = _now.AddSeconds(1200);
                var elapsed = _now.Subtract(_startTime).TotalSeconds;
                var weight = Math.Max(_startWeight - (elapsed * Factor), TargetWeight);
                var daysLeft = Math.Max(_endDate.Subtract(_now).TotalDays, 0);
                var secondsLeft = Math.Max(RoundUp(_endDate.Subtract(_now).TotalSeconds), 0);
                var displayDays = new StringBuilder();
                var displayRemain = "";
                var progress = "";
                var display = string.Format("{0:0.00}", weight);
                //var weightDisplay = string.Format("{0:0.000000}", weight);
                var weightDisplayRemain = string.Format("{0:0.00000}", weight - TargetWeight);
                if (Compare(Console.Title, display) != 0)
                    Console.Title = display;
                if (secondsLeft >= 0)
                {
                    displayRemain = "".PadRight(Math.Max(0, RoundUp(weight - TargetWeight)), '#');
                    displayDays = new StringBuilder();
                    var c = 0;
                    var t = new DateTime(_now.Year, _now.Month, _now.Day, 0, 0, 0);
                    while (t.CompareTo(_endDate) <= 0 && daysLeft > 0)
                    {
                        if (c != 0 && t.DayOfWeek == DayOfWeek.Monday) displayDays.Append(".");
                        displayDays.Append(t.ToString("MMM").ToLower()[0]);
                        t = t.AddDays(1);
                        c++;
                    }
                }
                progress = displayRemain + " " + displayDays + " " + weightDisplayRemain + " " +
                           DisplayTime(_endDate.Subtract(_now).TotalSeconds); //DisplayTime(secondsLeft);
                if (progress.Length < _displayProgress.Length)
                    progress = progress.PadRight(_displayProgress.Length, ' ');
                if (Compare(progress, _displayProgress) != 0)
                {
                    _displayProgress = progress;
                    if (_now.Second == 0)
                    {
                        Console.Clear();
                        Console.Write(Ruler);
                    }
                    Console.SetCursorPosition(0, 1);
                    while (progress.Length > 0)
                    {
                        var r = 40;
                        Console.WriteLine((progress.Length > r) ? progress.Substring(0, r) : progress);
                        progress = (progress.Length > r) ? progress.Substring(r) : "";
                    }
                }
                _timer.Enabled = (weight > TargetWeight);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
