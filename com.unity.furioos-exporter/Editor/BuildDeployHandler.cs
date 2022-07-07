using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Build.Reporting;
using System.IO.Compression;
using System;
using System.Threading.Tasks;
using System.Threading;

public class BuildAndDeployHandler
{

    const string _BUILD_DIR_ = "furioos-build/";
    bool inProgress = true;
    float progressStepTotal = 4.0f;
    float progressStep = 0.0f;
    FsApplicationProgress oldProgress = null;

    public BuildSummary BuildUnityProject()
    {
        if(Directory.Exists(_BUILD_DIR_))
            Directory.Delete(_BUILD_DIR_,true);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = FurioosSettings.GetEnabledScenes();
        buildPlayerOptions.locationPathName = _BUILD_DIR_ + PlayerSettings.productName + ".exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;
        
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;
        return summary;

    }

    public bool CreateZipfileFromBuild(string buildDirectory, string pathZipFile)
    {

        if (File.Exists(pathZipFile))
            File.Delete(pathZipFile);
        try
        {
            ZipFile.CreateFromDirectory(buildDirectory, pathZipFile);
            return true;
        }
        catch
        {
            return false;
        }
    }
    private float GetProgressValue()
    {
        return this.progressStep / this.progressStepTotal;
    }

    public async void BuildAndDeployToFurioos()
    {

        this.progressStep = 1.0f;
        if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows)){
            EditorUtility.DisplayDialog("Furioos Exporter", "Deployment FAILED\nWindows build target is not supported\nPlease install Windows Build Support", "Ok");
            return;
        }
        try
        {
            FurioosSettings.isDeploying = true;
            EditorUtility.DisplayProgressBar("Deploy to furioos platform", "Build " + FsSettings.Current.Name, this.GetProgressValue());
            var summary = this.BuildUnityProject();
            var pathZipFile = FurioosSettings.GetProjectName() + ".zip";

            if (summary.result == BuildResult.Succeeded)
            {
                EditorUtility.DisplayProgressBar("Deploy to furioos platform", "Generate zip file " + FsSettings.Current.Name, this.GetProgressValue());
                this.progressStep++;
                var zipCreated = CreateZipfileFromBuild(_BUILD_DIR_, pathZipFile);
                if (zipCreated)
                {

                    if (!String.IsNullOrEmpty(FsSettings.Current.ApplicationID))
                    {
                        var applicationDetail = await FurioosApiHandler.GetApplicationDetails(FsSettings.Current.ApplicationID);
                        if (applicationDetail == null)
                        {
                            FsSettings.Current.ApplicationID = "";
                            FsSettings.Current.ApplicationBinaryID = "";
                        }
                    }

                    FsApplication fsApplication = null;
                    if (string.IsNullOrEmpty(FsSettings.Current.ApplicationID))
                    {
                        EditorUtility.DisplayProgressBar("Deploy to furioos platform", "Create your application " + FsSettings.Current.Name, this.GetProgressValue());
                        this.progressStep++;
                        fsApplication = await FurioosApiHandler.CreateApplication(FsSettings.Current.Name,
                            FsSettings.Current.Description,
                            PlayerSettings.productName + ".exe",
                            FsSettings.Current.ThumbnailFilePath,
                            Enum.GetName(typeof(FurioosSettings.VirtualMachineConfiguration), FsSettings.Current.VirtualMachineSelected),
                            FurioosSettings.QualityConfiguration[FsSettings.Current.QualitySelected].Item2,
                            FsSettings.Current.ConvertTouch,
                            Enum.GetName(typeof(FurioosSettings.RatioMode), FsSettings.Current.RatioModeSelected).ToLower(),
                            FsSettings.Current.Ratio
                            );
                    }
                    else
                    {
                        EditorUtility.DisplayProgressBar("Deploy to furioos platform", "Update your application " + FsSettings.Current.Name, this.GetProgressValue());
                        this.progressStep++;
                        fsApplication = await FurioosApiHandler.UpdateApplication(FsSettings.Current.ApplicationID, FsSettings.Current.Name,
                            FsSettings.Current.Description,
                            PlayerSettings.productName + ".exe",
                            FsSettings.Current.ThumbnailFilePath,
                            Enum.GetName(typeof(FurioosSettings.VirtualMachineConfiguration), FsSettings.Current.VirtualMachineSelected),
                            FurioosSettings.QualityConfiguration[FsSettings.Current.QualitySelected].Item2,
                            FsSettings.Current.ConvertTouch,
                            Enum.GetName(typeof(FurioosSettings.RatioMode), FsSettings.Current.RatioModeSelected).ToLower(),
                            FsSettings.Current.Ratio
                            );
                    }



                    if (fsApplication == null || fsApplication.applicationBinaries == null)
                    {
                        EditorUtility.DisplayDialog("Furioos Deployment", "Your deployment has FAILED\nImpossible to create an application on Furioos", "Ok");
                        throw new Exception("Impossible to create an application on Furioos");
                    }
                    EditorUtility.DisplayProgressBar("Deploy to furioos platform", "Start uploading your application: " + FsSettings.Current.Name+".zip", this.GetProgressValue());
                    this.progressStep++;

                    FsSettings.Current.ApplicationID = fsApplication._id;
                    FsSettings.Current.ApplicationBinaryID = fsApplication.applicationBinaries[0]._id;
                    FsSettings.Current.ThumbnailFilePath = "";
                    FsSettings.Current.ThumbnailFileUrl = fsApplication.applicationBinaries[0].thumbnailUrl;
                    FsSettings.Current.SaveSettings();
                    this.oldProgress = await FurioosApiHandler.GetProgressInformation(FsSettings.Current.ApplicationBinaryID);

                    FurioosApiHandler.OnUploadStart += FurioosApiManager_OnUploadStart;
                    FurioosApiHandler.OnUploadEnd += FurioosApiManager_OnUploadEnd;
                    var fsUpload = await FurioosApiHandler.UploadApplicationBinaryFlurl(pathZipFile, FsSettings.Current.ApplicationBinaryID);
                    if (fsUpload == null)
                    {
                        EditorUtility.DisplayDialog("Furioos Deployment", "Your deployment has FAILED\nImpossible to upload your zip file on Furioos", "Ok");
                        throw new Exception("Impossible to upload your zip file on Furioos");
                    }
                    Debug.Log(FsSettings.Current.Name + " is now on furioos");
                    EditorUtility.DisplayDialog("Furioos Deployment", "Successful deployment\nYour application is now on Furioos", "Ok");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }
        finally
        {
            this.inProgress = false;
            FurioosApiHandler.OnUploadStart -= FurioosApiManager_OnUploadStart;
            FurioosApiHandler.OnUploadEnd -= FurioosApiManager_OnUploadEnd;
            EditorUtility.ClearProgressBar();
            FurioosSettings.isDeploying = false;
        }
    }

    public async void SyncOptions()
    {
        EditorUtility.DisplayProgressBar("Synchronization settings", "", 0);
        if (!String.IsNullOrEmpty(FsSettings.Current.ApplicationID))
        {
            var applicationDetail = await FurioosApiHandler.GetApplicationDetails(FsSettings.Current.ApplicationID);
            if (applicationDetail == null || applicationDetail.applicationBinaries == null)
            {
                FsSettings.Current.ApplicationID = "";
                FsSettings.Current.ApplicationBinaryID = "";
                FsSettings.Current.ThumbnailFilePath = "";
                FsSettings.Current.SaveSettings();
                EditorUtility.DisplayDialog("Furioos sync settings", "Impossible to synchronize your application settings.\nIt may have been deleted.", "Ok");
                Debug.LogWarning("Synchronization settings FAILED. Application not found");
                EditorUtility.ClearProgressBar();
                return;
            }
        }

        FsApplication fsApplication = await FurioosApiHandler.UpdateApplication(FsSettings.Current.ApplicationID, FsSettings.Current.Name,
            FsSettings.Current.Description,
            "",
            FsSettings.Current.ThumbnailFilePath,
            Enum.GetName(typeof(FurioosSettings.VirtualMachineConfiguration), FsSettings.Current.VirtualMachineSelected),
            FurioosSettings.QualityConfiguration[FsSettings.Current.QualitySelected].Item2,
            FsSettings.Current.ConvertTouch,
            Enum.GetName(typeof(FurioosSettings.RatioMode), FsSettings.Current.RatioModeSelected).ToLower(),
            FsSettings.Current.Ratio
            );

        FsSettings.Current.ThumbnailFilePath = "";
        FsSettings.Current.ThumbnailFileUrl = fsApplication.applicationBinaries[0].thumbnailUrl;
        FsSettings.Current.SaveSettings();
        if (fsApplication == null)
        {
            Debug.LogWarning("Impossible to synchronize your application settings");
            EditorUtility.ClearProgressBar();
            return;
        }
        EditorUtility.DisplayProgressBar("Synchronization settings", "", 1);
        EditorUtility.DisplayDialog("Furioos sync settings", "Your settings are successfully synchronized", "Ok");
        EditorUtility.ClearProgressBar();
    }

    private void FurioosApiManager_OnUploadEnd(object sender, EventArgs e)
    {
        this.inProgress = false;
        EditorUtility.ClearProgressBar();
    }

    private async void FurioosApiManager_OnUploadStart(object sender, EventArgs e)
    {
        this.inProgress = true;
        float progressValue = 0.0f;
        while (this.inProgress)
        {
            var progress = await FurioosApiHandler.GetProgressInformation(FsSettings.Current.ApplicationBinaryID);
            if (progress != null && progress._id != oldProgress?._id) 
            {
                float.TryParse(progress.progress, out progressValue);
                progressValue = progressValue / 100;
                EditorUtility.DisplayProgressBar("Deploy to furioos platform", "Uploading: " + FsSettings.Current.Name+".zip", progressValue);
            }
            Thread.Sleep(2000);
        }
        EditorUtility.ClearProgressBar();
    }

    public async void RunToFurioos()
    {
        FsShareLink shareLink = null;

        if (!String.IsNullOrEmpty(FsSettings.Current.ShareLinkID))
        {
            shareLink =  await FurioosApiHandler.GetSharelink(FsSettings.Current.ShareLinkID);
        }
        if (shareLink == null || shareLink.status != "active" || shareLink.applicationBinaryID != FsSettings.Current.ApplicationBinaryID)
        {
        
            shareLink = await FurioosApiHandler.CreateSharelink("Furioos exporter", FsSettings.Current.ApplicationBinaryID);
            if (shareLink != null)
            {
                FsSettings.Current.ShareLinkID = shareLink._id;
            }
            else
            {
                EditorUtility.DisplayDialog("Furioos Deployment", "Your share link cannot be created", "Ok");
                return;
            }
        }
        Application.OpenURL(shareLink.linkUrl);

    }


}