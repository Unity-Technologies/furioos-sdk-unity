using System;

[Serializable]
public class FsApplicationProgress
{
    public string _id;
    public string applicationBinaryID;
    public string filename;
    public string addedAt;
    public string updatedAt;
    public string status;
    public string progress;
    public string bytesRead;
    public string completeAt;
}