using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 추가
using System; // Action을 사용하기 위해 추가

public class BlackJackPlayer : MonoBehaviour
{
    public event Action<int> OnLifeChanged; // 라이프 변경 이벤트 추가

    private bool m_isBust = false;
    private bool m_isBlackJack = false;
    private Hand m_hand;
    [SerializeField] private int m_maxLife = 3; // 라이프 최대치 변수 추가
    private int m_life; // 현재 라이프 변수 (초기화는 Awake에서)
    public bool IsStandDisabled { get; set; } // 스탠드 금지 상태 변수 추가

    private List<ItemType> m_inventory = new List<ItemType>(); // 아이템 인벤토리
    private const int MAX_INVENTORY_SIZE = 3; // 인벤토리 최대 크기

    public bool IsTurnOn;
    public Player Player;
    public string CharacterType; // 캐릭터 타입 식별을 위한 필드 추가

    public bool IsBust { get { return m_isBust; } }
    public bool IsBlackJack { get { return m_isBlackJack; } }
    public Hand Hand { get { return m_hand; } }
    public int Life { get { return m_life; } } // 라이프 속성 추가됨
    public int MaxLife { get { return m_maxLife; } } // 라이프 최대치 속성 추가
    public IReadOnlyList<ItemType> Inventory { get { return m_inventory; } } // 인벤토리 읽기 전용 접근자

    private void Awake()
    {
        m_hand = new Hand();
        m_life = m_maxLife; // Awake에서 현재 라이프를 최대치로 초기화
        OnLifeChanged?.Invoke(m_life); // 초기 라이프 설정 시 이벤트 호출
    }

    public void PlayerReset()
    {
        Debug.Log($"[BlackJackPlayer] PlayerReset called for {name}. Inventory will NOT be cleared.");
        if (Hand == null)
            m_hand = new Hand();

        m_isBlackJack = false;
        m_isBust = false;
        IsTurnOn = true;
        Hand.Clear();
        IsStandDisabled = false; // 라운드 시작 시 스탠드 금지 상태 초기화
        OnLifeChanged?.Invoke(m_life); // 라이프 초기화 시 이벤트 호출
    }

    public void BustCheck()
    {
        m_isBust = Hand.IsBust();
        if (m_isBust)
        {
            IsTurnOn = false;
        }
    }
    public void BlackJackCheck()
    {
        m_isBlackJack = Hand.IsBlackJack();
    }

    // 라이프 감소 메서드 추가됨
    public void DecreaseLife()
    {
        Debug.Log($"[BlackJackPlayer] Life before decrease: {m_life}");
        m_life--;
        Debug.Log($"[BlackJackPlayer] Life after decrease: {m_life}");
        OnLifeChanged?.Invoke(m_life); // 라이프 변경 시 이벤트 호출
    }

    // 라이프 증가 메서드 추가됨
    public void IncreaseLife()
    {
        if (m_life < m_maxLife)
        {
            m_life++;
            OnLifeChanged?.Invoke(m_life); // 라이프 변경 시 이벤트 호출
        }
    }

    // 아이템 추가 메서드
    public bool AddItem(ItemType item)
    {
        Debug.Log($"[BlackJackPlayer] Attempting to add item: {item} to {name}'s inventory. Current size: {m_inventory.Count}/{MAX_INVENTORY_SIZE}");
        if (m_inventory.Count < MAX_INVENTORY_SIZE)
        {
            m_inventory.Add(item);
            Debug.Log($"[BlackJackPlayer] Successfully added item: {item}. New inventory size: {m_inventory.Count}/{MAX_INVENTORY_SIZE}");
            return true;
        }
        Debug.LogWarning($"[BlackJackPlayer] Player {name}'s inventory is full. Cannot add item: {item}.");
        return false;
    }

    // 아이템 제거 메서드 (사용 후)
    public void RemoveItem(ItemType item)
    {
        if (m_inventory.Remove(item))
        {
            Debug.Log($"Player {name} removed item: {item}. Inventory size: {m_inventory.Count}/{MAX_INVENTORY_SIZE}");
        }
        else
        {
            Debug.LogWarning($"Player {name} tried to remove item {item}, but it was not found in inventory.");
        }
    }

    // 인벤토리 초기화 메서드 추가
    public void ClearInventory()
    {
        m_inventory.Clear();
        Debug.Log($"Player {name}'s inventory cleared.");
    }

}

