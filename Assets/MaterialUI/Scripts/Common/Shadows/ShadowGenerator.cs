//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

namespace MaterialUI
{
    /// <summary>
    /// Component that generates a shadow image based on an existing Image component.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [ExecuteInEditMode]
    public class ShadowGenerator : MonoBehaviour
    {
        /// <summary>
        /// The shadow darken amount.
        /// Very sensitive, be careful if ever changing this.
        /// </summary>
        private const float shadowDarkenAmount = 0.97f;

        /// <summary>
        /// The unique identifier of the generated shadow.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private string m_Guid;

        /// <summary>
        /// The generated shadows directory.
        /// </summary>
        [SerializeField]
        private static string m_GeneratedShadowsDirectory = "GeneratedShadows";
        /// <summary>
        /// The generated shadows directory.
        /// </summary>
        public static string generatedShadowsDirectory
        {
            get { return m_GeneratedShadowsDirectory; }
            set { m_GeneratedShadowsDirectory = value; }
        }

        /// <summary>
        /// Has the shadow finished generating?
        /// </summary>
        private bool m_IsDone;
        /// <summary>
        /// Has the shadow finished generating?
        /// </summary>
        public bool isDone
        {
            get { return m_IsDone; }
            set { m_IsDone = value; }
        }
        
        /// <summary>
        /// The image that the shadow will be made from.
        /// </summary>
        private Image m_SourceImage;
        /// <summary>
        /// The image that the shadow will be made from.
        /// </summary>
        public Image sourceImage
        {
            get { return m_SourceImage; }
            set { m_SourceImage = value; }
        }
        
        /// <summary>
        /// The pixel range of each blur pass.
        /// </summary>
        [Range(0, 5)]
        [SerializeField]
        private int m_BlurRange = 2;
        /// <summary>
        /// The pixel range of each blur pass.
        /// </summary>
        public int blurRange
        {
            get { return m_BlurRange; }
            set { m_BlurRange = value; }
        }
        
        /// <summary>
        /// Number of blur passes.
        /// </summary>
        private int m_BlurIterations = 2;
        /// <summary>
        /// Number of blur passes.
        /// </summary>
        public int blurIterations
        {
            get { return m_BlurIterations; }
            set { m_BlurIterations = value; }
        }
        
        /// <summary>
        /// The specified position of the shadow, relative to the source image.
        /// </summary>
        private Vector3 m_ShadowRelativePosition = new Vector3(0f, -1f, 0f);
        /// <summary>
        /// The specified position of the shadow, relative to the source image.
        /// </summary>
        public Vector3 shadowRelativePosition
        {
            get { return m_ShadowRelativePosition; }
            set { m_ShadowRelativePosition = value; }
        }

        /// <summary>
        /// The specified size of the shadow, relative to the source image.
        /// </summary>
        private Vector2 m_ShadowRelativeSize = new Vector2(-2f, -2f);
        /// <summary>
        /// The specified size of the shadow, relative to the source image.
        /// </summary>
        public Vector2 shadowRelativeSize
        {
            get { return m_ShadowRelativeSize; }
            set { m_ShadowRelativeSize = value; }
        }

        /// <summary>
        /// The specified alpha level of the shadow.
        /// </summary>
        [Range(0, 1)]
        [SerializeField]
        private float m_ShadowAlpha = 0.5f;
        /// <summary>
        /// The specified alpha level of the shadow.
        /// </summary>
        public float shadowAlpha
        {
            get { return m_ShadowAlpha; }
            set { m_ShadowAlpha = value; }
        }

        /// <summary>
        /// Default size to add to Texture to fit shadow blur. If you know for sure you won't be using large shadows, you can lower this for performance.
        /// </summary>
        [Tooltip("Default size to add to Texture to fit shadow blur. If you know for sure you won't be using large shadows, you can lower this for performance")]
        [SerializeField]
        private int m_ImagePadding = 32;
        /// <summary>
        /// Default size to add to Texture to fit shadow blur. If you know for sure you won't be using large shadows, you can lower this for performance.
        /// </summary>
        public int imagePadding
        {
            get { return m_ImagePadding; }
            set { m_ImagePadding = value; }
        }
        
