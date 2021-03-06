﻿using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.AlwaysTrigger
{
    public class AlwaysTriggerConfig : TriggerConfigBase { }

    public class AlwaysTrigger : TriggerBase<AlwaysTriggerConfig>
    {
        public AlwaysTrigger(AlwaysTriggerConfig config) : base(config) { }

        public override bool Value(ICacheProvider cache) => true;
    }
}
