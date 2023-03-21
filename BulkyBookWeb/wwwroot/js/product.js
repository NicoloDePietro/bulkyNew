var dataTable;
$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
    //per i dettagli si veda qui: https://datatables.net/manual/ajax
    dataTable = $('#tblData').DataTable({
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.13.2/i18n/it-IT.json"
        },
        ajax: {
            url: "/Admin/Product/GetAll"
        },
        columns: [
            { data: "title", width: "15%" },
            { data: "isbn", width: "15%" },
            { data: "price", width: "15%" },
            { data: "author", width: "15%" },
        ]
    });
}