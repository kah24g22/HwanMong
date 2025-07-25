﻿using UnityEngine;
using System;

public class SoloBlackJackLogic
{
    private BlackJackPlayer m_player1;
    private BlackJackPlayer m_player2;

    private Deck m_deck;
    public Deck Deck { get { return m_deck; } } // Deck 접근자 추가

    private PlayerTurn m_turn;
    public bool IsNextCardHidden { get; set; } // 편지봉투 아이템을 위한 플래그

    public BlackJackPlayer Player1 { get { return m_player1; } }
    public BlackJackPlayer Player2 { get { return m_player2; } }
    public PlayerTurn Turn { get { return m_turn; } set { m_turn = value; } }

    private bool m_isGameEnded = false; // 게임 종료 플래그 추가
    private bool isFirstRound = true; // 첫 라운드 여부를 추적하는 플래그

    private ItemType GetRandomItemType()
    {
        Array itemTypes = Enum.GetValues(typeof(ItemType));
        // None을 제외한 아이템 중에서 랜덤 선택
        ItemType randomItem;
        do
        {
            randomItem = (ItemType)itemTypes.GetValue(UnityEngine.Random.Range(1, itemTypes.Length));
        } while (randomItem == ItemType.None); // None이 아닌 아이템을 선택할 때까지 반복
        return randomItem;
    }



    public delegate void CardDealtEventHandler(BlackJackPlayer pPlayer, Card card, bool isFaceUp);
    public event CardDealtEventHandler OnCardDealt;

    public delegate void GameStateChangedEventHandler(string message);
    public event GameStateChangedEventHandler OnGameEnded;

    public delegate void TurnChangedEventHandler(PlayerTurn currentPlayer);
    public event TurnChangedEventHandler OnTurnChanged;

    public SoloBlackJackLogic(BlackJackPlayer player1, BlackJackPlayer player2)
    {
        m_player1 = player1;
        m_player2 = player2;
        m_deck = new Deck();
        isFirstRound = true; // 게임 시작 시 첫 라운드로 설정
    }


    public void StartGame()
    {
        m_isGameEnded = false; // 게임 시작 시 플래그 초기화
        m_deck.CreateDeck();
        m_deck.Shuffle();

        Debug.Log(m_player1);
        Debug.Log(m_player2);

        m_player1.PlayerReset();
        m_player2.PlayerReset();

        if (isFirstRound)
        {
            // 첫 라운드에는 양측 플레이어에게 아이템 지급
            m_player1.AddItem(GetRandomItemType());
            m_player2.AddItem(GetRandomItemType());
            isFirstRound = false; // 첫 라운드 완료
        }

        m_turn = PlayerTurn.Player1;
        OnTurnChanged?.Invoke(m_turn);

        m_player1.BlackJackCheck();
        m_player2.BlackJackCheck();

        if (m_player1.IsBlackJack || m_player2.IsBlackJack) WinnerCheck();
    }

    public void DealCard(BlackJackPlayer pPlayer, bool isFaceUp)
    {
        Card newCard = m_deck.Deal();

        if (newCard != null)
        {
            newCard.IsFaceUp = isFaceUp; // 카드의 앞면/뒷면 상태 설정
            pPlayer.Hand.AddCard(newCard);
            OnCardDealt?.Invoke(pPlayer, newCard, isFaceUp);
        }
    }

    public void Hit()
    {
        BlackJackPlayer currentPlayer = (m_turn == PlayerTurn.Player1) ? m_player1 : m_player2;

        bool faceUp = !IsNextCardHidden; // 플래그에 따라 isFaceUp 결정
        DealCard(currentPlayer, faceUp);
        IsNextCardHidden = false; // 플래그 초기화

        SwitchTurnOrEndGame();

        currentPlayer.BlackJackCheck();
        currentPlayer.BustCheck();

        if (currentPlayer.IsBlackJack || currentPlayer.IsBust)
        {
            WinnerCheck();
            return;
        }
    }

    public void Stand()
    {
        BlackJackPlayer currentPlayer = (m_turn == PlayerTurn.Player1) ? m_player1 : m_player2;

        // 스탠드 금지 상태인지 확인
        if (currentPlayer.IsStandDisabled)
        {
            Debug.Log($"{currentPlayer.Player.NickName}님은 자물쇠 효과로 인해 Stand를 할 수 없습니다!");
            // (추가) 여기에 사용자에게 알림을 주는 UI 로직을 연결할 수 있습니다.
            return; // 행동 취소
        }

        currentPlayer.IsTurnOn = false;
        SwitchTurnOrEndGame();
    }

    private void SwitchTurnOrEndGame()
    {
        if (!m_player1.IsTurnOn && !m_player2.IsTurnOn)
        {
            WinnerCheck();
            return;
        }

        else if (!m_player1.IsTurnOn)
        {
            Debug.LogError("player1 IsTurnOn OFF");
            m_turn = PlayerTurn.Player2;
            OnTurnChanged?.Invoke(m_turn);
            return;
        }

        else if (!m_player2.IsTurnOn)
        {
            Debug.LogError("player2 IsTurnOn OFF");
            m_turn = PlayerTurn.Player1;
            OnTurnChanged?.Invoke(m_turn);
            return;
        }

        if (m_turn == PlayerTurn.Player1)
        {
            Debug.Log("player 1 -> player 2");
            m_turn = PlayerTurn.Player2;
            OnTurnChanged?.Invoke(m_turn);
            return;
        }
        else if (m_turn == PlayerTurn.Player2)
        {
            Debug.Log("player 2 -> player 1");
            m_turn = PlayerTurn.Player1;
            OnTurnChanged?.Invoke(m_turn);
            return;
        }
    }

    private void EndGame(string message)
    {
        m_isGameEnded = true; // 게임 종료 시 플래그 설정
        OnGameEnded?.Invoke(message);
    }

    public Result CalculrateWinner()
    {
        if (m_player1.IsBust && m_player2.IsBust) return Result.Draw;
        if (m_player1.Hand.CalculateValue() == m_player2.Hand.CalculateValue()) return Result.Draw;

        if (m_player1.IsBust) return Result.Lose;
        if (m_player2.IsBust) return Result.Win;
        if (m_player1.Hand.CalculateValue() > m_player2.Hand.CalculateValue()) return Result.Win;
        if (m_player1.Hand.CalculateValue() < m_player2.Hand.CalculateValue()) return Result.Lose;

        return Result.None;
    }
    public void WinnerCheck()
    {
        if (m_isGameEnded) return; // 이미 게임이 종료되었으면 중복 호출 방지

        Result result = CalculrateWinner();
        switch (result)
        {
            case Result.Win:
                {
                    m_player2.DecreaseLife(); // 라이프 감소 로직 추가
                    m_player2.AddItem(GetRandomItemType()); // 패배한 플레이어에게 아이템 지급
                    Debug.Log($"[SoloBlackJackLogic] Player2 (enemy) received an item.");
                    EndGame("Player1 Wins.");
                    break;
                }
            case Result.Lose:
                {
                    m_player1.DecreaseLife(); // 라이프 감소 로직 추가
                    m_player1.AddItem(GetRandomItemType()); // 패배한 플레이어에게 아이템 지급
                    Debug.Log($"[SoloBlackJackLogic] Player1 (my) received an item.");
                    EndGame("Player2 Wins.");
                    break;
                }
            case Result.Draw:
                {
                    EndGame("Draw");
                    break;
                }
        }
    }
}

