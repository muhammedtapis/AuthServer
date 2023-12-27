using AuthServer.Core.DTOs;
using FluentValidation;

namespace AuthServer.API.Validations
{
    public class CreateUserDTOValidator : AbstractValidator<CreateUserDTO>
    {
        public CreateUserDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("email alanı boş bırakılamaz").EmailAddress().WithMessage("email formatı yanlış");

            RuleFor(x => x.Password).NotEmpty().WithMessage("şifre alanı boş bırakılamaz");

            RuleFor(x => x.Username).NotEmpty().WithMessage("kullanıcı adı alanı boş bırakılamaz");
        }
    }
}