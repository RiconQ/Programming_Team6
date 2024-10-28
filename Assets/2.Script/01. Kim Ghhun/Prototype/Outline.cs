using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KimGhHun_Proto
{
    public class Outline : MonoBehaviour
    {
        public GameManager gameManager;


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<Fruit>(out var test))
            {
                gameManager.GameOver();
            }
        }
    }
}
