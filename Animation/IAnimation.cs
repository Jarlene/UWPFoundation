using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Animation
{
    public interface IAnimation
    {
        IAnimation PlayOn(UIElement target, Action ContinueWith);

        IAnimation PlayOn(UIElement target);

        void Stop();

        IAnimation SetDuration(TimeSpan timeSpan);

        IAnimation SetDelay(TimeSpan timeSpan);

        IAnimation SetRepeatBehavior(RepeatBehavior repeatBehavior);
    }
}
