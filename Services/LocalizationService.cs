namespace WowEvolved.Services;

public enum Lang { PT, EN }

public static class LocalizationService
{
    public static Lang Detect(string? langParam)
        => langParam?.Trim().ToLowerInvariant() == "en" ? Lang.EN : Lang.PT;

    public static string LangCode(Lang l) => l == Lang.EN ? "en" : "pt";

    // ── All static UI strings ─────────────────────────────────────────────────
    public static class UI
    {
        public static string SiteTagline(Lang l)     => l==Lang.EN ? "Combat Analytics System"         : "Combat Analytics System";
        public static string SiteLead(Lang l)        => l==Lang.EN
            ? "Upload two Warcraft Logs exports — class auto-detected, or use a custom JSON profile."
            : "Análise de DPS com detecção automática de classe e suporte a perfis JSON customizados.";
        public static string FeatClass(Lang l)       => l==Lang.EN ? "13 Classes"           : "13 Classes";
        public static string FeatClassDesc(Lang l)   => l==Lang.EN ? "Auto-detection + manual override per spec" : "Detecção automática + ajuste manual por spec";
        public static string FeatJson(Lang l)        => l==Lang.EN ? "Custom JSON"          : "JSON Customizado";
        public static string FeatJsonDesc(Lang l)    => l==Lang.EN ? "Priority list, uptime thresholds and phase notes" : "Priority list, thresholds de uptime e notas de fase";
        public static string FeatDps(Lang l)         => l==Lang.EN ? "DPS Lost"             : "DPS Perdido";
        public static string FeatDpsDesc(Lang l)     => l==Lang.EN ? "Per-ability estimate based on your thresholds" : "Estimativa por habilidade com base nos seus thresholds";
        public static string PanelTitle(Lang l)      => l==Lang.EN ? "⚔ Load Combat Reports"             : "⚔ Carregar Relatórios";
        public static string Player1(Lang l)         => l==Lang.EN ? "Player 1"             : "Jogador 1";
        public static string Player2(Lang l)         => l==Lang.EN ? "Player 2"             : "Jogador 2";
        public static string DropHint(Lang l)        => l==Lang.EN ? "Drag CSV here"        : "Arraste o CSV aqui";
        public static string ChooseCsv(Lang l)       => l==Lang.EN ? "Choose CSV"           : "Escolher CSV";
        public static string JsonLabel(Lang l)       => l==Lang.EN ? "📋 JSON Profile (optional)" : "📋 Perfil JSON (opcional)";
        public static string TemplateBtn(Lang l)     => l==Lang.EN ? "⬇ Template"           : "⬇ Template";
        public static string ChooseJson(Lang l)      => l==Lang.EN ? "Choose JSON"          : "Escolher JSON";
        public static string AwaitingCsv(Lang l)     => l==Lang.EN ? "Awaiting CSV..."      : "Aguardando CSV...";
        public static string Detecting(Lang l)       => l==Lang.EN ? "Detecting class..."   : "Detectando classe...";
        public static string DetectedOk(Lang l)      => l==Lang.EN ? "✔ Detected: "         : "✔ Detectado: ";
        public static string DetectFail(Lang l)      => l==Lang.EN ? "⚠ Class not identified — select manually or use JSON." : "⚠ Classe não identificada — selecione manualmente ou use JSON.";
        public static string ClassLabel(Lang l)      => l==Lang.EN ? "Class (if not using JSON)" : "Classe (se não usar JSON)";
        public static string SpecLabel(Lang l)       => l==Lang.EN ? "Spec"                 : "Spec";
        public static string AutoDetect(Lang l)      => l==Lang.EN ? "Auto-detect"          : "Detectar automaticamente";
        public static string SpecDefault(Lang l)     => l==Lang.EN ? "— Spec —"             : "— Spec —";
        public static string AnalyzeBtn(Lang l)      => l==Lang.EN ? "⚔ Start Analysis ⚔"  : "⚔ Iniciar Análise ⚔";
        public static string SchemaTitle(Lang l)     => l==Lang.EN ? "📖 JSON Schema — click to expand" : "📖 Schema do JSON — clique para ver os campos disponíveis";
        public static string SchemaDesc(Lang l)      => l==Lang.EN
            ? "Upload a <code>.json</code> to replace the internal profile. Download the template ⬇ above to start with your detected spec's values."
            : "Faça upload de um <code>.json</code> para substituir o perfil interno. Baixe o template ⬇ acima para começar com os valores da spec detectada.";
        public static string SchemaRequired(Lang l)  => l==Lang.EN ? "required"             : "obrigatório";
        public static string SchemaArchDesc(Lang l)  => l==Lang.EN ? "<code>DOT</code> | <code>Proc</code> | <code>Burst</code> | <code>Pet</code> | <code>Combo</code> | <code>Cooldown</code> | <code>Universal</code>" : "<code>DOT</code> | <code>Proc</code> | <code>Burst</code> | <code>Pet</code> | <code>Combo</code> | <code>Cooldown</code> | <code>Universal</code>";
        public static string FooterText(Lang l)      => l==Lang.EN ? "Powered by Warcraft Logs Data" : "Powered by Warcraft Logs Data";
        public static string BackBtn(Lang l)         => l==Lang.EN ? "← New Analysis"      : "← Nova Análise";
        public static string BossTag(Lang l)         => l==Lang.EN ? "⚔ Combat Analysis — Imperator Averzian Mythic ⚔" : "⚔ Análise de Combate — Imperator Averzian Mythic ⚔";
        public static string LostLabel(Lang l)       => l==Lang.EN ? "lost to"             : "perdeu para";
        public static string DiffLess(Lang l)        => l==Lang.EN ? "less total damage"   : "menos dano total";
        public static string TotalDmg(Lang l)        => l==Lang.EN ? "Total Damage"        : "Dano Total";
        public static string TotalDps(Lang l)        => l==Lang.EN ? "Total DPS"           : "DPS Total";
        public static string FightDur(Lang l)        => l==Lang.EN ? "Fight Duration"      : "Duração Fight";
        public static string AvgCrit(Lang l)         => l==Lang.EN ? "Avg Crit"            : "Crit Médio";
        public static string AvgUptime(Lang l)       => l==Lang.EN ? "Avg Uptime"          : "Uptime Médio";
        public static string AvgMiss(Lang l)         => l==Lang.EN ? "Avg Miss"            : "Miss Médio";
        public static string Abilities(Lang l)       => l==Lang.EN ? "Abilities"           : "Habilidades";
        public static string EffScore(Lang l)        => l==Lang.EN ? "Efficiency Score"    : "Score Eficiência";
        public static string DpsLostTitle(Lang l)    => l==Lang.EN ? "Estimated DPS Wasted" : "DPS Total Estimado Desperdiçado";
        public static string DpsLostSub(Lang l, string loser, string winner) => l==Lang.EN
            ? $"Sum of all per-ability gaps where {winner} outperformed {loser}"
            : $"Soma de todas as diferenças por habilidade onde {winner} superou {loser}";
        public static string DiagTitle(Lang l)       => l==Lang.EN ? "🔍 Full Diagnosis"   : "🔍 Diagnóstico Completo";
        public static string ImpTitle(Lang l, string loser) => l==Lang.EN ? $"🧠 Improvement Plan for {loser}" : $"🧠 Plano de Melhoria para {loser}";
        public static string PrioLegend(Lang l)      => l==Lang.EN ? "CRITICAL = rotation | HIGH = burst/uptime | NORMAL = optimisation" : "CRITICAL = rotação | HIGH = burst/uptime | NORMAL = otimização";
        public static string SkillCmpTitle(Lang l)   => l==Lang.EN ? "⚔️ Detailed Skill Comparison" : "⚔️ Comparação Detalhada por Habilidade";
        public static string Ability(Lang l)         => l==Lang.EN ? "Ability"             : "Habilidade";
        public static string Diff(Lang l)            => l==Lang.EN ? "Δ DPS"               : "Δ DPS";
        public static string DpsLostCol(Lang l)      => l==Lang.EN ? "DPS Lost"            : "DPS Perdido";
        public static string Distribution(Lang l)    => l==Lang.EN ? "Distribution"        : "Distribuição";
        public static string Top10Title(Lang l)      => l==Lang.EN ? "📊 Top 10 Abilities per Player" : "📊 Top 10 Habilidades por Jogador";
        public static string WinnerTitle(Lang l)     => l==Lang.EN ? "Battle Winner"       : "Vencedor da Batalha";
        public static string PhaseTitle(Lang l, string spec) => l==Lang.EN ? $"🗺️ Phase Notes — {spec} (custom JSON)" : $"🗺️ Notas de Fase — {spec} (JSON customizado)";
        public static string PrioTitle(Lang l, string spec)  => l==Lang.EN ? $"📋 Custom Priority List — {spec}" : $"📋 Priority List Customizada — {spec}";
        public static string Core(Lang l)            => l==Lang.EN ? "Core"                : "Core";
        public static string Filler(Lang l)          => l==Lang.EN ? "Filler"              : "Filler";
        public static string What(Lang l)            => l==Lang.EN ? "WHAT"                : "O QUÊ";
        public static string Why(Lang l)             => l==Lang.EN ? "WHY"                 : "POR QUÊ";
        public static string When(Lang l)            => l==Lang.EN ? "WHEN"                : "QUANDO";
        public static string DpsGainLabel(Lang l)    => l==Lang.EN ? "Estimated DPS gain"  : "Ganho de DPS estimado";
        public static string NotUsed(Lang l)         => l==Lang.EN ? "Not used"            : "Não utilizado";
        public static string ClassNotDet(Lang l)     => l==Lang.EN ? "Class not detected — universal analysis" : "Classe não detectada — análise universal";
        public static string CustomJson(Lang l)      => l==Lang.EN ? "✦ Custom JSON"       : "✦ JSON customizado";
        public static string Seconds(Lang l)         => l==Lang.EN ? "s"                   : "s";
    }

