using System.Text.Json;
using System.Text.Json.Serialization;

namespace WowEvolved.Models;

// ── Rich JSON schema that players upload ─────────────────────────────────────

public class CustomSpecProfile
{
    [JsonPropertyName("className")]
    public string ClassName { get; set; } = "";

    [JsonPropertyName("specName")]
    public string SpecName { get; set; } = "";

    [JsonPropertyName("role")]
    public string Role { get; set; } = "DPS";

    [JsonPropertyName("style")]
    public string Style { get; set; } = "Ranged"; // "Melee" | "Ranged"

    [JsonPropertyName("archetype")]
    public string Archetype { get; set; } = "Universal";

    // ── Detection ────────────────────────────────────────────────────────────
    [JsonPropertyName("signatureKeywords")]
    public List<string> SignatureKeywords { get; set; } = new();

    // ── Core tips ────────────────────────────────────────────────────────────
    [JsonPropertyName("uptimeTip")]
    public string UptimeTip { get; set; } = "";

    [JsonPropertyName("burstTip")]
    public string BurstTip { get; set; } = "";

    [JsonPropertyName("aoeTip")]
    public string AoeTip { get; set; } = "";

    [JsonPropertyName("generalTip")]
    public string GeneralTip { get; set; } = "";

    // ── Priority list ─────────────────────────────────────────────────────────
    /// <summary>Ordered list of abilities by priority (index 0 = highest).</summary>
    [JsonPropertyName("priorityList")]
    public List<PriorityEntry> PriorityList { get; set; } = new();

    // ── Uptime thresholds ─────────────────────────────────────────────────────
    /// <summary>Minimum expected uptime % per ability name.</summary>
    [JsonPropertyName("uptimeThresholds")]
    public Dictionary<string, double> UptimeThresholds { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    // ── Phase notes ───────────────────────────────────────────────────────────
    /// <summary>Generic contextual notes shown as a "know your fight" section.</summary>
    [JsonPropertyName("phaseNotes")]
    public List<PhaseNote> PhaseNotes { get; set; } = new();

    // ── Parse & validate ─────────────────────────────────────────────────────
    public static (CustomSpecProfile? profile, string? error) TryParse(Stream stream)
    {
        try
        {
            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            var profile = JsonSerializer.Deserialize<CustomSpecProfile>(stream, opts);
            if (profile == null) return (null, "JSON vazio ou inválido.");
            if (string.IsNullOrWhiteSpace(profile.ClassName)) return (null, "Campo 'className' é obrigatório.");
            if (string.IsNullOrWhiteSpace(profile.SpecName))  return (null, "Campo 'specName' é obrigatório.");
            return (profile, null);
        }
        catch (JsonException ex)
        {
            return (null, $"JSON inválido: {ex.Message}");
        }
    }

    // ── Convert to internal SpecProfile ──────────────────────────────────────
    public SpecProfile ToSpecProfile()
    {
        var archetype = Archetype switch
        {
            "DOT"       => RotationArchetype.DOT,
            "Proc"      => RotationArchetype.Proc,
            "Burst"     => RotationArchetype.Burst,
            "Pet"       => RotationArchetype.Pet,
            "Channeled" => RotationArchetype.Channeled,
            "Combo"     => RotationArchetype.Combo,
            "Cooldown"  => RotationArchetype.Cooldown,
            _           => RotationArchetype.Universal
        };
        var style = Style?.Equals("Melee", StringComparison.OrdinalIgnoreCase) == true
            ? DamageStyle.Melee : DamageStyle.Ranged;

        return new SpecProfile
        {
            ClassName         = ClassName,
            SpecName          = SpecName,
            Role              = Role,
            Style             = style,
            Archetype         = archetype,
            UptimeTip         = UptimeTip,
            BurstTip          = BurstTip,
            AoeTip            = AoeTip,
            GeneralTip        = GeneralTip,
            SignatureKeywords  = SignatureKeywords,
            // Extended fields stored in the custom profile only
            CustomPriorityList     = PriorityList,
            CustomUptimeThresholds = UptimeThresholds,
            CustomPhaseNotes       = PhaseNotes
        };
    }

    // ── Template generator ────────────────────────────────────────────────────
    public static string GenerateTemplate(SpecProfile? detected)
    {
        var profile = detected ?? WowClassRegistry.Unknown;
        var obj = new
        {
            className   = profile.ClassName == "Unknown" ? "YourClass" : profile.ClassName,
            specName    = profile.SpecName  == "Unknown" ? "YourSpec"  : profile.SpecName,
            role        = profile.Role,
            style       = profile.Style.ToString(),
            archetype   = profile.Archetype.ToString(),
            signatureKeywords = profile.SignatureKeywords.Count > 0
                ? profile.SignatureKeywords
                : new List<string> { "your main spell", "another signature skill" },
            uptimeTip   = profile.UptimeTip.Length > 0 ? profile.UptimeTip : "Maintain key buffs/debuffs at 100% uptime.",
            burstTip    = profile.BurstTip.Length > 0  ? profile.BurstTip  : "Stack offensive cooldowns together.",
            aoeTip      = profile.AoeTip.Length > 0    ? profile.AoeTip    : "Switch to AoE abilities when 2+ targets are active.",
            generalTip  = profile.GeneralTip.Length > 0? profile.GeneralTip: "Follow your class priority list and minimise GCD waste.",
            priorityList = profile.CustomPriorityList.Count > 0
                ? profile.CustomPriorityList.Select(e=>(object)e).ToList()
                .Select(e=>(object)e).ToList() : new List<object>
                {
                    new { skill="Ability 1 (highest priority)", note="Use on cooldown — never delay.", minCritPct=0.0, isCoreRotation=true },
                    new { skill="Ability 2",                    note="Use when proc is active.",       minCritPct=0.0, isCoreRotation=true },
                    new { skill="Ability 3 (filler)",          note="Fill GCDs when above are down.", minCritPct=0.0, isCoreRotation=false }
                },
            uptimeThresholds = profile.CustomUptimeThresholds.Count > 0
                ? profile.CustomUptimeThresholds
                : new Dictionary<string,double>
                {
                    ["Your DoT / Buff Name"] = 95.0,
                    ["Another Debuff"]       = 90.0
                },
            phaseNotes = profile.CustomPhaseNotes.Count > 0
                ? profile.CustomPhaseNotes.Select(e=>(object)e).ToList()
                .Select(e=>(object)e).ToList() : new List<object>
                {
                    new { phase="Pull / Opener",         note="Pre-pot + stack all offensive CDs. Follow opener priority strictly." },
                    new { phase="Add Phase",             note="Switch to AoE immediately. Return to ST when adds die." },
                    new { phase="Burn Phase / Execute",  note="Ignore resource pooling — spend everything for maximum output." }
                }
        };

        return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
    }
}

public class PriorityEntry
{
    [JsonPropertyName("skill")]
    public string Skill { get; set; } = "";

    [JsonPropertyName("note")]
    public string Note { get; set; } = "";

    [JsonPropertyName("minCritPct")]
    public double MinCritPct { get; set; } = 0;

    [JsonPropertyName("isCoreRotation")]
    public bool IsCoreRotation { get; set; } = true;
}

public class PhaseNote
{
    [JsonPropertyName("phase")]
    public string Phase { get; set; } = "";

    [JsonPropertyName("note")]
    public string Note { get; set; } = "";
}
