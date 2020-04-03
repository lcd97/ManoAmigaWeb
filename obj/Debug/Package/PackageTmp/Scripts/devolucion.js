var fila = "";

//LLENAR VISTA PARCIAL
function SeleccionarAlquiler(alquilerId) {   

    //OBTENER EL VALOR DE LA FILA
    $("#tablaPrestamo").on("click", "#filita", function () {
        //fila = $(this).parents("tr");
        //fila = $(this).closest('tr').index();
        fila = $(this).closest('tr');

        //alert("INDICE FILA = " + fila);
    });

    //CARGAR DATOS DE ALQUILER
    $.ajax({
        type: "GET",
        url: "/Prestamos/loadPrest",
        data: {
            id: alquilerId
        },
        success: function (list) {
            $("#codigo").val(list.CodigoAlquiler);
            $("#fecha1").val(list.FechaAlquiler);
            $("#fecha2").val(list.FechaDevo);
            $("#cliente").val(list.NombreCompleto);
            }        
    });
    //CARGAR DATOS DE DETALLE ALQUILER
    $.ajax({
        type: "GET",
        url: "/Prestamos/loadDetails",
        data: {
            id: alquilerId
        },
        success: function (resultado) {
            var agregar = "";            
            for (var i = 0; i < resultado.data.length; i++) {
                agregar += "<tr>" +
                    "<td style='width: 70 %'>" + resultado.data[i].TituloDeLibro + "</td>" +
                    "<td>" + resultado.data[i].NumeroCopia + "</td>" +
                    "</tr>";
            }

            $("#table").append(agregar);
        }
    });

    //DESACTIVAR CAMPOS PARA EVITAR EDICIONES
    $("#codigo").prop('disabled', true);
    $("#fecha1").prop('disabled', true);
    $("#fecha2").prop('disabled', true);
    $("#cliente").prop('disabled', true);
    //$("#date3").prop('disabled', true);
}

////GUARDAR CAMBIOS
function AceptarAlq() {
    $.ajax({
        type: "POST",
        url: "/Prestamos/EditDev",
        data: {
            codAlq: $("#codigo").val(),
            fechaDev: $("#date3").val()
        },
        success: function (list) {
            if (list.d > 0) {
                //$("#miModal .close").click();
                $('#miModal').modal('hide');
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
                //ELIMINAR FILA DE LA TABLA PRESTAMOS SELECCIONADO
                fila.remove().draw();
                //$("#GrdCred").index(filita).Remove();
            } else
                alert("Datos Incorrectos. Verificar");
        },
        error: function () {
            alert("Revisar campos. Solicitud Rechazada");
        }
    });
}

//ELIMINAR DATOS CARGADOS ANTERIORMENTE
function cancelar() {
    $("#table tbody").remove();
}