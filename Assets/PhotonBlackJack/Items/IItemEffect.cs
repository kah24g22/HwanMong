/// <summary>
/// 모든 아이템 효과가 구현해야 하는 인터페이스입니다.
/// </summary>
public interface IItemEffect
{
    /// <summary>
    /// 아이템의 효과를 실행합니다.
    /// </summary>
    /// <param name="gameLogic">현재 게임의 핵심 로직. 덱, 플레이어 정보에 접근하기 위해 필요합니다.</param>
    /// <param name="user">아이템을 사용한 플레이어입니다.</param>
    void Execute(SoloBlackJackLogic gameLogic, BlackJackPlayer user);
}
