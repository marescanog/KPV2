using UnityEngine;

public class MaskRendererSpriteAssets 
{
    // [Header("Sprite Data")]
    public Sprite Sprite_low { get; private set; }
    public Sprite Sprite_half { get; private set; }
    public Sprite Sprite_full { get; private set; }
    /*
    // [Header("Transform Data")]
    public Transform Transform_low { get; private set; }
    public Transform Transform_half { get; private set; }
    public Transform Transform_full { get; private set; }
    public Transform Transform_SpriteMask { get; private set; }
    public Transform Transform_MaskRendererObj { get; private set; }
    */
    private bool[] _enabledRenderers = new bool[3] { false, false, false };
    public MaskRendererSpriteAssets(MaskRendererSpriteAssets newAsset)
    {
        Sprite_low = newAsset.Sprite_low;
        Sprite_half = newAsset.Sprite_half;
        Sprite_full = newAsset.Sprite_full;
        int i = -1;
        foreach (bool flag in newAsset.GetEnabledRenderersArray())
        {
            i++;
            (newAsset.GetEnabledRenderersArray())[i] = flag;
        }
    }
    public MaskRendererSpriteAssets(Sprite spLow, Sprite spHalf, Sprite spFull, bool spLowFlag, bool spHalfFlag, bool spFullFlag)
    {
        Sprite_low = spLow;
        Sprite_half = spHalf;
        Sprite_full = spFull;
        _enabledRenderers[0] = spLowFlag;
        _enabledRenderers[1] = spHalfFlag;
        _enabledRenderers[2] = spFullFlag;
    }

    public bool[] GetEnabledRenderersArray()
    {
        return _enabledRenderers;
    }
}