    // ── Insight / suggestion strings ─────────────────────────────────────────
    public static class Insight
    {
        public static string Winner(Lang l, string winner, long gap, double pct) => l==Lang.EN
            ? $"🏆 Winner: {winner} with {gap:N0} more damage ({pct:F1}% advantage)."
            : $"🏆 Vencedor: {winner} com {gap:N0} de dano a mais ({pct:F1}% de vantagem).";
        public static string DpsGap(Lang l, string p1, double d1, string p2, double d2, double diff) => l==Lang.EN
            ? $"⚔️ DPS: {p1} {d1:N0} vs {p2} {d2:N0} — gap of {diff:N0} DPS."
            : $"⚔️ DPS: {p1} {d1:N0} vs {p2} {d2:N0} — diferença de {diff:N0} DPS.";
        public static string ClassDet(Lang l, string p1, string s1, string p2, string s2) => l==Lang.EN
            ? $"🔎 Classes: {p1} → {s1} | {p2} → {s2}."
            : $"🔎 Classes: {p1} → {s1} | {p2} → {s2}.";
        public static string Duration(Lang l, string p1, double d1, string p2, double d2, bool similar) => l==Lang.EN
            ? $"⏱️ Duration: {p1} ~{d1:F0}s vs {p2} ~{d2:F0}s. " + (similar ? "Similar — equivalent conditions." : "Longer fights dilute burst and cooldowns.")
            : $"⏱️ Duração: {p1} ~{d1:F0}s vs {p2} ~{d2:F0}s. " + (similar ? "Duração similar — condições equivalentes." : "Fights mais longos diluem burst e cooldowns.");
        public static string EffScore(Lang l, string p1, double e1, string p2, double e2) => l==Lang.EN
            ? $"📊 Efficiency: {p1} {e1:F1}/100 vs {p2} {e2:F1}/100."
            : $"📊 Eficiência: {p1} {e1:F1}/100 vs {p2} {e2:F1}/100.";
        public static string UptimeAvg(Lang l, string p1, double u1, string p2, double u2, bool diff) => l==Lang.EN
            ? $"🎯 Avg uptime: {p1} {u1:F1}% vs {p2} {u2:F1}%. " + (diff ? "Significant gap — review downtime and positioning." : "Similar.")
            : $"🎯 Uptime médio: {p1} {u1:F1}% vs {p2} {u2:F1}%. " + (diff ? "Diferença significativa — revise downtime e posicionamento." : "Similares.");
        public static string CritAvg(Lang l, string p1, double c1, string p2, double c2) => l==Lang.EN
            ? $"💥 Avg crit: {p1} {c1:F1}% vs {p2} {c2:F1}%."
            : $"💥 Crit médio: {p1} {c1:F1}% vs {p2} {c2:F1}%.";
        public static string ThresholdBreach(Lang l, string player, double actual, string skill, double min, double estLoss) => l==Lang.EN
            ? $"⚠️ THRESHOLD: {player} has {actual:F1}% uptime on {skill} — below defined minimum ({min:F0}%). Estimated DPS loss: ~{estLoss:N0}."
            : $"⚠️ THRESHOLD: {player} tem {actual:F1}% uptime em {skill} — abaixo do mínimo definido ({min:F0}%). Perda DPS estimada: ~{estLoss:N0}.";
        public static string AlertDot(Lang l, string loser, double lu, double wu) => l==Lang.EN
            ? $"🔴 DOT ALERT: Avg uptime {lu:F1}% vs {wu:F1}% — critical for a DoT spec."
            : $"🔴 ALERTA DOT: Uptime médio {lu:F1}% vs {wu:F1}% — crítico para spec de DoT.";
        public static string AlertPet(Lang l, string spec) => l==Lang.EN
            ? $"🐾 PET ALERT: For {spec}, pets/guardians account for 40-50% of damage — keep them on target."
            : $"🐾 ALERTA PET: Para {spec}, pets/guardiões representam 40-50% do dano — mantenha-os no alvo.";
        public static string AlertBurst(Lang l, string loser, double lc, double wc) => l==Lang.EN
            ? $"💥 BURST ALERT: {loser} has {lc:F1}% crit vs {wc:F1}% — align abilities inside proc/buff windows."
            : $"💥 ALERTA BURST: {loser} tem {lc:F1}% crit vs {wc:F1}% — alinhe com janelas de proc.";
        public static string SkillGap(Lang l, string icon, string skill, string loser, double ld, string winner, double wd, double gap, double pct) => l==Lang.EN
            ? $"{icon} {skill}: {loser} {ld:N0} DPS vs {wd:N0} by {winner} (−{gap:N0} DPS / ~{pct:F1}% of total)."
            : $"{icon} {skill}: {loser} {ld:N0} DPS vs {wd:N0} de {winner} (−{gap:N0} DPS / ~{pct:F1}% do total).";
        public static string Missing(Lang l, string loser, string skills) => l==Lang.EN
            ? $"❌ Not used by {loser}: {skills}. Check talents/build."
            : $"❌ Não utilizados por {loser}: {skills}. Verifique build.";
    }

