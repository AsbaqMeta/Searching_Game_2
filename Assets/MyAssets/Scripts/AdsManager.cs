using UnityEngine;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour
{
    private string intertestialId = "ca-app-pub-3940256099942544/1033173712";
    private string bannerId = "ca-app-pub-3940256099942544/6300978111";

    private InterstitialAd interstitialAd;
    private BannerView bannerView;
    private RewardedAd rewardedAd;

    public static AdsManager Instance;

    public bool enableAds;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);

        if (PlayerPrefs.HasKey("DisebleAds"))
            enableAds = false;
    }
    public void Start()
    {
        if (!enableAds)
            return;

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });
        LoadBannerAd();

        RequestInterstitial();
    }

    public void LoadBannerAd()
    {
        // create an instance of a banner view first.
        if (bannerView == null)
        {
            CreateBannerView();
        }
        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        bannerView.LoadAd(adRequest);
    }

    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (bannerView != null)
        {
            DestroyAd();
        }

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
    }

    public void DestroyAd()
    {
        if (bannerView != null)
        {
            Debug.Log("Destroying banner ad.");
            bannerView.Destroy();
            bannerView = null;
        }
    }
    private void RequestInterstitial()
    {
        // Clean up the old ad before loading a new one.
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        InterstitialAd.Load(intertestialId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                interstitialAd = ad;
            });
    }

    public void ShowInterstitialAd()
    {
        if (!enableAds)
            return;

        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
            RequestInterstitial();
        }
    }

    public void DisebleAds()
    {
        PlayerPrefs.SetString("DisebleAds", "");
        enableAds = false;
        //FindObjectOfType<CodelessIAPButton>().gameObject.SetActive(false);
    }
}