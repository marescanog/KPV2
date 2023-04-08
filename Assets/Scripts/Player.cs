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

    private Vector2 _playerFaceDirection = new Vector2(1,0);
    private bool _inventoryButtonPickedUpActive = false;
    private PlayerInput _playerInput; // is referenced by something else

    private Transform _transform;

    private float _interactRad = 0.5f; // Interaction Radius
    private float _interactDist = 0.75f; // Interaction Radius
    private Vector3 _interactOffsetStartDist = new Vector3(0,0.5f,0);

    RaycastHit2D[] m_Results = new RaycastHit2D[5]; // to be deleted used to be on pickup items buffer storage

    public Vector3 PlayerPositionWithOffset { get { return _transform.position + _interactOffsetStartDist; } } // Offset the transform position by half players height since the position starts at the feet of the player

    private void Awake()
    {
        PlayerInstance = this;
        _playerInput = GetComponent<PlayerInput>();
        _transform = GetComponent<Transform>();
        Inventory.OnInventoryItemPickUp += ShowItemOnHead;
        Inventory.OnInventoryItemDrop += HideItemOnHead;
    }

    private void ShowItemOnHead(Sprite newSprite)
    {
        if (!_itemOnHead.enabled)
        {
            _itemOnHead.enabled = true;
        }
        _itemOnHead.sprite = newSprite;
    }
    private void HideItemOnHead()
    {
        _itemOnHead.sprite = GameAssets.i.placeholderSprite;
        _itemOnHead.enabled = false;
    }

    private void Update()
    {
        Vector3 delta = _rawInput;
        transform.position += delta * Time.deltaTime * _moveSpeed;
    }

    // Called by Input System
    private void OnMove(InputValue value) 
    {
        _rawInput = value.Get<Vector2>();
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

    private void OnToggleInventory()
    {

        if (_inventoryButtonPickedUpActive)
        {
            _inventoryButtonPickedUpActive = false;
        }

        // _playerInput.SwitchCurrentActionMap(_inventory.GetInventoryToggleState() == false ? "OpenInventory" : "Player");
        //
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
    
    private void OnInteract()
    {
        // Change to fixture
        Debug.Log("Player (OnInteract)");
        // Debug.Log($"Itemhand is {_playerInventory.ItemHandItem?.name??"NULL"}");
        Item itemInfront = GetItemInFront();

        if (itemInfront != null && itemInfront.Appliance != null && itemInfront.Appliance.isActiveAndEnabled)
        {
            Debug.Log("Interact with appliance");
        }
    }

    private void OnPickupHold()
    {
        Debug.Log("Player (OnPickupHold)");
        // PickupAppliance(); // TODO Erase Function
        if (_playerInventory)
        {
            _playerInventory.PickupItem(GetItemInFront(), true);
        }
    }

    // TODO (CLEANUP): ERASE PickupAppliance FUNCTION (Not used anywhere else just used as reference)
    private void PickupAppliance()
    {
        // Offset the transform position by half players height since the position starts at the feet of the player
        RaycastHit2D rayCastHit = Physics2D.Raycast(_transform.position + _interactOffsetStartDist, _playerFaceDirection, _interactDist, LayerMask.GetMask("Item"));
        // DebugDrawRayCastLine();
        Item item = rayCastHit ? rayCastHit.collider.gameObject.GetComponent<Item>() : null;

        if (item)
        {
            // Debug.Log("Player(OnInteract) raycasthit: " + rayCastHit.collider.name);
            Debug.Log("Player(OnPickupHold) raycasthit: " + (rayCastHit.collider?.gameObject?.name ?? rayCastHit.collider?.name));

            if (item && item?.Appliance?.isActiveAndEnabled == true)
            {
                // Add appliance function TryEquipAppliance -> Make sure appliance is off? or if appliance can be equipped while running, write in ondisable code?

                Debug.Log("Player(OnPickupHold) : You are picking up an appliance");

                _playerInventory.TryEquipItem(item); // returns a bool true if equipped and false if not
            }

        }
    }
    
    private void OnDropPickupItem()
    {
        Debug.Log("Player (OnDropPickupItem)");
        // PickupDrop(); // TODO Erase Function
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
                }
            }
        } 
    }

    // TODO (CLEANUP): ERASE PICKUP DROP FUNCTION (Not used anywhere else just used as reference)
    private void PickupDrop()
    {
        // Offset the transform position by half players height since the position starts at the feet of the player
        Vector3 playerPosition = _transform.position + _interactOffsetStartDist;

        RaycastHit2D rayCastHit = Physics2D.Raycast(playerPosition, _playerFaceDirection, _interactDist, LayerMask.GetMask("Item"));
        // DebugDrawRayCastLine();
        Item itemInFront = rayCastHit ? rayCastHit.collider?.gameObject?.GetComponent<Item>() : null;

        // 1 DROP ITEM
        if (_playerInventory && _playerInventory?.ItemHandItem)
        {
            Debug.Log("Player(OnDropPickupItem): 1 - player has item equipped");
            if (itemInFront)
            {
                Debug.Log("Player(OnDropPickupItem): 1.1 - item is in front of player");
                if (itemInFront.Appliance.isActiveAndEnabled)
                {
                    /*
                        if appliance has tool attached (bowl or whatever)
                                if players current item is food
                                    add to appliance (bowl or whatever)
                                if players current item is material
                                    if fuel is allowed to be added to appliance
                                        add fuel
                        if appliance has no tool attached
                                if players current item is tool
                                    if tool is allowed as an attachment to the appliance
                                        Attach tool
                                if players current item is food
                                    if appliance allows food to be added straight
                                        add food 
                    */
                }
                else if (itemInFront.Tool.isActiveAndEnabled)
                {

                }
            }
            else
            {
                Debug.Log("Player(OnDropPickupItem): 1.2 - item is not in front of player");
                // check if there is a wall or something else in front of the player where we cant put item
                int hits = Physics2D.RaycastNonAlloc(playerPosition, _playerFaceDirection, m_Results);
                Debug.Log($"RaycastNonAlloc hits {hits}");
                if (hits <= 1)
                {
                    Debug.Log("Drop Item");
                }
            }
        }
        // 2 PICKUP ITEM
        else
        {
            Debug.Log("Player(OnDropPickupItem): 2 - Player has no item equipped");
            /* //pickup code
            if (item)
            {
                // Debug.Log("Player(OnDropPickupItem) raycasthit: " + rayCastHit.collider.name);
                Debug.Log("Player(OnDropPickupItem) raycasthit: " + rayCastHit.collider.gameObject.name);

            }
            //         _playerInventory.UnequipItem();
            */
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
