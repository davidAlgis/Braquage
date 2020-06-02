using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditModeFunctions : EditorWindow
{
    [MenuItem("Window/Edit Mode Functions")]
    public static void ShowWindow()
    {
        GetWindow<EditModeFunctions>("Generate Gizmos Door");
    }
    
    private void OnGUI()
    {
        if (GUILayout.Button("Default Function"))
        {
            DefaultFunction();
        }

        if (GUILayout.Button("Generate Front And Back Door"))
        {
            GenerateFrontAndBackDoor();
        }
    }

    private void DefaultFunction()
    {


    }

    private void GenerateFrontAndBackDoor()
    {
        GameObject[] doorsGO = GameObject.FindGameObjectsWithTag("Door");
        foreach(GameObject doorGo in doorsGO)
        {
            if (doorGo.TryGetComponent(out Door door))
            {
                if(door.BackGO == null)
                {
                    Debug.Log("Generate back " + door.gameObject.name);
                    door.BackGO = new GameObject();
                    door.BackGO.name = "back_" + door.gameObject.name;
                    door.BackGO.transform.parent = doorGo.transform;
                    door.BackGO.transform.localPosition = new Vector3(0.0f, -1.0f, 0.0f);

                }

                if(door.FrontGO == null)
                {
                    Debug.Log("Generate front " + door.gameObject.name);
                    door.FrontGO = new GameObject();
                    door.FrontGO.name = "front_" + door.gameObject.name;
                    door.FrontGO.transform.parent = doorGo.transform;
                    door.FrontGO.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
                }
                
            }
            else
                Debug.LogWarning("unable to find any door componnement");
        }
    }
}
