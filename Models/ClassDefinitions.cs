namespace WowEvolved.Models;

// ── Spec combat profile — drives generic insight generation ──────────────────
public enum DamageStyle { Melee, Ranged }
public enum RotationArchetype
{
    DOT,        // maintain multiple DoTs (Affliction, Balance, Shadow, Unholy, Assassination)
    Proc,       // react to procs / resource builders (Elemental, Enhancement, Fury, Retribution)
    Burst,      // long ramp → short explosive window (Arcane, Outlaw, Devastation)
    Pet,        // significant pet/guardian damage (Demo, BM, Unholy)
    Channeled,  // sustained channel spells (Destruction, Arms with Cleave)
    Combo,      // combo-point / resource builder → spender (Subtlety, Feral, Windwalker)
    Cooldown,   // aligned CD stacking (Fire, Havoc, Fury, Survival)
    Universal   // unknown / fallback
}

public class SpecProfile
{
    public string ClassName  { get; init; } = "";
    public string SpecName   { get; init; } = "";
    public string Role       { get; init; } = "DPS";
    public DamageStyle Style { get; init; }
    public RotationArchetype Archetype { get; init; }

    // General tips for this archetype shown in suggestions
    public string UptimeTip   { get; init; } = "";
    public string BurstTip    { get; init; } = "";
    public string AoeTip      { get; init; } = "";
    public string GeneralTip  { get; init; } = "";

    // Signature skill keywords used for auto-detection (lowercase substrings)
    public List<string> SignatureKeywords { get; init; } = new();

