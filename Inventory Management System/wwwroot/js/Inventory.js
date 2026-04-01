// ================= VALIDATION =================
function validateProduct() {

    let isValid = true;
    $(".error").text("");

    if ($("#Name").val().trim() === "") {
        $("#errName").text("Required");
        isValid = false;
    }

    if ($("#CategoryId").val() === "") {
        $("#errCategory").text("Select Category");
        isValid = false;
    }

    if ($("#SupplierId").val() === "") {
        $("#errSupplier").text("Select Supplier");
        isValid = false;
    }

    if ($("#Price").val() === "" || isNaN($("#Price").val())) {
        $("#errPrice").text("Invalid Price");
        isValid = false;
    }

    if ($("#Quantity").val() === "" || isNaN($("#Quantity").val())) {
        $("#errQty").text("Invalid Qty");
        isValid = false;
    }

    return isValid;
}

// Toast Notification
function showToast(type, message) {

    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        timer: 2000,
        showConfirmButton: false
    });

    Toast.fire({
        icon: type,
        title: message
    });

    // 🔊 SOUND
    if (type === 'success') {
        playSuccessSound();
    }
}

// ================= PAGE LOAD =================
$(document).ready(function () {
    loadDropdowns();
    loadData();
    loadDashboard(); 
    loadChart();
});
// ================= LOAD DATA =================
function loadData() {

    $.get('/Home/GetAll', function (data) {

        let html = '';

        data.forEach(x => {
            html += `<tr>
                <td>${x.name}</td>
                <td>${x.categoryName}</td>
                <td>${x.supplierName}</td>
                <td>${x.price}</td>
                <td>${x.quantity}</td>
                <td>
                    <button class="btn btn-sm btn-outline-warning" onclick="edit(${x.id})">Edit</button>
                    <button class="btn btn-outline-danger btn-sm" onclick="deleteProduct(${x.id})">Delete</button>
                    
                    <button class="btn btn-sm btn-outline-success" onclick="stockIn(${x.id}, this)">+ In</button>
                    <button class="btn btn-sm btn-outline-dark" onclick="stockOut(${x.id}, this)">- Out</button>
                </td>
            </tr>`;
        });

        $("#tblData").html(html);

        // ✅ DataTable FIX
        if ($.fn.DataTable.isDataTable('#productTable')) {
            $('#productTable').DataTable().destroy();
        }

        // re-init
        $('#productTable').DataTable({
            paging: true,
            searching: true,
            ordering: true,
            info: true
        });
    });
}

// ================= LOAD DROPDOWNS =================
function loadDropdowns() {

    $.get('/Home/GetCategories', function (data) {

        let opt = '<option value="">Select Category</option>';

        data.forEach(x => {
            opt += `<option value="${x.id}">${x.name}</option>`;
        });

        $("#CategoryId").html(opt);
    });

    $.get('/Home/GetSuppliers', function (data) {

        let opt = '<option value="">Select Supplier</option>';

        data.forEach(x => {
            opt += `<option value="${x.id}">${x.name}</option>`;
        });

        $("#SupplierId").html(opt);
    });
}

// ================= OPEN MODAL =================
function openModal() {
    clearForm();
    $("#productModal").modal('show');
}

// ================= SAVE (INSERT + UPDATE) =================
function saveProduct() {

    if (!validateProduct()) return;

    // SHOW LOADER
    $("#btnText").hide();
    $("#btnLoader").removeClass("d-none");

    let obj = {
        Id: $("#Id").val(),
        Name: $("#Name").val(),
        CategoryId: $("#CategoryId").val(),
        SupplierId: $("#SupplierId").val(),
        Price: $("#Price").val(),
        Quantity: $("#Quantity").val()
    };

    let url = obj.Id ? '/Home/Update' : '/Home/Create';

    $.post(url, obj, function () {

        // HIDE LOADER
        $("#btnText").show();
        $("#btnLoader").addClass("d-none");

        showToast('success', 'Product saved successfully');

        $("#productModal").modal('hide');

        loadData();
        loadDashboard();
        loadChart();
    });
}

