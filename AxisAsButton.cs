// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

#region Namespaces

using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

#endregion

#region AxisAsButton Struct

//A struct to allow joysticks, thumbsticks or triggers to be used as buttons
[Serializable] public class AxisAsButton
{
    public string AxisName; // "Horizontal" or "Vertical" for example
    public float TriggerThreshold; // At what value the axis should return true

    public bool Down { get { return axisDown; } } //Axis was pressed this frame
    public bool Up { get { return axisUp; } } //Axis was released this frame
    public bool Hold { get { return axisHold; } } //Axis is being held

    public event Action onAxisDown; //Called when the axis is pressed
    public event Action onAxisUp; //Called when the axis is released
    public event Action onAxisChanged; //Called when the axis changes
   
    public event Action<float> onAxisValue; //Returns the float value of the axis each frame GetAxisChanged() is called
    public event Action<bool> onAxisHold; //Returns the bool value of the axis each frame GetAxisChanged() is called

    bool axisDown; //Axis was pressed this frame
    bool axisUp; //Axis was released this frame
    bool axisHold; //Axis is being held

    bool ValueGreaterThisFrame; //The axis greater than threshold this frame
    bool ValueGreaterLastFrame; //The axis greater than threshold last frame

    bool ValueSmallerThisFrame; //The axis smaller than threshold this frame
    bool ValueSmallerLastFrame; //The axis smaller than threshold last frame

    //Manually update axis information
    public void UpdateAxisVariables()
    {
        GetValueDown();
        GetValueUp();
        GetAxisHold();
        GetAxisValue();
    }

    //Returns true on the frame the axis value becomes greater than the threshhold
    public bool GetAxisDown()
    {
        GetValueDown();
        return axisDown;
    }
    public bool GetAxisDown(Action onAxisDown)
    {
        GetValueDown();

        if (axisDown)
            onAxisDown?.Invoke();

        return axisDown;
    }

    //Returns true on the frame the axis value becomes smaller than the threshhold
    public bool GetAxisUp()
    {
        GetValueUp();
        return axisUp;
    }
    public bool GetAxisUp(Action onAxisUp)
    {
        GetValueUp();

        if (axisUp)
            onAxisUp?.Invoke();

        return axisUp;
    }

    //Returns true every frame the axis value is greater than the threshhold
    public bool GetAxisHold()
    {
        bool value = Mathf.Abs(Input.GetAxisRaw(AxisName)) > TriggerThreshold;

        axisHold = value;
        onAxisHold?.Invoke(axisHold);

        return value;
    }
    public bool GetAxisHold(Action<bool> onAxisHold)
    {
        bool value = Mathf.Abs(Input.GetAxisRaw(AxisName)) > TriggerThreshold;

        axisHold = value;
        onAxisHold?.Invoke(axisHold);
        this.onAxisHold?.Invoke(axisHold);

        return value;
    }

    //Returns the current direction of the axis
    public float GetAxisValue()
    {
        float value = Input.GetAxisRaw(AxisName);

        onAxisValue?.Invoke(value);

        return value;
    }
    public float GetAxisValue(Action<float> onAxisValue)
    {
        float value = Input.GetAxisRaw(AxisName);

        onAxisValue?.Invoke(value);
        this.onAxisValue?.Invoke(value);

        return value;
    }

    //Update the current direction of the axis
    bool GetValueDown()
    {
        //Reset axis variable
        axisDown = false;

        //Create a temporary bool to store the comparison of the axis last frame and this frame
        bool axisButtonTriggered;

        //Create a temporary float to store the current value of the axis this frame
        float axisValue = Mathf.Abs(Input.GetAxisRaw(AxisName));

        //Set the current value of the axis
        ValueGreaterThisFrame = axisValue > TriggerThreshold;

        //Set our temporary bool to true if the value changed from one frame to the next
        if (ValueGreaterLastFrame != ValueGreaterThisFrame)
        {
            axisButtonTriggered = true;
            onAxisChanged?.Invoke();

            //Check if the axis value is greater than the threshold
            if (axisValue > TriggerThreshold)
            {
                axisDown = true;
                onAxisDown?.Invoke();
            }
        }

        //Set our temporary to false if the value remains unchanged
        else axisButtonTriggered = false;

        //Store last frames axis for comparison next update
        ValueGreaterLastFrame = ValueGreaterThisFrame;

        return axisButtonTriggered;
    }
    bool GetValueUp()
    {
        //Reset axis variable
        axisUp = false;

        //Create a temporary bool to store the comparison of the axis last frame and this frame
        bool axisButtonTriggered;

        //Create a temporary float to store the current value of the axis this frame
        float axisValue = Mathf.Abs(Input.GetAxisRaw(AxisName));

        //Set the current value of the axis
        ValueSmallerThisFrame = axisValue > TriggerThreshold;

        //Set our temporary bool to true if the value changed from one frame to the next
        if (ValueSmallerLastFrame != ValueSmallerThisFrame)
        {
            axisButtonTriggered = true;
            onAxisChanged?.Invoke();

            //Check if the axis value is smaller than the threshold
            if (axisValue < TriggerThreshold)
            {
                axisUp = true;
                onAxisUp?.Invoke();
            }
        }

        //Set our temporary to false if the value remains unchanged
        else axisButtonTriggered = false;

        //Store last frames axis for comparison next update
        ValueSmallerLastFrame = ValueSmallerThisFrame;

        return axisButtonTriggered;
    }
}

#endregion

#region Property Drawer

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(AxisAsButton))]
public class AxisAsButtonDrawer : PropertyDrawer
{
    //Override OnGUI and populate the inspector with our own fields
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Begin new PropertyScope
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            //Calculate rects
            //var TitleRect = new Rect(position.x, position.y, position.width, 18);
            var AxisNameRect = new Rect(position.x, position.y, position.width, 18);
            var TriggerThreshholdRect = new Rect(position.x, position.y + 20, position.width, 18);

            //Draw fields
            //EditorGUI.LabelField(TitleRect, label);
            EditorGUI.PropertyField(AxisNameRect, property.FindPropertyRelative("AxisName"), new GUIContent(label));
            EditorGUI.PropertyField(TriggerThreshholdRect, property.FindPropertyRelative("TriggerThreshold"), new GUIContent("Threshold"));
        }
    }

    //Set PropertyHeight of AxisAsButton
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 40f;
}
#endif

#endregion
