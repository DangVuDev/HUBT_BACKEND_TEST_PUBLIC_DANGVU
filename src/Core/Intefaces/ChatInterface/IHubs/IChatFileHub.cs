namespace HUBTSOCIAL.src.Core.Interfaces.IHubs
{
    public interface IChatFileHub
    {
        Task SendFile(string chatRoomId, string userId, byte[] fileData, string fileName);
    }    
}
