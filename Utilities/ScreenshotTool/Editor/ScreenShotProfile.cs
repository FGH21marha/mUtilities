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
    public bool ortho;
    public float fov = 85;
    public float orthoSize = 10;

    public ScreenShotBackground screenShotBackground;
    public Color backgroundColor = new Color(0, 0.75f,1,1);

    public bool preview;
    public PreviewCamera previewTexture = new PreviewCamera();
}

public enum ScreenShotFormat
{
    JPG, PNG, TGA
}

public enum ScreenShotBackground
{
    Solid, Skybox, Transparent
}

[CustomEditor(typeof(ScreenShotProfile))]
public class ScreenShotProfileEditor : Editor
{
    public override void OnInspectorGUI() { }
}

public class PreviewCamera
{
    GameObject previewObject;
    Camera previewCam;
    RenderTexture renderTexture;

    public Camera GetCamera() => previewCam;

    public RenderTexture GetTexture() => renderTexture;

    public void GetPicture(ScreenShotTransform transform, Rect rect, int width, int height)
    {
        if (GameObject.Find("previewCamera"))
            previewObject = GameObject.Find("previewCamera");
        else
            previewObject = new GameObject("previewCamera");

        if (previewCam == null)
        {
            if (previewObject.GetComponent<Camera>())
                previewCam = previewObject.GetComponent<Camera>();
            else
                previewCam = previewObject.AddComponent<Camera>();
        }

        renderTexture = new RenderTexture(width, height, (int)RenderTextureFormat.ARGB32);
        previewCam.rect = rect;
        previewObject.hideFlags = HideFlags.HideAndDontSave;

        switch (transform.screenShotBackground)
        {
            case ScreenShotBackground.Solid: previewCam.clearFlags = CameraClearFlags.SolidColor; break;
            case ScreenShotBackground.Skybox: previewCam.clearFlags = CameraClearFlags.Skybox; break;
            case ScreenShotBackground.Transparent: previewCam.clearFlags = CameraClearFlags.Depth; break;
        }

        previewCam.backgroundColor = transform.backgroundColor;

        previewCam.transform.position = transform.position;
        previewCam.transform.eulerAngles = transform.rotation;

        previewCam.orthographic = transform.ortho;

        if (transform.ortho)
            previewCam.orthographicSize = transform.orthoSize;
        else
            previewCam.fieldOfView = transform.fov;

        previewCam.targetTexture = renderTexture;
        previewCam.Render();
        previewCam.targetTexture = null;
    }
}
