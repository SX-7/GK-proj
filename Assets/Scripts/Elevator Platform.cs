using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class ElevatorPlatform : MonoBehaviour
{
    //TODO: currently fails when opening gets interrupted. Also a few assumptions
    [SerializeField] public Transform expectedPlayerPosition;
    [SerializeField] public bool exitElevator = false;
    [SerializeField] float doorOpeningTime = 1;
    [SerializeField] GameObject door;
    public delegate void Finished();
    public static event Finished OnFinish;
    private Vector3 initDoorPos;
    private Vector3 segmentExitPosition;
    private bool closed = false;
    public Vector3 SegmentExitPosition { get { return segmentExitPosition; } set { segmentExitPosition = SegmentExitPosition; } }
    private void Start()
    {
        initDoorPos = door.transform.position;
        OpenElevator();
    }
    private void OnTriggerEnter(Collider other)
    {
        
        var player = other.GetComponent<PlayerController>();
        if (player != null & exitElevator &!closed)
        {
            CloseElevator();
            player.SendMessage("FadeOut");
            player.SendMessage("LockMovement",doorOpeningTime);
            StopCoroutine(FinishedCR());
            StartCoroutine(FinishedCR());
        }
    }

    IEnumerator FinishedCR()
    {
        var timer = 0f;
        while (timer < doorOpeningTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        OnFinish?.Invoke();
    }

    //Send message target
    private void OpenElevator()
    {
        StopAllCoroutines();
        StartCoroutine(OpenElevatorCR());
    }

    IEnumerator OpenElevatorCR()
    {
        float timer = 0f;
        while (timer < doorOpeningTime)
        {
            timer += Time.deltaTime;
            door.transform.localPosition += new Vector3(0, (5 * Time.deltaTime) / doorOpeningTime, 0);
            yield return null;
        }
    }

    private void CloseElevator()
    {
        StopAllCoroutines();
        StartCoroutine(CloseElevatorCR());
    }

    IEnumerator CloseElevatorCR()
    {
        float timer = 0f;
        while (timer < doorOpeningTime)
        {
            timer += Time.deltaTime;
            door.transform.localPosition -= new Vector3(0, (5 * Time.deltaTime) / doorOpeningTime, 0);
            yield return null;
        }
        door.transform.position = initDoorPos;
        closed=true;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ElevatorPlatform))]
public class DrawElevatorSegmentEnd : Editor
{
    // draws info in editor and allows for interaction (rn weirds out when rotating, otherwise works)
    void OnSceneGUI()
    {
        ElevatorPlatform myObj = (ElevatorPlatform)target;

        Handles.color = Color.red;
        Handles.DrawWireCube(myObj.transform.TransformPoint(myObj.SegmentExitPosition), new Vector3(10, 10, 0));
        EditorGUI.BeginChangeCheck();
        Vector3 new_exit_pos = Handles.PositionHandle(myObj.transform.TransformPoint(myObj.SegmentExitPosition), Quaternion.LookRotation(myObj.transform.forward, Vector3.up)) - myObj.transform.position;
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myObj, "Change Elevator Segment End Position");
            myObj.SegmentExitPosition = new_exit_pos;
        }

    }
}
#endif
