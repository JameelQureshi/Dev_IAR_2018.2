using System.Collections.Generic;


public class BaseAsset
{
    public int assetId;
    public UIEnums.AssetTypes assetType;
    public string name;
    public string userId;
    public string url;
    public string description;
    public string thumbnailURL;
}

public class UserAsset
{
    public List<Audio> audio;
    public List<Video> video;
    public List<Document> document;
    public List<Icon> icon;
    public List<ImageAsset> image;
    public List<ThreeDObjects> threeDObject;
    public string userId;
}

public class ElementAsset
{
    public UIEnums.AssetTypes assetType;
    public Audio audio;
    public Video video;
    public Document document;
    public Icon icon;
    public ImageAsset image;
    public ThreeDObjects threeDObject;
}

public class EventAction
{
    public UIEnums.EventTypes eventType;
    public List<ElementAsset> elementAssets;

}


public class Audio : BaseAsset
{

}

public class Document : BaseAsset
{

}

public class Icon : BaseAsset
{

}

public class ImageAsset : BaseAsset
{

}

public class ThreeDObjects : BaseAsset
{

}

public class Video : BaseAsset
{

}