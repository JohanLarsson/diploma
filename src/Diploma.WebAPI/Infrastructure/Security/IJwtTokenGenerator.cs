﻿using Diploma.WebAPI.Infrastructure.Entities;

namespace Diploma.WebAPI.Infrastructure.Security
{
    public interface IJwtTokenGenerator
    {
        string CreateToken(UserEntity user);
    }
}