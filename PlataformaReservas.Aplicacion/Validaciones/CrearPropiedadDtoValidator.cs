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


        RuleFor(x=> x.Ubicacion)
            .NotEmpty().WithMessage("La ubicacion no puede estar vacia")
            .MinimumLength(3).WithMessage("La ubicacion debe tener al menos 3 caracteres");

        
        RuleFor(x=> x.Descripcion)
            .NotEmpty().WithMessage("La descripcion no puede estar vacia")
            .MinimumLength(15).WithMessage("La descripcion debe tener al menos 15 caracteres");

        RuleFor(x=> x.Capacidad)
            .GreaterThan(0).WithMessage("La capacidad debe ser mayor a 0");


        RuleFor(x=> x.PrecioPorNoche)
            .GreaterThan(0).WithMessage("El precio por noche debe ser mayor a 0");

        RuleFor(x=> x.HostId)
            .GreaterThan(0).WithMessage("El HostId debe ser mayor a 0");
    }
}
