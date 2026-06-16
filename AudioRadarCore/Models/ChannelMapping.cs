using System;

namespace AudioRadarCore.Models
{
    public static class ChannelMapping
    {
        public const int ChannelCount = 8;

        public static readonly float[] ChannelAnglesDegrees = new float[]
        {
            -30f,   // FL
             30f,   // FR
              0f,   // FC
              0f,   // LFE
            -150f,  // BL
             150f,  // BR
            -90f,   // SL
             90f    // SR
        };

        // Coordinate system: X = Left (+), Right (-). Y = Front (+), Back (-)
        public static readonly float[] ChannelVectorsX;
        public static readonly float[] ChannelVectorsY;
        public static readonly bool[] IsDirectional;

        static ChannelMapping()
        {
            ChannelVectorsX = new float[ChannelCount];
            ChannelVectorsY = new float[ChannelCount];
            IsDirectional = new bool[ChannelCount];

            for (int i = 0; i < ChannelCount; i++)
            {
                if (i == 3) // LFE
                {
                    IsDirectional[i] = false;
                    continue;
                }

                IsDirectional[i] = true;
                float angleRad = ChannelAnglesDegrees[i] * (float)(Math.PI / 180.0);
                
                // X = Left (+), Right (-). This means X = -sin(angle)
                ChannelVectorsX[i] = (float)-Math.Sin(angleRad);
                // Y = Front (+), Back (-). This means Y = cos(angle)
                ChannelVectorsY[i] = (float)Math.Cos(angleRad);
            }
        }
    }
}
