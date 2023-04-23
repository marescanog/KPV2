using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static Item _itemPrefab;
    private static Transform _myTransform;

    private void Awake()
    {
        _itemPrefab = Resources.Load<Item>("Prefabs/Item");
        _myTransform = gameObject.GetComponent<Transform>();
    }

    private void Start()
    {
        // Maybe Pool Items instead of instantiating (Later)
        GenerateItem(1, new Vector3(-1, -5, 0));
        GenerateItem(1, new Vector3(2, -5, 0));
        GenerateItem(2, new Vector3(-1, -6.5f, 0));
        GenerateItem(3, new Vector3(-1, -7.5f, 0));
        GenerateItem(6, new Vector3(-2, -7.5f, 0));
        GenerateItem(7, new Vector3(-3, -7.5f, 0));
        GenerateItem(8, new Vector3(-4, -7.5f, 0));
        GenerateItem(5, new Vector3(2, -6.5f, 0));
        GenerateItem(9, new Vector3(3, -6.5f, 0));
    }

    public static Item GenerateItem(int itemID, Vector3 position)
    {
        Item newItem = Instantiate(_itemPrefab);
        newItem.name = "NewItem";
        newItem.GetComponent<Transform>()?.SetParent(_myTransform);
        newItem.SetItem(position, itemID);
        return newItem;
    }

}
