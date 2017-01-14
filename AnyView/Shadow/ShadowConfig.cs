﻿using System;
using System.Collections.Generic;

namespace AnyView
{
    public class EffectKey
    {
        public EffectKey(int width, int height, int zDepth, double cornerRadius)
        {
            Width = width;
            Height = height;
            Z_Depth = zDepth;
            CornerRadius = cornerRadius;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Z_Depth { get; set; }

        public double CornerRadius { get; set; }

        public override int GetHashCode()
        {
            return Width.GetHashCode() ^
                Height.GetHashCode() ^
                Z_Depth.GetHashCode() ^
                CornerRadius.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var key = obj as EffectKey;
            return key != null &&
                key.Width == Width &&
                key.Height == Height &&
                key.Z_Depth == Z_Depth &&
                key.CornerRadius == CornerRadius;
        }
    }

    public class ShadowParam
    {
        public byte Alpha { get; set; }

        public float Blur { get; set; }

        public int Offset_Y { get; set; }
    }

    public static class ShadowConfig
    {
        private static Dictionary<int, List<ShadowParam>> _shadowParamsWithZDepth = new Dictionary<int, List<ShadowParam>>()
        {
            [1] = new List<ShadowParam>() { new ShadowParam() { Alpha = 61, Blur = 1.5f, Offset_Y = 1 }, new ShadowParam() { Alpha = 31, Blur = 2f, Offset_Y = 1 } },
            [2] = new List<ShadowParam>() { new ShadowParam() { Alpha = 59, Blur = 3f, Offset_Y = 3 }, new ShadowParam() { Alpha = 41, Blur = 3f, Offset_Y = 3 } },
            [3] = new List<ShadowParam>() { new ShadowParam() { Alpha = 59, Blur = 4f, Offset_Y = 3 }, new ShadowParam() { Alpha = 48, Blur = 5f, Offset_Y = 5 } },
            [4] = new List<ShadowParam>() { new ShadowParam() { Alpha = 56, Blur = 5f, Offset_Y = 4 }, new ShadowParam() { Alpha = 64, Blur = 9f, Offset_Y = 7 } },
            [5] = new List<ShadowParam>() { new ShadowParam() { Alpha = 56, Blur = 7f, Offset_Y = 5 }, new ShadowParam() { Alpha = 77, Blur = 12f, Offset_Y = 10 } },
        };

        public static List<ShadowParam> GetShadowParamForZDepth(int z_depth)
        {
            if (z_depth < 1 || z_depth > 5)
                throw new ArgumentException("z_depth should between 1 to 5");

            return _shadowParamsWithZDepth[z_depth];
        }
    }
}
