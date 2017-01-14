using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Animation
{
    public class ShakeAnimation : AnimationBase
    {
        public ShakeAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(800);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var transform = AnimationUtils.PrepareTransform(target, typeof(CompositeTransform));

            var storyboard = PrepareStoryboard(continueWith);

            var anim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 10, -10, 10, -10, 6, -6, 2, -2, 0);

            AddAnimationToStoryboard(storyboard, transform, anim, "TranslateX");

            storyboard.Begin();

            return this;
        }
    }
}