    public static class Sug
    {
        public static string UptimeGeneral(Lang l, double lu, double wu) => l==Lang.EN
            ? $"Increase avg uptime from {lu:F1}% to ~{wu:F1}%"
            : $"Aumente uptime médio de {lu:F1}% para ~{wu:F1}%";
        public static string UptimeWhy(Lang l, string tip, double gain) => l==Lang.EN
            ? tip + $" Every 1% uptime recovered ≈ {gain:N0} DPS."
            : tip + $" Cada 1% de uptime recuperado equivale a ~{gain:N0} DPS.";
        public static string UptimeWhen(Lang l) => l==Lang.EN
            ? "Throughout the fight — use WeakAuras to monitor buffs/debuffs."
            : "Durante todo o fight — use WeakAuras para monitorar buffs e debuffs ativos.";
        public static string BurstWhat(Lang l) => l==Lang.EN
            ? "Align offensive cooldowns with trinkets and Bloodlust/Heroism"
            : "Alinhe cooldowns ofensivos com trinkets e Bloodlust/Heroism";
        public static string BurstWhy(Lang l, string tip, double loss) => l==Lang.EN
            ? tip + $" Estimated loss from misalignment: ~{loss:N0} DPS."
            : tip + $" Perda estimada por desalinhamento: ~{loss:N0} DPS.";
        public static string BurstWhen(Lang l) => l==Lang.EN
            ? "Use a macro to activate all CDs simultaneously on pull and each re-use."
            : "Use macro para ativar todos os CDs simultaneamente no pull.";
        public static string BuildWhat(Lang l, string skill) => l==Lang.EN
            ? $"Add {skill} to your rotation"
            : $"Inclua {skill} na rotação";
        public static string BuildWhy(Lang l, string winner, double dps) => l==Lang.EN
            ? $"{winner} extracts {dps:N0} DPS from this ability — you don't use it. Check talents."
            : $"{winner} extrai {dps:N0} DPS desta habilidade — você não utiliza. Verifique talents.";
        public static string BuildWhen(Lang l) => l==Lang.EN
            ? "Review build and add to action bar before next pull."
            : "Revise build e adicione ao action bar antes do próximo pull.";
        public static string UptimeWhat(Lang l, string skill, double lu, double wu) => l==Lang.EN
            ? $"Improve {skill} uptime: {lu:F1}% → target {wu:F1}%"
            : $"Melhore uptime de {skill}: {lu:F1}% → {wu:F1}%";
        public static string UptimeSkillWhy(Lang l, string tip, string winner, double diff, double gap) => l==Lang.EN
            ? tip + $" {winner} maintains {diff:F1}% more uptime (+{gap:N0} DPS)."
            : tip + $" {winner} mantém {diff:F1}% mais uptime (+{gap:N0} DPS).";
        public static string UptimeSkillWhen(Lang l) => l==Lang.EN
            ? "Reapply before 30% duration remaining — set alerts in WeakAuras."
            : "Reaplique antes de 30% de duração restante — configure WeakAuras.";
        public static string TimingWhat(Lang l, string skill) => l==Lang.EN
            ? $"Use {skill} inside buff/proc windows"
            : $"Use {skill} dentro de janelas de buff/proc";
        public static string TimingWhy(Lang l, string tip, string winner, double diff, double gap) => l==Lang.EN
            ? tip + $" {winner} has {diff:F1}% more crit on this skill (+{gap:N0} DPS). Indicates use during active proc windows."
            : tip + $" {winner} tem {diff:F1}% mais crit nessa skill (+{gap:N0} DPS). Indica uso dentro de janelas de proc ativas.";
        public static string TimingWhen(Lang l) => l==Lang.EN
            ? "During Bloodlust, trinket procs or offensive cooldown windows."
            : "Durante Bloodlust, procs de trinket ou janelas de cooldown ofensivo.";
        public static string GenWhat(Lang l, string skill) => l==Lang.EN
            ? $"Review priority and timing of {skill}"
            : $"Revise prioridade e timing de {skill}";
        public static string GenWhy(Lang l, double gap, string tip) => l==Lang.EN
            ? $"Gap of {gap:N0} DPS. " + tip
            : $"Diferença de {gap:N0} DPS. " + tip;
        public static string GenWhen(Lang l) => l==Lang.EN
            ? "Analyse the Timeline in Warcraft Logs to identify wasted GCDs."
            : "Analise o Timeline no Warcraft Logs para identificar GCDs perdidos.";
        public static string AoeWhat(Lang l) => l==Lang.EN
            ? "React faster to multi-target phases"
            : "Aja mais rápido nas fases de múltiplos alvos";
        public static string AoeWhy(Lang l, string tip, string winner) => l==Lang.EN
            ? tip + $" {winner} extracts more value from add phases."
            : tip + $" {winner} extrai mais valor das fases de adds.";
        public static string AoeWhen(Lang l) => l==Lang.EN
            ? "When the second target appears — switch to AoE immediately."
            : "Ao aparecer o segundo alvo — troque imediatamente para AOE.";
        // Rotation note strings
        public static string NotUsedNote(Lang l, double dps) => l==Lang.EN
            ? $"Not used — {dps:N0} potential DPS wasted. Check build/talents."
            : $"Não utilizado — {dps:N0} DPS potenciais desperdiçados. Verifique build/talents.";
        public static string UptimeLowNote(Lang l, double lu, double wu) => l==Lang.EN
            ? $"Low uptime ({lu:F0}% vs {wu:F0}%) — reapply before buff expires."
            : $"Uptime baixo ({lu:F0}% vs {wu:F0}%) — renove o buff antes de expirar.";
        public static string CritLowNote(Lang l, double lc, double wc) => l==Lang.EN
            ? $"Lower crit ({lc:F0}% vs {wc:F0}%) — use inside proc/buff windows."
            : $"Menor crit ({lc:F0}% vs {wc:F0}%) — use dentro de janelas de proc/buff.";
        public static string BurstNote(Lang l) => l==Lang.EN
            ? "Use inside burst window aligned with offensive cooldowns."
            : "Use dentro da janela de burst alinhado com cooldowns ofensivos.";
        public static string PetNote(Lang l) => l==Lang.EN
            ? "Ensure pets/guardians are on target throughout the window."
            : "Garanta que pets/guardiões estejam no alvo durante toda a janela.";
        public static string ComboNote(Lang l) => l==Lang.EN
            ? "Finish at max resources — sub-cap builders are DPS losses."
            : "Finalize a máximo de recursos — builders sub-cap são perdas.";
        public static string CooldownNote(Lang l) => l==Lang.EN
            ? "Align with major cooldowns (Bloodlust, trinkets, offensive CDs)."
            : "Alinhe com cooldowns maiores (Bloodlust, trinkets, CDs ofensivos).";
        public static string UptimeWeakAura(Lang l, double lu, double wu) => l==Lang.EN
            ? $"Uptime: {lu:F0}% vs {wu:F0}% — consider WeakAuras to monitor."
            : $"Uptime: {lu:F0}% vs {wu:F0}% — considere WeakAuras para monitorar.";
        public static string BigGapNote(Lang l, double lost) => l==Lang.EN
            ? $"Large gap ({lost:N0} DPS) — review timing in the Warcraft Logs Timeline."
            : $"Grande diferença ({lost:N0} DPS) — revise timing no Timeline do Warcraft Logs.";
        public static string ThresholdNote(Lang l, double lu, double threshold) => l==Lang.EN
            ? $"Uptime below target: {lu:F0}% (defined minimum: {threshold:F0}%)."
            : $"Uptime abaixo do esperado: {lu:F0}% (mínimo definido: {threshold:F0}%).";
        public static string PrioNote(Lang l, int prio, string note, string winner, double gap) => l==Lang.EN
            ? $"[P#{prio}] {note} {winner} extracts {gap:N0} more DPS."
            : $"[P#{prio}] {note} {winner} extrai {gap:N0} DPS a mais.";
        public static string PrioCat(Lang l, int prio) => l==Lang.EN
            ? $"Priority #{prio}"
            : $"Prioridade #{prio}";
        public static string CoreRotation(Lang l) => l==Lang.EN ? "Core Rotation" : "Rotação Core";
        public static string FillerCat(Lang l, int prio) => l==Lang.EN
            ? $"Priority #{prio} (Filler)"
            : $"Prioridade #{prio} (Filler)";
        public static string FillerWhy(Lang l, double gap) => l==Lang.EN
            ? $"Filler with {gap:N0} DPS gap — use in remaining GCDs."
            : $"Filler com diferença de {gap:N0} DPS — use nos GCDs restantes.";
        public static string FillerWhen(Lang l) => l==Lang.EN
            ? "When higher-priority abilities are on cooldown."
            : "Quando habilidades de maior prioridade estiverem em cooldown.";
        public static string ThresholdWhat(Lang l, string skill, double lu, double th) => l==Lang.EN
            ? $"Improve {skill} uptime: {lu:F1}% → {th:F0}% (defined minimum)"
            : $"Aumente o uptime de {skill}: {lu:F1}% → {th:F0}% (mínimo definido)";
        public static string ThresholdWhy(Lang l, string tip, double est) => l==Lang.EN
            ? tip + $" Estimated loss: ~{est:N0} DPS."
            : tip + $" Perda estimada: ~{est:N0} DPS.";
        public static string ThresholdWhen(Lang l) => l==Lang.EN
            ? "Reapply before 30% duration remaining — configure WeakAuras."
            : "Reaplique antes de 30% de duração restante — configure WeakAuras.";
        // Custom priority "what" label
        public static string PriorityWhat(Lang l, string note) => l==Lang.EN
            ? $"[Core] {note}"
            : $"[Core] {note}";
        public static string PriorityWhy(Lang l, int prio, string winner, double gap) => l==Lang.EN
            ? $"This is ability #{prio} in the custom priority list. {winner} extracts {gap:N0} more DPS."
            : $"Esta é a habilidade #{prio} na priority list customizada. {winner} extrai {gap:N0} DPS a mais.";
        public static string PriorityWhen(Lang l) => l==Lang.EN
            ? "Whenever available — it's core rotation."
            : "Sempre que disponível — é rotação core.";
    }
}
