using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using IG.CGrid;

public class RewardVideoAd : MonoBehaviour {
	[SerializeField]
    private string UnityAdID;

	public GameObject VideoDialogPanel;
	public bool IsAdShown;

	GameObject InputManager;
    //public ModalWindow ModalWindow;

    private void Start () {
		//Advertisement.Initialize(UnityAdID, false);
	}

	public void ShowVideoDialog() {
        if(VideoDialogPanel != null)
		    VideoDialogPanel.SetActive(true);
        else {
            Debug.LogError("Ad Error", this);
        }
	}
	
	public bool CheckIfWantsToSeeAd() {
		//Time.timeScale = 0;

		//
	    return true;
	}

	//public void ShowAd()
	//{
	//	if (UnityAdID != null) 
	//	{
	//		ShowOptions Options = new ShowOptions ();
	//		Options.resultCallback = AdCallbackhandler;
		
	//		if (Advertisement.IsReady ("rewardedVideoZone")) 
	//		{
	//			Advertisement.Show ("rewardedVideoZone", Options);
	//		}
	//		else 
	//		{
	//			Advertisement.Initialize (UnityAdID, false);
	//		}
	//	}
	//}
	
	//void AdCallbackhandler(ShowResult Result)
	//{
	//	switch(Result)
	//	{
	//	case ShowResult.Finished:
	//		IsAdShown = true;
	//		VideoDialogPanel.SetActive (false);
	//		ScoreClockScript.I.TimeLeft = 5;
	//		ScoreClockScript.I.CountDown ();
	//		Time.timeScale = 1;
	//		ScoreClockScript.I.AllowTouch = true;
	//		break;
	//	}
	//}
}
