window.showSuccess = (message) => {

    Swal.fire({
        title: 'Éxito',
        text: message,
        icon: 'success',
        confirmButtonText: 'Aceptar',
        confirmButtonColor: '#3085d6',
        background: '#1e1e2f',
        color: '#fff'
    });
};

window.showError = (message) => {

    Swal.fire({
        title: 'Error',
        text: message,
        icon: 'error',
        confirmButtonText: 'Cerrar',
        confirmButtonColor: '#d33'
    });
};

window.showQuestion = async (title, message) => {

    const result = await Swal.fire({
        title: title,
        text: message,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Sí',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        reverseButtons: true
    });

    return result.isConfirmed;
};