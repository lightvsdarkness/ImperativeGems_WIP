using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace IG.CGrid {
    /// <summary>
    /// UI-скрипт, показывающий счёт и создающий попап, показыывающий счёт
    /// </summary>
    public class UIScore : MonoBehaviour {
        public float TimeScoreDetailVisibleIfClicked = 2f; // When mouse is hovering - always show it and don't shide
        public bool ScoreDetailShouldBeVisible;

        [Space]
        [SerializeField] private TextMeshProUGUI ScoreCurrentText = null;

        [Space]
        [SerializeField] private GameObject BlockScoreDetail = null;
        [SerializeField] private TextMeshProUGUI ScoreFromGemsText = null;
        [SerializeField] private TextMeshProUGUI ScoreBonusWaveText = null;


        private void Start() {
            if (ScoreCurrentText == null)
                ScoreCurrentText = GetComponent<TextMeshProUGUI>();
            if (ScoreCurrentText == null)
                Debug.LogError("Set ScoreCurrentText", this);

            if (BlockScoreDetail == null)
                Debug.LogError("Set BlockScoreDetail", this);
            if (ScoreFromGemsText == null)
                Debug.LogError("Set ScoreFromGemsText", this);
            if (ScoreBonusWaveText == null)
                Debug.LogError("Set ScoreWaveBonusText", this);

            ScoreCurrentText.color = ManagerCGridGame.I.ColorScheme.CurrentColorUIText;


            ManagerClockScore.I.OnScoreChanged += RefreshScore;

            Canvas.ForceUpdateCanvases();
            StartCoroutine(ActuallyHideScoreDetail());
        }


        //private void PopupScore () 
        //{
        //  Instantiate(PopupScorePrefab, Vector2.zero, Quaternion.identity);
        //}

        public void RefreshScore(ScoreDetails scoreDetails) {
            ScoreCurrentText.text = scoreDetails.ScoreTotal.ToString();

            ScoreFromGemsText.text = scoreDetails.ScoreFromGems.ToString();
            ScoreBonusWaveText.text = scoreDetails.ScoreBonusWave.ToString();
            
        }

        public void HideScoreDetail() {
            StartCoroutine(ActuallyHideScoreDetail());
        }

        public void ShowScoreDetail() {
            ActuallyShowScoreDetail();
        }

        public void QueryToKeepScoreDetailShown() {
            ShowScoreDetail();

            StartCoroutine(FlagToHideAfterDelayScoreDetail());
        }


        private IEnumerator FlagToHideAfterDelayScoreDetail() {
            ScoreDetailShouldBeVisible = true;
            yield return new WaitForSeconds(TimeScoreDetailVisibleIfClicked);

            ScoreDetailShouldBeVisible = false;
        }


        private IEnumerator ActuallyHideScoreDetail() {
            while (ScoreDetailShouldBeVisible) {
                yield return new WaitForSeconds(0.1f);
            }

            BlockScoreDetail.SetActive(false);
        }

        private void ActuallyShowScoreDetail() {
            BlockScoreDetail.SetActive(true);
        }

    }
}