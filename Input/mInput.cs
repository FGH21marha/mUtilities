// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities/Input)
// NOTE: Requires AxisAsButton.cs (https://github.com/FGH21marha/mUtilities/blob/main/AxisAsButton.cs)

#region Packages
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
#endregion

#region Class

[Serializable]
public class mInput
{
    public bool RequireAll = false;

    public event Action<bool> onInput;
    public event Action<float> onInputFloat;
    public event Action<Vector2> onInputVector2;
    public event Action<Vector3> onInputVector3;

    public List<Combination> combination = new List<Combination>();
    public List<Combination> GetCombinations() => combination;
    public void ForEachInputType(Action<Combination> forEveryCombination)
    {
        foreach (Combination combination in combination)
            forEveryCombination?.Invoke(combination);
    }
    public bool GetCombination()
    {
        bool[] keys = new bool[combination.Count];

        if (keys.Length == 0) return false;

        bool success;

        if (RequireAll)
            success = RequireAllKeys(keys);
        else
            success = RequireAnyKey(keys);

        if (success)
            onInput?.Invoke(success);

        return success;
    }

    public T GetInput<T>(object identifier)
    {
        string stringID = null;
        KeyCode? keyID = null;
        int? intID = null;

        if (identifier is string)
            stringID = identifier as string;

        if (identifier is KeyCode)
            keyID = (KeyCode)identifier;

        if (identifier is int)
            intID = (int)identifier;

        if(stringID == null && keyID == null && intID == null) return default(T);

        if (typeof(T) == typeof(bool))
        {
            if (combination.Count == 0) return default(T);

            if (stringID != null || stringID != "")
            {
                foreach (Combination combination in combination)
                {
                    if (stringID == combination.axis.AxisName)
                    {
                        switch (combination.Keypress)
                        {
                            case mKeyPress.Down:
                                onInput?.Invoke(combination.axis.GetAxisDown());
                                return (T)(object)combination.axis.GetAxisDown();
                            case mKeyPress.Up:
                                onInput?.Invoke(combination.axis.GetAxisUp());
                                return (T)(object)combination.axis.GetAxisUp();
                            case mKeyPress.Hold:
                                onInput?.Invoke(combination.axis.GetAxisHold());
                                return (T)(object)combination.axis.GetAxisHold();
                        }
                    }
                }
            }

            if(keyID != KeyCode.None)
            {
                foreach (Combination combination in combination)
                {
                    if (keyID == combination.key)
                    {
                        switch (combination.Keypress)
                        {
                            case mKeyPress.Down:
                                onInput?.Invoke(Input.GetKeyDown(combination.key));
                                return (T)(object)Input.GetKeyDown(combination.key);
                            case mKeyPress.Up:
                                onInput?.Invoke(Input.GetKeyUp(combination.key));
                                return (T)(object)Input.GetKeyUp(combination.key);
                            case mKeyPress.Hold:
                                onInput?.Invoke(Input.GetKey(combination.key));
                                return (T)(object)Input.GetKey(combination.key);
                        }
                    }
                }
            }

            for (int i = 0; i < combination.Count; i++)
            {
                if (combination[i].type == mInputType.Mouse)
                    return (T)(object)GetMouseInput(i);
            }
        }

        if (typeof(T) == typeof(float))
        {
            if (combination.Count == 0) return default(T);

            for (int i = 0; i < combination.Count; i++)
            {
                if (stringID != null || stringID != "")
                {
                    if (stringID == combination[i].mAxis.AxisName)
                    {
                        onInputFloat?.Invoke(combination[i].mAxis.GetValue());
                        return (T)(object)combination[i].mAxis.GetValue();
                    }

                    if (stringID == combination[i].mAxis.GetName())
                    {
                        combination[i].mAxis.SetValue(Input.GetAxisRaw(combination[i].mAxis.GetName()));
                        onInputFloat?.Invoke(combination[i].mAxis.GetValue());
                        return (T)(object)combination[i].mAxis.GetValue();
                    }
                }
                if (intID != null)
                {
                    if (intID == i)
                    {
                        if(combination[i].type == mInputType.Axis)
                        {
                            onInputFloat?.Invoke(combination[i].axis.GetAxisValue());
                            return (T)(object)combination[i].axis.GetAxisValue();
                        }
                        if(combination[i].type == mInputType.AxisButton)
                        {
                            combination[i].mAxis.SetValue(Input.GetAxisRaw(combination[i].mAxis.GetName()));
                            onInputFloat?.Invoke(combination[i].mAxis.GetValue());
                            return (T)(object)combination[i].mAxis.GetValue();
                        }
                    }
                }
            }
        }

        if (typeof(T) == typeof(Vector2))
        {
            if (combination.Count == 0) return default(T);

            for (int i = 0; i < combination.Count; i++)
            {
                if(stringID != null || stringID != "")
                {
                    if (stringID == combination[i].vector2.Vector2Name)
                    {
                        onInputVector2?.Invoke(combination[i].vector2.GetValue());
                        return (T)(object)combination[i].vector2.GetValue();
                    }
                }
                if(intID != null)
                {
                    if (intID == i)
                    {
                        onInputVector2?.Invoke(combination[i].vector2.GetValue());
                        return (T)(object)combination[i].vector2.GetValue();
                    }
                }
            }
        }

        if (typeof(T) == typeof(Vector3))
        {
            if (combination.Count == 0) return default(T);

            for (int i = 0; i < combination.Count; i++)
            {
                if (stringID != null || stringID != "")
                {
                    if (stringID == combination[i].vector3.Vector3Name)
                    {
                        onInputVector3?.Invoke(combination[i].vector3.GetValue());
                        return (T)(object)combination[i].vector3.GetValue();
                    }
                }
                if (intID != null)
                {
                    if (intID == i)
                    {
                        onInputVector3?.Invoke(combination[i].vector3.GetValue());
                        return (T)(object)combination[i].vector3.GetValue();
                    }
                }
            }
        }

        if (typeof(T) == typeof(AxisAsButton))
        {
            if (combination.Count == 0) return default(T);

            for (int i = 0; i < combination.Count; i++)
            {
                if (stringID != "")
                {
                    if (stringID == combination[i].axis.AxisName)
                    {
                        onInput?.Invoke(combination[i].axis.GetAxisHold());
                        return (T)(object)combination[i].axis;
                    }
                }
                if (intID != null)
                {
                    if (intID == i)
                    {
                        onInput?.Invoke(combination[i].axis.GetAxisHold());
                        return (T)(object)combination[i].axis;
                    }
                }
            }
        }

        return default(T);
    }

