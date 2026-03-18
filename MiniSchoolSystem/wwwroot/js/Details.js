// 1. Get the ID at the very top
const lessonId = window.location.pathname.split('/').filter(Boolean).pop();

async function loadLesson(id) {
    // Guard: Stop immediately if there is no ID
    if (!id) {
        console.warn("No ID found in URL");
        return;
    }

    try {
        const res = await fetch('/Lesson/GetLessons');
        if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);

        const data = await res.json();

        // Ensure we actually got an array back before calling .find()
        if (!Array.isArray(data)) return;

        const lesson = data.find(l => String(l.id) === String(id));

        // Helper: Safe visibility toggle
        const toggle = (elId, display) => {
            const el = document.getElementById(elId);
            if (el) el.style.display = display;
        };

        toggle('loadingSkeleton', 'none');
        toggle('lessonContent', '');

        if (!lesson) {
            const content = document.getElementById('lessonContent');
            if (content) content.innerHTML = `<div class="alert">Lesson not found</div>`;
            return;
        }

        // --- Safe Assignments ---
        const setTxt = (elId, val) => {
            const el = document.getElementById(elId);
            if (el) el.textContent = val || '';
        };

        setTxt('lessonTitle', lesson.title);
        setTxt('lessonDescription', lesson.description);
        setTxt('lessonSectionBadge', lesson.lessonSection);

        // For HTML content (the lesson body)
        const bodyEl = document.getElementById('lessonBody');
        if (bodyEl) bodyEl.innerHTML = lesson.content || '';

        // Date formatting
        const dateEl = document.getElementById('lessonCreated');
        if (dateEl && lesson.createdAt) {
            dateEl.textContent = new Date(lesson.createdAt).toLocaleDateString();
        }

    } catch (e) {
        console.error("Critical Load Error:", e);
        const skeleton = document.getElementById('loadingSkeleton');
        if (skeleton) skeleton.textContent = "Error loading content.";
    }
}

// 2. Call it by passing the variable in
loadLesson(lessonId);