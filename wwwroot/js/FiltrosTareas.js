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
            { estadoId: 1, nombre: "RESERVADO" },
            { estadoId: 2, nombre: "EN PROCESO" },
            { estadoId: 3, nombre: "VENCIDO" }
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
                        end: e.fechaFin,
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
                        start: e.fechaInicio,
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

                let data = [];
                respuesta.data.map(function (e) {

                });


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

                let data = [];
                respuesta.data.map(function (e) {

                });


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
    } else if (solicitudEstado == 7) {
        estado = "FINALIZADO"
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