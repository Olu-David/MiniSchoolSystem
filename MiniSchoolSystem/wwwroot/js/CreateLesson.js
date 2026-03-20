var cur = 1;
function go(n) {
    if (n > 1 && cur === 1) {
        if (!q('Title').value.trim()) {
            SabiToast('Enter a lesson title.', 'warning');
            return;
        }

        if (!q('LessonSection').value) {
            SabiToast('Select a section.', 'warning');
            return;
        }

        if (!q('Id').value) {
            SabiToast('Enter a lesson ID.', 'warning');
            return;
        }

    }

    if (n > 2 && cur === 2) {
        if (!document.getElementById('Content').value.trim()) {
            SabiToast('Add lesson content.', 'warning');
            return;
        }

    }
    [1, 2, 3].forEach(i => {
        document.getElementById('step' + i).classList.toggle('act', i === n);
        var sn = document.getElementById('sn' + i), sl = document.getElementById('sl' + i);
        if (i < n) { sn.className = 'sn done'; sn.textContent = '✓'; sl.className = 'sl act'; }
        else if (i === n) { sn.className = 'sn act'; sn.textContent = i; sl.className = 'sl act'; }
        else { sn.className = 'sn pen'; sn.textContent = i; sl.className = 'sl pen'; }
    });
    cur = n;
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function q(n) {
    return document.querySelector('[name="' + n + '"]');
}

function cnt(el, id, max) {
    var l = el.value.length, c = document.getElementById(id);
    c.textContent = l.toLocaleString() + ' / ' + max.toLocaleString();
    c.className = 'cc' + (l > max * .9 ? ' w' : '');
}

function prevFile(inp) {
    var f = inp.files[0];
    if (!f) return;
    var icons =

    {
        pdf: '📄', doc: '📝', docx: '📝', ppt: '📊', pptx: '📊', xls: '📋', xlsx: '📋', mp4: '🎬', zip: '🗜️'
    }

        ; var ext = f.name.split('.').pop().toLowerCase(); var sz = f.size > 1048576 ? (f.size / 1048576).toFixed(1) + ' MB' : (f.size / 1024).toFixed(0) + ' KB'; document.getElementById('fdi').textContent = icons[ext] || '📎'; document.getElementById('fdn').textContent = f.name; document.getElementById('fds').textContent = sz; document.getElementById('fdp').classList.add('show'); document.querySelector('.fdtitle').textContent = 'File selected ✓';
}

function rmFile() {
    document.getElementById('fi').value = '';
    document.getElementById('fdp').classList.remove('show');
    document.querySelector('.fdtitle').textContent = 'Drag & drop or click to upload';
}

var d = document.getElementById('fdrop');
['dragover', 'dragenter'].forEach(e => d.addEventListener(e, ev => { ev.preventDefault(); d.classList.add('dg'); }));
['dragleave', 'drop'].forEach(e => d.addEventListener(e, () => d.classList.remove('dg')));
function onSubmit() {
    var btn = document.getElementById('subBtn');
    btn.disabled = true;
    document.getElementById('subSpin').style.display = 'block';
    document.getElementById('subTxt').textContent = 'Publishing…';
}
