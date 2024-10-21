using System.Collections.Generic;
using System.Threading.Tasks;
using HUBTSOCIAL.src.Features.Chat.DTOs;
using HUBTSOCIAL.src.Features.Chat.Models;

namespace HUBTSOCIAL.src.Core.Interfaces.ChatInterfaces
{
    public interface IChatService
    {
        Task<bool> SendMessageAsync(MessageDTO messageDto);
        Task<List<MessageModel>?> GetMessagesInChatRoomAsync(string chatRoomId);
        Task<bool> DeleteMessageAsync(string chatRoomId, string messageId);
        Task<List<MessageModel>> SearchMessagesInChatRoomAsync(string chatRoomId, string keyword);
        Task<bool> UploadImageAsync(string userId, string chatRoomId, byte[] imageData);
        Task<bool> UploadFileAsync(string chatRoomId, byte[] fileData, string fileName);
    }
}