// ================= EDIT =================
function edit(id) {

    $.get('/Home/GetById?id=' + id, function (res) {

        $("#Id").val(res.id);
        $("#Name").val(res.name);
        $("#CategoryId").val(res.categoryId);
        $("#SupplierId").val(res.supplierId);
        $("#Price").val(res.price);
        $("#Quantity").val(res.quantity);

        $("#productModal").modal('show');
    });
}

// ================= DELETE =================
function deleteProduct(id) {

    Swal.fire({
        title: 'Are you sure?',
        text: "This will be deleted!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33'
    }).then((result) => {

        if (result.isConfirmed) {

            $.post('/Home/Delete?id=' + id, function () {

                showToast('success', 'Product deleted');
                loadData();
                loadDashboard();   
                loadChart();
            });
        }
    });
}

function showError(id, message) {
    let el = $(id);
    el.text(message);

    el.prev().addClass("shake");

    setTimeout(() => {
        el.prev().removeClass("shake");
    }, 300);
}

// ================= CLEAR FORM =================
function clearForm() {
    $("#Id").val('');
    $("#Name").val('');
    $("#CategoryId").val('');
    $("#SupplierId").val('');
    $("#Price").val('');
    $("#Quantity").val('');
    $(".error").text('');
}

// ================= Load Dashboard =================

function loadDashboard() {

    $.get('/Home/GetDashboard', function (res) {

        $("#totalProducts").text(res.totalProducts);
        $("#totalCategories").text(res.totalCategories);
        $("#totalSuppliers").text(res.totalSuppliers);
        $("#lowStock").text(res.lowStock);
    });
}

// ================= Stock in out =================

function stockIn(id, btn) {

    Swal.fire({
        title: 'Stock In',
        input: 'number',
        inputLabel: 'Enter Quantity to Add',
        showCancelButton: true,
        confirmButtonText: 'Add'
    }).then((result) => {

        if (result.isConfirmed) {

            let qty = parseInt(result.value);

            $.post('/Home/StockIn', { productId: id, qty: qty }, function () {

             
                let row = $(btn).closest("tr");
                let qtyCell = row.find("td:eq(4)");

                let currentQty = parseInt(qtyCell.text());
                qtyCell.text(currentQty + qty);

                Swal.fire('Success!', 'Stock added', 'success');
            });
        }
    });
}

function stockOut(id, btn) {

    Swal.fire({
        title: 'Stock Out',
        input: 'number',
        inputLabel: 'Enter Quantity to Remove',
        showCancelButton: true,
        confirmButtonText: 'Remove'
    }).then((result) => {

        if (result.isConfirmed) {

            let qty = parseInt(result.value);

            $.post('/Home/StockOut', { productId: id, qty: qty }, function () {

              
                let row = $(btn).closest("tr");
                let qtyCell = row.find("td:eq(4)");

                let currentQty = parseInt(qtyCell.text());
                qtyCell.text(currentQty - qty);

                Swal.fire('Success!', 'Stock removed', 'success');
            });
        }
    });
}

// ================= Charts =================

let barChart;
let pieChart;

function loadChart() {

    $.get('/Home/GetChartData', function (data) {

        let labels = [];
        let values = [];

        data.forEach(x => {
            labels.push(x.category);
            values.push(x.total);
        });

        let colors = [
            '#4e73df', '#1cc88a', '#36b9cc', '#f6c23e',
            '#e74a3b', '#858796', '#20c997', '#6610f2',
            '#fd7e14', '#17a2b8'
        ];

        // 🔵 BAR CHART
        new Chart(document.getElementById('barChart'), {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: colors,
                    borderRadius: 6
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } }
            }
        });

        // 🟠 PIE CHART
        new Chart(document.getElementById('pieChart'), {
            type: 'pie',
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: colors
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false
            }
        });

    });
}

function playSuccessSound() {
    let sound = document.getElementById("successSound");
    if (sound) {
        sound.currentTime = 0;
        sound.play().catch(() => { });
    }
}