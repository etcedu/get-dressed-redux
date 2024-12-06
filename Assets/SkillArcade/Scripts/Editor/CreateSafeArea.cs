using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateSafeArea : MonoBehaviour
{
    [MenuItem("GameObject/UI/SafeArea", false, 10)]
    private static void NewSafeAreaObject(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("SafeArea");
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        RectTransform rectTrans = go.AddComponent<RectTransform>();
        rectTrans.anchorMin = new Vector2(0, 0);
        rectTrans.anchorMax = new Vector2(1, 1);
        rectTrans.offsetMin = new Vector2(0, 0);
        rectTrans.offsetMax = new Vector2(0, 0);
        rectTrans.pivot = new Vector2(0.5f, 0.5f);

        Undo.RegisterCompleteObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;


    }
}
