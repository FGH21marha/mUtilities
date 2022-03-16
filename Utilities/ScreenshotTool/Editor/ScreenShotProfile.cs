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
    public PreviewCamera previewTexture = new PreviewCamera();
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

public class PreviewCamera
{
    GameObject previewObject;
    Camera previewCam;
    RenderTexture renderTexture;

    public Camera GetCamera() => previewCam;

    public RenderTexture GetTexture() => renderTexture;

    public void UpdateCamera(ScreenShotTransform transform, Rect rect)
    {
        if (GameObject.Find("previewCamera"))
            previewObject = GameObject.Find("previewCamera");
        else
            previewObject = new GameObject("previewCamera");

        if(previewCam == null)
        {
            if (previewObject.GetComponent<Camera>())
                previewCam = previewObject.GetComponent<Camera>();
            else
                previewCam = previewObject.AddComponent<Camera>();
        }

        if(renderTexture == null)
            renderTexture = new RenderTexture(1920,1080,(int)RenderTextureFormat.ARGB32);

        previewCam.rect = rect;
        previewObject.hideFlags = HideFlags.HideAndDontSave;

        previewCam.transform.position = transform.position;
        previewCam.transform.eulerAngles = transform.rotation;
        previewCam.fieldOfView = transform.fov;

        previewCam.targetTexture = renderTexture;
        previewCam.Render();
        previewCam.targetTexture = null;
    }
}
