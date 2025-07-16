using UnityEngine;

/// <summary>
/// '자물쇠' 아이템의 효과를 구현한 클래스입니다.
/// 다음 턴 상대방의 Stand 행동을 금지합니다.
/// </summary>
public class LockEffect : IItemEffect
{
    public void Execute(PhotonBlackJackLogic gameLogic, BlackJackPlayer user)
    {
        // 1. 아이템을 사용한 플레이어를 기준으로 상대방을 찾습니다.
        BlackJackPlayer opponent = (gameLogic.Player1 == user) ? gameLogic.Player2 : gameLogic.Player1;
        if (opponent == null) return;

        // 2. 상대방의 스탠드 금지 상태를 true로 설정합니다.
        opponent.IsStandDisabled = true;

        Debug.Log($"{user.Player.NickName}님이 자물쇠 아이템을 사용하여 다음 턴 {opponent.Player.NickName}님의 Stand를 봉인했습니다!");

        // 이후 이 변경사항을 모든 클라이언트에게 동기화하는 네트워크 로직(RPC 호출 등)이 필요합니다.
    }
}
