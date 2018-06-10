function OpenPopup(actionID, id) {
    if (actionID == 1) {
        var Url = "/Product/AddProduct";
        StartPopup(Url);
    } else if (actionID == 2) {
        var Url = "/Product/EditProduct?id=" + id;
        StartPopup(Url);
    } else if (actionID == 3) {
        var Url = "/Product/DeleteProduct?id=" + id;
        StartPopup(Url);
    } else if (actionID == 4) {
        var Url = "/Product/ProductDetails?id=" + id;
        StartPopup(Url);
    } else if (actionID == 9) {
        var Url = "/Exchange/PokeRequest?id=" + id;
        StartPopup(Url);
    } else if (actionID == 10) {
        var Url = "/Exchange/ReSendPokeRequest?id=" + id;
        StartPopup(Url);
    }
}

function StartPopup(Url) {
    $("#myModalBody").load(Url, function () {
        $("#myModal").modal("show");
    })
}

function OpenCustomerPopup(actionID, id) {
    if (actionID == 5) {
        var Url = "/Admin/EditCustomer?id=" + id;
        StartPopup(Url);
    } else if (actionID == 6) {
        var Url = "/Admin/ViewCustomer?id=" + id;
        StartPopup(Url);
    } else if (actionID == 7) {
        var Url = "/Admin/DeleteCustomer?id=" + id;
        StartPopup(Url);
    } else if (actionID == 8) {
        var Url = "/Admin/AddCustomer";
        StartPopup(Url);
    }

}