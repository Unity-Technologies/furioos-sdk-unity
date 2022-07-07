using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Threading.Tasks;
using System;
using Flurl.Http;
using System.Linq;

public class FurioosApiHandler: EditorWindow
{

    public static event EventHandler OnUploadStart;
    public static event EventHandler OnUploadEnd;

    public enum RequestType
    {
        Get,
        Post,
        Put,
        Delete
    }

    public static UnityWebRequest CreateWebRequest(String url, RequestType requestType = RequestType.Get, WWWForm form = null)
    {

        UnityWebRequest www = null;
        if (String.IsNullOrEmpty(FurioosSettings.apiToken))
        {
            Debug.LogWarning("Your api token is empty");
            return www;
        }
        switch (requestType)
        {
            case RequestType.Get:
                www = UnityWebRequest.Get(url);
                break;
            case RequestType.Post:
                www = UnityWebRequest.Post(url, form);
                break;
            case RequestType.Put:
                www = UnityWebRequest.Post(url, form);
                www.method = "PUT";
                break;
            case RequestType.Delete:
                www = UnityWebRequest.Delete(url);
                break;
        }
        www.SetRequestHeader("Authorization", "Bearer " + FurioosSettings.apiToken);

        return www;
    }

    public static async void CheckConnection()
    {
        if (String.IsNullOrEmpty(FurioosSettings.apiToken))
        {
            FurioosSettings.isApiTokenValid = false;
            return;
        }
        FurioosSettings.isApiTokenValid = await GetValidToken();
        return;
    }

    public static async Task<bool> CheckValidApplication()
    {
        if (String.IsNullOrEmpty(FsSettings.Current.ApplicationID) || !FurioosSettings.isApiTokenValid)
        {
            return false;
        }

        var fsApplication = await GetApplicationDetails(FsSettings.Current.ApplicationID);

        if (fsApplication == null)
        {
            EditorUtility.DisplayDialog("Settings error", "Your application hast not found. It might have been removed from Furioos\nPlease Build and Deploy again", "Ok");
            Debug.LogWarning("Application not found");
            FsSettings.Current.ApplicationID = "";
            FsSettings.Current.ApplicationBinaryID = "";
            return false;
        }
        return true;
    }

    public static async Task<bool> GetValidToken()
    {

        using var www = CreateWebRequest(FurioosSettings.apiUrl + "/applications", RequestType.Get);
        var operation = www.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();
        var result = (www.result == UnityWebRequest.Result.Success);

        if (!result)
        {
            Debug.LogWarning(www.downloadHandler.text);
        }
        return result;

    } 

    public static async Task<FsApplication> CreateApplication(string name, string description, string mainExe, string thumbnailFilePath, string computeTypeName, int quality, bool touchConvert, string ratioMode, string ratio)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("description", description);
        form.AddField("mainExe", mainExe);
        form.AddField("computeTypeName", computeTypeName);
        form.AddField("quality", quality);
        form.AddField("touchConvert", touchConvert.ToString());

        form.AddField("ratioMode", ratioMode);
        if (!String.IsNullOrEmpty(ratio))
            form.AddField("ratio", ratio);

        if (!String.IsNullOrEmpty(thumbnailFilePath) && File.Exists(thumbnailFilePath))
        {
            byte[] byteArray = File.ReadAllBytes(thumbnailFilePath);
            form.AddBinaryData("thumbnail", byteArray, thumbnailFilePath, "image/png");
        }

