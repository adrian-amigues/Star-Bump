using System;
using UnityEditor;
using UnityEngine;

public class CustomGameObjectCreation {
    [MenuItem("GameObject/Custom/Create Empty Child %#n")] // Ctrl+Shift+N
    private static void CreateEmptyChild() {
        var newObj = new GameObject("GameObject");
        if (Selection.activeTransform != null) {
            newObj.transform.SetParent(Selection.activeTransform);
            newObj.transform.localPosition = Vector3.zero;
        }

        Undo.RegisterCreatedObjectUndo(newObj, "Create Empty Child");
        Selection.activeGameObject = newObj;

        // Delay the rename so it happens after selection is updated
        EditorApplication.delayCall += () => {
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            EditorApplication.ExecuteMenuItem("Edit/Rename");
        };
    }
}