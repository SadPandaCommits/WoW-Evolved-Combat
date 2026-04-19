using System.Text.RegularExpressions;
using WowEvolved.Models;
using WowEvolved.Services;

namespace WowEvolved.Services;

public class DamageAnalysisService
{
    public PlayerAnalysis ParseCsv(
        Stream csvStream, string fileName, Lang lang,
        Stream? jsonStream = null,
        string? overrideClass = null, string? overrideSpec = null)
    {
        using var reader = new StreamReader(csvStream);
        var lines = new List<string>();
        while (!reader.EndOfStream) { var ln=reader.ReadLine(); if(!string.IsNullOrWhiteSpace(ln))lines.Add(ln); }
        if (lines.Count < 2) throw new Exception(lang==Lang.EN?"Invalid or empty CSV.":"CSV inválido ou vazio.");

        var hdrs = ParseCsvLine(lines[0]);
        int Idx(string n)=>hdrs.FindIndex(h=>h.Trim().Equals(n,StringComparison.OrdinalIgnoreCase));
        int iName=Idx("Name"),iAmount=Idx("Amount"),iDps=Idx("DPS"),
            iCasts=Idx("Casts"),iAvgCast=Idx("Avg Cast"),iHits=Idx("Hits"),
            iAvgHit=Idx("Avg Hit"),iCrit=Idx("Crit %"),iUptime=Idx("Uptime %"),iMiss=Idx("Miss %");

        var records = new List<DamageRecord>();
        for(int i=1;i<lines.Count;i++){
            var cols=ParseCsvLine(lines[i]);
            string Get(int x)=>x>=0&&x<cols.Count?cols[x].Trim():"";
            var r=new DamageRecord{Name=Get(iName),Amount=Get(iAmount),DPS=Get(iDps),
                Casts=Get(iCasts),AvgCast=Get(iAvgCast),Hits=Get(iHits),
                AvgHit=Get(iAvgHit),CritPct=Get(iCrit),UptimePct=Get(iUptime),MissPct=Get(iMiss)};
            if(string.IsNullOrWhiteSpace(r.Name))continue;
            r.Parse(); records.Add(r);
        }

        var p=new PlayerAnalysis{PlayerName=ExtractPlayerName(fileName),FileName=fileName,Records=records};
        p.Compute();

        // Auto-detect using the correct language registry
        p.DetectedSpec = lang==Lang.EN
            ? WowClassRegistryEN.DetectFromSkills(records.Select(r=>r.Name))
            : WowClassRegistry.DetectFromSkills(records.Select(r=>r.Name));

        if(jsonStream!=null){
            var(custom,err)=CustomSpecProfile.TryParse(jsonStream);
            if(custom!=null){var sp=custom.ToSpecProfile();sp.IsCustom=true;p.OverrideSpec=sp;}
            else if(err!=null) p.JsonParseError=err;
        }
        else if(!string.IsNullOrEmpty(overrideClass)&&!string.IsNullOrEmpty(overrideSpec))
            p.OverrideSpec = lang==Lang.EN
                ? WowClassRegistryEN.GetSpec(overrideClass,overrideSpec)
                : WowClassRegistry.GetSpec(overrideClass,overrideSpec);

        return p;
    }

    public ComparisonResult Compare(PlayerAnalysis p1, PlayerAnalysis p2, Lang lang)
    {
        var winner=p1.TotalDamage>=p2.TotalDamage?p1:p2;
        var loser=ReferenceEquals(winner,p1)?p2:p1;
        var result=new ComparisonResult{Player1=p1,Player2=p2};
        result.DiffPercent=loser.TotalDamage>0?(winner.TotalDamage-loser.TotalDamage)/(double)loser.TotalDamage*100:0;

        var s1=BuildSkillMap(p1); var s2=BuildSkillMap(p2);
        result.SkillDiffs=s1.Keys.Union(s2.Keys,StringComparer.OrdinalIgnoreCase).Select(k=>{
            s1.TryGetValue(k,out var a); s2.TryGetValue(k,out var b);
            double d1d=a?.Dps??0,d2d=b?.Dps??0,d1c=a?.Crit??0,d2c=b?.Crit??0,
                   d1u=a?.Uptime??0,d2u=b?.Uptime??0,d1m=a?.Miss??0,d2m=b?.Miss??0,
                   d1h=a?.AvgHit??0,d2h=b?.AvgHit??0;
            bool lP1=ReferenceEquals(loser,p1);
            double ld=lP1?d1d:d2d,wd=lP1?d2d:d1d,lu=lP1?d1u:d2u,wu=lP1?d2u:d1u,
                   lc=lP1?d1c:d2c,wc=lP1?d2c:d1c,lost=Math.Max(0,wd-ld);
            string note=BuildRotationNote(k,loser.ActiveSpec,lost,ld,wd,lu,wu,lc,wc,lang);
            string prio=lost>3000?"critical":lost>1000?"high":"normal";
            return new SkillDiff{Name=k,P1Dps=d1d,P2Dps=d2d,DiffDps=d2d-d1d,
                P1Crit=d1c,P2Crit=d2c,P1Uptime=d1u,P2Uptime=d2u,P1Miss=d1m,P2Miss=d2m,
                P1AvgHit=d1h,P2AvgHit=d2h,EstDpsLost=lost,RotationNote=note,Priority=prio};
        }).OrderByDescending(x=>Math.Abs(x.DiffDps)).ToList();

        result.TotalEstDpsLost=result.SkillDiffs.Sum(x=>x.EstDpsLost);
        result.Insights=GenerateInsights(p1,p2,winner,loser,result.SkillDiffs,lang);
        result.Suggestions=GenerateSuggestions(loser,winner,result.SkillDiffs,p1,p2,lang);
        return result;
    }

