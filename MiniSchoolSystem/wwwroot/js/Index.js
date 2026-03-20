var all = [];
var isTeacher = @(Json.Serialize(User.IsInRole("Teacher") || User.IsInRole("SuperAdmin")));

fetch('/Lesson/GetLessons')
    .then(r => r.json())
    .then(d => { all = d; render(d); })
    .catch(() => {
        document.getElementById('grid').innerHTML =
            '<div class="empty"><div class="empty-icon">😔</div><h3>Could not load lessons</h3><p>Refresh the page and try again.</p></div>';
    });

var thumbs = ['💻', '📊', '🎨', '💰', '🤖', '🧪', '📐', '🌍', '📖', '🎵'];
var bgs = ['linear-gradient(135deg,#1a1c22,#2a1810)', 'linear-gradient(135deg,#0c1a12,#1a2a1a)', 'linear-gradient(135deg,#1a1228,#0c1020)', 'linear-gradient(135deg,#1a1500,#2a2000)', 'linear-gradient(135deg,#001a1a,#001028)'];

function render(lessons) {
    var g = document.getElementById('grid');
    var rc = document.getElementById('rc');
    if (!lessons.length) {
        g.innerHTML = '<div class="empty"><div class="empty-icon">📭</div><h3>No lessons found</h3><p>Try a different search or filter.</p></div>';
        rc.textContent = '0 lessons';
        return;
    }
    rc.textContent = lessons.length + ' lesson' + (lessons.length !== 1 ? 's' : '');

    g.innerHTML = lessons.map((l, i) => {
        var isNew = new Date(l.createdAt) > new Date(Date.now() - 7 * 864e5);
        var badge = isNew ? '<span class="tbadge tb-new">✨ New</span>' : '<span class="tbadge tb-live">Live</span>';
        var acts = isTeacher ? `
            <div class="card-actions">
                <a class="ca-btn ca-edit" href="/Lesson/EditLesson/${l.id}">✏️ Edit</a>
                <form method="post" action="/Lesson/ArchiveLesson/${l.id}" style="display:contents">
                    <input type="hidden" name="__RequestVerificationToken" value="${getToken()}" />
                    <button type="submit" class="ca-btn ca-arch">📦 Archive</button>
                </form>
                <a class="ca-btn ca-del" href="/Lesson/DeleteLesson?id=${l.id}">🗑 Delete</a>
            </div>` : '';

        return `<a class="lcard" href="/Lesson/Details/${l.id}">
            <div class="ct">
                <div class="ctbg" style="background:${bgs[i % bgs.length]}">${thumbs[i % thumbs.length]}</div>
                <div class="cto"><div class="pc">▶</div></div>
                ${badge}
            </div>
            <div class="cb">
                <div class="csec">${l.lessonSection || 'General'}</div>
                <h3 class="ctitle">${esc(l.title)}</h3>
                <p class="cdesc">${esc(l.description || '')}</p>
                <div class="cf">
                    <div class="cteacher">
                        <div class="tav">${(l.teacherName || 'T')[0]}</div>
                        <span class="tname">${esc(l.teacherName || 'Teacher')}</span>
                    </div>
                    <span class="cdate">${fmt(l.createdAt)}</span>
                </div>
            </div>
        </a>${acts}`;
    }).join('');
}

function filter() {
    var q = document.getElementById('srch').value.toLowerCase();
    var s = document.getElementById('secFilter').value;
    render(all.filter(l =>
        (!q || (l.title || '').toLowerCase().includes(q) || (l.description || '').toLowerCase().includes(q)) &&
        (!s || l.lessonSection === s)
    ));
}

function setView(v) {
    document.getElementById('grid').classList.toggle('lv', v === 'l');
    document.getElementById('btnG').classList.toggle('on', v === 'g');
    document.getElementById('btnL').classList.toggle('on', v === 'l');
}

function getToken() {
    var el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
}
function esc(s) { var d = document.createElement('div'); d.textContent = s; return d.innerHTML; }
function fmt(d) { return d ? new Date(d).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' }) : ''; }
