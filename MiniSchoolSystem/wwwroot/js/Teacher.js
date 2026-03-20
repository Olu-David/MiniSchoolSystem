/* ============================================================
   teacher.js  —  SabiSpace Teacher Portal JavaScript
   Views: Index (Dashboard), MyCourses, EditModule
   Future stubs: MyStudents, Analytics, Earnings, Schedule
   ============================================================ */

'use strict';

/* ============================================================
   UTILITIES
   ============================================================ */

function escHtml(str) {
    var d = document.createElement('div');
    d.textContent = String(str || '');
    return d.innerHTML;
}

function fmtDate(dateStr) {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('en-GB', {
        day: 'numeric', month: 'short', year: 'numeric'
    });
}

function fmtNum(n) {
    if (n >= 1000) return (n / 1000).toFixed(1) + 'K';
    return String(n || 0);
}

/* ============================================================
   TOAST
   ============================================================ */

function showToast(msg, type, ms) {
    // Delegate to SabiToast if the layout provides it
    if (typeof window.SabiToast === 'function') {
        window.SabiToast(msg, type || 'info', ms || 4000);
        return;
    }
    // Standalone fallback
    var zone = document.getElementById('toastZone');
    if (!zone) return;
    var t = document.createElement('div');
    t.className = 'toast ' + (type || 'info');
    t.innerHTML =
        '<span>' + escHtml(msg) + '</span>' +
        '<button class="toast-close" onclick="this.closest(\'.toast\').remove()">×</button>';
    zone.appendChild(t);
    setTimeout(function () {
        t.style.animation = 't-out .3s forwards';
        setTimeout(function () { t.remove(); }, 330);
    }, ms || 4000);
}

/* ============================================================
   SIDEBAR — mobile open/close
   ============================================================ */

function openSidebar() {
    var sb = document.getElementById('sidebar');
    var overlay = document.getElementById('sidebarOverlay');
    if (sb) sb.classList.add('open');
    if (overlay) overlay.classList.add('open');
    document.body.style.overflow = 'hidden';
}

function closeSidebar() {
    var sb = document.getElementById('sidebar');
    var overlay = document.getElementById('sidebarOverlay');
    if (sb) sb.classList.remove('open');
    if (overlay) overlay.classList.remove('open');
    document.body.style.overflow = '';
}

// Close on overlay click
document.addEventListener('DOMContentLoaded', function () {
    var overlay = document.getElementById('sidebarOverlay');
    if (overlay) overlay.addEventListener('click', closeSidebar);
});

/* ============================================================
   NAV ITEM  — mark active based on current path
   ============================================================ */

function markActiveNav() {
    var path = window.location.pathname.toLowerCase();
    var items = document.querySelectorAll('.nav-item[data-path]');
    items.forEach(function (item) {
        var p = (item.dataset.path || '').toLowerCase();
        if (p && path.includes(p)) {
            item.classList.add('active');
        }
    });
}

document.addEventListener('DOMContentLoaded', markActiveNav);

/* ============================================================
   MODAL — open / close
   ============================================================ */

function openModal(id) {
    var el = document.getElementById(id);
    if (el) el.classList.add('open');
}

function closeModal(id) {
    var el = document.getElementById(id);
    if (el) el.classList.remove('open');
}

// Close on backdrop click
document.addEventListener('click', function (e) {
    if (e.target.classList.contains('modal-backdrop')) {
        e.target.classList.remove('open');
    }
});

// Close on Escape key
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        document.querySelectorAll('.modal-backdrop.open').forEach(function (m) {
            m.classList.remove('open');
        });
    }
});

/* ============================================================
   SUBMIT SPINNER
   ============================================================ */

function showSpinner(btnId, spinId, txtId, loadingText) {
    var btn = document.getElementById(btnId);
    var spin = document.getElementById(spinId);
    var txt = document.getElementById(txtId);
    if (btn) btn.disabled = true;
    if (spin) spin.style.display = 'block';
    if (txt) txt.textContent = loadingText || 'Saving…';
}