    public bool GetInput(object ID)
    {
        if (IdentifierNotNull(ID, out string stringID, out KeyCode? keyID, out int? intID)) return false;

        if (stringID != "")
        {
            foreach (Combination combination in combination)
            {
                if (stringID == combination.axis.AxisName)
                {
                    switch (combination.Keypress)
                    {
                        case mKeyPress.Down:
                            onInput?.Invoke(combination.axis.GetAxisDown());
                            return combination.axis.GetAxisDown();
                        case mKeyPress.Up:
                            onInput?.Invoke(combination.axis.GetAxisUp());
                            return combination.axis.GetAxisUp();
                        case mKeyPress.Hold:
                            onInput?.Invoke(combination.axis.GetAxisHold());
                            return combination.axis.GetAxisHold();
                    }
                }
            }
        }

        if (keyID != KeyCode.None)
        {
            foreach (Combination combination in combination)
            {
                if (keyID == combination.key)
                {
                    switch (combination.Keypress)
                    {
                        case mKeyPress.Down:
                            onInput?.Invoke(Input.GetKeyDown(combination.key));
                            return Input.GetKeyDown(combination.key);
                        case mKeyPress.Up:
                            onInput?.Invoke(Input.GetKeyUp(combination.key));
                            return Input.GetKeyUp(combination.key);
                        case mKeyPress.Hold:
                            onInput?.Invoke(Input.GetKey(combination.key));
                            return Input.GetKey(combination.key);
                    }
                }
            }
        }

        for (int i = 0; i < combination.Count; i++)
        {
            if (combination[i].type == mInputType.Mouse)
                return GetMouseInput(i);
        }

        return false;
    }

    public bool GetAxisAsButton(string axis) => GetInput<AxisAsButton>(axis).GetAxisAsButton(GetInput<AxisAsButton>(axis).pressType);
    public AxisAsButton FindAxisAsButton(string axis) => GetInput<AxisAsButton>(axis);

    public bool IdentifierNotNull(object ID, out string stringID, out KeyCode? keyID, out int? intID)
    {
        stringID = null;
        keyID = null;
        intID = null;

        if (ID is string)
            stringID = ID as string;

        if (ID is KeyCode)
            keyID = (KeyCode)ID;

        if (ID is int)
            intID = (int)ID;

        if (stringID == null || keyID == null || intID == null) return false;

        return true;
    }

