using System;
using System.Collections;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class AppSettingsForm : EditorWindow
{
    Texture2D textureThumbnail = null;
    Texture2D textureLogoFurioos = null;

    Vector2 _scrollPosition = new Vector2();
    int _selectedTab = 0;
    bool _showGlobalInformation = true;
    bool _showVirtualConfiguration = true;
    bool _showQuality = true;
    bool _showSettings = true;
    bool _showOtherSettings = true;
    bool _showOptionsNotSynchronisedWithPortal = false;
    bool _customRatioValid = true;

    string[] tabsName = new string[] { "Create your App", "API Authentication" };
    string _EXTENSION_AUTORIZED_ = ".jpg,.png,.gif,.jpeg";
    const long _MAX_SIZE_THUMBNAIL = 3 * 1024 * 1024;

    public static void ShowWindow()
    {
        Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Packages/com.unity.furioos-exporter/Editor/Ressources/U_Furioos_Logo_Small.png");
        GUIContent titleContent = new GUIContent("App Settings", icon);
        //var window = GetWindow<AppSettingsForm>("App Settings");

        var editorAsm = typeof(Editor).Assembly;
        var inspWndType = editorAsm.GetType("UnityEditor.InspectorWindow");
        var window = GetWindow<AppSettingsForm>("App Settings", inspWndType);
        window.titleContent = titleContent;
    }

    public void OnFocus()
    {
        FurioosApiHandler.CheckConnection();
        this.CheckValidApplicationOrPortalOptionsChanged();
        if (!FurioosSettings.isApiTokenValid)
        {
            this._selectedTab = 1;
        }
    }

    IEnumerator LoadTexture()
    {
        if (this.textureThumbnail == null)
        {
            var filePath = !String.IsNullOrEmpty(FsSettings.Current.ThumbnailFilePath) ? "file://" + FsSettings.Current.ThumbnailFilePath : FsSettings.Current.ThumbnailFileUrl;
           
            if (!String.IsNullOrEmpty(filePath))
            {
                using var www = UnityWebRequestTexture.GetTexture(filePath);
                yield return www.SendWebRequest();
                try
                {
                    this.textureThumbnail = DownloadHandlerTexture.GetContent(www);
                }
                catch
                {
                    this.textureThumbnail = null;
                }
               Repaint();
            }
        }

    }

    private async void CheckValidApplicationOrPortalOptionsChanged()
    {
        if (String.IsNullOrEmpty(FsSettings.Current.ApplicationID) || !FurioosSettings.isApiTokenValid)
        {
            return;
        }

        var fsApplication = await FurioosApiHandler.GetApplicationDetails(FsSettings.Current.ApplicationID);

        if (fsApplication == null)
        {
            EditorUtility.DisplayDialog("Settings error", "Your application hast not found. It might have been removed from Furioos\nPlease Build & Deploy again", "Ok");
            Debug.LogWarning("Application not found");
            FsSettings.Current.ApplicationID = "";
            FsSettings.Current.ApplicationBinaryID = "";
            FsSettings.Current.ThumbnailFilePath = "";
            FsSettings.Current.ThumbnailFileUrl = "";
            this.textureThumbnail = null;
            FsSettings.Current.SaveSettings();
            return;
        }
        else
        {
            _showOptionsNotSynchronisedWithPortal = !FsSettings.IsEqual(fsApplication) && !FsSettings.HasChanged();
        }
    }

    private void CheckCustomRatio()
    {
        _customRatioValid = true;
        if (FsSettings.Current.FixedRatioPresetSelected == FurioosSettings.FixedRatioValuesPreset.Length - 1)
        {
            _customRatioValid = false;
            var arrayRatio = FsSettings.Current.Ratio.Split(':');
            if (arrayRatio.Length != 2)
               return;
            int value;
            if( !int.TryParse(arrayRatio[0], out value) || !int.TryParse(arrayRatio[1], out value))
            {
                return;
            }
            _customRatioValid = true;
        }
    }

    void ShowNameField()
    {
        GUILayout.Space(4.0f);
        EditorGUILayout.BeginHorizontal();
        {
            FsSettings.Current.Name = EditorGUILayout.TextField("Application name", FsSettings.Current.Name, GUILayout.ExpandWidth(true));
        }
        GUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
    }

    void ShowDescriptionArea()
    {
        GUILayout.Space(4.0f);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Description", GUILayout.Width(135));
            EditorStyles.textArea.wordWrap = true;
            FsSettings.Current.Description = EditorGUILayout.TextArea(FsSettings.Current.Description);
        }
        GUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
    }

    void DrawSectionThumbnailSelection()
    {
        GUILayout.Space(4.0f);
        EditorGUILayout.BeginHorizontal();

        string fileName = FsSettings.Current.ThumbnailFilePath == "" ? FsSettings.Current.ThumbnailFileUrl : FsSettings.Current.ThumbnailFilePath;
        fileName = fileName == "" ? "No file selected" : fileName;

        EditorGUILayout.LabelField("Thumbnail", GUILayout.Width(135));
        EditorGUILayout.LabelField(new GUIContent(fileName, fileName), GUILayout.MinWidth(10));
        GUILayout.Space(4);
        var icon = EditorGUIUtility.IconContent("_Popup");
        if (GUILayout.Button(icon, GUILayout.Width(30)))
        {
            string thumbnailPath = EditorUtility.OpenFilePanel("Choose your thumbnail file", FsSettings.Current.ThumbnailFilePath, "png,gif,jpg,jpeg");
            if (!String.IsNullOrEmpty(thumbnailPath))
            {
                var length = new FileInfo(thumbnailPath).Length ;
                if (!_EXTENSION_AUTORIZED_.Contains(Path.GetExtension(thumbnailPath).ToLower()))
                {
                    EditorUtility.DisplayDialog("Thumbnail error", "Please choose an image file (png, jpg, gif) ", "Ok");
                } else if (length > _MAX_SIZE_THUMBNAIL)
                {
                    EditorUtility.DisplayDialog("Thumbnail error", "Your file is too large.\n Please use a file smaller than " + _MAX_SIZE_THUMBNAIL + " Mb", "Ok");
                } else
                {
                    FsSettings.Current.ThumbnailFilePath = thumbnailPath;
                }
                textureThumbnail = null;
            }
        }

        GUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
    }

    void DrawLogo()
    {
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            if (textureLogoFurioos == null)
            {
                textureLogoFurioos = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.unity.furioos-exporter/Editor/Ressources/U_Furioos_Logo_White_RGB.png", typeof(Texture2D));
            }
            if (textureLogoFurioos != null)
            {
                GUILayout.Label(textureLogoFurioos, GUILayout.MaxHeight(30));
            }
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);
        EditorGUI.indentLevel--;
    }

    void DrawTexture()
    {
        int leftMargin = 135;
        int rigthMargin = 24;

        EditorGUI.indentLevel++;
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(leftMargin);
            if (textureThumbnail != null)
            {
                float ratio = (textureThumbnail.width > textureThumbnail.height) ? (float)textureThumbnail.width/textureThumbnail.height : (float)textureThumbnail.height / textureThumbnail.width;

                int width = ((int)(position.width - leftMargin - rigthMargin));
                int height = (int)(width / ratio);
    
                GUILayout.Label(textureThumbnail, GUILayout.MaxHeight(width-10), GUILayout.MaxHeight(height));
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;
    }
    void DrawSectionGlobalInformation()
    {
        this._showGlobalInformation = EditorGUILayout.BeginFoldoutHeaderGroup(this._showGlobalInformation, "Informations");
        if (this._showGlobalInformation)
        {
            EditorGUI.indentLevel++;
            ShowNameField();
            ShowDescriptionArea();
            DrawSectionThumbnailSelection();
            DrawTexture();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    void DrawSectionVirtualMachineConfiguration()
    {
        this._showVirtualConfiguration = EditorGUILayout.BeginFoldoutHeaderGroup(this._showVirtualConfiguration, "Virtual Machine Configuration");
        if (this._showVirtualConfiguration)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Virtual machine", GUILayout.Width(135));
            FsSettings.Current.VirtualMachineSelected = EditorGUILayout.Popup(FsSettings.Current.VirtualMachineSelected, Enum.GetNames(typeof(FurioosSettings.VirtualMachineConfiguration)));

            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

    }

    void DrawSectionQualityConfiguration()
    {

        this._showQuality = EditorGUILayout.BeginFoldoutHeaderGroup(this._showQuality, "Quality and Ratio");
        if (this._showQuality)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Default quality", GUILayout.Width(135));
            var names = FurioosSettings.QualityConfiguration.Select(x => x.Item1).ToArray();
            FsSettings.Current.QualitySelected = EditorGUILayout.Popup(FsSettings.Current.QualitySelected, names);
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
            DrawSectionRatioConfiguration();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    void DrawSectionRatioConfiguration()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ratio mode", GUILayout.Width(135));
        FsSettings.Current.RatioModeSelected = EditorGUILayout.Popup(FsSettings.Current.RatioModeSelected, FurioosSettings.RatioModeValues);
        if (FsSettings.Current.RatioModeSelected == 0)
        {
            FsSettings.Current.FixedRatioPresetSelected = EditorGUILayout.Popup(FsSettings.Current.FixedRatioPresetSelected, FurioosSettings.FixedRatioValuesPreset);
        }
        else
        {
            FsSettings.Current.FixedRatioPresetSelected = 0;
            FsSettings.Current.Ratio = "";
        }
        GUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
        CheckCustomRatio();

        if (FsSettings.Current.FixedRatioPresetSelected == FurioosSettings.FixedRatioValuesPreset.Length-1)
        {
            EditorGUI.indentLevel++;
            GUILayout.Space(4.0f);
            EditorGUILayout.BeginHorizontal();
            {
                FsSettings.Current.Ratio = EditorGUILayout.TextField("Custom ratio", FsSettings.Current.Ratio, GUILayout.ExpandWidth(true));
            }
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
            if (!_customRatioValid)
            {
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = Color.red;
                EditorGUILayout.LabelField("Enter a valid ratio (ex: 16:9)", labelStyle);
            }
            EditorGUI.indentLevel--;
        }
    }

    void DrawSectionOtherSettings()
    {
        this._showOtherSettings = EditorGUILayout.BeginFoldoutHeaderGroup(this._showSettings, "Other settings");
        if (this._showOtherSettings)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField(new GUIContent("Touch event to mouse event", "Enable this option to convert touch events into mouse clicks\n(disclaimer: it will disable multi-touch on any touch-screen)"), GUILayout.MaxWidth(205));
            FsSettings.Current.ConvertTouch = EditorGUILayout.Toggle("", FsSettings.Current.ConvertTouch);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    void DrawSectionButtonBuildAndDeploy()
    {

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.enabled = !FurioosSettings.isApiTokenValid ? false : (!FurioosSettings.isDeploying && FsSettings.HasChanged());
        GUI.enabled = GUI.enabled && _customRatioValid;
        Double buttonWidth = Math.Min(((position.width-20) * 0.7) / 2, 200f) ;
        if (String.IsNullOrEmpty(FsSettings.Current.ApplicationID))
        {
            buttonWidth = Math.Min(position.width * 0.7 , 200f);
        }


        if (!String.IsNullOrEmpty(FsSettings.Current.ApplicationID))
        {
            if (GUILayout.Button(new GUIContent("Sync settings", "Synchronize your settings edition with the portal, without having to build and deploy"), GUILayout.Width((float) buttonWidth) , GUILayout.Height(25)))
            {
                FurioosSettings.isDeploying = true;
                FsSettings.Current.SaveSettings();
                BuildAndDeployHandler build = new BuildAndDeployHandler();
                build.SyncOptions();
                FurioosSettings.isDeploying = false;
                Repaint();
            }
        }
        GUILayout.Space(20f);
        GUI.enabled = FurioosSettings.isApiTokenValid  && !FurioosSettings.isDeploying;
        GUI.enabled = GUI.enabled && _customRatioValid;
        if (GUILayout.Button(new GUIContent("Build & deploy", "Build & deploy"), GUILayout.Width((float)buttonWidth), GUILayout.Height(25)))
        {
            var buildManager = new BuildAndDeployHandler();
            buildManager.BuildAndDeployToFurioos();
            Repaint();
            return;
        }
        GUI.enabled = !FurioosSettings.isDeploying;

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    public void DrawHorizontalLine(float height, Vector2 margin )
    {
        GUILayout.Space(margin.x);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(5f);
        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, height), new Color(0.1f,0.1f,0.1f));
        GUILayout.Space(5f);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(margin.y);
    }

    void DrawSectionTokenAPISettings()
    {
        this._showSettings = EditorGUILayout.BeginFoldoutHeaderGroup(this._showSettings, "Connection with Furioos");
        if (this._showSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("API Token:", GUILayout.MaxWidth(80));

            GUILayout.FlexibleSpace();
            if (GUILayout.Button ("Create API Token"))
            {
                Application.OpenURL("https://portal.furioos.com/settings/developers");
            }
            GUIStyle buttonStyle = new GUIStyle(EditorStyles.boldLabel);
            var iconHelp = EditorGUIUtility.IconContent("_Help");
            if (GUILayout.Button(iconHelp, buttonStyle))
            {
                Application.OpenURL("https://support.furioos.com/article/settings-developers/");
            }

            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
            GUIStyle style = new GUIStyle(EditorStyles.textArea);
            style.wordWrap = true;
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            var lastApiToken = FurioosSettings.apiToken;
            FurioosSettings.apiToken = EditorGUILayout.TextArea(FurioosSettings.apiToken, style);
            if (lastApiToken != FurioosSettings.apiToken)
            {
                FurioosApiHandler.CheckConnection();
            }
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
            this.DrawHorizontalLine(1.0f, new Vector2(5f, 5f));
            GUILayout.Space(10);
            EditorGUI.indentLevel--;
        }

        string btnName = "Check connection with Furioos";
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(btnName, GUILayout.Width(250), GUILayout.Height(25)))
        {
            FurioosApiHandler.CheckConnection();
            var connectionMessage = (FurioosSettings.isApiTokenValid) ? "Your connection with Furioos is OK" : " Your connection FAILED";
            EditorUtility.DisplayDialog("Checking connection with Furioos", connectionMessage, "Ok");
            Repaint();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    public void DrawSectionErrorApiToken()
    {
        if (!FurioosSettings.isApiTokenValid)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = Color.red;
            var icon = EditorGUIUtility.IconContent("console.erroricon");
            EditorGUILayout.LabelField(icon, GUILayout.Width(40));
            string text = String.IsNullOrEmpty(FurioosSettings.apiToken) ? "Enter an API Token" : "Your API Token is not valid";

            EditorGUILayout.LabelField(text, labelStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
    }

    public async void DrawSectionOptionsNotSynchronisedWithPortal()
    {
        if (!FurioosSettings.isApiTokenValid)
            return;

        if (_showOptionsNotSynchronisedWithPortal)
        {

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = Color.yellow;
            var icon = EditorGUIUtility.IconContent("console.warnicon");
            EditorGUILayout.LabelField(icon, GUILayout.Width(40));

            EditorGUILayout.LabelField("Your local settings differ from the portal", labelStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            Double buttonWidth = Math.Min(((position.width - 20) * 0.7) / 2, 200f);
            GUI.enabled = !FurioosSettings.isDeploying;
            if (!String.IsNullOrEmpty(FsSettings.Current.ApplicationID))
            {
                if (GUILayout.Button(new GUIContent("Keep local options", "Keep local options"), GUILayout.Height(25)))
                {
                    FurioosSettings.isDeploying = true;
                    BuildAndDeployHandler build = new BuildAndDeployHandler();
                    build.SyncOptions();
                    _showOptionsNotSynchronisedWithPortal = false;
                    FurioosSettings.isDeploying = false;
                    Repaint();
                }
            }
            GUILayout.Space(20f);

            if (GUILayout.Button(new GUIContent("Sync from Furioos", "Sync from Furioos portal"), GUILayout.Height(25)))
            {
                FurioosSettings.isDeploying = true;
                EditorUtility.DisplayProgressBar("Sync from Furioos portal","",0);
                var fsApplication = await FurioosApiHandler.GetApplicationDetails(FsSettings.Current.ApplicationID);
                if(fsApplication != null)
                {
                    FsSettings.AssignFromFsApplication(fsApplication);
                    textureThumbnail = null;
                    _showOptionsNotSynchronisedWithPortal = false;
                }
                FurioosSettings.isDeploying = false;
                EditorUtility.ClearProgressBar();
                Repaint();
                return;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            this.DrawHorizontalLine(1.0f, new Vector2(5f, 5f));
            GUILayout.Space(10);
        }
    }

    void OnGUI()
    {

        GUILayout.Space(10);
        EditorCoroutineUtility.StartCoroutine(LoadTexture(), this);
        DrawLogo();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        tabsName[0] = (String.IsNullOrEmpty(FsSettings.Current.ApplicationID)) ? "Create your App" : "Edit your App"; 
        this._selectedTab = GUILayout.Toolbar(this._selectedTab, tabsName);
        GUILayout.Space(20);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);

        DrawSectionErrorApiToken();

        switch (_selectedTab)
        {
            case 0:
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                GUI.enabled = FurioosSettings.isApiTokenValid && !FurioosSettings.isDeploying;
                DrawSectionOptionsNotSynchronisedWithPortal();

                DrawSectionGlobalInformation();
                this.DrawHorizontalLine(1.0f, new Vector2(5f, 0f));

                DrawSectionVirtualMachineConfiguration();
                this.DrawHorizontalLine(1.0f, new Vector2(5f, 0f));

                DrawSectionQualityConfiguration();
                this.DrawHorizontalLine(1.0f, new Vector2(5f, 5f));

                DrawSectionOtherSettings();
                this.DrawHorizontalLine(1.0f, new Vector2(5f, 5f));
                GUILayout.Space(10);


                DrawSectionButtonBuildAndDeploy();
                EditorGUILayout.EndScrollView();
                GUI.enabled = true;

                break;
            case 1:
                DrawSectionTokenAPISettings();
                break;

        }
    }




}
