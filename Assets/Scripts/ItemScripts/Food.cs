using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public int ItemID { get; private set; }
    public List<Flavor> FlavorData { get; private set; }
    public float Amount { get; private set; }
    public float Temperature { get; private set; }
    public ChoppedState ChoppedState_curr { get; private set; }
    public SO_Food Default_Food_Data { get { return GameAssets.FoodDataReadOnly(ItemID); } }

    public float ProcessProgress { get; private set; }




    public bool IncrementProgress (float incrementVal)
    {
        ProcessProgress += incrementVal;
        Debug.Log("Food(IncrementProgress): ProcessProgress is "+ProcessProgress);
        if (ProcessProgress >= 100)
        {
            ProcessProgress = 0;
            return true;
        }
        return false;
    }

    public void SetFoodData(SO_Food foodData, int itemID)
    {
        if (foodData != null)
        {

        }
        ItemID = itemID;
        ChoppedState_curr = ChoppedState.None;
        ProcessProgress = 0;
    }

    // ??? Not sure if we'll need this below
    public void SetFoodData(Food foodData)
    {
        if (foodData != null)
        {
            ItemID = foodData.ItemID;
            FlavorData = foodData.FlavorData;
            Amount = foodData.Amount;
            Temperature = foodData.Temperature;
            ProcessProgress = foodData.ProcessProgress;
        }
    }

    public Sprite GetSprite()
    {
        Sprite retVal = GameAssets.i.placeholderSprite;
        if (ChoppedState_curr == ChoppedState.None)
        {
            retVal = GameAssets.ItemDataReadOnly(ItemID)?.baseSprite ?? GameAssets.i.placeholderSprite;
        } else
        {
            Sprite choppedSprite = GameAssets.FoodDataReadOnly(ItemID)?.choppedSprite[(int)ChoppedState_curr];

            retVal = choppedSprite == null ? GameAssets.i.placeholderSprite : choppedSprite;
        }
        return retVal;
    }

    public void ChopItem(Item item, Item itemContainer)
    {
        Debug.Log("Food(ChopItem): Chopping Item and Updating Sprite");
        if(Default_Food_Data!= null && ChoppedState_curr != Default_Food_Data.maxChoppedState)
        {
            switch (ChoppedState_curr)
            {
                case ChoppedState.None:
                    ChoppedState_curr = ChoppedState.Sliced;
                    break;
                case ChoppedState.Sliced:
                    ChoppedState_curr = ChoppedState.Julienne;
                    break;
                case ChoppedState.Julienne:
                    ChoppedState_curr = ChoppedState.Diced;
                    break;
                case ChoppedState.Diced:
                    ChoppedState_curr = ChoppedState.Minced;
                    break;
                case ChoppedState.Minced:
                    ChoppedState_curr = ChoppedState.Mashed;
                    break;
            }
            if (item != null)
            {
                item.UpdateSprite(GetSprite());
            }
            if (itemContainer != null)
            {
                itemContainer.SetSurfaceSprite(GetSprite());
            }
        }
    }
}
