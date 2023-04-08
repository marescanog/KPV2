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
    }

    private void OnDestroy()
    {
        Inventory.OnInventoryItemPickUp -= DisplayOnItemHand;
        Inventory.OnInventoryItemDrop -= HideFromItemHand;
    }

    private void DisplayOnItemHand(Sprite newSprite)
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
        Debug.Log("HideFromItemHand");
        itemHand.sprite = GameAssets.i.placeholderSprite;
        itemHand.enabled = false;
        itemHandBG.enabled = false;
    }
}