    // Set when loaded from a custom JSON — null otherwise
    public List<PriorityEntry>            CustomPriorityList      { get; set; } = new();
    public Dictionary<string, double>     CustomUptimeThresholds  { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<PhaseNote>                CustomPhaseNotes        { get; set; } = new();
    public bool                           IsCustom                { get; set; } = false;
}

// ── Class/spec registry ──────────────────────────────────────────────────────
public static class WowClassRegistry
{
    public static readonly List<SpecProfile> AllSpecs = new()
    {
        // ── Death Knight ──────────────────────────────────────────────────────
        new() {
            ClassName="Death Knight", SpecName="Blood", Role="Tank",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.Proc,
            UptimeTip="Mantenha Death and Decay ativo e Heart Strike em cooldown.",
            BurstTip="Alinhe Tombstone + Bone Shield alto para janelas de burst.",
            AoeTip="Death and Decay + Heart Strike cleave — não saia do range.",
            GeneralTip="Gerencie Runic Power evitando overflow; dump com Death Strike prioritariamente.",
            SignatureKeywords=new(){"death and decay","heart strike","bone shield","tombstone","marrowrend","dancing rune weapon","death strike"}
        },
        new() {
            ClassName="Death Knight", SpecName="Frost", Role="DPS",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.Burst,
            UptimeTip="Obliterate é seu core — zero downtime entre runes.",
            BurstTip="Pillar of Frost + Empower Rune Weapon devem ser stacked com trinkets.",
            AoeTip="Howling Blast com Rime proc em múltiplos alvos.",
            GeneralTip="Nunca desperdice Killing Machine proc — consuma antes de qualquer outro GCD.",
            SignatureKeywords=new(){"obliterate","pillar of frost","howling blast","rime","killing machine","frost strike","empower rune weapon"}
        },
        new() {
            ClassName="Death Knight", SpecName="Unholy", Role="DPS",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.DOT,
            UptimeTip="Virulence/Festering Wound stacks devem estar sempre presentes no alvo.",
            BurstTip="Dark Transformation + Unholy Assault definem a janela de burst.",
            AoeTip="Epidemic é o AOE prioritário quando >3 alvos.",
            GeneralTip="Nunca deixe Army of the Dead e Apocalypse desalinhados — são seu maior cooldown.",
            SignatureKeywords=new(){"festering wound","virulence","dark transformation","unholy assault","epidemic","apocalypse","army of the dead","raise dead"}
        },

        // ── Demon Hunter ─────────────────────────────────────────────────────
        new() {
            ClassName="Demon Hunter", SpecName="Havoc", Role="DPS",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.Cooldown,
            UptimeTip="Eyebeam e Blade Dance devem ser usados em cada cooldown sem demora.",
            BurstTip="Meta + The Hunt + Eyebeam stacked — maior janela de dano.",
            AoeTip="Blade Dance cleave e Fel Rush reposition para atingir múltiplos alvos.",
            GeneralTip="Gaste Fury antes de entrar em Meta para não desperdiçar geração.",
            SignatureKeywords=new(){"eyebeam","blade dance","metamorphosis","the hunt","chaos strike","fel rush","immolation aura","throw glaive"}
        },
        new() {
            ClassName="Demon Hunter", SpecName="Vengeance", Role="Tank",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.Proc,
            UptimeTip="Immolation Aura ativa o maior tempo possível.",
            BurstTip="Fiery Brand reduz dano recebido — alinhe com picos de dano do boss.",
            AoeTip="Spirit Bomb com 4+ Soul Fragments é AOE prioritário.",
            GeneralTip="Soul Fragments são a base — gere e consuma eficientemente.",
            SignatureKeywords=new(){"immolation aura","fiery brand","spirit bomb","soul cleave","shear","fracture","infernal strike"}
        },

        // ── Druid ─────────────────────────────────────────────────────────────
        new() {
            ClassName="Druid", SpecName="Balance", Role="DPS",
            Style=DamageStyle.Ranged, Archetype=RotationArchetype.DOT,
            UptimeTip="Moonfire e Sunfire devem ter 100% de uptime no alvo principal.",
            BurstTip="Celestial Alignment / Incarn é sua janela de burst — alinhe trinkets.",
            AoeTip="Starfall durante Celestial Alignment para máximo AOE.",
            GeneralTip="Gere Astral Power eficientemente — nunca cap e nunca gaste abaixo de 40.",
            SignatureKeywords=new(){"moonfire","sunfire","starsurge","starfall","celestial alignment","incarnation","astral power","solar empowerment","lunar empowerment"}
        },
        new() {
            ClassName="Druid", SpecName="Feral", Role="DPS",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.Combo,
            UptimeTip="Rip e Rake devem ter 100% uptime — renove antes de 30% de duração restante.",
            BurstTip="Berserk + Tiger's Fury stacked é a maior janela de dano.",
            AoeTip="Thrash e Swipe com Primal Wrath para distribuir Rip.",
            GeneralTip="Sempre finalize a 5 combo points — builders abaixo de 4 CPs são perdas.",
            SignatureKeywords=new(){"rip","rake","thrash","swipe","berserk","tiger's fury","primal wrath","bloodtalons","shred","ferocious bite"}
        },

        // ── Evoker ────────────────────────────────────────────────────────────
        new() {
            ClassName="Evoker", SpecName="Devastation", Role="DPS",
            Style=DamageStyle.Ranged, Archetype=RotationArchetype.Burst,
            UptimeTip="Disintegrate canal completo sem interrupções é prioritário.",
            BurstTip="Dragonrage define a janela — use todo Essence disponível dentro.",
            AoeTip="Pyre com stacks de Charged Blast durante Dragonrage.",
            GeneralTip="Gerencie Essence entre 3-5 para maximizar uso de Dragonrage.",
            SignatureKeywords=new(){"disintegrate","dragonrage","pyre","fire breath","eternity surge","charged blast","living flame","azure strike"}
        },
        new() {
            ClassName="Evoker", SpecName="Augmentation", Role="DPS",
            Style=DamageStyle.Ranged, Archetype=RotationArchetype.DOT,
            UptimeTip="Ebon Might deve ter 100% uptime — é o buff mais valioso do raid.",
            BurstTip="Breath of Eons alinhado com burst do grupo multiplica dano coletivo.",
            AoeTip="Upheaval e Eruption com Ebon Might ativo.",
            GeneralTip="Seu valor real está nos buffs ao grupo — priorize uptime de Ebon Might acima de tudo.",
            SignatureKeywords=new(){"ebon might","breath of eons","upheaval","eruption","prescience","fate mirror","blistering scales"}
        },

        // ── Hunter ────────────────────────────────────────────────────────────
        new() {
            ClassName="Hunter", SpecName="Beast Mastery", Role="DPS",
            Style=DamageStyle.Ranged, Archetype=RotationArchetype.Pet,
            UptimeTip="Kill Command em cada cooldown — nunca deixe cair.",
            BurstTip="Bestial Wrath + Call of the Wild stacked com trinkets.",
            AoeTip="Multi-Shot para aplicar Beast Cleave — mantenha ativo com 2+ alvos.",
            GeneralTip="Pets são 40-50% do seu dano — mantenha-os no alvo e fora de mechanics.",
            SignatureKeywords=new(){"kill command","bestial wrath","call of the wild","beast cleave","barbed shot","multi-shot","cobra shot","dire beast"}
        },
        new() {
            ClassName="Hunter", SpecName="Marksmanship", Role="DPS",
            Style=DamageStyle.Ranged, Archetype=RotationArchetype.Proc,
            UptimeTip="Trick Shots ativo sempre que houver 2+ alvos.",
            BurstTip="Trueshot é sua maior janela — use Rapid Fire e Aimed Shot maximamente.",
            AoeTip="Volley + Rapid Fire com Trick Shots para AOE.",
            GeneralTip="Nunca desperdice Precise Shots proc — consuma com Arcane Shot antes de Aimed.",
            SignatureKeywords=new(){"aimed shot","rapid fire","trueshot","trick shots","volley","precise shots","steady shot","windrunner"}
        },
        new() {
            ClassName="Hunter", SpecName="Survival", Role="DPS",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.Cooldown,
            UptimeTip="Wildfire Bomb em cada cooldown — maior fonte de dano.",
            BurstTip="Coordinated Assault + Tip of the Spear stacks é o burst.",
            AoeTip="Wildfire Infusion para variações de Bomb em AOE.",
            GeneralTip="Gerencie Focus — nunca cap, nunca fique zerado antes de Kill Command.",
            SignatureKeywords=new(){"wildfire bomb","coordinated assault","raptor strike","mongoose bite","flanking strike","tip of the spear","kill command","steel trap"}
        },

        // ── Mage ─────────────────────────────────────────────────────────────
        new() {
            ClassName="Mage", SpecName="Arcane", Role="DPS",
            Style=DamageStyle.Ranged, Archetype=RotationArchetype.Burst,
            UptimeTip="Gerencie Arcane Charges — sempre finalize spells com 4 charges.",
            BurstTip="Arcane Surge + Evocation são a janela de burst máxima.",
            AoeTip="Arcane Explosion com 4 charges para AOE próximo.",
            GeneralTip="Mana management é crítico — entre em burn phase apenas com Evocation disponível.",
            SignatureKeywords=new(){"arcane blast","arcane barrage","arcane surge","evocation","arcane missile","touch of the magi","arcane charges","presence of mind"}
        },
        new() {
            ClassName="Mage", SpecName="Fire", Role="DPS",
            Style=DamageStyle.Ranged, Archetype=RotationArchetype.Proc,
            UptimeTip="Hot Streak procs nunca devem expirar — consuma imediatamente.",
            BurstTip="Combustion + Pyroclasm stacks é a maior janela de dano do jogo.",
            AoeTip="Flamestrike durante Combustion para AOE máximo.",
            GeneralTip="Pyroblast instant (Hot Streak) seguido de Fireball para gerar próximo proc.",
            SignatureKeywords=new(){"fireball","pyroblast","combustion","hot streak","flamestrike","fire blast","scorch","phoenix flames"}
        },
        new() {
            ClassName="Mage", SpecName="Frost", Role="DPS",
            Style=DamageStyle.Ranged, Archetype=RotationArchetype.Proc,
            UptimeTip="Brain Freeze e Fingers of Frost nunca devem expirar.",
            BurstTip="Icy Veins + Frozen Orb + procs stacked é o burst.",
            AoeTip="Blizzard + Frozen Orb para AOE.",
            GeneralTip="Consuma Brain Freeze com Flurry seguido de Ice Lance (shatter combo).",
            SignatureKeywords=new(){"frostbolt","ice lance","frozen orb","flurry","icy veins","brain freeze","fingers of frost","glacial spike","blizzard"}
        },

        // ── Monk ─────────────────────────────────────────────────────────────
        new() {
            ClassName="Monk", SpecName="Windwalker", Role="DPS",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.Combo,
            UptimeTip="Tiger Palm e Blackout Kick mantêm Mastery stacks — zero downtime.",
            BurstTip="Storm, Earth, and Fire + Serenity stacked é o burst máximo.",
            AoeTip="Spinning Crane Kick com 4+ Mark of the Crane stacks.",
            GeneralTip="Prioridade: RSK > FoF proc > SCK (se ≥2 alvos) > BoK > TP.",
            SignatureKeywords=new(){"rising sun kick","fists of fury","blackout kick","tiger palm","storm earth and fire","serenity","spinning crane kick","whirling dragon punch"}
        },

        // ── Paladin ──────────────────────────────────────────────────────────
        new() {
            ClassName="Paladin", SpecName="Retribution", Role="DPS",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.Cooldown,
            UptimeTip="Blade of Justice e Judgment em cada cooldown para Holy Power.",
            BurstTip="Avenging Wrath + Final Reckoning é a janela — despeje Holy Power.",
            AoeTip="Divine Storm com 4+ Holy Power contra múltiplos alvos.",
            GeneralTip="Nunca cap Holy Power a 5 — consuma com Templar's Verdict.",
            SignatureKeywords=new(){"blade of justice","judgment","avenging wrath","final reckoning","divine storm","templar's verdict","holy power","wake of ashes","execution sentence"}
        },

        // ── Priest ───────────────────────────────────────────────────────────
        new() {
            ClassName="Priest", SpecName="Shadow", Role="DPS",
            Style=DamageStyle.Ranged, Archetype=RotationArchetype.DOT,
            UptimeTip="Shadow Word: Pain e Vampiric Touch com 100% uptime são fundamentais.",
            BurstTip="Void Eruption para entrar em Voidform — alinhe com trinkets.",
            AoeTip="Searing Nightmare para distribuir dots durante Void Torrent.",
            GeneralTip="Mantenha Insanity positivo — nunca saia de Voidform por excesso de downtime.",
            SignatureKeywords=new(){"shadow word pain","vampiric touch","void eruption","mind blast","void bolt","searing nightmare","devouring plague","voidform","mind flay"}
        },

        // ── Rogue ────────────────────────────────────────────────────────────
        new() {
            ClassName="Rogue", SpecName="Assassination", Role="DPS",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.DOT,
            UptimeTip="Rupture e Garrote com 100% uptime — prioridade máxima.",
            BurstTip="Vendetta + Deathmark stacked é a maior janela de burst.",
            AoeTip="Crimson Tempest para aplicar bleeding em múltiplos alvos.",
            GeneralTip="Consuma combo points apenas a 5 CPs — builders a menos são perdas de DPS.",
            SignatureKeywords=new(){"rupture","garrote","vendetta","deathmark","crimson tempest","mutilate","envenom","deadly poison","kingsbane"}
        },
        new() {
            ClassName="Rogue", SpecName="Outlaw", Role="DPS",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.Proc,
            UptimeTip="Roll the Bones deve ser mantido com bons buffs — re-roll se necessário.",
            BurstTip="Adrenaline Rush define o burst — use todos os CPs disponíveis dentro.",
            AoeTip="Blade Flurry para cleave — ative antes de usar finishers.",
            GeneralTip="Pistol Shot proc (Opportunity) substitui Sinister Strike quando disponível.",
            SignatureKeywords=new(){"between the eyes","dispatch","roll the bones","adrenaline rush","blade flurry","sinister strike","pistol shot","keep it rolling"}
        },
        new() {
            ClassName="Rogue", SpecName="Subtlety", Role="DPS",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.Combo,
            UptimeTip="Shadow Dance com Shadowstrike em cada carga — maximize encontros.",
            BurstTip="Shadow Blades + Symbols of Death definem o burst window.",
            AoeTip="Shuriken Storm dentro de Shadow Dance para AOE.",
            GeneralTip="Backstab fora de stealth é um finisher de emergência — priorize Shadowstrike.",
            SignatureKeywords=new(){"shadowstrike","eviscerate","shadow dance","symbols of death","shadow blades","shuriken storm","backstab","secret technique"}
        },

        // ── Shaman ───────────────────────────────────────────────────────────
        new() {
            ClassName="Shaman", SpecName="Elemental", Role="DPS",
            Style=DamageStyle.Ranged, Archetype=RotationArchetype.Proc,
            UptimeTip="Flame Shock com 100% uptime é pré-requisito para Lava Surge procs.",
            BurstTip="Stormkeeper + Primordial Wave durante Ascendance é o burst máximo.",
            AoeTip="Earthquake + Chain Lightning para AOE — priorize Primordial Wave.",
            GeneralTip="Gaste Maelstrom antes de atingir 100 — nunca cap.",
            SignatureKeywords=new(){"flame shock","lava burst","stormkeeper","ascendance","earthquake","chain lightning","primordial wave","lightning bolt","icefury"}
        },
        new() {
            ClassName="Shaman", SpecName="Enhancement", Role="DPS",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.Proc,
            UptimeTip="Flame Shock e Frost Shock mantêm o ciclo de procs — zero downtime.",
            BurstTip="Feral Spirit + Ascendance stacked é o burst — use todas as Maelstrom Weapons.",
            AoeTip="Crash Lightning habilita o cleave de todas as abilties físicas.",
            GeneralTip="Consuma Maelstrom Weapons a 10 stacks — nunca desperdice Lightning Bolt instante.",
            SignatureKeywords=new(){"stormstrike","lava lash","crash lightning","feral spirit","ascendance","windfury","maelstrom weapon","sundering","ice strike"}
        },

        // ── Warlock ──────────────────────────────────────────────────────────
        new() {
            ClassName="Warlock", SpecName="Affliction", Role="DPS",
            Style=DamageStyle.Ranged, Archetype=RotationArchetype.DOT,
            UptimeTip="Agony, Corruption e Unstable Affliction devem ter 100% uptime simultâneo.",
            BurstTip="Dark Soul: Misery alinhado com Rapture de Soul Shards é o burst.",
            AoeTip="Seed of Corruption para distribuir Corruption em múltiplos alvos instantaneamente.",
            GeneralTip="Gerencie Soul Shards — nunca cap; nunca gaste sem DoTs renovados.",
            SignatureKeywords=new(){"agony","corruption","unstable affliction","malefic rapture","seed of corruption","dark soul","haunt","drain soul","vile taint"}
        },
        new() {
            ClassName="Warlock", SpecName="Demonology", Role="DPS",
            Style=DamageStyle.Ranged, Archetype=RotationArchetype.Pet,
            UptimeTip="Maximize implings ativos — cada Imp spawna com dano independente.",
            BurstTip="Demonic Tyrant durante pico de imps spawned é a maior janela de burst.",
            AoeTip="Imps com Soul Strike e Nether Portal para múltiplos alvos.",
            GeneralTip="Gerencie Soul Shards — nunca cap; use Hand of Gul'dan a 3-4 shards.",
            SignatureKeywords=new(){"hand of guldan","demonic tyrant","call dreadstalkers","demonbolt","wild imp","inner demons","diabolic ritual","nether portal","soul strike","felguard"}
        },
        new() {
            ClassName="Warlock", SpecName="Destruction", Role="DPS",
            Style=DamageStyle.Ranged, Archetype=RotationArchetype.Burst,
            UptimeTip="Immolate com 100% uptime é pré-requisito para tudo.",
            BurstTip="Summon Infernal + Dark Soul: Instability + Rain of Fire stacked.",
            AoeTip="Rain of Fire com Cataclysm para múltiplos alvos com Immolate.",
            GeneralTip="Nunca gaste Soul Shards em Chaos Bolt fora de Dark Soul — guarde para o burst.",
            SignatureKeywords=new(){"chaos bolt","rain of fire","immolate","havoc","infernal","dark soul","cataclysm","incinerate","shadowburn","conflagrate"}
        },

        // ── Warrior ──────────────────────────────────────────────────────────
        new() {
            ClassName="Warrior", SpecName="Arms", Role="DPS",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.Cooldown,
            UptimeTip="Mortal Strike em cada cooldown — é o gerador de Overpower.",
            BurstTip="Colossus Smash + Avatar stacked define o burst — use Execute se disponível.",
            AoeTip="Sweeping Strikes antes de qualquer finisher para duplicar dano.",
            GeneralTip="Gerencie Rage — nunca cap a 100; priorize Mortal Strike para Overpower.",
            SignatureKeywords=new(){"mortal strike","colossus smash","avatar","sweeping strikes","overpower","execute","slam","warbreaker","bladestorm"}
        },
        new() {
            ClassName="Warrior", SpecName="Fury", Role="DPS",
            Style=DamageStyle.Melee, Archetype=RotationArchetype.Proc,
            UptimeTip="Rampage em cada 100 Rage — nunca cap.",
            BurstTip="Recklessness + Avatar + Spear of Bastion stacked é o burst máximo.",
            AoeTip="Whirlwind habilita o cleave — use antes de qualquer Rampage.",
            GeneralTip="Enrage deve estar ativo >80% do fight — Rampage e Sudden Death garantem uptime.",
            SignatureKeywords=new(){"rampage","recklessness","bloodthirst","execute","whirlwind","onslaught","crushing blow","avatar","spear of bastion","siegebreaker"}
        },
    };

