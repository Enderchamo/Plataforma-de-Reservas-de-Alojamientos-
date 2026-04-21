using System;

namespace PlataformaReservas.Aplicacion.Helpers
{
    public static class PlantillasEmail
    {
        public static string ObtenerPlantillaBase(string titulo, string mensaje, string textoBoton = "Ver Detalles", string urlBoton = "http://localhost:5173")
        {
            return $@"
            <div style='margin: 0; padding: 0; background-color: #f4f4f4; font-family: Arial, sans-serif;'>
                <table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: #f4f4f4; padding: 40px 0;'>
                    <tr>
                        <td align='center'>
                            <table border='0' cellpadding='0' cellspacing='0' width='600' style='background-color: #ffffff; border-radius: 16px; overflow: hidden; border: 1px solid #dddddd; box-shadow: 0 4px 12px rgba(0,0,0,0.05);'>
                                
                                <tr>
                                    <td align='center' style='background-color: #ff385c; padding: 40px 20px;'>
                                        <h1 style='color: #ffffff; margin: 0; font-size: 32px; font-weight: bold;'>Air reservas</h1>
                                    </td>
                                </tr>

                                <tr>
                                    <td style='padding: 40px; color: #222222;'>
                                        <h2 style='margin-top: 0; font-size: 22px; font-weight: bold;'>{titulo}</h2>
                                        
                                        <div style='margin-top: 20px; padding: 20px; background-color: #fff8f9; border-left: 4px solid #ff385c; border-radius: 8px;'>
                                            <p style='margin: 0; font-size: 16px; line-height: 1.6; color: #484848;'>
                                                {mensaje}
                                            </p>
                                        </div>

                                        <div style='margin-top: 35px; text-align: center;'>
                                            <a href='{urlBoton}' style='background-color: #ff385c; color: #ffffff; padding: 14px 28px; text-decoration: none; border-radius: 10px; font-weight: bold; display: inline-block;'>
                                                {textoBoton}
                                            </a>
                                        </div>
                                    </td>
                                </tr>

                                <tr>
                                    <td style='padding: 25px; background-color: #f7f7f7; text-align: center; border-top: 1px solid #eeeeee;'>
                                        <p style='margin: 0; font-size: 12px; color: #717171;'>© {DateTime.Now.Year} Air reservas - Proyecto Final ITLA</p>
                                        <p style='margin: 5px 0 0 0; font-size: 11px; color: #b0b0b0;'>Victor Manuel Contreras Aracena (2025-1135)</p>
                                    </td>
                                </tr>

                            </table>
                        </td>
                    </tr>
                </table>
            </div>";
        }
    }
}