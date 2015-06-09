using System;
using System.Text;
using System.Timers;

namespace weight
{
	class Program
	{
		private static Timer timer;
		private static DateTime startTime;
		private static DateTime endDate;
		private static double factor = 2.0 / (86400 * 7);
		private static string ruler = "----+---200---+---210---+---220---+---230---+---240---+---250---+---260\n";
		private static DateTime now = DateTime.Now;
		//private static int delta = 86400;
		private static double startWeight;
		private static double targetWeight;
		private static string displayProgress = "";

		static void Main(string[] args)
		{
			try
			{
				if (args.Length != 3)
				{
					Console.WriteLine("weight <start time> <start weight> <target weight>");
					return;
				}
				Console.CursorVisible = false;
				startTime = DateTime.Parse(args[0]);
				startWeight = double.Parse(args[1]);
				targetWeight = double.Parse(args[2]);
				endDate = startTime.AddSeconds((startWeight - 190.0) / factor);
				Console.Clear();
				Console.WriteLine(ruler);
				using (timer = new Timer(100))
				{
					timer.Elapsed += new ElapsedEventHandler(OnTimer);
					timer.AutoReset = false;
					timer.Enabled = true;
					Console.ReadLine();
					timer.Enabled = false;
					timer.Close();
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

		private static string FormatSeconds(int seconds)
		{
			int d = (seconds / 86400);
			seconds -= (d * 86400);
			int h = seconds / 3600;
			seconds -= (h * 3600);
			int m = seconds / 60;
			seconds -= (m * 60);
			int s = seconds;
			if (d > 0) return string.Format("{0:0}:{1,2:00}:{2,2:00}:{3,2:00}", d, h, m, s);
			if (h > 0) return string.Format("{0:0}:{1,2:00}:{2,2:00}", h, m, s);
			if (m > 0) return string.Format("{0:0}:{1,2:00}", m, s);
			if (s > 0) return string.Format("{0:0}", s);
			return "";
		}

		private static void OnTimer(object sender, ElapsedEventArgs args)
		{
			try
			{
				timer.Enabled = false;
				now = DateTime.Now;
				//now = now.AddSeconds(delta);
				double elapsed = now.Subtract(startTime).TotalSeconds;
				double weight = Math.Max(startWeight - (elapsed * factor), targetWeight);
				double remain = weight - targetWeight < 0 ? 0 : weight - targetWeight;
				double daysLeft = Math.Max(endDate.Subtract(now).TotalDays, 0);
				int secondsLeft = Math.Max(RoundUp(endDate.Subtract(now).TotalSeconds), 0);
				//if (secondsLeft <= 864000) delta = 3600;
				//if (secondsLeft <= 86400) delta = 60;
				//if (secondsLeft <= 3600) delta = 1;
				StringBuilder displayDays = new StringBuilder();
				string displayRemain = "";
				string displayWeight = "";
				string displayTime = "";
				string displayClock = "";
				string progress = "";
				string display = string.Format("{0:0.00000}", weight);
				if (string.Compare(Console.Title, display) != 0)
					Console.Title = display;
				if (secondsLeft >= 0)
				{
					displayRemain = "".PadRight((int)Math.Max(0, RoundUp(weight - targetWeight)), '#');
					displayDays = new StringBuilder();
					int c = 0;
					DateTime t = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
					while (t.CompareTo(endDate) <= 0 && daysLeft > 0)
					{
						if (c != 0 && t.DayOfWeek == DayOfWeek.Monday) displayDays.Append(".");
						displayDays.Append(t.ToString("MMM").ToLower()[0]);
						t = t.AddDays(1);
						c++;
					}
					displayWeight = string.Format("{0:0.00000}", remain);
					displayTime = string.Format("{0:#,##0}", secondsLeft);
					displayClock = FormatSeconds(secondsLeft);
				}
				progress = displayRemain + " " + displayDays + " " + displayWeight + " " + displayClock;
				if (progress.Length < displayProgress.Length)
					progress = progress.PadRight(displayProgress.Length, ' ');
				if (string.Compare(progress, displayProgress) != 0)
				{
					displayProgress = progress;
					if (now.Second == 0)
					{
						Console.Clear();
						Console.Write(ruler);
					}
					Console.SetCursorPosition(0, 1);
					while (progress.Length > 0)
					{
						Console.WriteLine((progress.Length > 70) ? progress.Substring(0, 70) : progress);
						progress = (progress.Length > 70) ? progress.Substring(70) : "";
					}
					Console.WriteLine(progress);
				}
				timer.Enabled = (weight > targetWeight);
			}
			catch
			{
			}
		}
	}
}
