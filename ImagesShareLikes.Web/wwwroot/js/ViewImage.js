$(() => {

    const id = $("#image-id").val();
    $("#like-button").on('click', function () {
        $.post('/home/updateimagelikes', { id }, function () {
        });
        $("#like-button").prop('disabled', true)
    });

    setInterval(() => {
        $.get('/home/getimage', { id }, image => {
            $("#likes-count").text(image.likes);
        })
    }, 500);       



});
