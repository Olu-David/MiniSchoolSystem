/* ============================================================
   sabispace.js  —  SabiSpace Combined JavaScript
   Covers: Lesson views · Teacher Portal · Shared utilities
   ============================================================ */

'use strict';


/* ────────────────────────────────────────────────────────
   LESSON + SHARED UTILITIES
   ──────────────────────────────────────────────────────── */
/* ============================================================
   lesson.js  —  SabiSpace Lesson Views shared JavaScript
   Used by: Index, Details, Create, Edit, Delete,
            Archive, Restore
   ============================================================ */



/* ============================================================
   UTILITIES
   ============================================================ */

/**
 * Escape HTML to prevent XSS when injecting dynamic content
 * @param {string} str
 * @returns {string}
 */
function escHtml(str) {
    var d = document.createElement('div');
    d.textContent = String(str || '');
    return d.innerHTML;
}

/**
 * Format a date string to "dd Mon yyyy"
 * @param {string} dateStr
 * @returns {string}
 */
function fmtDate(dateStr) {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('en-GB', {
        day: 'numeric', month: 'short', year: 'numeric'
    });
}

/**
 * Format a file size in bytes to a readable string
 * @param {number} bytes
 * @returns {string}
 */
function fmtFileSize(bytes) {
    if (bytes > 1048576) return (bytes / 1048576).toFixed(1) + ' MB';
    return (bytes / 1024).toFixed(0) + ' KB';
}

/**
 * Get the lesson ID from the current URL path (last segment)
 * @returns {string|null}
 */
function getLessonIdFromUrl() {
    var parts = window.location.pathname.split('/').filter(Boolean);
    var id = parts[parts.length - 1];
    return (!isNaN(id) && id) ? id : null;
}

/* ============================================================
   FILE UPLOAD HANDLERS
   Used by: CreateLesson, EditLesson
   ============================================================ */

var FILE_ICONS = {
    pdf: '📄', doc: '📝', docx: '📝',
    ppt: '📊', pptx: '📊',
    xls: '📋', xlsx: '📋',
    mp4: '🎬', zip: '🗜️', txt: '📃'
};

/**
 * Preview a selected file in the UI
 * @param {HTMLInputElement} input
 * @param {string} previewId   - ID of the .file-preview element
 * @param {string} dropTitleId - ID of the drop zone title span
 */
function previewFile(input, previewId, dropTitleId) {
    var file = input.files[0];
    if (!file) return;

    var ext = file.name.split('.').pop().toLowerCase();
    var icon = FILE_ICONS[ext] || '📎';
    var size = fmtFileSize(file.size);

    var preview = document.getElementById(previewId);
    if (preview) {
        preview.querySelector('.file-preview-name').textContent = file.name;
        preview.querySelector('.file-preview-size').textContent = size;
        preview.querySelector('[data-file-icon]').textContent = icon;
        preview.classList.add('show');
    }

    var titleEl = document.getElementById(dropTitleId);
    if (titleEl) titleEl.textContent = 'File selected ✓';
}

/**
 * Remove the selected file and reset the drop zone
 * @param {string} inputId
 * @param {string} previewId
 * @param {string} dropTitleId
 * @param {string} defaultTitle
 */
function removeFile(inputId, previewId, dropTitleId, defaultTitle) {
    var input = document.getElementById(inputId);
    if (input) input.value = '';

    var preview = document.getElementById(previewId);
    if (preview) preview.classList.remove('show');

    var titleEl = document.getElementById(dropTitleId);
    if (titleEl) titleEl.textContent = defaultTitle || 'Drag & drop or click to upload';
}

/**
 * Wire drag-and-drop highlight to a drop zone element
 * @param {HTMLElement} dropEl
 */
function initDragDrop(dropEl) {
    if (!dropEl) return;
    ['dragover', 'dragenter'].forEach(function (e) {
        dropEl.addEventListener(e, function (ev) {
            ev.preventDefault();
            dropEl.classList.add('dragover');
        });
    });
    ['dragleave', 'drop'].forEach(function (e) {
        dropEl.addEventListener(e, function () {
            dropEl.classList.remove('dragover');
        });
    });
}

