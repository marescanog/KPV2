using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int ItemID { get; private set; }
    [Header("Collider UI Display")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _transform;
    [SerializeField] private BoxCollider2D _collider2d;
    [Header("Properties")]
    [SerializeField] private Appliance _appliance;
    [SerializeField] private Food _food;
    [SerializeField] private Tool _tool;
    [SerializeField] private Material _material;
    [Header("Surface Display")]
    [SerializeField] private SpriteRenderer _surfaceDisplayRenderer;
    [SerializeField] private Transform _surfaceDisplayTransform;
    [SerializeField] private MaskedItemManager _surfaceDisplayMaskRenderer;
    [SerializeField] private SpriteMask _surfaceSpriteMask;
    [Header("Other UI Elements")]
    [SerializeField] private BarScript _progressBarThisItem;
    public Appliance Appliance { get; private set; }
    public Food Food { get; private set; }
    public Tool Tool { get; private set; }
    public Material Material { get; private set; }

    public ItemCategory ItemCategory_My { get { return GameAssets.ItemDataReadOnly(ItemID)?.itemCategory ?? ItemCategory.None; } }
    public Sprite Sprite_My { get { 
            if (ItemCategory_My == ItemCategory.Food)
            {
                return Food.GetSprite() ?? GameAssets.i.placeholderSprite;
            } else
            {
                return GameAssets.ItemDataReadOnly(ItemID)?.baseSprite ?? GameAssets.i.placeholderSprite;
            }
        } }
    public string Name_My { get { return GameAssets.ItemDataReadOnly(ItemID)?.itemName ?? "No_Name_Set_In_Item_ScriptableObject"; } }
    public SO_Item ItemReadOnlyData { get { return GameAssets.ItemDataReadOnly(ItemID); } }
    /*
    public Item(SO_Item item)
    {
        _itemID = item.itemID;
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Debug.Log(GameAssets.SpriteItem(_itemID)?.name);
    }
    */

    public BarScript ProgressBar_Appliance { get; private set; }

    private void Awake()
    {
        // _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        // _transform = gameObject.GetComponent<Transform>();
        Appliance = _appliance;
        Food = _food;
        Tool = _tool;
        Material = _material;
    }

    void OnDisable()
    {
        // Debug.Log("Item(OnDisable): script was disabled");
    }

    void OnEnable()
    {
        // Debug.Log("Item(OnDisable): script was enabled");
    }

    public void SetItemData()
    {
        // Food, appliance, tool, etc specific data?
        // we will see if this function is still needed or not
    }

    public Item TryDetachToolFromAppliance(out MaskRendererSpriteAssets maskRendererAssets)
    {
        Debug.Log("Item(TryDetachToolFromAppliance)");
        Item retVal = null;
        maskRendererAssets = null;
        if (ItemCategory_My == ItemCategory.Appliance && Appliance != null)
        {
            retVal = Appliance.GetItemHeld(out maskRendererAssets);
            if(retVal != null)
            {
                RemoveSurfaceSprite();
            }
        }
        return retVal;
    }

    public bool IsInToolWhiteList(int toolItemID)
    {
        bool retVal = false;
        if (ItemCategory_My == ItemCategory.Appliance && Appliance != null && Appliance.ApplianceData_ReadOnly != null)
        {
            retVal = (Appliance.ApplianceData_ReadOnly.toolWhiteList.Contains(toolItemID) || Appliance.ApplianceData_ReadOnly.toolWhiteList.Count <= 0);
        } else {
            Debug.LogError($"Item(IsInToolWhiteList): Readonly Appliance Data for {Name_My} with item ID {ItemID} is not found");
        }
        return retVal;
    }



    public Item GetFromItemHeld(out bool isFromContainer)
    {
        MaskRendererSpriteAssets mrSa;
        Item returnItem = Appliance.GetFromItemHeld(out isFromContainer, out mrSa);
        if (isFromContainer)
        {
            _surfaceDisplayMaskRenderer.CopySpriteAssets(mrSa);
        }
        return returnItem;
    }

    public Item GetFromToolContainer()
    {
        // return Tool.GetFromContainerList();
        return null;
    }

    public void SetItem(Vector3 location, int itemID)
    {
        SO_Item itemReadOnlyData = GameAssets.ItemDataReadOnly(itemID);
        if (itemReadOnlyData)
        {
            ItemID = itemID;
            _transform.localPosition = location;
            _spriteRenderer.sprite = itemReadOnlyData.baseSprite;
            _surfaceDisplayTransform.localPosition = itemReadOnlyData.surfaceDisplayPosition;
            DisableAllItemProperties();
            switch (itemReadOnlyData.itemCategory)
            {
                case ItemCategory.Appliance:
                    // Debug.Log("Item(SetItem): This Item is an Appliance");
                    _appliance.enabled = true;
                    _appliance.SetApplianceData(GameAssets.ApplianceDataReadOnly(itemID), itemID);
                    break;
                case ItemCategory.Food:
                    // Debug.Log("Item(SetItem): This Item is Food");
                    _food.enabled = true;
                    _food.SetFoodData(GameAssets.FoodDataReadOnly(itemID), itemID);
                    break;
                case ItemCategory.Material:
                    // Debug.Log("Item(SetItem): This Item is a Material");
                    _material.enabled = true;
                    break;
                case ItemCategory.Tool:
                    // Debug.Log("Item(SetItem): This Item is a Tool");
                    _tool.enabled = true;
                    _tool.SetToolData(GameAssets.ToolDataReadOnly(itemID), itemID);
                    break;
            }
        }
        else
        {
            Debug.LogError($"Item(SetItem): Sprite not found in GameAssets _itemBase dictionary for item ID {itemID}");
        }

    }

    public void UpdateSprite(Sprite newSprite)
    {
        _spriteRenderer.sprite = newSprite;
    }

    public void SetSurfaceSprite(Item ItemToSet)
    {
        if(ItemToSet != null)
        {
            if (_surfaceDisplayRenderer != null)
            {
                if (!_surfaceDisplayRenderer.enabled)
                {
                    _surfaceDisplayRenderer.enabled = true;
                }
                _surfaceDisplayRenderer.sprite = ItemToSet.Sprite_My;
            }

            _surfaceDisplayMaskRenderer.CopySpriteAssets(ItemToSet.GetMaskRendererSpriteAssets());
        }
    }

    public void SetSurfaceSprite(Sprite newSprite)
    {
        if (_surfaceDisplayRenderer != null)
        {
            if (!_surfaceDisplayRenderer.enabled)
            {
                _surfaceDisplayRenderer.enabled = true;
            }
            _surfaceDisplayRenderer.sprite = newSprite;
        }
    }

    public void RemoveSurfaceSprite()
    {
        Debug.Log("Item(RemoveSurfaceSprite) for _surfaceDisplayMaskRenderer & Disable & Clear ProgressBar for Item's appliance ");
        if (_surfaceDisplayRenderer != null)
        {
            _surfaceDisplayRenderer.enabled = false;
            _surfaceDisplayRenderer.sprite = GameAssets.i.placeholderSprite;
            _surfaceDisplayMaskRenderer.DisableEraseAllSpriteRenderers();
        }
    }

    public void DisableProgressBar()
    {
        if (ProgressBar_Appliance != null)
        {
            ProgressBar_Appliance.DisableBar();
            ProgressBar_Appliance = null;
        }
    }

        public bool TrySetItemSurface(Item ItemToSet) // I am the item in front
    {
        bool isSuccess = false;

        // refactor later
        if(ItemReadOnlyData!= null)
        {
            Debug.Log($"{ItemReadOnlyData.itemName}");
            Debug.Log($"Item(TrySetItemSurface): current{_surfaceDisplayTransform.localPosition} vs toSet{ItemReadOnlyData.surfaceDisplayPosition}");
            if(ItemReadOnlyData.surfaceDisplayMask != null)
            {
                _surfaceSpriteMask.enabled = true;
                _surfaceSpriteMask.sprite = ItemReadOnlyData.surfaceDisplayMask;
            }
        }

        if (ItemToSet != null)
        {
            var categoryName_ItemToSet = ItemToSet==null?"NULL":Enum.GetName(typeof(ItemCategory), ItemToSet.ItemCategory_My); // Delete later

            switch (ItemCategory_My)
            {
                case ItemCategory.Appliance: // item category of me
                    Debug.Log($"item(TrySetItemSurface): Try setting Item({categoryName_ItemToSet}):{ItemToSet?.Name_My??"NULL"} on Appliance:{Name_My}");
                    // check if the item to set is NOT an apliance
                    if (ItemToSet.ItemCategory_My != ItemCategory.Appliance)
                    {
                        SO_Appliance appData = Appliance.ApplianceData_ReadOnly;
                        // check if the appliance (me) has a whitelist
                        if (Appliance != null && appData != null)
                        {
                            if(appData.itemWhiteList.Count == 0 || appData.itemWhiteList.Contains(ItemToSet.ItemID))
                            {
                                isSuccess = Appliance.TrySetItemHeld(ItemToSet);
                                if (isSuccess)
                                {
                                    Debug.Log("Item(TrySetItemSurface): Set Appliance Progress bar of item To Set");
                                    ItemToSet.SetApplianceProgressBar(_progressBarThisItem);
                                }
                            } else
                            {
                                Debug.Log($"Item(TrySetItemSurface): The item to set({ItemToSet?.Name_My}) is not in the whitelist of the item surface ({Name_My})");
                            }
                        } else
                        {
                            Debug.LogError($"Item(TrySetItemSurface): The item with surface ({Name_My} - {ItemID}) does not have appData.");
                        }

                        if (isSuccess)
                        {
                            SetSurfaceSprite(ItemToSet);
                        }
                    }

                    break;
                case ItemCategory.Tool: // item category of me
                    Debug.Log($"item(TrySetItemSurface): Try setting Item({categoryName_ItemToSet}) on Tool");
                    // check if the item to set is not a tool
                    switch (ItemToSet.ItemCategory_My)
                    {
                        case ItemCategory.Food: // item category of the item to set
                            isSuccess = Tool.TryAddItemContainer(ItemToSet);
                            if (isSuccess)
                            {
                                // Update UI
                            }
                            break;
                        case ItemCategory.Material:
                            // Add match to matchbox or something
                            break;
                    }
                    break;
                case ItemCategory.Food:
                    Debug.Log($"item(TrySetItemSurface): Try setting Item({categoryName_ItemToSet}) on Food");
                    // Food.TrySetItem(ItemToSet);
                    break;
                case ItemCategory.Material:
                    Debug.Log($"item(TrySetItemSurface): Try setting Item({categoryName_ItemToSet}) on Material");
                    // Food.TrySetItem(ItemToSet);
                    break;
                case ItemCategory.Container:
                    Debug.Log($"item(TrySetItemSurface): Try setting Item({categoryName_ItemToSet}) on Container");
                    // Container.TrySetItem(ItemToSet);
                    break;
            }

            /*
            switch (ItemToSet.ItemCategory_My) // Maybe other way around
            {
                case ItemCategory.Appliance:
                    Debug.Log($"item(TrySetItemSurface): Try setting Item({categoryName}) on Appliance");
                    // Do nothing
                    break;
                case ItemCategory.Food:
                    Debug.Log($"item(TrySetItemSurface): Try setting Item({categoryName}) on Food");
                    // If I am an appliance or Tool Do something
                    break;
                case ItemCategory.Tool:
                    Debug.Log($"item(TrySetItemSurface): Try setting Item({categoryName}) on Tool");
                    // If I am an appliance or Food do something
                    break;
                case ItemCategory.Material:
                    Debug.Log($"item(TrySetItemSurface): Try setting Item({categoryName}) on material");
                    // If I am an appliance or Tool do something
                    break;
            }
            */
        }

        return isSuccess;
    }

    public void SetProgressBarValue(float totalValue)
    {
        if(ProgressBar_Appliance != null)
        {
            ProgressBar_Appliance.SetSize(totalValue);
        }
    }

    public void SetApplianceProgressBar(BarScript barscript)
    {
        ProgressBar_Appliance = barscript;
    }

    public void UseTool(Item itemInFront)
    {
        Debug.Log("Item(UseTool)");
        if (ItemCategory_My == ItemCategory.Tool)
        {
            Tool.Use(itemInFront);
        }

    }

    private void DisableAllItemProperties()
    {
        _appliance.enabled = false;
        _food.enabled = false;
        _material.enabled = false;
        _tool.enabled = false;
    }

    public void DisableUICollider()
    {
        _spriteRenderer.enabled = false;
        _collider2d.enabled = false;
        _surfaceDisplayRenderer.enabled = false;
    }

    public void EnableUICollider()
    {
        _spriteRenderer.enabled = true;
        _collider2d.enabled = true;
        if (Appliance.isActiveAndEnabled && Appliance.ItemHeld != null)
        {
            _surfaceDisplayRenderer.enabled = true;
        }
    }

    public void EnableUICollider(Vector3 newPosition)
    {
        EnableUICollider();
        _transform.position = newPosition;

        // If tool enable
        if(ItemCategory_My == ItemCategory.Tool)
        {
            Tool.EnableMask();
        }
    }
    
    public MaskRendererSpriteAssets GetMaskRendererSpriteAssets()
    {
        MaskRendererSpriteAssets retval = null;
        switch (ItemCategory_My)
        {
            case ItemCategory.Appliance:
                if(Appliance != null && Appliance.ItemHeld != null)
                {
                    if(Appliance.ItemHeld.ItemCategory_My == ItemCategory.Tool && Appliance.ItemHeld.Tool != null)
                    {
                        retval = Appliance.ItemHeld.Tool.GetMaskRendererSpriteAssets();
                    }
                }
                break;
            case ItemCategory.Tool:
                if(Tool != null)
                {
                    retval = Tool.GetMaskRendererSpriteAssets();
                }
                break;
        }
        return retval;
    }


    public bool UseToolWithoutAppliance(Vector3 position)
    {
        bool retval = false;
        if (Tool.ToolReadOnlyData_my.canUseWithoutAppliance)
        {
            Debug.Log("Item(TriggerHoldActionUse): Can Use without Appliance in front");
            retval = Tool.TryUseWithoutAppliance(position);
        }
        return Tool.ToolReadOnlyData_my.canUseWithoutAppliance;
    }
}