    private bool RequireAllKeys(bool[] keys)
    {
        CacheInputs(keys);

        for (int i = 0; i < combination.Count; i++)
        {
            if (keys[i])
                continue;
            else
                return false;
        }

        return true;
    }
    private bool RequireAnyKey(bool[] keys)
    {
        CacheInputs(keys);

        for (int i = 0; i < combination.Count; i++)
        {
            if (keys[i] && combination[i].Required)
                return true;
            else
                continue;
        }

        return false;
    }

    private void CacheInputs(bool[] keys)
    {
        for (int i = 0; i < combination.Count; i++)
        {
            switch (combination[i].type)
            {
                case mInputType.Keyboard:

                    if (combination[i].key == KeyCode.None) continue;

                    switch (combination[i].Keypress)
                    {
                        case mKeyPress.Down:
                            keys[i] = Input.GetKeyDown(combination[i].key);
                            continue;

                        case mKeyPress.Up:
                            keys[i] = Input.GetKeyUp(combination[i].key);
                            continue;

                        case mKeyPress.Hold:
                            keys[i] = Input.GetKey(combination[i].key);
                            continue;
                    }
                    break;

                case mInputType.Mouse: keys[i] = GetMouseInput(i); break;

                case mInputType.AxisButton:

                    if (combination[i].axis.AxisName == "") continue;

                    switch (combination[i].Keypress)
                    {
                        case mKeyPress.Down:
                            keys[i] = combination[i].axis.GetAxisDown();
                            continue;

                        case mKeyPress.Up:
                            keys[i] = combination[i].axis.GetAxisUp();
                            continue;

                        case mKeyPress.Hold:
                            keys[i] = combination[i].axis.GetAxisHold();
                            continue;
                    }
                    break;

                default:
                    break;
            }

            if (combination[i].Required && !keys[i])
                keys[i] = false;
        }
    }
    private bool GetMouseInput(int i)
    {
        Ray ray;
        RaycastHit hit;
        RaycastHit2D hit2D;
        bool raycast;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        raycast = Physics.Raycast(ray, out hit, Mathf.Infinity);
        hit2D = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

        var eventData = new PointerEventData(null);
        var results = new List<RaycastResult>();

        if (EventSystem.current != null)
        {
            eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            EventSystem.current.RaycastAll(eventData, results);
        }

        switch (combination[i].mousePress)
        {
            case mMousePress.Click:

                if (hit.collider != null)
                {
                    if (hit.collider.transform == combination[i].target)
                        return raycast && Input.GetMouseButtonDown(0);
                }

                if (hit2D.collider != null)
                {
                    if (hit2D.transform == combination[i].target)
                        return Input.GetMouseButtonDown(0);
                }

                if (EventSystem.current != null)
                {
                    foreach (var item in results)
                    {
                        if (item.gameObject == combination[i].target.gameObject)
                            return Input.GetMouseButtonDown(0);
                    }
                }

                break;

            case mMousePress.Release:

                if (hit.collider != null)
                {
                    if (hit.collider.transform == combination[i].target)
                        return raycast && Input.GetMouseButtonUp(0);
                }

                if (hit2D.collider != null)
                {
                    if (hit2D.transform == combination[i].target)
                        return Input.GetMouseButtonUp(0);
                }

                if (EventSystem.current != null)
                {
                    foreach (var item in results)
                    {
                        if (item.gameObject == combination[i].target.gameObject)
                            return Input.GetMouseButtonUp(0);
                    }
                }

                break;

            case mMousePress.Hover:

                if (hit.collider != null)
                    return hit.collider.transform == combination[i].target;

                if (hit2D.collider != null)
                    return hit2D.transform == combination[i].target;

                if (EventSystem.current != null)
                {
                    foreach (var item in results)
                        return item.gameObject == combination[i].target.gameObject;
                }
                break;

            default: return false;
        }

        return false;
    }
}

#endregion

#region Editor

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(mInput))]
public class MyDataListDrawer : PropertyDrawer
{
    private Dictionary<string, ReorderableList> _reorderableLists = new Dictionary<string, ReorderableList>();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty myDataList;

        myDataList = property.FindPropertyRelative("combination");

        if (!_reorderableLists.ContainsKey(property.propertyPath) || _reorderableLists[property.propertyPath].index > _reorderableLists[property.propertyPath].count - 1)
            _reorderableLists[property.propertyPath] = new ReorderableList(myDataList.serializedObject, myDataList, true, true, true, true);

