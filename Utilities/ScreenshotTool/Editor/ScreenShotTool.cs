// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

public class ScreenShotTool : EditorWindow
{
    #region Window
    [MenuItem("Screenshot/Open Screenshot window")]
    public static void OpenWindow()
    {
        EditorWindow window = (ScreenShotTool)GetWindow(typeof(ScreenShotTool));
        window.minSize = new Vector2(400, 200);
        window.autoRepaintOnSceneChange = true;
        window.Show();
    }

    public ScreenShotProfile profile;
    public ReorderableList cameras;
    Vector2 scrollPos;
    bool editingCenter;

    private void OnDestroy()
    {
        if (GameObject.Find("previewCamera"))
            DestroyImmediate(GameObject.Find("previewCamera"));

        SceneView.RepaintAll();
    }

    private void OnFocus()
    {
        SceneView.RepaintAll();
    }
    private void OnLostFocus()
    {
        if (GameObject.Find("previewCamera"))
            DestroyImmediate(GameObject.Find("previewCamera"));

        SceneView.RepaintAll();
        EditorUtility.UnloadUnusedAssetsImmediate();
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        profile = (ScreenShotProfile)EditorGUILayout.ObjectField(profile, typeof(ScreenShotProfile), false);
        if (EditorGUI.EndChangeCheck())
        {
            if (GameObject.Find("previewCamera"))
                DestroyImmediate(GameObject.Find("previewCamera"));

            SceneView.RepaintAll();
            InitializeList();
        }

        if (profile == null)
            return;

        if (cameras == null)
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
                var oldPos = profile.screenshotOrigin;
                EditorGUILayout.LabelField("position", GUILayout.Width(60));
                profile.screenshotOrigin = EditorGUILayout.Vector3Field("", profile.screenshotOrigin);

                if (GUI.changed && !editingCenter)
                {
                    var newPos = profile.screenshotOrigin;
                    var delta = oldPos - newPos;

                    foreach (var screens in profile.screenshots)
                        screens.position -= delta;
                }

                editingCenter = EditorGUILayout.Toggle(editingCenter, GUILayout.Width(16));
                EditorGUILayout.LabelField("Edit Center", GUILayout.Width(80));
            }

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
            var i = new ScreenShotTransform();
            i.position = profile.screenshotOrigin;
            mArray.Add(ref profile.screenshots, i);
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
                Resources.UnloadUnusedAssets();
            }
        };
        cameras.elementHeightCallback = (int index) =>
        {
            ScreenShotTransform screen = profile.screenshots[index];

            float y = profile.Width / 10;

            if (screen.expanded && !screen.preview)
                return 174f;
            if (screen.expanded && screen.preview)
                return 178f + (y * Mathf.Clamp(profile.previewSize, 0, 1));

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

                int height = screen.preview ? 158 + (int)(y * Mathf.Clamp(profile.previewSize, 0, 1)) : 154;

                GUI.Box(new Rect(rect.x, rect.y + 20, rect.width, height), "", EditorStyles.helpBox);
                GUI.backgroundColor = bgColor;

                if(GUI.Button(new Rect(rect.x + 4, rect.y + 22, rect.width, 20), "Paste Scene Camera Values"))
                {
                    CopySceneCameraValues(screen);
                }

                EditorGUI.LabelField(new Rect(rect.x + 4, rect.y + 42, 100, 20), "Position");
                screen.position = EditorGUI.Vector3Field(new Rect(rect.x + 78, rect.y + 42, rect.width - 80, 20), "", screen.position);

                EditorGUI.LabelField(new Rect(rect.x + 4, rect.y + 62, 80, 20), "Rotation");
                screen.rotation = EditorGUI.Vector3Field(new Rect(rect.x + 78, rect.y + 62, rect.width - 80, 20), "", screen.rotation);

                EditorGUI.LabelField(new Rect(rect.x + 4, rect.y + 86, 80, 20), "Ortographic");
                screen.ortho = EditorGUI.Toggle(new Rect(rect.x + 78, rect.y + 86, 20, 20), screen.ortho);

                if (screen.ortho)
                {
                    EditorGUI.LabelField(new Rect(rect.x + 98, rect.y + 86, 80, 20), "Ortho");
                    screen.orthoSize = EditorGUI.Slider(new Rect(rect.x + 148, rect.y + 86, rect.width - 150, 20), screen.orthoSize, 0.1f, 150f);
                }
                else
                {
                    EditorGUI.LabelField(new Rect(rect.x + 98, rect.y + 86, 80, 20), "FOV");
                    screen.fov = EditorGUI.Slider(new Rect(rect.x + 148, rect.y + 86, rect.width - 150, 20), screen.fov, 0.1f, 180f);
                }

                EditorGUI.LabelField(new Rect(rect.x + 4, rect.y + 106, 80, 20), "Camera Planes");
                screen.clippingPlanes = EditorGUI.Vector2Field(new Rect(rect.x + 94, rect.y + 106, rect.width - 80, 20), "", screen.clippingPlanes);

                EditorGUI.LabelField(new Rect(rect.x + 4, rect.y + 130, 80, 20), "Background");
                screen.screenShotBackground = (ScreenShotBackground)EditorGUI.EnumPopup(new Rect(rect.x + 94, rect.y + 130, 90, 20), screen.screenShotBackground);

                if (screen.screenShotBackground == ScreenShotBackground.Solid)
                    screen.backgroundColor = EditorGUI.ColorField(new Rect(rect.x + 190, rect.y + 130, rect.width - 192, 18), screen.backgroundColor);

                screen.preview = EditorGUI.Foldout(new Rect(rect.x + 4, rect.y + 154, rect.width, 18), screen.preview, new GUIContent("Preview"), true);

                if (screen.preview)
                {
                    using (var offset = new GUI.ScrollViewScope(new Rect(rect.x + 3, rect.y + 176, rect.width - 10, height - 120), screen.previewOffset, new Rect(rect.x + 3, rect.y + 176, (int)(rect.width - 10) * profile.previewSize, (height - 120) * profile.previewSize)))
                    {
                        screen.previewOffset = offset.scrollPosition;

                        GUI.backgroundColor = Color.white;
                        var c = GUI.color;
                        GUI.color = Color.white;

                        PreviewCamera preview = screen.previewTexture;
                        preview.GetPicture(screen, new Rect(0, 0, x, y), (int)x, (int)y);

                        GUI.DrawTexture(new Rect(rect.x + 3, rect.y + 176, x * profile.previewSize, y * profile.previewSize), preview.GetTexture());

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
    private void CopySceneCameraValues(ScreenShotTransform screen)
    {
        Camera t = SceneView.lastActiveSceneView.camera;
        screen.position = t.transform.position;
        screen.rotation = t.transform.eulerAngles;
        screen.ortho = t.orthographic;
        screen.orthoSize = t.orthographicSize;
        screen.fov = t.fieldOfView;
        screen.backgroundColor = t.backgroundColor;

        switch (t.clearFlags)
        {
            case CameraClearFlags.SolidColor: screen.screenShotBackground = ScreenShotBackground.Solid; break;
            case CameraClearFlags.Skybox: screen.screenShotBackground = ScreenShotBackground.Skybox; break;
            case CameraClearFlags.Nothing: screen.screenShotBackground = ScreenShotBackground.Transparent; break;
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
    #endregion

    #region SceneView
    private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    bool HasFocus(SceneView sv) => sv.drawGizmos && (sv.hasFocus || hasFocus);
    void OnSceneGUI(SceneView sv)
    {
        if (!HasFocus(sv) || profile == null) return;

        var lastPos = profile.screenshotOrigin;
        var newPos = Handles.PositionHandle(profile.screenshotOrigin, Quaternion.identity);
        var delta = lastPos - newPos;
        profile.screenshotOrigin = newPos;

        var e = Event.current;

        if (e != null)
        {
            editingCenter = e.alt;
        }

        if (Mathf.Abs(delta.magnitude) > 0.001f)
            Repaint();

        for (int i = 0; i < profile.screenshots.Length; i++)
        {
            if(!editingCenter)
                profile.screenshots[i].position -= delta;

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

        Handles.color = new Color(1,1,1,0.2f);
        Handles.DrawLine(screen.position, new Vector3(screen.position.x, profile.screenshotOrigin.y, screen.position.z));
        Handles.DrawLine(profile.screenshotOrigin, new Vector3(screen.position.x, profile.screenshotOrigin.y, screen.position.z));

        RaycastHit hit;
        Physics.Raycast(screen.position, forward, out hit);

        if (hit.collider != null)
        {
            Handles.color = Color.green;
            Handles.DrawSolidDisc(hit.point + (hit.normal * 0.002f), hit.normal, 0.05f);
            Handles.color = new Color(0, 1, 0, 0.5f);
            Handles.DrawDottedLine(screen.position, hit.point, 2f);
            //Handles.DrawLine(screen.position, hit.point);
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
            {
                screen.rotation = Handles.RotationHandle(Quaternion.Euler(screen.rotation), screen.position).eulerAngles;
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(profile);
            SceneView.RepaintAll();
            Repaint();
        }
    }
    #endregion

    #region Cleanup
    [MenuItem("Screenshot/Cleanup Project")]
    public static void CleanupProject()
    {
        mEditor.ModalWindow(
            "Cleanup Project",
            "This action will find all hidden gameobjects in the scene and remove them",
            "Cleanup",
            "Cancel",
            TryCleanup,
            null);
    }
    static void TryCleanup()
    {
        Debug.Log("Cleanup started...");

        var objects = EditorSceneManager.GetActiveScene().GetRootGameObjects();

        GameObject[] hiddenObjects = new GameObject[0];

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].hideFlags != HideFlags.None)
            {
                Debug.Log("Found " + objects[i].name + " as hidden");
                hiddenObjects = hiddenObjects.Add(objects[i]);
            }
        }

        if (hiddenObjects.Length > 0)
        {
            string names = "";

            for (int i = 0; i < names.Length; i++)
                names += hiddenObjects[i].name + "\n";

            mEditor.ModalWindow(
            "Found hidden items",
            "Do you wish to remove:" + names,
            "Cleanup",
            "Cancel",
            () => DeleteHiddenItems(hiddenObjects),
            null);
        }

        Debug.Log("Cleanup Finished");
    }

    static void DeleteHiddenItems(GameObject[] objects)
    {
        for (int i = 0; i < objects.Length; i++)
            DestroyImmediate(objects[i]);
    }
    #endregion
}
