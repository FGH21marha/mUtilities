// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

using UnityEditor;
using UnityEngine;

public class ScreenShotTool : EditorWindow
{
    [MenuItem("Screenshot/Open Screenshot window")]
    public static void OpenWindow()
    {
        EditorWindow window = (ScreenShotTool)GetWindow(typeof(ScreenShotTool));
        window.minSize = new Vector2(400, 200);
        window.autoRepaintOnSceneChange = true;
        window.Show();
    }

    public ScreenShotProfile profile;

    static Vector2 scrollPos;

    private void OnGUI()
    {
        profile = (ScreenShotProfile)EditorGUILayout.ObjectField(profile, typeof(ScreenShotProfile), false);

        if (profile == null)
            return;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Destination:");
        EditorGUILayout.LabelField(profile.folder, EditorStyles.wordWrappedLabel);

        if (GUILayout.Button("Select Folder"))
            profile.folder = EditorUtility.OpenFolderPanel("Screenshot Location", "", "");

        if (profile.folder == null || profile.folder == "")
        {
            EditorGUILayout.HelpBox("Please select a screenshot destination folder", MessageType.None);
            return;
        }

        DrawTitle("Image Properties");

        var bgColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.black;
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUI.backgroundColor = bgColor;
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Width", GUILayout.Width(50));
                        profile.Width = EditorGUILayout.IntField(profile.Width, GUILayout.Width(50));
                        EditorGUILayout.LabelField("px", GUILayout.Width(50));
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Height", GUILayout.Width(50));
                        profile.Height = EditorGUILayout.IntField(profile.Height, GUILayout.Width(50));
                        EditorGUILayout.LabelField("px", GUILayout.Width(50));
                    }
                }
                using (new EditorGUILayout.VerticalScope())
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Name", GUILayout.Width(50));
                        profile.filename = EditorGUILayout.TextField(profile.filename);
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Format", GUILayout.Width(50));
                        profile.format = (ScreenShotFormat)EditorGUILayout.EnumPopup(profile.format);
                    }
                }
            }
        }

        if (profile.screenshots.Length > 0)
        {
            if (GUILayout.Button("Take Screenshots", GUILayout.Height(30)))
            {
                TakeScreenshots();
            }
        }

        DrawTitle("Cameras");

        if (GUILayout.Button("Add Camera", GUILayout.Height(30)))
            mArray.Add(ref profile.screenshots, new ScreenShotTransform());

        using (var scope = new EditorGUILayout.ScrollViewScope(scrollPos))
        {
            scrollPos = scope.scrollPosition;
            foreach (var screenshot in profile.screenshots)
            {
                DrawScreenshotElement(screenshot);
            }
        }

        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(profile);
    }

    private void DrawScreenshotElement(ScreenShotTransform screen)
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            var bgColor = GUI.backgroundColor;

            GUI.backgroundColor = Color.black;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUI.backgroundColor = bgColor;
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Position", GUILayout.Width(80));
                    screen.position = EditorGUILayout.Vector3Field("", screen.position);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Rotation", GUILayout.Width(80));
                    screen.rotation = EditorGUILayout.Vector3Field("", screen.rotation);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Orthographic", GUILayout.Width(80));
                    screen.ortho = EditorGUILayout.Toggle(screen.ortho, GUILayout.Width(20));

                    if (screen.ortho)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Ortho", GUILayout.Width(40));
                            screen.orthoSize = EditorGUILayout.Slider(screen.orthoSize, 0.1f, 50f);
                        }
                    }
                    else
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("FOV", GUILayout.Width(40));
                            screen.fov = EditorGUILayout.Slider(screen.fov, 0.1f, 180f);
                        }
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Background", GUILayout.Width(80));
                    screen.screenShotBackground = (ScreenShotBackground)EditorGUILayout.EnumPopup(screen.screenShotBackground);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (screen.screenShotBackground == ScreenShotBackground.Solid)
                        screen.backgroundColor = EditorGUILayout.ColorField(screen.backgroundColor);
                }
            }

            screen.preview = EditorGUILayout.Foldout(screen.preview, new GUIContent("Preview"), true);

            if (screen.preview)
            {
                using (var rect = new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    float y = profile.Width / 10;
                    float x = y * profile.Width / profile.Height;

                    GUILayout.Space(y);

                    GUI.backgroundColor = Color.white;
                    var c = GUI.color;
                    GUI.color = Color.white;

                    PreviewCamera preview = screen.previewTexture;
                    preview.GetPicture(screen, new Rect(0, 0, x, y), (int)x, (int)y);

                    GUI.DrawTexture(new Rect(rect.rect.x + 3, rect.rect.y + 3, x, y), preview.GetTexture());

                    GUI.color = c;
                    GUI.backgroundColor = bgColor;
                }
            }

            if (GUILayout.Button("Delete Camera"))
            {
                for (int i = 0; i < profile.screenshots.Length; i++)
                {
                    if (profile.screenshots[i] == screen)
                    {
                        mArray.Remove(ref profile.screenshots, i);
                        break;
                    }
                }
            }
        }
    }

    private void TakeScreenshots()
    {
        int num = 0;

        foreach (var screen in profile.screenshots)
        {
            RenderTexture rt = new RenderTexture(profile.Width, profile.Height, 32);
            Texture2D screenShot = new Texture2D(profile.Width, profile.Height, TextureFormat.ARGB32, true);
            screen.previewTexture.GetPicture(screen, new Rect(0, 0, profile.Width, profile.Height), profile.Width, profile.Height);
            RenderTexture.active = screen.previewTexture.GetTexture();
            screenShot.ReadPixels(new Rect(0, 0, profile.Width, profile.Height), 0, 0);
            screenShot.Apply();
            RenderTexture.active = null;

            byte[] bytes = null;

            var fileType = "";

            switch (profile.format)
            {
                case ScreenShotFormat.JPG: bytes = screenShot.EncodeToJPG(100); fileType = "jpg"; break;
                case ScreenShotFormat.PNG: bytes = screenShot.EncodeToPNG(); fileType = "png"; break;
                case ScreenShotFormat.TGA: bytes = screenShot.EncodeToTGA(); fileType = "tga"; break;
            }

            string filename = string.Format("{0}/{1}.{2}", profile.folder, profile.filename + num.ToString(), fileType);

            if (System.IO.File.Exists(filename))
            {
                int option = EditorUtility.DisplayDialogComplex("Overwrite", profile.filename + " already exists in this folder. Do you want to overwrite this file?", "Overwrite", "Cancel", null);

                if (option == 1)
                    continue;
            }

            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            num++;
        }

        AssetDatabase.Refresh();
    }

    void DrawTitle(string title)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(title, BoldLabel());

        using (var pos = new EditorGUILayout.HorizontalScope())
        {
            Handles.color = new Color(0, 0, 0, 0.7f);
            Handles.DrawLine(new Vector3(pos.rect.x - 4, pos.rect.y), new Vector3(pos.rect.width + 11, pos.rect.y));
        }
    }

    GUIStyle BoldLabel()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1, 1, 1, 0.7f);
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 14;
        style.alignment = TextAnchor.MiddleLeft;
        style.contentOffset = Vector3.down * 1;
        return style;
    }

    private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    void OnSceneGUI(SceneView sv)
    {
        if (!sv.drawGizmos) return;

        if (profile == null)
            return;

        foreach (var screenshot in profile.screenshots)
        {
            DrawScreenshotInScene(sv, screenshot);
        }
    }

    void DrawScreenshotInScene(SceneView sv, ScreenShotTransform screen)
    {
        Handles.color = Color.white;

        var forward = Quaternion.Euler(screen.rotation) * Vector3.forward;

        Handles.matrix = Matrix4x4.TRS(screen.position - (forward * 0.3f), Quaternion.Euler(screen.rotation), Vector3.one);
        Handles.DrawWireCube(Vector3.zero, new Vector3(0.6f, 0.6f, 0.8f));

        float aspectRatio = (float)profile.Width / (float)profile.Height;

        var yScale = Mathf.Tan(20 / 2f * Mathf.Deg2Rad);
        float xScale = yScale * aspectRatio;

        var topLeft = Vector3.forward + (Vector3.left * xScale) + (Vector3.up * yScale);
        var topRight = Vector3.forward + (Vector3.right * xScale) + (Vector3.up * yScale);
        var bottomLeft = Vector3.forward + (Vector3.left * xScale) + (Vector3.down * yScale);
        var bottomRight = Vector3.forward + (Vector3.right * xScale) + (Vector3.down * yScale);

        Handles.DrawLine(Vector3.forward * 0.3f, topLeft);
        Handles.DrawLine(Vector3.forward * 0.3f, topRight);
        Handles.DrawLine(Vector3.forward * 0.3f, bottomLeft);
        Handles.DrawLine(Vector3.forward * 0.3f, bottomRight);

        Handles.DrawLine(topLeft, topRight);
        Handles.DrawLine(topRight, bottomRight);
        Handles.DrawLine(bottomRight, bottomLeft);
        Handles.DrawLine(bottomLeft, topLeft);

        Handles.matrix = Matrix4x4.identity;

        RaycastHit hit;
        Physics.Raycast(screen.position, forward, out hit);

        if (hit.collider != null)
        {
            Handles.color = Color.green;
            Handles.DrawSolidDisc(hit.point + (hit.normal * 0.002f), hit.normal, 0.05f);
            Handles.color = new Color(0, 1, 0, 0.5f);
            Handles.DrawLine(screen.position, hit.point);
        }

        EditorGUI.BeginChangeCheck();

        var currentWindow = focusedWindow;
        if (currentWindow == sv)
        {
            if (Tools.current == Tool.Move)
                screen.position = Handles.PositionHandle(screen.position, Quaternion.Euler(screen.rotation));

            if (Tools.current == Tool.Rotate)
                screen.rotation = Handles.RotationHandle(Quaternion.Euler(screen.rotation), screen.position).eulerAngles;
        }

        if (EditorGUI.EndChangeCheck())
        {
            Repaint();
        }
    }
}
