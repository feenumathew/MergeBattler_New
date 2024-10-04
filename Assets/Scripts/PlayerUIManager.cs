using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TinyWar
{
  
    public class PlayerUIManager : MonoBehaviour
    {
        private PlayerController playerController;
        private Canvas canvas;
        private Transform healthBar;
        private Coroutine healthDisableCoro;

        void Start()
        {
            playerController = GetComponent<PlayerController>();
            canvas = TinyWarManager.Instance.MainCanvas();
            healthBar = TinyWarManager.Instance.SpawnHealthBarForPlayer(playerController.playerName);
            healthBar.gameObject.SetActive(false);
        }

        public void UpdateHealthBar(float playerHealth)
        {
           
            Transform fillRect = healthBar.Find("Fill");
            fillRect.GetComponent<RectTransform>().sizeDelta = new Vector2(140 * (playerHealth / 100f), 600);
            Text text = healthBar.Find("Text").GetComponent<Text>();
            text.text = playerHealth.ToString();
            healthBar.gameObject.SetActive(true);
            if (healthDisableCoro != null)
                StopCoroutine(healthDisableCoro);
            healthDisableCoro = StartCoroutine(DisableHealthAfterDelay());
        }

        IEnumerator DisableHealthAfterDelay()
        {
            yield return new WaitForSeconds(3);
            healthBar.gameObject.SetActive(false);
            healthDisableCoro = null;
        }

        // Update is called once per frame
        void Update()
        {
            if (healthBar != null)
            {
                if (healthBar.gameObject.activeInHierarchy)
                {
                    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
                    Vector2 viewportPos = Camera.main.WorldToViewportPoint(playerController.transform.position + 2 * Vector3.up);
                    Vector2 screenPos = new Vector2((viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
                                                    (viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
                    healthBar.GetComponent<RectTransform>().anchoredPosition = screenPos;
                }
            }
        }

        public void DisableHealthBar()
        {
            healthBar.gameObject.SetActive(false);
        }

        public void DestroyHealthBar()
        {
            Destroy(healthBar.gameObject);
            StopAllCoroutines();
            healthDisableCoro = null;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            healthDisableCoro = null;
        }

    }
}
