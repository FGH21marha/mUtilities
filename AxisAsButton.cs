// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

using UnityEngine;
using System;

//A struct to allow triggers or thumbsticks to be used as buttons
[Serializable] public struct AxisAsButton
{
    public string AxisButton; // "Horizontal" or "Vertical" for example
    public float TriggerThreshhold; // At what value the axis should return true

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

    bool AxisValueThisFrame; //The state of this axis this frame
    bool AxisValueLastFrame; //The state of this axis last frame

    //Only returns true when the axis value becomes greater than the threshhold
    public bool GetAxisDown()
    {
        GetAxisChanged();
        return axisDown;
    }
    public bool GetAxisDown(Action onAxisDown)
    {
        GetAxisChanged();

        if(axisDown)
            onAxisDown?.Invoke();

        return axisDown;
    }

    //Only returns true when the axis value becomes smaller than the threshhold
    public bool GetAxisUp()
    {
        GetAxisChanged();
        return axisUp;
    }
    public bool GetAxisUp(Action onAxisUp)
    {
        GetAxisChanged();

        if (axisUp)
            onAxisUp?.Invoke();

        return axisUp;
    }

    //Returns true every frame the axis value is greater than the threshhold
    public bool GetAxisHold()
    {
        GetAxisChanged();

        bool value = Input.GetAxisRaw(AxisButton) > TriggerThreshhold;

        axisHold = value;
        onAxisHold?.Invoke(axisHold);

        return value;
    }
    public bool GetAxisHold(Action<bool> onAxisHold)
    {
        GetAxisChanged();

        bool value = Input.GetAxisRaw(AxisButton) > TriggerThreshhold;

        onAxisHold?.Invoke(value);
        axisHold = value;

        return value;
    }

    //Returns the current direction of the axis
    public float GetAxisValue()
    {
        GetAxisChanged();

        float value = Input.GetAxisRaw(AxisButton);

        onAxisValue?.Invoke(value);

        return value;
    }
    public float GetAxisValue(Action<float> onAxisValue)
    {
        GetAxisChanged();

        float value = Input.GetAxisRaw(AxisButton);

        onAxisValue?.Invoke(value);

        return value;
    }

    //Only returns true on the frame when the axis value changes
    public bool GetAxisChanged()
    {
        //Create a temporary bool to store the comparison of the axis last frame and this frame
        bool axisButtonTriggered;

        //Create a temporary float to store the current value of the axis this frame
        float axisValue = Input.GetAxisRaw(AxisButton);

        //Reset axis variables
        axisDown = false;
        axisUp = false;

        //Set the current value of the axis
        AxisValueThisFrame = axisValue > TriggerThreshhold;

        //Set our temporary bool to true if the value changed from one frame to the next
        if (AxisValueLastFrame != AxisValueThisFrame)
        {
            axisButtonTriggered = true;

            //Check the direction of the axis 
            if(axisValue > TriggerThreshhold)
            {
                //Set axisDown to true if the axis value is greater than the threshold
                axisDown = true;
                onAxisDown?.Invoke();
            }
            else
            {
                //Set axisUp to true if the axis value is smaller than the threshold
                axisUp = true;
                onAxisUp?.Invoke();
            }

            onAxisChanged?.Invoke();
        }

        //Set our temporary to false if the value remains unchanged
        else axisButtonTriggered = false;

        //Store last frames axis for comparison next update
        AxisValueLastFrame = AxisValueThisFrame;

        return axisButtonTriggered;
    }
}
