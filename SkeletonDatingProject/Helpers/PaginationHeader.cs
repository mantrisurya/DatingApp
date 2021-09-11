using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkeletonDatingProject.Helpers
{
    public class PaginationHeader
    {
        public PaginationHeader( int pageNumber, int pageSize, int totalItems, int itemsPerPage)
        {
            CurrentPage = pageNumber;
            ItemsPerPage = pageSize;
            TotalItems = totalItems;
            TotalPages = itemsPerPage;
        }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
