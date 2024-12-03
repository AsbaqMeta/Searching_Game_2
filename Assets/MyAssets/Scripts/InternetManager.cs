using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetManager : MonoBehaviour
{
    public GameObject noInternetPanel;
    public static InternetManager Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        CheckInternetConnection();
    }


    private void CheckInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (!noInternetPanel.activeSelf)
            {
                Debug.Log("No internet connection.");
                noInternetPanel.SetActive(true);
                AudioManager.Instance.Play("WifiError");
            }
        }
        else if (noInternetPanel.activeSelf)
        {
            noInternetPanel.SetActive(false);
        }
    }
}
