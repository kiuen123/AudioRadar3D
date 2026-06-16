using System;

namespace AudioRadarCore.Models
{
    public class SoundFrame
    {
        public long Timestamp { get; set; }
        public string AppName { get; set; }
        public SoundEvent[] Events { get; set; }
    }
}
