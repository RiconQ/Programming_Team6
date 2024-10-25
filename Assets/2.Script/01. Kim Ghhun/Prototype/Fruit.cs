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
        private Animator animator;
        CircleCollider2D col;
        SpriteRenderer sr;

        float deadTime;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            col = GetComponent<CircleCollider2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            animator.SetInteger("Level", level);
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

            rb.simulated = false;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            col.enabled = true;
        }

        private void Update()
        {
            if (isDrag)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float leftBorader = -3.25f;
                float rightBorader = 3.25f;
                mousePos.y = 0.91f;
                mousePos.z = 0f;
                mousePos.x = Mathf.Clamp(mousePos.x, leftBorader, rightBorader);
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
            rb.simulated = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            StartCoroutine(Attach_co());
        }

        private IEnumerator Attach_co()
        {
            if (isAttach) yield break;

            isAttach = true;
            gameManager.SfxPlay(GameManager.ESfx.Attach);
            yield return new WaitForSeconds(0.2f);
            isAttach = false;
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
                        //other hide
                        other.Hide(transform.position);

                        //me levelup
                        LevelUp();
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

        private void LevelUp()
        {
            isMerge = true;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0;

            StartCoroutine(LevelUp_co());
        }

        private IEnumerator LevelUp_co()
        {
            yield return new WaitForSeconds(0.2f);

            animator.SetInteger("Level", level + 1);
            EffectPlay();
            gameManager.SfxPlay(GameManager.ESfx.LevelUp);

            yield return new WaitForSeconds(0.3f);

            level++;

            gameManager.maxLevel = Mathf.Max(level, gameManager.maxLevel);

            isMerge = false;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Finish"))
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
