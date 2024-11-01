using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace KimGhHun_Proto
{
    public class GameManager : MonoBehaviour
    {
        [Header("Core")]
        public bool isOver;
        public int score;
        public int maxLevel;

        [Header("Object Pooling")]
        public GameObject fruitPrefab;
        public Transform fruitGroup;
        public List<Fruit> fruitPool;
        public GameObject effectPrefab;
        public Transform effectGroup;
        public List<ParticleSystem> effectPool;

        [Range(1, 30)]
        public int poolSize = 10;
        public int poolIndex;
        public Fruit lastFruit;
        public Fruit nextFruit;

        [Header("Audio")]
        public AudioSource bgmPlayer;
        public AudioSource[] sfxPlayer;
        public AudioClip[] sfxClip;

        [Header("UI")]
        public TMP_Text scoreText;
        public TMP_Text maxScoreText;
        public GameObject endGroup;
        public GameObject startGroup;
        public TMP_Text subScoreText;
        [SerializeField] private Image _nextFruitImage;

        [Header("etc")]
        public GameObject line;
        public GameObject bottom;
        public GameObject eventTriggerObj;

        public Sprite[] fruitSprite;

        public KeyRing keyRing;


        public enum ESfx
        {
            LevelUp,
            Next,
            Attach,
            Button,
            Over
        }

        private int sfxIndex;

        private void Awake()
        {
            Application.targetFrameRate = 60;

            fruitPool = new List<Fruit>();
            effectPool = new List<ParticleSystem>();

            for (int i = 0; i < poolSize; i++)
            {
                MakeFruit();
            }

            if (!PlayerPrefs.HasKey("MaxScore"))
            {
                PlayerPrefs.SetInt("MaxScore", 0);
            }
            maxScoreText.text = PlayerPrefs.GetInt("MaxScore").ToString();

            // 미리 nextFruit를 설정
            nextFruit = GetFruit();
        }


        public void GameStart()
        {
            line.SetActive(true);
            //  bottom.SetActive(true);
            scoreText.gameObject.SetActive(true);
            maxScoreText.gameObject.SetActive(true);
            eventTriggerObj.SetActive(true);

            startGroup.SetActive(false);

            bgmPlayer.Play();
            SfxPlay(ESfx.Button);

            NextFruit();
          //  Invoke("NextFruit", 0.5f);
        }

        private Fruit MakeFruit()
        {
            // Generate Effect
            var effectObj = Instantiate(effectPrefab, effectGroup);
            effectObj.name = "Effect" + effectPool.Count;
            var effectPrticle = effectObj.GetComponent<ParticleSystem>();
            effectPool.Add(effectPrticle);

            // Generate Fruit
            var fruitObj = Instantiate(fruitPrefab, fruitGroup);
            fruitObj.name = "Fruit" + fruitPool.Count;
            var returnFruit = fruitObj.GetComponent<Fruit>();
            returnFruit.gameManager = this;
            returnFruit.effect = effectPrticle;
            fruitPool.Add(returnFruit);

            return returnFruit;
        }

        private Fruit GetFruit()
        {
            for (int i = 0; i < fruitPool.Count; i++)
            {
                poolIndex = (poolIndex + 1) % fruitPool.Count;
                if (!fruitPool[poolIndex].gameObject.activeSelf)
                {
                    Fruit fruit = fruitPool[poolIndex];
                    fruit.level = Random.Range(0, 2); // 레벨을 여기서 미리 설정
                    //UpdateNextFruitImage();
                    return fruit;
                }
            }

            Fruit newFruit = MakeFruit();
            newFruit.level = Random.Range(0, 2); // 새로 생성되는 경우에도 레벨을 설정
            //UpdateNextFruitImage();
            return newFruit;
        }

        // NextFruit 메서드에서 호출하기 전에 미리 이미지 업데이트        
        private void UpdateNextFruitImage()
        {
            if (nextFruit != null)
            {
                _nextFruitImage.sprite = fruitSprite[nextFruit.level];
                _nextFruitImage.gameObject.SetActive(true); // UI가 비활성화된 경우 활성화
            }
        }

        private void NextFruit()
        {
            if (isOver)
                return;

            lastFruit = nextFruit;  // 예약된 _nextFruit을 사용
            lastFruit.gameManager = this;
            lastFruit.gameObject.SetActive(true);
            lastFruit.UpdateFruitSprite();

            keyRing.connectedFruit = lastFruit;
            keyRing.SetConnect();
            lastFruit.GetComponent<Collider2D>().enabled = false;

            lastFruit.rb.mass = 1f + 1f * lastFruit.level;

            //추가
            lastFruit.rb.gravityScale = 2.5f + 0.5f * lastFruit.level;

            SfxPlay(ESfx.Next);

            // 다음 Fruit을 예약하고 UI에 업데이트
            nextFruit = GetFruit();
            UpdateNextFruitImage();
        }

        private IEnumerator WaitNext_co()
        {
            yield return new WaitForSeconds(0.5f);
            NextFruit();
        }

        public void TouchDown()
        {
            if (lastFruit == null) return;

            //lastFruit.Drag();
            keyRing.Drag();
        }

        public void TouchUp()
        {
            if (lastFruit == null) return;

            //lastFruit.Drop();
            keyRing.Drop();

            lastFruit = null;

            StartCoroutine(WaitNext_co());
        }

        public void GameOver()
        {
            if (isOver) return;
            Debug.Log("GameOver");
            isOver = true;

            eventTriggerObj.SetActive(false);

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

            yield return new WaitForSeconds(1f);

            int maxScore = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore"));
            PlayerPrefs.SetInt("MaxScore", maxScore);

            endGroup.SetActive(true);
            subScoreText.text = "Score : " + score.ToString();

            bgmPlayer.Stop();
            SfxPlay(ESfx.Over);
        }

        public void Reset()
        {
            SfxPlay(ESfx.Button);
            StartCoroutine(reset_co());
        }

        private IEnumerator reset_co()
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void SfxPlay(ESfx eSfx)
        {
            switch (eSfx)
            {
                case ESfx.LevelUp:
                    sfxPlayer[sfxIndex].clip = sfxClip[Random.Range(0, 3)];
                    break;
                case ESfx.Next:
                    sfxPlayer[sfxIndex].clip = sfxClip[3];
                    break;
                case ESfx.Attach:
                    sfxPlayer[sfxIndex].clip = sfxClip[4];
                    break;
                case ESfx.Button:
                    sfxPlayer[sfxIndex].clip = sfxClip[5];
                    break;
                case ESfx.Over:
                    sfxPlayer[sfxIndex].clip = sfxClip[6];
                    break;
            }

            sfxPlayer[sfxIndex].Play();
            sfxIndex = (sfxIndex + 1) % sfxPlayer.Length;
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                Application.Quit();
            }
        }

        private void LateUpdate()
        {
            scoreText.text = score.ToString();
        }
    }
}