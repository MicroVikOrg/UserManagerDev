using FluentValidation;
using System.Text.Json.Serialization;
using UserManagerDev.Database.Entities;
using UserManagerDev.Endpoints;
using UserManagerDev.Helpers;

namespace UserManagerDev.Users
{
    public class LoginUser
    {
        public record Request(
            [property: JsonPropertyName("email")] string Email,
            [property: JsonPropertyName("password")] string Password);
        public record Response(
            [property: JsonPropertyName("jwt")] string Jwt);

        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.Email).NotEmpty().NotNull().Matches(Regexs.Email);
                RuleFor(r => r.Password).NotNull().NotEmpty().Matches(Regexs.Password);
            }
        }
        public sealed class EndPoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("api/users/login", Handler).WithTags("Users");
            }
        }
        public static async Task<IResult> Handler(Request request, IUserRepository userRepository, IPasswordHasher passwordHasher, TokenProvider tokenProvider, ApplicationContext context)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, context);
            if (user == null) return Results.BadRequest("The user was not found");

            bool verified = passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!verified) return Results.BadRequest("The password is incorrect");

            var token = tokenProvider.Create(user);

            return Results.Ok(new Response(token));
        }
    }
}
