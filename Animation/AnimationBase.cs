using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Animation
{
    public abstract class AnimationBase : IAnimation
    {
        protected TimeSpan? Delay { get; set; }

        protected TimeSpan Duration { get; set; }

        protected RepeatBehavior RepeatBehavior { get; set; } = new RepeatBehavior(1);

        protected Storyboard Storyboard { get; set; } = new Storyboard();

        public IAnimation ContinueWith(Type type)
        {
            throw new NotImplementedException();
        }

        public IAnimation PlayOn(UIElement target)
        {
            return PlayOn(target, null);
        }

        public abstract IAnimation PlayOn(UIElement target, Action ContinueWith);

        public virtual IAnimation SetDelay(TimeSpan delay)
        {
            Delay = delay;
            return this;
        }

        public IAnimation SetDuration(TimeSpan duration)
        {
            Duration = duration;
            return this;
        }

        public IAnimation SetRepeatBehavior(RepeatBehavior repeatBehavior)
        {
            RepeatBehavior = repeatBehavior;
            return this;
        }

        protected Storyboard PrepareStoryboard(Action continueWith)
        {
            Storyboard.Completed += (s, e) =>
            {
                if (continueWith != null)
                    continueWith();
            };

            Storyboard.BeginTime = Delay;
            Storyboard.RepeatBehavior = RepeatBehavior;

            return Storyboard;
        }

        public virtual void Stop()
        {
            Storyboard.Stop();
        }

        protected void AddAnimationToStoryboard(Storyboard storyboard, DependencyObject target, Timeline anim, string property)
        {
            storyboard.Children.Add(anim);

            Storyboard.SetTarget(anim, target);
            Storyboard.SetTargetProperty(anim, property);
        }
    }
}
