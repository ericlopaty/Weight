﻿using System;
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
		private static DateTime now;
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
				startTime = DateTime.Parse(args[0]);
				startWeight = double.Parse(args[1]);
				targetWeight = double.Parse(args[2]);
				endDate = startTime.AddSeconds((startWeight - 190.0) / factor);
				using (timer = new Timer(1000))
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
			if (d > 0) return string.Format("{0:0} {1,2:00}:{2,2:00}:{3,2:00}", d, h, m, s);
			if (h > 0) return string.Format("{0:0}:{1,2:00}:{2,2:00}", h, m, s);
			if (m > 0) return string.Format("{0:0}:{1,2:00}", m, s);
			if (s > 0) return string.Format("{0:0}", s);
			return "";
		}

		private static void UpdateTitle(double weight)
		{
			string display = string.Format("{0:0.0}", weight);
			if (string.Compare(Console.Title, display) != 0)
				Console.Title = display;
		}

		private static void UpdateProgress(string progress)
		{
			if (string.Compare(progress, displayProgress) != 0)
			{
				Console.Clear();
				Console.WriteLine(progress);
				displayProgress = progress;
			}
		}

		private static void OnTimer(object sender, ElapsedEventArgs args)
		{
			try
			{
				timer.Enabled = false;
				now = DateTime.Now;
				double elapsed = now.Subtract(startTime).TotalSeconds;
				double weight = Math.Max(startWeight - (elapsed * factor), targetWeight);
				double remain = weight - targetWeight < 0 ? 0 : weight - targetWeight;
				double daysLeft = Math.Max(endDate.Subtract(now).TotalDays, 0);
				int secondsLeft = Math.Max(RoundUp(endDate.Subtract(now).TotalSeconds), 0);
				UpdateTitle(weight);
				StringBuilder progress = new StringBuilder();
				if (weight > targetWeight)
				{
					progress.Append(ruler);
					progress.Append("".PadRight((int)Math.Max(0, RoundUp(weight - targetWeight)), '#') + "\n");
					int c = 0;
					int wrap=0;
					DateTime t = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
					while (t.CompareTo(endDate) <= 0 && daysLeft > 0)
					{
						if (c != 0 && t.DayOfWeek == DayOfWeek.Monday) 
						{
							progress.Append(".");
							wrap++;
						}
						progress.Append(t.ToString("MMM").ToLower()[0]);
						t = t.AddDays(1);
						c++;
						wrap++;
						if (wrap % 70 == 0)
							progress.Append("\n");
					}
					//progress.Append(string.Format("\n{0:0.0000}\n{1:#,###} ({2})", remain, secondsLeft, FormatSeconds(secondsLeft)));
					progress.Append(string.Format("\n{0:0.0000}\n{1:#,###}", remain, secondsLeft));
				}
				UpdateProgress(progress.ToString());
				timer.Enabled = (weight > targetWeight);
			}
			catch
			{
			}
		}
	}
}
