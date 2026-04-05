using System;
using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;


namespace PlataformaReservas.Aplicacion.Validaciones;

public class BloquearFechaDtoValidator : AbstractValidator<BloquearFechaDto>
{
    public BloquearFechaDtoValidator()
    {
    
        RuleFor(x => x.PropiedadId)
            .GreaterThan(0).WithMessage("El PropiedadId debe ser mayor a 0");
        
        RuleFor(x => x.FechaInicio)
            .NotEmpty().WithMessage("La fecha de inicio no puede estar vacia")
            .Must((dto, fechaInicio) => fechaInicio.Date >= DateTime.Today);
        
        RuleFor(x => x.FechaFin)
            .NotEmpty().WithMessage("La fecha de fin no puede estar vacia")
            .Must((dto, fechaFin) => fechaFin.Date > dto.FechaInicio.Date);
    }
}
