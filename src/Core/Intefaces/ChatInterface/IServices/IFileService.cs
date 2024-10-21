
namespace HUBTSOCIAL.src.Core.Interfaces.ChatInterfaces
{
    public interface IImageService
    {
        Task<bool> UploadImageAsync(string userId, string chatRoomId, byte[] imageData);
    }
}