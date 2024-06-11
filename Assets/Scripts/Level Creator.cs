using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelCreator : MonoBehaviour
{
    [Header("Externals")]
    [SerializeField] string generatableSceneName = "Generatable Level";
    [SerializeField] PlayerController player;
    [SerializeField] ElevatorPlatform platform;
    [SerializeField] List<LevelSegment> segmentsToChooseFrom = new();
    private List<LevelSegment> Segments { get => segmentsToChooseFrom; }
    [SerializeField] int levelCount = 3;
    [SerializeField] int segmentCountPerLevel = 10;
    private int SegmentCount { get => segmentCountPerLevel; }
    [Header("If you put anything here, it'll use these segment indexes, in order, across stages, to create levels.")]
    [SerializeField] List<int> segmentIndexes = new List<int>();
    private List<LevelSegment> instantiatedSegments = new();
    private PlayerController instantiatedPlayer;
    private ElevatorPlatform startPlatform;
    private ElevatorPlatform endPlatform;
    private int currentLevel = 0;
    private Vector3 baseOffset;


    void Awake()
    {
        ElevatorPlatform.OnFinish += Finished;
    }


    private void OnEnable()
    {

        //very annoying unity thing
        PlayerController.Score = 0;
        currentLevel = 0;
        baseOffset = transform.position;
        Rebuild();
        if (instantiatedPlayer == null)
        {
            instantiatedPlayer = Instantiate(player, startPlatform.expectedPlayerPosition.position, Quaternion.LookRotation(Vector3.forward));
        }
        //weird but needed idk
        ElevatorPlatform.OnFinish -= Finished;
        ElevatorPlatform.OnFinish += Finished;
    }

    private void OnDisable()
    {
        ElevatorPlatform.OnFinish -= Finished;
    }

    void Finished()
    {
        currentLevel++;
        Debug.Log(currentLevel);
        if (currentLevel < levelCount)
        {
            Delete();
            Rebuild();
            instantiatedPlayer.SendMessage("FadeIn");
        }
        else
        {
            Delete();
        }

    }
    void Delete()
    {
        foreach (var segment in instantiatedSegments)
        {
            Destroy(segment.gameObject);
        }
        instantiatedSegments.Clear();
        Destroy(startPlatform.gameObject);
        Destroy(endPlatform.gameObject);
    }

    void Rebuild()
    {
        if (baseOffset == null)
        {
            baseOffset = transform.position;
        }
        startPlatform = Instantiate(platform, baseOffset-platform.expectedPlayerPosition.position, Quaternion.identity);
        var next_position = startPlatform.transform.TransformPoint(startPlatform.SegmentExitPosition);
        for (int i = 0; i < SegmentCount; i++)
        {
            LevelSegment segment_to_add;
            if (segmentIndexes.Count > 0)
            {
                segment_to_add = Segments[segmentIndexes[i + currentLevel * SegmentCount]];
            }
            else
            {
                segment_to_add = Segments[Random.Range(0, Segments.Count)];
            }

            instantiatedSegments.Add(Instantiate(segment_to_add, next_position - segment_to_add.segmentEntryPosition, segment_to_add.transform.rotation));
            next_position = next_position - segment_to_add.segmentEntryPosition + segment_to_add.segmentExitPosition;
        }
        endPlatform = Instantiate(platform, next_position - platform.SegmentExitPosition, Quaternion.LookRotation(Vector3.back));
        endPlatform.exitElevator = true;
        baseOffset = endPlatform.expectedPlayerPosition.position;
    }
}
