//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;
#if UNITY_5_2
using LitJson;
#endif
using System;
using System.Collections;
using System.IO;

namespace MaterialUI
{
    public abstract class VectorImageFontParser
    {
        protected abstract string GetIconFontUrl();
        protected abstract string GetIconFontLicenseUrl();
        protected abstract string GetIconFontDataUrl();
        public abstract string GetFontName();
        public abstract string GetWebsite();
        protected abstract VectorImageSet GenerateIconSet(string fontDataContent);
        protected abstract string ExtractLicense(string fontLicenseDataContent);
        protected virtual void CleanUp() { }

        private VectorImageSet m_CachedVectorImageSet;
        private Action m_OnDoneDownloading;

        public void DownloadIcons(Action onDoneDownloading = null)
        {
            this.m_OnDoneDownloading = onDoneDownloading;
            EditorCoroutine.Start(DownloadIconFontCoroutine());
        }

        private IEnumerator DownloadIconFontCoroutine()
        {
            string iconFontURL = GetIconFontUrl();
            if (string.IsNullOrEmpty(iconFontURL))
            {
                yield break;
            }

            WWW www = new WWW(iconFontURL);
            while (!www.isDone)
            {
                yield return null;
            }

            if (!string.IsNullOrEmpty(www.error))
            {
                ClearProgressBar();
				throw new Exception("Error downloading icon font (" + GetFontName() + ") at path: " + GetIconFontUrl() + " - " + www.error);
            }

            CreateFolderPath();

            File.WriteAllBytes(GetIconFontPath(), www.bytes);
			EditorCoroutine.Start(DownloadFontLicenseCoroutine());
        }

        private IEnumerator DownloadFontLicenseCoroutine()
        {
            if (!string.IsNullOrEmpty(GetIconFontLicenseUrl()))
            {
                WWW www = new WWW(GetIconFontLicenseUrl());
                while (!www.isDone)
                {
                    yield return null;
                }

                if (!string.IsNullOrEmpty(www.error))
                {
                    ClearProgressBar();
                    throw new Exception("Error downloading icon font license (" + GetFontName() + ") at path: " + GetIconFontLicenseUrl());
                }

                CreateFolderPath();

                string licenseData = ExtractLicense(www.text);

                File.WriteAllText(GetIconFontLicensePath(), licenseData);
            }

			EditorCoroutine.Start(DownloadIconFontData());
        }

        private IEnumerator DownloadIconFontData()
        {
            WWW www = new WWW(GetIconFontDataUrl());
            while (!www.isDone)
            {
                yield return null;
            }

            if (!string.IsNullOrEmpty(www.error))
            {
                ClearProgressBar();
                throw new Exception("Error downloading icon font data (" + GetFontName() + ") at path: " + GetIconFontDataUrl() + "\n" + www.error);
            }

            CreateFolderPath();

            VectorImageSet vectorImageSet = GenerateIconSet(www.text);
            FormatNames(vectorImageSet);

			#if UNITY_5_2
			string codePointJson = JsonMapper.ToJson(vectorImageSet);
			codePointJson = codePointJson.Replace("name", "m_Name").Replace("unicode", "m_Unicode").Replace("iconGlyphList", "m_IconGlyphList");
			#else
			string codePointJson = JsonUtility.ToJson(vectorImageSet);
			#endif

            File.WriteAllText(GetIconFontDataPath(), codePointJson);

            if (m_OnDoneDownloading != null)
            {
                m_OnDoneDownloading();
            }

            CleanUp();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void ClearProgressBar()
        {
            EditorUtility.ClearProgressBar();
        }

        private void CreateFolderPath()
        {
            if (!Directory.Exists(GetFolderPath()))
            {
                Directory.CreateDirectory(GetFolderPath());
            }
        }

        public string GetFolderPath()
        {
			string path = Application.dataPath + "/" + VectorImageManager.fontDestinationFolder + "/" + GetFontName() + "/";

            if (VectorImageManager.IsMaterialDesignIconFont(GetFontName()))
            {
                path = VectorImageManager.materialDesignIconsFolderPath + "/";
            }

            path = path.Replace("//", "/");
            return path;
        }

        private string GetIconFontPath()
        {
            return GetFolderPath() + GetFontName() + ".ttf";
        }

        public string GetIconFontLicensePath()
        {
            return GetFolderPath() + "LICENSE.txt";
        }

        private string GetIconFontDataPath()
        {
            return GetFolderPath() + GetFontName() + ".json";
        }

        public bool IsFontAvailable()
        {
            bool isFontAvailable = File.Exists(GetIconFontPath());
            bool isFontDataAvailable = File.Exists(GetIconFontDataPath());

            return isFontAvailable && isFontDataAvailable;
        }

        private void FormatNames(VectorImageSet set)
        {
            for (int i = 0; i < set.iconGlyphList.Count; i++)
            {
                string name = set.iconGlyphList[i].name;
                name = name.Replace("-", "_");
                name = name.Replace(" ", "_");
                name = name.ToLower();
				set.iconGlyphList[i].name = name;

				string unicode = set.iconGlyphList[i].unicode;
				unicode = unicode.Replace("\\u", "");
				unicode = unicode.Replace("\\\\u", "");
				set.iconGlyphList[i].unicode = unicode;
            }
        }

        public VectorImageSet GetIconSet()
        {
            if (!IsFontAvailable())
            {
                throw new Exception("Can't get the icon set because the font has not been downloaded");
            }

			#if UNITY_5_2
			string iconFontData = File.ReadAllText(GetIconFontDataPath());
			iconFontData = iconFontData.Replace("m_Name", "name").Replace("m_Unicode", "unicode").Replace("m_IconGlyphList", "iconGlyphList");
			VectorImageSet vectorImageSet = JsonMapper.ToObject<VectorImageSet>(iconFontData);
			#else
			VectorImageSet vectorImageSet = JsonUtility.FromJson<VectorImageSet>(File.ReadAllText(GetIconFontDataPath()));
			#endif
            
            return vectorImageSet;
		}

		public VectorImageSet GetCachedIconSet()
		{
			if (m_CachedVectorImageSet == null)
			{
				m_CachedVectorImageSet = GetIconSet();
			}

			return m_CachedVectorImageSet;
		}

		public void Delete()
		{
			string path = GetFolderPath();

			// Delete folder
			Directory.Delete(path, true);

			// Sync AssetDatabase with the delete operation.
			string metaPath = path.Substring(Application.dataPath.IndexOf("/Assets") + 1);
			if (metaPath.EndsWith("/"))
			{
				metaPath = metaPath.Substring(0, metaPath.Length - 1);
			}

			AssetDatabase.DeleteAsset(metaPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
    }
}
