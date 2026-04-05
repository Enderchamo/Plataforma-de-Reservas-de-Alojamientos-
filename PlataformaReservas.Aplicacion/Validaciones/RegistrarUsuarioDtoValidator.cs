using System;
using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;

namespace PlataformaReservas.Aplicacion.Validaciones;


public class RegistrarUsuarioDtoValidator : AbstractValidator<RegistrarUsuarioDto>
{
    public RegistrarUsuarioDtoValidator()
    {
        RuleFor(x=>x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x=>x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email no es válido");


        RuleFor(x=>x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");
        
        RuleFor(x => x)
            .Must(x => x.EsHost || x.EsGuest)
            .WithMessage("Debes seleccionar al menos un rol (Host o Guest) para registrarte.");
    }
}
        