        /// <summary>
        /// The sprite of the source Image.
        /// </summary>
        private Sprite m_SourceSprite;
        /// <summary>
        /// The sprite of the source Image.
        /// </summary>
        public Sprite sourceSprite
        {
            get { return m_SourceSprite; }
            set { m_SourceSprite = value; }
        }

        /// <summary>
        /// The texture of the sprite of the source Image.
        /// </summary>
        private Texture2D m_SourceTex;
        /// <summary>
        /// The texture of the sprite of the source Image.
        /// </summary>
        public Texture2D sourceTex
        {
            get { return m_SourceTex; }
            set { m_SourceTex = value; }
        }

        /// <summary>
        /// The texture of the sprite of the created shadow Image.
        /// </summary>
        private Texture2D m_DestTex;
        /// <summary>
        /// The texture of the sprite of the created shadow Image.
        /// </summary>
        public Texture2D destTex
        {
            get { return m_DestTex; }
            set { m_DestTex = value; }
        }

        /// <summary>
        /// The sprite of the created shadow Image.
        /// </summary>
        private Sprite m_DestSprite;
        /// <summary>
        /// The sprite of the created shadow Image.
        /// </summary>
        public Sprite destSprite
        {
            get { return m_DestSprite; }
            set { m_DestSprite = value; }
        }

        /// <summary>
        /// The sprite of the created shadow Image.
        /// </summary>
        private Image m_DestImage;
        /// <summary>
        /// The sprite of the created shadow Image.
        /// </summary>
        public Image destImage
        {
            get { return m_DestImage; }
            set { m_DestImage = value; }
        }
        
        /// <summary>
        /// The file name of the generated texture.
        /// </summary>
        private string m_TextureFileName;
        /// <summary>
        /// The shadow sprite border.
        /// </summary>
        public static Vector4? ShadowSpriteBorder;
        /// <summary>
        /// The directory (and name) of the generated shadow.
        /// </summary>
        private string m_ShadowDir;

        /// <summary>
        /// See Monobehaviour.Start.
        /// </summary>
        void Start()
        {
            //	For some reason, the shadow image loses the sprite reference when play mode is entered. This re-assigns it.
            if (destSprite)
            {
                destImage.sprite = destSprite;
            }
        }

        /// <summary>
        /// Generates the shadow from the source Image.
        /// </summary>
        public void GenerateShadowFromImage()
        {
            isDone = false;

            //	Gets the source Image's Sprite's Texture to use
            destImage = gameObject.GetComponent<Image>();
            if (!destImage)
            {
                destImage = gameObject.AddComponent<Image>();
            }
            sourceSprite = sourceImage.sprite;

            if (sourceSprite != null)
            {
                Texture sourceOriginalTexture = sourceSprite.texture;
                sourceOriginalTexture.hideFlags = HideFlags.HideAndDontSave;

                //	We can't just use sourceOriginalTexture, as it may not be read/write enabled.
                //	Instead, we copy it using the filepath and WWW.LoadImageIntoTexture
                string path = "file://" + Application.dataPath.Replace("Assets", "");
                path += AssetDatabase.GetAssetPath(sourceOriginalTexture);
                WWW www = new WWW(path);
                ContinuationManager.Add(() => www.isDone, () =>
                {
                    sourceTex = new Texture2D(sourceOriginalTexture.width, sourceOriginalTexture.height);
                    sourceTex.hideFlags = HideFlags.HideAndDontSave;
                    www.LoadImageIntoTexture(sourceTex);
                    SetupFile();
                });
            }
            else
            {
                sourceTex = new Texture2D(8, 8);
                int yCounter = 0;
                int xCounter = 0;
                while (xCounter < sourceTex.width)
                {
                    while (yCounter < sourceTex.height)
                    {
                        sourceTex.SetPixel(xCounter, yCounter, Color.black);
                        yCounter++;
                    }

                    xCounter++;
                    yCounter = 0;
                }
                sourceTex.Apply();
                SetupFile();
            }
        }

