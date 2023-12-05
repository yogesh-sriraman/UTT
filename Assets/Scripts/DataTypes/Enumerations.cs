/// <summary>
/// ENUM for component type <br />
/// When a new component is added, add it before the <b>COUNT</b> enum. <br /><br />
/// To get the total number of components: <br />
/// <c> var numOfTypes = TRACKER_TYPE.COUNT</c>
/// </summary>
[System.Serializable]
public enum TRACKER_TYPE
{
    METRIA,
    ATRACSYS,
    COUNT,


    DEFAULT = -1,
    INVALID = -2,
}

[System.Serializable]
public enum MARKER_TYPE
{
    POINTER,
    RESECTION_GUIDE,

    DEFAULT
}
