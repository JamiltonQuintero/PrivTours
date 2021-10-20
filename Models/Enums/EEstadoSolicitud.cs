using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivTours.Models.Enums
{
    public enum EEstadoSolicitud
    {
        RESERVADO = 1,
        EN_PROCESO = 2,
        VENCIDO = 3,
        CANCELADO = 4,
        FINALIZADO_EMPLEADO = 5,
        FINALIZADO_ADMIN = 6,

    }
}
