//BUSQUEDA DE CLIENTE
function AceptarBusClick() {
    $("#CodE").val($("#CliSeleccionado").val());
    $("#oGrid").toggle();
    $("#NombCompleto").toggle();
    $('#bModal').modal('hide');
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
    //$("#bModal").modal("hide");
}

//LIMPIAR MODAL


//$(document).ready(function () {
//    $(".calendario").datepicker(
//       {dateFormat: 'dd/mm/yy',
//        changeMonth: true,
//        changeYear: true,
//        yearRange: '1980:2030'
//        }
//    );
//});

//TABLA DEL INDEX PRESTAMO DE LIBRO
$(document).ready(function () {
    $("#tabla").hide();
});

$("#cmbcopia").change(function () {
    if ($(this).val()) {
        $("#btnAcpLib").attr("disabled", false);
    } else
        $("#btnAcpLib").attr("disabled", true);
});
//Agregar libro a tabla de prestamo
function AceptarLibro() {
    $.ajax({
        type: "GET",
        url: "/Prestamos/Add",
        data: {
            titulo: $("#cmblibro").find("option:selected").text(),
            numcopia: $("#cmbcopia").find("option:selected").text(),
            id: $("#cmbcopia").val()
        },
        success: function (list) {
            //$("#bModal").modal("hide");
            $('#bModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $("#tabla").show();
            if (list.d.Id === -5)
                alert("Ya existe");
            else {
                var agregar = "<tr>" +
                    "<td>" + list.d.TituloDeLibro + "</td>" +
                    "<td>" + list.d.NumeroCopia + "</td>" +
                    "<td>" + "<input type='button' id='boton' class='btn btn-danger' value='Eliminar' onclick='EliminarFila();'/>"+"</td>" +
                    "</tr>";
                $("#tabla").append(agregar);
            }
        }
    });
}

//Eliminar libro (segun columna especificada)
$("#tabla").on("click", "#boton", function () {
    var title, copy;

    title = $(this).parents("tr").find('td').eq(0).html();
    copy = $(this).parents("tr").find('td').eq(1).html();
    $(this).parents("tr").remove();
    alert("Eliminando " + title + " Numero copia #" + copy);

    $.ajax({
        type: "GET",
        url: "/Prestamos/Remove",
        data: {
            titulo: title,
            numcopia: copy
        },
        success: function (data) {
            if (data > 0) {
                $(this).parents("tr").remove();            
            }            
        }
    });
});