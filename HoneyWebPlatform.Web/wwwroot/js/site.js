function statistics() {
    $('#statistics_btn').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        // hasClass('d-none') -> Statistics are hidden
        if ($('#statistics_box').hasClass('d-none')) {
            $.get('https://localhost:7180/api/statistics', function (data) {
                $('#total_honeys').text("Общо имаме: " + data.totalHoneys + " Медове");
                $('#total_propolises').text("Общо имаме: " + data.totalPropolises + " Прополиси");

                $('#statistics_box').removeClass('d-none');

                $('#statistics_btn').text('Скрий Статистиката');
                $('#statistics_btn').removeClass('btn-primary');
                $('#statistics_btn').addClass('btn-danger');
            });
        } else {
            $('#statistics_box').addClass('d-none');

            $('#statistics_btn').text('Покажи Статистиката');
            $('#statistics_btn').removeClass('btn-danger');
            $('#statistics_btn').addClass('btn-primary');
        }
    });
}