using Microsoft.AspNetCore.Identity;

namespace SobMedidaApi.Services
{
    public class PortugueseIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() => new IdentityError
        {
            Code = nameof(DefaultError),
            Description = "Ocorreu um erro desconhecido."
        };

        public override IdentityError ConcurrencyFailure() => new IdentityError
        {
            Code = nameof(ConcurrencyFailure),
            Description = "Falha de concorrência, o objeto foi modificado."
        };

        public override IdentityError PasswordMismatch() => new IdentityError
        {
            Code = nameof(PasswordMismatch),
            Description = "Senha incorreta."
        };

        public override IdentityError InvalidToken() => new IdentityError
        {
            Code = nameof(InvalidToken),
            Description = "Token inválido."
        };

        public override IdentityError LoginAlreadyAssociated() => new IdentityError
        {
            Code = nameof(LoginAlreadyAssociated),
            Description = "Já existe um usuário com este login."
        };

        public override IdentityError InvalidUserName(string? userName) => new IdentityError
        {
            Code = nameof(InvalidUserName),
            Description = $"O nome de usuário '{userName}' é inválido, pode conter apenas letras ou dígitos."
        };

        public override IdentityError InvalidEmail(string? email) => new IdentityError
        {
            Code = nameof(InvalidEmail),
            Description = $"O e-mail '{email}' é inválido."
        };

        public override IdentityError DuplicateUserName(string userName) => new IdentityError
        {
            Code = nameof(DuplicateUserName),
            Description = $"O nome de usuário '{userName}' já está em uso."
        };

        public override IdentityError DuplicateEmail(string email) => new IdentityError
        {
            Code = nameof(DuplicateEmail),
            Description = $"O e-mail '{email}' já está em uso."
        };

        public override IdentityError InvalidRoleName(string? role) => new IdentityError
        {
            Code = nameof(InvalidRoleName),
            Description = $"O nome do perfil '{role}' é inválido."
        };

        public override IdentityError DuplicateRoleName(string role) => new IdentityError
        {
            Code = nameof(DuplicateRoleName),
            Description = $"O nome do perfil '{role}' já está em uso."
        };

        public override IdentityError UserAlreadyHasPassword() => new IdentityError
        {
            Code = nameof(UserAlreadyHasPassword),
            Description = "O usuário já possui uma senha definida."
        };

        public override IdentityError UserLockoutNotEnabled() => new IdentityError
        {
            Code = nameof(UserLockoutNotEnabled),
            Description = "O bloqueio não está habilitado para este usuário."
        };

        public override IdentityError UserAlreadyInRole(string role) => new IdentityError
        {
            Code = nameof(UserAlreadyInRole),
            Description = $"O usuário já está no perfil '{role}'."
        };

        public override IdentityError UserNotInRole(string role) => new IdentityError
        {
            Code = nameof(UserNotInRole),
            Description = $"O usuário não está no perfil '{role}'."
        };

        public override IdentityError PasswordTooShort(int length) => new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = $"A senha deve ter pelo menos {length} caracteres."
        };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => new IdentityError
        {
            Code = nameof(PasswordRequiresUniqueChars),
            Description = $"A senha deve conter pelo menos {uniqueChars} caracteres distintos."
        };

        public override IdentityError PasswordRequiresNonAlphanumeric() => new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "A senha deve ter pelo menos um caractere não alfanumérico."
        };

        public override IdentityError PasswordRequiresDigit() => new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "A senha deve ter pelo menos um número ('0'-'9')."
        };

        public override IdentityError PasswordRequiresLower() => new IdentityError
        {
            Code = nameof(PasswordRequiresLower),
            Description = "A senha deve ter pelo menos uma letra minúscula ('a'-'z')."
        };

        public override IdentityError PasswordRequiresUpper() => new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "A senha deve ter pelo menos uma letra maiúscula ('A'-'Z')."
        };
    }
}