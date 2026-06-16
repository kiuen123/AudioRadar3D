namespace AudioRadarCore.Models
{
    /// <summary>
    /// Classifiable sound categories detected by the audio analysis pipeline.
    /// </summary>
    public enum SoundType
    {
        /// <summary>Unknown or unclassified sound.</summary>
        Unknown = 0,

        /// <summary>Footstep sounds (walking, running, crouching).</summary>
        Footstep = 1,

        /// <summary>Gunshot or weapon fire sounds.</summary>
        Gunshot = 2,

        /// <summary>Explosion or large detonation sounds.</summary>
        Explosion = 3,

        /// <summary>Human voice or speech sounds.</summary>
        Voice = 4,

        /// <summary>Vehicle engine or movement sounds.</summary>
        Vehicle = 5,

        /// <summary>Alert or notification sounds (UI beeps, alarms).</summary>
        Alert = 6
    }
}
