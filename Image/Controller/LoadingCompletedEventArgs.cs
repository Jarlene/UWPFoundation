using System;

namespace Image.Controller
{
    public class LoadingCompletedEventArgs : EventArgs
    {
        public double PixelHeight
        {
            get;
            set;
        }
        public double PixelWidth
        {
            get;
            set;
        }
        internal LoadingCompletedEventArgs(double pixelWidth, double pixelHeight)
        {
            this.PixelHeight = pixelHeight;
            this.PixelWidth = pixelWidth;
        }
    }
}
