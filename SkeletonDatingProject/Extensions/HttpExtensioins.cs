using Microsoft.AspNetCore.Http;
using SkeletonDatingProject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkeletonDatingProject.Extensions
{
    public static class HttpExtensioins
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPate, int itemsPerPage, int totalItems, int totalPages)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var paginationHeader = new PaginationHeader(currentPate, itemsPerPage, totalItems, totalPages);
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
