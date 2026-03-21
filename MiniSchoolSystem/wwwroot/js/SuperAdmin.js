/* ═══════════════════════════════════════════════
   SuperAdmin.js — shared across all SA views
═══════════════════════════════════════════════ */

/* ── Sidebar toggle (mobile) ── */
(function () {
    var toggleBtn = document.getElementById('saSidebarToggle');
    var sidebar = document.getElementById('saSidebar');
    var overlay = document.getElementById('saSidebarOverlay');

    function openSidebar() { sidebar?.classList.add('open'); overlay?.classList.add('open'); document.body.style.overflow = 'hidden'; }
    function closeSidebar() { sidebar?.classList.remove('open'); overlay?.classList.remove('open'); document.body.style.overflow = ''; }

    toggleBtn?.addEventListener('click', function () {
        sidebar?.classList.contains('open') ? closeSidebar() : openSidebar();
    });
    overlay?.addEventListener('click', closeSidebar);
})();

/* ── Active nav link ── */
(function () {
    var links = document.querySelectorAll('.sa-nav a');
    var path = window.location.pathname.toLowerCase();
    links.forEach(function (a) {
        if (a.getAttribute('href') && path.includes(a.getAttribute('href').toLowerCase().replace(/\/$/, ''))) {
            a.classList.add('active');
        }
    });
})();

/* ── Stat counter animation ── */
function animateCount(el) {
    var target = parseFloat(el.dataset.target || el.innerText.replace(/[^0-9.]/g, ''));
    var prefix = el.dataset.prefix || '';
    var suffix = el.dataset.suffix || '';
    var isFloat = el.dataset.float === 'true';
    var duration = 1200;
    var start = null;

    function step(timestamp) {
        if (!start) start = timestamp;
        var progress = Math.min((timestamp - start) / duration, 1);
        var eased = 1 - Math.pow(1 - progress, 3);
        var value = target * eased;
        el.innerText = prefix + (isFloat ? value.toFixed(2) : Math.floor(value).toLocaleString()) + suffix;
        if (progress < 1) requestAnimationFrame(step);
    }
    requestAnimationFrame(step);
}

(function () {
    var counters = document.querySelectorAll('[data-count]');
    if (!counters.length) return;
    var io = new IntersectionObserver(function (entries) {
        entries.forEach(function (e) {
            if (e.isIntersecting) {
                animateCount(e.target);
                io.unobserve(e.target);
            }
        });
    }, { threshold: 0.3 });
    counters.forEach(function (el) { io.observe(el); });
})();

/* ── Table live search ── */
window.saTableSearch = function (inputId, tableId) {
    var input = document.getElementById(inputId);
    var table = document.getElementById(tableId);
    if (!input || !table) return;

    input.addEventListener('keyup', function () {
        var q = this.value.toLowerCase();
        var rows = table.querySelectorAll('tbody tr');
        rows.forEach(function (row) {
            row.style.display = row.innerText.toLowerCase().includes(q) ? '' : 'none';
        });
        // show empty state if all hidden
        var visible = Array.from(rows).filter(function (r) { return r.style.display !== 'none'; });
        var emptyRow = table.querySelector('.sa-empty-row');
        if (emptyRow) emptyRow.style.display = visible.length === 0 ? '' : 'none';
    });
};

/* ── Select filter ── */
window.saTableFilter = function (selectId, tableId, colIndex) {
    var sel = document.getElementById(selectId);
    var table = document.getElementById(tableId);
    if (!sel || !table) return;

    sel.addEventListener('change', function () {
        var val = this.value.toLowerCase();
        var rows = table.querySelectorAll('tbody tr:not(.sa-empty-row)');
        rows.forEach(function (row) {
            var cell = row.cells[colIndex];
            if (!val || (cell && cell.innerText.toLowerCase().includes(val))) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    });
};

/* ── Modal helpers ── */
window.saOpenModal = function (id) { document.getElementById(id)?.classList.add('open'); document.body.style.overflow = 'hidden'; };
window.saCloseModal = function (id) { document.getElementById(id)?.classList.remove('open'); document.body.style.overflow = ''; };

// Close on backdrop click
document.querySelectorAll('.sa-modal-backdrop').forEach(function (el) {
    el.addEventListener('click', function (e) {
        if (e.target === el) saCloseModal(el.id);
    });
});

// Close on Escape
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        document.querySelectorAll('.sa-modal-backdrop.open').forEach(function (el) {
            saCloseModal(el.id);
        });
    }
});

/* ── Confirm danger actions ── */
window.saConfirm = function (message, formOrCallback) {
    // Uses SabiToast-style modal if available, else native confirm
    if (confirm(message)) {
        if (typeof formOrCallback === 'function') formOrCallback();
        else if (formOrCallback instanceof HTMLFormElement) formOrCallback.submit();
    }
};

/* ── Lock / Unlock toggle (ToggleLock form) ── */
document.querySelectorAll('.js-lock-toggle').forEach(function (btn) {
    btn.addEventListener('click', function () {
        var name = this.dataset.name || 'this user';
        var locked = this.dataset.locked === 'true';
        var action = locked ? 'unlock' : 'lock';
        var msg = 'Are you sure you want to ' + action + ' ' + name + '?';
        if (confirm(msg)) {
            this.closest('form').submit();
        }
    });
});

/* ── Role badge colour helper ── */
window.saRoleClass = function (role) {
    var map = { superadmin: 'role-superadmin', admin: 'role-admin', teacher: 'role-teacher', student: 'role-student' };
    return map[(role || '').toLowerCase()] || 'role-student';
};

/* ── Toast shortcut (wraps SabiToast from Layout) ── */
window.saToast = function (msg, type) {
    if (window.SabiToast) SabiToast(msg, type || 'info');
    else console.log('[' + (type || 'info').toUpperCase() + '] ' + msg);
};

/* ── Fade-up on scroll ── */
(function () {
    var els = document.querySelectorAll('.sa-fade-up');
    if (!els.length) return;
    var io = new IntersectionObserver(function (entries) {
        entries.forEach(function (e) {
            if (e.isIntersecting) { e.target.classList.add('visible'); io.unobserve(e.target); }
        });
    }, { threshold: 0.1 });
    els.forEach(function (el) { io.observe(el); });
})();