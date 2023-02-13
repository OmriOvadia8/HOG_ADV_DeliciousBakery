using System;
using System.Diagnostics;
using Core;
using UnityEngine.SocialPlatforms.Impl;

namespace Game
{
    public class HOGGameLogic : IHOGBaseManager
    {
        public static HOGGameLogic Instance;

        public HOGScoreManager ScoreManager;
        public HOGUpgradeManager UpgradeManager;
        public HOGMoneyHolder PlayerMoney;
        public UIManager UI;
        
        
        public HOGGameLogic()
        {
            if (Instance != null)
            {
                return;
            }
            Instance = this;
        }

        public void LoadManager(Action onComplete)
        {
            ScoreManager = new HOGScoreManager();
            UpgradeManager = new HOGUpgradeManager();
            PlayerMoney = new HOGMoneyHolder();
       
            onComplete.Invoke();
        }
    }
}