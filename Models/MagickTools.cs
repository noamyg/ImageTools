using ImageMagick;
using System;
using System.Collections.Generic;

namespace ImageTools.Models
{
    public sealed class MagickTools
    {
        public static MagickReadSettings ReadSettings = new MagickReadSettings()
        {
            Density = new Density(
                Double.Parse(System.Configuration.ConfigurationManager.AppSettings["ImageDensityX"].ToString()),
                Double.Parse(System.Configuration.ConfigurationManager.AppSettings["ImageDensityY"].ToString())
            )
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
            {"image/tiff", MagickFormat.Tiff}
        };
    }
}