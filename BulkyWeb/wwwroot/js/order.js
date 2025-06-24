var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    let status = "all";
    if (url.includes("inprocess")) {
        status = "inprocess";
    } else if (url.includes("completed")) {
        status = "completed";
    } else if (url.includes("pending")) {
        status = "pending";
    } else if (url.includes("approved")) {
        status = "approved";
    }
    loadDataTable(status);
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        ajax: { url: '/admin/order/getall?status='+status, dataSrc: 'data' },
        columns: [
            { data: 'id', "width": "10%" },
            { data: 'name', "width": "25%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'applicationUser.email', "width": "25%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            //{ data: null, width: '15%', defaultContent: "", orderable: false }
            {
                data: 'id',
                width: "10%",
                render: function (data) {
                    return `<div class="text-center">
                        <a href="/admin/order/details?orderId=${data}" class="btn btn-sm btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                    </div>`;
                },
                orderable: false
            }            
        ]
    });
}
