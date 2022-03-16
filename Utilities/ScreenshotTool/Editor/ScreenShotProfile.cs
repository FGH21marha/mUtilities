// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class ScreenShotProfile : ScriptableObject
{
    public string folder;
    public string filename = "New Screenshot";
    public float previewSize = 1f;

    public int Width = 1920;
    public int Height = 1080;

    public ScreenShotFormat format;

    public ScreenShotTransform[] screenshots = new ScreenShotTransform[0];
}

[System.Serializable]
public class ScreenShotTransform
{
    public Vector3 position;
    public Vector3 rotation;
    public float fov = 85;
    public bool preview;
}

public enum ScreenShotFormat
{
    JPG, PNG, TGA
}

[CustomEditor(typeof(ScreenShotProfile))]
public class ScreenShotProfileEditor : Editor
{
    public override void OnInspectorGUI() {}
}