    // ── Detection from skill names ─────────────────────────────────────────
    public static SpecProfile? DetectFromSkills(IEnumerable<string> skillNames)
    {
        var lowerSkills = skillNames
            .Select(s => s.ToLowerInvariant())
            .ToHashSet();

        return AllSpecs
            .Select(spec => new
            {
                spec,
                score = spec.SignatureKeywords.Count(kw =>
                    lowerSkills.Any(s => s.Contains(kw)))
            })
            .Where(x => x.score > 0)
            .OrderByDescending(x => x.score)
            .FirstOrDefault()?.spec;
    }

    // ── All class names for dropdown ───────────────────────────────────────
    public static List<string> AllClassNames =>
        AllSpecs.Select(s => s.ClassName).Distinct().OrderBy(x => x).ToList();

    public static List<SpecProfile> GetSpecsForClass(string className) =>
        AllSpecs.Where(s => s.ClassName.Equals(className, StringComparison.OrdinalIgnoreCase)).ToList();

    public static SpecProfile? GetSpec(string className, string specName) =>
        AllSpecs.FirstOrDefault(s =>
            s.ClassName.Equals(className, StringComparison.OrdinalIgnoreCase) &&
            s.SpecName.Equals(specName, StringComparison.OrdinalIgnoreCase));