    private static string BuildRotationNote(string skill,SpecProfile spec,double lost,
        double ld,double wd,double lu,double wu,double lc,double wc,Lang lang)
    {
        if(ld==0&&wd>0) return LocalizationService.Sug.NotUsedNote(lang,wd);
        var parts=new List<string>();
        var pe=spec.CustomPriorityList.FirstOrDefault(e=>
            e.Skill.Contains(skill,StringComparison.OrdinalIgnoreCase)||
            skill.Contains(e.Skill,StringComparison.OrdinalIgnoreCase));
        if(pe!=null) parts.Add(LocalizationService.Sug.PrioNote(lang,spec.CustomPriorityList.IndexOf(pe)+1,pe.Note,"",0));
        spec.CustomUptimeThresholds.TryGetValue(skill,out double thr);
        if(thr>0&&lu<thr) parts.Add(LocalizationService.Sug.ThresholdNote(lang,lu,thr));
        switch(spec.Archetype){
            case RotationArchetype.DOT:      if(lu<wu-8)  parts.Add(LocalizationService.Sug.UptimeLowNote(lang,lu,wu)); break;
            case RotationArchetype.Proc:     if(lc<wc-8)  parts.Add(LocalizationService.Sug.CritLowNote(lang,lc,wc));   break;
            case RotationArchetype.Burst:    if(lost>1500) parts.Add(LocalizationService.Sug.BurstNote(lang));            break;
            case RotationArchetype.Pet:      if(lost>800)  parts.Add(LocalizationService.Sug.PetNote(lang));              break;
            case RotationArchetype.Combo:    if(lu<wu-5)   parts.Add(LocalizationService.Sug.ComboNote(lang));            break;
            case RotationArchetype.Cooldown: if(lost>1500) parts.Add(LocalizationService.Sug.CooldownNote(lang));         break;
        }
        if(lu>0&&wu-lu>10&&parts.Count==0) parts.Add(LocalizationService.Sug.UptimeWeakAura(lang,lu,wu));
        if(lost>2000&&parts.Count==0)      parts.Add(LocalizationService.Sug.BigGapNote(lang,lost));
        return string.Join(" | ",parts);
    }

