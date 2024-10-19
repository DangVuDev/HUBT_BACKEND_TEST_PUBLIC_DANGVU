public interface IChatImageHub
{
    Task SendImage(string chatRoomId, string userId, byte[] imageData);
}