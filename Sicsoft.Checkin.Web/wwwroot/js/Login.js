
function Login() {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: 'https://localhost:44350/api/Login/Compañias?email=' + $("#Input_Email").val(),
        data: {  },
        success: function (result) {

            console.log(result);
            var t = '';
            $("#Compañia").html('');
            t += '<option value="" selected>Seleccione Compañía</option>';
            for (var i = 0; i < result.length; i++) {
                t += '<option value="' + result[i].CedulaJuridica + '">' + result[i].NombreEmpresa +'</option>';
            }



                $("#Compañia").html(t);
        },
        beforeSend: function () {

        },
        complete: function () {

        }
    });
}

function Guardar() {
    if (Validar()) {
        $("#formTipos").submit();
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Parece que faltan datos por llenar'

        });
    }
}

function Validar() {
    if ($("#Input_Email").val() == "") {
        return false;
    } else if ($("#Input_Password").val() == "")
    {
        return false;
    } else if ($("#Compañia").val() == "")
    {
        return false;
    } else {
        return true;
    }
}
