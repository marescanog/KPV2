using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Item")]
public class SO_Item : ScriptableObject
{
    // Read-only Data

    public int itemID;
    public string itemName;
    public ItemCategory itemCategory;   //    None,  Appliance,  Tool,   Food,  Material
    public Sprite baseSprite;
    public int maxStackLimit;
    public bool canBeContained;
    public float size;
    public bool canDecay;
    public Vector3 surfaceDisplayPosition;
}