        using var www = CreateWebRequest(FurioosSettings.apiUrl + "/applications", RequestType.Post, form);
        var operation = www.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(www.downloadHandler.text);
            return null;
        }
        else
        {
            return JsonUtility.FromJson<FsApplication>(www.downloadHandler.text);
        }

    }

    public static async Task<FsApplication> UpdateApplication(string applicationID, string name, string description, string mainExe, string thumbnailFilePath, string computeTypeName, int quality, bool touchConvert, string ratioMode, string ratio)
    {
        WWWForm form = new WWWForm();
        if (!String.IsNullOrEmpty(name))
            form.AddField("name", name);

        if (!String.IsNullOrEmpty(description))
            form.AddField("description", description);

        if (!String.IsNullOrEmpty(mainExe))
            form.AddField("mainExe", mainExe);

        if (!String.IsNullOrEmpty(computeTypeName))
            form.AddField("computeTypeName", computeTypeName);

        form.AddField("quality", quality);
        form.AddField("touchConvert", touchConvert.ToString());

        form.AddField("ratioMode", ratioMode);
        if(!String.IsNullOrEmpty(ratio))
            form.AddField("ratio", ratio);

        if (!String.IsNullOrEmpty(thumbnailFilePath) && File.Exists(thumbnailFilePath))
        {
            byte[] byteArray = File.ReadAllBytes(thumbnailFilePath);
            form.AddBinaryData("thumbnail", byteArray, thumbnailFilePath, "image/png");
        }

        using var www = CreateWebRequest(FurioosSettings.apiUrl + "/applications/" + applicationID, RequestType.Put, form);
        var operation = www.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (www.result != UnityWebRequest.Result.Success)
        {
            return null;
        }
        else
        {
            return JsonUtility.FromJson<FsApplication>(www.downloadHandler.text);
        }

    }

    public async static Task<FsApplicationBinary> UploadApplicationBinaryFlurl(string filePath, string binaryID)
    {
        try
        {
            string uri = FurioosSettings.apiUrl + "/applicationbinaries/" + binaryID + "/upload";
            FileStream fsFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            OnUploadStart.Invoke(null, EventArgs.Empty);
            var response = await uri.AllowAnyHttpStatus()
                                .WithOAuthBearerToken(FurioosSettings.apiToken)
                                .WithTimeout(1 * 60 * 60) // 1 hour timeout                                
                                .PostMultipartAsync(mp => mp
                                    .AddFile("file", fsFile, filePath, "application/zip")
                                );
            OnUploadEnd.Invoke(null, EventArgs.Empty);
             
            if (response.StatusCode != 200)
            {
                var result = await response.GetStringAsync();
                Debug.LogWarning(result);
                return null;
            }
            else
            {
                return await response.GetJsonAsync<FsApplicationBinary>();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        return null;

    }


    public static async Task<FsApplication> GetApplicationDetails(string applicationID, bool logMessage = false)
    {

        using var www = CreateWebRequest(FurioosSettings.apiUrl + "/applications/" + applicationID, RequestType.Get);
        var operation = www.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (www.result != UnityWebRequest.Result.Success)
        {
            if (logMessage)
            {
                Debug.LogWarning(www.downloadHandler.text);
            }
            return null;
        }
        else
        {
            return JsonUtility.FromJson<FsApplication>(www.downloadHandler.text);
        }

    }

    public static async Task<FsApplicationProgress> GetProgressInformation(string binaryID)
    {

        using var www = CreateWebRequest(FurioosSettings.apiUrl + "/applications/progress/" + binaryID, RequestType.Get);
        var operation = www.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (www.result != UnityWebRequest.Result.Success)
        {
            return null;
        }
        else
        {
            return JsonUtility.FromJson<FsApplicationProgress>(www.downloadHandler.text);
        }

    }

    public static async Task<FsShareLink> GetSharelink(string shareID, bool logMessage = false)
    {

        using var www = CreateWebRequest(FurioosSettings.apiUrl + "/applicationbinaries/shares/" + shareID, RequestType.Get);
        var operation = www.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (www.result != UnityWebRequest.Result.Success)
        {
            if (logMessage)
            {
                Debug.LogWarning(www.downloadHandler.text);
            }
            return null;
        }
        else
        {
            return JsonUtility.FromJson<FsShareLink>(www.downloadHandler.text);
        }

    }

    public static async Task<FsShareLink> CreateSharelink(string shareLinkName, string binaryID)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", shareLinkName);

        using var www = CreateWebRequest(FurioosSettings.apiUrl + "/applicationbinaries/" + binaryID + "/shares/links", RequestType.Post,form);
        var operation = www.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(www.downloadHandler.text);
            return null;
        }
        else
        {
            return JsonUtility.FromJson<FsShareLink>(www.downloadHandler.text);
        }

    }

}