using System;
using System.Threading;
using System.Collections.Generic;
using AudioRadarCore;
using AudioRadarCore.Models;

namespace AudioRadarCore.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.CursorVisible = false;

            using var engine = new AudioRadarEngine();

            engine.OnError += error => 
            {
                // Print errors below the UI
                Console.SetCursorPosition(0, 16);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {error}".PadRight(80));
                Console.ResetColor();
            };

            var devices = engine.GetAudioDevices();
            int currentDeviceIndex = 0;
            engine.Start();

            if (!engine.IsRunning)
            {
                Console.WriteLine("Failed to start engine.");
                return;
            }

            int lastOutputTime = 0;
            List<(int pid, string name)> sessions = new List<(int, string)>();
            List<SoundEvent[]> history = new List<SoundEvent[]>();
            string[] cols = { "L", "L-F", "F", "F-R", "R", "R-B", "B", "L-B" };
            
            string currentAppName = "None";
            bool inMenu = false;

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Escape) break;
                    
                    if (key == ConsoleKey.D)
                    {
                        if (devices.Length > 0)
                        {
                            currentDeviceIndex = (currentDeviceIndex + 1) % devices.Length;
                            engine.SetAudioDevice(currentDeviceIndex);
                            engine.SetTargetSession(0, "None");
                            currentAppName = "None";
                            history.Clear();
                        }
                    }
                    else if (key == ConsoleKey.A)
                    {
                        inMenu = true;
                        Console.Clear();
                        sessions = engine.GetActiveSessions();
                        if (sessions.Count == 0)
                        {
                            Console.WriteLine("No active audio sessions found on this device.");
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            Console.WriteLine("--- Active Applications ---");
                            for (int i = 0; i < sessions.Count; i++)
                            {
                                Console.WriteLine($" [{i}] {sessions[i].name} (PID: {sessions[i].pid})");
                            }
                            Console.Write("Select App Index: ");
                            string input = Console.ReadLine();
                            if (int.TryParse(input, out int sel) && sel >= 0 && sel < sessions.Count)
                            {
                                engine.SetTargetSession(sessions[sel].pid, sessions[sel].name);
                                currentAppName = sessions[sel].name;
                                history.Clear();
                            }
                        }
                        Console.Clear();
                        inMenu = false;
                    }
                }

                if (inMenu) 
                {
                    Thread.Sleep(50);
                    continue;
                }

                if (engine.TryGetLatestFrame(out SoundFrame frame))
                {
                    if (frame.Events != null && frame.Events.Length > 0)
                    {
                        int now = Environment.TickCount;
                        if (now - lastOutputTime > 100)
                        {
                            history.Insert(0, frame.Events);
                            if (history.Count > 10) history.RemoveAt(10);
                            lastOutputTime = now;
                        }
                    }
                }

                // --- UI Rendering ---
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("================================================================================");
                Console.WriteLine($" AudioRadarCore Tracker | Device: {devices[currentDeviceIndex],-15} | App: {currentAppName,-15}");
                Console.WriteLine(" Hotkeys: [D] Change Device | [A] Select App | [ESC] Quit                       ");
                Console.WriteLine("================================================================================");
                Console.ResetColor();

                Console.WriteLine("L         L-F       F         F-R       R         R-B       B         L-B       ");
                Console.WriteLine("--------- --------- --------- --------- --------- --------- --------- --------- ");

                for (int r = 0; r < 10; r++)
                {
                    if (r < history.Count)
                    {
                        var evts = history[r];
                        
                        for (int c = 0; c < 8; c++)
                        {
                            // Find the loudest event that maps to this column
                            SoundEvent bestMatch = null;
                            foreach (var e in evts)
                            {
                                if (GetDirectionString(e.DirectionX, e.DirectionY) == cols[c])
                                {
                                    if (bestMatch == null || e.Intensity > bestMatch.Intensity)
                                        bestMatch = e;
                                }
                            }

                            if (bestMatch != null)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write($"{bestMatch.Intensity * 100,3:F0}% {bestMatch.Distance * 100,2:F0}m ");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.Write("          "); // 10 spaces
                            }
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("                                                                                ");
                    }
                }

                Thread.Sleep(33); // ~30 FPS UI refresh
            }

            engine.Stop();
            Console.CursorVisible = true;
            Console.Clear();
        }

        private static string GetDirectionString(float x, float y)
        {
            if (Math.Abs(x) < 0.01f && Math.Abs(y) < 0.01f) return "C";
            
            double angle = Math.Atan2(y, x) * (180.0 / Math.PI);
            if (angle < 0) angle += 360; 

            // Primary angles: 30 degrees span. Mixed angles: 60 degrees span.
            if (angle >= 345 || angle < 15) return "L";
            if (angle >= 15 && angle < 75) return "L-F";
            if (angle >= 75 && angle < 105) return "F";
            if (angle >= 105 && angle < 165) return "F-R";
            if (angle >= 165 && angle < 195) return "R";
            if (angle >= 195 && angle < 255) return "R-B";
            if (angle >= 255 && angle < 285) return "B";
            if (angle >= 285 && angle < 345) return "L-B";

            return "Unk";
        }
    }
}
