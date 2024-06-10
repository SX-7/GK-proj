using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //stores info about "grabbable ledges"
    public Vector3 respawnPosition;
    [SerializeField] bool heals = true;
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            other.gameObject.BroadcastMessage("SetRespawn", transform.TransformPoint(respawnPosition));
            if (heals)
            {
                other.gameObject.BroadcastMessage("Heal", -1);
            }
        }

    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Checkpoint))]
public class DrawSelectorGizmo : Editor
{
    // draws info in editor and allows for interaction (rn weirds out when rotating, otherwise works)
    void OnSceneGUI()
    {
        Checkpoint myObj = (Checkpoint)target;
        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.PositionHandle(myObj.transform.TransformPoint(myObj.respawnPosition), Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myObj, "Change Respawn Position");
            myObj.respawnPosition = myObj.transform.InverseTransformPoint(newTargetPosition);
        }

    }
}
#endif