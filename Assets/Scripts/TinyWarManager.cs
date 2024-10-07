using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TinyWar
{
    public class TinyWarManager : MonoBehaviour
    {

        [SerializeField] private UIManager uIManager;
        [SerializeField] private List<PlayerController> activePlayers = new List<PlayerController>();

        public GameState gameState;

        public static TinyWarManager Instance;

        public List<PlayerController> ActivePlayers
        {
            get { return activePlayers; }
        }

        public Action<PlayerController> PlayerSpawnedEvent;

        public Transform enemySpawnPoint;


        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            gameState = GameState.UnPaused;
            Time.timeScale = 1;
        }




        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
                ToggleGameState();

            if (Input.GetKeyDown(KeyCode.E))
                SpawnEnemy();

        }

        private void ToggleGameState()
        {
            if (gameState == GameState.UnPaused)
            {
                Time.timeScale = 0;
                gameState = GameState.Paused;
            }
            else
            {
                Time.timeScale = 1;
                gameState = GameState.UnPaused;
            }
        }

        public void SpawnEnemy()
        {
            GameObject enemyPrefab = Resources.Load<GameObject>("RedUnit");
            SpawnPlayerInGround(enemyPrefab, enemySpawnPoint.position);
        }

        public GameObject SpawnPlayerInGround(GameObject prefab, Vector3 position)
        {
            foreach (Collider coll in Physics.OverlapSphere(position, .3f))
                if (coll.transform.CompareTag("Player"))
                    return null;
            PlayerController playerController = Instantiate(prefab, position, Quaternion.identity).GetComponent<PlayerController>();
            activePlayers.Add(playerController);
            playerController.playerName = prefab.name + "_" + UnityEngine.Random.Range(1000, 10000);
            playerController.transform.name = playerController.playerName;
            playerController.playerDied += RemovePlayerFromActiveList;
            uIManager.EventOccured(playerController.TeamType, playerController.playerName, playerController.playerName + " spawn at point...");
            return playerController.gameObject;
        }



        public Transform SpawnHealthBarForPlayer(string playerName)
        {
            return uIManager.SpawnHealthBarForPlayer(playerName);
        }

        public Canvas MainCanvas()
        {
            return uIManager.MainCanvas;
        }

        private void RemovePlayerFromActiveList(PlayerController diedPlayer, PlayerController killedPlayer)
        {
            activePlayers.Remove(diedPlayer);
            uIManager.EventOccured(diedPlayer.TeamType, diedPlayer.playerName, diedPlayer.playerName + " killed by " + killedPlayer.playerName);
            uIManager.EventOccured(killedPlayer.TeamType, killedPlayer.TeamType.ToString(), killedPlayer.TeamType.ToString() + " Updated by 10...");
        }
    }
    public enum GameState { Paused, UnPaused }
}
