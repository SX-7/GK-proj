using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class LevelSegment : MonoBehaviour
{
    //stores info about segment entry/exit
    public Vector3 segmentEntryPosition;
    public Vector3 segmentExitPosition;

    void OnDrawGizmosSelected()
    {
        //if (bc == null) { bc = GetComponent<BoxCollider>(); }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelSegment))]
public class DrawSegmentEnds : Editor
{
    // draws info in editor and allows for interaction (rn weirds out when rotating, otherwise works)
    void OnSceneGUI()
    {
        LevelSegment myObj = (LevelSegment)target;

        Handles.color = Color.blue;
        Handles.DrawWireCube(myObj.transform.TransformPoint(myObj.segmentEntryPosition), new Vector3(10, 10, 0));
        EditorGUI.BeginChangeCheck();
        Vector3 new_entry_pos = Handles.PositionHandle(myObj.transform.TransformPoint(myObj.segmentEntryPosition), Quaternion.LookRotation(myObj.transform.forward, Vector3.up)) - myObj.transform.position;
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myObj, "Change Segment Entry Position");
            myObj.segmentEntryPosition = new_entry_pos;
        }

        Handles.color = Color.red;
        Handles.DrawWireCube(myObj.transform.TransformPoint(myObj.segmentExitPosition), new Vector3(10, 10, 0));
        EditorGUI.BeginChangeCheck();
        Vector3 new_exit_pos = Handles.PositionHandle(myObj.transform.TransformPoint(myObj.segmentExitPosition), Quaternion.LookRotation(myObj.transform.forward, Vector3.up))-myObj.transform.position;
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myObj, "Change Segment Exit Position");
            myObj.segmentExitPosition = new_exit_pos;
        }

    }
}
#endif