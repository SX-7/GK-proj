using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] ElevatorPlatform platform;
    [SerializeField] List<LevelSegment> segments = new();
    [SerializeField] int segmentCount = 10;
    private List<LevelSegment> instantiatedSegments = new();
    private PlayerController instantiatedPlayer;
    private ElevatorPlatform startPlatform;
    private ElevatorPlatform endPlatform;

    // Start is called before the first frame update
    void Awake()
    {
        startPlatform = Instantiate(platform);
        var next_position = platform.transform.TransformPoint(platform.SegmentExitPosition);
        for (int i = 0; i < segmentCount; i++)
        {
            var segment_to_add = segments[Random.Range(0, segments.Count)];
            instantiatedSegments.Add(Instantiate(segment_to_add, next_position - segment_to_add.segmentEntryPosition, segment_to_add.transform.rotation));
            next_position = next_position - segment_to_add.segmentEntryPosition + segment_to_add.segmentExitPosition;
        }
        endPlatform= Instantiate(platform,next_position-platform.SegmentExitPosition,Quaternion.LookRotation(Vector3.back));
        endPlatform.exitElevator = true;
        instantiatedPlayer= Instantiate(player, startPlatform.expectedPlayerPosition.position, Quaternion.LookRotation(Vector3.forward));
    }

    private void OnEnable()
    {
        ElevatorPlatform.OnFinish += Finished;
    }

    private void OnDisable()
    {
        ElevatorPlatform.OnFinish -= Finished;
    }

    void Finished()
    {
        //Delete
        foreach(var segment in instantiatedSegments)
        {
            Destroy(segment.gameObject);
        }
        instantiatedSegments.Clear();
        Destroy(startPlatform.gameObject);
        Destroy(endPlatform.gameObject);
        Debug.Log("Deleted");
        //Reinstantate level
        startPlatform = Instantiate(platform);
        var next_position = platform.transform.TransformPoint(platform.SegmentExitPosition);
        for (int i = 0; i < segmentCount; i++)
        {
            var segment_to_add = segments[Random.Range(0, segments.Count)];
            instantiatedSegments.Add(Instantiate(segment_to_add, next_position - segment_to_add.segmentEntryPosition, segment_to_add.transform.rotation));
            next_position = next_position - segment_to_add.segmentEntryPosition + segment_to_add.segmentExitPosition;
        }
        endPlatform = Instantiate(platform, next_position - platform.SegmentExitPosition, Quaternion.LookRotation(Vector3.back));
        endPlatform.exitElevator = true;
        instantiatedPlayer.SendMessage("FadeIn");
        instantiatedPlayer.transform.position = startPlatform.expectedPlayerPosition.position;
        Debug.Log("Reinstated");
    }

}
