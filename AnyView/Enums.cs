using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyView
{

    public enum IconPosition
    {
        Left = 0,
        Right,
        Top,
        Bottom,
        NoIcon,
        OnlyIcon,
    }

    public enum SwipeDirection
    {
        None,
        Left,
        Right
    }

    public enum SwipeMode
    {
        None,
        Fix,
        Collapse,
        Expand,
    }
}
