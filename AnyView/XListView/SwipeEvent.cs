using Windows.UI.Xaml.Media.Animation;

namespace AnyView
{
    public delegate void SwipeBeginEventHandler(object sender);
    public delegate void SwipeProgressEventHandler(object sender, SwipeProgressEventArgs args);
    public delegate void SwipeCompleteEventHandler(object sender, SwipeCompleteEventArgs args);
    public delegate void SwipeReleaseEventHandler(object sender, SwipeReleaseEventArgs args);
    public delegate void SwipeTriggerEventHandler(object sender, SwipeTriggerEventArgs args);

    public class SwipeProgressEventArgs
    {
        public SwipeProgressEventArgs(SwipeDirection direction, double cumulative, double delta, double currRate)
        {
            SwipeDirection = direction;
            Cumulative = cumulative;
            CurrentRate = currRate;
            Delta = delta;
        }

        public SwipeDirection SwipeDirection { get; set; }

        public double Cumulative { get; set; }

        public double Delta { get; set; }

        public double CurrentRate { get; set; }
    }

    public class SwipeCompleteEventArgs
    {
        public SwipeCompleteEventArgs(SwipeDirection direction)
        {
            SwipeDirection = direction;
        }

        public SwipeDirection SwipeDirection { get; set; }
    }

    public class SwipeReleaseEventArgs
    {
        public SwipeReleaseEventArgs(SwipeDirection direction, EasingFunctionBase easingFunc, double itemToX, double duration)
        {
            SwipeDirection = direction;
            EasingFunc = easingFunc;
            ItemToX = itemToX;
            Duration = duration;
        }

        public SwipeDirection SwipeDirection { get; set; }

        public EasingFunctionBase EasingFunc { get; set; }

        public double ItemToX { get; set; }

        public double Duration { get; set; }
    }

    public class SwipeTriggerEventArgs
    {
        public SwipeTriggerEventArgs(SwipeDirection direction, bool isTrigger)
        {
            SwipeDirection = direction;
            IsTrigger = isTrigger;
        }

        public SwipeDirection SwipeDirection { get; set; }

        public bool IsTrigger { get; set; }
    }
}