    private static List<string> GenerateInsights(
        PlayerAnalysis p1,PlayerAnalysis p2,PlayerAnalysis winner,PlayerAnalysis loser,
        List<SkillDiff> diffs,Lang lang)
    {
        var ins=new List<string>();
        bool lP1=ReferenceEquals(loser,p1);
        double LD(SkillDiff d)=>lP1?d.P1Dps:d.P2Dps;
        double WD(SkillDiff d)=>lP1?d.P2Dps:d.P1Dps;

        ins.Add(LocalizationService.Insight.Winner(lang,winner.PlayerName,winner.TotalDamage-loser.TotalDamage,
            loser.TotalDamage>0?(winner.TotalDamage-loser.TotalDamage)/(double)loser.TotalDamage*100:0));
        ins.Add(LocalizationService.Insight.DpsGap(lang,p1.PlayerName,p1.TotalDps,p2.PlayerName,p2.TotalDps,Math.Abs(p1.TotalDps-p2.TotalDps)));

        string SpecLbl(PlayerAnalysis p){
            var c=p.ActiveSpec.ClassName=="Unknown"
                ? LocalizationService.UI.ClassNotDet(lang)
                : $"{p.ActiveSpec.ClassName} — {p.ActiveSpec.SpecName}";
            if(p.ActiveSpec.IsCustom) c+=" "+LocalizationService.UI.CustomJson(lang);
            return c;
        }
        ins.Add(LocalizationService.Insight.ClassDet(lang,p1.PlayerName,SpecLbl(p1),p2.PlayerName,SpecLbl(p2)));
        ins.Add(LocalizationService.Insight.Duration(lang,p1.PlayerName,p1.FightDuration,p2.PlayerName,p2.FightDuration,
            Math.Abs(p1.FightDuration-p2.FightDuration)<=10));
        ins.Add(LocalizationService.Insight.EffScore(lang,p1.PlayerName,p1.EfficiencyScore,p2.PlayerName,p2.EfficiencyScore));
        ins.Add(LocalizationService.Insight.UptimeAvg(lang,p1.PlayerName,p1.AvgUptimePct,p2.PlayerName,p2.AvgUptimePct,
            Math.Abs(p1.AvgUptimePct-p2.AvgUptimePct)>5));
        ins.Add(LocalizationService.Insight.CritAvg(lang,p1.PlayerName,p1.AvgCritPct,p2.PlayerName,p2.AvgCritPct));

        var spec=loser.ActiveSpec;
        if(spec.IsCustom&&spec.CustomUptimeThresholds.Any())
            foreach(var(sk,th)in spec.CustomUptimeThresholds){
                var rec=loser.Records.FirstOrDefault(r=>r.Name.Contains(sk,StringComparison.OrdinalIgnoreCase)||sk.Contains(r.Name,StringComparison.OrdinalIgnoreCase));
                if(rec==null||rec.UptimeNum>=th-5)continue;
                ins.Add(LocalizationService.Insight.ThresholdBreach(lang,loser.PlayerName,rec.UptimeNum,rec.Name,th,rec.DpsNum*(th-rec.UptimeNum)/100));
            }

        var arch=loser.ActiveSpec.Archetype;
        if(arch==RotationArchetype.DOT&&loser.AvgUptimePct<winner.AvgUptimePct-5)
            ins.Add(LocalizationService.Insight.AlertDot(lang,loser.PlayerName,loser.AvgUptimePct,winner.AvgUptimePct));
        else if(arch==RotationArchetype.Pet)
            ins.Add(LocalizationService.Insight.AlertPet(lang,loser.ActiveSpec.SpecName));
        else if(arch==RotationArchetype.Burst&&loser.AvgCritPct<winner.AvgCritPct-8)
            ins.Add(LocalizationService.Insight.AlertBurst(lang,loser.PlayerName,loser.AvgCritPct,winner.AvgCritPct));

        foreach(var d in diffs.Where(x=>WD(x)-LD(x)>500).OrderByDescending(x=>WD(x)-LD(x)).Take(5)){
            double gap=WD(d)-LD(d);
            string icon=d.Priority=="critical"?"🔴":d.Priority=="high"?"🟡":"🔵";
            ins.Add(LocalizationService.Insight.SkillGap(lang,icon,d.Name,loser.PlayerName,LD(d),winner.PlayerName,WD(d),gap,
                loser.TotalDps>0?gap/loser.TotalDps*100:0));
        }
        var missing=diffs.Where(d=>LD(d)==0&&WD(d)>500).ToList();
        if(missing.Any()) ins.Add(LocalizationService.Insight.Missing(lang,loser.PlayerName,string.Join(", ",missing.Select(x=>x.Name))));
        return ins;
    }

