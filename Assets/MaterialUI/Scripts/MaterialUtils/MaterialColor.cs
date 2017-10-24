//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using Random = UnityEngine.Random;

namespace MaterialUI
{
    /// <summary>Static util class for getting and using material palette colors.</summary>
    public static class MaterialColor
    {
        /// <summary>Converts a Color to hex string format.</summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>A string containing the color's hex code.</returns>
        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        /// <summary>Converts a hex string to Color format.</summary>
        /// <param name="hex">The hex code to convert.</param>
        /// <param name="alpha">The desired alpha of the Color.</param>
        /// <returns>A Color containing the hex code's color value.</returns>
        public static Color HexToColor(string hex, float alpha = 1f)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, (byte)Mathf.RoundToInt(alpha * 255f));
        }

        /// <summary>Blends a base and highlight Color to create a single highlighted Color.</summary>
        /// <param name="baseColor">The base Color to blend.</param>
        /// <param name="baseHighlightColor">The highlight Color to blend.</param>
        /// <param name="blendAmount">The amount of baseHighlightColor to blend - the higher the value, the stronger the highlight.</param>
        /// <returns>The blended Color.</returns>
        public static Color HighlightColor(Color baseColor, Color baseHighlightColor, float blendAmount = 0.2f)
        {
            if (baseColor.a == 0)
            {
                baseHighlightColor.a = blendAmount;
            }
            else
            {
                baseHighlightColor.r = Tween.Linear(baseColor.r, baseHighlightColor.r, blendAmount, 1f);
                baseHighlightColor.g = Tween.Linear(baseColor.g, baseHighlightColor.g, blendAmount, 1f);
                baseHighlightColor.b = Tween.Linear(baseColor.b, baseHighlightColor.b, blendAmount, 1f);
                baseHighlightColor.a = Tween.Linear(baseColor.a, baseHighlightColor.a, blendAmount, 1f);
            }

            return baseHighlightColor;
        }

        /// <summary>Calculates whether light icons/text would be suitable over a certain color.</summary>
        /// <param name="baseColor">The base color to test.</param>
        /// <returns>True if white icons/text is suitable, false if not.</returns>
        public static bool LightOrDarkElements(Color baseColor)
        {
            if (baseColor.a < 0.5f) return true;
            return (baseColor.r + baseColor.b + baseColor.g) / 3 < 0.5f;
        }

        /// <summary>Calculates the distance between two Colors.</summary>
        /// <param name="color1">The first Color.</param>
        /// <param name="color2">The second Color.</param>
        /// <returns>The distance between the two Colors.</returns>
        public static float ColorDistance(Color color1, Color color2)
        {
            float rmean = (color1.r + color2.r) / 2;
            int r = (int)(color1.r * 256 - color2.r * 256);
            int g = (int)(color1.g * 256 - color2.g * 256);
            int b = (int)(color1.b * 256 - color2.b * 256);
            float weightR = 2 + rmean / 256;
            float weightG = 4f;
            float weightB = 2 + (255 - rmean) / 256;
            return Mathf.Sqrt(weightR * r * r + weightG * g * g + weightB * b * b);
        }

        /// <summary>Finds the closest Color from the material palette to a given Color.</summary>
        /// <param name="color">The Color to match.</param>
        /// <returns>The closest material palette Color to the input Color.</returns>
        public static Color ClosestMaterialColor(Color color)
        {
            //  Hard-coded matches for common colors that don't match well
            if (color == Color.blue)
            {
                return indigoA700;
            }

            float distance = float.MaxValue;
            Color closestColor = new Color();

            if (HSBColor.FromColor(color).s < m_LowestSaturation)
            {
                for (int i = 0; i < greySet.Length; i++)
                {
                    float currentDistance = ColorDistance(color, greySet[i]);
                    if (!(currentDistance < distance)) continue;
                    distance = currentDistance;
                    closestColor = greySet[i];
                }
                return closestColor;
            }

            for (int i = 0; i < m_ColorSets.Length; i++)
            {
                for (int j = 0; j < m_ColorSets[i].Length - 1; j++)
                {
                    float currentDistance = ColorDistance(color, m_ColorSets[i][j]);
                    if (!(currentDistance < distance)) continue;
                    distance = currentDistance;
                    closestColor = m_ColorSets[i][j];
                }
            }

            return closestColor;
        }

        /// <summary>Finds the closest Color set from the material palette to a given Color.</summary>
        /// <param name="color">The Color to match.</param>
        /// <returns>The closest material palette Color set to the input Color.</returns>
        public static Color[] ClosestMaterialColorSet(Color color)
        {
            float distance = float.MaxValue;
            Color[] closestSet = null;

            if (HSBColor.FromColor(color).s < m_LowestSaturation)
            {
                return greySet;
            }

            for (int i = 0; i < m_ColorSets.Length; i++)
            {
                for (int j = 0; j < m_ColorSets[i].Length - 1; j++)
                {
                    float currentDistance = ColorDistance(color, m_ColorSets[i][j]);
                    if (!(currentDistance < distance)) continue;
                    distance = currentDistance;
                    closestSet = m_ColorSets[i];
                }
            }

            return closestSet;
        }

        /// <summary>Finds the closest Material color to a given color, and offsets te shade within its color set.</summary>
        /// <param name="color">The color to match.</param>
        /// <param name="shadeOffset">The number of shades to offset the matched given color (can be negative).</param>
        /// <param name="colorSet">The colorset to sample from, if known. If null, ClosestMaterialColorSet() is used to find the matching set.</param>
        /// <returns>The shade-offset color, clamped to the set's lightest/darkest colors.</returns>
        public static Color OffsetShade(Color color, int shadeOffset, Color[] colorSet = null)
        {
            color = ClosestMaterialColor(color);

            if (colorSet == null)
            {
                colorSet = ClosestMaterialColorSet(color);
            }

            int colorIndex = -1;

            for (int i = 0; i < colorSet.Length; i++)
            {
                if (!color.Approximately(colorSet[i])) continue;
                colorIndex = i;
                break;
            }

            colorIndex = Mathf.Clamp(colorIndex - shadeOffset, 0, colorSet.Length - 1);

            return colorSet[colorIndex];
        }

        /// <summary>Randomly picks a '500' color from any of the material palette Color sets.</summary>
        /// <param name="includeGrey">Allow grey500 to be randomly selected.</param>
        /// <returns>A random '500' Color.</returns>
        public static Color Random500(bool includeGrey = false)
        {
            return m_ColorSets[Random.Range(0, includeGrey ? m_ColorSets.Length : m_ColorSets.Length - 1)][5];
        }

        #region Palette

        private const float m_LowestSaturation = 0.0207468f;

        private static readonly Color[][] m_ColorSets =
        {
            new []  // red
			{
                new Color(1.0000000f, 0.9215686f, 0.9333333f), // 50
				new Color(1.0000000f, 0.8039216f, 0.8235294f), // 100
				new Color(0.9372549f, 0.6039216f, 0.6039216f), // 200
				new Color(0.8980392f, 0.4509804f, 0.4509804f), // 300
				new Color(0.9372549f, 0.3254902f, 0.3137255f), // 400
				new Color(0.9568627f, 0.2627451f, 0.2117647f), // 500
				new Color(0.8980392f, 0.2235294f, 0.2078431f), // 600
				new Color(0.8274510f, 0.1843137f, 0.1843137f), // 700
				new Color(0.7764706f, 0.1568628f, 0.1568628f), // 800
				new Color(0.7176471f, 0.1098039f, 0.1098039f), // 900
				new Color(1.0000000f, 0.5411765f, 0.5019608f), // A100
				new Color(1.0000000f, 0.3215686f, 0.3215686f), // A200
				new Color(1.0000000f, 0.0901961f, 0.2666667f), // A400
				new Color(0.8352941f, 0.0000000f, 0.0000000f), // A700
			},
            new []  // pink
			{
                new Color(0.9882353f, 0.8941177f, 0.9254902f), // 50
				new Color(0.9725490f, 0.7333333f, 0.8156863f), // 100
				new Color(0.9568627f, 0.5607843f, 0.6941177f), // 200
				new Color(0.9411765f, 0.3843137f, 0.5725490f), // 300
				new Color(0.9254902f, 0.2509804f, 0.4784314f), // 400
				new Color(0.9137255f, 0.1176471f, 0.3882353f), // 500
				new Color(0.8470588f, 0.1058824f, 0.3764706f), // 600
				new Color(0.7607843f, 0.0941177f, 0.3568628f), // 700
				new Color(0.6784314f, 0.0784314f, 0.3411765f), // 800
				new Color(0.5333334f, 0.0549020f, 0.3098039f), // 900
				new Color(1.0000000f, 0.5019608f, 0.6705883f), // A100
				new Color(1.0000000f, 0.2509804f, 0.5058824f), // A200
				new Color(0.9607843f, 0.0000000f, 0.3411765f), // A400
				new Color(0.7725490f, 0.0666667f, 0.3843137f), // A700
			},
            new []  // purple
			{
                new Color(0.9529412f, 0.8980392f, 0.9607843f), // 50
				new Color(0.8823529f, 0.7450981f, 0.9058824f), // 100
				new Color(0.8078431f, 0.5764706f, 0.8470588f), // 200
				new Color(0.7294118f, 0.4078431f, 0.7843137f), // 300
				new Color(0.6705883f, 0.2784314f, 0.7372549f), // 400
				new Color(0.6117647f, 0.1529412f, 0.6901961f), // 500
				new Color(0.5568628f, 0.1411765f, 0.6666667f), // 600
				new Color(0.4823529f, 0.1215686f, 0.6352941f), // 700
				new Color(0.4156863f, 0.1058824f, 0.6039216f), // 800
				new Color(0.2901961f, 0.0784314f, 0.5490196f), // 900
				new Color(0.9176471f, 0.5019608f, 0.9882353f), // A100
				new Color(0.8784314f, 0.2509804f, 0.9843137f), // A200
				new Color(0.8352941f, 0.0000000f, 0.9764706f), // A400
				new Color(0.7725490f, 0.0666667f, 0.3843137f), // A700
			},
            new []  // deepPurple
			{
                new Color(0.9294118f, 0.9058824f, 0.9647059f), // 50
				new Color(0.8196079f, 0.7686275f, 0.9137255f), // 100
				new Color(0.7019608f, 0.6156863f, 0.8588235f), // 200
				new Color(0.5843138f, 0.4588235f, 0.8039216f), // 300
				new Color(0.4941176f, 0.3411765f, 0.7607843f), // 400
				new Color(0.4039216f, 0.2274510f, 0.7176471f), // 500
				new Color(0.3686275f, 0.2078431f, 0.6941177f), // 600
				new Color(0.3176471f, 0.1764706f, 0.6588235f), // 700
				new Color(0.2705882f, 0.1529412f, 0.6274510f), // 800
				new Color(0.1921569f, 0.1058824f, 0.5725490f), // 900
				new Color(0.7019608f, 0.5333334f, 1.0000000f), // A100
				new Color(0.4862745f, 0.3019608f, 1.0000000f), // A200
				new Color(0.3960784f, 0.1215686f, 1.0000000f), // A400
				new Color(0.3843137f, 0.0000000f, 0.9176471f), // A700
			},
            new []  // indigo
			{
                new Color(0.9098039f, 0.9176471f, 0.9647059f), // 50
				new Color(0.7725490f, 0.7921569f, 0.9137255f), // 100
				new Color(0.6235294f, 0.6588235f, 0.8549020f), // 200
				new Color(0.4745098f, 0.5254902f, 0.7960784f), // 300
				new Color(0.3607843f, 0.4196078f, 0.7529412f), // 400
				new Color(0.2470588f, 0.3176471f, 0.7098039f), // 500
				new Color(0.2235294f, 0.2862745f, 0.6705883f), // 600
				new Color(0.1882353f, 0.2470588f, 0.6235294f), // 700
				new Color(0.1568628f, 0.2078431f, 0.5764706f), // 800
				new Color(0.1019608f, 0.1372549f, 0.4941176f), // 900
				new Color(0.5490196f, 0.6196079f, 1.0000000f), // A100
				new Color(0.3254902f, 0.4274510f, 0.9960784f), // A200
				new Color(0.2392157f, 0.3529412f, 0.9960784f), // A400
				new Color(0.1882353f, 0.3098039f, 0.9960784f), // A700
			},
            new []  // blue
			{
                new Color(0.8901961f, 0.9490196f, 0.9921569f), // 50
				new Color(0.7333333f, 0.8705882f, 0.9843137f), // 100
				new Color(0.5647059f, 0.7921569f, 0.9764706f), // 200
				new Color(0.3921569f, 0.7098039f, 0.9647059f), // 300
				new Color(0.2588235f, 0.6470588f, 0.9607843f), // 400
				new Color(0.1294118f, 0.5882353f, 0.9529412f), // 500
				new Color(0.1176471f, 0.5333334f, 0.8980392f), // 600
				new Color(0.0980392f, 0.4627451f, 0.8235294f), // 700
				new Color(0.0823529f, 0.3960784f, 0.7529412f), // 800
				new Color(0.0509804f, 0.2784314f, 0.6313726f), // 900
				new Color(0.5098040f, 0.6941177f, 1.0000000f), // A100
				new Color(0.2666667f, 0.5411765f, 1.0000000f), // A200
				new Color(0.1607843f, 0.4745098f, 1.0000000f), // A400
				new Color(0.1607843f, 0.3843137f, 1.0000000f), // A700
			},
            new []  // lightBlue
			{
                new Color(0.8823529f, 0.9607843f, 0.9960784f), // 50
				new Color(0.7019608f, 0.8980392f, 0.9882353f), // 100
				new Color(0.5058824f, 0.8313726f, 0.9803922f), // 200
				new Color(0.3098039f, 0.7647059f, 0.9686275f), // 300
				new Color(0.1607843f, 0.7137255f, 0.9647059f), // 400
				new Color(0.0117647f, 0.6627451f, 0.9568627f), // 500
				new Color(0.0117647f, 0.6078432f, 0.8980392f), // 600
				new Color(0.0078431f, 0.5333334f, 0.8196079f), // 700
				new Color(0.0078431f, 0.4666667f, 0.7411765f), // 800
				new Color(0.0039216f, 0.3411765f, 0.6078432f), // 900
				new Color(0.5019608f, 0.8470588f, 1.0000000f), // A100
				new Color(0.2509804f, 0.7686275f, 1.0000000f), // A200
				new Color(0.0000000f, 0.6901961f, 1.0000000f), // A400
				new Color(0.0000000f, 0.5686275f, 0.9176471f), // A700
			},
            new []  // cyan
			{
                new Color(0.8784314f, 0.9686275f, 0.9803922f), // 50
				new Color(0.6980392f, 0.9215686f, 0.9490196f), // 100
				new Color(0.5019608f, 0.8705882f, 0.9176471f), // 200
				new Color(0.3019608f, 0.8156863f, 0.8823529f), // 300
				new Color(0.1490196f, 0.7764706f, 0.8549020f), // 400
				new Color(0.0000000f, 0.7372549f, 0.8313726f), // 500
				new Color(0.0000000f, 0.6745098f, 0.7568628f), // 600
				new Color(0.0000000f, 0.5921569f, 0.6549020f), // 700
				new Color(0.0000000f, 0.5137255f, 0.5607843f), // 800
				new Color(0.0000000f, 0.3764706f, 0.3921569f), // 900
				new Color(0.5176471f, 1.0000000f, 1.0000000f), // A100
				new Color(0.0941177f, 1.0000000f, 1.0000000f), // A200
				new Color(0.0000000f, 0.8980392f, 1.0000000f), // A400
				new Color(0.0000000f, 0.7215686f, 0.8313726f), // A700
			},
            new []  // teal
			{
                new Color(0.8784314f, 0.9490196f, 0.9450980f), // 50
				new Color(0.6980392f, 0.8745098f, 0.8588235f), // 100
				new Color(0.5019608f, 0.7960784f, 0.7686275f), // 200
				new Color(0.3019608f, 0.7137255f, 0.6745098f), // 300
				new Color(0.1490196f, 0.6509804f, 0.6039216f), // 400
				new Color(0.0000000f, 0.5882353f, 0.5333334f), // 500
				new Color(0.0000000f, 0.5372549f, 0.4823529f), // 600
				new Color(0.0000000f, 0.4745098f, 0.4196078f), // 700
				new Color(0.0000000f, 0.4117647f, 0.3607843f), // 800
				new Color(0.0000000f, 0.3019608f, 0.2509804f), // 900
				new Color(0.6549020f, 1.0000000f, 0.9215686f), // A100
				new Color(0.3921569f, 1.0000000f, 0.8549020f), // A200
				new Color(0.1137255f, 0.9137255f, 0.7137255f), // A400
				new Color(0.0000000f, 0.7490196f, 0.6470588f), // A700
			},
            new []  // green
			{
                new Color(0.9098039f, 0.9607843f, 0.9137255f), // 50
				new Color(0.7843137f, 0.9019608f, 0.7882353f), // 100
				new Color(0.6470588f, 0.8392157f, 0.6549020f), // 200
				new Color(0.5058824f, 0.7803922f, 0.5176471f), // 300
				new Color(0.4000000f, 0.7333333f, 0.4156863f), // 400
				new Color(0.2980392f, 0.6862745f, 0.3137255f), // 500
				new Color(0.2627451f, 0.6274510f, 0.2784314f), // 600
				new Color(0.2196078f, 0.5568628f, 0.2352941f), // 700
				new Color(0.1803922f, 0.4901961f, 0.1960784f), // 800
				new Color(0.1058824f, 0.3686275f, 0.1254902f), // 900
				new Color(0.7254902f, 0.9647059f, 0.7921569f), // A100
				new Color(0.4117647f, 0.9411765f, 0.6823530f), // A200
				new Color(0.0000000f, 0.9019608f, 0.4627451f), // A400
				new Color(0.0000000f, 0.7843137f, 0.3254902f), // A700
			},
            new []  // lightGreen
			{
                new Color(0.9450980f, 0.9725490f, 0.9137255f), // 50
				new Color(0.8627451f, 0.9294118f, 0.7843137f), // 100
				new Color(0.7725490f, 0.8823529f, 0.6470588f), // 200
				new Color(0.6823530f, 0.8352941f, 0.5058824f), // 300
				new Color(0.6117647f, 0.8000000f, 0.3960784f), // 400
				new Color(0.5450981f, 0.7647059f, 0.2901961f), // 500
				new Color(0.4862745f, 0.7019608f, 0.2588235f), // 600
				new Color(0.4078431f, 0.6235294f, 0.2196078f), // 700
				new Color(0.3333333f, 0.5450981f, 0.1843137f), // 800
				new Color(0.2000000f, 0.4117647f, 0.1176471f), // 900
				new Color(0.8000000f, 1.0000000f, 0.5647059f), // A100
				new Color(0.6980392f, 1.0000000f, 0.3490196f), // A200
				new Color(0.4627451f, 1.0000000f, 0.0117647f), // A400
				new Color(0.3921569f, 0.8666667f, 0.0901961f), // A700
			},
            new []  // lime
			{
                new Color(0.9764706f, 0.9843137f, 0.9058824f), // 50
				new Color(0.9411765f, 0.9568627f, 0.7647059f), // 100
				new Color(0.9019608f, 0.9333333f, 0.6117647f), // 200
				new Color(0.8627451f, 0.9058824f, 0.4588235f), // 300
				new Color(0.8313726f, 0.8823529f, 0.3411765f), // 400
				new Color(0.8039216f, 0.8627451f, 0.2235294f), // 500
				new Color(0.7529412f, 0.7921569f, 0.2000000f), // 600
				new Color(0.6862745f, 0.7058824f, 0.1686275f), // 700
				new Color(0.6196079f, 0.6156863f, 0.1411765f), // 800
				new Color(0.5098040f, 0.4666667f, 0.0901961f), // 900
				new Color(0.9568627f, 1.0000000f, 0.5058824f), // A100
				new Color(0.9333333f, 1.0000000f, 0.2549020f), // A200
				new Color(0.7764706f, 1.0000000f, 0.0000000f), // A400
				new Color(0.6823530f, 0.9176471f, 0.0000000f), // A700
			},
            new []  // yellow
			{
                new Color(1.0000000f, 0.9921569f, 0.9058824f), // 50
				new Color(1.0000000f, 0.9764706f, 0.7686275f), // 100
				new Color(1.0000000f, 0.9607843f, 0.6156863f), // 200
				new Color(1.0000000f, 0.9450980f, 0.4627451f), // 300
				new Color(1.0000000f, 0.9333333f, 0.3450980f), // 400
				new Color(1.0000000f, 0.9215686f, 0.2313726f), // 500
				new Color(0.9921569f, 0.8470588f, 0.2078431f), // 600
				new Color(0.9843137f, 0.7529412f, 0.1764706f), // 700
				new Color(0.9764706f, 0.6588235f, 0.1450980f), // 800
				new Color(0.9607843f, 0.4980392f, 0.0901961f), // 900
				new Color(1.0000000f, 1.0000000f, 0.5529412f), // A100
				new Color(1.0000000f, 1.0000000f, 0.0000000f), // A200
				new Color(1.0000000f, 0.9176471f, 0.0000000f), // A400
				new Color(1.0000000f, 0.8392157f, 0.0000000f), // A700
			},
            new []  // amber
			{
                new Color(1.0000000f, 0.9725490f, 0.8823529f), // 50
				new Color(1.0000000f, 0.9254902f, 0.7019608f), // 100
				new Color(1.0000000f, 0.8784314f, 0.5098040f), // 200
				new Color(1.0000000f, 0.8352941f, 0.3098039f), // 300
				new Color(1.0000000f, 0.7921569f, 0.1568628f), // 400
				new Color(1.0000000f, 0.7568628f, 0.0274510f), // 500
				new Color(1.0000000f, 0.7019608f, 0.0000000f), // 600
				new Color(1.0000000f, 0.6274510f, 0.0000000f), // 700
				new Color(1.0000000f, 0.5607843f, 0.0000000f), // 800
				new Color(1.0000000f, 0.4352941f, 0.0000000f), // 900
				new Color(1.0000000f, 0.8980392f, 0.4980392f), // A100
				new Color(1.0000000f, 0.8431373f, 0.2509804f), // A200
				new Color(1.0000000f, 0.7686275f, 0.0000000f), // A400
				new Color(1.0000000f, 0.6705883f, 0.0000000f), // A700
			},
            new []  // orange
			{
                new Color(1.0000000f, 0.9529412f, 0.8784314f), // 50
				new Color(1.0000000f, 0.8784314f, 0.6980392f), // 100
				new Color(1.0000000f, 0.8000000f, 0.5019608f), // 200
				new Color(1.0000000f, 0.7176471f, 0.3019608f), // 300
				new Color(1.0000000f, 0.6549020f, 0.1490196f), // 400
				new Color(1.0000000f, 0.5960785f, 0.0000000f), // 500
				new Color(0.9843137f, 0.5490196f, 0.0000000f), // 600
				new Color(0.9607843f, 0.4862745f, 0.0000000f), // 700
				new Color(0.9372549f, 0.4235294f, 0.0000000f), // 800
				new Color(0.9019608f, 0.3176471f, 0.0000000f), // 900
				new Color(1.0000000f, 0.8196079f, 0.5019608f), // A100
				new Color(1.0000000f, 0.6705883f, 0.2509804f), // A200
				new Color(1.0000000f, 0.5686275f, 0.0000000f), // A400
				new Color(1.0000000f, 0.4274510f, 0.0000000f), // A700
			},
            new []  // deepOrange
			{
                new Color(0.9843137f, 0.9137255f, 0.9058824f), // 50
				new Color(1.0000000f, 0.8000000f, 0.7372549f), // 100
				new Color(1.0000000f, 0.6705883f, 0.5686275f), // 200
				new Color(1.0000000f, 0.5411765f, 0.3960784f), // 300
				new Color(1.0000000f, 0.4392157f, 0.2627451f), // 400
				new Color(1.0000000f, 0.3411765f, 0.1333333f), // 500
				new Color(0.9568627f, 0.3176471f, 0.1176471f), // 600
				new Color(0.9019608f, 0.2901961f, 0.0980392f), // 700
				new Color(0.8470588f, 0.2627451f, 0.0823529f), // 800
				new Color(0.7490196f, 0.2117647f, 0.0470588f), // 900
				new Color(1.0000000f, 0.6196079f, 0.5019608f), // A100
				new Color(1.0000000f, 0.4313726f, 0.2509804f), // A200
				new Color(1.0000000f, 0.2392157f, 0.0000000f), // A400
				new Color(0.8666667f, 0.1725490f, 0.0000000f), // A700
			},
            new []  // brown
			{
                new Color(0.9372549f, 0.9215686f, 0.9137255f), // 50
				new Color(0.8431373f, 0.8000000f, 0.7843137f), // 100
				new Color(0.7372549f, 0.6666667f, 0.6431373f), // 200
				new Color(0.6313726f, 0.5333334f, 0.4980392f), // 300
				new Color(0.5529412f, 0.4313726f, 0.3882353f), // 400
				new Color(0.4745098f, 0.3333333f, 0.2823530f), // 500
				new Color(0.4274510f, 0.2980392f, 0.2549020f), // 600
				new Color(0.3647059f, 0.2509804f, 0.2156863f), // 700
				new Color(0.3058824f, 0.2039216f, 0.1803922f), // 800
				new Color(0.2431373f, 0.1529412f, 0.1372549f), // 900
			},
            new []  // blueGrey
			{
                new Color(0.9254902f, 0.9372549f, 0.9450980f), // 50
				new Color(0.8117647f, 0.8470588f, 0.8627451f), // 100
				new Color(0.6901961f, 0.7450981f, 0.7725490f), // 200
				new Color(0.5647059f, 0.6431373f, 0.6823530f), // 300
				new Color(0.4705882f, 0.5647059f, 0.6117647f), // 400
				new Color(0.3764706f, 0.4901961f, 0.5450981f), // 500
				new Color(0.3294118f, 0.4313726f, 0.4784314f), // 600
				new Color(0.2705882f, 0.3529412f, 0.3921569f), // 700
				new Color(0.2156863f, 0.2784314f, 0.3098039f), // 800
				new Color(0.1490196f, 0.1960784f, 0.2196078f), // 900
			},
            new []  // grey
			{
                new Color(0.9803922f, 0.9803922f, 0.9803922f), // 50
				new Color(0.9607843f, 0.9607843f, 0.9607843f), // 100
				new Color(0.9333333f, 0.9333333f, 0.9333333f), // 200
				new Color(0.8784314f, 0.8784314f, 0.8784314f), // 300
				new Color(0.7411765f, 0.7411765f, 0.7411765f), // 400
				new Color(0.6196079f, 0.6196079f, 0.6196079f), // 500
				new Color(0.4588235f, 0.4588235f, 0.4588235f), // 600
				new Color(0.3803922f, 0.3803922f, 0.3803922f), // 700
				new Color(0.2588235f, 0.2588235f, 0.2588235f), // 800
				new Color(0.1294118f, 0.1294118f, 0.1294118f), // 900
			}
        };
        public static Color[] colorSets { get { return m_ColorSets[0]; } }

        public static Color[] redSet { get { return m_ColorSets[0]; } }
        public static Color red50 { get { return m_ColorSets[0][0]; } }
        public static Color red100 { get { return m_ColorSets[0][1]; } }
        public static Color red200 { get { return m_ColorSets[0][2]; } }
        public static Color red300 { get { return m_ColorSets[0][3]; } }
        public static Color red400 { get { return m_ColorSets[0][4]; } }
        public static Color red500 { get { return m_ColorSets[0][5]; } }
        public static Color red600 { get { return m_ColorSets[0][6]; } }
        public static Color red700 { get { return m_ColorSets[0][7]; } }
        public static Color red800 { get { return m_ColorSets[0][8]; } }
        public static Color red900 { get { return m_ColorSets[0][9]; } }
        public static Color redA100 { get { return m_ColorSets[0][10]; } }
        public static Color redA200 { get { return m_ColorSets[0][11]; } }
        public static Color redA400 { get { return m_ColorSets[0][12]; } }
        public static Color redA700 { get { return m_ColorSets[0][13]; } }

        public static Color[] pinkSet { get { return m_ColorSets[1]; } }
        public static Color pink50 { get { return m_ColorSets[1][0]; } }
        public static Color pink100 { get { return m_ColorSets[1][1]; } }
        public static Color pink200 { get { return m_ColorSets[1][2]; } }
        public static Color pink300 { get { return m_ColorSets[1][3]; } }
        public static Color pink400 { get { return m_ColorSets[1][4]; } }
        public static Color pink500 { get { return m_ColorSets[1][5]; } }
        public static Color pink600 { get { return m_ColorSets[1][6]; } }
        public static Color pink700 { get { return m_ColorSets[1][7]; } }
        public static Color pink800 { get { return m_ColorSets[1][8]; } }
        public static Color pink900 { get { return m_ColorSets[1][9]; } }
        public static Color pinkA100 { get { return m_ColorSets[1][10]; } }
        public static Color pinkA200 { get { return m_ColorSets[1][11]; } }
        public static Color pinkA400 { get { return m_ColorSets[1][12]; } }
        public static Color pinkA700 { get { return m_ColorSets[1][13]; } }

        public static Color[] purpleSet { get { return m_ColorSets[2]; } }
        public static Color purple50 { get { return m_ColorSets[2][0]; } }
        public static Color purple100 { get { return m_ColorSets[2][1]; } }
        public static Color purple200 { get { return m_ColorSets[2][2]; } }
        public static Color purple300 { get { return m_ColorSets[2][3]; } }
        public static Color purple400 { get { return m_ColorSets[2][4]; } }
        public static Color purple500 { get { return m_ColorSets[2][5]; } }
        public static Color purple600 { get { return m_ColorSets[2][6]; } }
        public static Color purple700 { get { return m_ColorSets[2][7]; } }
        public static Color purple800 { get { return m_ColorSets[2][8]; } }
        public static Color purple900 { get { return m_ColorSets[2][9]; } }
        public static Color purpleA100 { get { return m_ColorSets[2][10]; } }
        public static Color purpleA200 { get { return m_ColorSets[2][11]; } }
        public static Color purpleA400 { get { return m_ColorSets[2][12]; } }
        public static Color purpleA700 { get { return m_ColorSets[2][13]; } }

        public static Color[] deepPurpleSet { get { return m_ColorSets[3]; } }
        public static Color deepPurple50 { get { return m_ColorSets[3][0]; } }
        public static Color deepPurple100 { get { return m_ColorSets[3][1]; } }
        public static Color deepPurple200 { get { return m_ColorSets[3][2]; } }
        public static Color deepPurple300 { get { return m_ColorSets[3][3]; } }
        public static Color deepPurple400 { get { return m_ColorSets[3][4]; } }
        public static Color deepPurple500 { get { return m_ColorSets[3][5]; } }
        public static Color deepPurple600 { get { return m_ColorSets[3][6]; } }
        public static Color deepPurple700 { get { return m_ColorSets[3][7]; } }
        public static Color deepPurple800 { get { return m_ColorSets[3][8]; } }
        public static Color deepPurple900 { get { return m_ColorSets[3][9]; } }
        public static Color deepPurpleA100 { get { return m_ColorSets[3][10]; } }
        public static Color deepPurpleA200 { get { return m_ColorSets[3][11]; } }
        public static Color deepPurpleA400 { get { return m_ColorSets[3][12]; } }
        public static Color deepPurpleA700 { get { return m_ColorSets[3][13]; } }

        public static Color[] indigoSet { get { return m_ColorSets[4]; } }
        public static Color indigo50 { get { return m_ColorSets[4][0]; } }
        public static Color indigo100 { get { return m_ColorSets[4][1]; } }
        public static Color indigo200 { get { return m_ColorSets[4][2]; } }
        public static Color indigo300 { get { return m_ColorSets[4][3]; } }
        public static Color indigo400 { get { return m_ColorSets[4][4]; } }
        public static Color indigo500 { get { return m_ColorSets[4][5]; } }
        public static Color indigo600 { get { return m_ColorSets[4][6]; } }
        public static Color indigo700 { get { return m_ColorSets[4][7]; } }
        public static Color indigo800 { get { return m_ColorSets[4][8]; } }
        public static Color indigo900 { get { return m_ColorSets[4][9]; } }
        public static Color indigoA100 { get { return m_ColorSets[4][10]; } }
        public static Color indigoA200 { get { return m_ColorSets[4][11]; } }
        public static Color indigoA400 { get { return m_ColorSets[4][12]; } }
        public static Color indigoA700 { get { return m_ColorSets[4][13]; } }

        public static Color[] blueSet { get { return m_ColorSets[5]; } }
        public static Color blue50 { get { return m_ColorSets[5][0]; } }
        public static Color blue100 { get { return m_ColorSets[5][1]; } }
        public static Color blue200 { get { return m_ColorSets[5][2]; } }
        public static Color blue300 { get { return m_ColorSets[5][3]; } }
        public static Color blue400 { get { return m_ColorSets[5][4]; } }
        public static Color blue500 { get { return m_ColorSets[5][5]; } }
        public static Color blue600 { get { return m_ColorSets[5][6]; } }
        public static Color blue700 { get { return m_ColorSets[5][7]; } }
        public static Color blue800 { get { return m_ColorSets[5][8]; } }
        public static Color blue900 { get { return m_ColorSets[5][9]; } }
        public static Color blueA100 { get { return m_ColorSets[5][10]; } }
        public static Color blueA200 { get { return m_ColorSets[5][11]; } }
        public static Color blueA400 { get { return m_ColorSets[5][12]; } }
        public static Color blueA700 { get { return m_ColorSets[5][13]; } }

        public static Color[] lightBlueSet { get { return m_ColorSets[6]; } }
        public static Color lightBlue50 { get { return m_ColorSets[6][0]; } }
        public static Color lightBlue100 { get { return m_ColorSets[6][1]; } }
        public static Color lightBlue200 { get { return m_ColorSets[6][2]; } }
        public static Color lightBlue300 { get { return m_ColorSets[6][3]; } }
        public static Color lightBlue400 { get { return m_ColorSets[6][4]; } }
        public static Color lightBlue500 { get { return m_ColorSets[6][5]; } }
        public static Color lightBlue600 { get { return m_ColorSets[6][6]; } }
        public static Color lightBlue700 { get { return m_ColorSets[6][7]; } }
        public static Color lightBlue800 { get { return m_ColorSets[6][8]; } }
        public static Color lightBlue900 { get { return m_ColorSets[6][9]; } }
        public static Color lightBlueA100 { get { return m_ColorSets[6][10]; } }
        public static Color lightBlueA200 { get { return m_ColorSets[6][11]; } }
        public static Color lightBlueA400 { get { return m_ColorSets[6][12]; } }
        public static Color lightBlueA700 { get { return m_ColorSets[6][13]; } }

        public static Color[] cyanSet { get { return m_ColorSets[7]; } }
        public static Color cyan50 { get { return m_ColorSets[7][0]; } }
        public static Color cyan100 { get { return m_ColorSets[7][1]; } }
        public static Color cyan200 { get { return m_ColorSets[7][2]; } }
        public static Color cyan300 { get { return m_ColorSets[7][3]; } }
        public static Color cyan400 { get { return m_ColorSets[7][4]; } }
        public static Color cyan500 { get { return m_ColorSets[7][5]; } }
        public static Color cyan600 { get { return m_ColorSets[7][6]; } }
        public static Color cyan700 { get { return m_ColorSets[7][7]; } }
        public static Color cyan800 { get { return m_ColorSets[7][8]; } }
        public static Color cyan900 { get { return m_ColorSets[7][9]; } }
        public static Color cyanA100 { get { return m_ColorSets[7][10]; } }
        public static Color cyanA200 { get { return m_ColorSets[7][11]; } }
        public static Color cyanA400 { get { return m_ColorSets[7][12]; } }
        public static Color cyanA700 { get { return m_ColorSets[7][13]; } }

        public static Color[] tealSet { get { return m_ColorSets[8]; } }
        public static Color teal50 { get { return m_ColorSets[8][0]; } }
        public static Color teal100 { get { return m_ColorSets[8][1]; } }
        public static Color teal200 { get { return m_ColorSets[8][2]; } }
        public static Color teal300 { get { return m_ColorSets[8][3]; } }
        public static Color teal400 { get { return m_ColorSets[8][4]; } }
        public static Color teal500 { get { return m_ColorSets[8][5]; } }
        public static Color teal600 { get { return m_ColorSets[8][6]; } }
        public static Color teal700 { get { return m_ColorSets[8][7]; } }
        public static Color teal800 { get { return m_ColorSets[8][8]; } }
        public static Color teal900 { get { return m_ColorSets[8][9]; } }
        public static Color tealA100 { get { return m_ColorSets[8][10]; } }
        public static Color tealA200 { get { return m_ColorSets[8][11]; } }
        public static Color tealA400 { get { return m_ColorSets[8][12]; } }
        public static Color tealA700 { get { return m_ColorSets[8][13]; } }

        public static Color[] greenSet { get { return m_ColorSets[9]; } }
        public static Color green50 { get { return m_ColorSets[9][0]; } }
        public static Color green100 { get { return m_ColorSets[9][1]; } }
        public static Color green200 { get { return m_ColorSets[9][2]; } }
        public static Color green300 { get { return m_ColorSets[9][3]; } }
        public static Color green400 { get { return m_ColorSets[9][4]; } }
        public static Color green500 { get { return m_ColorSets[9][5]; } }
        public static Color green600 { get { return m_ColorSets[9][6]; } }
        public static Color green700 { get { return m_ColorSets[9][7]; } }
        public static Color green800 { get { return m_ColorSets[9][8]; } }
        public static Color green900 { get { return m_ColorSets[9][9]; } }
        public static Color greenA100 { get { return m_ColorSets[9][10]; } }
        public static Color greenA200 { get { return m_ColorSets[9][11]; } }
        public static Color greenA400 { get { return m_ColorSets[9][12]; } }
        public static Color greenA700 { get { return m_ColorSets[9][13]; } }

        public static Color[] lightGreenSet { get { return m_ColorSets[10]; } }
        public static Color lightGreen50 { get { return m_ColorSets[10][0]; } }
        public static Color lightGreen100 { get { return m_ColorSets[10][1]; } }
        public static Color lightGreen200 { get { return m_ColorSets[10][2]; } }
        public static Color lightGreen300 { get { return m_ColorSets[10][3]; } }
        public static Color lightGreen400 { get { return m_ColorSets[10][4]; } }
        public static Color lightGreen500 { get { return m_ColorSets[10][5]; } }
        public static Color lightGreen600 { get { return m_ColorSets[10][6]; } }
        public static Color lightGreen700 { get { return m_ColorSets[10][7]; } }
        public static Color lightGreen800 { get { return m_ColorSets[10][8]; } }
        public static Color lightGreen900 { get { return m_ColorSets[10][9]; } }
        public static Color lightGreenA100 { get { return m_ColorSets[10][10]; } }
        public static Color lightGreenA200 { get { return m_ColorSets[10][11]; } }
        public static Color lightGreenA400 { get { return m_ColorSets[10][12]; } }
        public static Color lightGreenA700 { get { return m_ColorSets[10][13]; } }

        public static Color[] limeSet { get { return m_ColorSets[11]; } }
        public static Color lime50 { get { return m_ColorSets[11][0]; } }
        public static Color lime100 { get { return m_ColorSets[11][1]; } }
        public static Color lime200 { get { return m_ColorSets[11][2]; } }
        public static Color lime300 { get { return m_ColorSets[11][3]; } }
        public static Color lime400 { get { return m_ColorSets[11][4]; } }
        public static Color lime500 { get { return m_ColorSets[11][5]; } }
        public static Color lime600 { get { return m_ColorSets[11][6]; } }
        public static Color lime700 { get { return m_ColorSets[11][7]; } }
        public static Color lime800 { get { return m_ColorSets[11][8]; } }
        public static Color lime900 { get { return m_ColorSets[11][9]; } }
        public static Color limeA100 { get { return m_ColorSets[11][10]; } }
        public static Color limeA200 { get { return m_ColorSets[11][11]; } }
        public static Color limeA400 { get { return m_ColorSets[11][12]; } }
        public static Color limeA700 { get { return m_ColorSets[11][13]; } }

        public static Color[] yellowSet { get { return m_ColorSets[12]; } }
        public static Color yellow50 { get { return m_ColorSets[12][0]; } }
        public static Color yellow100 { get { return m_ColorSets[12][1]; } }
        public static Color yellow200 { get { return m_ColorSets[12][2]; } }
        public static Color yellow300 { get { return m_ColorSets[12][3]; } }
        public static Color yellow400 { get { return m_ColorSets[12][4]; } }
        public static Color yellow500 { get { return m_ColorSets[12][5]; } }
        public static Color yellow600 { get { return m_ColorSets[12][6]; } }
        public static Color yellow700 { get { return m_ColorSets[12][7]; } }
        public static Color yellow800 { get { return m_ColorSets[12][8]; } }
        public static Color yellow900 { get { return m_ColorSets[12][9]; } }
        public static Color yellowA100 { get { return m_ColorSets[12][10]; } }
        public static Color yellowA200 { get { return m_ColorSets[12][11]; } }
        public static Color yellowA400 { get { return m_ColorSets[12][12]; } }
        public static Color yellowA700 { get { return m_ColorSets[12][13]; } }

        public static Color[] amberSet { get { return m_ColorSets[13]; } }
        public static Color amber50 { get { return m_ColorSets[13][0]; } }
        public static Color amber100 { get { return m_ColorSets[13][1]; } }
        public static Color amber200 { get { return m_ColorSets[13][2]; } }
        public static Color amber300 { get { return m_ColorSets[13][3]; } }
        public static Color amber400 { get { return m_ColorSets[13][4]; } }
        public static Color amber500 { get { return m_ColorSets[13][5]; } }
        public static Color amber600 { get { return m_ColorSets[13][6]; } }
        public static Color amber700 { get { return m_ColorSets[13][7]; } }
        public static Color amber800 { get { return m_ColorSets[13][8]; } }
        public static Color amber900 { get { return m_ColorSets[13][9]; } }
        public static Color amberA100 { get { return m_ColorSets[13][10]; } }
        public static Color amberA200 { get { return m_ColorSets[13][11]; } }
        public static Color amberA400 { get { return m_ColorSets[13][12]; } }
        public static Color amberA700 { get { return m_ColorSets[13][13]; } }

        public static Color[] orangeSet { get { return m_ColorSets[14]; } }
        public static Color orange50 { get { return m_ColorSets[14][0]; } }
        public static Color orange100 { get { return m_ColorSets[14][1]; } }
        public static Color orange200 { get { return m_ColorSets[14][2]; } }
        public static Color orange300 { get { return m_ColorSets[14][3]; } }
        public static Color orange400 { get { return m_ColorSets[14][4]; } }
        public static Color orange500 { get { return m_ColorSets[14][5]; } }
        public static Color orange600 { get { return m_ColorSets[14][6]; } }
        public static Color orange700 { get { return m_ColorSets[14][7]; } }
        public static Color orange800 { get { return m_ColorSets[14][8]; } }
        public static Color orange900 { get { return m_ColorSets[14][9]; } }
        public static Color orangeA100 { get { return m_ColorSets[14][10]; } }
        public static Color orangeA200 { get { return m_ColorSets[14][11]; } }
        public static Color orangeA400 { get { return m_ColorSets[14][12]; } }
        public static Color orangeA700 { get { return m_ColorSets[14][13]; } }

        public static Color[] deepOrangeSet { get { return m_ColorSets[15]; } }
        public static Color deepOrange50 { get { return m_ColorSets[15][0]; } }
        public static Color deepOrange100 { get { return m_ColorSets[15][1]; } }
        public static Color deepOrange200 { get { return m_ColorSets[15][2]; } }
        public static Color deepOrange300 { get { return m_ColorSets[15][3]; } }
        public static Color deepOrange400 { get { return m_ColorSets[15][4]; } }
        public static Color deepOrange500 { get { return m_ColorSets[15][5]; } }
        public static Color deepOrange600 { get { return m_ColorSets[15][6]; } }
        public static Color deepOrange700 { get { return m_ColorSets[15][7]; } }
        public static Color deepOrange800 { get { return m_ColorSets[15][8]; } }
        public static Color deepOrange900 { get { return m_ColorSets[15][9]; } }
        public static Color deepOrangeA100 { get { return m_ColorSets[15][10]; } }
        public static Color deepOrangeA200 { get { return m_ColorSets[15][11]; } }
        public static Color deepOrangeA400 { get { return m_ColorSets[15][12]; } }
        public static Color deepOrangeA700 { get { return m_ColorSets[15][13]; } }

        public static Color[] brownSet { get { return m_ColorSets[16]; } }
        public static Color brown50 { get { return m_ColorSets[16][0]; } }
        public static Color brown100 { get { return m_ColorSets[16][1]; } }
        public static Color brown200 { get { return m_ColorSets[16][2]; } }
        public static Color brown300 { get { return m_ColorSets[16][3]; } }
        public static Color brown400 { get { return m_ColorSets[16][4]; } }
        public static Color brown500 { get { return m_ColorSets[16][5]; } }
        public static Color brown600 { get { return m_ColorSets[16][6]; } }
        public static Color brown700 { get { return m_ColorSets[16][7]; } }
        public static Color brown800 { get { return m_ColorSets[16][8]; } }
        public static Color brown900 { get { return m_ColorSets[16][9]; } }

        public static Color[] blueGreySet { get { return m_ColorSets[17]; } }
        public static Color blueGrey50 { get { return m_ColorSets[17][0]; } }
        public static Color blueGrey100 { get { return m_ColorSets[17][1]; } }
        public static Color blueGrey200 { get { return m_ColorSets[17][2]; } }
        public static Color blueGrey300 { get { return m_ColorSets[17][3]; } }
        public static Color blueGrey400 { get { return m_ColorSets[17][4]; } }
        public static Color blueGrey500 { get { return m_ColorSets[17][5]; } }
        public static Color blueGrey600 { get { return m_ColorSets[17][6]; } }
        public static Color blueGrey700 { get { return m_ColorSets[17][7]; } }
        public static Color blueGrey800 { get { return m_ColorSets[17][8]; } }
        public static Color blueGrey900 { get { return m_ColorSets[17][9]; } }

        public static Color[] greySet { get { return m_ColorSets[18]; } }
        public static Color grey50 { get { return m_ColorSets[18][0]; } }
        public static Color grey100 { get { return m_ColorSets[18][1]; } }
        public static Color grey200 { get { return m_ColorSets[18][2]; } }
        public static Color grey300 { get { return m_ColorSets[18][3]; } }
        public static Color grey400 { get { return m_ColorSets[18][4]; } }
        public static Color grey500 { get { return m_ColorSets[18][5]; } }
        public static Color grey600 { get { return m_ColorSets[18][6]; } }
        public static Color grey700 { get { return m_ColorSets[18][7]; } }
        public static Color grey800 { get { return m_ColorSets[18][8]; } }
        public static Color grey900 { get { return m_ColorSets[18][9]; } }

        #endregion

        public static Color textDark { get { return new Color(0f, 0f, 0f, 0.87f); } }
        public static Color textLight { get { return new Color(1f, 1f, 1f, 1f); } }

        public static Color textSecondaryDark { get { return new Color(0f, 0f, 0f, 0.54f); } }
        public static Color textSecondaryLight { get { return new Color(1f, 1f, 1f, 0.7f); } }

        public static Color textHintDark { get { return new Color(0f, 0f, 0f, 0.26f); } }
        public static Color textHintLight { get { return new Color(1f, 1f, 1f, 0.3f); } }

        public static Color iconDark { get { return new Color(0f, 0f, 0f, 0.54f); } }
        public static Color iconLight { get { return new Color(1f, 1f, 1f, 1f); } }

        public static Color disabledDark { get { return new Color(0f, 0f, 0f, 0.26f); } }
        public static Color disabledLight { get { return new Color(1f, 1f, 1f, 0.3f); } }

        public static Color dividerDark { get { return new Color(0f, 0f, 0f, 0.12f); } }
        public static Color dividerLight { get { return new Color(1f, 1f, 1f, 0.12f); } }
    }
}