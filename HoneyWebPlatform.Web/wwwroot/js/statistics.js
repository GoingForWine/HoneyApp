function statistics(statisticsUrl) {
    $('#statistics_btn').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        // hasClass('d-none') -> Statistics are hidden
        if ($('#statistics_box').hasClass('d-none')) {
            $.get(statisticsUrl, function (data) {
                $('#total_honeys').text("Общо медове: " + data.totalHoneys);
                $('#total_active_honeys').text("Активни медове: " + data.totalActiveHoneys);
                $('#total_propolises').text("Общо прополиси: " + data.totalPropolises);
                $('#total_active_propolises').text("Активни прополиси: " + data.totalActivePropolises);
                $('#total_posts').text("Общо публикации: " + data.totalPosts);
                $('#total_active_posts').text("Активни публикации: " + data.totalActivePosts);

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