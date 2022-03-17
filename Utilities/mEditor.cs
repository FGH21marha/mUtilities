// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public static class mEditor
{
    public static int ModalWindow(string title, string content, string option1, Action onOption1)
    {
        bool state = EditorUtility.DisplayDialog(
                title,
                content,
                option1,
                null);

        if(state)
            onOption1?.Invoke();

        return 0;
    }
    public static int ModalWindow(string title, string content, string option1, string option2, Action onOption1, Action onOption2)
    {
        int option = EditorUtility.DisplayDialogComplex(
                title,
                content,
                option1,
                option2,
                null);

        switch (option)
        {
            case 0: onOption1?.Invoke(); return 0;
            case 1: onOption2?.Invoke(); return 1;
        }

        return 0;
    }
    public static int ModalWindow(string title, string content, string option1, string option2, string option3, Action onOption1, Action onOption2, Action onOption3)
    {
        int option = EditorUtility.DisplayDialogComplex(
                title,
                content,
                option1,
                option2,
                option3);

        switch (option)
        {
            case 0: onOption1?.Invoke(); return 0;
            case 1: onOption2?.Invoke(); return 1;
            case 2: onOption3?.Invoke(); return 2;
        }

        return 0;
    }
    public static int ModalWindow(string title, string content, string option1)
    {
        EditorUtility.DisplayDialog(
                title,
                content,
                option1,
                null);

        return 0;
    }
    public static int ModalWindow(string title, string content, string option1, string option2)
    {
        return EditorUtility.DisplayDialogComplex(
                title,
                content,
                option1,
                option2,
                null);
    }
    public static int ModalWindow(string title, string content, string option1, string option2, string option3)
    {
        return EditorUtility.DisplayDialogComplex(
                title,
                content,
                option1,
                option2,
                option3);
    }
}
