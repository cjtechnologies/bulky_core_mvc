var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: { url: '/admin/product/getall', dataSrc: 'data' },
        columns: [
            { data: 'title', "width": "25%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "15%" },
            { data: 'category.name', "width": "10%" },
            //{ data: null, width: '15%', defaultContent: "", orderable: false }
            {
                data: 'id',
                width: "25%",
                render: function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i>Edit</a>
                        <a onClick=deleteProduct('/admin/product/delete/${data}') class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i>Delete</a>
                    </div>`;
                },
                orderable: false
            }
        ]
    });
}

function deleteProduct(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'POST',
                //success: function (data) {
                //    dataTable.ajax.reload();
                //    toastr.success(data.message);
                //}
                success: function (data) {
                    dataTable.ajax.reload();
                    Swal.fire({
                        title: "Deleted!",
                        text: data.message,
                        icon: "success"
                    });
                }
            });            
        }
    });
}