using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}
    public GameObject missionInformation;
    public Text missionTextLabel;
    public Image[] missionImages;
    public Transform[] missionStartPoints;
    public GameObject[] playerCars;
    public GameObject[] missionAreas;
    public ParkingTrigger[] parkingTriggers;
    public int currentMission=0;
    public bool[] missionCompleted;
    

    public void Awake()
    {
        missionCompleted = new bool[missionStartPoints.Length];
        if (Instance == null)
        {
            Instance=this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }    

    void Start()
{
    

    LoadProgress();

    ShowMissionTextForCurrentMission();
    SetActiveMissionArea();
    SpawnPlayerAtMissionStart();
}


    public void ReplayMission(int missionIndex)
    {
        if(missionIndex <0 || missionIndex >= missionStartPoints.Length) return;
        currentMission=missionIndex;
        
        UpdateMissionText();
        SpawnPlayerAtMissionStart();  
        
    }

    private void UpdateMissionText()
{
    // LEVEL 4 = Parallel Parking
    if (currentMission == 3)     
    {
        missionTextLabel.text = "Park Car Parallel";
    }
    // REVERSE LEVELS
    else if (currentMission < parkingTriggers.Length && parkingTriggers[currentMission].isReverseMission)
    {
        missionTextLabel.text = "Park Car in Reverse";
    }
    // NORMAL STRAIGHT LEVELS
    else
    {
        missionTextLabel.text = "Park Car Straight";
    }

    // Activate correct mission sprite
    for (int i = 0; i < missionImages.Length; i++)
    {
        missionImages[i].gameObject.SetActive(i == currentMission);
    }
}


    private IEnumerator DisplayMissionText()
    {
        missionInformation.SetActive(true);
        yield return new WaitForSeconds(2f);
        missionInformation.SetActive(false);
    }

    public void ShowMissionTextForCurrentMission()
    {
        UpdateMissionText();
        StartCoroutine(DisplayMissionText());
    }

    public void SetActiveMissionArea()
    {
        for(int i = 0; i < missionAreas.Length; i++)
        {
            missionAreas[i].SetActive(i==currentMission);
        }
    }

    public void SpawnPlayerAtMissionStart()
    {
        if(currentMission < missionStartPoints.Length && playerCars.Length > 0)
        {
            foreach(var car in playerCars)
            {
                if(car != null)
                {
                    GlobalAudio.Instance.PlaySFX(GlobalAudio.Instance.ignition);

                    car.transform.position=missionStartPoints[currentMission].position;
                    car.transform.rotation=missionStartPoints[currentMission].rotation;
                }
            }
        }
    }

    public void CompleteMission()
    {
        missionCompleted[currentMission]=true;
        currentMission++;
        if (currentMission >= missionStartPoints.Length)
        {
            missionTextLabel.text="All Missions Completed";
        }
        else
        {
            missionCompleted[currentMission]=true;
            UpdateMissionText();        
        }
        SaveProgress();
        FindFirstObjectByType<MainMenuManager>().UpdateMissionButtons();

    }

    public void SaveProgress()
{
    
    PlayerPrefs.SetInt("CurrentMission", currentMission);

    // Save mission completion status
    for (int i = 0; i < missionCompleted.Length; i++)
    {
        PlayerPrefs.SetInt($"Mission{i}Completed", missionCompleted[i] ? 1 : 0);
    }

    // Save player car position (only first car)
    if (playerCars.Length > 0 && playerCars[0] != null)
    {
        PlayerPrefs.SetFloat("PlayerPositionX", playerCars[0].transform.position.x);
        PlayerPrefs.SetFloat("PlayerPositionY", playerCars[0].transform.position.y);
        PlayerPrefs.SetFloat("PlayerPositionZ", playerCars[0].transform.position.z);
    }

    PlayerPrefs.Save();
}

public void LoadProgress()
{
    // Check if saved data exists
    if (PlayerPrefs.HasKey("CurrentMission"))
    {
        // Load saved mission index
        currentMission = PlayerPrefs.GetInt("CurrentMission");

        
        for (int i = 0; i < missionCompleted.Length; i++)
        {
            missionCompleted[i] =
                PlayerPrefs.GetInt($"Mission{i}Completed", 0) == 1;
        }
    }
    else
    {
        
        currentMission = 0;

        // First mission unlocked
        missionCompleted[0] = true;

        // All other missions locked
        for (int i = 1; i < missionCompleted.Length; i++)
        {
            missionCompleted[i] = false;
        }
    }
    FindFirstObjectByType<MainMenuManager>().UpdateMissionButtons();
}


}


