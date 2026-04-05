using System;
using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;

namespace PlataformaReservas.Aplicacion.Validaciones;


public class LoginUsuarioDtoValidator : AbstractValidator<LoginUsuarioDto>
{
    public LoginUsuarioDtoValidator()
    {
        RuleFor(x=>x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email no es válido");

        RuleFor(x=>x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");
    }
}
        
