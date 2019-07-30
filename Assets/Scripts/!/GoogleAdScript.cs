//using UnityEngine;
//using System.Collections;
//using GoogleMobileAds.Api;

//public class GoogleAdScript : MonoBehaviour 
//{
//	[SerializeField]
//	string AndroidBannerAdID;

//	[SerializeField]
//	string AndroidInterstitialAdID;

//	BannerView bannerView;
//	AdRequest request;
//	InterstitialAd interstitial;

//	public int Deaths;

//	void Start () 
//	{
//		DontDestroyOnLoad (gameObject);
//		RequestBanner ();
//		RequestInterstitial ();
//	}

//	public void RequestBanner()
//	{

//		#if UNITY_ANDROID
//		string adUnitId = AndroidBannerAdID;
//		#endif

//		// Create a 320x50 banner at the top of the screen.
//		bannerView = new  BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
//		// Create an empty ad request.
//		request = new AdRequest.Builder().Build();
//		// Load the banner with the request.
//		bannerView.LoadAd(request);
//		bannerView.Hide ();
//	}

//	public void RequestInterstitial()
//	{
//		#if UNITY_ANDROID
//		string adUnitId = AndroidInterstitialAdID;
//		#endif
		
//		// Initialize an InterstitialAd.
//		interstitial = new InterstitialAd(adUnitId);
//		// Create an empty ad request.
//		AdRequest request = new AdRequest.Builder().Build();
//		// Load the interstitial with the request.
//		interstitial.LoadAd(request);
//	}

//	public void ShowBannerAd()
//	{
//		bannerView.Show ();
//	}
	
//	public void DestroyBannerAd()
//	{
//		bannerView.Destroy ();
//	}

//	public void ShowInterstitialAd()
//	{
//		if (interstitial.IsLoaded ()) 
//		{
//			interstitial.Show ();
//		}
//	}
//}
