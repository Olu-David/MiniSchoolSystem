/* Load lesson info if ViewBag.Lesson was not set (fallback via JS) */
(function () {
    var id = new URLSearchParams(location.search).get('id') ||
        location.pathname.split('/').pop();
    var titleEl = document.getElementById('lessonTitle');
    var metaEl = document.getElementById('lessonMeta');
    if (!titleEl || !id || isNaN(id)) return;

    fetch('/Lesson/GetLessons')
        .then(function (r) { return r.json(); })
        .then(function (lessons) {
            var l = lessons.find(function (x) { return String(x.id) === String(id); });
            if (!l) return;
            titleEl.textContent = l.title || 'Lesson';
            metaEl.textContent = l.lessonSection || '';
        });
})();

function toggleBtn() {
    document.getElementById('archiveBtn').disabled =
        !document.getElementById('confirmCheck').checked;
}
function handleSubmit(btn) {
    btn.disabled = true;
    document.getElementById('spin').style.display = 'block';
    document.getElementById('btnTxt').textContent = 'Archiving…';
}