        return _reorderableLists[property.propertyPath].GetHeight();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position = EditorGUI.IndentedRect(position);

        //Draw header content
        _reorderableLists[property.propertyPath].drawHeaderCallback = (Rect rect) => 
        {
            EditorGUI.LabelField(rect, property.displayName, GetBoldLabel());

            EditorGUI.LabelField(new Rect(rect.x + rect.width - 88, rect.y-2, 80, 20), "Require All");
            property.FindPropertyRelative("RequireAll").boolValue = EditorGUI.Toggle(new Rect(rect.x + rect.width - 16, rect.y-1, 20, 20), property.FindPropertyRelative("RequireAll").boolValue);
        };

        //Set height of element
        _reorderableLists[property.propertyPath].elementHeightCallback = (index) => {

            SerializedProperty prop = _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index);

            switch (prop.FindPropertyRelative("type").enumValueIndex)
            {
                case 3: return 40;
                case 2: return 24;
                case 4: return 46;
                case 5: return 46;
            }

            return 20;
        };

        //Set element background
        _reorderableLists[property.propertyPath].drawElementBackgroundCallback = (rect, index, active, focused) =>
        {
            if (_reorderableLists[property.propertyPath].count == 0) return;

            SerializedProperty prop = _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index);

            Texture2D tex = new Texture2D(1, 1);

            if (focused)
            {
                float color = 0.3f;
                tex.SetPixel(0, 0, new Color(color, color, color, 1f));
                tex.Apply();
                GUI.DrawTexture(rect, tex);
            }
            else
            {
                float color = 0f;
                tex.SetPixel(0, 0, new Color(color, color, color, 0.1f));
                tex.Apply();
                switch (prop.FindPropertyRelative("type").enumValueIndex)
                {
                    case 2: GUI.DrawTexture(rect, tex); break;
                    case 4: GUI.DrawTexture(rect, tex); break;
                    case 5: GUI.DrawTexture(rect, tex); break;
                }
            }
        };

        //Draw element content
        _reorderableLists[property.propertyPath].drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty prop = _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index);

            int offset = (prop.FindPropertyRelative("type").enumValueIndex == 2 || prop.FindPropertyRelative("type").enumValueIndex == 4 || prop.FindPropertyRelative("type").enumValueIndex == 5) ? 4 : 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y + offset, 90, 18), prop.FindPropertyRelative("type"), GUIContent.none);

            switch (prop.FindPropertyRelative("type").enumValueIndex)
            {
                case 0: DrawKeyboardElements(property, rect, index); break;
                case 1: DrawMouseElements(property, rect, index); break;
                case 2: DrawAxisElements(property, rect, index); break;
                case 3: DrawAxisButtonElements(property, rect, index); break;
                case 4: DrawVector2Elements(property, rect, index); break;
                case 5: DrawVector3Elements(property, rect, index); break;
                default:
                    break;
            }
        };

        //Update the list and draw it in the editor
        _reorderableLists[property.propertyPath].DoList(position);
    }

    float subtraction = 20f;
    Color dividerColor = new Color(0.1f, 0.1f, 0.1f, 1f);

    private void DrawKeyboardElements(SerializedProperty property, Rect rect, int index)
    {
        SerializedProperty prop = _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(new Rect(rect.x + 94, rect.y + 2, 70, 18), prop.FindPropertyRelative("Keypress"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(rect.x + 168, rect.y + 2, rect.width - 168 - subtraction, 18), prop.FindPropertyRelative("key"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(rect.width + 24, rect.y + 2, 18, 18), prop.FindPropertyRelative("Required"), GUIContent.none);
    }

    private void DrawMouseElements(SerializedProperty property, Rect rect, int index)
    {
        SerializedProperty prop = _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(new Rect(rect.x + 94, rect.y + 2, 70, 18), prop.FindPropertyRelative("mousePress"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(rect.x + 168, rect.y + 2, rect.width - 168 - subtraction, 18), prop.FindPropertyRelative("target"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(rect.width + 24, rect.y + 2, 18, 18), prop.FindPropertyRelative("Required"), GUIContent.none);
    }

    private void DrawAxisElements(SerializedProperty property, Rect rect, int index)
    {
        if (DisplayLine(index, property, true))
            EditorGUI.DrawRect(new Rect(rect.x - 20, rect.y, rect.width + 27, 0.5f), dividerColor);
        
        SerializedProperty prop = _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(new Rect(rect.x + 94, rect.y + 4, rect.width - 94, 18), prop.FindPropertyRelative("mAxis").FindPropertyRelative("AxisName"), GUIContent.none);
        
        if (DisplayLine(index, property, false))
            EditorGUI.DrawRect(new Rect(rect.x - 20, rect.y + 25, rect.width + 27, 0.5f), dividerColor);
    }

    private void DrawAxisButtonElements(SerializedProperty property, Rect rect, int index)
    {
        SerializedProperty prop = _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.PropertyField(new Rect(rect.x + 94, rect.y + 2, rect.width - 112, 18), 
            prop.FindPropertyRelative("axis").FindPropertyRelative("pressType"), GUIContent.none);

        EditorGUI.PropertyField(new Rect(rect.x, rect.y + 22, rect.width - 120, 18), 
            prop.FindPropertyRelative("axis").FindPropertyRelative("AxisName"), GUIContent.none);

        EditorGUI.LabelField(new Rect(rect.width - 78, rect.y + 22, 124, 18), "Threshold");
        EditorGUI.PropertyField(new Rect(rect.width - 13, rect.y + 22, 51, 18), 
            prop.FindPropertyRelative("axis").FindPropertyRelative("TriggerThreshold"), GUIContent.none);

        EditorGUI.PropertyField(new Rect(rect.width + 24, rect.y + 2, 18, 18), 
            prop.FindPropertyRelative("Required"), GUIContent.none);
    }

    private void DrawVector2Elements(SerializedProperty property, Rect rect, int index)
    {
        if (DisplayLine(index, property, true))
            EditorGUI.DrawRect(new Rect(rect.x - 20, rect.y, rect.width + 27, 0.5f), dividerColor);

        SerializedProperty prop = _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(new Rect(rect.x + 94, rect.y + 4, rect.width - 94 - 92, 18), prop.FindPropertyRelative("vector2").FindPropertyRelative("Vector2Name"), GUIContent.none);

        EditorGUI.LabelField(new Rect(rect.width - 48, rect.y + 4, 124, 18), "Normalized");
        EditorGUI.PropertyField(new Rect(rect.width + 24, rect.y + 4, 124, 18), prop.FindPropertyRelative("vector2").FindPropertyRelative("normalized"), GUIContent.none);

        EditorGUI.LabelField(new Rect(rect.x + 2, rect.y + 26, 42, 18), "X Axis");
        EditorGUI.PropertyField(new Rect(rect.x + 44, rect.y + 26, rect.width/2 - 44, 18), prop.FindPropertyRelative("vector2").FindPropertyRelative("X"), GUIContent.none);

        EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 2, rect.y + 26, 42, 18), "Y Axis");
        EditorGUI.PropertyField(new Rect(rect.x + rect.width / 2 + 44, rect.y + 26, rect.width/2 -44, 18), prop.FindPropertyRelative("vector2").FindPropertyRelative("Y"), GUIContent.none);

        if (DisplayLine(index, property, false))
            EditorGUI.DrawRect(new Rect(rect.x - 20, rect.y + 47, rect.width + 27, 0.5f), dividerColor);
    }

    private void DrawVector3Elements(SerializedProperty property, Rect rect, int index)
    {
        if (DisplayLine(index, property, true))
            EditorGUI.DrawRect(new Rect(rect.x - 20, rect.y, rect.width + 27, 0.5f), dividerColor);

        SerializedProperty prop = _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(new Rect(rect.x + 94, rect.y + 4, rect.width - 94 - 92, 18), prop.FindPropertyRelative("vector3").FindPropertyRelative("Vector3Name"), GUIContent.none);

        EditorGUI.LabelField(new Rect(rect.width - 48, rect.y + 4, 124, 18), "Normalized");
        EditorGUI.PropertyField(new Rect(rect.width + 24, rect.y + 4, 124, 18), prop.FindPropertyRelative("vector3").FindPropertyRelative("normalized"), GUIContent.none);

        float width = rect.width;

        EditorGUI.LabelField(new Rect(rect.x + 2, rect.y + 26, 42, 18), "X Axis");
        EditorGUI.PropertyField(new Rect(rect.x + 44, rect.y + 26, width / 3 - 44, 18), prop.FindPropertyRelative("vector3").FindPropertyRelative("X"), GUIContent.none);

        EditorGUI.LabelField(new Rect(rect.x + width / 3 + 2, rect.y + 26, 42, 18), "Y Axis");
        EditorGUI.PropertyField(new Rect(rect.x + width / 3 + 44, rect.y + 26, width / 3 - 44, 18), prop.FindPropertyRelative("vector3").FindPropertyRelative("Y"), GUIContent.none);

        EditorGUI.LabelField(new Rect(rect.x + (width / 3) + (width / 3) + 2, rect.y + 26, 42, 18), "Z Axis");
        EditorGUI.PropertyField(new Rect(rect.x + ((width / 3) + (width / 3)) + 44, rect.y + 26, (width / 3) - 44, 18), prop.FindPropertyRelative("vector3").FindPropertyRelative("Z"), GUIContent.none);

        if (DisplayLine(index, property, false))
            EditorGUI.DrawRect(new Rect(rect.x - 20, rect.y + 47, rect.width + 27, 0.5f), dividerColor);
    }

    public GUIStyle GetBoldLabel()
    {
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = new Color(1, 1, 1, 0.7f);
        style.contentOffset = Vector2.up;
        return style;
    }

    private bool DisplayLine(int index, SerializedProperty property, bool direction)
    {
        SerializedProperty up;
        SerializedProperty down;
        bool showUp = false;
        bool showDown = false;

        if (index + 1 < _reorderableLists[property.propertyPath].count)
        {
            up = _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index + 1).FindPropertyRelative("type");
            showUp = up.enumValueIndex == 2 || up.enumValueIndex == 4 || up.enumValueIndex == 5;
        }
        if (index - 1 >= 0)
        {
            down = _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index - 1).FindPropertyRelative("type");
            showDown = down.enumValueIndex == 2 || down.enumValueIndex == 4 || down.enumValueIndex == 5;
        }

        if (direction)
            return showDown == false && showUp == false || showDown == false;
        else
            return showDown == false && showUp == false || showUp == false;
    }
}
#endif

