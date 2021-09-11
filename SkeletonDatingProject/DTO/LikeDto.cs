using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkeletonDatingProject.DTO
{
    public class LikeDto
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string UserName { get; set; }
        public string KnownAs { get; set; }
        public string City { get; set; }
        public string PhotoUrl { get; set; }
    }
}
