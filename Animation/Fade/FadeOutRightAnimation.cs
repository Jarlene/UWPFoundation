using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Animation
{
    public class FadeOutRightAnimation : AnimationBase
    {
        public FadeOutRightAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(500);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var transform = (CompositeTransform)AnimationUtils.PrepareTransform(target, typeof(CompositeTransform));
            var storyboard = PrepareStoryboard(continueWith);

            var opacityAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 0);
            var translateAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, target.RenderSize.Width / 2);
            AddAnimationToStoryboard(storyboard, target, opacityAnim, "Opacity");
            AddAnimationToStoryboard(storyboard, transform, translateAnim, "TranslateX");

            storyboard.Begin();

            return this;
        }
    }
}