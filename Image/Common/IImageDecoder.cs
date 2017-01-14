﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

namespace Image.Common
{
    public interface IImageDecoder:IDisposable
    {
        /// <summary>
        /// Gets the size of the header for this image type.
        /// </summary>
        /// <value>The size of the header.</value>
        int HeaderSize { get; }

        /// <summary>
        /// Indicates if the image decoder supports the specified
        /// file header.
        /// </summary>
        /// <param name="header">The file header.</param>
        /// <returns>
        /// <c>true</c>, if the decoder supports the specified
        /// file header; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="header"/>
        /// is null (Nothing in Visual Basic).</exception>
        bool IsSupportedFileFormat(byte[] header);

        /// <summary>
        /// 执行播放(动态图片)
        /// </summary>
        void Start();

        /// <summary>
        /// 执行停止(动态图片)
        /// </summary>
        void Stop();

        /// <summary>
        /// Recreate Surfaces
        /// </summary>
        /// <returns></returns>
        ImageSource RecreateSurfaces();

        /// <summary>
        /// 异步初始化
        /// </summary>
        /// <param name="dispatcher">用于UI线程绘制</param>
        /// <param name="streamSource">stream</param>
        /// <returns></returns>
        Task<ExtendImageSource> InitializeAsync(CoreDispatcher dispatcher,
            IRandomAccessStream streamSource, CancellationTokenSource cancellationTokenSource);
    }
}
