public interface IUserContext
{
    int? UserId { get; }
    bool IsHost { get; }
}