    private static List<ImprovementSuggestion> GenerateSuggestions(
        PlayerAnalysis loser,PlayerAnalysis winner,List<SkillDiff> diffs,
        PlayerAnalysis p1,PlayerAnalysis p2,Lang lang)
    {
        var sugs=new List<ImprovementSuggestion>();
        bool lP1=ReferenceEquals(loser,p1);
        double LD(SkillDiff d)=>lP1?d.P1Dps:d.P2Dps;
        double WD(SkillDiff d)=>lP1?d.P2Dps:d.P1Dps;
        double LU(SkillDiff d)=>lP1?d.P1Uptime:d.P2Uptime;
        double WU(SkillDiff d)=>lP1?d.P2Uptime:d.P1Uptime;
        double LC(SkillDiff d)=>lP1?d.P1Crit:d.P2Crit;
        double WC(SkillDiff d)=>lP1?d.P2Crit:d.P1Crit;
        var spec=loser.ActiveSpec;

        if(spec.IsCustom&&spec.CustomPriorityList.Any()){
            foreach(var e in spec.CustomPriorityList.Where(x=>x.IsCoreRotation)){
                var d=diffs.FirstOrDefault(x=>x.Name.Contains(e.Skill,StringComparison.OrdinalIgnoreCase)||e.Skill.Contains(x.Name,StringComparison.OrdinalIgnoreCase));
                if(d==null)continue; double gap=WD(d)-LD(d); if(gap<300)continue;
                int prio=spec.CustomPriorityList.IndexOf(e)+1;
                sugs.Add(new(){Skill=d.Name,Category=LocalizationService.Sug.PrioCat(lang,prio),
                    What=LocalizationService.Sug.PriorityWhat(lang,e.Note),Why=LocalizationService.Sug.PriorityWhy(lang,prio,winner.PlayerName,gap),
                    When=LocalizationService.Sug.PriorityWhen(lang),DpsGainEst=gap*0.75,Priority=prio<=2?"critical":"high"});
            }
            foreach(var e in spec.CustomPriorityList.Where(x=>!x.IsCoreRotation)){
                var d=diffs.FirstOrDefault(x=>x.Name.Contains(e.Skill,StringComparison.OrdinalIgnoreCase)||e.Skill.Contains(x.Name,StringComparison.OrdinalIgnoreCase));
                if(d==null)continue; double gap=WD(d)-LD(d); if(gap<500)continue;
                int prio=spec.CustomPriorityList.IndexOf(e)+1;
                sugs.Add(new(){Skill=d.Name,Category=LocalizationService.Sug.FillerCat(lang,prio),
                    What=e.Note,Why=LocalizationService.Sug.FillerWhy(lang,gap),When=LocalizationService.Sug.FillerWhen(lang),
                    DpsGainEst=gap*0.5,Priority="normal"});
            }
            foreach(var(sk,th)in spec.CustomUptimeThresholds){
                var rec=loser.Records.FirstOrDefault(r=>r.Name.Contains(sk,StringComparison.OrdinalIgnoreCase)||sk.Contains(r.Name,StringComparison.OrdinalIgnoreCase));
                if(rec==null||rec.UptimeNum>=th-5)continue;
                double est=rec.DpsNum*(th-rec.UptimeNum)/100;
                sugs.Add(new(){Skill=rec.Name,Category=lang==Lang.EN?"Uptime":"Uptime",
                    What=LocalizationService.Sug.ThresholdWhat(lang,rec.Name,rec.UptimeNum,th),
                    Why=LocalizationService.Sug.ThresholdWhy(lang,spec.UptimeTip,est),When=LocalizationService.Sug.ThresholdWhen(lang),
                    DpsGainEst=est,Priority=rec.UptimeNum<th-20?"critical":"high"});
            }
        } else {
            if(loser.AvgUptimePct<winner.AvgUptimePct-5)
                sugs.Add(new(){Skill=lang==Lang.EN?"General Uptime":"Uptime Geral",
                    Category=lang==Lang.EN?"Rotation":"Rotação",
                    What=LocalizationService.Sug.UptimeGeneral(lang,loser.AvgUptimePct,winner.AvgUptimePct),
                    Why=LocalizationService.Sug.UptimeWhy(lang,spec.UptimeTip,loser.TotalDps*0.008),
                    When=LocalizationService.Sug.UptimeWhen(lang),
                    DpsGainEst=(winner.AvgUptimePct-loser.AvgUptimePct)/100.0*loser.TotalDps*0.8,
                    Priority=loser.AvgUptimePct<winner.AvgUptimePct-15?"critical":"high"});
        }

        foreach(var d in diffs.OrderByDescending(x=>WD(x)-LD(x)).Take(10)){
            double gap=WD(d)-LD(d),ud=WU(d)-LU(d),cd=WC(d)-LC(d);
            if(sugs.Any(s=>s.Skill.Equals(d.Name,StringComparison.OrdinalIgnoreCase)))continue;
            if(gap<200&&ud<5&&Math.Abs(cd)<5)continue;
            if(LD(d)==0&&WD(d)>400){
                sugs.Add(new(){Skill=d.Name,Category=lang==Lang.EN?"Build":"Build",
                    What=LocalizationService.Sug.BuildWhat(lang,d.Name),Why=LocalizationService.Sug.BuildWhy(lang,winner.PlayerName,WD(d)),
                    When=LocalizationService.Sug.BuildWhen(lang),DpsGainEst=WD(d)*0.85,Priority=WD(d)>3000?"critical":"high"});continue;
            }
            if(ud>8&&gap>200){
                sugs.Add(new(){Skill=d.Name,Category=lang==Lang.EN?"Uptime":"Uptime",
                    What=LocalizationService.Sug.UptimeWhat(lang,d.Name,LU(d),WU(d)),
                    Why=LocalizationService.Sug.UptimeSkillWhy(lang,spec.UptimeTip,winner.PlayerName,ud,gap),
                    When=LocalizationService.Sug.UptimeSkillWhen(lang),DpsGainEst=gap*0.65,Priority=ud>20?"critical":"high"});continue;
            }
            if(cd>8&&gap>200){
                sugs.Add(new(){Skill=d.Name,Category=lang==Lang.EN?"Timing":"Timing",
                    What=LocalizationService.Sug.TimingWhat(lang,d.Name),
                    Why=LocalizationService.Sug.TimingWhy(lang,spec.BurstTip,winner.PlayerName,cd,gap),
                    When=LocalizationService.Sug.TimingWhen(lang),DpsGainEst=gap*0.5,Priority="normal"});continue;
            }
            if(gap>1500)
                sugs.Add(new(){Skill=d.Name,Category=lang==Lang.EN?"Rotation":"Rotação",
                    What=LocalizationService.Sug.GenWhat(lang,d.Name),Why=LocalizationService.Sug.GenWhy(lang,gap,spec.GeneralTip),
                    When=LocalizationService.Sug.GenWhen(lang),DpsGainEst=gap*0.45,Priority=gap>4000?"critical":"normal"});
        }

        double bl=diffs.Where(d=>WD(d)-LD(d)>1500).Sum(d=>WD(d)-LD(d));
        if(bl>3000) sugs.Add(new(){Skill=lang==Lang.EN?"Burst Alignment":"Alinhamento de Burst",
            Category=lang==Lang.EN?"Burst":"Burst",What=LocalizationService.Sug.BurstWhat(lang),Why=LocalizationService.Sug.BurstWhy(lang,spec.BurstTip,bl),
            When=LocalizationService.Sug.BurstWhen(lang),DpsGainEst=bl*0.5,Priority="high"});

        if(diffs.Any(d=>WD(d)-LD(d)>2000))
            sugs.Add(new(){Skill=lang==Lang.EN?"AoE / Multi-Target":"AOE / Multi-Target",
                Category=lang==Lang.EN?"AoE":"AOE",What=LocalizationService.Sug.AoeWhat(lang),Why=LocalizationService.Sug.AoeWhy(lang,spec.AoeTip,winner.PlayerName),
                When=LocalizationService.Sug.AoeWhen(lang),DpsGainEst=diffs.Where(d=>WD(d)-LD(d)>2000).Sum(d=>WD(d)-LD(d))*0.3,Priority="normal"});

        return sugs.OrderBy(s=>s.Priority=="critical"?0:s.Priority=="high"?1:2)
                   .ThenByDescending(s=>s.DpsGainEst).ToList();
    }

