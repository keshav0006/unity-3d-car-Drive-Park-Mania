using UnityEngine;

public class ResetPrefs : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs Cleared!");

        // Remove script after running once
        Destroy(this);
    }
}