        /// <summary>
        /// Prepares a file of the shadow texture to be saved.
        /// </summary>
        private void SetupFile()
        {
            m_ShadowDir = "Assets/" + generatedShadowsDirectory;

            //	Create shadow sprite directory if none exists
            if (!Directory.Exists(m_ShadowDir))
            {
                Directory.CreateDirectory(m_ShadowDir);
            }

            //	Failsafe
            if (sourceImage == null)
            {
                Debug.LogWarning("Must have source image");
                isDone = true;
                return;
            }

            //	If the shadow Image is not correctly assigned, remove the texture to create new shadow image
            if (!destImage.sprite)
            {
                if (destTex)
                {
                    DestroyImmediate(destTex);
                }
            }

            //	Check to see if the shadow is already using an image - if so, overwrite it - if not, prepare to create a new one
            if (!AssetDatabase.LoadAssetAtPath(string.Format(m_ShadowDir + "/{0}.png", m_Guid), typeof(Sprite)))
            {
                m_Guid = Guid.NewGuid().ToString();
            }

            m_TextureFileName = string.Format(m_ShadowDir + "/{0}.png", m_Guid);

            //	Calls the functions for each stage of the shadow generation process
            Setup();
            Darken();
            Blur();
            ApplyChanges();
            DestroyImmediate(sourceTex);
            DestroyImmediate(destTex);
            isDone = true;
        }

        /// <summary>
        /// Sets up the shadow's texture.
        /// </summary>
        private void Setup()
        {
            //	Creates a new texture for the shadow that is bigger to accommodate the shadow blur
            int widthWithPadding = sourceTex.width + imagePadding * 2;
            int heightWithPadding = sourceTex.height + imagePadding * 2;

            destTex = new Texture2D(widthWithPadding, heightWithPadding, TextureFormat.RGBA32, false);
            destTex.hideFlags = HideFlags.HideAndDontSave;
            destTex.filterMode = FilterMode.Trilinear;
            destTex.wrapMode = TextureWrapMode.Clamp;

            //	Makes the entire shadow image fully-transparent, pixel-by-pixel (As a newly-created texture isn't, for some strange reason)
            int yCounter = 0;
            int xCounter = 0;
            Color pixCol = new Color(0, 0, 0, 0);
            while (xCounter < destTex.width)
            {
                while (yCounter < destTex.height)
                {
                    destTex.SetPixel(xCounter, yCounter, pixCol);
                    yCounter++;
                }
                xCounter++;
                yCounter = 0;
            }
        }

        /// <summary>
        /// Copies each pixel from the source texture to the destination texture, and darkens them.
        /// </summary>
        private void Darken()
        {
            int yCounter = 0;
            int xCounter = 0;
            Color pixCol = new Color(0, 0, 0, 0);

            while (xCounter < sourceTex.width)
            {
                while (yCounter < sourceTex.height)
                {
                    pixCol.a = sourceTex.GetPixel(xCounter, yCounter).a;
                    destTex.SetPixel(xCounter + imagePadding, yCounter + imagePadding, pixCol);
                    yCounter++;
                }

                xCounter++;
                yCounter = 0;
            }
        }

