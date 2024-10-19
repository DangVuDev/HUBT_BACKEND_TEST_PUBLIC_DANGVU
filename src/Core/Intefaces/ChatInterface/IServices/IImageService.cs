
namespace HUBTSOCIAL.src.Core.Interfaces.ChatInterfaces
{
    public interface IFileService
    {
        Task<bool> UploadFileAsync(string chatRoomId, byte[] fileData, string fileName);
    }
}