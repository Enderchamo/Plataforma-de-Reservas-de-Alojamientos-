using System;
using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;

namespace PlataformaReservas.Aplicacion.Validaciones;

public class CrearPropiedadDtoValidator : AbstractValidator<CrearPropiedadDto>
{
    public CrearPropiedadDtoValidator()
    {
        
        RuleFor(x=> x.Titulo)
            .NotEmpty().WithMessage("El nombre no puede estar vacio")
            .MinimumLength(5).WithMessage("El nombre debe tener al menos 5 caracteres");


        RuleFor(x=> x.Direccion)
            .NotEmpty().WithMessage("La direccion no puede estar vacia")
            .MinimumLength(10).WithMessage("La direccion debe tener al menos 10 caracteres");

        
        RuleFor(x=> x.PrecioPorNoche)
            .GreaterThan(0).WithMessage("El precio por noche debe ser mayor a 0");
    }
}
