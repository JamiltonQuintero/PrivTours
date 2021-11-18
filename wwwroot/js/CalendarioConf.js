var calendar = null;
var filtro = null;
var lTareas = new Array();
var count = 0;


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
  

        dateClick: function (info) {
            var now = new Date();
            if (info.date.setHours(0, 0, 0, 0) < now.setHours(0, 0, 0, 0)) {
                Swal.fire({
                    icon: 'error',
                    title: 'Recuerde que no puede seleccionar dias anteriores a la fecha actual.',
                    showConfirmButton: false,
                    timer: 2500
                })
            }
            else {
                $("#modalCrearSoliciutd").modal();            
            }
            
        },

        eventClick: function (info) {
            obtenerServicioPorId(info.event._def.publicId);
            $("#modalSolicitudDetalle").modal();

        }

    });//fin Calendar

    calendar.render();

}//fin calendario


function agregarTarea() {
    document.getElementById("formularioTarea").style.display = "block";
    $('#btnAgregarTareaC').hide()
}


function limpiarModal() {
    $("#modalCrearSoliciutd").on("hidden.bs.modal", function () {
        $("#d").html("")
        count = 0;
        lTareas = [];
        $('#btnAgregarTareaC').show()
        $("#FechaInicioTarea").val(moment("").format("YYYY-MM-DD"));
        $("#FechaFinTarea").val(moment("").format("YYYY-MM-DD"));
        $("#UsuarioIdentityId").val(0);
        $("#ServicioId").val(0);
        $("#DescripcionTarea").val("");
        $("#OperacionId").val(0);

        $("#FechaInicio").val(moment("").format("YYYY-MM-DD"));
        $("#FechaFin").val(moment("").format("YYYY-MM-DD"));
        $("#Cliente").val(0);
        $("#Servicio").val(0);
        $("#Descripcion").val("");
        $("#SolicitudId").val(0);
        $("#SolicitudEstado").val(0);
        $('#formularioTarea').hide()
    });

    $("#modalSolicitudDetalle").on("hidden.bs.modal", function () {
        $("#e").html("")
        count = 0;
        lTareas = [];
        $('#btnAgregarTarea').show()
        $("#FechaInicioTareaD").val(moment("").format("YYYY-MM-DD"));
        $("#FechaFinTareaD").val(moment("").format("YYYY-MM-DD"));
        $("#UsuarioIdentityIdD").val(0);
        $("#ServicioIdD").val(0);
        $("#DescripcionTareaD").val("");
        $("#OperacionIdD").val(0);
        $("#TareaIdD").val(0);
        $("#TareaSolicitudIdD").val(0);

        $("#FechaInicioTareaDA").val(moment("").format("YYYY-MM-DD"));
        $("#FechaFinTareaDA").val(moment("").format("YYYY-MM-DD"));
        $("#UsuarioIdentityIdDA").val(0);
        $("#ServicioIdDA").val(0);
        $("#DescripcionTareaDA").val("");
        $("#OperacionIdDA").val(0);
        $("#TareaSolicitudIdDA").val(0);
        $('#formularioTareaEditAgregar').hide()

        $("#FechaInicioD").val(moment("").format("YYYY-MM-DD"));
        $("#FechaFinD").val(moment("").format("YYYY-MM-DD"));
        $("#ClienteD").val(0);
        $("#ServicioD").val(0);
        $("#DescripcionD").val("");
        $("#SolicitudIdD").val(0);
        $("#SolicitudEstadoD").val(0);
        $('#formularioTareaEdit').hide()
    });
}


function guardarTarea() {
    count+=1;
    let tarea = {
        "Id": count,
        "FechaInicioTarea": document.getElementById('FechaInicioTarea').value,
        "FechaFinTarea": document.getElementById('FechaFinTarea').value,
        "DescripcionTarea": document.getElementById('DescripcionTarea').value,
        "UsuarioIdentityId": document.getElementById('UsuarioIdentityId').value,
        "OperacionId": document.getElementById('OperacionId').value
    }

    $("#d").append("<tr id=" + "tr" + count + ">" + "<td>" + $('#OperacionId option:selected').text() + "</td> <td>" + "<button onclick='eliminarTarea(" + tarea.Id+")'>Eliminar</button>" + "</td>" +
        "</tr>")
    lTareas.push(tarea)

    Swal.fire({
        icon: 'success',
        title: 'Tarea creada exitosamente',
        showConfirmButton: false,
        timer: 1500
    })
}

