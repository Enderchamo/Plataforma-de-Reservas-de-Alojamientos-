using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;

namespace PlataformaReservas.Aplicacion.Validaciones;

public class ActualizarPropiedadDtoValidator : AbstractValidator<ActualizarPropiedadDto>
{
    public ActualizarPropiedadDtoValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El nombre no puede estar vacío")
            .MinimumLength(5).WithMessage("El nombre debe tener al menos 5 caracteres");

        RuleFor(x => x.Ubicacion)
            .NotEmpty().WithMessage("La ubicación no puede estar vacía")
            .MinimumLength(10).WithMessage("La ubicación debe tener al menos 10 caracteres");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción no puede estar vacía")
            .MinimumLength(20).WithMessage("La descripción debe tener al menos 20 caracteres");

        RuleFor(x => x.Capacidad)
            .GreaterThan(0).WithMessage("La capacidad debe ser mayor a 0");

        RuleFor(x => x.PrecioPorNoche)
            .GreaterThan(0).WithMessage("El precio por noche debe ser mayor a 0");
    }
}