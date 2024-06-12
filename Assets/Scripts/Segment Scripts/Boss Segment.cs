using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class BossSegment : MonoBehaviour
{
    public bool disableSkullSpawn = false;
    public Vector3 segmentEntryPosition;
}

#if UNITY_EDITOR
[CustomEditor(typeof(BossSegment))]
public class DrawBossSegmentEnd : Editor
{
    // draws info in editor and allows for interaction (rn weirds out when rotating, otherwise works)
    void OnSceneGUI()
    {
        BossSegment myObj = (BossSegment)target;

        Handles.color = Color.blue;
        Handles.DrawWireCube(myObj.transform.TransformPoint(myObj.segmentEntryPosition), new Vector3(10, 10, 0));
        EditorGUI.BeginChangeCheck();
        Vector3 new_enter_pos = Handles.PositionHandle(myObj.transform.TransformPoint(myObj.segmentEntryPosition), Quaternion.LookRotation(myObj.transform.forward, Vector3.up)) - myObj.transform.position;
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myObj, "Change Boss Segment Entry Position");
            myObj.segmentEntryPosition = new_enter_pos;
        }

    }
}
#endif
