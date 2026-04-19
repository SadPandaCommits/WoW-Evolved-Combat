# WoW-Evolved-Combat

> **Warcraft Logs DPS analyzer with class-aware insights, custom JSON profiles, and PT/EN support.**  
> Upload two CSV exports. Get a full breakdown of why one player outperformed the other.

---

## What it does

WoW Evolved compares two **Warcraft Logs Damage Done CSV exports** side by side and produces a detailed analysis — ability by ability, stat by stat — with concrete suggestions on what to fix, why it matters, and when to apply it during the fight.

No account. No login. No data sent anywhere. Runs entirely on your machine.

---

## Features

- **Automatic class detection** across all 13 WoW classes and 27 specs via keyword scoring
- **Per-ability breakdown** — DPS, Crit%, Uptime%, Miss%, Avg Hit, estimated DPS lost
- **Improvement plan** with three priority tiers: `CRITICAL` · `HIGH` · `NORMAL`
- **Custom JSON profiles** — replace the built-in spec profile with your own priority list, uptime thresholds, and phase notes
- **Efficiency Score** — blended metric of uptime, crit utilisation, and DPS density
- **Drag & drop upload** with live class detection preview
- **English and Portuguese** — full server-side localisation, switched via `?lang=en` / `?lang=pt`
- **Classic WoW UI aesthetic** — dark gold theme inspired by Vanilla WoW frames
- **Zero external dependencies** — hand-written RFC-4180 CSV parser, no NuGet packages required

---

## Quick Start

**Requirements:** [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)

```bash
git clone https://github.com/SADPANDA/wow-evolved.git
cd wow-evolved
dotnet run
```

Open `http://localhost:5000` — or `http://localhost:5000?lang=en` for English.

---

## How to export CSVs from Warcraft Logs

