using System;
using System.Collections.Generic;
using System.Diagnostics;
using NAudio.CoreAudioApi;
using AudioRadarCore.Models;
using AudioRadarCore.Analyze;

namespace AudioRadarCore.Capture
{
    public class AudioSessionCaptureService : IDisposable
    {
        private MMDeviceEnumerator _enumerator;
        private MMDevice _device;
        private AudioSessionManager _sessionManager;
        private AppDirectionCalculator _directionCalc;
        
        public string TargetProcessName { get; private set; }
        public int TargetProcessId { get; private set; }
        
        public AudioSessionCaptureService()
        {
            _enumerator = new MMDeviceEnumerator();
            _directionCalc = new AppDirectionCalculator(0.3f);
        }

        public void SetDevice(string deviceId)
        {
            if (_device != null)
            {
                _device.Dispose();
                _device = null;
            }

            if (string.IsNullOrEmpty(deviceId))
            {
                _device = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
            else
            {
                _device = _enumerator.GetDevice(deviceId);
            }

            _sessionManager = _device.AudioSessionManager;
            TargetProcessId = 0;
            TargetProcessName = "None";
        }

        public List<(int ProcessId, string ProcessName)> GetActiveSessions()
        {
            var list = new List<(int, string)>();
            if (_sessionManager == null) return list;

            _sessionManager.RefreshSessions();
            for (int i = 0; i < _sessionManager.Sessions.Count; i++)
            {
                var session = _sessionManager.Sessions[i];
                int pid = (int)session.GetProcessID;
                if (pid == 0) continue;

                string pName = "Unknown";
                try
                {
                    pName = Process.GetProcessById(pid).ProcessName;
                }
                catch { }

                list.Add((pid, pName));
            }
            return list;
        }

        public void SetTargetSession(int processId, string processName)
        {
            TargetProcessId = processId;
            TargetProcessName = processName;
            _directionCalc.Reset();
        }

        public SoundFrame GetLatestFrame()
        {
            var frame = new SoundFrame
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                AppName = TargetProcessName,
                Events = Array.Empty<SoundEvent>()
            };

            if (_sessionManager == null || TargetProcessId == 0)
                return frame;

            _sessionManager.RefreshSessions();
            for (int i = 0; i < _sessionManager.Sessions.Count; i++)
            {
                var session = _sessionManager.Sessions[i];
                if ((int)session.GetProcessID == TargetProcessId)
                {
                    var meter = session.AudioMeterInformation;
                    if (meter != null)
                    {
                        var peakValues = meter.PeakValues;
                        if (peakValues.Count > 0)
                        {
                            float[] peaks = new float[peakValues.Count];
                            for (int c = 0; c < peakValues.Count; c++)
                            {
                                peaks[c] = peakValues[c];
                            }

                            var events = _directionCalc.Calculate(peaks);
                            var eventList = new List<SoundEvent>();

                            foreach (var e in events)
                            {
                                if (e.magnitude > 0.01f)
                                {
                                    eventList.Add(new SoundEvent
                                    {
                                        DirectionX = e.x,
                                        DirectionY = e.y,
                                        Intensity = e.magnitude,
                                        Distance = Math.Max(0f, 1.0f - e.magnitude)
                                    });
                                }
                            }

                            if (eventList.Count > 0)
                            {
                                frame.Events = eventList.ToArray();
                            }
                        }
                    }
                    break;
                }
            }

            return frame;
        }

        public void Dispose()
        {
            _device?.Dispose();
            _enumerator?.Dispose();
        }
    }
}
