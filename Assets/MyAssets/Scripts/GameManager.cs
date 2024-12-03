using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Purchasing;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public GameObject winPanel, startPanel, commingSoonPanel, newAreaUnlockPopup;
    public Text levelText;
    CameraDrag cameraDrag;

    void Awake()
    {
        Application.targetFrameRate = 60;
        int levelNo = PlayerPrefs.GetInt("Level", 0);

        if (SceneManager.GetActiveScene().buildIndex != levelNo)
            SceneManager.LoadScene(levelNo);

        int num = levelNo + 1;
        levelText.text = "Level " + num.ToString();

        if (PlayerPrefs.HasKey("DisebleAds"))
            Destroy(FindObjectOfType<CodelessIAPButton>().gameObject);

        cameraDrag = FindObjectOfType<CameraDrag>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.DeleteAll();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            Next();
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                //Debug.Log("Touch is over a UI element");
                MouseDown();
            }
            else
            {
                //Debug.Log("Touch is not over a UI element");
                MouseUp();
            }
        }
    }

    public void MouseDown()
    {

        if (cameraDrag.enabled)
            cameraDrag.enabled = false;
    }

    public void MouseUp()
    {

        if (!cameraDrag.enabled)
            cameraDrag.enabled = true;
    }

    public void Reload()
    {
        AudioManager.Instance.Play("Click");

      /*  AdsManager.Instance.ShowInterstitialAd();*/

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowNewAreaPopup()
    {
        GetComponent<AudioSource>().Play();

        newAreaUnlockPopup.SetActive(true);
        newAreaUnlockPopup.transform.DOScale(new Vector3(1, 1, 1), .5f);
    }

    public void StartGame()
    {
        AudioManager.Instance.Play("Click");
        startPanel.SetActive(false);
    }

    public void Win()
    {
        Invoke("InvokeWinGame", 1);
    }

    private void InvokeWinGame()
    {
        print("Win");
        AudioManager.Instance.Play("Win");

        if (SceneManager.GetActiveScene().buildIndex < 18)
        {
            winPanel.SetActive(true);
            winPanel.transform.DOScale(new Vector3(1, 1, 1), .5f);
        }
        else
        {
            commingSoonPanel.SetActive(true);
            commingSoonPanel.transform.DOScale(new Vector3(1, 1, 1), .5f);
        }
    }

    public void Next()
    {
        AudioManager.Instance.Play("Click");

       /* AdsManager.Instance.ShowInterstitialAd();*/

        PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        PlayerPrefs.SetInt("CurrentFill", 0);
    }

    public void ShowInterstitialAds()
    {
        /*AdsManager.Instance.ShowInterstitialAd();*/
    }

    public void SoundToggle()
    {
        AudioManager.Instance.Play("Click");
        AudioManager.Instance.SoundToggle();
    }

    public void ResetGame()
    {
       /* AdsManager.Instance.ShowInterstitialAd();*/

        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }

    public void RemoveAdsIAP()
    {
        AdsManager.Instance.DisebleAds();
    }
}
