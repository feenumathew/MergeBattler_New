using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinyWar
{
  
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private int eventHistoryCount = 15;

        private Transform healthBars;


        private void Start()
        {
            healthBars = Helper.FindDeepChild(canvas.transform, "HealthBars");

        }

        public Canvas MainCanvas
        {
            get { return canvas; }
        }

        public Transform SpawnHealthBarForPlayer(string playerName)
        {
            Transform healthBar =  Instantiate(Resources.Load<GameObject>("HealthBar"), canvas.transform.Find("HealthBars")).transform;
            healthBar.name = "HB_" + playerName;
            return healthBar;
        }


        public void EventOccured(Team teamType,string title,string body)
        {
            Debug.Log($"teamtype {teamType} title {title} body {body}");

        }


    }

    
}
