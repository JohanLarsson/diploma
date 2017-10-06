﻿using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Diploma.BLL.Properties;
using Diploma.BLL.Queries.Requests;
using Diploma.BLL.Queries.Responses;
using Diploma.BLL.Services.Interfaces;
using Diploma.DAL.Contexts;
using MediatR;

namespace Diploma.BLL.Queries.Handlers
{
    internal sealed class GetUserByCredentialsRequestHandler :
        ICancellableAsyncRequestHandler<GetUserByCredentialsRequest, OperationResult<UserDataResponse>>
    {
        private readonly Func<CompanyContext> _companyContextFactory;

        private readonly IMapper _mapper;

        private readonly IPasswordHasher _passwordHasher;

        public GetUserByCredentialsRequestHandler(Func<CompanyContext> companyContextFactory, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _companyContextFactory = companyContextFactory ?? throw new ArgumentNullException(nameof(companyContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<OperationResult<UserDataResponse>> Handle(GetUserByCredentialsRequest message, CancellationToken cancellationToken)
        {
            using (var context = _companyContextFactory())
            {
                var username = message.Username.ToUpper();
                var userDb = await context.Users.AsNoTracking().SingleOrDefaultAsync(x => username == x.Username.ToUpper(), cancellationToken)
                    .ConfigureAwait(false);

                if (userDb == null)
                {
                    return OperationResult<UserDataResponse>.CreateFailure(Resources.Exception_Authorization_Username_Not_Found);
                }

                if (!_passwordHasher.VerifyPasswordHash(message.Password, userDb.PasswordHash))
                {
                    return OperationResult<UserDataResponse>.CreateFailure(Resources.Exception_Authorization_Username_Or_Password_Invalid);
                }

                var userDto = _mapper.Map<UserDataResponse>(userDb);
                return OperationResult<UserDataResponse>.CreateSuccess(userDto);
            }
        }
    }
}
