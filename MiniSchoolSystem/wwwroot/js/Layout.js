/* ═══════════════════════════════════════════════
   Layout.js — SabiSpace public layout scripts
   NOTE: This is already inlined in _Layout.cshtml.
   Only use this file if you prefer an external JS.
   If using inline, DELETE this file to avoid duplication.
═══════════════════════════════════════════════ */

/* ── Restore theme before first paint ── */
(function () {
    var t = localStorage.getItem('sabi-theme') || 'dark';
    document.getElementById('htmlRoot')?.setAttribute('data-theme', t);
})();

/* ── Toggle theme ── */
function toggleTheme() {
    var h = document.getElementById('htmlRoot');
    var n = h.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
    h.setAttribute('data-theme', n);
    localStorage.setItem('sabi-theme', n);
}

/* ── Progress bar ── */
(function () {
    var bar = document.getElementById('progressBar'), w = 0;
    if (!bar) return;
    var iv = setInterval(function () { w = Math.min(w + Math.random() * 18, 85); bar.style.width = w + '%'; }, 120);
    window.addEventListener('load', function () {
        clearInterval(iv);
        bar.style.width = '100%';
        setTimeout(function () { bar.style.opacity = '0'; }, 400);
    });
})();

/* ── Nav scroll shadow ── */
window.addEventListener('scroll', function () {
    document.getElementById('mainNav')?.classList.toggle('scrolled', scrollY > 20);
}, { passive: true });

/* ── Mobile drawer ── */
function toggleDrawer() {
    var d = document.getElementById('navDrawer');
    var b = document.getElementById('burger');
    if (!d) return;
    var open = d.classList.toggle('open');
    b?.classList.toggle('open', open);
    document.body.style.overflow = open ? 'hidden' : '';
}
document.getElementById('navDrawer')?.addEventListener('click', function (e) {
    if (e.target === this) toggleDrawer();
});

/* ── Announce bar dismiss ── */
function dismissAnnounce() {
    var b = document.getElementById('announceBar');
    if (!b) return;
    b.style.transition = 'opacity .3s, max-height .4s';
    b.style.opacity = '0';
    b.style.maxHeight = '0';
    b.style.padding = '0';
    b.style.overflow = 'hidden';
    sessionStorage.setItem('sabi-announce-dismissed', '1');
}
if (sessionStorage.getItem('sabi-announce-dismissed')) dismissAnnounce();

/* ── Toast system ── */
window.SabiToast = function (msg, type, duration, title) {
    type = type || 'info';
    duration = duration || 4500;
    var icons = { success: '✅', error: '❌', info: 'ℹ️', warning: '⚠️' };
    var zone = document.getElementById('toastZone');
    if (!zone) return;
    var t = document.createElement('div');
    t.className = 'toast ' + type;
    t.innerHTML =
        '<span class="t-icon">' + (icons[type] || 'ℹ️') + '</span>' +
        '<div class="t-body">' +
        (title ? '<div class="t-title">' + title + '</div>' : '') +
        '<div class="t-msg">' + msg + '</div>' +
        '</div>' +
        '<button class="t-close" onclick="this.closest(\'.toast\').remove()">×</button>';
    zone.appendChild(t);
    setTimeout(function () {
        t.style.animation = 'toast-out .35s forwards';
        setTimeout(function () { t.remove(); }, 370);
    }, duration);
};

/* ── Newsletter ── */
function subscribeNewsletter() {
    var e = document.getElementById('newsletterEmail')?.value.trim();
    if (!e || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(e)) {
        SabiToast('Please enter a valid email.', 'warning');
        return;
    }
    SabiToast("You're subscribed! Welcome to SabiSpace 🎉", 'success', 5000, 'Subscribed!');
    document.getElementById('newsletterEmail').value = '';
}

/* ── Fade-up on scroll ── */
(function () {
    var fels = document.querySelectorAll('.fade-up');
    if (!fels.length) return;
    var fio = new IntersectionObserver(function (en) {
        en.forEach(function (e) {
            if (e.isIntersecting) { e.target.classList.add('visible'); fio.unobserve(e.target); }
        });
    }, { threshold: 0.1 });
    fels.forEach(function (el) { fio.observe(el); });
})();

/* ── Global search ── */
document.getElementById('globalSearch')?.addEventListener('keydown', function (e) {
    if (e.key === 'Enter' && this.value.trim())
        SabiToast('Searching for "' + this.value.trim() + '"…', 'info');
});