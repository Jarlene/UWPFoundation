using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Animation
{
    public class Animator
    {
        
        public static IAnimation Use(AnimationType animType)
        {
            return CreateAnimationbyType(animType);
        }

        private static IAnimation CreateAnimationbyType(AnimationType animType)
        {
            var animName = Enum.GetName(typeof(AnimationType), animType);
            var animation = (IAnimation)Activator.CreateInstance(Type.GetType(string.Format("Animation.{0}Animation", animName)));
            return animation;
        }
    }
}
