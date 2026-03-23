/* ═══════════════════════════════════════════════
   Auth.js — shared for Login.cshtml + Registration.cshtml
═══════════════════════════════════════════════ */

/* ── Password eye toggle ── */
function toggleEye(id, btn) {
    var inp = document.getElementById(id);
    var show = inp.type === 'password';
    inp.type = show ? 'text' : 'password';
    btn.querySelector('svg').innerHTML = show
        ? '<path d="M17.94 17.94A10.07 10.07 0 0112 20c-7 0-11-8-11-8a18.45 18.45 0 015.06-5.94"/><path d="M9.9 4.24A9.12 9.12 0 0112 4c7 0 11 8 11 8a18.5 18.5 0 01-2.16 3.19"/><line x1="1" y1="1" x2="23" y2="23"/>'
        : '<path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/>';
}

/* ── Password strength meter ── */
function measureStrength(pw) {
    var wrap = document.getElementById('sw');
    var lbl = document.getElementById('sl');
    if (!wrap) return;

    wrap.style.display = pw.length ? 'block' : 'none';

    var s = 0;
    if (pw.length >= 8) s++;
    if (/[A-Z]/.test(pw)) s++;
    if (/[0-9]/.test(pw)) s++;
    if (/[^A-Za-z0-9]/.test(pw)) s++;

    var lvls = [
        { c: '#f87171', t: 'Too weak' },
        { c: '#fb923c', t: 'Weak' },
        { c: '#facc15', t: 'Good' },
        { c: '#4ade80', t: 'Strong 💪' }
    ];
    var idx = Math.max(0, s - 1);

    for (var i = 1; i <= 4; i++) {
        var bar = document.getElementById('sb' + i);
        if (bar) bar.style.background = i <= s ? lvls[idx].c : 'rgba(255,255,255,.07)';
    }
    if (lbl) {
        lbl.textContent = pw.length ? lvls[idx].t : '';
        lbl.style.color = lvls[idx].c;
    }
}

/* ── Role selector (registration) ── */
function selectRole(role) {
    var roleInput = document.getElementById('roleInput');
    if (roleInput) roleInput.value = role;

    var btnStudent = document.getElementById('btnStudent');
    var btnParent = document.getElementById('btnParent');
    if (btnStudent) btnStudent.classList.toggle('on', role === 'Student');
    if (btnParent) btnParent.classList.toggle('on', role === 'Parent');

    var sg = document.getElementById('sectionGroup');
    var ss = document.getElementById('sectionSelect');
    if (sg && ss) {
        if (role === 'Student') {
            sg.style.display = 'block';
            ss.required = true;
        } else {
            sg.style.display = 'none';
            ss.required = false;
            ss.value = '';
        }
    }
}

/* ── Login form submit spinner ── */
(function () {
    var form = document.getElementById('loginForm');
    if (!form) return;
    form.addEventListener('submit', function () {
        var btn = document.getElementById('loginBtn');
        var spin = document.getElementById('loginSpin');
        var txt = document.getElementById('loginTxt');
        if (btn) btn.disabled = true;
        if (spin) spin.style.display = 'block';
        if (txt) txt.textContent = 'Signing in…';
    });
})();

/* ── Registration form submit with terms check ── */
(function () {
    var form = document.getElementById('regForm');
    if (!form) return;
    form.addEventListener('submit', function (e) {
        var terms = document.getElementById('terms');
        var termsErr = document.getElementById('termsErr');

        if (terms && !terms.checked) {
            if (termsErr) termsErr.style.display = 'block';
            e.preventDefault();
            return;
        }
        if (termsErr) termsErr.style.display = 'none';

        var btn = document.getElementById('regBtn');
        var spin = document.getElementById('regSpin');
        var txt = document.getElementById('regTxt');
        if (btn) btn.disabled = true;
        if (spin) spin.style.display = 'block';
        if (txt) txt.textContent = 'Creating account…';
    });
})();