1. Open your report on [warcraftlogs.com](https://warcraftlogs.com)
2. Go to the **Damage Done** tab
3. Select the boss/pull you want
4. Click **Export → CSV**
5. Repeat for the second player
6. Upload both on the WoW Evolved home page

---

## Custom JSON Profiles

The built-in spec profiles are intentionally generic. For your specific talent build, boss strategy, or guild meta, upload a `.json` alongside the CSV to override everything.

**Click ⬇ Template on the upload page** — the app generates a pre-filled `.json` for the auto-detected spec. Edit it in any text editor and re-upload.

### Full schema

```json
{
  "className": "Warlock",
  "specName": "Affliction",
  "role": "DPS",
  "style": "Ranged",
  "archetype": "DOT",

  "signatureKeywords": [
    "agony", "corruption", "unstable affliction", "malefic rapture"
  ],

  "uptimeTip": "Agony, Corruption and UA must have 100% uptime simultaneously.",
  "burstTip": "Dark Soul: Misery aligned with Rapture of Soul Shards is the burst.",
  "aoeTip": "Seed of Corruption to instantly spread Corruption to multiple targets.",
  "generalTip": "Manage Soul Shards — never cap; never spend without DoTs refreshed.",

  "priorityList": [
    {
      "skill": "Unstable Affliction",
      "note": "Highest priority — never let it drop. Reapply at 4s remaining.",
      "isCoreRotation": true
    },
    {
      "skill": "Agony",
      "note": "Second priority — 100% uptime for Soul Shard generation.",
      "isCoreRotation": true
    },
    {
      "skill": "Malefic Rapture",
      "note": "Spend at 5 shards — never overcap.",
      "isCoreRotation": true
    },
    {
      "skill": "Drain Soul",
      "note": "Execute filler below 20%.",
      "isCoreRotation": false
    }
  ],

  "uptimeThresholds": {
    "Agony": 98.0,
    "Corruption": 97.0,
    "Unstable Affliction": 95.0
  },

  "phaseNotes": [
    {
      "phase": "Pull / Opener",
      "note": "Pre-pot on pull timer. Apply DoTs: Agony → Corruption → UA. Activate Dark Soul immediately."
    },
    {
      "phase": "Add Phase",
      "note": "Maintain boss DoTs. Apply Seed of Corruption to the largest add cluster."
    },
    {
      "phase": "Execute (sub-20%)",
      "note": "Switch to Drain Soul for Inevitable Demise stacks. Keep DoTs rolling."
    }
  ]
}
```

### Field reference

| Field | Required | Description |
|---|---|---|
| `className` | ✅ | WoW class name. E.g. `"Warlock"` |
| `specName` | ✅ | Spec name. E.g. `"Affliction"` |
| `role` | | `"DPS"` · `"Tank"` · `"Healer"` |
| `style` | | `"Melee"` or `"Ranged"` |
| `archetype` | | `DOT` · `Proc` · `Burst` · `Pet` · `Combo` · `Cooldown` · `Universal` |
| `signatureKeywords` | | Lowercase ability name fragments used for auto-detection |
| `uptimeTip` | | Shown in uptime-related suggestions |
| `burstTip` | | Shown in burst alignment suggestions |
| `aoeTip` | | Shown in AoE / multi-target suggestions |
| `generalTip` | | Fallback tip for generic rotation suggestions |
| `priorityList` | | Ordered array of abilities. Index 0 = highest priority |
| `priorityList[].skill` | | Ability name (partial match, case-insensitive) |
| `priorityList[].note` | | Rotation instruction shown in the result and suggestion cards |
| `priorityList[].isCoreRotation` | | `true` → flagged CRITICAL/HIGH if underperforming · `false` → flagged NORMAL |
| `uptimeThresholds` | | `{ "Ability Name": 95.0 }` — triggers alert and DPS loss estimate if breached |
| `phaseNotes` | | `[{ "phase": "...", "note": "..." }]` — contextual tips shown in a dedicated section |

**The JSON always takes priority over the built-in profile and the dropdown override.**

---

## Project Structure

```
WowEvolved/
├── Controllers/
│   └── HomeController.cs          # Upload, Analyze, GetSpecs, DownloadTemplate
├── Models/
│   ├── ClassDefinitions.cs        # PT + EN spec registries for all 27 specs
│   ├── CustomSpecProfile.cs       # JSON schema, parser, template generator
│   └── DamageRecord.cs            # CSV row model, PlayerAnalysis, ComparisonResult
├── Services/
│   ├── DamageAnalysisService.cs   # CSV parsing, comparison engine, insight/suggestion generation
│   └── LocalizationService.cs     # All UI strings, insight strings, suggestion strings in PT + EN
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml           # Upload page
│   │   └── Result.cshtml          # Analysis dashboard
│   └── Shared/
│       └── _LangSwitcher.cshtml   # Floating PT/EN flag button
└── Program.cs
```

---

## Supported Classes

All 13 classes with individual spec profiles in both English and Portuguese:

| Class | Specs |
|---|---|
| Death Knight | Blood · Frost · Unholy |
| Demon Hunter | Havoc · Vengeance |
| Druid | Balance · Feral |
| Evoker | Devastation · Augmentation |
| Hunter | Beast Mastery · Marksmanship · Survival |
| Mage | Arcane · Fire · Frost |
| Monk | Windwalker |
| Paladin | Retribution |
| Priest | Shadow |
| Rogue | Assassination · Outlaw · Subtlety |
| Shaman | Elemental · Enhancement |
| Warlock | Affliction · Demonology · Destruction |
| Warrior | Arms · Fury |

---

## Rotation Archetypes

Each spec is tagged with an archetype that shapes the analysis:

| Archetype | How it affects suggestions |
|---|---|
| `DOT` | Flags uptime gaps as critical; calculates DoT drop DPS loss |
| `Proc` | Flags crit% gaps; suggests use inside proc/buff windows |
| `Burst` | Flags burst window misalignment; checks CD stacking |
| `Pet` | Warns if pet-sourced abilities are underperforming |
| `Combo` | Checks resource spender efficiency and sub-cap builders |
| `Cooldown` | Flags CD misalignment with trinkets and Bloodlust |
| `Universal` | Pure statistical analysis — no archetype-specific logic |

---

## Known Limitations

- **No phase detection** — the Damage Done CSV has no timestamps. Phase notes in JSON are informational, not automatically linked to fight segments.
- **Cross-spec comparisons** — mechanically valid but improvement suggestions target the losing player's class specifically. Absolute DPS numbers across different specs are not comparable.
- **No DPS-over-time graph** — requires timeline data from the Warcraft Logs API, not the CSV export.

---

## Tech Stack

- **Runtime:** .NET 8 / ASP.NET Core MVC
- **CSV parser:** Hand-written, RFC-4180 compliant, no external packages
- **Frontend:** Vanilla HTML/CSS/JS — no framework, no build step
- **Localisation:** Server-side, `?lang=en` / `?lang=pt` URL parameter
- **Dependencies:** Zero NuGet packages

---

## AI Disclosure

This project was scaffolded and translated with the help of Claude (Anthropic). The analysis engine — uptime thresholds, DPS loss estimates, archetype detection, priority-list scoring — is deterministic C# code with no AI at runtime. The English and Portuguese class profiles were generated with AI assistance and reviewed for WoW-specific accuracy.

---

## License

MIT — do whatever you want with it.

---

*Built by a Brazilian WoW player who got tired of arguing about logs after every pull.*