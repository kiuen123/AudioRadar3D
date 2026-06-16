using System.Collections.Generic;
using AudioRadarCore.Models;

namespace AudioRadarCore.Config
{
    /// <summary>
    /// Provides default profile configurations for common games and applications.
    /// These profiles are used when no user-configured profile is available.
    /// </summary>
    public static class DefaultProfiles
    {
        /// <summary>
        /// Gets the default (generic) profile suitable for most applications.
        /// </summary>
        public static ProfileData Default => new ProfileData
        {
            Name = "Default",
            ProcessName = "",
            AutoDetect = false,
            Filters = new Dictionary<SoundType, FilterBandConfig>
            {
                [SoundType.Footstep] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 200f,
                    FreqHigh = 800f,
                    Threshold = 0.15f,
                    Color = "#66FF66"
                },
                [SoundType.Gunshot] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 500f,
                    FreqHigh = 4000f,
                    Threshold = 0.40f,
                    Color = "#FF6633"
                },
                [SoundType.Explosion] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 30f,
                    FreqHigh = 300f,
                    Threshold = 0.50f,
                    Color = "#FF3300"
                },
                [SoundType.Voice] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 300f,
                    FreqHigh = 3400f,
                    Threshold = 0.20f,
                    Color = "#3399FF"
                },
                [SoundType.Vehicle] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 50f,
                    FreqHigh = 500f,
                    Threshold = 0.30f,
                    Color = "#FFCC00"
                },
                [SoundType.Alert] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 1000f,
                    FreqHigh = 5000f,
                    Threshold = 0.25f,
                    Color = "#CC66FF"
                }
            },
            RadarRadius = 5.0f,
            BlipLifetime = 1.5f,
            SmoothingAlpha = 0.3f,
            ShowGrid = true,
            GridRings = 4,
            CameraOffset = new float[] { 0f, 5f, -8f },
            Hotkeys = new Dictionary<string, string>
            {
                ["toggle"] = "F9",
                ["nextProfile"] = "F10",
                ["settings"] = "F11"
            }
        };

        /// <summary>
        /// Gets a profile optimized for Counter-Strike 2.
        /// Emphasizes footstep and gunshot detection.
        /// </summary>
        public static ProfileData CounterStrike2 => new ProfileData
        {
            Name = "Counter-Strike 2",
            ProcessName = "cs2",
            AutoDetect = true,
            Filters = new Dictionary<SoundType, FilterBandConfig>
            {
                [SoundType.Footstep] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 300f,
                    FreqHigh = 800f,
                    Threshold = 0.12f,
                    Color = "#66FF66"
                },
                [SoundType.Gunshot] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 500f,
                    FreqHigh = 5000f,
                    Threshold = 0.35f,
                    Color = "#FF6633"
                },
                [SoundType.Explosion] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 30f,
                    FreqHigh = 400f,
                    Threshold = 0.45f,
                    Color = "#FF3300"
                },
                [SoundType.Voice] = new FilterBandConfig
                {
                    Enabled = false,
                    FreqLow = 300f,
                    FreqHigh = 3400f,
                    Threshold = 0.20f,
                    Color = "#3399FF"
                },
                [SoundType.Vehicle] = new FilterBandConfig
                {
                    Enabled = false,
                    FreqLow = 50f,
                    FreqHigh = 500f,
                    Threshold = 0.30f,
                    Color = "#FFCC00"
                },
                [SoundType.Alert] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 1000f,
                    FreqHigh = 5000f,
                    Threshold = 0.30f,
                    Color = "#CC66FF"
                }
            },
            RadarRadius = 5.0f,
            BlipLifetime = 1.0f,
            SmoothingAlpha = 0.25f,
            ShowGrid = true,
            GridRings = 4,
            CameraOffset = new float[] { 0f, 5f, -8f },
            Hotkeys = new Dictionary<string, string>
            {
                ["toggle"] = "F9",
                ["nextProfile"] = "F10",
                ["settings"] = "F11"
            }
        };

        /// <summary>
        /// Gets a profile optimized for Valorant.
        /// Emphasizes footstep detection with tighter frequency range.
        /// </summary>
        public static ProfileData Valorant => new ProfileData
        {
            Name = "Valorant",
            ProcessName = "VALORANT-Win64-Shipping",
            AutoDetect = true,
            Filters = new Dictionary<SoundType, FilterBandConfig>
            {
                [SoundType.Footstep] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 250f,
                    FreqHigh = 750f,
                    Threshold = 0.10f,
                    Color = "#66FF66"
                },
                [SoundType.Gunshot] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 600f,
                    FreqHigh = 4500f,
                    Threshold = 0.35f,
                    Color = "#FF6633"
                },
                [SoundType.Explosion] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 30f,
                    FreqHigh = 350f,
                    Threshold = 0.50f,
                    Color = "#FF3300"
                },
                [SoundType.Voice] = new FilterBandConfig
                {
                    Enabled = false,
                    FreqLow = 300f,
                    FreqHigh = 3400f,
                    Threshold = 0.20f,
                    Color = "#3399FF"
                },
                [SoundType.Vehicle] = new FilterBandConfig
                {
                    Enabled = false,
                    FreqLow = 50f,
                    FreqHigh = 500f,
                    Threshold = 0.30f,
                    Color = "#FFCC00"
                },
                [SoundType.Alert] = new FilterBandConfig
                {
                    Enabled = true,
                    FreqLow = 1200f,
                    FreqHigh = 4000f,
                    Threshold = 0.25f,
                    Color = "#CC66FF"
                }
            },
            RadarRadius = 5.0f,
            BlipLifetime = 1.2f,
            SmoothingAlpha = 0.28f,
            ShowGrid = true,
            GridRings = 4,
            CameraOffset = new float[] { 0f, 5f, -8f },
            Hotkeys = new Dictionary<string, string>
            {
                ["toggle"] = "F9",
                ["nextProfile"] = "F10",
                ["settings"] = "F11"
            }
        };

        /// <summary>
        /// Gets all built-in profiles.
        /// </summary>
        /// <returns>A list of all default profiles.</returns>
        public static List<ProfileData> GetAll()
        {
            return new List<ProfileData>
            {
                Default,
                CounterStrike2,
                Valorant
            };
        }

        /// <summary>
        /// Finds a profile matching the given process name.
        /// </summary>
        /// <param name="processName">The process name to match (case-insensitive).</param>
        /// <returns>The matching profile, or null if not found.</returns>
        public static ProfileData FindByProcess(string processName)
        {
            if (string.IsNullOrEmpty(processName))
                return null;

            var lowerName = processName.ToLowerInvariant();
            foreach (var profile in GetAll())
            {
                if (profile.AutoDetect &&
                    !string.IsNullOrEmpty(profile.ProcessName) &&
                    profile.ProcessName.ToLowerInvariant() == lowerName)
                {
                    return profile;
                }
            }

            return null;
        }
    }
}
