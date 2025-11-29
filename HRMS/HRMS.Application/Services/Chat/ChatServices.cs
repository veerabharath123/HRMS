using HRMS.Application.Common.Interface;
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
    public class ChatServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ChatServices(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponseDto> GetChatConversationListAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out int currentUserId))
            {
                return ApiResponseDto.FailureStatus("Enable to fetch chat list, please try again later");
            }

            var convos = await _unitOfWork.ConversationsRepo.TableNoTracking
                .Where(c => !c.IsDeleted && c.Participants.Any(p => p.EmployeeId == 0))
                .Select(c => new ChatConversationListResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.TypeNavigation != null ? c.TypeNavigation.Name : string.Empty,
                    Participants = c.Participants
                                .Where(p => p.Employee != null && p.EmployeeId != currentUserId)
                                .Select(p => p.Employee!.FullName)
                                .ToList(),
                    LastMessageDate = c.Messages
                                .OrderByDescending(m => m.CreatedDate)
                                .Select(m => (DateTime?)m.CreatedDate)
                                .FirstOrDefault()
                })
                .OrderByDescending(x => x.LastMessageDate)
                .ToListAsync();

            return ApiResponseDto.SuccessStatus(convos);
        }

        //public async Task<ApiResponseDto> GetChatConversationAsync(int conversationId)
        //{
        //    var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (!int.TryParse(userId, out int currentUserId))
        //    {
        //        return ApiResponseDto.FailureStatus("Enable to fetch chat conversation, please try again later");
        //    }
        //    var messages = await _unitOfWork.MessagesRepo.TableNoTracking
        //                    .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
        //                    .OrderBy(m => m.CreatedDate)
        //                    .Select(m => new MessageViewModel
        //                    {
        //                        Id = m.Id,
        //                        ConversationId = m.ConversationId,
        //                        SenderId = m.SenderId,
        //                        SenderName = m.Sender.FullName,
        //                        Content = m.Content,
        //                        ParentMessageId = m.ParentMessageId,
        //                        CreatedDate = m.CreatedDate,

        //                        IsMine = m.SenderId == currentUserId,

        //                        DeliveredAt = m.MessageStatuses
        //                            .Where(ms => ms.EmployeeId == currentUserId)
        //                            .Select(ms => ms.DeliveredAt)
        //                            .FirstOrDefault(),

        //                        ReadAt = m.MessageStatuses
        //                            .Where(ms => ms.EmployeeId == currentUserId)
        //                            .Select(ms => ms.ReadAt)
        //                            .FirstOrDefault()
        //                    })
        //                    .ToListAsync();

        //    return ApiResponseDto.SuccessStatus(conversation);
        //}
    }
}
