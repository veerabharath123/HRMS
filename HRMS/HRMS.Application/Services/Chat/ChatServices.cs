using HRMS.Application.Common.Interface;
using HRMS.Domain.Entites;
using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Services.Chat
{
    public class ChatServices : IChatServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ChatServices(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        private async Task<int> GetCurrentEmployeeIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int currentUserId))
            {
                var emp = await _unitOfWork.UserRepo.TableNoTracking.FirstOrDefaultAsync(u => u.Id == currentUserId && !u.IsDeleted);
                if (emp != null)
                {
                    return emp.EmployeeId ?? 0;
                }
            }
            return 0;
        }
        public async Task<ApiResponseDto> GetChatConversationListAsync()
        {
            int employeeId = await GetCurrentEmployeeIdAsync();

            if (employeeId == 0)
                return ApiResponseDto.FailureStatus("Enable to fetch chat conversations, please try again later");

            var convos = await _unitOfWork.ConversationsRepo.TableNoTracking
                        .Where(c => !c.IsDeleted && c.Participants.Any(p => p.EmployeeId == employeeId))

                        .Select(c => new
                        {
                            Conversation = c,
                            LastMessage = c.Messages
                                .OrderByDescending(m => m.CreatedDate)
                                .Select(m => new { m.Content, m.CreatedDate, m.SenderId })
                                .FirstOrDefault(),

                            UnreadCount = c.Messages
                                .Where(m => m.SenderId != employeeId &&
                                            m.MessageStatuses.Any(ms => ms.EmployeeId == employeeId && ms.ReadAt == null))
                                .Count()
                        })
                        .OrderByDescending(x => x.LastMessage != null ? x.LastMessage.CreatedDate : x.Conversation.CreatedDate)
                        .Select(x => new ChatConversationListResponseDto
                        {
                            Id = x.Conversation.Id,
                            Name = x.Conversation.Name,
                            Type = x.Conversation.TypeNavigation != null ? x.Conversation.TypeNavigation.Name : string.Empty,

                            Participants = x.Conversation.Participants
                                .Where(p => p.Employee != null && p.EmployeeId != employeeId)
                                .Select(p => p.Employee!.FullName)
                                .ToList(),

                            LastMessageDate = x.LastMessage != null ? x.LastMessage.CreatedDate : x.Conversation.CreatedDate,
                            UnreadCount = x.UnreadCount,
                            LastMessage = x.LastMessage != null ? x.LastMessage.Content : "start a new conversation",
                            EmployeeId = x.LastMessage != null ? x.LastMessage.SenderId : 0
                        })
                        .ToListAsync();


            return ApiResponseDto.SuccessStatus(convos);
        }
        public async Task<ApiResponseDto> StartNewChatWithAsync(int chatWithEmployeeId)
        {
            int employeeId = await GetCurrentEmployeeIdAsync();
            if (employeeId == 0 || chatWithEmployeeId == 0) return ApiResponseDto.FailureStatus("");

            var convo = await CreateOrGetDirectConversationAsync(employeeId, chatWithEmployeeId);
            if (convo != null) return await GetChatConversationListAsync();

            return ApiResponseDto.FailureStatus("Failed to create Conversation.");
        }
        public async Task<Conversation> CreateOrGetDirectConversationAsync(int employeeId, int otherEmployeeId)
        {
            // Get Direct Type Id
            int directTypeId = await _unitOfWork.ConversationTypesRepo.TableNoTracking
                .Where(t => t.Name == "Direct")
                .Select(t => t.Id)
                .FirstAsync();

            // Check existing direct chat
            var existing = await _unitOfWork.ConversationsRepo.Table
                .Where(c => !c.IsDeleted && c.Type == directTypeId)
                .Where(c => c.Participants.Any(p => p.EmployeeId == employeeId))
                .Where(c => c.Participants.Any(p => p.EmployeeId == otherEmployeeId))
                .Include(c => c.Participants)
                .FirstOrDefaultAsync();

            if (existing != null)
                return existing;  // conversation already exists

            var newConv = new Conversation
            {
                Name = null, // Optional or can auto-generate
                Type = directTypeId
            };

            _unitOfWork.ConversationsRepo.Add(newConv);
            await _unitOfWork.SaveChangesAsync();

            var mine = new ConversationParticipants();
            var other = new ConversationParticipants();

            mine.AddConversation(newConv.Id, employeeId);
            other.AddConversation(newConv.Id, otherEmployeeId);

            _unitOfWork.ConversationParticipantsRepo.Add(mine);
            _unitOfWork.ConversationParticipantsRepo.Add(other);
            await _unitOfWork.SaveChangesAsync();

            return newConv;
        }
        public async Task<ApiResponseDto> GetChatConversationDetailsAsync(int conversationId)
        {
            int employeeId = await GetCurrentEmployeeIdAsync();
            if (employeeId == 0 || conversationId == 0) return ApiResponseDto.FailureStatus("Enable to fetch chat conversation, please try again later");

            var convo = await _unitOfWork.ConversationsRepo.Table
                .Where(c => !c.IsDeleted && c.Id == conversationId)
                .Include(c => c.Participants)
                .Select(c => new ChatConversationDetailResponseDto
                {
                    Id = c.Id,
                    Type = c.TypeNavigation != null ? c.TypeNavigation.Name : string.Empty,
                    Name = c.Name ?? c.Participants.Where(cp => cp.ConversationId == c.Id && cp.EmployeeId != employeeId).First().Employee.FullName,
                    Messages = c.Messages.Where(m => !m.IsDeleted)
                        .OrderBy(m => m.CreatedDate)
                        .Select(m => new ChatMessageResponseDto
                        {
                            Id = m.Id,
                            ConversationId = m.ConversationId,
                            SenderId = m.SenderId,
                            Content = m.Content,
                            ParentMessageId = m.ParentMessageId,
                            CreatedDate = m.CreatedDate,
                            IsMine = m.SenderId == employeeId,
                            DeliveredAt = m.MessageStatuses
                                .Where(ms => ms.EmployeeId == employeeId)
                                .Select(ms => ms.DeliveredAt)
                                .FirstOrDefault(),
                            ReadAt = m.MessageStatuses
                                .Where(ms => ms.EmployeeId == employeeId)
                                .Select(ms => ms.ReadAt)
                                .FirstOrDefault()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return ApiResponseDto.SuccessStatus(convo);
        }

        public async Task<ApiResponseDto> SendMessageAsync(ChatMessageRequestDto request)
        {
            int employeeId = await GetCurrentEmployeeIdAsync();
            var messageTypeId = await _unitOfWork.GeneralReferenceRepo.TableNoTracking
                .Where(mt => mt.Code == "T" && mt.Category == "MessageType")
                .Select(mt => mt.Id)
                .FirstAsync();

            if (employeeId == 0 || messageTypeId == 0)
                return ApiResponseDto.FailureStatus("Failed to send message, please try again later.");

            var message = new Message
            {
                ConversationId = request.ConversationId,
                SenderId = employeeId,
                Content = request.Content,
                MessageTypeId = messageTypeId, // Assuming 1 is for text messages
                ParentMessageId = null,
                IsEdited = false
            };
            _unitOfWork.MessagesRepo.Add(message);
            await _unitOfWork.SaveChangesAsync();
            // Add Message Status for all participants
            var participants = await _unitOfWork.ConversationParticipantsRepo.TableNoTracking
                .Where(p => p.ConversationId == request.ConversationId)
                .ToListAsync();

            foreach (var participant in participants)
            {
                var status = new MessageStatus();
                status.Assign(message.Id, participant.EmployeeId);
                _unitOfWork.MessageStatusRepo.Add(status);
            }
            await _unitOfWork.SaveChangesAsync();

            return ApiResponseDto.SuccessStatus(new ChatMessageResponseDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = participants.FirstOrDefault(p => p.EmployeeId == message.SenderId)?.Employee?.FullName ?? "Unknown",
                Content = message.Content,
                ParentMessageId = message.ParentMessageId,
                CreatedDate = message.CreatedDate,
                IsMine = true,
                DeliveredAt = null,
                ReadAt = null
            }, "Message sent successfully.");
        }
    }
}
