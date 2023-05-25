using System;
using System.Windows.Media;

namespace BingedIt.Common
{
    internal readonly struct HSLColor
    {
        readonly byte _alpha;
        readonly int _hue;
        readonly float _sat;
        readonly float _bri;

        public byte Alpha => _alpha;
        public int Hue => _hue;
        public float Saturation => _sat;
        public float Brightness => _bri;

        public HSLColor(byte a, int hue, float saturation, float brightness)
        {
            _alpha = a;
            _hue = hue;
            _sat = saturation;
            _bri = brightness;
        }

        public Color ToColor()
        {
            byte r, g, b;
            if (_sat == 0)
                r = g = b = (byte)(_bri * 255);
            else
            {
                float v1, v2;
                float hue = _hue / 360f;

                v2 = (_bri < 0.5) ? (_bri * (1 + _sat)) : (_bri + _sat - _bri * _sat);
                v1 = 2 * _bri - v2;

                r = (byte)(255 * ToRatio(v1, v2, hue + (1.0f / 3)));
                g = (byte)(255 * ToRatio(v1, v2, hue));
                b = (byte)(255 * ToRatio(v1, v2, hue - (1.0f / 3)));
            }

            return Color.FromArgb(_alpha, r, g, b);
        }

        private static float ToRatio(float v1, float v2, float vH)
        {
            if (vH < 0) vH += 1;
            if (vH > 1) vH -= 1;

            float value = float.Round(6 * vH, 2);
            if (value < 1)
                return v1 + ((v2 - v1) * 6 * vH);
            value = float.Round(2 * vH, 2);
            if (value < 1)
                return v2;
            value = float.Round(3 * vH, 2);
            if (value < 2)
                return v1 + (v2 - v1) * ((2f / 3) - vH) * 6;

            return v1;
        }

        public static HSLColor[] SampleColor(int sampleHue, int sampleSaturation, float saturationRange, int sampleBrightness, float brightnessRange)
        {
            int hueRate;
            float sat, bright, satRate, brightRate, brightOffset;
            // Check the input
            if (sampleHue <= 0 || sampleHue > 360) throw new ArgumentOutOfRangeException(nameof(sampleHue));
            hueRate = 360 / sampleHue;

            if (sampleSaturation <= 0) throw new ArgumentOutOfRangeException(nameof(sampleSaturation));
            if (saturationRange <= 0 || saturationRange > 1) throw new ArgumentOutOfRangeException(nameof(saturationRange));
            satRate = saturationRange / sampleSaturation;

            if (sampleBrightness <= 0) throw new ArgumentOutOfRangeException(nameof(sampleBrightness));
            if (brightnessRange <= 0 || brightnessRange > 1) throw new ArgumentOutOfRangeException(nameof(brightnessRange));
            // Brightness 0.0 and brighness 1.0 are just black and white, we should sample from the center
            brightRate = brightnessRange / sampleBrightness;
            brightOffset = (1 - brightnessRange) / 2f;

            // Start sampling
            HSLColor[] HSLArray = new HSLColor[sampleBrightness * sampleHue * sampleSaturation];
            int index = 0;
            sat = 1;
            for (int s = 0; s < sampleSaturation; s++, sat -= satRate)
            {
                for (int l = sampleBrightness - 1; l >= 0; l--)
                {
                    bright = (l + 0.5f) * brightRate + brightOffset;
                    int hue = 0;
                    for (int h = 0; h < sampleHue; h++, hue += hueRate)
                    {
                        HSLArray[index++] = new(byte.MaxValue, hue, sat, bright);
                    }
                }
            }
            return HSLArray;
        }
        public static HSLColor[] SampleGray(int sample)
        {
            // Sample the gray range of colors from black to white
            const int hue = 0, sat = 0;
            float brightRate = 1f / sample;
            HSLColor[] grayColors = new HSLColor[sample];
            for (int i = 0; i < sample; i++)
            {
                float bright = (i + 0.5f) * brightRate;
                grayColors[i] = new(byte.MaxValue, hue, sat, bright);
            }
            return grayColors;
        }
    }


    public static class BrushesProvider
    {
        static private SolidColorBrush[] _brushes;

        static BrushesProvider()
        {
            const int defaultHue = 12, defaultBright = 5, defaultSat = 2, defaultGray = 9, blackWhiteTrans = 3;
            const float defaultBrightnessRange = 0.75f, defaultSatRange = 0.5f;
            _brushes = new SolidColorBrush[defaultHue * defaultBright * defaultSat + blackWhiteTrans + defaultGray];
            HSLColor[] colors = HSLColor.SampleColor(defaultHue, defaultSat, defaultSatRange, defaultBright, defaultBrightnessRange);
            HSLColor[] grayColors = HSLColor.SampleGray(defaultGray);

            int index = 0;
            _brushes[index++] = Brushes.Transparent;
            _brushes[index++] = Brushes.Black;
            for (int i = 0; i < grayColors.Length; i++)
            {
                var brush = new SolidColorBrush(grayColors[i].ToColor());
                brush.Freeze();
                _brushes[index++] = brush;
            }
            _brushes[index++] = Brushes.White;
            for (int i = 0; i < colors.Length; i++)
            {
                var brush = new SolidColorBrush(colors[i].ToColor());
                brush.Freeze();
                _brushes[index++] = brush;
            }
        }

        static public SolidColorBrush[] ProvideValue() => _brushes;


    }
}