#endregion

#region Structs
[Serializable] public enum mKeyPress { Down, Up, Hold }
[Serializable] public enum mMousePress { Click, Release, Hover }
[Serializable] public enum mInputType { Keyboard, Mouse, Axis, AxisButton, Vector2, Vector3 }

[Serializable]
public class Combination
{
    public mInputType type;
    public bool Required;

    public mKeyPress Keypress;
    public KeyCode key;

    public mMousePress mousePress;
    public Transform target;

    public mAxis mAxis;
    public AxisAsButton axis;

    public mVector2 vector2;
    public mVector3 vector3;
}

[Serializable] 
public class mAxis
{
    public string AxisName;
    public float AxisValue;

    public mAxis(string name, float value)
    {
        AxisName = name;
        AxisValue = value;
    }

    public float SetValue(float i) { AxisValue = i; return AxisValue; }
    public string GetName() => AxisName;
    public float GetValue() => AxisValue;
}

[Serializable]
public class mVector2
{
    public string Vector2Name;
    public string X;
    public string Y;
    public bool normalized;

    public mVector2(string name, string x, string y)
    {
        Vector2Name = name;
        X = x;
        Y = y;
    }
    public mVector2(string name, mAxis x, mAxis y)
    {
        Vector2Name = name;
        X = x.AxisName;
        Y = y.AxisName;
    }

