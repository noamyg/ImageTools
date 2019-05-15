using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageTools.Models
{
    public sealed class MagickTools
    {
        public static MagickReadSettings ReadSettings = new MagickReadSettings()
        {
            Density = new Density(100, 100)
        };

        public static readonly List<MagickFormat> TransparencySupportedFormats = new List<MagickFormat>()
        {
            MagickFormat.Gif,
            MagickFormat.Png,
            MagickFormat.Bmp,
            MagickFormat.Tiff,
            MagickFormat.Jp2
        };

        public static readonly Dictionary<String, MagickFormat> ContentTypeToFormat = new Dictionary<String, MagickFormat>()
        {
            {"image/gif", MagickFormat.Gif},
            {"image/jpeg", MagickFormat.Jpeg},
            {"image/png", MagickFormat.Png},
            {"image/webp", MagickFormat.WebP},
            {"image/svg+xml", MagickFormat.Svg},
            {"image/x-icon", MagickFormat.Ico},
        };
    }
}