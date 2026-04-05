using System;
using FluentValidation;
using PlataformaReservas.Aplicacion.DTOs;

namespace PlataformaReservas.Aplicacion.Validaciones;


public class CrearReservaDtoValidator : AbstractValidator<CrearReservaDto>
{
    public CrearReservaDtoValidator()
    {
        RuleFor(x=>x.PropiedadId)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");    
        
        RuleFor(x=> x.UsuarioInvitadoId )
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");


        RuleFor(x=> x.FechaEntrada)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("La fecha de inicio debe ser mayor a la fecha actual"); 

        RuleFor(x=> x.FechaSalida)    
            .GreaterThan(x => x.FechaEntrada).WithMessage("La fecha de salida debe ser mayor a la fecha de entrada");
    }
}