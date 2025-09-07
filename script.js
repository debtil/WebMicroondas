
    // ---------- API CONFIG + SERVICE ----------
    class ApiConfig {
      static baseUrl = '';
      static token = localStorage.getItem('microwaveToken');
      static headers() {
        const h = { 'Content-Type': 'application/json' };
        if (this.token) h['Authorization'] = `Bearer ${this.token}`;
        return h;
      }
      static setBaseUrl(url){ this.baseUrl = url.replace(/\/+$/,''); }
      static setToken(t){ this.token = t; localStorage.setItem('microwaveToken', t); }
      static clearToken(){ this.token = null; localStorage.removeItem('microwaveToken'); }
    }

    class MicrowaveAPI {
      static async request(path, opts = {}) {
        const url = `${ApiConfig.baseUrl}${path}`;
        const res = await fetch(url, { headers: ApiConfig.headers(), ...opts });
        let body = null;
        try { body = await res.json(); } catch {}
        if (!res.ok || (body && body.success === false)) {
          const msg = (body && (body.message || body.title)) || `${res.status} ${res.statusText}`;
          throw new Error(msg);
        }
        return body?.data ?? body;
      }
      // AUTH
      static async login(username, password, apiUrl) {
        ApiConfig.setBaseUrl(apiUrl);
        const data = await this.request('/auth/login', { method:'POST', body:JSON.stringify({ username, password }) });
        ApiConfig.setToken(data.token);
        return true;
      }
      static async validate(){ await this.request('/auth/validate', { method:'POST' }); return true; }
      static disconnect(){ ApiConfig.clearToken(); }

      // PROGRAMS
      static getPrograms(){ return this.request('/programs'); }
      static createProgram(p){ return this.request('/programs', { method:'POST', body:JSON.stringify(p) }); }
      static deleteProgram(id){ return this.request(`/programs/${id}`, { method:'DELETE' }); }

      // HEATING
      static startHeating(req){ return this.request('/heating/start', { method:'POST', body:JSON.stringify(req) }); }
      static status(){ return this.request('/heating/status'); }
      static pause(){ return this.request('/heating/pause', { method:'POST' }); }
      static resume(){ return this.request('/heating/resume', { method:'POST' }); }
      static cancel(){ return this.request('/heating/cancel', { method:'POST' }); }
      static addTime(sec=30){ return this.request('/heating/add-time', { method:'POST', body:JSON.stringify({ additionalTime: sec }) }); }
      static complete(){ return this.request('/heating/complete', { method:'POST' }); }

      // LOGS
      static getLogs(){ return this.request('/logs'); }
      static clearLogs(){ return this.request('/logs', { method:'DELETE' }); }
    }

    // ---------- UI HELPERS ----------
    function updateApiStatus(){
      const el = document.getElementById('apiStatus');
      if (ApiConfig.token) { el.textContent='API: Conectada'; el.className='api-status api-connected'; }
      else { el.textContent='API: Desconectada'; el.className='api-status api-disconnected'; }
    }
    function openConfigModal(){ document.getElementById('configModal').style.display='block'; }
    function closeConfigModal(){ document.getElementById('configModal').style.display='none'; }

    async function connectAPI(){
      const username = document.getElementById('apiUsername').value.trim();
      const password = document.getElementById('apiPassword').value;
      const apiUrl = document.getElementById('apiUrl').value.trim();
      try {
        await MicrowaveAPI.login(username, password, apiUrl);
        updateApiStatus();
        await microwave.afterApiConnected();
        microwave.showMessage('Conectado à API com sucesso!', 'success');
        closeConfigModal();
      } catch(e){
        microwave.showMessage(e.message || 'Falha ao autenticar', 'error');
      }
    }
    function disconnectAPI(){
      MicrowaveAPI.disconnect();
      updateApiStatus();
      microwave.resetUi();
      microwave.showMessage('Desconectado da API', 'success');
      closeConfigModal();
    }

    async function loadLogs(){
      if (!ApiConfig.token) { document.getElementById('logViewer').textContent='Conecte à API para ver os logs.'; return; }
      try{
        const logs = await MicrowaveAPI.getLogs();
        const txt = (logs||[]).map(l=>{
          const ts = new Date(l.timestamp).toLocaleString('pt-BR');
          let line = `[${ts}] ${l.level}: ${l.message}`;
          if (l.exception) line += `\n  Exception: ${l.exception}`;
          return line;
        }).join('\n\n');
        const viewer = document.getElementById('logViewer');
        viewer.textContent = txt || 'Nenhum log.';
        viewer.scrollTop = viewer.scrollHeight;
      }catch(e){
        document.getElementById('logViewer').textContent = 'Erro ao carregar logs: ' + e.message;
      }
    }
    async function clearLogs(){
      try{ await MicrowaveAPI.clearLogs(); await loadLogs(); }
      catch(e){ microwave.showMessage(e.message,'error'); }
    }

    // ---------- CONTROLLER ----------
    class MicrowaveController {
      constructor(){
        this.currentTime = 0;
        this.currentPower = 10;
        this.isHeating = false;
        this.isPaused = false;
        this.heatingString = '';
        this.heatingChar='.';
        this.selectedProgram=null;
        this.programs=[];
        this.statusTimer=null;
      }

      async afterApiConnected(){ await this.loadPrograms(); this.startStatusPolling(); }
      resetUi(){
        this.stopStatusPolling();
        this.programs=[]; this.selectedProgram=null;
        this.currentTime=0; this.currentPower=10; this.isHeating=false; this.isPaused=false; this.heatingString=''; this.heatingChar='.';
        this.updateProgramsDisplay(); this.updateDisplay();
        document.getElementById('logViewer').textContent='';
      }

      async loadPrograms(){
        try{
          if (!ApiConfig.token) return;
          this.programs = await MicrowaveAPI.getPrograms();
          this.updateProgramsDisplay();
        }catch(e){
          this.showMessage('Erro ao carregar programas: '+e.message,'error');
        }
      }

      startStatusPolling(){
        this.stopStatusPolling();
        this.statusTimer = setInterval(async ()=>{
          try{
            if (!ApiConfig.token) return;
            const s = await MicrowaveAPI.status();
            this.isHeating = s.isHeating;
            this.isPaused = s.isPaused;
            this.currentTime = s.remainingTime;
            this.currentPower = s.power;
            this.heatingChar = s.heatingChar;

            if (this.isHeating && !this.isPaused) {
              this.heatingString += this.heatingChar.repeat(Math.max(1, s.power));
            } else if (!this.isHeating && this.currentTime===0 && !this.heatingString.endsWith('Aquecimento concluído')) {
              this.heatingString += (this.heatingString? ' ' : '') + 'Aquecimento concluído';
            }
            this.updateDisplay();
          }catch{}
        }, 1000);
      }
      stopStatusPolling(){ if (this.statusTimer){ clearInterval(this.statusTimer); this.statusTimer=null; } }

      formatTime(seconds){
        if (seconds>=60){ const m=Math.floor(seconds/60); const s=seconds%60; return `${m}:${s.toString().padStart(2,'0')}`; }
        return `00:${(seconds||0).toString().padStart(2,'0')}`;
      }
      updateDisplay(){
        document.getElementById('timeDisplay').textContent = this.formatTime(this.currentTime);
        document.getElementById('powerDisplay').textContent = `Potência: ${this.currentPower}`;
        const statusEl = document.getElementById('statusDisplay');
        if (this.isHeating && !this.isPaused) statusEl.textContent = this.heatingString || 'Aquecendo...';
        else if (this.isPaused) statusEl.textContent = 'Aquecimento pausado';
        else statusEl.textContent = this.heatingString || 'Micro-ondas pronto';
      }
      updateProgramsDisplay(){
        const grid = document.getElementById('programsGrid');
        grid.innerHTML='';
        if (!ApiConfig.token){
          grid.innerHTML = '<p style="color:#ccc;text-align:center;grid-column:1/-1">Conecte à API para ver os programas</p>';
          return;
        }
        this.programs.forEach(p=>{
          const card = document.createElement('div');
          card.className = 'program-card' + (p.isPredefined ? '' : ' custom');
          if (this.selectedProgram && this.selectedProgram.id === p.id) card.classList.add('selected');
          card.innerHTML = `
            <h4>${p.name}</h4>
            <div class="program-food">${p.food}</div>
            <div class="program-details">
              Tempo: ${this.formatTime(p.time)} | Potência: ${p.power} | Char: ${p.heatingChar}
            </div>
            ${!p.isPredefined ? `<button class="delete-btn" onclick="event.stopPropagation(); deleteProgram(${p.id})">×</button>` : ''}
          `;
          card.onclick = ()=> this.selectProgram(p);
          grid.appendChild(card);
        });
        if (!this.selectedProgram) document.getElementById('programInstructions').style.display='none';
      }
      selectProgram(p){
        this.selectedProgram=p;
        this.currentTime=p.time; this.currentPower=p.power; this.heatingChar=p.heatingChar;
        document.getElementById('timeInput').value=p.time; document.getElementById('powerInput').value=p.power;
        const inst = document.getElementById('programInstructions');
        inst.textContent = p.instructions || '—';
        inst.style.display='block';
        this.updateDisplay(); this.updateProgramsDisplay();
      }
      showMessage(msg, type='error'){
        const area = document.getElementById('messageArea');
        const el = document.createElement('div');
        el.className = (type==='success') ? 'success-message' : 'error-message';
        el.textContent = msg;
        area.innerHTML=''; area.appendChild(el);
        setTimeout(()=> area.innerHTML='', 4000);
      }

      validateTime(t){ if (t<1 || t>120) throw new Error('Tempo manual deve estar entre 1 e 120 segundos.'); }
      validatePower(p){ if (p<1 || p>10) throw new Error('Potência deve estar entre 1 e 10.'); }

      async createCustomProgram(name, food, time, power, heatingChar, instructions=''){
        if (!ApiConfig.token) throw new Error('Conecte à API.');
        if (!name?.trim()) throw new Error('Nome é obrigatório.');
        if (!food?.trim()) throw new Error('Alimento é obrigatório.');
        if (time==null || power==null || !heatingChar) throw new Error('Tempo, potência e caractere são obrigatórios.');
        await MicrowaveAPI.createProgram({ name, food, time, power, heatingChar, instructions, isPredefined:false });
        await this.loadPrograms();
        this.showMessage('Programa customizado criado!', 'success');
        this.clearProgramForm();
        showTab('programs');
      }
      async deleteCustomProgram(id){
        if (!ApiConfig.token) throw new Error('Conecte à API.');
        await MicrowaveAPI.deleteProgram(id);
        if (this.selectedProgram?.id===id) this.selectedProgram=null;
        await this.loadPrograms();
        this.heatingChar='.'; document.getElementById('programInstructions').style.display='none';
        this.showMessage('Programa deletado.', 'success');
      }

      async startHeating(time=null, power=null){
        if (!ApiConfig.token) throw new Error('Conecte à API.');
        // Add-time durante aquecimento manual
        if (this.isHeating && !this.isPaused) {
          if (this.selectedProgram && this.selectedProgram.isPredefined) { this.showMessage('Não é permitido acrescentar tempo em programa pré-definido.'); return; }
          await MicrowaveAPI.addTime(30);
          this.showMessage('Adicionados +30s', 'success');
          return;
        }
        // Retomar se pausado
        if (this.isPaused) { await MicrowaveAPI.resume(); this.showMessage('Aquecimento retomado','success'); return; }

        let body = {};
        if (this.selectedProgram) {
          body = { programId: this.selectedProgram.id };
        } else {
          const t = (time ?? parseInt(document.getElementById('timeInput').value)) || 30;
          const p = (power ?? parseInt(document.getElementById('powerInput').value)) || 10;
          this.validateTime(t); this.validatePower(p);
          body = { time: t, power: p, heatingChar: this.heatingChar || '.' };
        }
        await MicrowaveAPI.startHeating(body);
        this.isHeating = true; this.isPaused=false; this.heatingString='';
        this.showMessage('Aquecimento iniciado', 'success');
      }

      async pauseHeating(){
        if (!ApiConfig.token) { this.showMessage('Conecte à API.'); return; }
        if (this.isHeating && !this.isPaused) { await MicrowaveAPI.pause(); this.showMessage('Aquecimento pausado','success'); }
        else if (this.isPaused) { await this.cancelHeating(); }
        else { this.clearInputs(); }
      }

      async cancelHeating(){
        if (ApiConfig.token) await MicrowaveAPI.cancel();
        this.isHeating=false; this.isPaused=false; this.heatingString='';
        this.clearInputs(); this.selectedProgram=null; this.heatingChar='.';
        document.getElementById('statusDisplay').textContent='Micro-ondas pronto';
        document.getElementById('programInstructions').style.display='none';
        this.updateProgramsDisplay();
        this.showMessage('Aquecimento cancelado','success');
      }

      async completeHeating(){
        if (ApiConfig.token) await MicrowaveAPI.complete();
        this.isHeating=false; this.isPaused=false;
        this.heatingString += (this.heatingString?' ':'')+'Aquecimento concluído';
        document.getElementById('statusDisplay').textContent = this.heatingString;
        document.getElementById('timeDisplay').textContent = '00:00';
      }

      clearInputs(){
        this.currentTime=0; this.currentPower=10; this.heatingString='';
        document.getElementById('timeInput').value=''; document.getElementById('powerInput').value='';
        this.updateDisplay();
      }
      clearProgramForm(){
        document.getElementById('programForm').reset();
        document.getElementById('charPreview').textContent='?';
      }
    }

    // ---------- GLOBALS + UI WIRING ----------
    const microwave = new MicrowaveController();
    let currentInput='time';

    function addDigit(d){
      const timeInput = document.getElementById('timeInput');
      const powerInput = document.getElementById('powerInput');
      if (currentInput==='time'){ timeInput.value += d; microwave.currentTime = parseInt(timeInput.value)||0; }
      else { powerInput.value += d; microwave.currentPower = parseInt(powerInput.value)||10; }
      microwave.updateDisplay();
    }
    function clearInput(){
      const timeInput = document.getElementById('timeInput');
      const powerInput = document.getElementById('powerInput');
      if (currentInput==='time'){ timeInput.value=''; microwave.currentTime=0; }
      else { powerInput.value=''; microwave.currentPower=10; }
      microwave.updateDisplay();
    }
    function startHeating(){
      const time = parseInt(document.getElementById('timeInput').value) || null;
      const power = parseInt(document.getElementById('powerInput').value) || null;
      microwave.startHeating(time, power);
    }
    function pauseCancel(){ microwave.pauseHeating(); }

    async function deleteProgram(programId){
      if (!confirm('Tem certeza que deseja deletar este programa?')) return;
      try { await microwave.deleteCustomProgram(programId); }
      catch(e){ microwave.showMessage(e.message,'error'); }
    }

    function showTab(tabName){
      document.querySelectorAll('.tab-btn').forEach(b=>b.classList.remove('active'));
      document.querySelector(`[onclick="showTab('${tabName}')"]`).classList.add('active');
      document.querySelectorAll('.tab-pane').forEach(p=>p.classList.remove('active'));
      if (tabName==='programs') document.getElementById('programsTab').classList.add('active');
      if (tabName==='newProgram') document.getElementById('newProgramTab').classList.add('active');
      if (tabName==='logs') { document.getElementById('logsTab').classList.add('active'); loadLogs(); }
    }

    document.getElementById('timeInput').addEventListener('focus', ()=> currentInput='time');
    document.getElementById('powerInput').addEventListener('focus', ()=> currentInput='power');
    document.getElementById('timeInput').addEventListener('input', e=>{ microwave.currentTime=parseInt(e.target.value)||0; microwave.updateDisplay(); });
    document.getElementById('powerInput').addEventListener('input', e=>{ microwave.currentPower=parseInt(e.target.value)||10; microwave.updateDisplay(); });
    document.getElementById('programChar').addEventListener('input', e=>{ document.getElementById('charPreview').textContent = e.target.value || '?'; });

    document.getElementById('programForm').addEventListener('submit', async (e)=>{
      e.preventDefault();
      const name = document.getElementById('programName').value;
      const food = document.getElementById('programFood').value;
      const time = parseInt(document.getElementById('programTime').value);
      const power = parseInt(document.getElementById('programPower').value);
      const heatingChar = document.getElementById('programChar').value;
      const instructions = document.getElementById('programInstructions').value;
      try { await microwave.createCustomProgram(name, food, time, power, heatingChar, instructions); }
      catch(err){ microwave.showMessage(err.message,'error'); }
    });

    document.getElementById('apiConfigForm').addEventListener('submit', async (e)=>{
      e.preventDefault(); await connectAPI();
    });

    document.addEventListener('keydown', (e)=>{
      if (e.target.tagName==='INPUT' || e.target.tagName==='TEXTAREA') return;
      if (e.key>='0' && e.key<='9') addDigit(e.key);
      else if (e.key==='Enter') startHeating();
      else if (e.key==='Escape') pauseCancel();
    });

    window.onclick = function(event){ const modal=document.getElementById('configModal'); if (event.target==modal) closeConfigModal(); }

    window.addEventListener('load', ()=>{
      updateApiStatus();
      microwave.updateDisplay();
      microwave.updateProgramsDisplay();
    });