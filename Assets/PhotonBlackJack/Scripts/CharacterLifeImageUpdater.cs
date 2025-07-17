using UnityEngine;
using System;

[RequireComponent(typeof(BlackJackPlayer))]
public class CharacterLifeImageUpdater : MonoBehaviour
{
    [SerializeField] private string characterName; // 인스펙터에서 수동으로 입력할 캐릭터 이름
    [SerializeField] private SpriteRenderer targetSpriteRenderer; // 이미지를 표시할 SpriteRenderer

    [Header("Game Over Settings")]
    [SerializeField] private Vector3 gameOverPosition; // 게임 오버 시 위치
    [SerializeField] private Vector3 gameOverScale;    // 게임 오버 시 크기
    [SerializeField] private string startSceneName = "Start"; // 시작 화면 씬 이름
    [SerializeField] private GameObject canvasToDeactivate; // 비활성화할 Canvas 오브젝트

    private BlackJackPlayer myBlackJackPlayer;

    private void Awake()
    {
        myBlackJackPlayer = GetComponent<BlackJackPlayer>();

        if (myBlackJackPlayer != null)
        {
            myBlackJackPlayer.OnLifeChanged += UpdateCharacterImage; // 라이프 변경 이벤트 구독
        }
        else
        {
            Debug.LogError("BlackJackPlayer component not found on this GameObject.");
        }
    }

    private void Start()
    {
        // 초기 이미지 설정
        if (myBlackJackPlayer != null)
        {
            UpdateCharacterImage(myBlackJackPlayer.Life);
        }
    }

    private void OnDestroy()
    {
        // 오브젝트 파괴 시 이벤트 구독 해제
        if (myBlackJackPlayer != null)
        {
            myBlackJackPlayer.OnLifeChanged -= UpdateCharacterImage;
        }
    }

    private void UpdateCharacterImage(int currentLife)
    {
        if (targetSpriteRenderer == null) return; // 지정된 SpriteRenderer가 없으면 리턴

        if (currentLife <= 0) // 체력이 0 이하가 되면 게임 오버 처리
        {
            // 위치 및 크기 변경
            targetSpriteRenderer.transform.position = gameOverPosition;
            targetSpriteRenderer.transform.localScale = gameOverScale;
            targetSpriteRenderer.sortingOrder = 100; // Order in Layer를 100으로 설정

            // SpriteRenderer 오브젝트만 남기고 다른 컴포넌트 비활성화
            MonoBehaviour[] components = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                if (component != this && component != targetSpriteRenderer) // 이 스크립트와 SpriteRenderer는 제외
                {
                    component.enabled = false;
                }
            }
            // Collider가 있다면 활성화하여 클릭 가능하게 함
            Collider2D col = targetSpriteRenderer.gameObject.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;
            else
            {
                // Collider가 없으면 경고 메시지 출력 (클릭 이벤트 처리를 위해 필요)
                Debug.LogWarning("No Collider2D found on the target SpriteRenderer GameObject. Click event will not work.");
            }

            // GameOverClickable 스크립트 추가 및 설정
            GameOverClickable clickable = targetSpriteRenderer.gameObject.AddComponent<GameOverClickable>();
            clickable.SetSceneToLoad(startSceneName);

            // 지정된 Canvas 오브젝트 비활성화
            if (canvasToDeactivate != null)
            {
                canvasToDeactivate.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Canvas to deactivate is not assigned in the Inspector.");
            }
        }

        if (!string.IsNullOrEmpty(characterName))
        {
            // Resources 폴더 내의 Sprites/{characterName} 폴더에서 {currentLife} 형식의 스프라이트를 로드
            Sprite loadedSprite = Resources.Load<Sprite>($"Sprites/{characterName}/{currentLife}");
            if (loadedSprite != null)
            {
                targetSpriteRenderer.sprite = loadedSprite;
            }
            else
            {
                Debug.LogWarning($"Character sprite not found in Resources/Sprites/{characterName}/{currentLife}");
            }
        }
        else
        {
            Debug.LogWarning("Character name is not set in the Inspector.");
        }
    }
}