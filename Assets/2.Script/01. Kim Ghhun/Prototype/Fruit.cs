using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KimGhHun_Proto
{
    public class Fruit : MonoBehaviour
    {
        public GameManager gameManager;
        public ParticleSystem effect;

        public int level;
        public bool isDrag;
        public bool isMerge;
        public bool isAttach;

        public Rigidbody2D rb;
        private CircleCollider2D col;
        private SpriteRenderer sr;

        float deadTime;

        private void Awake()
        {
            col = GetComponent<CircleCollider2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            // 기본 초기화 설정
            rb.simulated = true;
            rb.gravityScale = 2.0f;
            col.enabled = true;
            deadTime = 0f;
            sr.color = Color.white;
        }

        public void UpdateFruitSprite()
        {
            Debug.Log($"Level {level}");
            sr.sprite = gameManager.fruitSprite[level];
            transform.localScale = new Vector3(1f + 0.5f * level, 1f + 0.5f * level, 1);
            Debug.Log($"scale {transform.localScale}");
        }

        private void OnDisable()
        {
            level = 0;
            isDrag = false;
            isMerge = false;
            isAttach = false;

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.zero;

            rb.simulated = true;
            //rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            col.enabled = true;
        }

        private void Update()
        {
            if (isDrag)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float leftBorder = -3.25f;
                float rightBorder = 3.25f;
                mousePos.y = 7.5f;
                mousePos.z = 0f;
                mousePos.x = Mathf.Clamp(mousePos.x, leftBorder, rightBorder);
                transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);
            }
        }

        public void Drag()
        {
            isDrag = true;
        }

        public void Drop()
        {
            isDrag = false;
            rb.simulated = true; // Drop 시 물리 효과 재적용
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.transform.CompareTag("Fruit"))
            {
                Fruit other = collision.transform.GetComponent<Fruit>();

                if (level == other.level && !isMerge && !other.isMerge && level < 7)
                {
                    float meX = transform.position.x;
                    float meY = transform.position.y;
                    float otherX = other.transform.position.x;
                    float otherY = other.transform.position.y;

                    if (meY < otherY || (meY == otherY && meX > otherX))
                    {
                        Vector3 targetPos = new Vector3((meX + otherX) / 2, (meY + otherY) / 2, 0);
                        other.Hide(targetPos);
                        LevelUp(targetPos);
                    }
                }
            }
        }

        public void Hide(Vector3 targetPos)
        {
            isMerge = true;

            rb.simulated = false;
            col.enabled = false;

            if (targetPos == Vector3.up * 100)
            {
                EffectPlay();
            }

            StartCoroutine(Hide_co(targetPos));
        }

        private IEnumerator Hide_co(Vector3 targetPos)
        {
            int frameCount = 0;

            while (frameCount < 20)
            {
                frameCount++;
                if (targetPos != Vector3.up * 100)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
                }
                else
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
                }
                yield return null;
            }

            gameManager.score += (int)Mathf.Pow(2, level);

            isMerge = false;
            gameObject.SetActive(false);
        }

        private void LevelUp(Vector3 targetPos)
        {
            isMerge = true;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0;

            rb.simulated = false;
            col.enabled = false;

            StartCoroutine(LevelUp_co(targetPos));
        }

        private IEnumerator LevelUp_co(Vector3 targetPos)
        {
            int frameCount = 0;

            while (frameCount < 20)
            {
                frameCount++;
                if (targetPos != Vector3.up * 100)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
                }
                else
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
                }
                yield return null;
            }

            rb.simulated = true;
            col.enabled = true;
            EffectPlay();
            gameManager.SfxPlay(GameManager.ESfx.LevelUp);

            level++;

            UpdateFruitSprite();

            rb.mass = 1f + 0.5f * level;

            gameManager.maxLevel = Mathf.Max(level, gameManager.maxLevel);

            isMerge = false;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Finish") && !this.Equals(gameManager.lastFruit) && transform.position.y >= collision.transform.position.y)
            {
                deadTime += Time.deltaTime;
                if (deadTime > 2f)
                {
                    sr.color = Color.red;
                }
                if (deadTime > 5f)
                {
                    gameManager.GameOver();
                }
            }
            else if (collision.transform.CompareTag("Finish") && !this.Equals(gameManager.lastFruit) && transform.position.y < collision.transform.position.y)
            {
                deadTime = 0f;
                sr.color = Color.white;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Finish"))
            {
                deadTime = 0f;
                sr.color = Color.white;
            }
        }

        private void EffectPlay()
        {
            effect.transform.position = transform.position;
            effect.transform.localScale = transform.localScale;
            effect.Play();
        }
    }
}
