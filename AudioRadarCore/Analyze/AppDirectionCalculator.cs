using System;
using System.Collections.Generic;
using AudioRadarCore.Models;

namespace AudioRadarCore.Analyze
{
    public class AppDirectionCalculator
    {
        private float _smoothingAlpha;
        private float[] _smoothedX = new float[8];
        private float[] _smoothedY = new float[8];

        public AppDirectionCalculator(float smoothingAlpha = 0.3f)
        {
            _smoothingAlpha = smoothingAlpha;
            Reset();
        }

        public List<(float x, float y, float magnitude)> Calculate(float[] peakValues)
        {
            var results = new List<(float x, float y, float magnitude)>();
            if (peakValues == null) return results;

            int count = Math.Min(peakValues.Length, ChannelMapping.ChannelCount);

            if (count == 2)
            {
                float left = peakValues[0];
                float right = peakValues[1];
                float maxPeak = Math.Max(left, right);
                
                if (maxPeak > 0.01f)
                {
                    float x = left - right;
                    float y = 2.0f * Math.Min(left, right);
                    float mag = (float)Math.Sqrt(x * x + y * y);
                    
                    float rawX = 0, rawY = 0;
                    if (mag > 1e-6f)
                    {
                        rawX = x / mag;
                        rawY = y / mag;
                    }

                    _smoothedX[0] = _smoothingAlpha * rawX + (1f - _smoothingAlpha) * _smoothedX[0];
                    _smoothedY[0] = _smoothingAlpha * rawY + (1f - _smoothingAlpha) * _smoothedY[0];
                    
                    float smoothMag = (float)Math.Sqrt(_smoothedX[0] * _smoothedX[0] + _smoothedY[0] * _smoothedY[0]);
                    if (smoothMag > 1e-6f)
                    {
                        results.Add((_smoothedX[0] / smoothMag, _smoothedY[0] / smoothMag, maxPeak));
                    }
                }
                else
                {
                    _smoothedX[0] = 0;
                    _smoothedY[0] = 0;
                }
            }
            else
            {
                // For 7.1, evaluate each speaker independently
                for (int ch = 0; ch < count; ch++)
                {
                    if (!ChannelMapping.IsDirectional[ch]) continue;

                    float peak = peakValues[ch];
                    if (peak > 0.01f)
                    {
                        float rawX = ChannelMapping.ChannelVectorsX[ch];
                        float rawY = ChannelMapping.ChannelVectorsY[ch];

                        _smoothedX[ch] = _smoothingAlpha * rawX + (1f - _smoothingAlpha) * _smoothedX[ch];
                        _smoothedY[ch] = _smoothingAlpha * rawY + (1f - _smoothingAlpha) * _smoothedY[ch];

                        float smoothMag = (float)Math.Sqrt(_smoothedX[ch] * _smoothedX[ch] + _smoothedY[ch] * _smoothedY[ch]);
                        if (smoothMag > 1e-6f)
                        {
                            results.Add((_smoothedX[ch] / smoothMag, _smoothedY[ch] / smoothMag, peak));
                        }
                    }
                    else
                    {
                        _smoothedX[ch] *= (1f - _smoothingAlpha);
                        _smoothedY[ch] *= (1f - _smoothingAlpha);
                    }
                }
            }

            return results;
        }

        public void Reset()
        {
            for (int i = 0; i < 8; i++)
            {
                _smoothedX[i] = 0f;
                _smoothedY[i] = 0f;
            }
        }
    }
}
