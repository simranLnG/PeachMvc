function GetStatus(resourcePath) {

    var currentUrl = window.location.href;
    var url = new URL(currentUrl);
    var resourcePath = url.searchParams.get("resourcePath");

    $.ajax({

        url: "/Home/GetPaymentStatus",
        type: "GET",
        data: { resourcePath: resourcePath },
        success: function (result) {
            console.log(result);
            //if (result.toString == "Success") {
            //    $('#GrnNumber').text('Entered GRN already exists..!')
            //    $('#GRNfield').val(null);
            //}
            //else {
            //    $('#GrnNumber').text('')
            //}
        },
        error: function (xhr, status, error) {
            // Handle the error
        }
    });

}

