var lid = window.location.pathname.split('/').pop();

fetch('/Lesson/GetLessons')
    .then(r => r.json())
    .then(lessons => {
        var l = lessons.find(x => String(x.id) === String(lid));
        if (!l) { showErr(); return; }
        fill(l);
    }).catch(showErr);

function fill(l) {
    var t = n => document.getElementById(n);
    document.title = (l.title || 'Lesson') + ' — SabiSpace';
    t('bcTitle').textContent = l.title || '—';
    t('hSec').textContent = l.lessonSection || 'General';
    t('hTitle').textContent = l.title || '—';
    t('hDesc').textContent = l.description || '';
    t('hDate').textContent = fmt(l.createdAt);
    t('hTeacher').textContent = l.teacherName || '—';
    t('tcAv').textContent = (l.teacherName || 'T')[0];
    t('tcName').textContent = l.teacherName || '—';
    t('siSec').textContent = l.lessonSection || '—';
    t('siDate').textContent = fmt(l.createdAt);
    t('siTeacher').textContent = l.teacherName || '—';
    t('siStatus').innerHTML = l.isArchived
        ? '<span class="pill p-arch">Archived</span>'
        : '<span class="pill p-live">Active</span>';

    var c = (l.lessonContents || l.lessonContent || [])[0] || {};
    t('contentTxt').textContent = c.content || 'No content yet.';
    t('aboutTxt').textContent = l.description || '—';
    if (c.fileUrl) {
        t('fileWrap').style.display = 'block';
        t('fileCard').href = c.fileUrl;
        var parts = c.fileUrl.split('/');
        t('fName').textContent = parts[parts.length - 1] || 'Attachment';
    }

    // Wire action links & forms
    var setHref = (id, url) => { var el = document.getElementById(id); if (el) el.href = url; };
    setHref('editLink', '/Lesson/EditLesson/' + l.id);
    setHref('delLink', '/Lesson/DeleteLesson?id=' + l.id);
    setHref('sEditLink', '/Lesson/EditLesson/' + l.id);
    setHref('sDelLink', '/Lesson/DeleteLesson?id=' + l.id);

    var af = document.getElementById('archForm');
    if (af) af.action = '/Lesson/ArchiveLesson/' + l.id;
    var saf = document.getElementById('sArchForm');
    if (saf) saf.action = '/Lesson/ArchiveLesson/' + l.id;
    var srf = document.getElementById('sRestForm');
    if (srf) srf.action = '/Lesson/RestoreLesson/' + l.id;
}

function showErr() {
    document.getElementById('hTitle').textContent = 'Lesson not found';
    document.getElementById('contentTxt').textContent = 'This lesson could not be loaded.';
}

function swTab(id, btn) {
    document.querySelectorAll('.panel').forEach(p => p.classList.remove('on'));
    document.querySelectorAll('.tab').forEach(b => b.classList.remove('on'));
    document.getElementById(id).classList.add('on');
    btn.classList.add('on');
}

function fmt(d) { return d ? new Date(d).toLocaleDateString('en-GB', { day: 'numeric', month: 'long', year: 'numeric' }) : '—'; }