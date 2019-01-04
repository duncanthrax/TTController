﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Service.Trigger
{
    public enum LogicOperation
    {
        And,
        Or
    }

    public class LogicTriggerConfig : TriggerConfigBase
    {
        public LogicOperation Operation { get; set; }
        public bool Negate { get; set; } = false;
        public List<ITriggerBase> Triggers { get; set; }
    }

    public class LogicTrigger : TriggerBase<LogicTriggerConfig>
    {
        public LogicTrigger(LogicTriggerConfig config) : base(config) { }

        public override bool Value()
        {
            var result = false;
            if(Config.Operation == LogicOperation.And)
                result = Config.Triggers.All(t => t.Value());
            else if(Config.Operation == LogicOperation.Or)
                result = Config.Triggers.Any(t => t.Value());

            if (Config.Negate)
                result = !result;

            return result;
        }
    }
}
