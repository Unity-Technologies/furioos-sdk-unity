using System;

[Serializable]
public class FsShareLink
{
    public string _id;
    public string name;
    public int type;
    public string applicationBinaryID;
    public string linkID;
    public bool autorun;
    public bool hasPassword;
    public string sharedAt;
    public string status;
    public FsShareQuota quota;
    public string linkUrl;

}

[Serializable]
public class FsShareQuota
{
    public int compute;
    public FsShareQuotaUsage usage;
    public FsShareQuotaSession session;
    public string creatededAt;
    public string refreshedAt;
    public string status;
}

[Serializable]
public class FsShareQuotaUsage
{
    public int compute;
}

[Serializable]
public class FsShareQuotaSession
{
    public int maxDuration;
}
