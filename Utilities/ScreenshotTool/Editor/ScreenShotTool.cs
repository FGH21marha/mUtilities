// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

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

    Vector2 scrollPos;

    public ReorderableList cameras;

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        profile = (ScreenShotProfile)EditorGUILayout.ObjectField(profile, typeof(ScreenShotProfile), false);
        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
            InitializeList();
        }

        if (profile == null)
            return;

        if(cameras == null)
            InitializeList();

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

        cameras.onRemoveCallback = (ReorderableList list) =>
        {
            mArray.Remove(ref profile.screenshots, profile.screenshots.Length);
            list.index--;
            InitializeList();
            SceneView.RepaintAll();
        };
        cameras.onAddCallback = (ReorderableList list) =>
        {
            mArray.Add(ref profile.screenshots, new ScreenShotTransform());
            list.index++;

            InitializeList();
            SceneView.RepaintAll();
        };
        cameras.drawHeaderCallback = (Rect rect) => 
        { 
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 80, rect.height), "Cameras", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();

            EditorGUI.LabelField(new Rect(rect.width - 188, rect.y, 80, rect.height), "Preview");
            profile.previewSize = EditorGUI.Slider(new Rect(rect.width - 128, rect.y, 140, rect.height), profile.previewSize, 0.1f, 2f);

            if (EditorGUI.EndChangeCheck())
            {
                InitializeList();
                EditorUtility.SetDirty(profile);
                SceneView.RepaintAll();
            }
        };
        cameras.elementHeightCallback = (int index) =>
        {
            ScreenShotTransform screen = profile.screenshots[index];

            float y = profile.Width / 10;

            if (screen.expanded && !screen.preview)
                return 134f;
            if (screen.expanded && screen.preview)
                return 138f + (y * Mathf.Clamp(profile.previewSize, 0, 1));

            return 20f;
        };
        cameras.drawElementCallback = (Rect r, int index, bool isActive, bool isFocused) =>
        {
            Rect rect = new Rect(r.x - 18, r.y, r.width + 22, r.height);
            ScreenShotTransform screen = profile.screenshots[index];
            var bgColor = GUI.backgroundColor;

            screen.expanded = EditorGUI.Foldout(new Rect(r.x, r.y, 80, 18), screen.expanded, new GUIContent("Camera " + index.ToString()), true);

            if (screen.expanded)
            {
                GUI.backgroundColor = Color.black;

                float y = profile.Width / 10;
                float x = y * profile.Width / profile.Height;

                int height = screen.preview ? 118 + (int)(y * Mathf.Clamp(profile.previewSize, 0, 1)) : 114;

                GUI.Box(new Rect(rect.x, rect.y + 20, rect.width, height), "", EditorStyles.helpBox);     
                GUI.backgroundColor = bgColor;

                EditorGUI.LabelField(new Rect(rect.x + 4, rect.y + 22, 100, 20),"Position");
                screen.position = EditorGUI.Vector3Field(new Rect(rect.x + 78, rect.y + 22, rect.width - 80, 20),"", screen.position);

                EditorGUI.LabelField(new Rect(rect.x + 4, rect.y + 42, 80, 20), "Rotation");
                screen.rotation = EditorGUI.Vector3Field(new Rect(rect.x + 78, rect.y + 42, rect.width - 80, 20), "", screen.rotation);

                EditorGUI.LabelField(new Rect(rect.x + 4, rect.y + 66, 80, 20), "Ortographic");
                screen.ortho = EditorGUI.Toggle(new Rect(rect.x + 78, rect.y + 66, 20, 20), screen.ortho);

                if (screen.ortho)
                {
                    EditorGUI.LabelField(new Rect(rect.x + 98, rect.y + 66, 80, 20), "Ortho");
                    screen.orthoSize = EditorGUI.Slider(new Rect(rect.x + 148, rect.y + 66, rect.width - 150, 20), screen.orthoSize, 0.1f, 50f);
                }
                else
                {
                    EditorGUI.LabelField(new Rect(rect.x + 98, rect.y + 66, 80, 20), "FOV");
                    screen.fov = EditorGUI.Slider(new Rect(rect.x + 148, rect.y + 66, rect.width - 150, 20), screen.fov, 0.1f, 180f);
                }

                EditorGUI.LabelField(new Rect(rect.x + 4, rect.y + 90, 80, 20),"Background");
                screen.screenShotBackground = (ScreenShotBackground)EditorGUI.EnumPopup(new Rect(rect.x + 94, rect.y + 90, 90, 20),screen.screenShotBackground);

                if (screen.screenShotBackground == ScreenShotBackground.Solid)
                    screen.backgroundColor = EditorGUI.ColorField(new Rect(rect.x + 190, rect.y + 90, rect.width - 192, 18), screen.backgroundColor);

                screen.preview = EditorGUI.Foldout(new Rect(rect.x + 4, rect.y + 114, rect.width, 18), screen.preview, new GUIContent("Preview"), true);

                using (var offset = new GUI.ScrollViewScope(new Rect(rect.x + 3, rect.y + 136, rect.width - 10, height - 120), screen.previewOffset, new Rect(rect.x + 3, rect.y + 136, (int)(rect.width - 10) * profile.previewSize, (height - 120) * profile.previewSize)))
                {
                    screen.previewOffset = offset.scrollPosition;
                    if (screen.preview)
                    {

                        GUI.backgroundColor = Color.white;
                        var c = GUI.color;
                        GUI.color = Color.white;

                        PreviewCamera preview = screen.previewTexture;
                        preview.GetPicture(screen, new Rect(0, 0, x, y), (int)x, (int)y);

                        GUI.DrawTexture(new Rect(rect.x + 3, rect.y + 136, x * profile.previewSize, y * profile.previewSize), preview.GetTexture());

                        GUI.color = c;
                        GUI.backgroundColor = bgColor;
                    }
                }
            }
        };

        EditorGUILayout.Space();

        using (var pos = new GUILayout.ScrollViewScope(scrollPos))
        {
            scrollPos = pos.scrollPosition;
            cameras.DoLayoutList();
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(profile);
            SceneView.RepaintAll();
        }
    }

    private void InitializeList()
    {
        cameras = new ReorderableList(profile.screenshots, typeof(ScreenShotTransform), true, true, true, true);
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

        for (int i = 0; i < profile.screenshots.Length; i++)
        {
            DrawScreenshotInScene(sv, profile.screenshots[i], i);
        }
    }

    void DrawScreenshotInScene(SceneView sv, ScreenShotTransform screen, int i)
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

        Handles.Label(screen.position + sv.camera.transform.right, "Camera " + i.ToString());

        EditorGUI.BeginChangeCheck();

        Undo.RecordObject(profile, "Screenshot camera transformation");

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
            EditorUtility.SetDirty(profile);
            SceneView.RepaintAll();
            Repaint();
        }
    }
}
