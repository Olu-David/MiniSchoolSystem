(function () {
    var id = new URLSearchParams(location.search).get('id') ||
        location.pathname.split('/').pop();
    var titleEl = document.getElementById('lessonTitle');
    if (!titleEl || !id || isNaN(id)) return;
    fetch('/Lesson/GetLessons')
        .then(function (r) { return r.json(); })
        .then(function (lessons) {
            /* GetLessons only returns active ones — need to handle this */
            var l = lessons.find(function (x) { return String(x.id) === String(id); });
            if (!l) return;
            titleEl.textContent = l.title || 'Lesson';
        });
})();
function handleSubmit(btn) { btn.disabled = true; document.getElementById('spin').style.display = 'block'; document.getElementById('btnTxt').textContent = 'Restoring…'; }