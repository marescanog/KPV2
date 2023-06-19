using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    public static Player PlayerInstance { get; private set; }

    public static event Action<Vector2> OnPlayerMove;
    public static event Action<int, int> OnPlayerEnterFloorTrigger;

    Vector2 _rawInput;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private SpriteRenderer _equippedItemSpriteRenderer;
    [SerializeField] private float _itemDropOffset;
    [SerializeField] private PlayerFaceDirectionUI _playerFaceDirectionUI; // temp
    [SerializeField] private Inventory _playerInventory;
    [SerializeField] private SpriteRenderer _itemOnHead;
    [SerializeField] private MaskedItemManager _itemOnHeadMaskRenderer;

    private Vector2 _playerFaceDirection = new Vector2(1,0);
    // private bool _inventoryButtonPickedUpActive = false;
    private bool _inventoryToggleOn = false;
    private PlayerInput _playerInput; // is referenced by something else


    private Transform _transform;

    private float _interactRad = 0.5f; // Interaction Radius
    private float _interactDist = 0.75f; // Interaction Radius
    private Vector3 _interactOffsetStartDist = new Vector3(0,0.5f,0);

    RaycastHit2D[] m_Results = new RaycastHit2D[5]; // to be deleted used to be on pickup items buffer storage

    public Vector3 PlayerPositionWithOffset { get { return _transform.position + _interactOffsetStartDist; } } // Offset the transform position by half players height since the position starts at the feet of the player
    PlayerActionControls playerActionControls;

    private bool pickupHoldPerformed = false;
    private bool toolUseHoldPerformed = false;
    private bool interactHoldPerformed = false;

    private Item _ItemInfront_cache = null;

    private void Awake()
    {
        PlayerInstance = this;
        _playerInput = GetComponent<PlayerInput>();
        _transform = GetComponent<Transform>();
        Inventory.OnInventoryItemPickUp += ShowItemOnHead;
        Inventory.OnInventoryItemDrop += HideItemOnHead;
        playerActionControls = new PlayerActionControls();
        playerActionControls.Player.Enable();
        PlayerActionControlSubscriptions();

    }

    private void PlayerActionControlSubscriptions()
    {
        playerActionControls.Player.Move.performed += Move_performed;
        playerActionControls.Player.Move.canceled += Move_cancelled;
        playerActionControls.Player.PickupHold.performed += PickupHold_performed;
        playerActionControls.Player.PickupHold.canceled += PickupHold_cancelled;
        playerActionControls.Player.EquipmentToggle.performed += EquipmentToggle_performed;
        playerActionControls.Player.ToolToggle.performed += ToolToggle_performed; // Cycle Through Tools
        playerActionControls.Player.GetTool.performed += GetTool_performed;  // Equip from item hand into Tool Hand
        playerActionControls.Player.UseTool.performed += UseTool_performed;
        playerActionControls.Player.UseTool.canceled += UseTool_canceled;
        playerActionControls.Player.Interact.performed += Interact_performed;
        playerActionControls.Player.Interact.canceled += Interact_canceled;
    }

    private void CleanUp()
    {
        Inventory.OnInventoryItemPickUp -= ShowItemOnHead;
        Inventory.OnInventoryItemDrop -= HideItemOnHead;
        playerActionControls.Player.Move.performed -= Move_performed;
        playerActionControls.Player.Move.canceled -= Move_cancelled;
        playerActionControls.Player.PickupHold.performed -= PickupHold_performed;
        playerActionControls.Player.PickupHold.canceled -= PickupHold_cancelled;
        playerActionControls.Player.EquipmentToggle.performed -= EquipmentToggle_performed;
        playerActionControls.Player.ToolToggle.performed -= ToolToggle_performed;
        playerActionControls.Player.GetTool.performed -= GetTool_performed;
        playerActionControls.Player.UseTool.performed -= UseTool_performed;
        playerActionControls.Player.UseTool.canceled -= UseTool_canceled;
        playerActionControls.Player.Interact.performed -= Interact_performed;
        playerActionControls.Player.Interact.canceled -= Interact_canceled;
    }

    void OnApplicationQuit()
    {
        // void OnDestroy(){}
        CleanUp();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    //  private void Move_performed(InputAction.CallbackContext context)
    private void Move_cancelled(InputAction.CallbackContext context)
    {
        _rawInput = Vector2.zero;
    }
    private void Move_performed(InputAction.CallbackContext context)
    {
        _rawInput = context.ReadValue<Vector2>();

        if (_rawInput != Vector2.zero)
        {
            if (_playerFaceDirectionUI != null && _rawInput != _playerFaceDirection)
            {
                _playerFaceDirectionUI.ShowArrowUI(_rawInput);
            }
            _playerFaceDirection = _rawInput;
        }

        OnPlayerMove?.Invoke(_playerFaceDirection);
    }
    private void PickupHold_performed(InputAction.CallbackContext context)
    {
        // Debug.Log("Player(PickupHold): PERFORMED");
        // Debug.Log("Player Pick Up Item Hold");
        pickupHoldPerformed = true;
        OnPickupHold();
    }
    private void PickupHold_cancelled(InputAction.CallbackContext context)
    {
        // Debug.Log("Player(PickupHold): CANCELLED");
        if (!pickupHoldPerformed)
        {
            // Debug.Log("Player Pick Up Item Tap");
            OnDropPickupItem();
        }
        pickupHoldPerformed = false;
    }
    private void EquipmentToggle_performed(InputAction.CallbackContext context)
    {
        // Debug.Log("Equipment Toggle Performed");
        OnEquipmentToggle();
    }
    private void ToolToggle_performed(InputAction.CallbackContext context) {
        // Cycle through tools
        OnToolToggle();
    }
    private void GetTool_performed(InputAction.CallbackContext context)
    {
        // Equip from item hand into Tool Hand
        OnGetTool();
    }
    private void UseTool_performed(InputAction.CallbackContext context){
        // Debug.Log("Player(UseTool): Hold");
        toolUseHoldPerformed = true;
        UseToolHold();
    }
    private void UseTool_canceled(InputAction.CallbackContext context){
        if (!toolUseHoldPerformed)
        {
            // Debug.Log("Player(UseTool): Tap");
            UseToolTap(); 
        } else
        {
            TimeTickManager.OnTick -= IncrementUse;
            Debug.Log("Incrementing Stopped");
        }
        _ItemInfront_cache = null;
        toolUseHoldPerformed = false;
    }

    private void Interact_performed(InputAction.CallbackContext context)
    {
        Debug.Log("Player(Interact): Hold");
        interactHoldPerformed = true;
        // OnPickupHold(); // Use Tool
    }
    private void Interact_canceled(InputAction.CallbackContext context)
    {
        if (!interactHoldPerformed)
        {
            Debug.Log("Player(Interact): Tap");
            // OnDropPickupItem(); // Open Menu
        }
        interactHoldPerformed = false;
    }



    private void ShowItemOnHead(Sprite newSprite, MaskRendererSpriteAssets maskRendererAssets)
    {
        if (!_itemOnHead.enabled)
        {
            _itemOnHead.enabled = true;
            
        }
        if (!_itemOnHeadMaskRenderer.enabled)
        {
            _itemOnHeadMaskRenderer.enabled = true;
        }
        _itemOnHead.sprite = newSprite;
        _itemOnHeadMaskRenderer.CopySpriteAssets(maskRendererAssets);
    }

    private void HideItemOnHead()
    {
        _itemOnHead.sprite = GameAssets.i.placeholderSprite;
        _itemOnHead.enabled = false;
        _itemOnHeadMaskRenderer.enabled = false;
        _itemOnHeadMaskRenderer.DisableEraseAllSpriteRenderers();
    }

    private void Update()
    {
        Vector3 delta = _rawInput;
        transform.position += delta * Time.deltaTime * _moveSpeed;
    }


    private void OnToggleInventory()
    {
        _inventoryToggleOn = !_inventoryToggleOn;

        Debug.Log($"Player(OnToggleInventory): The Inventory has been toggledON {_inventoryToggleOn}");

        _playerInput.SwitchCurrentActionMap(_inventoryToggleOn ? "OpenInventory" : "Player");
        /*
        if (_inventoryButtonPickedUpActive)
        {
            _inventoryButtonPickedUpActive = false;
        }
        */
        // _playerInput.SwitchCurrentActionMap(_inventory.GetInventoryToggleState() == false ? "OpenInventory" : "Player");
        //
    }


    private void UseToolTap()
    {
        _ItemInfront_cache = GetItemInFront();
        // Add Incrementally
        if (_ItemInfront_cache != null && _ItemInfront_cache.ItemCategory_My == ItemCategory.Appliance)
        {
            if (_playerInventory && _playerInventory?.ToolHandItem)
            {
                Debug.Log("Player(UseTool): Tap (Increment by X Amount)");
                _ItemInfront_cache.Appliance.Use(GameAssets.ToolDataReadOnly(_playerInventory.ToolHandItem.ItemID), 10);
            }
            else
            {
                Debug.Log("Player(OnUseTool): No Tool Equipped. Press hold to detach tool");
            }
        }
        else
        {
            Debug.Log("Player(UseTool): Player is NOT in front of an appliance");
        }
    }

    private void UseToolHold()
    {
        _ItemInfront_cache = GetItemInFront();
        if (_ItemInfront_cache != null && _ItemInfront_cache.ItemCategory_My == ItemCategory.Appliance)
        {
            // Start a Timer
            if (_playerInventory && _playerInventory?.ToolHandItem)
            {
                // use only if the tool is in the item's tool whitelist
                // if not then try equipping the item into tool hand
                    // Add an If for UseToolWithoutAppliance = // Trigger hold, if it has a trigger hold action use, then 
                    // it would trigger that (example, bowl pour or pour other)
                if (_ItemInfront_cache.IsInToolWhiteList(_playerInventory.ToolHandItem.ItemID))
                {
                    Debug.Log("Player(UseTool): Hold (Increment by Timer)");
                    TimeTickManager.OnTick += IncrementUse;
                } else
                {
                    Debug.Log("Player(OnUseTool): Not int tool whitelist");
                    TryDetachToolFromAppliance();
                }

            }
            else
            {
                Debug.Log("Player(OnUseTool): No Tool Equipped");
                TryDetachToolFromAppliance();
            }
        }
        else
        {
            // For now if bowl, it will just pour onto the ground and not counter
            Debug.Log("Player(OnUseTool): Player is NOT in front of an appliance");
            if (_playerInventory.ToolHandItem != null)
            {
                if (_playerInventory.ToolHandItem.UseToolWithoutAppliance(PlayerPositionWithOffset))
                {
                    Debug.Log("Player(OnUseTool): Player has a tool equipped which can be used without appliance");
                }
            } 
        }
    }

    private void IncrementUse()
    {
        Debug.Log("Incrementing...");
        if(_ItemInfront_cache != null)
        {
            _ItemInfront_cache.Appliance.Use(GameAssets.ToolDataReadOnly(_playerInventory.ToolHandItem.ItemID), 5);
        }
    }
    private void TryDetachToolFromAppliance()
    {
        // check if player item hand is empty
        if (_playerInventory.ItemHandItem == null)
        {
            MaskRendererSpriteAssets mrSa = null;
            Item detachedTool = _ItemInfront_cache.TryDetachToolFromAppliance(out mrSa);
            if (detachedTool != null)
            {
                Debug.Log($"Player(OnUseTool): Successfully Detached tool ({detachedTool?.Name_My}) from appliance");
                if (_playerInventory.PickupItem(detachedTool, false))
                {
                    Debug.Log($"Player(OnUseTool): Successfully Equipped tool ({detachedTool?.Name_My}) into item hand");
                }
            }
            else
            {
                Debug.Log("Player(OnUseTool): No tool attached to appliance.");
            }
        }
        else
        {
            Debug.Log($"Player(OnUseTool): Item hand full, cannot detach tool from appliance");
        }
    }



    // I don't know who is using this, delete later if I cant figure it out
    /*
    private void OnUseTool()
    {
        // Go Use Tool
        Debug.Log("Player(OnUseTool)");
        if (_playerInventory && _playerInventory?.ToolHandItem)
        {
           _playerInventory?.ToolHandItem?.UseTool(GetItemInFront());
        }
        else
        {
            Debug.Log("Player(OnUseTool): No Tool Equipped");
        }
    }
    */

    private void OnToolToggle()
    {
        Debug.Log("Player(OnToolToggle)");
        _playerInventory?.ToggleEquippedToolHand();
    }

    private void OnGetTool()
    {
        Debug.Log("Player(OnGetTool)");
        _playerInventory?.ToggleFetchTool();
    }
    private void OnSlotInteract()
    {

    }
    private Item GetItemInFront()
    {
        RaycastHit2D rayCastHit = Physics2D.Raycast(PlayerPositionWithOffset, _playerFaceDirection, _interactDist, LayerMask.GetMask("Item"));
        return rayCastHit ? rayCastHit.collider?.gameObject?.GetComponent<Item>() : null;
    }
    private Item GetItemInFront(Vector3 originPoint, Vector2 faceDirection, float reach)
    {
        RaycastHit2D rayCastHit = Physics2D.Raycast(originPoint, faceDirection, reach, LayerMask.GetMask("Item"));
        return rayCastHit ? rayCastHit.collider?.gameObject?.GetComponent<Item>() : null;
    }

    private void OnEquipmentToggle()
    {
        Debug.Log("Player(OnEquipmentToggle)");
        _playerInventory?.ToggleEquippedItemHand();
    }

    private void OnInteract()
    {
        // Change to fixture
        Debug.Log("Player (OnInteract)");
        // Debug.Log($"Itemhand is {_playerInventory?.ItemHandItem?.name??"NULL"}");
        // Debug.Log($"Name is {_playerInventory?.ItemHandItem?.Name_My ?? "NULL"}");

        /*
        Item itemInfront = GetItemInFront();

        if (itemInfront != null && itemInfront.Appliance != null && itemInfront.Appliance.isActiveAndEnabled)
        {
            Debug.Log("Interact with appliance");
        }
        */
    }

    private void OnPickupHold()
    {
        Debug.Log("Player (OnPickupHold)");
        if (_playerInventory)
        {
            _playerInventory?.PickupItem(GetItemInFront(), true);
        }
    }
    
    private void OnDropPickupItem()
    {
        Debug.Log("Player (OnDropPickupItem)");
        if (_playerInventory)
        {
            Item itemInFront = GetItemInFront();          

            if (_playerInventory.ItemHandItem)
            {   
                if (_playerInventory.DropItem(itemInFront, PlayerPositionWithOffset, _playerFaceDirection, _interactDist))
                {
                    // Update Player UI on Drop Success
                }
            }
            else
            {
                if (_playerInventory.PickupItem(itemInFront))
                {
                    // Update Player UI on Pickup Success
                    // Sprite associated with the drop/pickup -> from Item in front  
                    
                    if (itemInFront != null)
                    {
                        // _itemOnHeadMaskRenderer.CopySpriteAssets(itemInFront.GetMaskRendererSpriteAssets());
                    }
                    
                }
            }
        } 
    }

    private void DebugDrawRayCastLine()
    {
        Debug.DrawLine(_transform.position + _interactOffsetStartDist, _transform.position + _interactOffsetStartDist + new Vector3(_playerFaceDirection.x * _interactDist, _playerFaceDirection.y * _interactDist, 0), Color.white, 2.5f);
    }

    private void DebugDrawRadius()
    {
        Debug.DrawLine(_transform.position + _interactOffsetStartDist, _transform.position + _interactOffsetStartDist + new Vector3(_interactRad, 0, 0), Color.white, 2.5f);
        Debug.DrawLine(_transform.position + _interactOffsetStartDist, _transform.position + _interactOffsetStartDist + new Vector3(-_interactRad, 0, 0), Color.white, 2.5f);
        Debug.DrawLine(_transform.position + _interactOffsetStartDist, _transform.position + _interactOffsetStartDist + new Vector3(0, _interactRad, 0), Color.white, 2.5f);
        Debug.DrawLine(_transform.position + _interactOffsetStartDist, _transform.position + _interactOffsetStartDist + new Vector3(0, -_interactRad, 0), Color.white, 2.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FloorStepTrigger floorTigger = other?.gameObject?.GetComponent<FloorStepTrigger>();

        if (floorTigger)
        {
            if (OnPlayerEnterFloorTrigger != null) OnPlayerEnterFloorTrigger(floorTigger.MyFloorID,floorTigger.ChangeToFloorID);
        }

    }


}
