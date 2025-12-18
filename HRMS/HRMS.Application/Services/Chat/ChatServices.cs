using HRMS.Application.Common.Class.LinqExtensions;
using HRMS.Application.Common.Interface;
using HRMS.Application.Services.File;
using HRMS.Domain.Entites;
using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Security.Claims;

namespace HRMS.Application.Services.Chat
{
    public class ChatServices : IChatServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IChatNotificationServices _chatNotificationServices;
        private readonly IFileServices _fileServices;
        public ChatServices(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IChatNotificationServices chatNotificationServices,
            IFileServices fileServices)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _chatNotificationServices = chatNotificationServices;
            _fileServices = fileServices;
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
        private IQueryable<ChatConversationListResponseDto> GetChatConversationListQuery(int employeeId, AdvanceTableRequestDto? search = null)
        {
            return _unitOfWork.ConversationsRepo.TableNoTracking
                        .Where(c => !c.IsDeleted && c.Participants.Any(p => p.EmployeeId == employeeId))
                        .Select(c => new
                        {
                            Conversation = c,
                            LastMessage = c.Messages
                                .OrderByDescending(m => m.CreatedUtcAt)
                                .Select(m => new { m.Content, m.CreatedUtcAt, m.SenderId, m.Id })
                                .FirstOrDefault(),

                            UnreadCount = c.Messages
                                .Where(m => m.SenderId != employeeId &&
                                            m.MessageStatuses.Any(ms => ms.EmployeeId == employeeId && ms.ReadAt == null))
                                .Count()
                        })
                        .OrderByDescending(x => x.LastMessage != null ? x.LastMessage.CreatedUtcAt : x.Conversation.CreatedDate)
                        .Select(x => new ChatConversationListResponseDto
                        {
                            Id = x.Conversation.Id,
                            Name = x.Conversation.Name,
                            Type = x.Conversation.TypeNavigation != null ? x.Conversation.TypeNavigation.Name : string.Empty,

                            Participants = x.Conversation.Participants
                                .Where(p => p.Employee != null && p.EmployeeId != employeeId)
                                .Select(p => p.Employee!.FullName)
                                .ToList(),

                            LastMessageDate = x.LastMessage != null ? x.LastMessage.CreatedUtcAt : x.Conversation.CreatedDate,
                            UnreadCount = x.UnreadCount,
                            LastMessage = x.LastMessage != null ? x.LastMessage.Content : "start a new conversation",
                            LastMessageId = x.LastMessage != null ? x.LastMessage.Id : null,
                            EmployeeId = x.LastMessage != null ? x.LastMessage.SenderId : 0
                        })
                        .SortBy(search?.Sort)
                        .FilterBy(search?.FilterGroup);
        }
        public Task<ApiResponseDto> GetChatConversationListAsync() => GetChatConversationSearchedListAsync();
        public async Task<ApiResponseDto> GetChatConversationSearchedListAsync(AdvanceTableRequestDto? request = null)
        {
            int employeeId = await GetCurrentEmployeeIdAsync();

            if (employeeId == 0)
                return ApiResponseDto.FailureStatus("Enable to fetch chat conversations, please try again later");

            var convos = await GetChatConversationListQuery(employeeId, request).ToListAsync();

            return ApiResponseDto.SuccessStatus(convos);
        }
        public async Task<ApiResponseDto> StartNewChatWithAsync(int chatWithEmployeeId)
        {
            int employeeId = await GetCurrentEmployeeIdAsync();
            if (employeeId == decimal.Zero || chatWithEmployeeId == decimal.Zero) return ApiResponseDto.FailureStatus("");

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
        private IQueryable<ChatMessageResponseDto> GetBaseMessageQuery(Expression<Func<Message, bool>> predicate, int employeeId, int take = 20)
        {
            return _unitOfWork.MessagesRepo.Table
                .Where(predicate)
                .OrderByDescending(m => m.CreatedUtcAt)   // newest of the older messages first
                .Take(take)
                .Select(m => new ChatMessageResponseDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    ParentMessageId = m.ParentMessageId,
                    CreatedDate = DateTime.SpecifyKind(m.CreatedUtcAt, DateTimeKind.Utc),
                    CreatedDateUtc = DateTime.SpecifyKind(m.CreatedUtcAt, DateTimeKind.Utc).ToString("o"),
                    IsMine = m.SenderId == employeeId,
                    SenderName = m.Sender != null ? m.Sender.FullName : string.Empty,
                    ParentMessage = m.ParentMessage != null ? (m.ParentMessage.Content ?? string.Empty) : string.Empty,
                    ParentMessageSenderName = m.ParentMessage != null && m.ParentMessage.Sender != null ? m.ParentMessage.Sender.FullName : string.Empty,
                    DeliveredAt = m.MessageStatuses
                                    .Where(ms => ms.EmployeeId != employeeId)
                                    .Select(ms => ms.DeliveredAt)
                                    .FirstOrDefault(),
                    ReadAt = m.MessageStatuses
                                    .Where(ms => 
                                        (m.SenderId == employeeId && ms.EmployeeId != employeeId)
                                        || (m.SenderId != employeeId && ms.EmployeeId == employeeId)
                                    )
                                    .Select(ms => ms.ReadAt)
                                    .FirstOrDefault(),
                    MessageType = m.MessageType != null ? m.MessageType.Value : "Text"
                });
        }
        public async Task<ApiResponseDto> GetPreviousMessagesAsync(NextMessagesRequestDto request)
        {
            int employeeId = await GetCurrentEmployeeIdAsync();
            if (employeeId == 0 || request.ConversationId == 0)
                return ApiResponseDto.FailureStatus("Failed to fetch chat messages.");

            var lastMessageUtc = request.LastMessageTime!.Value;
            // Fetch next (older) 20 messages older than LastMessageTime
            var messages = await GetBaseMessageQuery(
                                m => !m.IsDeleted
                                && m.ConversationId == request.ConversationId
                                && m.CreatedUtcAt < lastMessageUtc,
                            employeeId, 20)
                            .ToListAsync();

            var ordered = messages.OrderBy(m => m.CreatedDate);

            return ApiResponseDto.SuccessStatus(ordered);
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
                })
                .FirstOrDefaultAsync();

            if(convo is not null)
            {
                var messages = await GetBaseMessageQuery(m => !m.IsDeleted && m.ConversationId == conversationId, employeeId, 20).ToListAsync();
                convo.Messages = [.. messages.OrderBy(m => m.CreatedDate)];
            }

            return ApiResponseDto.SuccessStatus(convo);
        }
        public async Task<Message> SaveMessageAsync(int employeeId, ChatMessageRequestDto request)
        {
            var messageTypeId = await _unitOfWork.GeneralReferenceRepo.TableNoTracking
                .Where(mt => mt.Value.ToLower() == request.MessageType.ToLower() && mt.Category == "MessageType")
                .Select(mt => mt.Id)
                .FirstOrDefaultAsync();

            var message = new Message
            {
                ConversationId = request.ConversationId,
                SenderId = employeeId,
                Content = request.Content,
                MessageTypeId = messageTypeId == 0 ? 1 : messageTypeId, 
                ParentMessageId = request.ParentMessageId,
                IsEdited = false,
                CreatedUtcAt = DateTime.UtcNow,
                UpdatedUtcAt = DateTime.UtcNow
            };
            _unitOfWork.MessagesRepo.Add(message);
            await _unitOfWork.SaveChangesAsync();

            if (request.MessageType == "File" && request.FileId is not null) await AddAttachmentAsync(request.FileId.Value, message.Id);

            message = await _unitOfWork.MessagesRepo.TableNoTracking
                        .Include(m => m.Sender)
                        .Include(m => m.ParentMessage).ThenInclude(pm => pm!.Sender)
                        .Include(m => m.MessageType)
                        .Include(m => m.Attachments).ThenInclude(a => a.File)
                        .FirstAsync(m => m.Id == message.Id);

            return message;
        }
        private async Task AddAttachmentAsync(Guid Id, int messageId)
        {
            var fileId = await _unitOfWork.StoredFilesRepo.GetIdByGuid(Id);

            if(fileId is not null)
            {
                var attachment = new Attachment();
                attachment.Attach(messageId, fileId.Value);
                _unitOfWork.AttachmentsRepo.Add(attachment);
                await _unitOfWork.SaveAsync();
            }

        }
        public async Task SaveMessageStatus(int messageId, List<ConversationParticipants> participants)
        {
            foreach (var participant in participants)
            {
                var status = new MessageStatus();
                status.Assign(messageId, participant.EmployeeId);
                _unitOfWork.MessageStatusRepo.Add(status);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(int conversationId, int employeeId)
        {
            return await _unitOfWork.MessageStatusRepo.TableNoTracking
                .Where(ms => ms.EmployeeId == employeeId                    // status belongs to current employee
                             && ms.ReadAt == null                           // unread only
                             && ms.Message != null                          
                             && ms.Message.ConversationId == conversationId // specific conversation
                             && ms.Message.SenderId != employeeId)          // messages SENT BY OTHERS
                .CountAsync();
        }
        public async Task<ApiResponseDto> SendMessageAsync(ChatMessageRequestDto request)
        {
            int employeeId = await GetCurrentEmployeeIdAsync();

            if (employeeId == 0)
                return ApiResponseDto.FailureStatus("Failed to send message, please try again later.");

            var message = await SaveMessageAsync(employeeId, request);
            
            var participants = await _unitOfWork.ConversationParticipantsRepo.TableNoTracking
                .Where(p => p.ConversationId == request.ConversationId)
                .ToListAsync();

            await SaveMessageStatus(message.Id, participants);

            var response = new ChatMessageResponseDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = message.Sender?.FullName ?? "Unknown",
                Content = message.Content,
                ParentMessageId = message.ParentMessageId,
                CreatedDate = DateTime.SpecifyKind(message.CreatedUtcAt, DateTimeKind.Utc),
                CreatedDateUtc = DateTime.SpecifyKind(message.CreatedUtcAt, DateTimeKind.Utc).ToString("o"),
                DeliveredAt = null,
                ReadAt = null,
                ParentMessageSenderName = message.ParentMessage?.Sender?.FullName ?? string.Empty,
                ParentMessage = message.ParentMessage?.Content ?? string.Empty,
                MessageType = message.MessageType != null ? message.MessageType.Value : "Text",
                FileId = message.Attachments.Count != 0 ? message.Attachments.First().File!.GuidId : null
            };

            foreach (var participant in participants)
            {
                if (participant.EmployeeId != employeeId)
                {
                    var user = await _unitOfWork.UserRepo.TableNoTracking.FirstOrDefaultAsync(u => u.EmployeeId == participant.EmployeeId);
                    response.UnreadCount = await GetUnreadCountAsync(response.ConversationId, participant.EmployeeId);
                    await _chatNotificationServices.SendMessageToUserAsync(user?.Id.ToString() ?? string.Empty, response);
                }
            }

            response.IsMine = true;
            return ApiResponseDto.SuccessStatus(response, "Message sent successfully.");
        }

        public async Task<ApiResponseDto> MarkMessageAsDeliveredAsync(int messageId)
        {
            int employeeId = await GetCurrentEmployeeIdAsync();

            if (employeeId == 0)
                return ApiResponseDto.FailureStatus("Failed to send message, please try again later.");

            var statusesToMark = await _unitOfWork.MessageStatusRepo.Table
                .Where(ms => ms.EmployeeId == employeeId
                             && ms.ReadAt == null
                             && ms.Message != null
                             && ms.Message.Id == messageId
                             && ms.Message.SenderId != employeeId)
                .Include(ms => ms.Message)
                .ToListAsync();

            if (statusesToMark.Count == 0)
                return ApiResponseDto.SuccessStatus("No undelivered messages.");

            foreach(var messageStatus in statusesToMark)
            {
                messageStatus.MarkDelivered(DateTime.UtcNow);
                _unitOfWork.MessageStatusRepo.Update(messageStatus);
            }

            return await NotifyDeliveredStatusAsync(statusesToMark);
        }
        private async Task<ApiResponseDto> NotifyDeliveredStatusAsync(List<MessageStatus> seenStatusList)
        {
            var groupedBySender = seenStatusList
                .Where(ms => ms.Message != null)
                .GroupBy(ms => ms.Message!.SenderId)
                .ToList();

            foreach (var group in groupedBySender)
            {
                var senderUser = await _unitOfWork.UserRepo.TableNoTracking.FirstOrDefaultAsync(u => u.EmployeeId == group.Key);

                if (senderUser == null) continue;

                var user = await _unitOfWork.UserRepo.TableNoTracking.FirstOrDefaultAsync(u => u.EmployeeId == senderUser.EmployeeId);

                if (user is not null) await _chatNotificationServices.SendDeliveredStatusToUserAsync<List<int>>(
                    user.Id.ToString(),
                    [.. group.Select(ms => ms.MessageId).Distinct()]
                );
            }

            return ApiResponseDto.SuccessStatus("Messages marked as delivered.");
        }

        public async Task<ApiResponseDto> MarkMessageAsReadAsync(UpdateSeenRequestDto request)
        {
            int employeeId = await GetCurrentEmployeeIdAsync();
            if (employeeId == 0)
                return ApiResponseDto.FailureStatus("Failed to mark messages as read.");

            var tillReadUtc = request.TillRead!.Value.AddMilliseconds(1);

            // 1) Load message statuses that belong to this employee and are unread
            var statusesToMark = await _unitOfWork.MessageStatusRepo.Table
                .Where(ms => ms.EmployeeId == employeeId
                             && ms.ReadAt == null
                             && ms.Message != null
                             && ms.Message.CreatedUtcAt <= tillReadUtc
                             && ms.Message.ConversationId == request.ConversationId
                             && ms.Message.SenderId != employeeId)
                .Include(ms => ms.Message)
                .ToListAsync();

            if (statusesToMark.Count == 0)
                return ApiResponseDto.SuccessStatus("No unread messages.");

            // 2) Mark all statuses as read
            foreach (var ms in statusesToMark)
            {
                ms.MarkRead(DateTime.UtcNow);

                if (ms.DeliveredAt is null) 
                    ms.MarkDelivered(DateTime.UtcNow);

                _unitOfWork.MessageStatusRepo.Update(ms);
            }

            var saved = await _unitOfWork.SaveAsync();

            if (!saved)
                return ApiResponseDto.FailureStatus("Failed to update read status.");

            return await NotifySeenStatusAsync(statusesToMark);
        }
        private async Task<ApiResponseDto> NotifySeenStatusAsync(List<MessageStatus> seenStatusList)
        {
            var groupedBySender = seenStatusList
                .Where(ms => ms.Message != null)
                .GroupBy(ms => ms.Message!.SenderId)
                .ToList();

            foreach (var group in groupedBySender)
            {
                var senderUser = await _unitOfWork.UserRepo.TableNoTracking.FirstOrDefaultAsync(u => u.EmployeeId == group.Key);

                if (senderUser == null) continue;

                var user = await _unitOfWork.UserRepo.TableNoTracking.FirstOrDefaultAsync(u => u.EmployeeId == senderUser.EmployeeId);

                if (user is not null) await _chatNotificationServices.SendSeenStatusToUserAsync<List<int>>(
                    user.Id.ToString(),
                    [.. group.Select(ms => ms.MessageId).Distinct()]
                );
            }

            return ApiResponseDto.SuccessStatus("Messages marked as read.");
        }

        public async Task<ApiResponseDto> SendTypingStatusAsync(TypingRequestDto request)
        {
            int employeeId = await GetCurrentEmployeeIdAsync();
            if (employeeId == 0)
                return ApiResponseDto.FailureStatus("Failed to mark messages as read.");

            // Validate conversation + participant
            bool isMember = await _unitOfWork.ConversationParticipantsRepo.TableNoTracking
                .AnyAsync(p => p.ConversationId == request.ConversationId && p.EmployeeId == employeeId);

            if (!isMember)
                return ApiResponseDto.FailureStatus("User not part of conversation.");

            // OPTIONAL: log typing time, update DB etc.

            // Fetch all participants except the one typing
            var participantUserIds = await _unitOfWork.UserRepo.TableNoTracking
                .Where(u => u.EmployeeId != employeeId
                         && _unitOfWork.ConversationParticipantsRepo.TableNoTracking
                             .Any(cp => cp.EmployeeId == u.EmployeeId &&
                                        cp.ConversationId == request.ConversationId))
                .Select(u => u.Id.ToString())
                .ToListAsync();

            // Broadcast typing status to Hub
            foreach (var userId in participantUserIds)
                await _chatNotificationServices.SendTypingToUserStatus(userId, request);

            return ApiResponseDto.SuccessStatus("");
        }

        public async Task<ApiResponseDto> GetAttachmentFileAsync(Guid Id)
        {
            var attachment = await _unitOfWork.AttachmentsRepo.TableNoTracking
                .Include(a => a.File)
                .FirstOrDefaultAsync(a => a.File != null && a.File.GuidId == Id);

            if (attachment == null || attachment.File == null)
                return ApiResponseDto.FailureStatus("Attachment not found.");

            var file = await _fileServices.GetFileByStoredFileIdAsync(attachment.File.Id);

            if(string.IsNullOrEmpty(file))
                return ApiResponseDto.FailureStatus("Attachment file could not be retrieved.");

            return ApiResponseDto.SuccessStatus(new FileBase64ResponseDto
            {
                FileBase64 = file,
                FileId = attachment.File.GuidId
            });
        }
    }
}
