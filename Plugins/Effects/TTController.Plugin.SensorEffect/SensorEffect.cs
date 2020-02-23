﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using LibreHardwareMonitor.Hardware;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.SensorEffect
{
    public class SensorEffectConfig : EffectConfigBase
    {
        public List<Identifier> Sensors { get; internal set; } = new List<Identifier>();
        [DefaultValue(SensorMixFunction.Maximum)] public SensorMixFunction SensorMixFunction { get; internal set; } = SensorMixFunction.Maximum;
        [DefaultValue(0.05)] public double SmoothingFactor { get; internal set; } = 0.05;
        public LedColorGradient ColorGradient { get; internal set; } = new LedColorGradient();
    }

    public class SensorEffect : EffectBase<SensorEffectConfig>
    {
        private double _r, _g, _b;
        private readonly float _minValue, _maxValue;

        public override string EffectType => "PerLed";

        public SensorEffect(SensorEffectConfig config) : base(config, config.Sensors)
        {
            _r = config.ColorGradient.Start.Color.R;
            _g = config.ColorGradient.Start.Color.G;
            _b = config.ColorGradient.Start.Color.B;

            _minValue = (float)config.ColorGradient.Start.Location;
            _maxValue = (float)config.ColorGradient.End.Location;
        }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            var values = Config.Sensors.Select(cache.GetSensorValue);
            var value = float.NaN;
            if (Config.SensorMixFunction == SensorMixFunction.Average)
                value = values.Average();
            else if (Config.SensorMixFunction == SensorMixFunction.Minimum)
                value = values.Min();
            else if (Config.SensorMixFunction == SensorMixFunction.Maximum)
                value = values.Max();

            if (value < _minValue)
                value = _minValue;
            if (value > _maxValue)
                value = _maxValue;

            if (float.IsNaN(value))
            {
                _r = Config.ColorGradient.Start.Color.R;
                _g = Config.ColorGradient.Start.Color.G;
                _b = Config.ColorGradient.Start.Color.B;
            }
            else
            {
                var (rr, gg, bb) = Config.ColorGradient.GetColorSmooth(value);

                _r = _r * (1 - Config.SmoothingFactor) + rr * Config.SmoothingFactor;
                _g = _g * (1 - Config.SmoothingFactor) + gg * Config.SmoothingFactor;
                _b = _b * (1 - Config.SmoothingFactor) + bb * Config.SmoothingFactor;
            }

            var color = new LedColor((byte)_r, (byte)_g, (byte)_b);
            return ports.ToDictionary(p => p, p => Enumerable.Repeat(color, cache.GetDeviceConfig(p).LedCount).ToList());
        }
    }
}
