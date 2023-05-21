using DB_Core;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DB_Game
{
    public class DBPassedTimeBonusComponent : DBLogicMonoBehaviour
    {
        [SerializeField] private TMP_Text rewardText;
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
        }

        public void GiveDoubleBonusAccordingToTimePassed()
        {
            GivePassiveBonusAccordingToTimePassed();

            if (totalReturnBonus > 0)
            {
                DBExtension.WatchAd();
            }

            InvokeEvent(DBEventNames.PlaySound, SoundEffectType.ButtonClick);
        }

        private void OpenOfflineRewardWindow(int timePassed)
        {
            totalReturnBonus = pauseCurrencyManager.PassedTimeFoodRewardCalc(timePassed);

            rewardText.text = totalReturnBonus.ToReadableNumber();
            float xPos = initialXPos - (totalReturnBonus.ToReadableNumber().Length - 1) * xOffsetPerDigit;
            coinRectTransform.anchoredPosition = new Vector2(xPos, coinRectTransform.anchoredPosition.y);

            GivePassiveBonusAccordingToTimePassed();
        }
    }
}