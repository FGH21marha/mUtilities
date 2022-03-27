// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities/Editor)

#if UNITY_EDITOR

using UnityEditor;

public class mEditorTime
{
    double editorDeltaTime = 0f;
    double lastTimeSinceStartup = 0f;
    float framecountCheck = 0f;

    public float time;
    public double timeAsDouble;
    public float deltaTime => (float)editorDeltaTime;
    public double deltaTimeAsDouble => editorDeltaTime;

    public mEditorTime Update(EditorWindow window, float frameCount)
    {
        UpdateTime().Repaint(window, frameCount);
        return this;
    }
    public mEditorTime UpdateTime()
    {
        if (lastTimeSinceStartup == 0f)
            lastTimeSinceStartup = EditorApplication.timeSinceStartup;

        editorDeltaTime = EditorApplication.timeSinceStartup - lastTimeSinceStartup;
        lastTimeSinceStartup = EditorApplication.timeSinceStartup;

        time += deltaTime;
        timeAsDouble += deltaTimeAsDouble;
        framecountCheck += deltaTime;

        return this;
    }
    public mEditorTime Repaint(EditorWindow window, float frameCount)
    {
        if (framecountCheck > 1f / frameCount)
        {
            framecountCheck = 0f;
            window.Repaint();
        }

        return this;
    }
}
#endif
