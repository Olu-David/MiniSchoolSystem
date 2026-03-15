<script>
        // Get lesson ID from URL
    const lessonId = window.location.pathname.split('/').filter(Boolean).pop();

    async function loadLesson() {
            try {
                const res = await fetch('/Lesson/GetLessons');
    const data = await res.json();
                const lesson = data.find(l => String(l.id) === String(lessonId));

    document.getElementById('loadingSkeleton').style.display = 'none';
    document.getElementById('lessonContent').style.display = '';

    if (!lesson) {
        document.getElementById('lessonContent').innerHTML = `
                        <div class="empty-state card">
                            <div class="empty-state-icon"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg></div>
                            <h3>Lesson not found</h3>
                            <p>It may have been deleted or archived.</p>
                        </div>`;
    return;
                }

    document.getElementById('lessonTitle').textContent = lesson.title || '(No title)';
    document.getElementById('lessonDescription').textContent = lesson.description || '';
    document.getElementById('lessonSectionBadge').textContent = lesson.lessonSection || 'General';
    document.getElementById('lessonBody').innerHTML = lesson.content || '<p style="color:var(--text-muted)">No content available.</p>';
    document.getElementById('lessonCreated').textContent = lesson.createdAt
    ? new Date(lesson.createdAt).toLocaleDateString('en-US', {year: 'numeric', month: 'long', day: 'numeric' })
    : '—';

    if (lesson.fileUrl) {
        document.getElementById('attachmentRow').style.display = '';
    document.getElementById('attachmentLink').href = lesson.fileUrl;
                }

    // Edit btn
    const editBtn = document.getElementById('editLessonBtn');
    if (editBtn) editBtn.href = '/Lesson/EditLesson/' + lesson.id;

    // Delete
    const deleteBtn = document.getElementById('deleteLessonBtn');
    if (deleteBtn) {
        deleteBtn.addEventListener('click', () => openModal('deleteModal'));
                }
                document.getElementById('confirmDeleteBtn').addEventListener('click', async () => {
        closeModal('deleteModal');
    const r = await fetch('/Lesson/' + lesson.id, {method: 'DELETE' });
    if (r.ok) {
        window.showToast('Lesson scheduled for deletion.', 'success');
                        setTimeout(() => window.location = '/Lesson', 1500);
                    }
                });

    // Archive
    const archiveBtn = document.getElementById('archiveLessonBtn');
    if (archiveBtn) {
        archiveBtn.addEventListener('click', async () => {
            const ok = await window.confirmAction('Archive this lesson? Students will no longer see it.', 'Archive');
            if (!ok) return;
            const r = await fetch('/Lesson/archive/' + lesson.id, { method: 'PUT' });
            if (r.ok) window.showToast('Lesson archived.', 'success');
        });
                }

            } catch (e) {
        console.error(e);
            }
        }

    loadLesson();
</script>