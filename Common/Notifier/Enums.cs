using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notifier
{
    public enum NotifyPriority
    {
        Lowest = 0,
        BelowNormal,
        Normal,
        AboveNormal,
        Highest
    }

    public enum ThreadMode
    {
        Current = 0,
        Main,
        Background
    }
}
