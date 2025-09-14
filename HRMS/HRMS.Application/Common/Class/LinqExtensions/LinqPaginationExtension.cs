using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Common.Class.LinqExtensions
{
    public static class LinqPaginationExtension
    {
        private static PaginationResponseDto<T> BuildPager<T>(PaginationRequestDto request, int totalItems)
        {
            var totalPages = Math.Max(1, (int)Math.Ceiling((decimal)totalItems / request.PageSize));
            request.CurrentPage = Math.Clamp(request.CurrentPage, 1, totalPages);

            var half = request.MaxPages / 2;
            var startPage = Math.Max(1, request.CurrentPage - half);
            var endPage = Math.Min(totalPages, startPage + request.MaxPages - 1);

            var startIndex = (request.CurrentPage - 1) * request.PageSize;
            var endIndex = Math.Min(startIndex + request.PageSize - 1, totalItems - 1);

            return new PaginationResponseDto<T>
            {
                TotalItems = totalItems,
                PageNumber = request.CurrentPage,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                FirstPageToShow = startPage,
                LastPageToShow = endPage,
                FirstItemIndex = startIndex,
                LastItemIndex = endIndex
            };
        }

        public static async Task<PaginationResponseDto<T>> PaginateAsync<T>(
            this IQueryable<T> source, PaginationRequestDto? request)
        {
            var totalItems = await source.CountAsync();

            if (totalItems == 0) return new PaginationResponseDto<T>();

            var pager = BuildPager<T>(request ?? new(), totalItems);

            pager.Items = await source.Skip(pager.FirstItemIndex).Take(pager.PageSize).ToListAsync();

            return pager;
        }
        public static PaginationResponseDto<T> Paginate<T>(
            this IQueryable<T> source, PaginationRequestDto? request)
        {
            var totalItems = source.Count();

            if (totalItems == 0) return new PaginationResponseDto<T>();

            var pager = BuildPager<T>(request ?? new(), totalItems);

            pager.Items = [.. source.Skip(pager.FirstItemIndex).Take(pager.PageSize)];

            return pager;
        }
    }
}
