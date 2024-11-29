using FluentEmail.Core;
using FluentValidation;
using System.Text.Json.Serialization;
using UserManagerDev.Database.Entities;
using UserManagerDev.Endpoints;
using UserManagerDev.Helpers;

namespace UserManagerDev.Users
{
    public static class CreateUser
    {
        public record Request(
            [property: JsonPropertyName("username")] string UserName,
            [property: JsonPropertyName("password")] string Password,
            [property: JsonPropertyName("email")] string Email);
        public record Response(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("jwt")] string Jwt);
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.UserName).NotEmpty().NotNull().MaximumLength(50).Matches(Regexs.Username);
                RuleFor(r => r.Password).NotEmpty().NotNull().MaximumLength(100).Matches(Regexs.Password);
                RuleFor(r => r.Email).NotEmpty().NotNull().MaximumLength(100).Matches(Regexs.Email);
            }
        }
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("api/users", Handler).WithTags("Users");
            }
        }
        public static async Task<IResult> Handler(Request request, IFluentEmail fluentEmail, TokenProvider tokenProvider, IUserRepository userRepository, IPasswordHasher passwordHasher, IValidator<Request> validator, ApplicationContext context)
        {
            if (userRepository.ExistsAsync(request.Email, context).Result) return Results.BadRequest("The email is already in use");
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = request.UserName,
                Email = request.Email,
                PasswordHash = passwordHasher.Hash(request.Password)
            };

            await userRepository.InsertAsync(user, context);

            await fluentEmail
                .To(user.Email)
                .Subject("Email verification from MicroVik")
                .Body("To verify your email address click here")
                .SendAsync();

            var token = tokenProvider.Create(user);
            return Results.Ok(new Response(user.UserId.ToString(), token));
        }

    }
}