function crearTarea() {
    var empleadoId = document.getElementById('UsuarioIdentityId').value;
    validarDisponibilidadEmpleado(empleadoId, 1);
}

function validarDisponibilidadEmpleado(empleadoId, tipoGuardado ) {

    $.ajax({
        url: '/Tareas/ObtenerTareasPorEmpleadoId',
        data: jQuery.param({ empleadoId: empleadoId }),
        type: 'get',
        dataType: 'json',
    }).done(function (respuesta) {
        if (respuesta.status) {

            var continuar = true;
            respuesta.data.map(function (e) {

                var start = e.fechaInicioTarea;
                var end = e.fechaFinTarea;

                var startNew = null;
                var endNew = null;

                if (tipoGuardado == 1) {
                     startNew = $("#FechaInicioTarea").val();
                     endNew = $("#FechaFinTarea").val();
                } else if (tipoGuardado == 2) {
                     startNew = $("#FechaInicioTareaD").val();
                     endNew = $("#FechaFinTareaD").val();
                } else if (tipoGuardado == 3) {
                     startNew = $("#FechaInicioTareaDA").val();
                     endNew = $("#FechaFinTareaDA").val();
                }

                const test = moment(startNew).isBetween(start, end);
                const test2 = moment(endNew).isBetween(start, end);

                if (test || test2) {
                    continuar = false;
                }

            });

            if (continuar) {

                if (tipoGuardado == 1) {
                    guardarTarea()
                } else if (tipoGuardado == 2) {
                    editarTareaConfirm()
                } else if (tipoGuardado == 3) {
                    editarAgregarTareaConfirm()
                }
   
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'El empleado seleccionado no se encuentra disponible, por favor validar.',
                    showConfirmButton: false,
                    timer: 9000
                })
            }

        } else {

            Swal.fire({
                icon: 'error',
                title: 'No fue posible guardar la solicitud. Por favor intente nuevamente',
                showConfirmButton: false,
                timer: 1500
            })
        }
    })
}

function eliminarTarea(id) {
    var tarea = lTareas.find(s => s.Id = id);
    $("#tr" + id).remove()
    lTareas.pop(tarea)
}


function cerrarTareas() {
    $("#FechaInicioTarea").val(moment("").format("YYYY-MM-DD"));
    $("#FechaFinTarea").val(moment("").format("YYYY-MM-DD"));
    $("#UsuarioIdentityId").val(0);
    $("#DescripcionTarea").val("");
    $("#OperacionId").val(0);
    $('#formularioTarea').hide()
    $('#btnAgregarTareaC').show()
}

