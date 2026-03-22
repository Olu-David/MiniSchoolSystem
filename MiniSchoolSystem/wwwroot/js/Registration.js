
    /* Role toggle */
function selectRole(role) {
    console.log("Role selected: " + role); // 👈 This helps you debug!

    // 1. Set the hidden input value
    const roleInput = document.getElementById('roleInput');
    if (roleInput) roleInput.value = role;

    // 2. Handle Button Highlights
    // This turns 'on' for the clicked one and 'off' for the other
    document.getElementById('btnStudent').classList.toggle('on', role === 'Student');
    document.getElementById('btnParent').classList.toggle('on', role === 'Parent');

    // 3. Handle the Section Group Visibility
    const sg = document.getElementById('sectionGroup');
    const ss = document.getElementById('sectionSelect');

    if (role === 'Student') {
        sg.style.display = 'block'; // Or sg.classList.add('show')
        ss.required = true;
    } else {
        sg.style.display = 'none'; // Or sg.classList.remove('show')
        ss.required = false;
        ss.value = ""; // Clear the value so it doesn't send JSS1 for a Parent
    }
}

// 
    /* Password eye */
    function toggleEye(id, btn) {
      const inp = document.getElementById(id);
      const show = inp.type === 'password';
      inp.type = show ? 'text' : 'password';
      btn.querySelector('svg').innerHTML = show
        ? `<path d="M17.94 17.94A10.07 10.07 0 0112 20c-7 0-11-8-11-8a18.45 18.45 0 015.06-5.94"/><path d="M9.9 4.24A9.12 9.12 0 0112 4c7 0 11 8 11 8a18.5 18.5 0 01-2.16 3.19"/><line x1="1" y1="1" x2="23" y2="23"/>`
        : `<path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/>`;
    }

    /* Strength */
    function measureStrength(pw) {
      const wrap = document.getElementById('sw');
      const lbl  = document.getElementById('sl');
      wrap.style.display = pw.length ? 'block' : 'none';
      let s = 0;
      if (pw.length >= 8)         s++;
      if (/[A-Z]/.test(pw))        s++;
      if (/[0-9]/.test(pw))        s++;
      if (/[^A-Za-z0-9]/.test(pw)) s++;
      const lvls = [{c:'#f87171',t:'Too weak'},{c:'#fb923c',t:'Weak'},{c:'#facc15',t:'Good'},{c:'#4ade80',t:'Strong 💪'}];
      const idx = Math.max(0, s - 1);
      [1,2,3,4].forEach(i => document.getElementById('sb'+i).style.background = i <= s ? lvls[idx].c : 'rgba(255,255,255,.07)');
      lbl.textContent = pw.length ? lvls[idx].t : '';
      lbl.style.color = lvls[idx].c;
    }

    /* Submit */
    document.getElementById('regForm').addEventListener('submit', function (e) {
      const terms    = document.getElementById('terms');
      const termsErr = document.getElementById('termsErr');
      if (!terms.checked) {
        termsErr.style.display = 'block';
        e.preventDefault();
        return;
      }
      termsErr.style.display = 'none';
      const btn  = document.getElementById('regBtn');
      const spin = document.getElementById('regSpin');
      const txt  = document.getElementById('regTxt');
      btn.disabled = true; spin.style.display = 'block'; txt.textContent = 'Creating account…';
    });