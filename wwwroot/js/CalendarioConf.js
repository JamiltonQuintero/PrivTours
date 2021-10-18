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
            obtenerServicioPorId(info.event._def.publicId);
            $("#modalSolicitudDetalle").modal();

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
            $("#modalCrearSoliciutd").modal('hide');
            listar();
            $("#FechaInicio").val(moment("").format("YYYY-MM-DD"));
            $("#HoraInicio").val(0);
            $("#FechaFin").val(moment("").format("YYYY-MM-DD"));
            $("#HoraFinal").val(0);
            $("#Cliente").val(0);
            $("#Empleado").val(0);
            $("#Servicio").val(0);
            $("#Descripcion").val("");

            Swal.fire({
                icon: 'success',
                title: 'Solicitud de servicio guardada',
                showConfirmButton: false,
                timer: 1500
            })
        } else {
            /*
            listar();
            $("#FechaInicio").val(moment("").format("YYYY-MM-DD"));
            $("#HoraInicio").val(0);
            $("#FechaFin").val(moment("").format("YYYY-MM-DD"));
            $("#HoraFinal").val(0);
            $("#Cliente").val(0);
            $("#Empleado").val(0);
            $("#Servicio").val(0);
            $("#Descripcion").val("");
            */
            Swal.fire({
                icon: 'error',
                title: 'No fue posible guardar la solicitud. Por favor intente nuevamente',
                showConfirmButton: false,
                timer: 1500
            })
        }
    })
}//fin Guardar

function editar() {
    let formularioSolicitudEditar = $("#formularioSolicitudEditar").serialize();

    $.ajax({
        url: '/Solicitudes/Edit',
        type: 'post',
        data: formularioSolicitudEditar,
        dataType: 'json'
    }).done(function (respuesta) {

        if (respuesta.status) {
            $("#modalSolicitudDetalle").modal('hide');
            listar();
            $("#FechaInicioD").val(moment("").format("YYYY-MM-DD"));
            $("#HoraInicioD").val(0);
            $("#FechaFinD").val(moment("").format("YYYY-MM-DD"));
            $("#HoraFinalD").val(0);
            $("#ClienteD").val(0);
            $("#EmpleadoD").val(0);
            $("#ServicioD").val(0);
            $("#DescripcionD").val("");
            $("#SolicitudIdD").val(0);

            Swal.fire({
                icon: 'success',
                title: 'Solicitud de servicio editada',
                showConfirmButton: false,
                timer: 1500
            })
        } else {
            /*
            listar();
            $("#FechaInicio").val(moment("").format("YYYY-MM-DD"));
            $("#HoraInicio").val(0);
            $("#FechaFin").val(moment("").format("YYYY-MM-DD"));
            $("#HoraFinal").val(0);
            $("#Cliente").val(0);
            $("#Empleado").val(0);
            $("#Servicio").val(0);
            $("#Descripcion").val("");
            */
            Swal.fire({
                icon: 'error',
                title: 'No fue posible guardar la solicitud. Por favor intente nuevamente',
                showConfirmButton: false,
                timer: 1500
            })
        }
    })
}//fin editar


function eliminar() {
    $.ajax({
        url: '/Solicitudes/Eliminar',
        type: 'delete',
        dataType: 'json'
    })
}//fin eliminar

function obtenerServicioPorId(id) {
    $.ajax({
        //url: '@Url.Action("Solicitudes","ObtenerDetalle", new { id = "id" })'.replace("id", encodeURIComponent(id)),
        url: '/Solicitudes/ObtenerDetalle/',
        data: jQuery.param({ id: id}),
        type: 'get',
        dataType: 'json',
    }).done(function (respuesta) {
        if (respuesta) {       
            $("#FechaInicioD").val(moment(respuesta.data.fechaInicio).format("YYYY-MM-DD"));
            $("#HoraInicioD").val(respuesta.data.horaInicio);
            $("#FechaFinD").val(moment(respuesta.data.fechaFin).format("YYYY-MM-DD"));
            $("#HoraFinalD").val(respuesta.data.horaFinal);
            $("#ClienteD").val(respuesta.data.clienteId);
            $("#EmpleadoD").val(respuesta.data.empleadoId);
            $("#ServicioD").val(respuesta.data.servicioId);
            $("#DescripcionD").val(respuesta.data.descripcion);
            $("#SolicitudIdD").val(respuesta.data.solicitudId);
            

        }
    })
}//fin listar

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
            calendar.removeAllEvents();
            refrescarCalendario(data)

        }
    })
}//fin listar

function refrescarCalendario(data) {
    calendar.addEventSource(data);
    calendar.refetchEvents();
}//fin refrescar