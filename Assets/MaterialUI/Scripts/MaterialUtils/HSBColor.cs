//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

//  Credit to author Jonathan Czeck (aarku)
//	See more here: http://wiki.unity3d.com/index.php?title=HSBColor

using System;
using UnityEngine;

namespace MaterialUI
{
    /// <summary>
    /// Struct that contains color data in (hue, saturation, brightness) format.
    /// </summary>
    [Serializable]
    public struct HSBColor
    {
        /// <summary>
        /// The hue level.
        /// </summary>
        private float m_H;
        /// <summary>
        /// The saturation level.
        /// </summary>
        private float m_S;
        /// <summary>
        /// The brightness level.
        /// </summary>
        private float m_B;
        /// <summary>
        /// The alpha level.
        /// </summary>
        private float m_A;

        /// <summary>
        /// Initializes a new instance of the <see cref="HSBColor"/> struct.
        /// </summary>
        /// <param name="h">The hue.</param>
        /// <param name="s">The saturation.</param>
        /// <param name="b">The brightness.</param>
        /// <param name="a">The alpha.</param>
        public HSBColor(float h, float s, float b, float a)
        {
            m_H = h;
            m_S = s;
            m_B = b;
            m_A = a;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HSBColor"/> struct.
        /// </summary>
        /// <param name="h">The hue.</param>
        /// <param name="s">The saturation.</param>
        /// <param name="b">The brightness.</param>
        public HSBColor(float h, float s, float b)
        {
            m_H = h;
            m_S = s;
            m_B = b;
            m_A = 1f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HSBColor"/> struct.
        /// </summary>
        /// <param name="color">The Color to get color values from.</param>
        public HSBColor(Color color)
        {
            HSBColor temp = FromColor(color);
            m_H = temp.m_H;
            m_S = temp.m_S;
            m_B = temp.m_B;
            m_A = temp.m_A;
        }

        /// <summary>
        /// The hue level.
        /// </summary>
        public float h
        {
            get { return m_H; }
            set { m_H = value; }
        }

        /// <summary>
        /// The saturation level.
        /// </summary>
        public float s
        {
            get { return m_S; }
            set { m_S = value; }
        }

        /// <summary>
        /// The brightness level.
        /// </summary>
        public float b
        {
            get { return m_B; }
            set { m_B = value; }
        }

        /// <summary>
        /// The alpha level.
        /// </summary>
        public float a
        {
            get { return m_A; }
            set { m_A = value; }
        }

        /// <summary>
        /// Initializes a new HSBColor from a Color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The initialized HSBColor.</returns>
        public static HSBColor FromColor(Color color)
        {
            HSBColor ret = new HSBColor(0f, 0f, 0f, color.a);

            float r = color.r;
            float g = color.g;
            float b = color.b;

            float max = Mathf.Max(r, Mathf.Max(g, b));

            if (max <= 0)
            {
                return ret;
            }

            float min = Mathf.Min(r, Mathf.Min(g, b));
            float dif = max - min;

            if (max > min)
            {
                if (g == max)
                {
                    ret.m_H = (b - r) / dif * 60f + 120f;
                }
                else if (b == max)
                {
                    ret.m_H = (r - g) / dif * 60f + 240f;
                }
                else if (b > g)
                {
                    ret.m_H = (g - b) / dif * 60f + 360f;
                }
                else
                {
                    ret.m_H = (g - b) / dif * 60f;
                }
                if (ret.m_H < 0)
                {
                    ret.m_H = ret.m_H + 360f;
                }
            }
            else
            {
                ret.m_H = 0;
            }

            ret.m_H *= 1f / 360f;
            ret.m_S = (dif / max) * 1f;
            ret.m_B = max;

            return ret;
        }

        /// <summary>
        /// Initializes a Color from a HSBColor.
        /// </summary>
        /// <param name="hsbColor">The HSBColor.</param>
        /// <returns>The initialized Color.</returns>
        public static Color ToColor(HSBColor hsbColor)
        {
            float r = hsbColor.m_B;
            float g = hsbColor.m_B;
            float b = hsbColor.m_B;

            if (hsbColor.m_S != 0)
            {
                float max = hsbColor.m_B;
                float dif = hsbColor.m_B * hsbColor.m_S;
                float min = hsbColor.m_B - dif;

                float h = hsbColor.m_H * 360f;

                if (h < 60f)
                {
                    r = max;
                    g = h * dif / 60f + min;
                    b = min;
                }
                else if (h < 120f)
                {
                    r = -(h - 120f) * dif / 60f + min;
                    g = max;
                    b = min;
                }
                else if (h < 180f)
                {
                    r = min;
                    g = max;
                    b = (h - 120f) * dif / 60f + min;
                }
                else if (h < 240f)
                {
                    r = min;
                    g = -(h - 240f) * dif / 60f + min;
                    b = max;
                }
                else if (h < 300f)
                {
                    r = (h - 240f) * dif / 60f + min;
                    g = min;
                    b = max;
                }
                else if (h <= 360f)
                {
                    r = max;
                    g = min;
                    b = -(h - 360f) * dif / 60 + min;
                }
                else
                {
                    r = 0;
                    g = 0;
                    b = 0;
                }
            }

            return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.m_A);
        }

        /// <summary>
        /// Converts the HSBColor to Color.
        /// </summary>
        /// <returns>The initialized Color.</returns>
        public Color ToColor()
        {
            return ToColor(this);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "H:" + m_H + " S:" + m_S + " B:" + m_B;
        }

        /// <summary>
        /// Lerps between 2 HSBColors over value t.
        /// </summary>
        /// <param name="a">The first HSBColor.</param>
        /// <param name="b">The second HSBColor.</param>
        /// <param name="t">The t value. Closer to 0 will result in a color closer to a, and a value closer to 1 will result in a color closer to b.</param>
        /// <returns>The lerped Color.</returns>
        public static HSBColor Lerp(HSBColor a, HSBColor b, float t)
        {
            float h, s;

            //check special case black (Color.b==0): interpolate neither hue nor saturation!
            //check special case grey (Color.s==0): don't interpolate hue!
            if (a.m_B == 0)
            {
                h = b.m_H;
                s = b.m_S;
            }
            else if (b.m_B == 0)
            {
                h = a.m_H;
                s = a.m_S;
            }
            else
            {
                if (a.m_S == 0)
                {
                    h = b.m_H;
                }
                else if (b.m_S == 0)
                {
                    h = a.m_H;
                }
                else
                {
                    // works around bug with LerpAngle
                    float angle = Mathf.LerpAngle(a.m_H * 360f, b.m_H * 360f, t);
                    while (angle < 0f)
                    {
                        angle += 360f;
                    }
                    while (angle > 360f)
                    {
                        angle -= 360f;
                    }
                    h = angle / 360f;
                }
                s = Mathf.Lerp(a.m_S, b.m_S, t);
            }
            return new HSBColor(h, s, Mathf.Lerp(a.m_B, b.m_B, t), Mathf.Lerp(a.m_A, b.m_A, t));
        }
    }
}