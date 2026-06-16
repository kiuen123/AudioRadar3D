using System;

namespace AudioRadarCore.Models
{
    /// <summary>
    /// Configuration for a band-pass filter associated with a specific <see cref="SoundType"/>.
    /// Defines the frequency range, detection threshold, and display color for radar blips.
    /// </summary>
    public class FilterBandConfig
    {
        /// <summary>
        /// Whether this filter band is active. Disabled filters are skipped during analysis.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Lower frequency bound in Hz for the band-pass filter.
        /// </summary>
        public float FreqLow { get; set; }

        /// <summary>
        /// Upper frequency bound in Hz for the band-pass filter.
        /// </summary>
        public float FreqHigh { get; set; }

        /// <summary>
        /// Detection threshold in the range [0.0, 1.0].
        /// Energy in this band must exceed this value to trigger a sound event.
        /// </summary>
        public float Threshold { get; set; }

        /// <summary>
        /// Display color for radar blips of this sound type, as a hex string (e.g., "#FF6633").
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Creates a new <see cref="FilterBandConfig"/> with default values.
        /// </summary>
        public FilterBandConfig()
        {
            Enabled = true;
            FreqLow = 20f;
            FreqHigh = 20000f;
            Threshold = 0.1f;
            Color = "#FFFFFF";
        }

        /// <summary>
        /// Creates a new <see cref="FilterBandConfig"/> with the specified parameters.
        /// </summary>
        /// <param name="enabled">Whether this filter is active.</param>
        /// <param name="freqLow">Lower frequency bound in Hz.</param>
        /// <param name="freqHigh">Upper frequency bound in Hz.</param>
        /// <param name="threshold">Detection threshold [0.0, 1.0].</param>
        /// <param name="color">Hex color string for display.</param>
        public FilterBandConfig(bool enabled, float freqLow, float freqHigh, float threshold, string color)
        {
            Enabled = enabled;
            FreqLow = freqLow;
            FreqHigh = freqHigh;
            Threshold = threshold;
            Color = color ?? "#FFFFFF";
        }

        /// <summary>
        /// Validates that the filter configuration has sensible values.
        /// </summary>
        /// <returns>True if the configuration is valid.</returns>
        public bool IsValid()
        {
            return FreqLow >= 0f
                && FreqHigh > FreqLow
                && Threshold >= 0f
                && Threshold <= 1f
                && !string.IsNullOrEmpty(Color);
        }

        /// <summary>
        /// Returns a human-readable summary of this filter configuration.
        /// </summary>
        public override string ToString()
        {
            return string.Format(
                "FilterBand[Enabled={0}, {1:F0}-{2:F0} Hz, Threshold={3:F2}, Color={4}]",
                Enabled, FreqLow, FreqHigh, Threshold, Color);
        }
    }
}
