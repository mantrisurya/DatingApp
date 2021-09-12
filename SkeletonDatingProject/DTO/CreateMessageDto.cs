using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkeletonDatingProject.DTO
{
    public class CreateMessageDto
    {
        public string RecipientUserName { get; set; }
        public string Content { get; set; }
    }
}
