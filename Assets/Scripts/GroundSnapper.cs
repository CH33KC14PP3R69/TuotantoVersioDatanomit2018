#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
public class GroundSnapper : EditorWindow
{
    [MenuItem("Window/AUD/Snapper/Snapper")]                                                // add menu item
    static void OpenWindow()
    {
        EditorWindow.GetWindow<GroundSnapper>().Show();                         // Get existing open window or if none, make a new one
    }

    void OnGUI()
    {
        if (GUILayout.Button("SnapToGround", GUILayout.Width(50), GUILayout.Height(50)))
        {
            DropObjects();
        }

        if (GUILayout.Button("Rotate", GUILayout.Width(50), GUILayout.Height(50)))
        {
            RotateSelectedObjects();
        }
    }

    [MenuItem("Window/AUD/Snapper/RotateY #r")]
    static void RotateSelectedObjects()
    {      
        foreach (Transform obj in Selection.transforms)
        {
            Vector3 rot = obj.rotation.eulerAngles;
            obj.rotation = Quaternion.Euler(rot.x, Random.Range(0, 360), rot.z);
        }
    }

    [MenuItem("Window/AUD/Snapper/QuickSnap #s")]
    static void DropObjects()
    {
        Undo.RecordObjects(Selection.transforms, "Drop Objects");

        // Invoke the DropObjects twice to get the best result for the snap (Otherwise you might need to snap the object twice if the selected objects rotation is odd)
        for (int r = 0; r < 2; r++)
        {
            for (int i = 0; i < Selection.transforms.Length; i++)                       // drop multi-selected objects using the selected method
            {
                GameObject go = Selection.transforms[i].gameObject;                     // get the gameobject
                if (!go) { continue; }                                                  // if there's no gameobject, skip the step — probably unnecessary but hey…

                Bounds bounds = go.GetComponent<Renderer>().bounds;                     // get the renderer's bounds
                int savedLayer = go.layer;                                              // save the gameobject's initial layer
                go.layer = 2;                                                           // set the gameobject's layer to ignore raycast

                RaycastHit hit; 

                if (Physics.Raycast(go.transform.position, -Vector3.up, out hit))       // check if raycast hits something
                {
                    float yOffset = go.transform.position.y - bounds.min.y;
                    go.transform.up = hit.normal;
                    go.transform.position = new Vector3(hit.point.x, hit.point.y + yOffset, hit.point.z);
                }
                go.layer = savedLayer;                                                  // restore the gameobject's layer to it's initial layer
            }
        }
    }
}
#endif
