using DB_Core;
using System;
using TMPro;
using UnityEngine;

namespace DB_Game
{
    public class DBTextsManager : FoodDataAccess
    {
        [Header("Managers")]
        [SerializeField] private DBCurrencyManager currencyManager;
        [SerializeField] private DBToastingManager toastingManager;
        [SerializeField] private DBCookingUIManager cookingUIManager;

        [Header("Texts")]
        [SerializeField] private Texts texts;

        private void OnEnable() => RegisterEvents();
   
        private void OnDisable() => UnregisterEvents();

        private void Start() => UpdateCurrencyAmountText();

        #region Currency Texts

        private void UpdateCurrencyAmountText()
        {
            UpdateCoinsAmountText();
            UpdateStarsAmountText();
        }

        private void UpdateCoinsAmountText()
        {
            var moneyText = texts.MoneyText;
            double currentCurrency = currencyManager.currencySaveData.CoinsAmount;
            moneyText.text = currentCurrency.ToReadableNumber();
        }

        private void UpdateStarsAmountText()
        {
            var starText = texts.StarText;
            double currentStars = currencyManager.currencySaveData.StarsAmount;
            starText.text = currentStars.GetFormattedNumber();
        }

        private void OnCoinsAmountUpdate(object obj)
        {
            double currency = 0;

            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref currency))
            {
                var moneyText = texts.MoneyText;
                moneyText.text = currency.ToReadableNumber();
            }
        }

        private void OnStarsAmountUpdate(object obj)
        {
            double premCurrency = 0;

            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.PremiumCurrency, ref premCurrency))
            {
                var starText = texts.StarText;
                starText.text = premCurrency.GetFormattedNumber();
            }
        }

        #endregion

        #region Texts Updates On Actions

        private void OnUpgradeTextUpdate(object obj)
        {
            int index = (int)obj;
            int foodLevel = GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, index).CurrentLevel;
            var foodData = GetFoodData(index);
            double foodProfit = foodData.Profit;
            double upgradeCost = foodData.UpgradeCost;

            UpdateUpgradeTexts(index, foodLevel, foodProfit, upgradeCost);
            UpdateCurrencyAndButtonCheck();
        }

        private void LearnRecipeTextUpdate(object obj) 
        {
            int index = (int)obj;
            var foodData = GetFoodData(index);
            double learnCost = foodData.UnlockCost;
            var learnRecipeCostText = texts.LearnRecipeCostText[index];

            learnRecipeCostText.text = learnCost.ToReadableNumber();

            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        private void OnHireTextUpdate(object obj)
        {
            int index = (int)obj;
            var foodData = GetFoodData(index);

            double hireCost = foodData.HireCost;
            double cookFoodMultiplier = foodData.CookFoodMultiplier;
            int bakersCount = foodData.BakersCount;

            UpdateHireTexts(index, bakersCount, cookFoodMultiplier, hireCost);
            UpdateCurrencyAndButtonCheck();
        }

        private void UpdateCurrencyAndButtonCheck()
        {
            currencyManager.EarnCoins(currencyManager.initialCoinsAmount);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        private void UpdateHireTexts(int index, int bakersCount, double cookFoodMultiplier, double hireCost)
        {
            var bakersCountText = cookingUIManager.uiBakerComponents.BakersCountText[index];
            var cookFoodMultiplierText = cookingUIManager.uiBakerComponents.CookFoodMultiplierText[index];
            var hireCostText = texts.HireCostText[index];

            bakersCountText.text = $"x{bakersCount}";
            cookFoodMultiplierText.text = $"x{cookFoodMultiplier.ToReadableNumber()}";
            hireCostText.text = hireCost.ToReadableNumber();
        }

        private void UpdateUpgradeTexts(int index, int foodLevel, double foodProfit, double upgradeCost)
        {
            var foodLevelText = texts.FoodLevelText[index];
            var foodProfitText = texts.FoodProfitText[index];
            var upgradeCostText = texts.UpgradeCostText[index];

            foodLevelText.text = $"Lv. {foodLevel}";
            foodProfitText.text = foodProfit.ToReadableNumber(1);
            upgradeCostText.text = upgradeCost.ToReadableNumber();
        }

        #endregion

        #region Text Toastings

        private void MoneyTextToastAfterActiveCooking(object obj) 
        {
            InvokeEvent(DBEventNames.CookFoodButtonCheck, null);
            int index = (int)obj;
            double foodProfit = GetFoodData(index).Profit;
            double totalFoodProfit = foodProfit * DBDoubleProfitController.DoubleProfitMultiplier;
            toastingManager.DisplayMoneyToast(totalFoodProfit, PoolNames.MoneyToast);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        private void MoneyTextToastAfterBakerCooking(object obj) 
        {
            int index = (int)obj;
            double foodProfit = GetFoodData(index).Profit;
            double cookFoodMultiplier = GetFoodData(index).CookFoodMultiplier;
            InvokeEvent(DBEventNames.CookFoodButtonCheck, null);
            double totalFoodProfit = foodProfit * cookFoodMultiplier * DBDoubleProfitController.DoubleProfitMultiplier;
            toastingManager.DisplayMoneyToast(totalFoodProfit, PoolNames.MoneyToast);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        private void SpendUpgradeMoneyTextToast(object obj) 
        {
            int index = (int)obj;
            double upgradeCost = GetFoodData(index).UpgradeCost;
            toastingManager.DisplayMoneyToast(upgradeCost, PoolNames.SpendMoneyToast);
        }

        private void SpendLearnRecipeMoneyTextToast(object obj) 
        {
            int index = (int)obj;
            double learnCost = GetFoodData(index).UnlockCost;
            InvokeEvent(DBEventNames.CookButtonAlphaOn, index);
            toastingManager.DisplayMoneyToast(learnCost, PoolNames.SpendMoneyToast);
            UpdateCurrencyAndButtonCheck();
        }

        private void SpendHireMoneyTextToast(object obj) 
        {
            int index = (int)obj;
            double hireCost = GetFoodData(index).HireCost;
            toastingManager.DisplayMoneyToast(hireCost, PoolNames.SpendMoneyToast);
        }

        #endregion

        #region Events Register/Unregister

        private void RegisterEvents()
        {
            AddListener(DBEventNames.OnCurrencySet, OnCoinsAmountUpdate);
            AddListener(DBEventNames.OnPremCurrencySet, OnStarsAmountUpdate);
            AddListener(DBEventNames.OnUpgradeTextUpdate, OnUpgradeTextUpdate);
            AddListener(DBEventNames.MoneyToastOnCook, MoneyTextToastAfterActiveCooking);
            AddListener(DBEventNames.MoneyToastOnAutoCook, MoneyTextToastAfterBakerCooking);
            AddListener(DBEventNames.OnUpgradeMoneySpentToast, SpendUpgradeMoneyTextToast);
            AddListener(DBEventNames.OnHireMoneySpentToast, SpendHireMoneyTextToast);
            AddListener(DBEventNames.OnHiredTextUpdate, OnHireTextUpdate);
            AddListener(DBEventNames.OnLearnRecipe, LearnRecipeTextUpdate);
            AddListener(DBEventNames.OnLearnRecipeSpentToast, SpendLearnRecipeMoneyTextToast);
        }

        private void UnregisterEvents()
        {
            RemoveListener(DBEventNames.OnCurrencySet, OnCoinsAmountUpdate);
            RemoveListener(DBEventNames.OnUpgradeTextUpdate, OnUpgradeTextUpdate);
            RemoveListener(DBEventNames.MoneyToastOnCook, MoneyTextToastAfterActiveCooking);
            RemoveListener(DBEventNames.MoneyToastOnAutoCook, MoneyTextToastAfterBakerCooking);
            RemoveListener(DBEventNames.OnUpgradeMoneySpentToast, SpendUpgradeMoneyTextToast);
            RemoveListener(DBEventNames.OnHireMoneySpentToast, SpendHireMoneyTextToast);
            RemoveListener(DBEventNames.OnHiredTextUpdate, OnHireTextUpdate);
            RemoveListener(DBEventNames.OnLearnRecipe, LearnRecipeTextUpdate);
            RemoveListener(DBEventNames.OnLearnRecipeSpentToast, SpendLearnRecipeMoneyTextToast);
            RemoveListener(DBEventNames.OnPremCurrencySet, OnStarsAmountUpdate);
        }

        #endregion
    }

    [Serializable]
    public class Texts
    {
        public TMP_Text MoneyText;
        public TMP_Text StarText;

        public TMP_Text[] FoodProfitText;
        public TMP_Text[] FoodLevelText;
        public TMP_Text[] UpgradeCostText;
        public TMP_Text[] HireCostText;
        public TMP_Text[] LearnRecipeCostText;
    }
}