/* ============================================================
   MODULE ACCORDION
   Used by: MyCourses
   ============================================================ */

function toggleModule(moduleEl) {
    moduleEl.classList.toggle('open');
}

function initModuleAccordions() {
    document.querySelectorAll('.module-header').forEach(function (header) {
        header.addEventListener('click', function () {
            toggleModule(this.closest('.module-item'));
        });
    });
}

document.addEventListener('DOMContentLoaded', initModuleAccordions);

/* ============================================================
   EDIT MODULE MODAL
   Used by: MyCourses — wire the edit buttons
   ============================================================ */

var currentEditModuleId = null;

/**
 * Open the EditModule modal and pre-fill with current values
 * @param {number} moduleId
 * @param {string} currentTitle
 */
function openEditModule(moduleId, currentTitle) {
    currentEditModuleId = moduleId;

    var titleInput = document.getElementById('editModuleTitle');
    var idInput = document.getElementById('editModuleId');
    var subtitle = document.getElementById('editModuleSub');

    if (titleInput) titleInput.value = currentTitle || '';
    if (idInput) idInput.value = moduleId;
    if (subtitle) subtitle.textContent = 'Editing module #' + moduleId;

    openModal('editModuleModal');
    if (titleInput) titleInput.focus();
}

/**
 * Submit the edit module form via fetch (AJAX)
 */
function submitEditModule() {
    var titleInput = document.getElementById('editModuleTitle');
    var titleErr = document.getElementById('editModuleTitleErr');
    var title = (titleInput ? titleInput.value.trim() : '');

    // Validate
    if (!title) {
        if (titleErr) titleErr.classList.add('show');
        if (titleInput) titleInput.classList.add('error');
        return;
    }
    if (titleErr) titleErr.classList.remove('show');
    if (titleInput) titleInput.classList.remove('error');

    var token = getAntiForgeryToken();

    showSpinner('saveModuleBtn', 'saveModuleSpin', 'saveModuleTxt', 'Saving…');

    fetch('/Teacher/EditModule', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify({
            CourseId: currentEditModuleId,
            Title: title
        })
    })
        .then(function (r) {
            if (r.ok || r.redirected) {
                showToast('Module updated successfully!', 'success');
                closeModal('editModuleModal');
                // Update the title in the DOM without full reload
                var titleEl = document.querySelector('[data-module-id="' + currentEditModuleId + '"] .module-title');
                if (titleEl) titleEl.textContent = title;
            } else {
                showToast('Could not save changes. Please try again.', 'error');
            }
        })
        .catch(function () {
            showToast('Network error. Please check your connection.', 'error');
        })
        .finally(function () {
            var btn = document.getElementById('saveModuleBtn');
            var spin = document.getElementById('saveModuleSpin');
            var txt = document.getElementById('saveModuleTxt');
            if (btn) btn.disabled = false;
            if (spin) spin.style.display = 'none';
            if (txt) txt.textContent = 'Save Changes';
        });
}

/* ============================================================
   STAT COUNTER ANIMATION
   Used by: Dashboard — animates the numbers up on load
   ============================================================ */

function animateCounter(el, target, duration) {
    if (!el) return;
    var start = 0;
    var startTime = null;
    var num = parseInt(target, 10) || 0;
    duration = duration || 1200;

    function step(timestamp) {
        if (!startTime) startTime = timestamp;
        var progress = Math.min((timestamp - startTime) / duration, 1);
        var eased = 1 - Math.pow(1 - progress, 3); // ease-out cubic
        el.textContent = Math.round(eased * num).toLocaleString();
        if (progress < 1) requestAnimationFrame(step);
    }
    requestAnimationFrame(step);
}

function initCounterAnimations() {
    document.querySelectorAll('[data-counter]').forEach(function (el) {
        var target = el.dataset.counter;
        // Animate when element enters viewport
        var io = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    animateCounter(el, target);
                    io.unobserve(el);
                }
            });
        }, { threshold: 0.3 });
        io.observe(el);
    });
}