        /// <summary>
        /// Blurs the shadow texture.
        /// </summary>
        private void Blur()
        {
            //	Iterates through each pixel of the shadow image and makes the color an average of the surrounding pixels (Radius is specified in editor)
            int i = 0;
            int xCounter = 0;
            int yCounter = 0;
            Color pixCol = new Color(0, 0, 0, 0);

            while (i < blurIterations)
            {
                while (xCounter < destTex.width)
                {
                    while (yCounter < destTex.height)
                    {
                        if (blurRange == 1)
                        {
                            pixCol.a = destTex.GetPixel(xCounter, yCounter - 1).a / 4 + destTex.GetPixel(xCounter, yCounter).a / 2 + destTex.GetPixel(xCounter, yCounter + 1).a / 4;
                            pixCol.r = 0;
                            pixCol.g = 0;
                            pixCol.b = 0;
                            destTex.SetPixel(xCounter, yCounter, pixCol / (shadowDarkenAmount));
                        }
                        else if (blurRange == 2)
                        {
                            pixCol = destTex.GetPixel(xCounter, yCounter - 2) + destTex.GetPixel(xCounter, yCounter - 1) + destTex.GetPixel(xCounter, yCounter) + destTex.GetPixel(xCounter, yCounter + 1) + destTex.GetPixel(xCounter, yCounter + 2);
                            destTex.SetPixel(xCounter, yCounter, pixCol / (shadowDarkenAmount * 5));
                        }
                        else if (blurRange == 3)
                        {
                            pixCol = destTex.GetPixel(xCounter, yCounter - 3) + destTex.GetPixel(xCounter, yCounter - 2) + destTex.GetPixel(xCounter, yCounter - 1) + destTex.GetPixel(xCounter, yCounter) + destTex.GetPixel(xCounter, yCounter + 1) + destTex.GetPixel(xCounter, yCounter + 2) + destTex.GetPixel(xCounter, yCounter + 3);
                            destTex.SetPixel(xCounter, yCounter, pixCol / (shadowDarkenAmount * 7));
                        }
                        else if (blurRange == 4)
                        {
                            pixCol = destTex.GetPixel(xCounter, yCounter - 4) + destTex.GetPixel(xCounter, yCounter - 3) + destTex.GetPixel(xCounter, yCounter - 2) + destTex.GetPixel(xCounter, yCounter - 1) + destTex.GetPixel(xCounter, yCounter) + destTex.GetPixel(xCounter, yCounter + 1) + destTex.GetPixel(xCounter, yCounter + 2) + destTex.GetPixel(xCounter, yCounter + 3) + destTex.GetPixel(xCounter, yCounter + 4);
                            destTex.SetPixel(xCounter, yCounter, pixCol / (shadowDarkenAmount * 9));
                        }
                        else if (blurRange == 5)
                        {
                            pixCol = destTex.GetPixel(xCounter, yCounter - 5) + destTex.GetPixel(xCounter, yCounter - 4) + destTex.GetPixel(xCounter, yCounter - 3) + destTex.GetPixel(xCounter, yCounter - 2) + destTex.GetPixel(xCounter, yCounter - 1) + destTex.GetPixel(xCounter, yCounter) + destTex.GetPixel(xCounter, yCounter + 1) + destTex.GetPixel(xCounter, yCounter + 2) + destTex.GetPixel(xCounter, yCounter + 3) + destTex.GetPixel(xCounter, yCounter + 4) + destTex.GetPixel(xCounter, yCounter + 5);
                            destTex.SetPixel(xCounter, yCounter, pixCol / (shadowDarkenAmount * 11));
                        }
                        yCounter++;
                    }
                    xCounter++;
                    yCounter = 0;
                }

                xCounter = 0;
                yCounter = 0;

                while (xCounter < destTex.width)
                {
                    while (yCounter < destTex.height)
                    {
                        if (blurRange == 1)
                        {
                            pixCol.a = destTex.GetPixel(xCounter - 1, yCounter).a / 4 + destTex.GetPixel(xCounter, yCounter).a / 2 + destTex.GetPixel(xCounter + 1, yCounter).a / 4;
                            pixCol.r = 0;
                            pixCol.g = 0;
                            pixCol.b = 0;
                            destTex.SetPixel(xCounter, yCounter, pixCol / (shadowDarkenAmount));
                        }
                        else if (blurRange == 2)
                        {
                            pixCol = destTex.GetPixel(xCounter - 2, yCounter) + destTex.GetPixel(xCounter - 1, yCounter) + destTex.GetPixel(xCounter, yCounter) + destTex.GetPixel(xCounter + 1, yCounter) + destTex.GetPixel(xCounter + 2, yCounter);
                            destTex.SetPixel(xCounter, yCounter, pixCol / (shadowDarkenAmount * 5));
                        }
                        else if (blurRange == 3)
                        {
                            pixCol = destTex.GetPixel(xCounter - 3, yCounter) + destTex.GetPixel(xCounter - 2, yCounter) + destTex.GetPixel(xCounter - 1, yCounter) + destTex.GetPixel(xCounter, yCounter) + destTex.GetPixel(xCounter + 1, yCounter) + destTex.GetPixel(xCounter + 2, yCounter) + destTex.GetPixel(xCounter + 3, yCounter);
                            destTex.SetPixel(xCounter, yCounter, pixCol / (shadowDarkenAmount * 7));
                        }
                        else if (blurRange == 4)
                        {
                            pixCol = destTex.GetPixel(xCounter - 4, yCounter) + destTex.GetPixel(xCounter - 3, yCounter) + destTex.GetPixel(xCounter - 2, yCounter) + destTex.GetPixel(xCounter - 1, yCounter) + destTex.GetPixel(xCounter, yCounter) + destTex.GetPixel(xCounter + 1, yCounter) + destTex.GetPixel(xCounter + 2, yCounter) + destTex.GetPixel(xCounter + 3, yCounter) + destTex.GetPixel(xCounter + 4, yCounter);
                            destTex.SetPixel(xCounter, yCounter, pixCol / (shadowDarkenAmount * 9));
                        }
                        else if (blurRange == 5)
                        {
                            pixCol = destTex.GetPixel(xCounter - 5, yCounter) + destTex.GetPixel(xCounter - 4, yCounter) + destTex.GetPixel(xCounter - 3, yCounter) + destTex.GetPixel(xCounter - 2, yCounter) + destTex.GetPixel(xCounter - 1, yCounter) + destTex.GetPixel(xCounter, yCounter) + destTex.GetPixel(xCounter + 1, yCounter) + destTex.GetPixel(xCounter + 2, yCounter) + destTex.GetPixel(xCounter + 3, yCounter) + destTex.GetPixel(xCounter + 4, yCounter) + destTex.GetPixel(xCounter + 5, yCounter);
                            destTex.SetPixel(xCounter, yCounter, pixCol / (shadowDarkenAmount * 11));
                        }


                        yCounter++;
                    }
                    xCounter++;
                    yCounter = 0;
                }

                xCounter = 0;
                yCounter = 0;
                i++;
            }
        }

