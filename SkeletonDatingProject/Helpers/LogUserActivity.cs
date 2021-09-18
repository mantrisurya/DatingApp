using Microsoft.AspNetCore.Mvc.Filters;
using SkeletonDatingProject.Extensions;
using SkeletonDatingProject.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SkeletonDatingProject.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
            var userId = resultContext.HttpContext.User.GetUserId();
            var UOW = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var user = await UOW.UserRepository.GetUserByIdAsync(userId);
            user.LastActive = DateTime.UtcNow;
            await UOW.Complete();
        }
    }
}
