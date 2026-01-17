using UnityEngine;
using UnityEngine.UI;

public class MissionFailed : MonoBehaviour
{
    public GameObject missionFailedUI;
    public Button retryButton;
    public CarController[] carControllers;

    void Start()
    {
        missionFailedUI.SetActive(false);
        retryButton.onClick.AddListener(RetryMission); 
    }


    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.gameObject.CompareTag("Huddle"))
        {
            missionFailedUI.SetActive(true);
        
        foreach (var car in carControllers)
        {
            if (car != null)
            {
                car.maxAcceleration=0;
            }
        }
        
        Cursor.lockState=CursorLockMode.None;
        Cursor.visible=true;
        }

    }
    public void RetryMission()
    {
        foreach (var car in carControllers)
        {
            if (car != null)
            {
                car.ResetCar();

            }
        }

        GameManager.Instance.SpawnPlayerAtMissionStart();
        missionFailedUI.SetActive(false);
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;
    }
}