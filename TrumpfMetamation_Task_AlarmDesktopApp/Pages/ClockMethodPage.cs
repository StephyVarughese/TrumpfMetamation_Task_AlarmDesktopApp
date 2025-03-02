using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using System;
using System.Linq;
using System.Threading;
using FlaUI.Core.Definitions;
using System.Collections.Generic;

namespace TrumpfMetamation_Task_AlarmDesktopApp
{
    class ClockMethodPage
    {
        // Method to validate and format time
        public static string HoursMinute(int hours, int minutes)
        {
            if (hours < 0 || hours > 23)
                throw new ArgumentOutOfRangeException(nameof(hours), "Hours must be between 0 and 23.");
            if (minutes < 0 || minutes > 59)
                throw new ArgumentOutOfRangeException(nameof(minutes), "Minutes must be between 0 and 59.");
            return $"{hours:00}:{minutes:00}";
        }

        // Method to create an alarm
        public bool CreateAlarm(AutomationElement mainWindow, int hour, int minute)
        {
            string formattedTime;
            try
            {
                // Validate and format time
                formattedTime = HoursMinute(hour, minute);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Invalid time: {ex.Message}");
                return false;
            }

            var addAlarmButton = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("AddAlarmButton"))?.AsButton();
            if (addAlarmButton == null)
            {
                Console.WriteLine("Add Alarm button not found.");
                return false;
            }
            addAlarmButton.Click();
            //Thread.Sleep(2000);

            var hourPicker = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("HourPicker"))?.AsTextBox();
            var minutePicker = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("MinutePicker"))?.AsTextBox();

            if (hourPicker != null && minutePicker != null)
            {
                var timeParts = formattedTime.Split(':');
                hourPicker.Text = timeParts[0];
                minutePicker.Text = timeParts[1];
            }
            else
            {
                Console.WriteLine("Hour or Minute picker not found.");
                return false;
            }

            var alarmName = mainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit))?.AsTextBox();
            if (alarmName != null)
            {
                alarmName.Text = "Trumpf Metamation - Login Time";
            }
            else
            {
                Console.WriteLine("Alarm name textbox not found.");
                return false;
            }

            var primaryButton = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PrimaryButton"))?.AsButton();
            if (primaryButton != null)
            {
                primaryButton.Invoke();
                //Thread.Sleep(1000);
            }
            else
            {
                Console.WriteLine("Primary button not found.");
                return false;
            }

            return true;
        }

        public void VerifyAlarms(AutomationElement mainWindow, List<string> expectedAlarms)
        {
           // Thread.Sleep(5000);
            var alarmTimes = mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Text))
                .Select(a => a.Properties.Name.ValueOrDefault?.Trim())
                .Where(text => !string.IsNullOrEmpty(text))
                .ToList();

            var adjustedExpectedAlarms = expectedAlarms.Select(a => a.TrimStart('0'));
            foreach (var expectedAlarm in adjustedExpectedAlarms)
                Console.WriteLine(alarmTimes.Any(t => t.TrimStart('0') == expectedAlarm)
                    ? $"Alarm verified: {expectedAlarm} found."
                    : $"Alarm verification failed: {expectedAlarm} not found.");
        }


    }
}
