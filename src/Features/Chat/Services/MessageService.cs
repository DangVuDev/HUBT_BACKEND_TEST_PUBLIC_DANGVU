using HUBTSOCIAL.src.Features.Chat.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HUBTSOCIAL.src.Core.Interfaces.ChatInterfaces;

namespace HUBTSOCIAL.src.Features.Chat.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMongoCollection<ChatRoom> _chatRooms;

        public MessageService(IMongoCollection<ChatRoom> chatRooms)
        {
            _chatRooms = chatRooms;
        }

        public async Task<bool> SendMessageAsync(string chatRoomId, string userId, string content)
        {
            var chatRoom = await _chatRooms.Find(cr => cr.Id == chatRoomId).FirstOrDefaultAsync();
            if (chatRoom == null)
                return false;

            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Content = content,
                Timestamp = DateTime.UtcNow
            };

            var update = Builders<ChatRoom>.Update.Push("Messages", message);
            await _chatRooms.UpdateOneAsync(cr => cr.Id == chatRoomId, update);

            return true;
        }

        public async Task<List<Message>?> GetMessagesInChatRoomAsync(string chatRoomId)
        {
            var chatRoom = await _chatRooms.Find(cr => cr.Id == chatRoomId).FirstOrDefaultAsync();
            return chatRoom?.Messages;
        }

        public async Task<bool> DeleteMessageAsync(string chatRoomId, string messageId)
        {
            var chatRoom = await _chatRooms.Find(cr => cr.Id == chatRoomId).FirstOrDefaultAsync();
            if (chatRoom == null)
                return false;

            var update = Builders<ChatRoom>.Update.PullFilter("Messages", Builders<Message>.Filter.Eq(m => m.Id, messageId));
            await _chatRooms.UpdateOneAsync(cr => cr.Id == chatRoomId, update);
            return true;
        }

        public async Task<List<Message>> SearchMessagesInChatRoomAsync(string chatRoomId, string keyword)
        {
            var chatRoom = await _chatRooms.Find(cr => cr.Id == chatRoomId).FirstOrDefaultAsync();
            return chatRoom?.Messages.Where(m => m.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList() ?? new List<Message>();
        }
    }
}
