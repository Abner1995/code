namespace BuildingBlocks.Identity;

/// <summary>
/// 用户上下文接口，提供当前认证用户的信息
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// 当前用户ID（如果用户未认证则返回null）
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// 当前用户名（如果用户未认证则返回null）
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// 当前用户是否已认证
    /// </summary>
    bool IsAuthenticated { get; }
}