    private record SkillStats(double Dps,double Crit,double Uptime,double Miss,double AvgHit);

    private static Dictionary<string,SkillStats> BuildSkillMap(PlayerAnalysis p)
        =>p.Records.GroupBy(r=>r.Name,StringComparer.OrdinalIgnoreCase)
           .ToDictionary(g=>g.Key,g=>new SkillStats(
               g.Sum(r=>r.DpsNum),
               g.Where(r=>r.CritNum>0).Select(r=>r.CritNum).DefaultIfEmpty(0).Average(),
               g.Where(r=>r.UptimeNum>0).Select(r=>r.UptimeNum).DefaultIfEmpty(0).Average(),
               g.Where(r=>r.MissNum>0).Select(r=>r.MissNum).DefaultIfEmpty(0).Average(),
               g.Where(r=>r.AvgHitNum>0).Select(r=>r.AvgHitNum).DefaultIfEmpty(0).Average()),
           StringComparer.OrdinalIgnoreCase);

    private static List<string> ParseCsvLine(string line){
        var f=new List<string>();var sb=new System.Text.StringBuilder();bool q=false;
        foreach(char c in line){if(c=='"')q=!q;else if(c==','&&!q){f.Add(sb.ToString());sb.Clear();}else sb.Append(c);}
        f.Add(sb.ToString());return f;
    }

    private static string ExtractPlayerName(string file){
        var n=Path.GetFileNameWithoutExtension(file);
        var m=Regex.Match(n,@"Damage_Done_([^_]+(?:_[^_]+)*)_-_");
        if(m.Success)return m.Groups[1].Value.Replace("_"," ");
        var p=n.Split(new[]{" - ","_-_"},StringSplitOptions.None);
        return p.Length>0?p[0].Replace("Damage Done","").Replace("Damage_Done_","").Trim():n;
    }
}
