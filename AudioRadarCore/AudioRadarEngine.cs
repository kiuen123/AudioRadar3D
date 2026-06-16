using System;
using System.Collections.Generic;
using AudioRadarCore.Capture;
using AudioRadarCore.Models;

namespace AudioRadarCore
{
    public class AudioRadarEngine : IDisposable
    {
        private AudioSessionCaptureService _captureService;
        private AudioDeviceManager _deviceManager;
        
        public bool IsRunning { get; private set; }

        public event Action<string> OnError;

        public AudioRadarEngine()
        {
            _deviceManager = new AudioDeviceManager();
            _captureService = new AudioSessionCaptureService();
        }

        public void Start()
        {
            if (IsRunning) return;

            try
            {
                // Init with default device if none selected
                if (_captureService.TargetProcessId == 0)
                {
                    _captureService.SetDevice(null);
                }
                IsRunning = true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to start: {ex.Message}");
                IsRunning = false;
            }
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public string[] GetAudioDevices()
        {
            try
            {
                var devices = _deviceManager.GetOutputDevices();
                var names = new string[devices.Count];
                for (int i = 0; i < devices.Count; i++)
                {
                    names[i] = devices[i].Name;
                }
                return names;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to enumerate devices: {ex.Message}");
                return Array.Empty<string>();
            }
        }

        public void SetAudioDevice(int deviceIndex)
        {
            try
            {
                var devices = _deviceManager.GetOutputDevices();
                if (deviceIndex >= 0 && deviceIndex < devices.Count)
                {
                    _captureService.SetDevice(devices[deviceIndex].Id);
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to set device: {ex.Message}");
            }
        }

        public List<(int ProcessId, string ProcessName)> GetActiveSessions()
        {
            try
            {
                return _captureService.GetActiveSessions();
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to get active sessions: {ex.Message}");
                return new List<(int, string)>();
            }
        }

        public void SetTargetSession(int processId, string processName)
        {
            _captureService.SetTargetSession(processId, processName);
        }

        public bool TryGetLatestFrame(out SoundFrame frame)
        {
            if (!IsRunning)
            {
                frame = null;
                return false;
            }

            try
            {
                frame = _captureService.GetLatestFrame();
                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to get frame: {ex.Message}");
                frame = null;
                return false;
            }
        }

        public void Dispose()
        {
            Stop();
            _captureService?.Dispose();
            _deviceManager?.Dispose();
        }
    }
}