/* ============================================================
   CHARACTER COUNTER
   Used by: CreateLesson, EditLesson
   ============================================================ */

/**
 * Update a character counter display
 * @param {HTMLElement} inputEl
 * @param {string}      counterId
 * @param {number}      max
 */
function countChars(inputEl, counterId, max) {
    var len = inputEl.value.length;
    var el = document.getElementById(counterId);
    if (!el) return;
    el.textContent = len.toLocaleString() + ' / ' + max.toLocaleString();
    el.className = 'char-count' + (len > max * .9 ? ' warn' : '');
}

/* ============================================================
   CHANGED FIELD INDICATOR
   Used by: EditLesson
   ============================================================ */

/**
 * Append a pulsing dot to a label when its field is edited
 * @param {HTMLElement} labelEl
 */
function markChanged(labelEl) {
    if (!labelEl || labelEl.querySelector('.changed-dot')) return;
    var dot = document.createElement('span');
    dot.className = 'changed-dot';
    labelEl.appendChild(dot);
}

/* ============================================================
   CONFIRM CHECKBOX → BUTTON ENABLE
   Used by: DeleteLesson, ArchiveLesson, RestoreLesson
   ============================================================ */

/**
 * Enable/disable a button based on a checkbox state
 * @param {string} checkboxId
 * @param {string} buttonId
 */
function toggleConfirmBtn(checkboxId, buttonId) {
    var cb = document.getElementById(checkboxId);
    var btn = document.getElementById(buttonId);
    if (cb && btn) btn.disabled = !cb.checked;
}

/* ============================================================
   SUBMIT SPINNER
   Used by: all forms
   ============================================================ */

/**
 * Show spinner and disable the submit button on form submission
 * @param {string} btnId
 * @param {string} spinId
 * @param {string} txtId
 * @param {string} loadingText
 */
function showSubmitSpinner(btnId, spinId, txtId, loadingText) {
    var btn = document.getElementById(btnId);
    var spin = document.getElementById(spinId);
    var txt = document.getElementById(txtId);
    if (btn) btn.disabled = true;
    if (spin) spin.style.display = 'block';
    if (txt) txt.textContent = loadingText || 'Processing…';
}

/* ============================================================
   STEP WIZARD
   Used by: CreateLesson
   ============================================================ */

var currentStep = 1;
var TOTAL_STEPS = 3;

/**
 * Navigate to a step in the multi-step form
 * @param {number} targetStep
 * @param {Function} [validateFn] - optional validation before advancing
 */
