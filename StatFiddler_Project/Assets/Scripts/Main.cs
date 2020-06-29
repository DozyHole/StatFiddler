using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;
using System.IO;

public class Main : MonoBehaviour {
    public Transform CanvasMain;
    public Transform CanvasShare;
    private BannerView bannerView;

    public RectTransform BottomContainer;
    public RectTransform BottomContainer2;
    public Transform TxtDebug;

    public Transform SliderTable;
    public Transform PanelMessage;
    public Transform PanelBottom;
    public Transform PanelPromote;
    public Transform TxtHeading;
    public Transform TxtMessage;

    // interstitial
    private InterstitialAd interstitial;

    // todo - call destroy on adBanner and interstitial
    float nextAdTime        = 0.0f;
    float adWaitDuration    = 30.0f;
    float adWaitDurationIncrease = 10.0f;
    float adWaitDurationMax = 90.0f; 

    // we increase wait time between ads up to a maximum
    void stepAdTime()
    {
        nextAdTime = Time.time + adWaitDuration;
        adWaitDuration += adWaitDurationIncrease;
        if (adWaitDuration > adWaitDurationMax)
            adWaitDuration = adWaitDurationMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (nextAdTime < Time.time)
        {
            ShowInterstitial();
            stepAdTime();
        }
    }

    // Use this for initialization
    void Start () {
        stepAdTime();

#if UNITY_ANDROID
        string appId = "ca-app-pub-3290491453629228~5908540561";
    #elif UNITY_IPHONE
            string appId = "";
    #else
            string appId = "unexpected_platform";
    #endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);


        SwitchToMainCanvas();

        RequestBanner();
        RequestInterstitial();
    }

    void HideBeforeSceenshot()
    {
        PanelBottom.gameObject.SetActive(false);
        SliderTable.gameObject.SetActive(false);
        PanelPromote.gameObject.SetActive(true);
    }

    void ShowAfterScreenShot()
    {
        PanelBottom.gameObject.SetActive(true);
        SliderTable.gameObject.SetActive(true);
        PanelPromote.gameObject.SetActive(false);
    }

    public void ScreenShot()
    {
        // Take a screenshot and save it to Gallery/Photos
        HideBeforeSceenshot();
        StartCoroutine(TakeScreenshotAndShare());
    }

    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();
        
        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());
        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath).SetSubject("Stat Fiddler").SetText("").Share();


        ShowAfterScreenShot();
        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).SetText( "Hello world!" ).SetTarget( "com.whatsapp" ).Share();

    }

    private IEnumerator TakeScreenshotAndSave()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        // Save the screenshot to Gallery/Photos
        //TxtHeading.GetComponent<Text>().text = "Saving to Gallary...";
        //TxtMessage.GetComponent<Text>().text = "";
        //PanelMessage.gameObject.SetActive(true);
        //ButtonSave.gameObject.SetActive(false);

        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(ss, "StatFiddler", "table.png", msCallback);
        if(permission == NativeGallery.Permission.Denied)
        {
            msCallback("error - pernmission denied");
        }

        // To avoid memory leaks
        Destroy(ss);
    }

    private void msCallback(string error)
    {
        Debug.Log("msCallback called!");
        // show saved to gallary message
        if (error == null)
        {
            TxtHeading.GetComponent<Text>().text = "Saved to Gallary";
            TxtMessage.GetComponent<Text>().text = "Share on Social Media!";
        }
        else
        {
            TxtHeading.GetComponent<Text>().text = "Error";
            TxtMessage.GetComponent<Text>().text = "Save to Gallary failed";
        }

        PanelMessage.gameObject.SetActive(true);
        ShowAfterScreenShot();
        StartCoroutine(HideMessage(4.5f));
    }

    IEnumerator HideMessage(float t)
    {
        yield return new WaitForSeconds(t);
        PanelMessage.gameObject.SetActive(false);
    }

    public static float adHeight()
    {
        float f = Screen.dpi / 160f;
        float dp = Screen.height / f;
        return (dp > 720f) ? 90f * f
              : (dp > 400f) ? 50f * f
              : 32f * f;
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        //string adUnitId = "ca-app-pub-3940256099942544/6300978111"; // test ads
        string adUnitId = "ca-app-pub-3290491453629228/2518646819"; // my ads
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        AdSize myAdsize = new AdSize(Screen.width, 100);
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        // smart banner

        float dpScreenHeight = ((float)Screen.height * 160f) / Screen.dpi;
        float dpAdHeight = 0;
        if (dpScreenHeight <= 400.0)
            dpAdHeight = 32f;
        else if (dpScreenHeight > 720.0)
            dpAdHeight = 90f;
        else
            dpAdHeight = 50f;
        float pxAdHeight = dpAdHeight * (Screen.dpi / 160f);
        // fraction relative to parent, which is screen
        float value = pxAdHeight / Screen.height;
        BottomContainer.anchorMin = new Vector2(BottomContainer.anchorMin.x, BottomContainer.anchorMin.y + value);
        BottomContainer2.anchorMin = new Vector2(BottomContainer2.anchorMin.x, BottomContainer2.anchorMin.y + value);
        TxtDebug.GetComponent<Text>().text = "pxAdY:" + pxAdHeight + " pxScY: " + Screen.height + " dpAdY:" + dpAdHeight + " dpScY:" + dpScreenHeight; 

        // Called when an ad request has successfully loaded.
        bannerView.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        bannerView.OnAdOpening += HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        bannerView.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication;


        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().AddTestDevice("53EF627C1AD22DBBCAAF3296A78DE88D").Build();
        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        //string adUnitId = "ca-app-pub-3940256099942544/1033173712";  // test ads
        string adUnitId = "ca-app-pub-3290491453629228/7567318771";   // my ads
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().AddTestDevice("53EF627C1AD22DBBCAAF3296A78DE88D").Build();

        //setTestDeviceIds(Arrays.asList("53EF627C1AD22DBBCAAF3296A78DE88D") to get test ads on this device.

        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    private void ShowInterstitial()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
            this.interstitial.Destroy();
            RequestInterstitial();
        }

    }

    public void SwitchToShareCanvas()
    {
        CanvasMain.gameObject.SetActive(false);
        CanvasShare.gameObject.SetActive(true);
    }
    public void SwitchToMainCanvas()
    {
        CanvasMain.gameObject.SetActive(true);
        CanvasShare.gameObject.SetActive(false);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }
}
