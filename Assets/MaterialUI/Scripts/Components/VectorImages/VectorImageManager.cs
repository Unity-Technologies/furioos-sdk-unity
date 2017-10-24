//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.IO;
#if UNITY_5_2
using LitJson;
#endif
using UnityEngine;

namespace MaterialUI
{
    public static class VectorImageManager
    {
        public static int currentPack;

        public const string materialDesignIconsFolderPath = "Assets/MaterialUI/Fonts/Resources/Material Design Icons";

        public const string materialUIIconsFontName = "MaterialUI Icons";
        public const string materialDesignIconsFontName = "Material Design Icons";

		private const string prefFontDestinationFolder = "PREF_FONT_ICON_DESTINATION_FOLDER";
		public static string fontDestinationFolder
		{
			get { return PlayerPrefs.GetString(prefFontDestinationFolder, "/Vector Fonts") + "/Resources"; }
			set
			{
				PlayerPrefs.SetString(prefFontDestinationFolder, value);
				PlayerPrefs.Save();
			}
		}

        private static bool FontDirExists()
        {
            return Directory.Exists(Application.dataPath + "/" + fontDestinationFolder);
        }

        private static string[] GetFontDirectories(string fontsPath)
        {
            if (FontDirExists())
            {
                return Directory.GetDirectories(fontsPath);
            }
            return new string[0];
        }

        public static string[] GetAllIconSetNames()
        {
            string fontsPath = Application.dataPath + "/" + fontDestinationFolder;

            string[] fontStringNames = GetFontDirectories(fontsPath);

            string[] fontStrings = new string[fontStringNames.Length + 2];

            fontStrings[0] = materialUIIconsFontName;
            fontStrings[1] = materialDesignIconsFontName;

            for (int i = 0; i < fontStringNames.Length; i++)
            {
                fontStrings[i + 2] = new DirectoryInfo(fontStringNames[i].Replace("\\", "/")).Name;
            }
            return fontStrings;
        }

        public static bool IsMaterialDesignIconFont(string fontName)
        {
            return fontName == materialDesignIconsFontName;
        }

        public static bool IsMaterialUIIconFont(string fontName)
        {
            return fontName == materialUIIconsFontName;
        }

        public static Font GetIconFont(string name)
        {
            return Resources.Load<Font>(name + "/" + name);
        }

        public static VectorImageSet GetIconSet(string name)
        {
			#if UNITY_5_2
			string codePointJson = Resources.Load<TextAsset>(name + "/" + name).text;
			codePointJson = codePointJson.Replace("m_Name", "name").Replace("m_Unicode", "unicode").Replace("m_IconGlyphList", "iconGlyphList");
			return JsonMapper.ToObject<VectorImageSet>(codePointJson);
			#else
			return JsonUtility.FromJson<VectorImageSet>(Resources.Load<TextAsset>(name + "/" + name).text);
			#endif
        }

        public static string GetIconCodeFromName(string name, string setName = "*")
        {
            bool noPackSpecified = (setName == "*");

            if (noPackSpecified)
            {
                string[] setNames = GetAllIconSetNames();
                VectorImageSet[] sets = new VectorImageSet[setNames.Length];

                for (int i = 0; i < setNames.Length; i++)
                {
                    sets[i] = GetIconSet(setNames[i]);
                }

                for (int i = 0; i < sets.Length; i++)
                {
                    for (int j = 0; j < sets[i].iconGlyphList.Count; j++)
                    {
                        if (name == sets[i].iconGlyphList[j].name)
                        {
                            return sets[i].iconGlyphList[j].unicode;
                        }
                    }
                }
            }
            else
            {
                VectorImageSet set = GetIconSet(setName);

                for (int j = 0; j < set.iconGlyphList.Count; j++)
                {
                    if (name == set.iconGlyphList[j].name)
                    {
                        return set.iconGlyphList[j].unicode;
                    }
                }
            }
            return null;
        }

        public static string GetIconNameFromCode(string code, string setName = "*")
        {
            bool noPackSpecified = (setName == "*");

            if (noPackSpecified)
            {
                string[] setNames = GetAllIconSetNames();
                VectorImageSet[] sets = new VectorImageSet[setNames.Length];

                for (int i = 0; i < setNames.Length; i++)
                {
                    sets[i] = GetIconSet(setNames[i]);
                }

                for (int i = 0; i < sets.Length; i++)
                {
                    for (int j = 0; j < sets[i].iconGlyphList.Count; j++)
                    {
                        if (code == sets[i].iconGlyphList[j].unicode)
                        {
                            return sets[i].iconGlyphList[j].name;
                        }
                    }
                }
            }
            else
            {
                VectorImageSet set = GetIconSet(setName);

                for (int j = 0; j < set.iconGlyphList.Count; j++)
                {
                    if (code == set.iconGlyphList[j].unicode)
                    {
                        return set.iconGlyphList[j].name;
                    }
                }
            }
            return null;
        }
    }
}