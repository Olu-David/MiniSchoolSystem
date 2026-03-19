<script>
    function cnt(el,id,max){ var l=el.value.length,c=document.getElementById(id); c.textContent=l.toLocaleString()+' / '+max.toLocaleString(); c.className='cc'+(l>max*.9?' w':''); }
    function prevFile(inp){ var f=inp.files[0]; if(!f) return; var icons={pdf:'📄',doc:'📝',docx:'📝',ppt:'📊',pptx:'📊',xls:'📋',xlsx:'📋',mp4:'🎬',zip:'🗜️'}; var ext=f.name.split('.').pop().toLowerCase(); var sz=f.size>1048576?(f.size/1048576).toFixed(1)+' MB':(f.size/1024).toFixed(0)+' KB'; document.getElementById('fdi').textContent=icons[ext]||'📎'; document.getElementById('fdn').textContent=f.name; document.getElementById('fds').textContent=sz; document.getElementById('fdp').classList.add('show'); }
    function rmFile(){document.getElementById('fi').value = ''; document.getElementById('fdp').classList.remove('show'); }
    var d=document.getElementById('fdrop');
        ['dragover','dragenter'].forEach(e=>d.addEventListener(e,ev=>{ev.preventDefault();d.classList.add('dg');}));
        ['dragleave','drop'].forEach(e=>d.addEventListener(e,()=>d.classList.remove('dg')));
    function onSave(){ if(!document.getElementById('Content').value.trim()){SabiToast('Content cannot be empty.', 'warning'); return false; } var btn=document.getElementById('sb'); btn.disabled=true; document.getElementById('ss').style.display='block'; document.getElementById('st').textContent='Saving…'; }
    var dirty=false;
        document.getElementById('ef').addEventListener('input',()=>dirty=true);
        document.getElementById('sb').addEventListener('click',()=>dirty=false);
        window.addEventListener('beforeunload',e=>{if(dirty){e.preventDefault();e.returnValue='';}});
</script>