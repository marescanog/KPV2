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

    /*
    public Item(SO_Item item)
    {
        _itemID = item.itemID;
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Debug.Log(GameAssets.SpriteItem(_itemID)?.name);
    }
    */

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
        if (_surfaceDisplayRenderer != null)
        {
            _surfaceDisplayRenderer.enabled = false;
            _surfaceDisplayRenderer.sprite = GameAssets.i.placeholderSprite;
        }
    }

    public bool TrySetItemSurface(Item ItemToSet) // I am the item in front
    {
        bool isSuccess = false;

        if (ItemToSet != null)
        {
            var categoryName_ItemToSet = ItemToSet==null?"NULL":Enum.GetName(typeof(ItemCategory), ItemToSet.ItemCategory_My); // Delete later

            switch (ItemCategory_My)
            {
                case ItemCategory.Appliance:
                    Debug.Log($"item(TrySetItemSurface): Try setting Item({categoryName_ItemToSet}):{ItemToSet?.Name_My??"NULL"} on Appliance:{Name_My}");
                    // check if the item to set is an apliance
                    if(ItemToSet.ItemCategory_My != ItemCategory.Appliance)
                    {
                        isSuccess = Appliance.TrySetItemHeld(ItemToSet);
                        if (isSuccess)
                        {
                            SetSurfaceSprite(ItemToSet.Sprite_My);
                        }
                    }
                    break;
                case ItemCategory.Tool:
                    Debug.Log($"item(TrySetItemSurface): Try setting Item({categoryName_ItemToSet}) on Tool");
                    // Tool.TrySetItem(ItemToSet);
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
    }
    
}