document.addEventListener('DOMContentLoaded', initCounterAnimations);

/* ============================================================
   SEARCH  — filter courses / students client-side
   ============================================================ */

/**
 * Generic client-side search filter
 * @param {string} inputId    - ID of the search input
 * @param {string} itemsSelector - CSS selector for the items to filter
 * @param {string} textSelector  - CSS selector inside each item to search
 */
function initSearch(inputId, itemsSelector, textSelector) {
    var input = document.getElementById(inputId);
    if (!input) return;

    input.addEventListener('input', function () {
        var q = this.value.toLowerCase().trim();
        var items = document.querySelectorAll(itemsSelector);
        items.forEach(function (item) {
            var text = (item.querySelector(textSelector) || item).textContent.toLowerCase();
            item.style.display = (!q || text.includes(q)) ? '' : 'none';
        });
    });
}

document.addEventListener('DOMContentLoaded', function () {
    // Courses search
    initSearch('courseSearch', '.course-card', '.course-title');
    // Students search
    initSearch('studentSearch', '.student-row', '.table-name');
});

/* ============================================================
   PROGRESS BAR ANIMATION
   ============================================================ */

function initProgressBars() {
    var io = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                var fill = entry.target.querySelector('.progress-fill');
                var pct = entry.target.dataset.pct || '0';
                if (fill) fill.style.width = pct + '%';
                io.unobserve(entry.target);
            }
        });
    }, { threshold: 0.2 });

    document.querySelectorAll('.progress-wrap[data-pct]').forEach(function (el) {
        var fill = el.querySelector('.progress-fill');
        if (fill) fill.style.width = '0%';
        io.observe(el);
    });
}

document.addEventListener('DOMContentLoaded', initProgressBars);

/* ============================================================
   ANTI-FORGERY TOKEN
   ============================================================ */

function getAntiForgeryToken() {
    var el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
}

/* ============================================================
   FUTURE FEATURE STUBS
   These functions are wired to "coming soon" buttons.
   Replace the body with real implementation when ready.
   ============================================================ */

// ── MyStudents ──────────────────────────────────────────────
function viewStudent(studentId) {
    // TODO: navigate to student profile or open a detail modal
    showToast('Student profiles coming soon!', 'info');
}

function messageStudent(studentId) {
    // TODO: open messaging modal or redirect to messages page
    showToast('Messaging feature coming soon!', 'info');
}

// ── Analytics ───────────────────────────────────────────────
function loadAnalytics(period) {
    // TODO: fetch analytics data for selected period (7d, 30d, 90d)
    // and update the chart
    showToast('Analytics coming soon!', 'info');
}

// ── Earnings ────────────────────────────────────────────────
function requestPayout() {
    // TODO: open payout request modal, validate bank details
    showToast('Earnings & payouts coming soon!', 'info');
}

// ── Schedule ────────────────────────────────────────────────
function createScheduleEvent() {
    // TODO: open calendar event creation modal
    showToast('Schedule management coming soon!', 'info');
}

// ── Notifications ────────────────────────────────────────────
function markAllRead() {
    // TODO: POST /Teacher/MarkNotificationsRead
    document.querySelectorAll('.notif-item.unread').forEach(function (el) {
        el.classList.remove('unread');
    });
    var dot = document.querySelector('.notif-dot');
    if (dot) dot.style.display = 'none';
    showToast('All notifications marked as read', 'success');
}

// ── Course actions ───────────────────────────────────────────
function archiveCourse(courseId) {
    // TODO: POST /Teacher/ArchiveCourse/{id}
    showToast('Course archiving coming soon!', 'info');
}

function duplicateCourse(courseId) {
    // TODO: POST /Teacher/DuplicateCourse/{id}
    showToast('Course duplication coming soon!', 'info');
}

function addModule(courseId) {
    // TODO: open add module modal
    showToast('Adding modules coming soon!', 'info');
}