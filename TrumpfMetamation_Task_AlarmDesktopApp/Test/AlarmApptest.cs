using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TrumpfMetamation_Task_AlarmDesktopApp;

namespace TrumpfMetamation_Task_AlarmApp
{ 
   class AlarmAppTest
    {
        public static void Main(string[] args)
        {
            var clockMethods = new ClockMethodPage();

            var process = System.Diagnostics.Process.Start("explorer", "ms-clock:");
            if (process == null)
            {
                Console.WriteLine("Failed to open the Clock app.");
            }
            Console.WriteLine("Clock app Opened successfully.");
            Thread.Sleep(3000);

            using (var automation = new UIA3Automation())
            {
                try
                {
                    var windows = automation.GetDesktop().FindAllChildren(c => c.ByControlType(ControlType.Window));
                    var mainWindow = windows.FirstOrDefault(w => w.Name.Contains("Clock"));

                    if (mainWindow == null)
                    {
                        Console.WriteLine("Failed to find the Clock app window.");
                        return;
                    }
                    Console.WriteLine("Clock app found.");

                    var alarmTab = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("AlarmButton"))?.AsButton();
                    if (alarmTab != null)
                    {
                        alarmTab.Click();
                        Console.WriteLine("Navigated to Alarm tab.");
                     Thread.Sleep(2000);
                    }
                    List<string> expectedAlarms = new List<string>();
                    int count = 3;
                    Random rand = new Random();

                    for (int i = 0; i < count; i++)
                    {
                        var hour = rand.Next(1, 12);
                        var minute = rand.Next(0, 60);
                        string formattedTime;
                        try
                        {
                            formattedTime = ClockMethodPage.HoursMinute(hour, minute);
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            Console.WriteLine($"Invalid time: {ex.Message}");
                            continue;
                        }

                        if (clockMethods.CreateAlarm(mainWindow, hour, minute))
                        {
                            Thread.Sleep(2000);
                            expectedAlarms.Add(formattedTime);
                            Console.WriteLine($"Alarm {formattedTime} created successfully!");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to create alarm {formattedTime}.");
                        }
                    }

                    clockMethods.VerifyAlarms(mainWindow, expectedAlarms);

                    //i have tried to delete the created alarm , while automating it throws Microsoft sign in popup and need to check if it only from my laptop or is it expected.
                
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