    // Fallback universal profile
    public static SpecProfile Unknown => new()
    {
        ClassName="Unknown", SpecName="Unknown", Role="DPS",
        Style=DamageStyle.Ranged, Archetype=RotationArchetype.Universal,
        UptimeTip="Mantenha suas principais abilities em uso constante — zero downtime entre casts.",
        BurstTip="Alinhe cooldowns ofensivos com trinkets e Bloodlust/Heroism.",
        AoeTip="Troque para abilities de AOE assim que houver 2+ alvos ativos.",
        GeneralTip="Priorize abilities de maior DPS no topo da sua priority list.",
        SignatureKeywords=new()
    };
}

// ── Extension fields populated from CustomSpecProfile ────────────────────────
// Added as partial extension to SpecProfile (appended to same file)

// ── English spec registry ────────────────────────────────────────────────────
public static class WowClassRegistryEN
{
    public static readonly List<SpecProfile> AllSpecs = new()
    {
        new(){ClassName="Death Knight",SpecName="Blood",Role="Tank",Style=DamageStyle.Melee,Archetype=RotationArchetype.Proc,
            UptimeTip="Keep Death and Decay active and Heart Strike on cooldown.",
            BurstTip="Align Tombstone + high Bone Shield stacks for burst windows.",
            AoeTip="Death and Decay + Heart Strike cleave — don't leave the range.",
            GeneralTip="Manage Runic Power avoiding overflow; dump with Death Strike first.",
            SignatureKeywords=new(){"death and decay","heart strike","bone shield","tombstone","marrowrend","dancing rune weapon","death strike"}},
        new(){ClassName="Death Knight",SpecName="Frost",Role="DPS",Style=DamageStyle.Melee,Archetype=RotationArchetype.Burst,
            UptimeTip="Obliterate is your core — zero downtime between runes.",
            BurstTip="Pillar of Frost + Empower Rune Weapon must be stacked with trinkets.",
            AoeTip="Howling Blast with Rime proc on multiple targets.",
            GeneralTip="Never waste a Killing Machine proc — consume before any other GCD.",
            SignatureKeywords=new(){"obliterate","pillar of frost","howling blast","rime","killing machine","frost strike","empower rune weapon"}},
        new(){ClassName="Death Knight",SpecName="Unholy",Role="DPS",Style=DamageStyle.Melee,Archetype=RotationArchetype.DOT,
            UptimeTip="Festering Wound stacks must always be present on the target.",
            BurstTip="Dark Transformation + Unholy Assault define the burst window.",
            AoeTip="Epidemic is the priority AoE when there are 3+ targets.",
            GeneralTip="Never let Army of the Dead and Apocalypse fall out of alignment.",
            SignatureKeywords=new(){"festering wound","virulence","dark transformation","unholy assault","epidemic","apocalypse","army of the dead","raise dead"}},
        new(){ClassName="Demon Hunter",SpecName="Havoc",Role="DPS",Style=DamageStyle.Melee,Archetype=RotationArchetype.Cooldown,
            UptimeTip="Eye Beam and Blade Dance must be used on every cooldown without delay.",
            BurstTip="Meta + The Hunt + Eye Beam stacked — greatest damage window.",
            AoeTip="Blade Dance cleave and Fel Rush repositioning to hit multiple targets.",
            GeneralTip="Spend Fury before entering Meta to avoid wasting generation.",
            SignatureKeywords=new(){"eyebeam","blade dance","metamorphosis","the hunt","chaos strike","fel rush","immolation aura","throw glaive"}},
        new(){ClassName="Demon Hunter",SpecName="Vengeance",Role="Tank",Style=DamageStyle.Melee,Archetype=RotationArchetype.Proc,
            UptimeTip="Keep Immolation Aura active as long as possible.",
            BurstTip="Fiery Brand reduces damage taken — align with boss damage spikes.",
            AoeTip="Spirit Bomb with 4+ Soul Fragments is priority AoE.",
            GeneralTip="Soul Fragments are the foundation — generate and consume efficiently.",
            SignatureKeywords=new(){"immolation aura","fiery brand","spirit bomb","soul cleave","shear","fracture","infernal strike"}},
        new(){ClassName="Druid",SpecName="Balance",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.DOT,
            UptimeTip="Moonfire and Sunfire must have 100% uptime on the main target.",
            BurstTip="Celestial Alignment / Incarnation is your burst window — align trinkets.",
            AoeTip="Starfall during Celestial Alignment for maximum AoE.",
            GeneralTip="Generate Astral Power efficiently — never cap and never spend below 40.",
            SignatureKeywords=new(){"moonfire","sunfire","starsurge","starfall","celestial alignment","incarnation","astral power","solar empowerment","lunar empowerment"}},
        new(){ClassName="Druid",SpecName="Feral",Role="DPS",Style=DamageStyle.Melee,Archetype=RotationArchetype.Combo,
            UptimeTip="Rip and Rake must have 100% uptime — renew before 30% duration remaining.",
            BurstTip="Berserk + Tiger's Fury stacked is the highest damage window.",
            AoeTip="Thrash and Swipe with Primal Wrath to spread Rip.",
            GeneralTip="Always finish at 5 combo points — builders below 4 CPs are losses.",
            SignatureKeywords=new(){"rip","rake","thrash","swipe","berserk","tiger's fury","primal wrath","bloodtalons","shred","ferocious bite"}},
        new(){ClassName="Evoker",SpecName="Devastation",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.Burst,
            UptimeTip="Complete Disintegrate channels without interruption — top priority.",
            BurstTip="Dragonrage defines the window — spend all available Essence inside it.",
            AoeTip="Pyre with Charged Blast stacks during Dragonrage.",
            GeneralTip="Manage Essence between 3-5 to maximise Dragonrage usage.",
            SignatureKeywords=new(){"disintegrate","dragonrage","pyre","fire breath","eternity surge","charged blast","living flame","azure strike"}},
        new(){ClassName="Evoker",SpecName="Augmentation",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.DOT,
            UptimeTip="Ebon Might must have 100% uptime — it is the raid's most valuable buff.",
            BurstTip="Breath of Eons aligned with the group's burst multiplies collective damage.",
            AoeTip="Upheaval and Eruption with Ebon Might active.",
            GeneralTip="Your real value is in group buffs — prioritise Ebon Might uptime above all.",
            SignatureKeywords=new(){"ebon might","breath of eons","upheaval","eruption","prescience","fate mirror","blistering scales"}},
        new(){ClassName="Hunter",SpecName="Beast Mastery",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.Pet,
            UptimeTip="Kill Command on every cooldown — never let it sit.",
            BurstTip="Bestial Wrath + Call of the Wild stacked with trinkets.",
            AoeTip="Multi-Shot to apply Beast Cleave — keep active with 2+ targets.",
            GeneralTip="Pets are 40-50% of your damage — keep them on target and out of mechanics.",
            SignatureKeywords=new(){"kill command","bestial wrath","call of the wild","beast cleave","barbed shot","multi-shot","cobra shot","dire beast"}},
        new(){ClassName="Hunter",SpecName="Marksmanship",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.Proc,
            UptimeTip="Trick Shots active whenever there are 2+ targets.",
            BurstTip="Trueshot is your biggest window — maximise Rapid Fire and Aimed Shot inside.",
            AoeTip="Volley + Rapid Fire with Trick Shots for AoE.",
            GeneralTip="Never waste a Precise Shots proc — consume with Arcane Shot before Aimed.",
            SignatureKeywords=new(){"aimed shot","rapid fire","trueshot","trick shots","volley","precise shots","steady shot","windrunner"}},
        new(){ClassName="Hunter",SpecName="Survival",Role="DPS",Style=DamageStyle.Melee,Archetype=RotationArchetype.Cooldown,
            UptimeTip="Wildfire Bomb on every cooldown — largest damage source.",
            BurstTip="Coordinated Assault + Tip of the Spear stacks is the burst window.",
            AoeTip="Wildfire Infusion for Bomb variations in AoE.",
            GeneralTip="Manage Focus — never cap, never be empty before Kill Command.",
            SignatureKeywords=new(){"wildfire bomb","coordinated assault","raptor strike","mongoose bite","flanking strike","tip of the spear","kill command","steel trap"}},
        new(){ClassName="Mage",SpecName="Arcane",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.Burst,
            UptimeTip="Manage Arcane Charges — always finish spells at 4 charges.",
            BurstTip="Arcane Surge + Evocation is the maximum burst window.",
            AoeTip="Arcane Explosion at 4 charges for melee-range AoE.",
            GeneralTip="Mana management is critical — only enter burn phase with Evocation available.",
            SignatureKeywords=new(){"arcane blast","arcane barrage","arcane surge","evocation","arcane missile","touch of the magi","arcane charges","presence of mind"}},
        new(){ClassName="Mage",SpecName="Fire",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.Proc,
            UptimeTip="Hot Streak procs must never expire — consume immediately.",
            BurstTip="Combustion + Pyroclasm stacks is the highest damage window in the game.",
            AoeTip="Flamestrike during Combustion for maximum AoE.",
            GeneralTip="Instant Pyroblast (Hot Streak) followed by Fireball to generate the next proc.",
            SignatureKeywords=new(){"fireball","pyroblast","combustion","hot streak","flamestrike","fire blast","scorch","phoenix flames"}},
        new(){ClassName="Mage",SpecName="Frost",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.Proc,
            UptimeTip="Brain Freeze and Fingers of Frost must never expire.",
            BurstTip="Icy Veins + Frozen Orb + stacked procs is the burst window.",
            AoeTip="Blizzard + Frozen Orb for AoE.",
            GeneralTip="Consume Brain Freeze with Flurry followed by Ice Lance (shatter combo).",
            SignatureKeywords=new(){"frostbolt","ice lance","frozen orb","flurry","icy veins","brain freeze","fingers of frost","glacial spike","blizzard"}},
        new(){ClassName="Monk",SpecName="Windwalker",Role="DPS",Style=DamageStyle.Melee,Archetype=RotationArchetype.Combo,
            UptimeTip="Tiger Palm and Blackout Kick maintain Mastery stacks — zero downtime.",
            BurstTip="Storm, Earth, and Fire + Serenity stacked is peak burst.",
            AoeTip="Spinning Crane Kick with 4+ Mark of the Crane stacks.",
            GeneralTip="Priority: RSK > FoF proc > SCK (if ≥2 targets) > BoK > TP.",
            SignatureKeywords=new(){"rising sun kick","fists of fury","blackout kick","tiger palm","storm earth and fire","serenity","spinning crane kick","whirling dragon punch"}},
        new(){ClassName="Paladin",SpecName="Retribution",Role="DPS",Style=DamageStyle.Melee,Archetype=RotationArchetype.Cooldown,
            UptimeTip="Blade of Justice and Judgment on every cooldown for Holy Power.",
            BurstTip="Avenging Wrath + Final Reckoning is the window — dump Holy Power.",
            AoeTip="Divine Storm at 4+ Holy Power against multiple targets.",
            GeneralTip="Never cap Holy Power at 5 — consume with Templar's Verdict.",
            SignatureKeywords=new(){"blade of justice","judgment","avenging wrath","final reckoning","divine storm","templar's verdict","holy power","wake of ashes","execution sentence"}},
        new(){ClassName="Priest",SpecName="Shadow",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.DOT,
            UptimeTip="Shadow Word: Pain and Vampiric Touch at 100% uptime are fundamental.",
            BurstTip="Void Eruption to enter Voidform — align with trinkets.",
            AoeTip="Searing Nightmare to spread dots during Void Torrent.",
            GeneralTip="Keep Insanity positive — never exit Voidform due to excessive downtime.",
            SignatureKeywords=new(){"shadow word pain","vampiric touch","void eruption","mind blast","void bolt","searing nightmare","devouring plague","voidform","mind flay"}},
        new(){ClassName="Rogue",SpecName="Assassination",Role="DPS",Style=DamageStyle.Melee,Archetype=RotationArchetype.DOT,
            UptimeTip="Rupture and Garrote at 100% uptime — maximum priority.",
            BurstTip="Vendetta + Deathmark stacked is the largest burst window.",
            AoeTip="Crimson Tempest to apply bleeds on multiple targets.",
            GeneralTip="Only spend combo points at 5 CPs — builders below that are DPS losses.",
            SignatureKeywords=new(){"rupture","garrote","vendetta","deathmark","crimson tempest","mutilate","envenom","deadly poison","kingsbane"}},
        new(){ClassName="Rogue",SpecName="Outlaw",Role="DPS",Style=DamageStyle.Melee,Archetype=RotationArchetype.Proc,
            UptimeTip="Roll the Bones must be maintained with good buffs — re-roll if needed.",
            BurstTip="Adrenaline Rush defines the burst — spend all available CPs inside.",
            AoeTip="Blade Flurry for cleave — activate before using finishers.",
            GeneralTip="Pistol Shot proc (Opportunity) replaces Sinister Strike when available.",
            SignatureKeywords=new(){"between the eyes","dispatch","roll the bones","adrenaline rush","blade flurry","sinister strike","pistol shot","keep it rolling"}},
        new(){ClassName="Rogue",SpecName="Subtlety",Role="DPS",Style=DamageStyle.Melee,Archetype=RotationArchetype.Combo,
            UptimeTip="Shadow Dance with Shadowstrike every charge — maximise encounters.",
            BurstTip="Shadow Blades + Symbols of Death define the burst window.",
            AoeTip="Shuriken Storm inside Shadow Dance for AoE.",
            GeneralTip="Backstab outside stealth is an emergency filler — prioritise Shadowstrike.",
            SignatureKeywords=new(){"shadowstrike","eviscerate","shadow dance","symbols of death","shadow blades","shuriken storm","backstab","secret technique"}},
        new(){ClassName="Shaman",SpecName="Elemental",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.Proc,
            UptimeTip="Flame Shock at 100% uptime is the prerequisite for Lava Surge procs.",
            BurstTip="Stormkeeper + Primordial Wave during Ascendance is peak burst.",
            AoeTip="Earthquake + Chain Lightning for AoE — prioritise Primordial Wave.",
            GeneralTip="Spend Maelstrom before reaching 100 — never cap.",
            SignatureKeywords=new(){"flame shock","lava burst","stormkeeper","ascendance","earthquake","chain lightning","primordial wave","lightning bolt","icefury"}},
        new(){ClassName="Shaman",SpecName="Enhancement",Role="DPS",Style=DamageStyle.Melee,Archetype=RotationArchetype.Proc,
            UptimeTip="Flame Shock and Frost Shock maintain the proc cycle — zero downtime.",
            BurstTip="Feral Spirit + Ascendance stacked is the burst — spend all Maelstrom Weapons.",
            AoeTip="Crash Lightning enables cleave on all physical abilities.",
            GeneralTip="Consume Maelstrom Weapons at 10 stacks — never waste instant Lightning Bolt.",
            SignatureKeywords=new(){"stormstrike","lava lash","crash lightning","feral spirit","ascendance","windfury","maelstrom weapon","sundering","ice strike"}},
        new(){ClassName="Warlock",SpecName="Affliction",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.DOT,
            UptimeTip="Agony, Corruption and Unstable Affliction must have 100% uptime simultaneously.",
            BurstTip="Dark Soul: Misery aligned with Rapture of Soul Shards is the burst.",
            AoeTip="Seed of Corruption to instantly spread Corruption to multiple targets.",
            GeneralTip="Manage Soul Shards — never cap; never spend without DoTs refreshed.",
            SignatureKeywords=new(){"agony","corruption","unstable affliction","malefic rapture","seed of corruption","dark soul","haunt","drain soul","vile taint"}},
        new(){ClassName="Warlock",SpecName="Demonology",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.Pet,
            UptimeTip="Maximise active imps — each Imp spawns with independent damage.",
            BurstTip="Demonic Tyrant during peak imp count is the largest burst window.",
            AoeTip="Imps with Soul Strike and Nether Portal for multiple targets.",
            GeneralTip="Manage Soul Shards — never cap; use Hand of Gul'dan at 3-4 shards.",
            SignatureKeywords=new(){"hand of guldan","demonic tyrant","call dreadstalkers","demonbolt","wild imp","inner demons","diabolic ritual","nether portal","soul strike","felguard"}},
        new(){ClassName="Warlock",SpecName="Destruction",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.Burst,
            UptimeTip="Immolate at 100% uptime is the prerequisite for everything.",
            BurstTip="Summon Infernal + Dark Soul: Instability + Rain of Fire stacked.",
            AoeTip="Rain of Fire with Cataclysm on multiple targets with Immolate.",
            GeneralTip="Never spend Soul Shards on Chaos Bolt outside Dark Soul — save for burst.",
            SignatureKeywords=new(){"chaos bolt","rain of fire","immolate","havoc","infernal","dark soul","cataclysm","incinerate","shadowburn","conflagrate"}},
        new(){ClassName="Warrior",SpecName="Arms",Role="DPS",Style=DamageStyle.Melee,Archetype=RotationArchetype.Cooldown,
            UptimeTip="Mortal Strike on every cooldown — it is the Overpower generator.",
            BurstTip="Colossus Smash + Avatar stacked defines the burst — use Execute if available.",
            AoeTip="Sweeping Strikes before any finisher to duplicate damage.",
            GeneralTip="Manage Rage — never cap at 100; prioritise Mortal Strike for Overpower.",
            SignatureKeywords=new(){"mortal strike","colossus smash","avatar","sweeping strikes","overpower","execute","slam","warbreaker","bladestorm"}},
        new(){ClassName="Warrior",SpecName="Fury",Role="DPS",Style=DamageStyle.Melee,Archetype=RotationArchetype.Proc,
            UptimeTip="Rampage every 100 Rage — never cap.",
            BurstTip="Recklessness + Avatar + Spear of Bastion stacked is peak burst.",
            AoeTip="Whirlwind enables cleave — use before any Rampage.",
            GeneralTip="Enrage must be active >80% of the fight — Rampage and Sudden Death maintain it.",
            SignatureKeywords=new(){"rampage","recklessness","bloodthirst","execute","whirlwind","onslaught","crushing blow","avatar","spear of bastion","siegebreaker"}},
    };

