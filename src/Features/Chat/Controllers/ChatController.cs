using Microsoft.AspNetCore.Mvc;
using HUBTSOCIAL.src.Features.Chat.Services;
using HUBTSOCIAL.src.Features.Chat.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using HUBTSOCIAL.src.Core.Interfaces.ChatInterfaces;

namespace HUBTSOCIAL.src.Features.Chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Gửi tin nhắn vào phòng chat.
        /// </summary>
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDTO messageDto)
        {
            if (messageDto == null || string.IsNullOrEmpty(messageDto.Content) || string.IsNullOrEmpty(messageDto.ChatRoomId))
            {
                return BadRequest(new { message = "Invalid message data" });
            }

            bool result = await _chatService.SendMessageAsync(messageDto);
            return result 
                ? Ok(new { message = "Message sent successfully" }) 
                : BadRequest(new { message = "Failed to send message" });
        }

        /// <summary>
        /// Lấy tất cả tin nhắn trong phòng chat.
        /// </summary>
        [HttpGet("{chatRoomId}/messages")]
        public async Task<IActionResult> GetMessages(string chatRoomId)
        {
            if (string.IsNullOrEmpty(chatRoomId))
            {
                return BadRequest(new { message = "Chat room ID is required" });
            }

            var messages = await _chatService.GetMessagesInChatRoomAsync(chatRoomId);
            return messages == null || messages.Count == 0 
                ? NotFound(new { message = "No messages found" }) 
                : Ok(messages);
        }

        /// <summary>
        /// Xóa tin nhắn trong phòng chat.
        /// </summary>
        [HttpDelete("{chatRoomId}/messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(string chatRoomId, string messageId)
        {
            if (string.IsNullOrEmpty(chatRoomId) || string.IsNullOrEmpty(messageId))
            {
                return BadRequest(new { message = "Chat room ID and message ID are required" });
            }

            bool result = await _chatService.DeleteMessageAsync(chatRoomId, messageId);
            return result 
                ? Ok(new { message = "Message deleted successfully" }) 
                : BadRequest(new { message = "Failed to delete message" });
        }

        /// <summary>
        /// Tìm kiếm tin nhắn trong phòng chat theo từ khóa.
        /// </summary>
        [HttpGet("{chatRoomId}/search")]
        public async Task<IActionResult> SearchMessages(string chatRoomId, [FromQuery] string keyword)
        {
            if (string.IsNullOrEmpty(chatRoomId) || string.IsNullOrEmpty(keyword))
            {
                return BadRequest(new { message = "Chat room ID and keyword are required" });
            }

            var messages = await _chatService.SearchMessagesInChatRoomAsync(chatRoomId, keyword);
            return messages == null || messages.Count == 0 
                ? NotFound(new { message = "No messages found for the given keyword" }) 
                : Ok(messages);
        }

        /// <summary>
        /// Upload hình ảnh lên phòng chat.
        /// </summary>
        [HttpPost("{chatRoomId}/upload-image")]
        public async Task<IActionResult> UploadImage(string userId, string chatRoomId, [FromForm] byte[] imageData)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(chatRoomId) || imageData == null || imageData.Length == 0)
            {
                return BadRequest(new { message = "Invalid image data" });
            }

            bool result = await _chatService.UploadImageAsync(userId, chatRoomId, imageData);
            return result 
                ? Ok(new { message = "Image uploaded successfully" }) 
                : BadRequest(new { message = "Failed to upload image" });
        }

        /// <summary>
        /// Upload file lên phòng chat.
        /// </summary>
        [HttpPost("{chatRoomId}/upload-file")]
        public async Task<IActionResult> UploadFile(string chatRoomId, [FromForm] byte[] fileData, [FromForm] string fileName)
        {
            if (string.IsNullOrEmpty(chatRoomId) || fileData == null || fileData.Length == 0 || string.IsNullOrEmpty(fileName))
            {
                return BadRequest(new { message = "Invalid file data" });
            }

            bool result = await _chatService.UploadFileAsync(chatRoomId, fileData, fileName);
            return result 
                ? Ok(new { message = "File uploaded successfully" }) 
                : BadRequest(new { message = "Failed to upload file" });
        }
    }
}
