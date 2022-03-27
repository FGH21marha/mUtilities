// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities/Editor)

using System;
using UnityEngine;

[Serializable]
public class mEditorInput
{
    private bool LMDown => Event.current.type == EventType.MouseDown && Event.current.button == 0;
    private bool LMUp => Event.current.type == EventType.MouseUp && Event.current.button == 0;
    private bool isLMHeld;
    private bool isLMDown;

    private bool RMDown => Event.current.type == EventType.MouseDown && Event.current.button == 1;
    private bool RMUp => Event.current.type == EventType.MouseUp && Event.current.button == 1;
    private bool isRMHeld;
    private bool isRMDown;

    private bool SWDown => Event.current.type == EventType.MouseDown && Event.current.button == 2;
    private bool SWUp => Event.current.type == EventType.MouseUp && Event.current.button == 2;
    private bool isSWHeld;
    private bool isSWDown;

    public bool GetMouseDown(int button)
    {
        if (button == 0) return isLMDown;
        if (button == 1) return isRMDown;
        if (button == 2) return isSWDown;

        return false;
    }
    public bool GetMouse(int button)
    {
        if (button == 0) return isLMHeld;
        if (button == 1) return isRMHeld;
        if (button == 2) return isSWHeld;

        return false;
    }
    public bool GetMouseUp(int button)
    {
        if (button == 0) return LMUp;
        if (button == 1) return RMUp;
        if (button == 2) return SWUp;

        return false;
    }

    public void Update()
    {
        #region Left Mouse
        if (LMDown && !isLMHeld)
        {
            isLMHeld = true;
            isLMDown = true;
        }
        else
        {
            isLMDown = false;
        }

        if (LMUp)
            isLMHeld = false;
        #endregion

        #region Right Mouse
        if (RMDown && !isRMHeld)
        {
            isRMHeld = true;
            isRMDown = true;
        }
        else
        {
            isRMDown = false;
        }

        if (RMUp)
            isRMHeld = false;
        #endregion

        #region Right Mouse
        if (SWDown && !isSWHeld)
        {
            isSWHeld = true;
            isSWDown = true;
        }
        else
        {
            isSWDown = false;
        }

        if (SWUp)
            isSWHeld = false;
        #endregion
    }
}