function goStep(targetStep, validateFn) {
    // Run validation if provided
    if (validateFn && targetStep > currentStep) {
        if (!validateFn(currentStep)) return;
    }

    // Hide all panels
    for (var i = 1; i <= TOTAL_STEPS; i++) {
        var panel = document.getElementById('step' + i);
        if (panel) panel.classList.remove('active');

        var sn = document.getElementById('sn' + i);
        var sl = document.getElementById('sl' + i);
        if (!sn || !sl) continue;

        if (i < targetStep) {
            sn.className = 'step-num done';
            sn.textContent = '✓';
            sl.className = 'step-label active';
        } else if (i === targetStep) {
            sn.className = 'step-num active';
            sn.textContent = String(i);
            sl.className = 'step-label active';
        } else {
            sn.className = 'step-num pending';
            sn.textContent = String(i);
            sl.className = 'step-label pending';
        }
    }

    var targetPanel = document.getElementById('step' + targetStep);
    if (targetPanel) targetPanel.classList.add('active');

    currentStep = targetStep;
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

/* ============================================================
   LESSON CARD RENDERER
   Used by: Index
   ============================================================ */

var CARD_THUMBS = ['💻', '📊', '🎨', '💰', '🤖', '🧪', '📐', '🌍', '📖', '🎵'];
var CARD_BGS = [
    'linear-gradient(135deg,#1a1c22,#2a1810)',
    'linear-gradient(135deg,#0c1a12,#1a2a1a)',
    'linear-gradient(135deg,#1a1228,#0c1020)',
    'linear-gradient(135deg,#1a1500,#2a2000)',
    'linear-gradient(135deg,#001a1a,#001028)'
];

/**
 * Render lesson cards into a grid container
 * @param {Array}       lessons
 * @param {HTMLElement} gridEl
 * @param {HTMLElement} countEl
 */
function renderLessons(lessons, gridEl, countEl) {
    if (!gridEl) return;

    if (countEl) {
        countEl.textContent = lessons.length + ' lesson' + (lessons.length !== 1 ? 's' : '');
    }

    if (!lessons.length) {
        gridEl.innerHTML =
            '<div class="empty-state">' +
            '<div class="empty-icon">📭</div>' +
            '<h3>No lessons found</h3>' +
            '<p>Try adjusting your search or filter.</p>' +
            '</div>';
        return;
    }

    var now = Date.now();
    var weekMs = 7 * 24 * 60 * 60 * 1000;

    gridEl.innerHTML = lessons.map(function (l, i) {
        var isNew = new Date(l.createdAt).getTime() > now - weekMs;
        var statusBadge = isNew
            ? '<span class="thumb-status thumb-new">✨ New</span>'
            : '<span class="thumb-status thumb-live">Live</span>';

        var teacherInit = (l.teacherName || 'T').charAt(0).toUpperCase();

        return (
            '<a class="lesson-card" href="/Lesson/Details/' + l.id + '">' +
            '<div class="card-thumb">' +
            '<div class="card-thumb-bg" style="background:' + CARD_BGS[i % CARD_BGS.length] + '">' +
            CARD_THUMBS[i % CARD_THUMBS.length] +
            '</div>' +
            '<div class="card-thumb-overlay"><div class="play-circle">▶</div></div>' +
            statusBadge +
            '</div>' +
            '<div class="card-body">' +
            '<div class="card-section">' + escHtml(l.lessonSection || 'General') + '</div>' +
            '<h3 class="card-title-text">' + escHtml(l.title || '') + '</h3>' +
            '<p class="card-desc-text">' + escHtml(l.description || '') + '</p>' +
            '<div class="card-footer-row">' +
            '<div class="card-teacher-wrap">' +
            '<div class="teacher-av">' + escHtml(teacherInit) + '</div>' +
            '<span class="teacher-name-text">' + escHtml(l.teacherName || 'Teacher') + '</span>' +
            '</div>' +
            '<span class="card-date">' + fmtDate(l.createdAt) + '</span>' +
            '</div>' +
            '</div>' +
            '</a>'
        );
    }).join('');
}

/**
 * Filter lessons by search query and section
 * @param {Array}  allLessons
 * @param {string} query
 * @param {string} section
 * @returns {Array}
 */
function filterLessons(allLessons, query, section) {
    var q = (query || '').toLowerCase();
    var s = section || '';
    return allLessons.filter(function (l) {
        var matchQ = !q || (l.title || '').toLowerCase().includes(q)
            || (l.description || '').toLowerCase().includes(q);
        var matchS = !s || (l.lessonSection || '') === s;
        return matchQ && matchS;
    });
}

/* ============================================================
   DETAIL PAGE — TAB SWITCHING
   Used by: Details
   ============================================================ */

/**
 * Switch active tab panel
 * @param {string} panelId
 * @param {HTMLElement} btnEl
 */
function switchTab(panelId, btnEl) {
    document.querySelectorAll('.tab-panel').forEach(function (p) {
        p.classList.remove('active');
    });
    document.querySelectorAll('.tab-btn').forEach(function (b) {
        b.classList.remove('active');
    });
    var panel = document.getElementById(panelId);
    if (panel) panel.classList.add('active');
    if (btnEl) btnEl.classList.add('active');
}

/* ============================================================
   DETAIL PAGE — ACTION CALLS (Archive / Restore / Delete)
   Used by: Details sidebar buttons
   ============================================================ */

/**
 * Send a PUT request (archive or restore)
 * @param {string} url
 * @param {string} successMsg
 */
function sendPut(url, successMsg) {
    var token = getAntiForgeryToken();
    fetch(url, {
        method: 'PUT',
        headers: { 'RequestVerificationToken': token }
    })
        .then(function (r) {
            if (r.ok) {
                showToast(successMsg, 'success');
                setTimeout(function () { location.reload(); }, 1200);
            } else {
                showToast('Action failed. Please try again.', 'error');
            }
        })
        .catch(function () {
            showToast('Network error. Please check your connection.', 'error');
        });
}

/**
 * Send a DELETE request
 * @param {string} url
 * @param {string} successMsg
 * @param {string} redirectUrl
 */
function sendDelete(url, successMsg, redirectUrl) {
    var token = getAntiForgeryToken();
    fetch(url, {
        method: 'DELETE',
        headers: { 'RequestVerificationToken': token }
    })
        .then(function (r) {
            if (r.ok) {
                showToast(successMsg, 'success');
                setTimeout(function () {
                    window.location.href = redirectUrl || '/Lesson';
                }, 1200);
            } else {
                showToast('Could not delete lesson.', 'error');
            }
        })
        .catch(function () {
            showToast('Network error. Please check your connection.', 'error');
        });
}

/* ============================================================
   MODAL
   Used by: Details (delete confirm modal)
   ============================================================ */

/**
 * Open a modal by ID
 * @param {string} modalId
 */
function openModal(modalId) {
    var el = document.getElementById(modalId);
    if (el) el.classList.add('open');
}

/**
 * Close a modal by ID
 * @param {string} modalId
 */
function closeModal(modalId) {
    var el = document.getElementById(modalId);
    if (el) el.classList.remove('open');
}

// Close modals on backdrop click
document.addEventListener('click', function (e) {
    if (e.target.classList.contains('modal-backdrop')) {
        e.target.classList.remove('open');
    }
});

/* ============================================================
   TOAST NOTIFICATION
   ============================================================ */

/**
 * Show a toast notification
 * SabiToast is defined in _Layout.cshtml — this is a fallback
 * for standalone HTML usage
 * @param {string} msg
 * @param {string} type  - 'success' | 'error' | 'info' | 'warning'
 * @param {number} [ms]
 */
function showToast(msg, type, ms) {
    if (typeof window.SabiToast === 'function') {
        window.SabiToast(msg, type || 'info', ms || 4000);
        return;
    }
    // Fallback — simple alert
    alert(msg);
}

/* ============================================================
   ANTI-FORGERY TOKEN HELPER
   ============================================================ */

/**
 * Get the ASP.NET anti-forgery token value from the page
 * @returns {string}
 */
function getAntiForgeryToken() {
    var el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
}

/* ============================================================
   UNSAVED CHANGES WARNING
   Used by: EditLesson
   ============================================================ */

/**
 * Warn user before leaving page if form is dirty
 * @param {string} formId
 * @param {string} submitBtnId - clicking submit clears the dirty flag
 */
function initUnsavedWarning(formId, submitBtnId) {
    var isDirty = false;
    var form = document.getElementById(formId);
    var btn = document.getElementById(submitBtnId);

    if (form) {
        form.addEventListener('input', function () { isDirty = true; });
    }
    if (btn) {
        btn.addEventListener('click', function () { isDirty = false; });
    }

    window.addEventListener('beforeunload', function (e) {
        if (isDirty) {
            e.preventDefault();
            e.returnValue = '';
        }
    });
}

/* ============================================================
   RESTORE PAGE — DAYS REMAINING RING
   Used by: RestoreLesson
   ============================================================ */

/**
 * Animate the SVG ring on the restore page
 * @param {number} daysRemaining  - 0 to 30
 * @param {number} totalDays      - e.g. 30
 */
function initTimerRing(daysRemaining, totalDays) {
    var fillEl = document.querySelector('.ring-fill');
    if (!fillEl) return;
    var pct = Math.max(0, Math.min(1, daysRemaining / totalDays));
    var offset = 138 - (138 * pct);
    fillEl.style.strokeDashoffset = offset.toFixed(1);

    // Colour: green → amber → red as days run out
    if (pct > .5) fillEl.style.stroke = '#3dba8c';
    else if (pct > .2) fillEl.style.stroke = '#f5c842';
    else fillEl.style.stroke = '#f87171';
}



/* ────────────────────────────────────────────────────────
   TEACHER PORTAL
   ──────────────────────────────────────────────────────── */
/* ============================================================
   teacher.js  —  SabiSpace Teacher Portal JavaScript
   Views: Index (Dashboard), MyCourses, EditModule
   Future stubs: MyStudents, Analytics, Earnings, Schedule
   ============================================================ */



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



/* ────────────────────────────────────────────────────────
   COURSE CARD RENDERER (Course/Index)
   ──────────────────────────────────────────────────────── */

var COURSE_THUMBS = ['📚', '💻', '📊', '🎨', '💰', '🤖', '🧪', '📐', '🌍', '📖'];
var COURSE_BGS = [
    'linear-gradient(135deg,#1a1c22,#2a1810)',
    'linear-gradient(135deg,#0c1a12,#1a2a1a)',
    'linear-gradient(135deg,#1a1228,#0c1020)',
    'linear-gradient(135deg,#1a1500,#2a2000)',
    'linear-gradient(135deg,#001a1a,#001028)'
];

/**
 * Render course cards into a grid container
 */
function renderCourses(courses, gridEl, countEl) {
    if (!gridEl) return;
    if (countEl) countEl.textContent = courses.length + ' course' + (courses.length !== 1 ? 's' : '');
    if (!courses.length) {
        gridEl.innerHTML = '<div class="empty-state"><div class="empty-icon">📭</div><h3>No courses found</h3><p>Try adjusting your filter.</p></div>';
        return;
    }
    var now = Date.now(), weekMs = 7 * 24 * 60 * 60 * 1000;
    gridEl.innerHTML = courses.map(function (c, i) {
        var isNew = new Date(c.createdAt).getTime() > now - weekMs;
        var badge = isNew ? '<span class="thumb-badge tb-new">✨ New</span>' : '';
        var tInit = (c.teacherName || 'T').charAt(0).toUpperCase();
        return '<a class="course-card" href="/Course/Details/' + c.id + '">' +
            '<div class="card-thumb" style="background:' + COURSE_BGS[i % COURSE_BGS.length] + '">' +
            COURSE_THUMBS[i % COURSE_THUMBS.length] +
            '<div class="card-thumb-overlay"><div class="play-circle">▶</div></div>' +
            badge +
            '</div>' +
            '<div class="card-body">' +
            '<div class="card-section-tag">' + escHtml(c.courseSections || 'General') + '</div>' +
            '<h3 class="card-title">' + escHtml(c.title || '') + '</h3>' +
            '<p class="card-desc">' + escHtml(c.description || '') + '</p>' +
            '<div class="card-footer">' +
            '<div class="card-teacher-wrap">' +
            '<div class="teacher-av">' + escHtml(tInit) + '</div>' +
            '<span class="teacher-name">' + escHtml(c.teacherName || 'Instructor') + '</span>' +
            '</div>' +
            '<span class="card-meta-item">' + fmtDate(c.createdAt) + '</span>' +
            '</div>' +
            '</div>' +
            '</a>';
    }).join('');
}

/**
 * Filter courses by search query and section
 */
function filterCourses(allCourses, query, section) {
    var q = (query || '').toLowerCase();
    var s = section || '';
    return allCourses.filter(function (c) {
        var mq = !q || (c.title || '').toLowerCase().includes(q) || (c.description || '').toLowerCase().includes(q);
        var ms = !s || (c.courseSections || '') === s;
        return mq && ms;
    });
}

/**
 * Toggle a module accordion open/closed (used in Course/Details)
 */
function toggleMod(moduleId) {
    var el = document.getElementById(moduleId);
    if (el) el.classList.toggle('open');
}

/**
 * Enable/disable submit button based on checkbox (ArchiveCourse, Duplicate confirm pages)
 */
function toggleConfirmSubmit(checkboxId, buttonId) {
    var cb = document.getElementById(checkboxId);
    var btn = document.getElementById(buttonId);
    if (cb && btn) btn.disabled = !cb.checked;
}

/**
 * Show spinner on any confirm submit button
 */
function showSpin(btnId, spinId, txtId, msg) {
    var btn = document.getElementById(btnId);
    var spin = document.getElementById(spinId);
    var txt = document.getElementById(txtId);
    if (btn) btn.disabled = true;
    if (spin) spin.style.display = 'block';
    if (txt) txt.textContent = msg || 'Processing…';
}