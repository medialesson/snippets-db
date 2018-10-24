using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public class UserVerificationData
        {
            public string VerificationKey { get; set; }
        }

        public class UserVerificationDataValidator : AbstractValidator<UserVerificationData>
        {
            public UserVerificationDataValidator()
            {
                RuleFor(x => x.VerificationKey).NotEmpty().WithMessage("Verification key has to have a value");
            }
        } 

        public class Command : IRequest<IActionResult>
        {
            public UserVerificationData Verification { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Verification)
                    .NotNull().WithMessage("Payload has to contain a verification object")
                    .SetValidator(new UserVerificationDataValidator());
           }
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
                var person = await _context.Persons.SingleOrDefaultAsync(x => x.VerificationKey == message.Verification.VerificationKey, cancellationToken);
                
                // Check whether user exists in database
                if(person == null)
                    throw RestException.CreateFromDictionary(HttpStatusCode.NotFound, new Dictionary<string, string>
                    {
                        {"verification.key", $"Verification key '{ message.Verification.VerificationKey }' does not belong to any user"}
                    });

                // Compare verification keys from request and database
                if (person.VerificationKey.Equals(message.Verification.VerificationKey))
                {
                    // Set it to null upon match
                    person.VerificationKey = null;
                    await _context.SaveChangesAsync();

                    throw RestException.CreateFromDictionary(HttpStatusCode.OK, new Dictionary<string, string>
                    {
                        {"verification", $"Email for person with id '{ person.PersonId }' has been verified"}
                    });
                }
                else
                {
                    throw RestException.CreateFromDictionary(HttpStatusCode.BadRequest, new Dictionary<string, string>
                    {
                        {"verification.key", $"Verification key '{ message.Verification.VerificationKey }' is invalid"}
                    });
                }
            }
        }
    }
}
