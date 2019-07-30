using UnityEngine;
using System.Collections;
using IG.CGrid;


public class PopupScoreScript : MonoBehaviour {

    public GameObject UIPopupObject;


	private void Update() {
		if(transform.position.y < 2) {
			transform.Translate (Vector2.up * 0.1f);
		}
		else
		{
			Destroy(UIPopupObject);
		}
	}

    public void ShowPopup() {

        UIPopupObject.GetComponent<MeshRenderer>().sortingOrder = 10;
        UIPopupObject.GetComponent<TextMesh>().text = "+" + ManagerClockScore.I.ScoreInfo.CurrentTurnScore.ToString();
    }
}
