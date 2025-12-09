namespace OxyPlot.Maui.Skia.Fonts;

internal static class SkFontsHelper
{
    public static IPlotFontsResolver FontsResolver { get; set; }

    public static IMauiFontLoader MauiFontLoader { get; set; } =
#if __ANDROID__
         new Platforms.Android.MauiFontLoader();
#elif WINDOWS
         new Windows.MauiFontLoader();
#elif MACCATALYST
         new mac.MauiFontLoader();
#elif __IOS__
         new OxyPlot.Maui.Skia.ios.MauiFontLoader();
#else
        null;
#endif

    public static SKTypeface ResolveFont(string fontFamily, int fontWeight)
    {
        if (string.IsNullOrEmpty(fontFamily)) return SKTypeface.Default;

        var typeface = SKTypeface.FromFamilyName(fontFamily, new SKFontStyle(fontWeight, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Upright));
        if (typeface != null && typeface.FamilyName == fontFamily)
            return typeface;

        var fontStream = MauiFontLoader?.Load(fontFamily);
        if (fontStream == null)
        {
            fontStream = FontsResolver?.ResolveFont(fontFamily, fontWeight);
        }

        if (fontStream != null)
        {
            typeface?.Dispose();
            typeface = SKTypeface.FromStream(fontStream);
            fontStream.Close();
            return typeface;
        }

        return typeface ?? SKTypeface.Default;
    }
}