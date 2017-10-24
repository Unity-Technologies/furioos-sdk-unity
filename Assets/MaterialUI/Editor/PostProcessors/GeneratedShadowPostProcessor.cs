//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

namespace MaterialUI
{
    /// <summary>
    /// Used when a new texture is created by the
    /// shadow generator. It turns the texture into a sprite and
    /// applies the right settings to apply to an image.
    /// </summary>
    public class GeneratedShadowPostProcessor : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (assetPath.Contains(ShadowGenerator.generatedShadowsDirectory))
            {
                TextureImporter importer = assetImporter as TextureImporter;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.generateCubemap = TextureImporterGenerateCubemap.None;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.wrapMode = TextureWrapMode.Clamp;
                if (ShadowGenerator.ShadowSpriteBorder != null)
                {
                    importer.spriteBorder = (Vector4)ShadowGenerator.ShadowSpriteBorder;
                    ShadowGenerator.ShadowSpriteBorder = null;
                }
                importer.mipmapEnabled = false;
                importer.textureType = TextureImporterType.Sprite;

                Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
                if (asset)
                {
                    EditorUtility.SetDirty(asset);
                }
                else
                {
                    importer.textureType = TextureImporterType.Default;
                }
            }
        }
    }
}

#endif