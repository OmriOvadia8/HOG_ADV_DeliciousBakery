using DB_Core;

namespace DB_Game
{
    public class DBAchievementsDataManager : DBLogicMonoBehaviour
    {
        public AchievementDataCollection AchievementsData;
        public AchievementClaimData AchievementClaimData;
        private const string ACHIEVEMENTS_CONFIG_PATH = "achievements";

        private void OnEnable() => LoadAchievementsData();

        private void LoadAchievementsData()
        {
            Manager.SaveManager.Load<AchievementDataCollection>(data =>
            {
                if (data != null)
                {
                    LoadSavedAchievementsData(data);
                }
                else
                {
                    LoadDefaultAchievementsData();
                }
            });

            LoadAchievementClaims();
        }

        private void LoadSavedAchievementsData(AchievementDataCollection data)
        {
            AchievementsData = data;
            DBDebug.Log("Saved achievements data loaded successfully");
        }

        private void LoadDefaultAchievementsData()
        {
            Manager.ConfigManager.GetConfigAsync<AchievementDataCollection>(ACHIEVEMENTS_CONFIG_PATH, OnAchievementsConfigLoaded);
            DBDebug.Log("Default Data Achievements Loaded Successfully");
        }

        private void OnAchievementsConfigLoaded(AchievementDataCollection configData)
        {
            AchievementsData = configData;
            DBDebug.Log("OnConfigLoaded Achievements Success");
        }

        public void SaveAchievementsData()
        {
            Manager.SaveManager.Save(AchievementsData);
        }

        private void LoadAchievementClaims() =>
        Manager.SaveManager.Load((AchievementClaimData data) =>
        AchievementClaimData = data ?? new AchievementClaimData(DBFoodManager.FOOD_COUNT, DBAchievementsController.FoodItemsAchievementsRewards.Length));

        public void SaveAchievementClaims()
        {
            Manager.SaveManager.Save(AchievementClaimData);
        }
    }
}