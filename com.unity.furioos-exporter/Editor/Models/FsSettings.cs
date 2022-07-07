using System;
using System.Linq;
using UnityEngine;

public class FsSettings
{
    private string _name;
    private string _description;
    private string _thumbnailFilePath;
    private string _thumbnailFileUrl;
    private int _virtualMachineSelected;
    private int _qualitySelected;
    private int _ratioModeSelected;
    private int _fixedRatioPresetSelected;
    private string _ratio;
    private bool convertTouch;

    private string _applicationID;
    private string _applicationBinaryID;
    private string _shareLinkID;

    private static FsSettings _currentInstance = null;

    public string Name
    {
        get { return this._name; }
        set
        {
            if (String.IsNullOrEmpty(value))
            {
                this._name = FurioosSettings.GetProjectName();
            } 
            else
            {
                this._name = value;
            }
        }
    }
    public string Description { get => _description; set => _description = value; }
    public string ThumbnailFilePath { get => _thumbnailFilePath; set => _thumbnailFilePath = value; }
    public string ThumbnailFileUrl { get => _thumbnailFileUrl; set => _thumbnailFileUrl = value; }

    public int VirtualMachineSelected 
    {
        get
        {
            return this._virtualMachineSelected;
        }
        set
        {
            this._virtualMachineSelected = (value >= 0 && value <= Enum.GetValues(typeof(FurioosSettings.VirtualMachineConfiguration)).Cast<FurioosSettings.VirtualMachineConfiguration>().Count()) ? value : 0;
        }

    }
    public int QualitySelected
    {
        get
        {
            return this._qualitySelected;
        }
        set
        {
            this._qualitySelected = (value >= 0 && value < FurioosSettings.QualityConfiguration.Length) ? value : 0;
        }
    }

    public int RatioModeSelected
    {
        get
        {
            return this._ratioModeSelected;
        }
        set
        {
            this._ratioModeSelected = (value >= 0 && value <= Enum.GetValues(typeof(FurioosSettings.RatioMode)).Cast<FurioosSettings.RatioMode>().Count()) ? value : 0;
        }
    }

    public int FixedRatioPresetSelected
    {
        get
        {
            return this._fixedRatioPresetSelected;
        }
        set
        {
            this._fixedRatioPresetSelected = (value >= 0 && value < FurioosSettings.FixedRatioValuesPreset.Length) ? value : 0;
            if(this._fixedRatioPresetSelected < FurioosSettings.FixedRatioValuesPreset.Length -1)
            {
                this.Ratio = FurioosSettings.FixedRatioValuesPreset[this._fixedRatioPresetSelected];
            }
        }
    }

    public string Ratio { get => _ratio; set => _ratio = value; }
    public bool ConvertTouch { get => convertTouch; set => convertTouch = value; }
    public string ApplicationID { get => _applicationID; set => _applicationID = value; }
    public string ApplicationBinaryID { get => _applicationBinaryID; set => _applicationBinaryID = value; }
    public string ShareLinkID { get => _shareLinkID; set => _shareLinkID = value; }


    public static bool HasChanged()
    {
        bool hasChanged = false;
        hasChanged = hasChanged || (_currentInstance.Name != PlayerPrefs.GetString("name", ""));
        hasChanged = hasChanged || (_currentInstance.Description != PlayerPrefs.GetString("description", ""));
        hasChanged = hasChanged || (_currentInstance.ThumbnailFilePath != PlayerPrefs.GetString("thumbnailFilePath", ""));
        hasChanged = hasChanged || (_currentInstance.VirtualMachineSelected != PlayerPrefs.GetInt("virtualMachineSelected", 0));
        hasChanged = hasChanged || (_currentInstance.QualitySelected != PlayerPrefs.GetInt("qualitySelected", 0));
        hasChanged = hasChanged || (_currentInstance.RatioModeSelected != PlayerPrefs.GetInt("ratioModeSelected", 0));
        hasChanged = hasChanged || (_currentInstance.Ratio != PlayerPrefs.GetString("ratio", ""));
        hasChanged = hasChanged || (_currentInstance.ConvertTouch != Convert.ToBoolean(PlayerPrefs.GetInt("convertTouch", 0)));
        return hasChanged;
    }

    public void LoadSettings()
    {
        this.LoadCurrentSettings();
        this.LoadRatioPresetIfExist();
    }

    private void LoadCurrentSettings()
    {
        _currentInstance.Name = PlayerPrefs.GetString("name", "");
        _currentInstance.Description = PlayerPrefs.GetString("description", "");
        _currentInstance.ThumbnailFilePath = PlayerPrefs.GetString("thumbnailFilePath", "");
        _currentInstance.ThumbnailFileUrl = PlayerPrefs.GetString("thumbnailFileUrl", "");
        _currentInstance.VirtualMachineSelected = PlayerPrefs.GetInt("virtualMachineSelected", 0);
        _currentInstance.QualitySelected = PlayerPrefs.GetInt("qualitySelected", 0);
        _currentInstance.RatioModeSelected = PlayerPrefs.GetInt("ratioModeSelected", 0);
        _currentInstance.Ratio = PlayerPrefs.GetString("ratio", "16:9");
        _currentInstance.ConvertTouch = Convert.ToBoolean(PlayerPrefs.GetInt("convertTouch", 0));
        _currentInstance.ApplicationID = PlayerPrefs.GetString("applicationID", "");
        _currentInstance.ApplicationBinaryID = PlayerPrefs.GetString("applicationBinaryID", "");
        _currentInstance.ShareLinkID = PlayerPrefs.GetString("shareLinkID", "");
    }

