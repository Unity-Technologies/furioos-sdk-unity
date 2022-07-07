using System;

[Serializable]
public class FsApplication
{
    public string _id;
    public string name;
    public FsApplicationParameters parameters;
    public string description;
    public string[] tags;
    public string viewCount;
    public string updateAt;
    public string createdAt;
    public string status;
    public FsApplicationBinary[] applicationBinaries;

}

[Serializable]
public class FsApplicationParameters
{
    public int quality;
    public string ratioMode;
    public string [] ratio;
    public bool touchConvert;
    public bool isVR;
}

