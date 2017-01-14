﻿using Common.Cache;
using Common.Cache.Memory;
using Common.Cache.Storage;
using Image.Common;
using Image.Parser;
using System;
using System.Collections.Generic;
using Windows.Storage.Streams;

namespace Image
{
    public class ImageConfig
    {
        public readonly CacheMode CacheMode;
        public readonly MemoryCacheBase<string, IRandomAccessStream> MemoryCacheImpl;
        public readonly StorageCacheBase StorageCacheImpl;
        public readonly IUriParser UriParser;
        public readonly List<Type> DecoderTypes;

        private ImageConfig(Builder builder)
        {
            CacheMode = builder.CacheMode;
            MemoryCacheImpl = builder.MemoryCacheImpl;
            StorageCacheImpl = builder.StorageCacheImpl;
            DecoderTypes = builder.DecoderTypes;
            UriParser = builder.UriParser;
        }

        /// <summary>
        /// Implements Builder pattern
        /// </summary>
        /// <see cref="http://en.wikipedia.org/wiki/Builder_pattern"/>
        public class Builder
        {

            /// <summary>
            /// Cache Mode
            /// </summary>
            private CacheMode _cacheMode = CacheMode.MemoryAndStorageCache;

            /// <summary>
            /// Decoder Types
            /// </summary>
            private List<Type> _decoderTypes = new List<Type>();

            /// <summary>
            /// Gets/Sets caching mode for ImageLoader
            /// Default - CacheMode.MemoryAndStorageCache
            /// </summary>
            public CacheMode CacheMode { get { return _cacheMode; } set { _cacheMode = value; } }

            /// <summary>
            /// Gets/Sets memory cache implementation for ImageLoader
            /// If you will leave it empty but CacheMode will require it, will be used WeakMemoryCache implementation 
            /// </summary>
            public MemoryCacheBase<string, IRandomAccessStream> MemoryCacheImpl { get; set; }

            /// <summary>
            /// Gets/Sets storage cache implementation for ImageLoader
            /// If you will leave it empty but CacheMode will require it, exception will be thrown
            /// Default - null, I am sorry for that, but it requires StorageFolder instance, so you have to init it anyway
            /// </summary>
            public StorageCacheBase StorageCacheImpl { get; set; }

            public IUriParser UriParser { get; set; }

            public List<Type> DecoderTypes
            {
                get
                {
                    return _decoderTypes;
                }
                set
                {
                    _decoderTypes = value;
                }
            }

            public Builder AddDecoder<TDecoder>() where TDecoder : IImageDecoder
            {
                if (!_decoderTypes.Contains(typeof(TDecoder)))
                {
                    _decoderTypes.Add(typeof(TDecoder));
                }
                return this;
            }

            public ImageConfig Build()
            {
                CheckParams();
                return new ImageConfig(this);
            }

            private void CheckParams()
            {
                if ((CacheMode == CacheMode.MemoryAndStorageCache ||
                    CacheMode == CacheMode.OnlyMemoryCache) && MemoryCacheImpl == null)
                {
                    throw new ArgumentException("CacheMode " + CacheMode + " requires MemoryCacheImpl");
                }
                if ((CacheMode == CacheMode.MemoryAndStorageCache ||
                    CacheMode == CacheMode.OnlyStorageCache) && StorageCacheImpl == null)
                {
                    throw new ArgumentException("CacheMode " + CacheMode + " requires StorageCacheImpl");
                }
            }
        }
    }
}
