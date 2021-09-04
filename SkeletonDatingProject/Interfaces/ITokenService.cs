using SkeletonDatingProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkeletonDatingProject.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}