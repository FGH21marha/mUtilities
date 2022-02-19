// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

//A struct to allow triggers or thumbsticks to be used as buttons
[System.Serializable] public struct AxisAsButton
{
    private bool AxisUpThisFrame; //The state of this axis this frame
    private bool AxisUpLastFrame; //The state of this axis last frame

    public string AxisButton; // "Horizontal" or "Vertical" for example
    public float TriggerThreshhold; // At what value the axis should return true

    //Only returns true on the frame when the axis value becomes greater than the threshhold
    public bool GetAxisDown()
    {
        //Create a temporary bool to store the comparison of the axis last frame and this frame
        bool axisButtonTriggered;

        //Set the current value of the axis
        AxisUpThisFrame = Input.GetAxisRaw(AxisButton) < TriggerThreshhold;

        //Set our temporary bool to true if the value changed from one frame to the next
        if (AxisUpLastFrame != AxisUpThisFrame) axisButtonTriggered = true;

        //Set our temporary to false if the value remains unchanged
        else axisButtonTriggered = false;

        //Store last frames axis for comparison next update
        AxisUpLastFrame = AxisUpThisFrame;

        return axisButtonTriggered;
    }

    //Returns true for every frame the axis value is greater than the threshold
    public bool GetAxis() => Input.GetAxisRaw(AxisButton) > TriggerThreshhold;

    //Return the raw float value of the axis
    public float GetAxisValue() => Input.GetAxisRaw(AxisButton);
}