function guardar() {
    
                $.ajax({
                    url: '/Solicitudes/Guardar',
                    data: jQuery.param({
                        Descripcion: document.getElementById('Descripcion').value,
                        ClienteId: document.getElementById('ClienteId').value,
                        ServicioId: document.getElementById('ServicioId').value,
                        lTareas: lTareas
                    }),
                    type: 'post',
                    dataType: 'json',
                }).done(function (guardar) {

                    if (guardar.status) {
                        $("#modalCrearSoliciutd").modal('hide');
                        listar();
                        $("#Cliente").val(0);
                        $("#Servicio").val(0);
                        $("#Descripcion").val("");
                        $("#SolicitudId").val(0);
                        $("#SolicitudEstado").val(0);

                        Swal.fire({
                            icon: 'success',
                            title: 'Solicitud de servicio guardada',
                            showConfirmButton: false,
                            timer: 1500
                        })
                    } else {

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

    var formData = new FormData();
    formData.append("Descripcion", document.getElementById('DescripcionD').value);
    formData.append("ClienteId", document.getElementById('ClienteD').value);
    formData.append("ServicioId", document.getElementById('ServicioD').value);
    formData.append("EstadoSoliciud", document.getElementById('SolicitudEstadoD').value);
    formData.append("SolicitudId", document.getElementById('SolicitudIdD').value);
    $.ajax({
        url: '/Solicitudes/Edit',
        type: 'post',
        contentType: false,
        processData: false,
        data: formData,
    }).done(function (respuesta) {

        if (respuesta.status) {
            $("#modalSolicitudDetalle").modal('hide');
            listar();
            $("#FechaInicioD").val(moment("").format("YYYY-MM-DD"));
            $("#FechaFinD").val(moment("").format("YYYY-MM-DD"));
            $("#ClienteD").val(0);
            $("#ServicioD").val(0);
            $("#DescripcionD").val("");
            $("#SolicitudIdD").val(0);
            $("#SolicitudEstadoD").val(0);

            Swal.fire({
                icon: 'success',
                title: 'Solicitud de servicio editada',
                showConfirmButton: false,
                timer: 1500
            })
        } else {

            Swal.fire({
                icon: 'error',
                title: 'No fue posible guardar la solicitud. Por favor intente nuevamente',
                showConfirmButton: false,
                timer: 1500
            })
        }
    })
}//fin editar



function obtenerServicioPorId(id) {
    $.ajax({
        //url: '@Url.Action("Solicitudes","ObtenerDetalle", new { id = "id" })'.replace("id", encodeURIComponent(id)),
        url: '/Solicitudes/ObtenerDetalle',
        data: jQuery.param({ id: id}),
        type: 'get',
        dataType: 'json',
    }).done(function (respuesta) {
        if (respuesta.status) {       
            $("#ClienteD").val(respuesta.data.clienteId);
            $("#ServicioD").val(respuesta.data.servicioId);
            $("#DescripcionD").val(respuesta.data.descripcion);
            $("#SolicitudIdD").val(respuesta.data.solicitudId);
            $("#SolicitudEstadoD").val(respuesta.data.estadoSoliciud);

            for (var tarea = 0; tarea < respuesta.data.tareas.length; tarea++) {
                $("#e").append("<tr id=" + "tr" + respuesta.data.tareas[tarea].tareaId + ">" + "<td>" + respuesta.data.tareas[tarea].nombreOperacion + "</td> <td>" + "<button onclick='obtenerTareaporId(" + respuesta.data.tareas[tarea].tareaId + ")'>Editar</button>" +
                    "</td> <td>" + "<button onclick='eliminarTareaEditar(" + respuesta.data.tareas[tarea].tareaId + ")'>Eliminar</button>" + "</td>" +
        "</tr>")
            }
        } else {
            Swal.fire({
                icon: 'error',
                title: 'No fue posible obtenr la solicitud. Por favor intente nuevamente',
                showConfirmButton: false,
                timer: 1500
            })
        } 
    })
}//fin listar


function obtenerTareaporId(id) {
    $.ajax({
        url: '/Tareas/ObtenerTareaPorId',
        data: jQuery.param({ id: id }),
        type: 'get',
        dataType: 'json',
    }).done(function (respuesta) {
        if (respuesta.status) {
            editarViewTarea()
            $('#formularioTareaEditAgregar').hide()
            $("#TareaIdD").val(respuesta.data.tareaId);
            $("#FechaInicioTareaD").val(respuesta.data.fechaInicioTarea);
            $("#FechaFinTareaD").val(respuesta.data.fechaFinTarea);
            $("#DescripcionTareaD").val(respuesta.data.descripcionTarea);
            $("#OperacionIdD").val(respuesta.data.operacionId);
            $("#UsuarioIdentityIdD").val(respuesta.data.usuarioIdentityId); TareaSolicitudIdD
            $("#TareaSolicitudIdD").val(respuesta.data.solicitudId);
            $("#TareaEstadoD").val(respuesta.data.estadoTarea);
        } else {
            Swal.fire({
                icon: 'error',
                title: 'No fue posible obtenr la tarea. Por favor intente nuevamente',
                showConfirmButton: false,
                timer: 1500
            })
        }
    })
}

function eliminarTareaEditar(id) {
    $.ajax({
        url: '/Tareas/EliminarTareaPorId',
        data: jQuery.param({ id: id }),
        type: 'delete',
        dataType: 'json',
    }).done(function (respuesta) {
        if (respuesta.status) {

            $("#tr" + respuesta.data).remove()

            Swal.fire({
                icon: 'success',
                title: 'Tarea eliminada exitosamente',
                showConfirmButton: false,
                timer: 1500
            })

        } else {
            Swal.fire({
                icon: 'error',
                title: 'No fue posible obtenr la tarea. Por favor intente nuevamente',
                showConfirmButton: false,
                timer: 1500
            })
        }
    })
}

function editarTarea() {
    var empleadoId = document.getElementById('UsuarioIdentityIdD').value;
    validarDisponibilidadEmpleado(empleadoId,2);
}

function editarTareaConfirm() {
    var formData = new FormData();
    formData.append("TareaId", document.getElementById('TareaIdD').value) ;
    formData.append("FechaInicioTarea", document.getElementById('FechaInicioTareaD').value);
    formData.append("FechaFinTarea", document.getElementById('FechaFinTareaD').value);
    formData.append("DescripcionTarea", document.getElementById('DescripcionTareaD').value);
    formData.append("OperacionId", document.getElementById('OperacionIdD').value);
    formData.append("UsuarioIdentityId", document.getElementById('UsuarioIdentityIdD').value);
    formData.append("SolicitudId", document.getElementById('TareaSolicitudIdD').value);
    formData.append("EstadoTarea", document.getElementById('TareaEstadoD').value);
    
    $.ajax({
        url: '/Tareas/Edit',
        type: "POST",
        contentType: false,
        processData: false,
        data: formData,
    }).done(function (respuesta) {
        if (respuesta.status) {
            Swal.fire({
                icon: 'success',
                title: 'Tarea editada correctamente',
                showConfirmButton: false,
                timer: 1500
            })

        } else {
            Swal.fire({
                icon: 'error',
                title: 'No fue posible obtenr la tarea. Por favor intente nuevamente',
                showConfirmButton: false,
                timer: 1500
            })
        }
    })
}

function editarViewTarea() {
    document.getElementById("formularioTareaEdit").style.display = "block";
    $('#btnAgregarTarea').hide()
}

function cerrarTareasEditar() {
    $("#FechaInicioTareaD").val(moment("").format("YYYY-MM-DD"));
    $("#FechaFinTareaD").val(moment("").format("YYYY-MM-DD"));
    $("#UsuarioIdentityIdD").val(0);
    $("#DescripcionTareaD").val("");
    $("#OperacionIdD").val(0);
    $("#TareaSolicitudIdD").val(0);
    $("#TareaIdD").val(0);
    $('#formularioTareaEdit').hide()
    $('#btnAgregarTarea').show()
}

function editarAgregarTarea() {
    var empleadoId = document.getElementById('UsuarioIdentityIdDA').value;
    validarDisponibilidadEmpleado(empleadoId,3);
}

function editarAgregarTareaConfirm() {
    var formData = new FormData();
    formData.append("FechaInicioTarea", document.getElementById('FechaInicioTareaDA').value);
    formData.append("FechaFinTarea", document.getElementById('FechaFinTareaDA').value);
    formData.append("DescripcionTarea", document.getElementById('DescripcionTareaDA').value);
    formData.append("OperacionId", document.getElementById('OperacionIdDA').value);
    formData.append("UsuarioIdentityId", document.getElementById('UsuarioIdentityIdDA').value);
    formData.append("SolicitudId", document.getElementById('SolicitudIdD').value);
    $.ajax({
        url: '/Tareas/Guardar',
        type: "POST",
        contentType: false,
        processData: false,
        data: formData,
    }).done(function (respuesta) {
        if (respuesta.status) {

            $("#e").append("<tr id=" + "tr" + respuesta.data.tareaId + ">" + "<td>" + respuesta.data.nombreOperacion + "</td> <td>" + "<button onclick='obtenerTareaporId(" + respuesta.data.tareaId + ")'>Editar</button>" +
                "</td> <td>" + "<button onclick='eliminarTareaEditar(" + respuesta.data.tareaId + ")'>Eliminar</button>" + "</td>" +
                "</tr>")

            Swal.fire({
                icon: 'success',
                title: 'Tarea guardada correctamente',
                showConfirmButton: false,
                timer: 1500
            })

        } else {
            Swal.fire({
                icon: 'error',
                title: 'No fue posible obtenr la tarea. Por favor intente nuevamente',
                showConfirmButton: false,
                timer: 1500
            })
        }
    })
}

function editarAgregarViewTarea() {
    document.getElementById("formularioTareaEditAgregar").style.display = "block";
    $('#btnAgregarTarea').hide()
}


function cerrarTareasEditarAgregar() {
    $("#FechaInicioTareaDA").val(moment("").format("YYYY-MM-DD"));
    $("#FechaFinTareaDA").val(moment("").format("YYYY-MM-DD"));
    $("#UsuarioIdentityIdDA").val(0);
    $("#TareaSolicitudIdDA").val(0);
    $("#DescripcionTareaDA").val("");
    $("#OperacionIdDA").val(0);
    $('#formularioTareaEditAgregar').hide()
    $('#btnAgregarTarea').show()
}

function listar() {
    $.ajax({
        url: '/Solicitudes/Listar',
        type: 'get',
        dataType: 'json'
    }).done(function (respuesta) {

        if (respuesta.status) {

            let data = [];

            respuesta.data.map(function (e) {
                console.log(e)
                var titulo = 'Cliente: ' + e.cliente.nombre + ' ' + e.cliente.apellidos + ', ' + tipoDocumento(e.cliente.idTipoDoc) + ': ' +  e.cliente.numDoc;
                data.push({
                    id: e.solicitudId,
                    title: titulo,
                    start: e.fechaInicio,
                    end: e.fechaFin,
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


function start() {
    document.getElementById('TipoDeBusqueda').addEventListener('change', function () {
        filtroSeleccionado(this.value)
    });
}

async function filtroSeleccionado(tipoFiltro) {
    this.filtro = tipoFiltro;

    if (tipoFiltro === "1") {
        $('#TipoDeBusquedaSeleccionado').find('option').remove().end()
    }else if (tipoFiltro === "2") {
        $.ajax({
            url: '/Solicitudes/ObtenerListaEmpleados',
            type: 'get',
            dataType: 'json',
        }).done(function (respuesta) {
            if (respuesta.status) {
                $('#TipoDeBusquedaSeleccionado').find('option').remove().end()
                newOptionsSelect = '';
                respuesta.data.forEach(empleado => {
                    newOptionsSelect = newOptionsSelect + '<option value="' + empleado.id + '">' + empleado.nombre + '</option>';
                })
                $('#TipoDeBusquedaSeleccionado').append(newOptionsSelect);
            }
        })
    } else if (tipoFiltro === "3") {
        $.ajax({
            url: '/Solicitudes/ObtenerListaClientes',
            type: 'get',
            dataType: 'json',
        }).done(function (respuesta) {
            if (respuesta.status) {
                $('#TipoDeBusquedaSeleccionado').find('option').remove().end()
                newOptionsSelect = '';

                respuesta.data.forEach(cliente => {
                    newOptionsSelect = newOptionsSelect + '<option value="' + cliente.clienteId + '">' + cliente.nombre + '</option>';
                })
                $('#TipoDeBusquedaSeleccionado').append(newOptionsSelect);
            }
        })
    } else if (tipoFiltro === "4") {
        $.ajax({
            url: '/Solicitudes/ObtenerListaServicios',
            type: 'get',
            dataType: 'json',
        }).done(function (respuesta) {
            if (respuesta.status) {
                $('#TipoDeBusquedaSeleccionado').find('option').remove().end()
                newOptionsSelect = '';

                respuesta.data.forEach(servicio => {
                    newOptionsSelect = newOptionsSelect + '<option value="' + servicio.servicioId + '">' + servicio.nombre + '</option>';
                })
                $('#TipoDeBusquedaSeleccionado').append(newOptionsSelect);
            }
        })
    } else if (tipoFiltro === "5") {

        var listaEstados = [
            { estadoId: 1, nombre: "RESERVADO" },
            { estadoId: 2, nombre: "EN PROCESO" },
            { estadoId: 3, nombre: "VENCIDO" },
            { estadoId: 4, nombre: "CANCELADO" },
            { estadoId: 5, nombre: "FINALIZADO EMPLEADO" },
            { estadoId: 6, nombre: "FINALIZADO ADMINISTRADOR"}
        ]

        $('#TipoDeBusquedaSeleccionado').find('option').remove().end()
        newOptionsSelect = '';

        listaEstados.forEach(estado => {
            newOptionsSelect = newOptionsSelect + '<option value="' + estado.estadoId + '">' + estado.nombre + '</option>';
        })
        $('#TipoDeBusquedaSeleccionado').append(newOptionsSelect);

    }  
}

function filtar() {

    if (this.filtro == "1") {
        listar();
    } else if (this.filtro == "2") {
        $.ajax({
            url: '/Solicitudes/ObtenerListaSolicitudesPorEmpleado/',
            data: jQuery.param({ id: document.getElementById('TipoDeBusquedaSeleccionado').value }),
            type: 'get',
            dataType: 'json',
        }).done(function (respuesta) {
            if (respuesta.status) {
                let data = [];
                respuesta.data.map(function (e) {
                    console.log(e)
                    var titulo = 'Empleado: ' + e.usuarioIdentity.nombre + ' ' + e.usuarioIdentity.apellido;
                    data.push({
                        id: e.solicitudId,
                        title: titulo,
                        start: e.fechaInicio,
                        end: e.fechaFin,
                        descripcion: e.descripcion
                    });

                });

                if (!data || data.length == 0) {
                    var titulo = 'El Empleado seleccionado, no tiene servicios programados';
                    Swal.fire({
                        icon: 'warning',
                        title: titulo,
                        showConfirmButton: false,
                        timer: 3000
                    })
                }
                calendar.removeAllEvents();
                refrescarCalendario(data);
            }
        })
    } else if (this.filtro == "3") {
        $.ajax({
            url: '/Solicitudes/ObtenerListaSolicitudesPorCliente/',
            data: jQuery.param({ clienteId: document.getElementById('TipoDeBusquedaSeleccionado').value }),
            type: 'get',
            dataType: 'json',
        }).done(function (respuesta) {
            if (respuesta.status) {
                let data = [];
                respuesta.data.map(function (e) {
                    console.log(e)
                    var titulo = 'Cliente: ' + e.cliente.nombre + ' ' + e.cliente.apellidos + ', Documento: ' + e.cliente.numDoc;
                    data.push({
                        id: e.solicitudId,
                        title: titulo,
                        start: e.fechaInicio,
                        end: e.fechaFin,
                        descripcion: e.descripcion
                    });

                });

                if (!data || data.length == 0) {
                    var titulo = 'El Cliente seleccionado, no tiene servicios programados';
                    Swal.fire({
                        icon: 'warning',
                        title: titulo,
                        showConfirmButton: false,
                        timer: 3000
                    })
                }
                calendar.removeAllEvents();
                refrescarCalendario(data);
            }
        })
    } else if (this.filtro == "4") {
        $.ajax({
            url: '/Solicitudes/ObtenerListaSolicitudesPorServicio/',
            data: jQuery.param({ servicioId: document.getElementById('TipoDeBusquedaSeleccionado').value }),
            type: 'get',
            dataType: 'json',
        }).done(function (respuesta) {
            if (respuesta.status) {
                let data = [];
                respuesta.data.map(function (e) {
                    console.log(e)
                    var titulo = 'Servicio: ' + e.servicio.nombre + ', Descripción: ' + e.servicio.descripcion;
                    data.push({
                        id: e.solicitudId,
                        title: titulo,
                        start: e.fechaInicio,
                        end:e.fechaFin,
                        descripcion: e.descripcion
                    });

                });

                if (!data || data.length == 0) {
                    var titulo = 'El Servicio seleccionado, no tiene registros';
                    Swal.fire({
                        icon: 'warning',
                        title: titulo,
                        showConfirmButton: false,
                        timer: 3000
                    })
                }
                calendar.removeAllEvents();
                refrescarCalendario(data);
            }
        })
    } else if (this.filtro == "5") {
        $.ajax({
            url: '/Solicitudes/ObtenerListaSolicitudesPorEstado/',
            data: jQuery.param({ estado: document.getElementById('TipoDeBusquedaSeleccionado').value }),
            type: 'get',
            dataType: 'json',
        }).done(function (respuesta) {
            if (respuesta.status) {
                let data = [];
                respuesta.data.map(function (e) {

                    var titulo = 'Cliente: ' + e.cliente.nombre + ' ' + e.cliente.apellidos + ', Estado: ' + nombteEstado(e.estadoSolicitud);
                    data.push({
                        id: e.solicitudId,
                        title: titulo,
                        start:e.fechaInicio,
                        end: e.fechaFin,
                        descripcion: e.descripcion
                    });

                });

                if (!data || data.length == 0) {
                    var titulo = 'El Estado seleccionado, no tiene registros';
                    Swal.fire({
                        icon: 'warning',
                        title: titulo,
                        showConfirmButton: false,
                        timer: 3000
                    })
                }
                calendar.removeAllEvents();
                refrescarCalendario(data);
            }
        })
    }
}

function nombteEstado(estadoEnum) {
    var nombreEstado = ""

    if (estadoEnum == 1) {
        nombreEstado = "RESERVADO";
    } else if (estadoEnum == 2) {
        nombreEstado = "EN PROCESO";
    } else if (estadoEnum == 3) {
        nombreEstado = "VENCIDO";
    }
    else if (estadoEnum == 4) {
        nombreEstado = "CANCELADO";
    }
    else if (estadoEnum == 5) {
        nombreEstado = "FINALIZADO EMPLEADO";
    }
    else if (estadoEnum == 6) {
        nombreEstado = "FINALIZADO ADMINISTRADOR";
    }
    
    return nombreEstado;
}

function tipoDocumento(tipoDocumento) {
    var nombreDocumento = ""

    if (tipoDocumento == 1) {
        nombreDocumento = "Cedula";
    } else if (tipoDocumento == 2) {
        nombreDocumento = "Cedula extranjera";
    } else if (tipoDocumento == 3) {
        nombreDocumento = "Pasaporte";
    }

    return nombreDocumento;
}






