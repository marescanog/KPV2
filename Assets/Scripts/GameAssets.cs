using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i; 
    private static Dictionary<int, SO_Item> _itemBase = new Dictionary<int, SO_Item>(); // RO
    private static Dictionary<int, SO_Appliance> _applianceBase = new Dictionary<int, SO_Appliance>();
    private static Dictionary<int, SO_Tool> _toolBase = new Dictionary<int, SO_Tool>();
    private static Dictionary<int, SO_Food> _foodBase = new Dictionary<int, SO_Food>();

    public static GameAssets i { get {
            if ( _i == null) { _i = Instantiate(Resources.Load<GameAssets>("Prefabs/Managers/GameAssets")); }
            return _i; }
    }

    public GameObject emptyObjectPrefab { get; private set; }
    public Sprite placeholderSprite { get; private set; }

    private void Awake()
    {
        _i = this;
        emptyObjectPrefab = Resources.Load<GameObject>("Prefabs/EmptyObject");
        placeholderSprite = Resources.Load<Sprite>("Sprites/Items/placeholder");
        LoadData();
    }


    private void LoadData()
    {
        LoadItemData();
        LoadApplianceData();
        LoadToolData();
        LoadFoodData();
        // LoadCookingSOData();
    }

    private void LoadFoodData()
    {
        UnityEngine.Object[] foodBases = Resources.LoadAll("Data/Food", typeof(SO_Food));
        foreach (SO_Food foodBase in foodBases)
        {
            if (!_applianceBase.ContainsKey(foodBase.itemID))
            {
                _foodBase.Add(foodBase.itemID, foodBase);
            }
            else
            {
                Debug.LogError("GameAssets(LoadApplianceData): Dictionary foodBases already contains key for " + foodBase.name + ". (" + foodBase.itemID + ", " + foodBases[foodBase.itemID].name + ")");
            }
        }
    }

    private void LoadToolData()
    {
        UnityEngine.Object[] toolBases = Resources.LoadAll("Data/Tool", typeof(SO_Tool));
        foreach (SO_Tool toolBase in toolBases)
        {
            if (!_applianceBase.ContainsKey(toolBase.itemID))
            {
                _toolBase.Add(toolBase.itemID, toolBase);
            }
            else
            {
                Debug.LogError("GameAssets(LoadApplianceData): Dictionary toolBases already contains key for " + toolBase.name + ". (" + toolBase.itemID + ", " + toolBases[toolBase.itemID].name + ")");
            }
        }
    }

    private void LoadApplianceData()
    {
        UnityEngine.Object[] applianceBases = Resources.LoadAll("Data/Appliance", typeof(SO_Appliance));
        foreach (SO_Appliance applianceBase in applianceBases)
        {
            if (!_applianceBase.ContainsKey(applianceBase.itemID))
            {
                _applianceBase.Add(applianceBase.itemID, applianceBase);
            }
            else
            {
                Debug.LogError("GameAssets(LoadApplianceData): Dictionary applianceBases already contains key for " + applianceBase.name + ". (" + applianceBase.itemID + ", " + applianceBases[applianceBase.itemID].name + ")");
            }
        }
    }

    private void LoadItemData()
    {
        UnityEngine.Object[] itemBases = Resources.LoadAll("Data/Items", typeof(SO_Item));
        foreach (SO_Item itemBase in itemBases)
        {
            if (!_itemBase.ContainsKey(itemBase.itemID))
            {
                _itemBase.Add(itemBase.itemID, itemBase);
                //_itemBaseRef.Add(itemBase.itemName, itemBase.itemID);
            }
            else
            {
                Debug.LogError("GameAssets(LoadItemData): Dictionary itemBases already contains key for " + itemBase.name + ". (" + itemBase.itemID + ", " + itemBases[itemBase.itemID].name + ")");
            }
        }
    }

    // For All Item Types -> Appliance, Tool, Food, Material
    public static Sprite SpriteItem(int itemID)
    {
        SO_Item result;
        _itemBase.TryGetValue(itemID, out result);
        return result.baseSprite;
    }

    public static SO_Item ItemDataReadOnly(int itemID)
    {
        SO_Item result;
        _itemBase.TryGetValue(itemID, out result);
        return result;
    }

    public static SO_Appliance ApplianceDataReadOnly(int itemID)
    {
        SO_Appliance result;
        _applianceBase.TryGetValue(itemID, out result);
        return result;
    }

    public static SO_Tool ToolDataReadOnly(int itemID)
    {
        SO_Tool result;
        _toolBase.TryGetValue(itemID, out result);
        return result;
    }

    public static SO_Food FoodDataReadOnly(int itemID)
    {
        SO_Food result;
        _foodBase.TryGetValue(itemID, out result);
        return result;
    }
}