    private void LoadRatioPresetIfExist()
    {
        if (_currentInstance.RatioModeSelected == (int)FurioosSettings.RatioMode.Fixed)
        {
            var index = Array.IndexOf(FurioosSettings.FixedRatioValuesPreset, _currentInstance.Ratio);
            _currentInstance.FixedRatioPresetSelected = (index >= 0) ? index : FurioosSettings.FixedRatioValuesPreset.Length - 1;
        }
    }

    public void SaveSettings()
    {
        LoadRatioPresetIfExist();
        PlayerPrefs.SetString("name", _currentInstance.Name) ;
        PlayerPrefs.SetString("description", _currentInstance.Description);
        PlayerPrefs.SetString("thumbnailFilePath", _currentInstance.ThumbnailFilePath);
        PlayerPrefs.SetString("thumbnailFileUrl", _currentInstance.ThumbnailFileUrl);
        PlayerPrefs.SetInt("virtualMachineSelected", _currentInstance.VirtualMachineSelected);
        PlayerPrefs.SetInt("qualitySelected", _currentInstance.QualitySelected);
        PlayerPrefs.SetInt("ratioModeSelected", _currentInstance.RatioModeSelected);
        PlayerPrefs.SetString("ratio", _currentInstance.Ratio);
        PlayerPrefs.SetInt("convertTouch", Convert.ToInt32(_currentInstance.ConvertTouch));
        PlayerPrefs.SetString("applicationID", _currentInstance.ApplicationID);
        PlayerPrefs.SetString("applicationBinaryID", _currentInstance.ApplicationBinaryID);
        PlayerPrefs.SetString("shareLinkID", _currentInstance.ShareLinkID);
    }

    public static FsSettings Current
    {
        get
        {
            if (_currentInstance == null)
            {
                _currentInstance = new FsSettings();
                _currentInstance.LoadSettings();
            }
            return _currentInstance;
        }
    }

    public static bool IsEqual(FsApplication fsApplication)
    {
        bool result = true;
        result = result && (PlayerPrefs.GetString("name", "") == fsApplication.name);
        result = result && (PlayerPrefs.GetString("description", "") == fsApplication.description);
        result = result && (PlayerPrefs.GetString("thumbnailFileUrl", "") == fsApplication.applicationBinaries[0].thumbnailUrl);
        result = result && (PlayerPrefs.GetInt("virtualMachineSelected", 0) == (int)Enum.Parse(typeof(FurioosSettings.VirtualMachineConfiguration), fsApplication.applicationBinaries[0].computeTypeName));
        var quality = FurioosSettings.QualityConfiguration.First(x => x.Item2 == fsApplication.parameters.quality);
        var index = Array.IndexOf(FurioosSettings.QualityConfiguration, quality);
        result = result && (PlayerPrefs.GetInt("qualitySelected", 0) == index);
        result = result && (PlayerPrefs.GetInt("ratioModeSelected", 0) == (int)Enum.Parse(typeof(FurioosSettings.RatioMode), fsApplication.parameters.ratioMode, true));
        if((FurioosSettings.RatioMode)Enum.Parse(typeof(FurioosSettings.RatioMode), fsApplication.parameters.ratioMode, true) == FurioosSettings.RatioMode.Fixed)
        {
            result = result && (PlayerPrefs.GetString("ratio", "") ==  (fsApplication.parameters.ratio != null ? fsApplication.parameters?.ratio[0] : ""));
        }
        result = result && (Convert.ToBoolean(PlayerPrefs.GetInt("convertTouch", 0)) == fsApplication.parameters.touchConvert);

        return result;
    }

    public static void AssignFromFsApplication(FsApplication fsApplication)
    {
        Current.Name = fsApplication.name;
        Current.Description = fsApplication.description;
        Current.ThumbnailFileUrl = fsApplication.applicationBinaries[0].thumbnailUrl;
        Current.VirtualMachineSelected = (int)Enum.Parse(typeof(FurioosSettings.VirtualMachineConfiguration), fsApplication.applicationBinaries[0].computeTypeName);
        var quality = FurioosSettings.QualityConfiguration.First(x => x.Item2 == fsApplication.parameters.quality);
        var index = Array.IndexOf(FurioosSettings.QualityConfiguration, quality);
        Current.QualitySelected = index;
        Current.RatioModeSelected = (int)Enum.Parse(typeof(FurioosSettings.RatioMode), fsApplication.parameters.ratioMode, true);
        if ((FurioosSettings.RatioMode)Enum.Parse(typeof(FurioosSettings.RatioMode), fsApplication.parameters.ratioMode, true) == FurioosSettings.RatioMode.Fixed)
        {
            index = -1;
            if(fsApplication.parameters.ratio != null)
            {
                index = Array.IndexOf(FurioosSettings.FixedRatioValuesPreset, fsApplication.parameters.ratio[0]);
            }
            Current.FixedRatioPresetSelected = (index >= 0) ? index : FurioosSettings.FixedRatioValuesPreset.Length - 1;
            Current.Ratio = fsApplication.parameters.ratio[0];
        }
        Current.ConvertTouch = fsApplication.parameters.touchConvert;
        Current.SaveSettings();
    }

}