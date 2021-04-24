var calendar = null;

function calendario() {

    let calendario = document.getElementById("calendar");
    calendar = new FullCalendar.Calendar(calendario, {

        locale: 'es',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
        },

        buttonIcons: true,
        weekNumbers: true,
        navLinks: true,
        dayMaxEvents: true,
        loading: function (bool) {
            document.getElementById('loading').style.display =
                bool ? 'block' : 'none';
        },

        dateClick: function (info) {
            $("#FechaInicio").val(info.dateStr);
            $("#modalCrearSoliciutd").modal();
        },

        eventClick: function (info) {


            $("#ClienteR").val(info.event.title);
            $("#FechaR").val(info.event.start);
            $("#FechaF").val(info.event.end);
            $("#modalSolicitudDetalle").modal();

            console.log(info.event.publicId);
        }

    });//fin Calendar

    calendar.render();

}//fin calendario


function guardar() {
    let formularioSolicitud = $("#formularioSolicitud").serialize();

    $.ajax({
        url: '/Solicitudes/Guardar',
        type: 'post',
        data: formularioSolicitud,
        dataType: 'json'
    }).done(function (respuesta) {

        if (respuesta.status) {
            listar();

            Swal.fire({
                icon: 'success',
                title: 'Solicitud de servicio guardada',
                showConfirmButton: false,
                timer: 1500
            })
        }
    })
}//fin Guardar


function eliminar() {
    $.ajax({
        url: '/Solicitudes/Eliminar',
        type: 'delete',
        dataType: 'json'
    })
}//fin eliminar

function listar() {
    $.ajax({
        url: '/Solicitudes/Listar',
        type: 'get',
        dataType: 'json'
    }).done(function (respuesta) {

        if (respuesta.status) {

            let data = [];
            respuesta.data.map(function (e) {

                data.push({

                    id: e.solicitudId,
                    title: e.nombreCliente,
                    start: moment(e.fechaInicio).format("YYYY-MM-DD") + " " + e.horaInicio,
                    end: moment(e.fechaFin).format("YYYY-MM-DD") + " " + e.horaFinal,
                    descripcion: e.descripcion
                });

            });
            console.log(data)
            calendar.removeAllEvents();
            refrescarCalendario(data)

        }
    })
}//fin listar

function refrescarCalendario(data) {
    calendar.addEventSource(data);
    calendar.refetchEvents();
}//fin refrescar