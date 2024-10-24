using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KimGhHun_Proto
{
    public class GameManager : MonoBehaviour
    {
        public Fruit lastFruit;
        public GameObject fruitPrefab;
        public Transform fruitGroup;

        public GameObject effectPrefab;
        public Transform effectGroup;

        public int score;
        public int maxLevel;
        public bool isOver;

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            NextFruit();
        }

        private Fruit GetFruit()
        {
            // Generate Effect
            var effectObj = Instantiate(effectPrefab, effectGroup);
            var effectPrticle = effectObj.GetComponent<ParticleSystem>();

            // Generate Fruit
            var fruitObj = Instantiate(fruitPrefab, fruitGroup);
            var returnFruit = fruitObj.GetComponent<Fruit>();
            returnFruit.effect = effectPrticle;

            return returnFruit;
        }

        private void NextFruit()
        {
            if (isOver) return;
            var newFruit = GetFruit();
            lastFruit = newFruit;
            lastFruit.gameManager = this;
            lastFruit.level = Random.Range(0, maxLevel);
            lastFruit.gameObject.SetActive(true);
        }
        
        private IEnumerator WaitNext_co()
        {
            yield return new WaitForSeconds(0.5f);
            NextFruit();
        }

        public void TouchDown()
        {
            if (lastFruit == null) return;

            lastFruit.Drag();
        }

        public void TouchUp()
        {
            if (lastFruit == null) return;

            lastFruit.Drop();
            lastFruit = null;

            StartCoroutine(WaitNext_co());
        }

        public void GameOver()
        {
            if (isOver) return;
            Debug.Log("GameOver");
            isOver = true;

            StartCoroutine(GameOverRoutine());
        }

        private IEnumerator GameOverRoutine()
        {
            // Get All Fruit
            Fruit[] Fruits = FindObjectsOfType<Fruit>();

            foreach (var fruit in Fruits)
            {
                fruit.rb.simulated = false;
            }

            // Delete All Fruit
            foreach (Fruit fruit in Fruits)
            {
                fruit.Hide(Vector3.up * 100);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
