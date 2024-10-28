using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KimGhHun_Proto
{
    public class Preline : MonoBehaviour
    {
        public GameObject line;
        private void OnTriggerStay2D(Collider2D collision)
        {
            line.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            line.SetActive(false);
        }

    }
}
