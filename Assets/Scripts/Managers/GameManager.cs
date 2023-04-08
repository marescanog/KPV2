using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class GameManager : MonoBehaviour
{

    public static GameAssets GO_GameAssets { get; private set; }
    public static ItemManager GO_ItemManager { get; private set; }

    // public Player playerPrefab { get; private set; }
    // public CinemachineVirtualCamera virtualCameraPrefab { get; private set; }


    private void Awake()
    {
        LoadAssetsPrefab();
        LoadItemManagerPrefab();
    }

    private void LoadAssetsPrefab()
    {
        // Load Prefab for Loading Game Assets
        GameAssets _gameAssetsPrefab = Resources.Load<GameAssets>("Prefabs/Managers/GameAssets");

        if (_gameAssetsPrefab)
        {
            GO_GameAssets = Instantiate(_gameAssetsPrefab);
            GO_GameAssets.name = "GameAssets";
            GO_GameAssets.GetComponent<Transform>()?.SetParent(gameObject?.transform?.parent?.transform);
        }
        else
        {
            Debug.LogError("GameManager(Awake): GameAssets cannot be found on Resources.Load");
        }
    }

    private void LoadItemManagerPrefab()
    {
        // Load Prefab for Loading Game Assets
        ItemManager _itemManager = Resources.Load<ItemManager>("Prefabs/Managers/ItemManager");

        if (_itemManager)
        {
            GO_ItemManager = Instantiate(_itemManager);
            GO_ItemManager.name = "ItemManager";
            GO_ItemManager.GetComponent<Transform>()?.SetParent(gameObject?.transform?.parent?.transform);
        }
        else
        {
            Debug.LogError("GameManager(Awake): ItemManager cannot be found on Resources.Load");
        }
    }




    /*
        private void LoadGameAssets()
        {
            playerPrefab = Resources.Load<Player>("Prefabs/Characters/Player");
            virtualCameraPrefab = Resources.Load<CinemachineVirtualCamera>("Prefabs/Characters/VC_Follow_player");

            player = Instantiate(_playerPrefab);
            player.name = "Player";
            VC_Camera_Player = Instantiate(_virtualCameraPrefab);
            VC_Camera_Player.name = "Player Virtual Camera";
            VC_Camera_Player.Follow = player.transform;
        }
    */

}
