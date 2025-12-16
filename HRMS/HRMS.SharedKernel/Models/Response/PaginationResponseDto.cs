using HRMS.SharedKernel.Models.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Response
{
    public class PaginationResponseDto<T> : IPaginationResponseDto
    {
        public int TotalItems { get; set; }         
        public int PageNumber { get; set; }         
        public int PageSize { get; set; }           
        public int TotalPages { get; set; }         
        public int FirstPageToShow { get; set; }   
        public int LastPageToShow { get; set; }     
        public int FirstItemIndex { get; set; }     
        public int LastItemIndex { get; set; }      
        public IEnumerable<T> Items { get; set; } = [];
    }
}
