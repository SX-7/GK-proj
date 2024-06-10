using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] ElevatorPlatform platform;
    [SerializeField] List<LevelSegment> segments = new();
    [SerializeField] int segmentCount = 10;

    // Start is called before the first frame update
    void Awake()
    {
        var start_platform = Instantiate(platform);
        var next_position = platform.transform.TransformPoint(platform.SegmentExitPosition);
        for (int i = 0; i < segmentCount; i++)
        {
            var segment_to_add = segments[Random.Range(0, segments.Count)];
            Instantiate(segment_to_add, next_position - segment_to_add.segmentEntryPosition, segment_to_add.transform.rotation);
            next_position = next_position - segment_to_add.segmentEntryPosition + segment_to_add.segmentExitPosition;
        }
        var end_platform = Instantiate(platform,next_position-platform.SegmentExitPosition,Quaternion.LookRotation(Vector3.back));
        end_platform.exitElevator = true;
        Instantiate(player, start_platform.expectedPlayerPosition.position, Quaternion.LookRotation(Vector3.forward));
    }
}
