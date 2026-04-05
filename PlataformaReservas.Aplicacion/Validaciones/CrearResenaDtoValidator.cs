using System;
using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;

namespace PlataformaReservas.Aplicacion.Validaciones;


public class CrearResenaDtoValidator : AbstractValidator<CrearResenaDto>
{
    public CrearResenaDtoValidator()
    {
        RuleFor(x=>x.ReservaId)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");    

        RuleFor(x=> x.Calificacion )
            .InclusiveBetween(1, 5).WithMessage("La calificación debe estar entre 1 y 5");

        RuleFor(x=> x.Comentario )
            .NotEmpty().WithMessage("El comentario no puede estar vacío")
            .MaximumLength(200).WithMessage("El comentario no puede tener más de 200 caracteres");
    }
}