    public static SpecProfile? DetectFromSkills(IEnumerable<string> skillNames)
    {
        var lower = skillNames.Select(s=>s.ToLowerInvariant()).ToHashSet();
        return AllSpecs
            .Select(s=>new{s,score=s.SignatureKeywords.Count(k=>lower.Any(x=>x.Contains(k)))})
            .Where(x=>x.score>0).OrderByDescending(x=>x.score).FirstOrDefault()?.s;
    }

    public static List<SpecProfile> GetSpecsForClass(string cls)
        => AllSpecs.Where(s=>s.ClassName.Equals(cls,StringComparison.OrdinalIgnoreCase)).ToList();

    public static SpecProfile? GetSpec(string cls, string spec)
        => AllSpecs.FirstOrDefault(s=>
            s.ClassName.Equals(cls,StringComparison.OrdinalIgnoreCase)&&
            s.SpecName.Equals(spec,StringComparison.OrdinalIgnoreCase));

    public static SpecProfile Unknown => new(){
        ClassName="Unknown",SpecName="Unknown",Role="DPS",Style=DamageStyle.Ranged,Archetype=RotationArchetype.Universal,
        UptimeTip="Keep your main abilities in constant use — zero downtime between casts.",
        BurstTip="Align offensive cooldowns with trinkets and Bloodlust/Heroism.",
        AoeTip="Switch to AoE abilities as soon as 2+ targets are active.",
        GeneralTip="Prioritise the highest-DPS abilities at the top of your priority list.",
        SignatureKeywords=new()};
}
