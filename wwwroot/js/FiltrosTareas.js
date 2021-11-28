var filtro = null;

async function filtroSeleccionado(tipoFiltro) {
    this.filtro = tipoFiltro;

    if (tipoFiltro === "1") {
        $('#fechasFiltros').hide()
        $('#tipoBusquedaSelect').show()
        $('#TipoDeBusquedaSeleccionado').find('option').remove().end()
    } else if (tipoFiltro === "3") {
        $('#fechasFiltros').hide()
        $('#tipoBusquedaSelect').show()
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
        $('#fechasFiltros').hide()
        $('#tipoBusquedaSelect').show()
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

        $('#fechasFiltros').hide()
        $('#tipoBusquedaSelect').show()

        var listaEstados = [
            { estadoId: 1, nombre: "RESERVADA" },
            { estadoId: 2, nombre: "INICIADA" },
            { estadoId: 3, nombre: "VENCIDA" }
        ]

        $('#TipoDeBusquedaSeleccionado').find('option').remove().end()
        newOptionsSelect = '';

        listaEstados.forEach(estado => {
            newOptionsSelect = newOptionsSelect + '<option value="' + estado.estadoId + '">' + estado.nombre + '</option>';
        })
        $('#TipoDeBusquedaSeleccionado').append(newOptionsSelect);

    } else if (tipoFiltro === "6") {
        $('#fechasFiltros').show()
        $('#tipoBusquedaSelect').hide()

    } else if (tipoFiltro === "7") {
        $('#fechasFiltros').show()
        $('#tipoBusquedaSelect').hide()
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

            }
        })
    } else if (this.filtro == "5") {
        $.ajax({
            url: '/Tareas/ObtenerListaTareasPorEstado/',
            data: jQuery.param({ estado: document.getElementById('TipoDeBusquedaSeleccionado').value }),
            type: 'get',
            dataType: 'json',
        }).done(function (respuesta) {
            if (respuesta.status) {
                $("#cardTarea").html("")
                respuesta.data.map(function (e) {

                    var estado = "";
                    if (e.estadoTarea == 1) {
                        estado = "RESERVADA"
                    }
                    else if (e.estadoTarea == 2) {
                        estado = "INICIADA"
                    }
                    else if (e.estadoTarea == 3) {
                        estado = "VENCIDA"
                    }
                    else if (e.estadoTarea == 4) {
                        estado = "CANCELADA"
                    }
                    else if (e.estadoTarea == 5) {
                        estado = "FINALIZADA EMPLEADO"
                    }
                    else if (e.estadoTarea == 6) {
                        estado = "FINALIZADA ADMIN"
                    }

                    $("#cardTarea").append(

                        "<div class='courses-container'>" +
                        "<div class= 'course'>" +
                        "<div class='course-preview'>" +

                        "<h4 class='card-subtitle mb-2 text-muted'>Nombre cliente :  </h4>" +
                        "<p>" + e.cliente.nombre + " " + e.cliente.apellidos + "</p>" +
                        "<h4 class='card-subtitle mb-2 text-muted'>Telefono cliente</h4>" +
                        "<p>" + e.cliente.telefono + "</p>" +

                        "<h4 class='ard-subtitle mb-2 text-muted'>Nombre tarea :  </h4>" +
                        "<p>" + e.operacion.nombre + "</p>" +
                        "<h4 class='card-subtitle mb-2 text-muted'>Descripcion tarea :  </h4>" +
                        "<p>" + e.descripcionTarea + "</p>" +


                        "</div>" +
                        "<div class='course-info'>" +
                        "<div class='progress-container'>" +
                        "<div class='progress'></div>" +

                        "<span class='progress-text'>" +

                        "<a>" + estado + "</a>" +

                        "</span>" +
                        "</div>" +
                        "<h6>Tarea # " + e.tareaId + "</h6>" +
                        "<h4 class='card-subtitle mb-2 text-muted'>Fecha inicio:</h4>" +
                        "<p>" + e.fechaInicioTarea + "</p>" +
                        "<h4 class='card-subtitle mb-2 text-muted'>Fecha Fin:</h4>" +
                        "<p>" + e.fechaFinTarea + "</p>" +

                        "<div class='row-cols-3'>" +
                        "<a onclick='cambiarEstadoTarea("+ e.tareaId + "," + 3 + ", null)'" + " class='btn btn-success'>Iniciar tarea</a>" +
                        "<a onclick='cambiarEstadoTareaCancelar(" + e.tareaId + "," + 1 + ", null )'" + " class='btn btn-danger'>Cancelar tarea</a>" +
                        "<a onclick='cambiarEstadoTarea(" + e.tareaId + "," + 2 + ", null )'" + " class='btn btn-primary'>Terminar tarea</a>" +

                        "</div>" +
                        "</div>" +
                        "</div>" +
                        "</div >"
                    )
                    

                });

                if (!respuesta.data || respuesta.data.length == 0) {
                    var titulo = 'El Estado seleccionado, no tiene registros';
                    Swal.fire({
                        icon: 'warning',
                        title: titulo,
                        showConfirmButton: false,
                        timer: 3000
                    })
                }
            }
        })
    } else if (this.filtro == "6") {


        var startDate = $("#FechaInicioFiltro").val();
        var endDate = $("#FechaFinFiltro").val();
        $.ajax({
            url: '/Tareas/ObtenerTareasPorRangoFechaDeInicio/',
            data: jQuery.param({ fechaInicioFiltro: startDate, fechaFinFiltro: endDate }),
            type: 'get',
            dataType: 'json',
        }).done(function (respuesta) {
            if (respuesta.status) {

                $("#cardTarea").html("")
                respuesta.data.map(function (e) {

                    var estado = "";
                    if (e.estadoTarea == 1) {
                        estado = "RESERVADA"
                    }
                    else if (e.estadoTarea == 2) {
                        estado = "INICIADA"
                    }
                    else if (e.estadoTarea == 3) {
                        estado = "VENCIDA"
                    }
                    else if (e.estadoTarea == 4) {
                        estado = "CANCELADA"
                    }
                    else if (e.estadoTarea == 5) {
                        estado = "FINALIZADA EMPLEADO"
                    }
                    else if (e.estadoTarea == 6) {
                        estado = "FINALIZADA ADMIN"
                    }

                    $("#cardTarea").append(

                        "<div class='courses-container'>" +
                        "<div class= 'course'>" +
                        "<div class='course-preview'>" +

                        "<h4 class='card-subtitle mb-2 text-muted'>Nombre cliente :  </h4>" +
                        "<p>" + e.cliente.nombre + " " + e.cliente.apellidos + "</p>" +
                        "<h4 class='card-subtitle mb-2 text-muted'>Telefono cliente</h4>" +
                        "<p>" + e.cliente.telefono + "</p>" +

                        "<h4 class='ard-subtitle mb-2 text-muted'>Nombre tarea :  </h4>" +
                        "<p>" + e.operacion.nombre + "</p>" +
                        "<h4 class='card-subtitle mb-2 text-muted'>Descripcion tarea :  </h4>" +
                        "<p>" + e.descripcionTarea + "</p>" +


                        "</div>" +
                        "<div class='course-info'>" +
                        "<div class='progress-container'>" +
                        "<div class='progress'></div>" +

                        "<span class='progress-text'>" +

                        "<a>" + estado + "</a>" +

                        "</span>" +
                        "</div>" +
                        "<h6>Tarea # " + e.tareaId + "</h6>" +
                        "<h4 class='card-subtitle mb-2 text-muted'>Fecha inicio:</h4>" +
                        "<p>" + e.fechaInicioTarea + "</p>" +
                        "<h4 class='card-subtitle mb-2 text-muted'>Fecha Fin:</h4>" +
                        "<p>" + e.fechaFinTarea + "</p>" +

                        "<div class='row-cols-3'>" +
                        "<a onclick='cambiarEstadoTarea(" + e.tareaId + "," + 3 + "," + " " + ")'" + " class='btn btn-success'>Iniciar tarea</a>" +
                        "<a onclick='cambiarEstadoTareaCancelar(" + e.tareaId + "," + 1 + "," + " " + ")'" + " class='btn btn-danger'>Cancelar tarea</a>" +
                        "<a onclick='cambiarEstadoTarea(" + e.tareaId + "," + 2 + "," + " " + ")'" + " class='btn btn-primary'>Terminar tarea</a>" +

                        "</div>" +

                        "</div>" +
                        "</div>" +
                        "</div >"
                    )

                });


                if (!respuesta.data || respuesta.data.length == 0) {
                    var titulo = 'El rango de fechas no tiene tareas';
                    Swal.fire({
                        icon: 'warning',
                        title: titulo,
                        showConfirmButton: false,
                        timer: 3000
                    })
                }

            }
        })


    } else if (this.filtro == "7") {

        var startDate = $("#FechaInicioFiltro").val();
        var endDate = $("#FechaFinFiltro").val();
        $.ajax({
            url: '/Tareas/ObtenerTareasPorRangoFechaDeFin/',
            data: jQuery.param({ fechaInicioFiltro: startDate, fechaFinFiltro: endDate }),
            type: 'get',
            dataType: 'json',
        }).done(function (respuesta) {
            if (respuesta.status) {

                $("#cardTarea").html("")
                respuesta.data.map(function (e) {

                    var estado = "";
                    if (e.estadoTarea == 1) {
                        estado = "RESERVADA"
                    }
                    else if (e.estadoTarea == 2) {
                        estado = "INICIADA"
                    }
                    else if (e.estadoTarea == 3) {
                        estado = "VENCIDA"
                    }
                    else if (e.estadoTarea == 4) {
                        estado = "CANCELADA"
                    }
                    else if (e.estadoTarea == 5) {
                        estado = "FINALIZADA EMPLEADO"
                    }
                    else if (e.estadoTarea == 6) {
                        estado = "FINALIZADA ADMIN"
                    }

                    $("#cardTarea").append(

                        "<div class='courses-container'>" +
                        "<div class= 'course'>" +
                        "<div class='course-preview'>" +

                        "<h4 class='card-subtitle mb-2 text-muted'>Nombre cliente :  </h4>" +
                        "<p>" + e.cliente.nombre + " " + e.cliente.apellidos + "</p>" +
                        "<h4 class='card-subtitle mb-2 text-muted'>Telefono cliente</h4>" +
                        "<p>" + e.cliente.telefono + "</p>" +

                        "<h4 class='ard-subtitle mb-2 text-muted'>Nombre tarea :  </h4>" +
                        "<p>" + e.operacion.nombre + "</p>" +
                        "<h4 class='card-subtitle mb-2 text-muted'>Descripcion tarea :  </h4>" +
                        "<p>" + e.descripcionTarea + "</p>" +


                        "</div>" +
                        "<div class='course-info'>" +
                        "<div class='progress-container'>" +
                        "<div class='progress'></div>" +

                        "<span class='progress-text'>" +

                        "<a>" + estado + "</a>" +

                        "</span>" +
                        "</div>" +
                        "<h6>Tarea # " + e.tareaId + "</h6>" +
                        "<h4 class='card-subtitle mb-2 text-muted'>Fecha inicio:</h4>" +
                        "<p>" + e.fechaInicioTarea + "</p>" +
                        "<h4 class='card-subtitle mb-2 text-muted'>Fecha Fin:</h4>" +
                        "<p>" + e.fechaFinTarea + "</p>" +

                        "<div class='row-cols-3'>" +
                        "<a onclick='cambiarEstadoTarea(" + e.tareaId + "," + 3 + "," + " " + ")'" + " class='btn btn-success'>Iniciar tarea</a>" +
                        "<a onclick='cambiarEstadoTareaCancelar(" + e.tareaId + "," + 1 + "," + " " + ")'" + " class='btn btn-danger'>Cancelar tarea</a>" +
                        "<a onclick='cambiarEstadoTarea(" + e.tareaId + "," + 2 + "," + " " + ")'" + " class='btn btn-primary'>Terminar tarea</a>" +

                        "</div>" +

                        "</div>" +
                        "</div>" +
                        "</div >"
                    )

                });
                if (!respuesta.data || respuesta.data.length == 0) {
                    var titulo = 'El rango de fechas no tiene tareas';
                    Swal.fire({
                        icon: 'warning',
                        title: titulo,
                        showConfirmButton: false,
                        timer: 3000
                    })
                }
            }
        })
    }
}

