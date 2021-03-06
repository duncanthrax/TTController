﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.BlinkEffect
{
    public class BlinkEffectConfig : EffectConfigBase
    {
        [DefaultValue(1000)] public int OnTime { get; internal set; } = 1000;
        [DefaultValue(1000)] public int OffTime { get; internal set; } = 1000;
        public LedColorProvider OnColor { get; internal set; } = new LedColorProvider();
        public LedColorProvider OffColor { get; internal set; } = new LedColorProvider();
    }

    public class BlinkEffect : EffectBase<BlinkEffectConfig>
    {
        private int _ticks;
        private bool _state;

        public BlinkEffect(BlinkEffectConfig config) : base(config) { }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            var current = Environment.TickCount;
            var diff = current - _ticks;

            if ((_state && diff > Config.OnTime) || (!_state && diff > Config.OffTime))
            {
                _ticks = current;
                _state = !_state;
            }

            if (Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
            {
                return EffectUtils.GenerateColorsPerPort(ports, cache,
                    (port, ledCount) => (_state ? Config.OnColor : Config.OffColor).Get(ledCount).ToList()
                );
            }
            else if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var totalLedCount = ports.Select(p => cache.GetDeviceConfig(p).LedCount).Sum();
                var colors = (_state ? Config.OnColor : Config.OffColor).Get(totalLedCount).ToList();
                return EffectUtils.SplitColorsPerPort(colors, ports, cache);
            }

            return null;
        }
    }
}
