async function loadLessons() {
    try {
        const res = await fetch('/Lesson/GetLessons');
        const data = await res.json();
        const tbody = document.getElementById('lessonsBody');

        document.getElementById('statLessons').textContent = data.length;

        if (!data || data.length === 0) {
            tbody.innerHTML = `
                        <tr><td colspan="6">
                            <div class="empty-state">
                                <div class="empty-state-icon">
                                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"/></svg>
                                </div>
                                <h3>No lessons yet</h3>
                                <p>Create your first lesson to get started.</p>
                            </div>
                        </td></tr>`;
            return;
        }

        tbody.innerHTML = data.map((lesson, i) => `
                    <tr>
                        <td style="color:var(--text-muted);font-size:0.78rem;font-weight:600;">${i + 1}</td>
                        <td>
                            <div style="font-weight:500;">${escapeHtml(lesson.title)}</div>
                            <div style="font-size:0.75rem;color:var(--text-muted);margin-top:2px;">${escapeHtml(lesson.description || '')}</div>
                        </td>
                        <td>
                            <span class="badge badge-section">${escapeHtml(lesson.lessonSection ?? '—')}</span>
                        </td>
                        <td>
                            <span class="badge badge-active">
                                <span class="badge-dot"></span>Active
                            </span>
                        </td>
                        <td style="color:var(--text-muted);font-size:0.82rem;">${formatDate(lesson.createdAt)}</td>
                        <td>
                            <div style="display:flex;gap:6px;flex-wrap:wrap;">
                                <a href="/Lesson/Details/${lesson.id}" class="btn btn-secondary btn-sm">View</a>
                                ${canTeacher ? `
                                <button class="btn btn-secondary btn-sm" data-archive-lesson="${lesson.id}">Archive</button>
                                <button class="btn btn-danger btn-sm" data-delete-lesson="${lesson.id}">Delete</button>
                                ` : ''}
                            </div>
                        </td>
                    </tr>
                `).join('');

        // Re-attach event listeners for dynamic buttons
        initTableActions();

    } catch (e) {
        document.getElementById('lessonsBody').innerHTML = `
                    <tr><td colspan="6" style="text-align:center;padding:24px;color:var(--text-muted);">
                        Failed to load lessons. Check console for details.
                    </td></tr>`;
    }
}

const canTeacher = @((User.IsInRole("Teacher") || User.IsInRole("SuperAdmin")) ? "true" : "false");

function escapeHtml(str) {
    const d = document.createElement('div');
    d.textContent = str ?? '';
    return d.innerHTML;
}

function formatDate(dateStr) {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
}

function initTableActions() {
    document.querySelectorAll('[data-delete-lesson]').forEach(btn => {
        btn.addEventListener('click', async function () {
            const id = this.dataset.deleteLesson;
            const confirmed = await window.confirmAction('This lesson will be scheduled for deletion.', 'Delete');
            if (!confirmed) return;
            const res = await fetch('/Lesson/' + id, { method: 'DELETE' });
            if (res.ok) {
                this.closest('tr').remove();
                window.showToast('Lesson scheduled for deletion.', 'success');
            }
        });
    });

    document.querySelectorAll('[data-archive-lesson]').forEach(btn => {
        btn.addEventListener('click', async function () {
            const id = this.dataset.archiveLesson;
            const confirmed = await window.confirmAction('Archive this lesson?', 'Archive');
            if (!confirmed) return;
            const res = await fetch('/Lesson/archive/' + id, { method: 'PUT' });
            if (res.ok) window.showToast('Archived successfully.', 'success');
        });
    });
}

// Search/filter
document.getElementById('searchLessons').addEventListener('input', function () {
    const q = this.value.toLowerCase();
    document.querySelectorAll('#lessonsBody tr').forEach(row => {
        row.style.display = row.textContent.toLowerCase().includes(q) ? '' : 'none';
    });
});

loadLessons();
  