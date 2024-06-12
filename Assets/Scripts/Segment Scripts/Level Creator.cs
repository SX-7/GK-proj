using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelCreator : MonoBehaviour
{
    [Header("Externals")]
    [SerializeField] PlayerController player;
    [SerializeField] ElevatorPlatform platform;
    [SerializeField] List<LevelSegment> segmentsToChooseFrom = new();
    [SerializeField] List<BossSegment> bossSegmentsToChooseFrom = new();
    [SerializeField] Skull skull;
    private List<LevelSegment> Segments { get => segmentsToChooseFrom; }
    private List<BossSegment> BossSegments { get => bossSegmentsToChooseFrom; }
    [Header("Generation type")]
    [SerializeField] bool manual = false;
    [Header("Use these for suto generation")]
    [SerializeField] int levelCount = 3;
    [SerializeField] int segmentCountPerLevel = 10;
    private int SegmentCount { get => segmentCountPerLevel; }
    [Header("Use these for manual generation")]
    [Tooltip("If the sum of these is smaller than length of Segment Indexes, throw error")]
    [SerializeField] List<int> segmentsPerLevel = new();
    [SerializeField] List<int> segmentIndexes = new List<int>();
    private List<LevelSegment> instantiatedSegments = new();

    private PlayerController instantiatedPlayer;
    private ElevatorPlatform startPlatform;
    private ElevatorPlatform endPlatform;
    private Skull instantiatedSkull;
    private int currentLevel = 0;
    private Vector3 baseOffset;


    void Awake()
    {
        ElevatorPlatform.OnFinish += Finished;
        ElevatorPlatform.OnFinishEnter += EarlyFinish;
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
        if(instantiatedSkull== null)
        {
            instantiatedSkull = Instantiate(skull, instantiatedPlayer.transform.position + instantiatedPlayer.SkullRespawnOffset, Quaternion.LookRotation(Vector3.forward));
        }
        //weird but needed idk
        ElevatorPlatform.OnFinish -= Finished;
        ElevatorPlatform.OnFinish += Finished;

        ElevatorPlatform.OnFinishEnter -= EarlyFinish;
        ElevatorPlatform.OnFinishEnter += EarlyFinish;
    }

    private void OnDisable()
    {
        ElevatorPlatform.OnFinish -= Finished;
        ElevatorPlatform.OnFinishEnter -= EarlyFinish;
    }

    void EarlyFinish()
    {
        instantiatedSkull.SendMessage("Stop");
        instantiatedPlayer.SendMessage("SnapSetRespawn");
    }
    void Finished()
    {
        currentLevel++;
        int adjusted_count=levelCount;
        if (manual)
        {
            adjusted_count = segmentsPerLevel.Count;
        }
        if (currentLevel < adjusted_count)
        {
            Delete();
            Rebuild();
            instantiatedPlayer.SendMessage("FadeIn");
            instantiatedSkull.SendMessage("PlaceAgain");
        }
        else
        {
            Delete();

            if (baseOffset == null)
            {
                baseOffset = transform.position;
            }

            startPlatform = Instantiate(platform, baseOffset - platform.expectedPlayerPosition.position, Quaternion.identity);
            var next_position = startPlatform.transform.TransformPoint(startPlatform.SegmentExitPosition);
            var segment_to_add = BossSegments[Random.Range(0, BossSegments.Count)];    
            var boss_segment = Instantiate(segment_to_add, next_position - segment_to_add.segmentEntryPosition, segment_to_add.transform.rotation);
            instantiatedPlayer.SendMessage("FadeIn");
            if (boss_segment.disableSkullSpawn)
            {
                Destroy(instantiatedSkull.gameObject);
            }
            else
            {
                instantiatedSkull.SendMessage("PlaceAgain");
            }
            
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

        var end_i = 0;
        if (manual)
        {
            end_i = segmentsPerLevel[currentLevel];
        }
        else
        {
            end_i = SegmentCount;
        }

        for (int i = 0; i < end_i; i++)
        {
            LevelSegment segment_to_add;
            if (manual)
            {
                segment_to_add = Segments[segmentIndexes[i + segmentsPerLevel.Take(currentLevel).Sum()]];
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