        /// <summary>
        /// Applies the changes to the shadow texture.
        /// </summary>
        private void ApplyChanges()
        {
            destTex.Apply();

            //	Encodes destTex as a PNG
            byte[] bytes = destTex.EncodeToPNG();

            //	Tells the texture importer to automatically import the images (PNG) as sprites
            if (sourceSprite)
            {
                ShadowSpriteBorder = new Vector4(sourceSprite.border.w + imagePadding, sourceSprite.border.x + imagePadding,
                    sourceSprite.border.y + imagePadding, sourceSprite.border.z + imagePadding);
            }
            else
            {
                ShadowSpriteBorder = new Vector4(imagePadding, imagePadding, imagePadding, imagePadding);
            }

            //	Saves destTex as a PNG in /Assets/ShadowGenerator/GeneratedShadows
            File.WriteAllBytes(m_TextureFileName, bytes);

            //	Safety net for the importer
            AssetDatabase.Refresh();

            //	References the newly-created and imported Sprite
            destSprite = AssetDatabase.LoadAssetAtPath(m_TextureFileName, typeof(Sprite)) as Sprite;

            //	Resizes, slices and assigns the Sprite
            destImage.rectTransform.sizeDelta = new Vector2(sourceImage.rectTransform.sizeDelta.x + imagePadding * 2, sourceImage.rectTransform.sizeDelta.y + imagePadding * 2);
            destImage.rectTransform.position = sourceImage.rectTransform.position;
            destImage.sprite = destSprite;
            if (sourceSprite)
            {
                destImage.type = sourceImage.type;
            }
            else
            {
                destImage.type = Image.Type.Sliced;
            }

            //	Positions the shadow Image
            Vector3 tempVec3 = destImage.rectTransform.position + shadowRelativePosition;
            destImage.rectTransform.position = tempVec3;

            //	Resizes the shadow Image
            Vector2 tempVec2 = destImage.rectTransform.sizeDelta + shadowRelativeSize;
            destImage.rectTransform.sizeDelta = tempVec2;

            //	Makes the shadow Image the desired transparency
            Color tempColor = destImage.color;
            tempColor.a = shadowAlpha;
            destImage.color = tempColor;
        }
    }
}
#endif