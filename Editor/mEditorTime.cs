// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities/Editor)

public class mEditorTime
{
    double editorDeltaTime = 0f;
    double lastTimeSinceStartup = 0f;
    float framecountCheck = 0f;

    public float time;
    public double doubleTime;
    public float deltaTime => (float)editorDeltaTime;
    public double deltaTimeAsDouble => editorDeltaTime;

    public mEditorTime Update()
    {
        if (lastTimeSinceStartup == 0f)
            lastTimeSinceStartup = EditorApplication.timeSinceStartup;

        editorDeltaTime = EditorApplication.timeSinceStartup - lastTimeSinceStartup;
        lastTimeSinceStartup = EditorApplication.timeSinceStartup;

        time += deltaTime;
        doubleTime += deltaTimeAsDouble;
        framecountCheck += deltaTime;

        return this;
    }

    public mEditorTime Update(EditorWindow window, float frameCount)
    {
        Update().Repaint(window, frameCount);
        return this;
    }

    public mEditorTime Repaint(EditorWindow window, float frameCount)
    {
        if(framecountCheck > 1f / frameCount)
        {
            framecountCheck = 0f;
            window.Repaint();
        }

        return this;
    }
}