    public (string, string) SetValue(string x, string y) 
    { 
        X = x;
        Y = y;

        return (X, Y);
    }
    public string GetName() => Vector2Name;
    public Vector2 GetValue() => normalized ? new Vector2(Input.GetAxisRaw(X), Input.GetAxisRaw(Y)).normalized : new Vector2(Input.GetAxisRaw(X), Input.GetAxisRaw(Y));
}

[Serializable]
public class mVector3
{
    public string Vector3Name;
    public string X;
    public string Y;
    public string Z;

    public bool normalized;

    public mVector3(string name, string x, string y, string z)
    {
        Vector3Name = name;
        X = x;
        Y = y;
        Z = z;
    }
    public mVector3(string name, mAxis x, mAxis y, mAxis z)
    {
        Vector3Name = name;
        X = x.AxisName;
        Y = y.AxisName;
        Z = z.AxisName;
    }

    public (string, string, string) SetValue(string x, string y, string z)
    {
        X = x;
        Y = y;
        Z = z;

        return (X, Y, Z);
    }
    public string GetName() => Vector3Name;
    public Vector3 GetValue() => normalized ? new Vector3(Input.GetAxisRaw(X), Input.GetAxisRaw(Y), Input.GetAxisRaw(Z)).normalized : new Vector3(Input.GetAxisRaw(X), Input.GetAxisRaw(Y), Input.GetAxisRaw(Z));
}

#endregion
