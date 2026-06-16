using System;
using System.Collections.Generic;

namespace AudioRadarCore.Models
{
    /// <summary>
    /// Configuration profile for a specific game or application.
    /// Contains filter band settings for each sound type, radar display parameters,
    /// camera offset, and hotkey bindings.
    /// </summary>
    public class ProfileData
    {
        /// <summary>
        /// Display name of this profile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Process name used for auto-detection (e.g., "cs2", "valorant").
        /// Matched against running process names when <see cref="AutoDetect"/> is true.
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// Whether this profile should be automatically activated when
        /// the matching <see cref="ProcessName"/> is detected as running.
        /// </summary>
        public bool AutoDetect { get; set; }

        /// <summary>
        /// Filter band configurations keyed by <see cref="SoundType"/>.
        /// Each entry defines the frequency range, threshold, and color
        /// for detecting and displaying that sound type.
        /// </summary>
        public Dictionary<SoundType, FilterBandConfig> Filters { get; set; }

        /// <summary>
        /// Radius of the radar display in Unity world units. Default is 5.0.
        /// </summary>
        public float RadarRadius { get; set; }

        /// <summary>
        /// Lifetime of a radar blip in seconds before it fades out. Default is 1.5.
        /// </summary>
        public float BlipLifetime { get; set; }

        /// <summary>
        /// Exponential smoothing factor for audio level smoothing.
        /// Range [0.0, 1.0] where lower values produce smoother (more delayed) output.
        /// Default is 0.3.
        /// </summary>
        public float SmoothingAlpha { get; set; }

        /// <summary>
        /// Whether to show the grid overlay on the radar display. Default is true.
        /// </summary>
        public bool ShowGrid { get; set; }

        /// <summary>
        /// Number of concentric grid rings on the radar display. Default is 4.
        /// </summary>
        public int GridRings { get; set; }

        /// <summary>
        /// Camera offset from the radar center in Unity coordinates [x, y, z].
        /// Default is {0, 5, -8}.
        /// </summary>
        public float[] CameraOffset { get; set; }

        /// <summary>
        /// Hotkey bindings as action-name to key-string mappings
        /// (e.g., "ToggleRadar" → "F2").
        /// </summary>
        public Dictionary<string, string> Hotkeys { get; set; }

        /// <summary>
        /// Creates a new <see cref="ProfileData"/> with sensible default values
        /// and a default set of filter bands for all sound types.
        /// </summary>
        public ProfileData()
        {
            Name = "Default";
            ProcessName = string.Empty;
            AutoDetect = false;
            RadarRadius = 5.0f;
            BlipLifetime = 1.5f;
            SmoothingAlpha = 0.3f;
            ShowGrid = true;
            GridRings = 4;
            CameraOffset = new float[] { 0f, 5f, -8f };
            Hotkeys = new Dictionary<string, string>();
            Filters = CreateDefaultFilters();
        }

        /// <summary>
        /// Creates the default set of filter band configurations for all sound types.
        /// Frequency ranges are based on typical characteristics of each sound category.
        /// </summary>
        /// <returns>A dictionary of default filter configurations keyed by sound type.</returns>
        public static Dictionary<SoundType, FilterBandConfig> CreateDefaultFilters()
        {
            return new Dictionary<SoundType, FilterBandConfig>
            {
                // Footsteps: low-mid frequency transients, typically 100-600 Hz
                {
                    SoundType.Footstep,
                    new FilterBandConfig(true, 100f, 600f, 0.15f, "#4CAF50")
                },
                // Gunshots: sharp broadband transients, strong energy 800-4000 Hz
                {
                    SoundType.Gunshot,
                    new FilterBandConfig(true, 800f, 4000f, 0.25f, "#FF5722")
                },
                // Explosions: heavy low-frequency energy, 30-300 Hz
                {
                    SoundType.Explosion,
                    new FilterBandConfig(true, 30f, 300f, 0.30f, "#FF9800")
                },
                // Voice: human speech fundamentals and formants, 300-3400 Hz
                {
                    SoundType.Voice,
                    new FilterBandConfig(true, 300f, 3400f, 0.10f, "#2196F3")
                },
                // Vehicle: engine rumble and mechanical noise, 50-800 Hz
                {
                    SoundType.Vehicle,
                    new FilterBandConfig(true, 50f, 800f, 0.20f, "#9C27B0")
                },
                // Alert: high-frequency tones and beeps, 1000-8000 Hz
                {
                    SoundType.Alert,
                    new FilterBandConfig(true, 1000f, 8000f, 0.10f, "#FFEB3B")
                }
            };
        }

        /// <summary>
        /// Validates the profile configuration for correctness.
        /// </summary>
        /// <returns>True if all configuration values are within acceptable ranges.</returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Name))
                return false;

            if (RadarRadius <= 0f)
                return false;

            if (BlipLifetime <= 0f)
                return false;

            if (SmoothingAlpha < 0f || SmoothingAlpha > 1f)
                return false;

            if (GridRings < 1)
                return false;

            if (CameraOffset == null || CameraOffset.Length != 3)
                return false;

            if (Filters == null)
                return false;

            foreach (var kvp in Filters)
            {
                if (kvp.Value != null && !kvp.Value.IsValid())
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a human-readable summary of this profile.
        /// </summary>
        public override string ToString()
        {
            int filterCount = Filters != null ? Filters.Count : 0;
            return string.Format(
                "Profile[Name={0}, Process={1}, AutoDetect={2}, Filters={3}]",
                Name, ProcessName, AutoDetect, filterCount);
        }
    }
}
