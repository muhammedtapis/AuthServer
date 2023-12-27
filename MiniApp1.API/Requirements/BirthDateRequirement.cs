using Microsoft.AspNetCore.Authorization;
using SharedLibrary.Exceptions;
using System.Security.Claims;

namespace MiniApp1.API.Requirements
{
    public class BirthDateRequirement : IAuthorizationRequirement
    {
        public int Age { get; set; }

        public BirthDateRequirement(int age)
        {
            Age = age;
        }

        public class BirthDateRequirementHandler : AuthorizationHandler<BirthDateRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BirthDateRequirement requirement)
            {
                var birthDateClaim = context.User.FindFirst(ClaimTypes.DateOfBirth);

                if (birthDateClaim == null)
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
                var today = DateTime.Now;
                var userAge = today.Year - Convert.ToDateTime(birthDateClaim.Value).Year;

                if (userAge >= requirement.Age)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }

                return Task.CompletedTask;
            }
        }
    }
}