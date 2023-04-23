using DB_Core;

namespace DB_Game
{
    public class DBFoodManager : FoodDataAccess
    {
        public static FoodDataCollection Foods;
        public const int FOOD_COUNT = 10;
        private const string FOOD_CONFIG_PATH = "food_data";

        private IFoodUpgrader foodUpgrader;
        private IFoodUnlocker foodUnlocker;
        private IBakerFoodManager bakerFoodManager;

        private void OnEnable()
        {
            DBManager.Instance.SaveManager.Load<FoodDataCollection>(data =>
            {
                if (data != null)
                {
                    Foods = data;
                    DBDebug.Log("Saved food data loaded successfully");
                    
                    base.Awake();
                }
                else
                {
                    Manager.ConfigManager.GetConfigAsync<FoodDataCollection>(FOOD_CONFIG_PATH, OnConfigLoaded);
                    DBDebug.Log("Default Data Loaded Successfully");
                }

                InitializeManagers();
            });
        }

        private void InitializeManagers()
        {
            foodUpgrader = new FoodUpgrader(foodDataRepository, Manager);
            foodUnlocker = new FoodUnlocker(foodDataRepository, Manager);
            bakerFoodManager = new BakerFoodManager(foodDataRepository, Manager);
        }

        private void OnConfigLoaded(FoodDataCollection configData)
        {
            Foods = configData;
            DBDebug.Log("OnConfigLoaded Success");

            base.Awake();
        }

        private void Start() => InitializeFoodData();

        public void LearnRecipe(int foodIndex) => foodUnlocker.LearnRecipe(foodIndex);

        public void UpgradeFood(int foodIndex) => foodUpgrader.UpgradeFood(foodIndex);

        public void UnlockOrUpgradeIdleFood(int foodIndex) => bakerFoodManager.UnlockOrUpgradeIdleFood(foodIndex);

        private void InitializeFoodData()
        {
            for (int i = 0; i < FOOD_COUNT; i++)
            {
                var foodData = foodDataRepository.GetFoodData(i);
                AddFoodToUpgradeablesData(i);
                LoadFoodsStats(foodData, i);
            }
        }

        private void AddFoodToUpgradeablesData(int foodIndex)
        {
            GameLogic.UpgradeManager.PlayerUpgradeInventoryData.Upgradeables.Add(new DBUpgradeableData
            {
                upgradableTypeID = UpgradeablesTypeID.Food,
                CurrentLevel = 1,
                foodID = foodIndex
            });
        }

        private void LoadFoodsStats(FoodData foodData, int i)
        {
            InvokeEvent(DBEventNames.OnUpgraded, i);
            InvokeEvent(DBEventNames.OnHired, i);
            InvokeEvent(DBEventNames.OnLearnRecipe, i);

            if (foodData.IsFoodLocked)
            {
                InvokeEvent(DBEventNames.FoodBarLocked, i);
            }
        }
    }
}