
function Delete(url) {
    // display sweet alert
    swal({
        title: "Are you sure you want to Delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        // make ajax call to api delete
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        Swal.fire(
                            'Success!',
                            data.message,
                            'success'
                        ).then((btnOk) => {
                            // reload when okay btn is pressed
                            location.reload(true);
                        // dataTable.ajax.reload();
                        });
                    } else {
                        Swal.fire(
                            'Error!',
                            data.message,
                            'error'
                        )
                    }
                }
            });
        }
    });
}