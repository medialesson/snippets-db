using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Snippets.Web.Features.Users
{
    public class Verify
    {
        public class VerificationData
        {
            public string UserId { get; set; }

            public string VerificationKey { get; set; }
        }

        public class Command : IRequest<IActionResult>
        {
            public VerificationData Verification { get; set; }
        }

        public class Handler : IRequestHandler<Command, IActionResult>
        {
            readonly SnippetsContext _context;

            public Handler(SnippetsContext context)
            {
                _context = context;
            }

            public async Task<IActionResult> Handle(Command message, CancellationToken cancellationToken)
            {
                var result = await _context.Persons.FindAsync(message.Verification.UserId);
                
                // Check whether user exists in database
                if(result == null)
                    throw new RestException(HttpStatusCode.NotFound, $"User with ID {message.Verification.UserId} does not exist");

                // Check whether user was already verified in the past
                if(result.VerificationKey == null)
                    throw new RestException(HttpStatusCode.OK, "Account already verified");


                // Compare verification keys from request and database
                if (result.VerificationKey.Equals(message.Verification.VerificationKey))
                {
                    // Set it to null upon match
                    result.VerificationKey = null;
                    await _context.SaveChangesAsync();

                    return new OkResult();
                }
                else
                {
                    throw new RestException(HttpStatusCode.BadRequest, "Verification key invalid");
                }
            }
        }
    }
}
