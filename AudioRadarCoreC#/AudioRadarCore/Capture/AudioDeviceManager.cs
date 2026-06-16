using System;
using System.Collections.Generic;
using NAudio.CoreAudioApi;

namespace AudioRadarCore.Capture
{
    /// <summary>
    /// Provides audio device enumeration and discovery using the Windows
    /// Core Audio API (via NAudio's <see cref="MMDeviceEnumerator"/>).
    /// </summary>
    public class AudioDeviceManager : IDisposable
    {
        private MMDeviceEnumerator _enumerator;
        private volatile bool _disposed;

        /// <summary>
        /// Initializes a new instance of <see cref="AudioDeviceManager"/>.
        /// </summary>
        public AudioDeviceManager()
        {
            _enumerator = new MMDeviceEnumerator();
        }

        /// <summary>
        /// Retrieves a list of all active audio output (render) devices.
        /// </summary>
        /// <returns>
        /// A list of tuples where each entry contains the device ID and friendly name.
        /// Returns an empty list if no audio devices are available.
        /// </returns>
        public List<AudioDeviceInfo> GetOutputDevices()
        {
            var devices = new List<AudioDeviceInfo>();

            if (_disposed)
                return devices;

            try
            {
                var endpoints = _enumerator.EnumerateAudioEndPoints(
                    DataFlow.Render, DeviceState.Active);

                foreach (var device in endpoints)
                {
                    try
                    {
                        devices.Add(new AudioDeviceInfo(
                            device.ID,
                            device.FriendlyName,
                            device.AudioClient.MixFormat.Channels,
                            device.AudioClient.MixFormat.SampleRate));
                    }
                    catch (Exception)
                    {
                        // Skip devices that fail to enumerate properties.
                        // This can happen with certain virtual or disconnecting devices.
                        try
                        {
                            devices.Add(new AudioDeviceInfo(
                                device.ID,
                                device.FriendlyName,
                                0,
                                0));
                        }
                        catch
                        {
                            // Completely inaccessible device, skip entirely
                        }
                    }
                }
            }
            catch (Exception)
            {
                // No audio subsystem available or COM error
            }

            return devices;
        }

        /// <summary>
        /// Gets information about the default audio render (output) endpoint.
        /// </summary>
        /// <returns>
        /// An <see cref="AudioDeviceInfo"/> for the default render device,
        /// or null if no default device is available.
        /// </returns>
        public AudioDeviceInfo GetDefaultDevice()
        {
            if (_disposed)
                return null;

            try
            {
                var device = _enumerator.GetDefaultAudioEndpoint(
                    DataFlow.Render, Role.Multimedia);

                if (device == null)
                    return null;

                try
                {
                    return new AudioDeviceInfo(
                        device.ID,
                        device.FriendlyName,
                        device.AudioClient.MixFormat.Channels,
                        device.AudioClient.MixFormat.SampleRate);
                }
                catch (Exception)
                {
                    return new AudioDeviceInfo(
                        device.ID,
                        device.FriendlyName,
                        0,
                        0);
                }
            }
            catch (Exception)
            {
                // No default device available (e.g., all devices removed)
                return null;
            }
        }

        /// <summary>
        /// Checks whether any active audio output device is available on the system.
        /// </summary>
        /// <returns>True if at least one active render device exists.</returns>
        public bool HasOutputDevice()
        {
            if (_disposed)
                return false;

            try
            {
                var endpoints = _enumerator.EnumerateAudioEndPoints(
                    DataFlow.Render, DeviceState.Active);

                // NAudio returns a collection; check if it has any elements
                foreach (var _ in endpoints)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                // COM or system error
            }

            return false;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="AudioDeviceManager"/>.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_enumerator != null)
            {
                try
                {
                    _enumerator.Dispose();
                }
                catch
                {
                    // Best-effort cleanup
                }
                finally
                {
                    _enumerator = null;
                }
            }
        }
    }

    /// <summary>
    /// Describes an audio output device with its identification and capabilities.
    /// </summary>
    public class AudioDeviceInfo
    {
        /// <summary>
        /// Unique system identifier for this audio endpoint.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Human-readable name of the audio device (e.g., "Speakers (Realtek High Definition Audio)").
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Number of audio channels supported by this device's mix format.
        /// 0 if the information could not be retrieved.
        /// </summary>
        public int Channels { get; }

        /// <summary>
        /// Sample rate in Hz of this device's mix format.
        /// 0 if the information could not be retrieved.
        /// </summary>
        public int SampleRate { get; }

        /// <summary>
        /// Creates a new <see cref="AudioDeviceInfo"/>.
        /// </summary>
        /// <param name="id">System device identifier.</param>
        /// <param name="name">Friendly device name.</param>
        /// <param name="channels">Number of channels.</param>
        /// <param name="sampleRate">Sample rate in Hz.</param>
        public AudioDeviceInfo(string id, string name, int channels, int sampleRate)
        {
            Id = id ?? string.Empty;
            Name = name ?? string.Empty;
            Channels = channels;
            SampleRate = sampleRate;
        }

        /// <summary>
        /// Returns the friendly name of this audio device.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} ({1}ch, {2}Hz)", Name, Channels, SampleRate);
        }
    }
}
