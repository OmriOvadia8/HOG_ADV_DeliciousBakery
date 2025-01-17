using DB_Core;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

namespace DB_Game
{
    public class DBPassedTimeBonusComponent : DBLogicMonoBehaviour
    {
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private TMP_Text countdownText;
        [SerializeField] private float xOffsetPerDigit;
        [SerializeField] private RectTransform coinRectTransform;
        [SerializeField] Button gotItButton;
        [SerializeField] Button claimButton;
        private DBPauseCurrencyManager pauseCurrencyManager;
        private double totalReturnBonus;
        private float initialXPos;

        private void Awake()
        {
            pauseCurrencyManager = FindObjectOfType<DBPauseCurrencyManager>();
            initialXPos = coinRectTransform.anchoredPosition.x;
        }

        private void Start()
        {
            OpenOfflineRewardWindow(Manager.TimerManager.GetLastOfflineTimeSeconds());

            Manager.EventsManager.AddListener(DBEventNames.OfflineTimeRefreshed, OnRefreshedTime);

            if(totalReturnBonus == 0)
            {
                gotItButton.gameObject.SetActive(true);
                claimButton.gameObject.SetActive(false);
            }
            else
            {
                gotItButton.gameObject.SetActive(false);
                claimButton.gameObject.SetActive(true);
                StartCoroutine(ActivateButtonAfterDelay(5));
            }
        }

        private void OnDestroy() => Manager.EventsManager.RemoveListener(DBEventNames.OfflineTimeRefreshed, OnRefreshedTime);

        private void OnRefreshedTime(object timeValue)
        {
            int timePassed = (int)timeValue;
            OpenOfflineRewardWindow(timePassed);
        }

        public void GivePassiveBonusAccordingToTimePassed()
        {
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, totalReturnBonus);
            InvokeEvent(DBEventNames.CurrencyUpdateUI, null);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        public void GiveDoubleBonusAccordingToTimePassed()
        {
            InvokeEvent(DBEventNames.PlaySound, SoundEffectType.ButtonClick);

            if (totalReturnBonus > 0)
            {
                GivePassiveBonusAccordingToTimePassed();
                ShowAdOrDisplayFailure();
                InvokeEvent(DBEventNames.BuyButtonsCheck, null);
            }
        }

        private void ShowAdOrDisplayFailure()
        {
            if (Manager.AdsManager.IsAdReady())
            {
                Manager.AdsManager.ShowAd();
            }
            else
            {
                Manager.PopupManager.OpenPopup(DBPopupData.LoadingAdFailed);
            }
        }

        private void OpenOfflineRewardWindow(int timePassed)
        {
            totalReturnBonus = pauseCurrencyManager.PassedTimeFoodRewardCalc(timePassed);

            rewardText.text = totalReturnBonus.ToReadableNumber();
            float xPos = initialXPos - ((totalReturnBonus.ToReadableNumber().Length - 1) * xOffsetPerDigit);
            coinRectTransform.anchoredPosition = new Vector2(xPos, coinRectTransform.anchoredPosition.y);

            GivePassiveBonusAccordingToTimePassed();
        }

        private IEnumerator ActivateButtonAfterDelay(float delay)
        {
            claimButton.interactable = false; 

            float countdown = delay;
            while (countdown > 0)
            {
                countdownText.text = Mathf.Round(countdown).ToString();
                countdown -= Time.deltaTime;
                yield return null;
            }

            countdownText.text = ""; 
            claimButton.interactable = true; 
        }
    }
}