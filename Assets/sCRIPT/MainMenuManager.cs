using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject selectCarUI;
    public GameObject selectMissionUI;
    public GameObject missionInformation;
    public MonoBehaviour[] carControllerScripts;
    public Button[] missionButtons;

    public GameObject menuCamera;
    public GameObject mainCamera;

    private bool isGamePaused = false;

    public Button resumeButton;


    void Start()
    {
        ShowMainMenu();
        InitializeMissionButton();
        UpdateMissionButtons();
        resumeButton.gameObject.SetActive(false);
    }

    public void OpenSelectMissionUI()
{
    mainMenu.SetActive(false);
    selectCarUI.SetActive(false);
    selectMissionUI.SetActive(true);
}

public void OpenSelectCarUI()
{
    mainMenu.SetActive(false);
    selectCarUI.SetActive(true);
    selectMissionUI.SetActive(false);
}

public void BackToMainMenu()
{
    mainMenu.SetActive(true);
    selectCarUI.SetActive(false);
    selectMissionUI.SetActive(false);
}

public void QuitGame()
{
    Application.Quit();
    Debug.Log("Game Quit"); // Works only in editor
}

public void ShowMainMenu()
{

    // ENABLE LISTENER ON MENU CAMERA, DISABLE ON MAIN CAMERA
    menuCamera.GetComponent<AudioListener>().enabled = true;
    mainCamera.GetComponent<AudioListener>().enabled = false;

    isGamePaused = true;
    Time.timeScale = 0f;

    mainMenu.SetActive(true);
    selectCarUI.SetActive(false);
    selectMissionUI.SetActive(false);

    if (menuCamera != null)
        menuCamera.SetActive(true);

    if (mainCamera != null)
        mainCamera.SetActive(false);

    if (missionInformation != null)
        missionInformation.SetActive(false);

    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    resumeButton.gameObject.SetActive(isGamePaused && Time.timeSinceLevelLoad > 0);
}


public void ResumeGame()
{
    
    menuCamera.GetComponent<AudioListener>().enabled = false;
    mainCamera.GetComponent<AudioListener>().enabled = true;

    isGamePaused = false;
    Time.timeScale = 1f;

    mainMenu.SetActive(false);
    selectCarUI.SetActive(false);
    selectMissionUI.SetActive(false);

    if (menuCamera != null)
        menuCamera.SetActive(false);

    if (mainCamera != null)
        mainCamera.SetActive(true);

    if (missionInformation != null)
        missionInformation.SetActive(true);

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    }


    public void ReplayMission(int missionIndex)
    {
        
       GameManager.Instance.ReplayMission(missionIndex);
       GameManager.Instance.SetActiveMissionArea();
       GameManager.Instance.SpawnPlayerAtMissionStart();
       GameManager.Instance.ShowMissionTextForCurrentMission();

       ResumeGame();
    }
   
    

public void UpdateMissionButtons()
   {
    for (int i = 0; i < missionButtons.Length; i++)
    {
        missionButtons[i].interactable = GameManager.Instance.missionCompleted[i] || i == GameManager.Instance.currentMission;
    }
}
    void InitializeMissionButton()
    {
        for(int i = 0; i < missionButtons.Length; i++)
        {
            int missionIndex=i;
            if (i < GameManager.Instance.missionCompleted.Length)
            {
                missionButtons[i].interactable=GameManager.Instance.missionCompleted[i];
                missionButtons[i].onClick.AddListener(()=>ReplayMission(missionIndex));
            }
        }
    }
    void Update()
{
    if (Input.GetKeyDown(KeyCode.Escape))
    {
    
        if (!isGamePaused)
            ShowMainMenu();
        else
            ResumeGame();
    }
}


}
