using System.Globalization;
using System.Text.RegularExpressions;

namespace WowEvolved.Models;

public sealed class DamageRecord
{
    public string Name      { get; set; } = string.Empty;
    public string Amount    { get; set; } = string.Empty;
    public string Casts     { get; set; } = string.Empty;
    public string AvgCast   { get; set; } = string.Empty;
    public string Hits      { get; set; } = string.Empty;
    public string AvgHit    { get; set; } = string.Empty;
    public string CritPct   { get; set; } = string.Empty;
    public string UptimePct { get; set; } = string.Empty;
    public string MissPct   { get; set; } = string.Empty;
    public string DPS       { get; set; } = string.Empty;

    public long   BossTotal  { get; private set; }
    public double Pct        { get; set; }
    public long   DmgValue   { get; private set; }
    public double AmountNum  { get; private set; }
    public double CastsNum   { get; private set; }
    public double HitsNum    { get; private set; }
    public double CritNum    { get; private set; }
    public double UptimeNum  { get; private set; }
    public double MissNum    { get; private set; }
    public double DpsNum     { get; private set; }
    public double AvgHitNum  { get; private set; }
    public double AvgCastNum { get; private set; }

    public void Parse()
    {
        ParseAmount();
        AmountNum  = DmgValue;
        CastsNum   = ParseNumber(Casts);
        HitsNum    = ParseNumber(Hits);
        CritNum    = ParseNumber(CritPct);
        UptimeNum  = ParseNumber(UptimePct);
        MissNum    = ParseNumber(MissPct);
        DpsNum     = ParseNumber(DPS);
        AvgHitNum  = ParseNumber(AvgHit);
        AvgCastNum = ParseNumber(AvgCast);
    }

    private void ParseAmount()
    {
        try
        {
            var s = Amount.Trim();
            var di = s.IndexOf('$');
            if (di > 0) BossTotal = (long)ParseNumber(s[..di]);

            var pm = Regex.Match(s, @"\$([\d.]+)%");
            if (pm.Success) Pct = ParseNumber(pm.Groups[1].Value);

            var vm = Regex.Match(s, @"%(.+)$");
            if (vm.Success)
            {
                var v = vm.Groups[1].Value.Trim().ToLowerInvariant();
                double mult = 1;
                if (v.EndsWith("m")) { mult = 1_000_000; v = v[..^1]; }
                else if (v.EndsWith("k")) { mult = 1_000; v = v[..^1]; }
                DmgValue = (long)(ParseNumber(v) * mult);
            }
        }
        catch { }
    }

    public static double ParseNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        value = value.Trim().Replace("%", "").Replace(" ", "");
        if (value.Contains(',') && value.Contains('.')) value = value.Replace(",", "");
        else if (value.Contains(',') && !value.Contains('.')) value = value.Replace(",", ".");
        return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var r) ? r : 0;
    }
}

public class PlayerAnalysis
{
    public string             PlayerName      { get; set; } = "";
    public string             FileName        { get; set; } = "";
    public List<DamageRecord> Records         { get; set; } = new();
    public long               TotalDamage     { get; set; }
    public double             TotalDps        { get; set; }
    public double             FightDuration   { get; set; }
    public double             AvgCritPct      { get; set; }
    public double             AvgUptimePct    { get; set; }
    public double             AvgMissPct      { get; set; }
    public double             EfficiencyScore { get; set; }

    // JSON parse error (if any)
    public string? JsonParseError { get; set; }

    // Class detection
    public SpecProfile?       DetectedSpec    { get; set; }
    public SpecProfile?       OverrideSpec    { get; set; }
    public SpecProfile        ActiveSpec      => OverrideSpec ?? DetectedSpec ?? WowClassRegistry.Unknown;

    public void Compute()
    {
        if (Records.Count == 0) return;
        TotalDamage   = Records.First().BossTotal;
        TotalDps      = Records.Sum(r => r.DpsNum);
        FightDuration = TotalDps > 0 ? TotalDamage / TotalDps : 0;
        AvgCritPct    = Records.Where(r => r.CritNum > 0).Select(r => r.CritNum).DefaultIfEmpty(0).Average();
        AvgUptimePct  = Records.Where(r => r.UptimeNum > 0).Select(r => r.UptimeNum).DefaultIfEmpty(0).Average();
        AvgMissPct    = Records.Where(r => r.MissNum > 0).Select(r => r.MissNum).DefaultIfEmpty(0).Average();

        double uptimeScore = Math.Clamp(AvgUptimePct, 0, 100);
        double critScore   = Math.Clamp(AvgCritPct * 2, 0, 100);
        double dpsScore    = Math.Clamp(TotalDps / 5000d * 30, 0, 100);
        EfficiencyScore    = Math.Round(uptimeScore * 0.45 + critScore * 0.25 + dpsScore * 0.30, 1);

        // Auto-detect class
        DetectedSpec = WowClassRegistry.DetectFromSkills(Records.Select(r => r.Name));
    }
}

public class SkillDiff
{
    public string Name         { get; set; } = "";
    public double P1Dps        { get; set; }
    public double P2Dps        { get; set; }
    public double DiffDps      { get; set; }
    public double DiffPct      { get; set; }
    public double P1Crit       { get; set; }
    public double P2Crit       { get; set; }
    public double P1Uptime     { get; set; }
    public double P2Uptime     { get; set; }
    public double P1Miss       { get; set; }
    public double P2Miss       { get; set; }
    public double P1AvgHit     { get; set; }
    public double P2AvgHit     { get; set; }
    public double EstDpsLost   { get; set; }
    public string RotationNote { get; set; } = "";
    public string Priority     { get; set; } = "normal";
}

public class ImprovementSuggestion
{
    public string Skill      { get; set; } = "";
    public string Category   { get; set; } = "";
    public string What       { get; set; } = "";
    public string Why        { get; set; } = "";
    public string When       { get; set; } = "";
    public double DpsGainEst { get; set; }
    public string Priority   { get; set; } = "normal";
}

public class ComparisonResult
{
    public PlayerAnalysis              Player1         { get; set; } = new();
    public PlayerAnalysis              Player2         { get; set; } = new();
    public List<SkillDiff>             SkillDiffs      { get; set; } = new();
    public List<string>                Insights        { get; set; } = new();
    public List<ImprovementSuggestion> Suggestions     { get; set; } = new();
    public double                      DiffPercent     { get; set; }
    public double                      TotalEstDpsLost { get; set; }
    public PlayerAnalysis Winner => Player1.TotalDamage >= Player2.TotalDamage ? Player1 : Player2;
    public PlayerAnalysis Loser  => ReferenceEquals(Winner, Player1) ? Player2 : Player1;
}
