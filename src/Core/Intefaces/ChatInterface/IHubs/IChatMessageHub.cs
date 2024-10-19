
public interface IChatMessageHub
{
    Task SendMessage(string chatRoomId, string userId, string messageContent);
    Task JoinRoom(string chatRoomId);
    Task LeaveRoom(string chatRoomId);
}