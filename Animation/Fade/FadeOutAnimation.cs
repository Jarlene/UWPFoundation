
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
    public class FadeOutAnimation : AnimationBase
    {
        public FadeOutAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(500);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var storyboard = PrepareStoryboard(continueWith);

            var opacityAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 0);
            AddAnimationToStoryboard(storyboard, target, opacityAnim, "Opacity");

            storyboard.Begin();

            return this;
        }
    }
}