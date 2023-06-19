using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Inventory : MonoBehaviour
{
    [SerializeField] private Image itemHand;
    [SerializeField] private Image toolHand; 
    [SerializeField] private Image itemHandBG;
    [SerializeField] private Image toolHandBG;

    private void Awake()
    {
        Inventory.OnInventoryItemPickUp += DisplayOnItemHand;
        Inventory.OnInventoryItemDrop += HideFromItemHand;
        Inventory.OnInventoryToolEquip += DisplayOnToolHand;
        Inventory.OnInventoryToolDrop += HideFromToolHand;
    }

    private void OnDestroy()
    {
        Inventory.OnInventoryItemPickUp -= DisplayOnItemHand;
        Inventory.OnInventoryItemDrop -= HideFromItemHand;
        Inventory.OnInventoryToolEquip -= DisplayOnToolHand;
        Inventory.OnInventoryToolDrop -= HideFromToolHand;
    }

    private void DisplayOnItemHand(Sprite newSprite, MaskRendererSpriteAssets maskRendererAssets)
    {
        if (!itemHand.isActiveAndEnabled)
        {
            itemHandBG.enabled = true;
            itemHand.enabled = true; 
        }
        itemHand.sprite = newSprite ?? GameAssets.i.placeholderSprite;

    }

    private void HideFromItemHand()
    {
        itemHand.sprite = GameAssets.i.placeholderSprite;
        itemHand.enabled = false;
        itemHandBG.enabled = false;
    }

    private void DisplayOnToolHand(Sprite newSprite)
    {
        if (!itemHand.isActiveAndEnabled)
        {
            toolHandBG.enabled = true;
            toolHand.enabled = true;
        }
        toolHand.sprite = newSprite ?? GameAssets.i.placeholderSprite;
    }

    private void HideFromToolHand()
    {
        toolHand.sprite = GameAssets.i.placeholderSprite;
        toolHand.enabled = false;
        toolHandBG.enabled = false;
    }
}
