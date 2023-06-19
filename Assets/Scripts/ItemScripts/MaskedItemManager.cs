using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskedItemManager : MonoBehaviour
{
    [Header("Renderers and Masks")]
    [SerializeField] private SpriteRenderer sprite_low;
    [SerializeField] private SpriteRenderer sprite_half;
    [SerializeField] private SpriteRenderer sprite_full;
    [SerializeField] private SpriteMask spriteMask;
    [Header("Transforms")]
    [SerializeField] private Transform transform_low;
    [SerializeField] private Transform transform_half;
    [SerializeField] private Transform transform_full;
    [SerializeField] private Transform transform_SpriteMask;
    [SerializeField] private Transform transform_MaskRendererObj;

    private bool[] enabledRenderers = new bool[3] { false, false, false};

    private void OnEnable()
    {
        if (!spriteMask.enabled)
        {
            spriteMask.enabled = true;
        }
        sprite_low.enabled = enabledRenderers[0];
        sprite_half.enabled = enabledRenderers[1];
        sprite_full.enabled = enabledRenderers[2];
    }

    private void OnDisable()
    {
        sprite_low.enabled = false;
        sprite_half.enabled = false;
        sprite_full.enabled = false;
        spriteMask.enabled = false;
    }

    public void DisableEraseAllSpriteRenderers()
    {
        sprite_low.enabled = enabledRenderers[0] = false;
        sprite_half.enabled = enabledRenderers[1] = false;
        sprite_full.enabled = enabledRenderers[2] = false;
    }

    public void AddSprite(Sprite sprite_My, float capacityPercentage) // float currentCapacity / int maxTotal
    {
        if (capacityPercentage < 0.33f)
        {
            if (!sprite_low.enabled)
            {
                sprite_low.enabled = enabledRenderers[0] = true;
                sprite_low.sprite = sprite_My;
            }
        } else if (capacityPercentage < 0.66f)
        {
            if (!sprite_half.enabled)
            {
                sprite_half.enabled = enabledRenderers[1] = true;
                sprite_half.sprite = sprite_My;
            }
        } else
        {
            if (!sprite_full.enabled)
            {
                sprite_full.enabled = enabledRenderers[2] = true;
                sprite_full.sprite = sprite_My;
            }
        }
    }

    // DELETE LATER BELOW
    public void RemoveSprite(Sprite sprite_My, float capacityPercentage)
    {
        Debug.Log($"MaskedItemManager(RemoveSprite): currentCapacity {capacityPercentage}");
        // Enable the mask renderer if it is not already enabled
        if (capacityPercentage >= 0.66f)
        {
            sprite_full.enabled = false;
            enabledRenderers[2] = false;
            Debug.Log("MaskedItemManager(RemoveSprite): remove sprite full");
        }
        else if (capacityPercentage >= 0.33f)
        {
            sprite_full.enabled = false;
            enabledRenderers[2] = false;
            sprite_half.enabled = false;
            enabledRenderers[1] = false;
            Debug.Log("MaskedItemManager(RemoveSprite): remove sprite half");
        }
        else
        {
            sprite_full.enabled = false;
            enabledRenderers[2] = false;
            sprite_half.enabled = false;
            enabledRenderers[1] = false;
            sprite_low.enabled = false;
            enabledRenderers[0] = false;
            Debug.Log("MaskedItemManager(RemoveSprite): remove sprite low");
        }
    }

    public void RemoveSprite(float capacityPercentage)
    {
        Debug.Log("MaskedItemManager(RemoveSprite): currentCapacity");
        if (capacityPercentage >= 99f)
        {
            sprite_full.enabled = false;
            enabledRenderers[2] = false;
            Debug.Log("MaskedItemManager(RemoveSprite): remove sprite full");
        }
        else if (capacityPercentage >= 0.66f)
        {
            sprite_full.enabled = false;
            enabledRenderers[2] = false;
            sprite_half.enabled = false;
            enabledRenderers[1] = false;
            Debug.Log("MaskedItemManager(RemoveSprite): remove sprite half");
        }
        else
        {
            sprite_full.enabled = false;
            enabledRenderers[2] = false;
            sprite_half.enabled = false;
            enabledRenderers[1] = false;
            sprite_low.enabled = false;
            enabledRenderers[0] = false;
            Debug.Log("MaskedItemManager(RemoveSprite): remove sprite low");
        }
    }

    internal void CopySpriteAssets(MaskRendererSpriteAssets newSprites)
    {
        if (newSprites != null)
        {
            sprite_low.sprite = newSprites.Sprite_low;
            sprite_half.sprite = newSprites.Sprite_half;
            sprite_full.sprite = newSprites.Sprite_full;
            sprite_low.enabled = enabledRenderers[0] = (newSprites.GetEnabledRenderersArray())[0];
            sprite_half.enabled = enabledRenderers[1] = (newSprites.GetEnabledRenderersArray())[1];
            sprite_full.enabled = enabledRenderers[2] = (newSprites.GetEnabledRenderersArray())[2];
        }
    }

    internal MaskRendererSpriteAssets GenerateMaskRendererSpriteAssets()
    {
        return new MaskRendererSpriteAssets(
            sprite_low.sprite, sprite_half.sprite, sprite_full.sprite,
            enabledRenderers[0], enabledRenderers[1], enabledRenderers[2]);
    }
}
