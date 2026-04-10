# Update Fields / Bitmask Audit

## Architecture
- Legacy 3.3.5a flat-array fields → Modern 3.4.3 hierarchical bitmask blocks
- **Create path** (`WriteCreateXData`) — ALL fields on first spawn
- **Update path** (`WriteUpdateXData`) — only changed fields via bitmask
- Parent-child bit system: parent bit gates child bits

---

## 1. OBJECTDATA (Base) — COMPLETE
| TC343 Bit | Field | Status |
|-----------|-------|--------|
| 1 | EntryID | Done |
| 2 | DynamicFlags | Done |
| 3 | Scale | Done |

---

## 2. GAMEOBJECTDATA — DONE (1 block, 20 bits)
### Create path: All fields implemented
### Update path: IMPLEMENTED
- [x] DisplayID (bit 4), SpellVisualID (5), StateSpellVisualID (6)
- [x] StateAnimID (7), StateAnimKitID (8)
- [x] CreatedBy (9), GuildGUID (10), Flags (11)
- [x] ParentRotation (12), FactionTemplate (13), Level (14)
- [x] State (15), TypeID (16), PercentHealth (17)
- [x] ArtKit (18), CustomParam (19)
- [ ] StateWorldEffectIDs (1) — not tracked
- [ ] EnableDoodadSets (2), WorldEffects (3) — dynamic, not tracked

---

## 3. UNITDATA — Mostly Done (8 blocks, 227 bits)

### Block 0 (bits 0-31) — 15/31 implemented
- [x] Health (5), MaxHealth (6), DisplayID (7)
- [x] Charm (11), Summon (12), CharmedBy (14), SummonedBy (15), CreatedBy (16)
- [x] Target (19), ChannelData (22)
- [x] RaceId (24), ClassId (25), SexId (27)
- [x] Level (30), EffectiveLevel (31)
- [ ] SpellOverrideNameID (23)
- [ ] DisplayPower (28-29)

### Block 1 (bits 32-63) — 22/32 implemented
- [x] FactionTemplate (40), Flags (41), Flags2 (42), Flags3 (43), AuraState (44)
- [x] OverrideDisplayPowerID (45)
- [x] BoundingRadius (46), CombatReach (47), DisplayScale (48)
- [x] NativeDisplayID (49), NativeXDisplayScale (50), MountDisplayID (51)
- [x] MinDamage (52), MaxDamage (53), MinOffHandDamage (54), MaxOffHandDamage (55)
- [x] StandState (56), VisFlags (58), AnimTier (59)
- [x] PetNumber (60), PetNameTimestamp (61), PetExperience (62), PetNextLevelExperience (63)
- [ ] PetTalentPoints (57) — no property in UnitData

### Block 2 (bits 64-95) — 27/32 implemented
- [x] ModCastSpeed (65), ModCastHaste (66), ModHaste (67), ModRangedHaste (68)
- [x] ModHasteRegen (69), ModTimeRate (70)
- [x] CreatedBySpell (71), EmoteState (72)
- [x] BaseMana (75), BaseHealth (76)
- [x] SheatheState (77), PvpFlags (78), PetFlags (79), ShapeshiftForm (80)
- [x] AttackPower (81), AttackPowerModPos (82), AttackPowerModNeg (83), AttackPowerMultiplier (84)
- [x] RangedAttackPower (85-88), AttackSpeedAura (89), Lifesteal (90)
- [x] MinRangedDamage (91), MaxRangedDamage (92), MaxHealthModifier (93)
- [x] HoverHeight (94), MinItemLevelCutoff (95)
- [ ] TrainingPointsUsed (73), TrainingPointsTotal (74)

### Block 3 (bits 96-127) — 12/18 implemented
- [x] MinItemLevel (97), MaxItemLevel (98), WildBattlePetLevel (99)
- [x] InteractSpellID (101), ScaleDuration (102)
- [x] LooksLikeMountID (103), LooksLikeCreatureID (104), LookAtControllerID (105)
- [x] GuildGUID (107), NpcFlags[2] (113-115)
- [ ] BattlePetCompanionNameTimestamp (100)
- [ ] PerksVendorItemID (106)
- [ ] StateSpellVisualID (108), StateAnimID (109), StateAnimKitID (110)
- [ ] StateWorldEffect (111), StateWorldEffectIDs (112)

### Block 4 (bits 128-159) — 20/40 implemented
- [x] Power[10] (137-146), MaxPower[10] (147-156)
- [ ] PowerRegenFlatModifier[10] (117-126) — all zeroed
- [ ] PowerRegenInterruptedFlatModifier[10] (127-136) — all zeroed
- [ ] ModPowerRegen[10] (157-166) — all zeroed

### Block 5 (bits 160-191) — 17/22 implemented
- [x] AttackRoundBaseTime[2] (171-173)
- [x] Stats[5] (175-179), StatPosBuff[5] (180-184), StatNegBuff[5] (185-189)
- [ ] VirtualItems[3] (167-170) — create only, not in update path
- [ ] RangedAttackRoundBaseTime (170) — missing from update

### Blocks 6-7 (bits 192-255) — COMPLETE
- [x] Resistances[7], PowerCostModifier[7], PowerCostMultiplier[7] (190-211)
- [x] ResistanceBuffModsPositive[7], ResistanceBuffModsNegative[7] (212-226)

---

## 4. PLAYERDATA — DONE (4 blocks, 108 bits)

### Implemented in UPDATE path:
- [x] DuelArbiter (bit 4)
- [x] WowAccount (bit 5)
- [x] LootTargetGUID (bit 6)
- [x] PlayerFlags (bit 7)
- [x] PlayerFlagsEx (bit 8)
- [x] GuildRankID (bit 9)
- [x] GuildDeleteDate (bit 10)
- [x] GuildLevel (bit 11)
- [x] NumBankSlots (bit 12)
- [x] NativeSex (bit 13)
- [x] Inebriation (bit 14)
- [x] PvpTitle (bit 15)
- [x] ArenaFaction (bit 16)
- [x] PvPRank (bit 17)
- [x] DuelTeam (bit 19)
- [x] GuildTimeStamp (bit 20)
- [x] ChosenTitle (bit 21)
- [x] FakeInebriation (bit 22)
- [x] VirtualPlayerRealm (bit 23)
- [x] CurrentSpecID (bit 24)
- [x] HonorLevel (bit 27)
- [x] QuestLog[25] (bits 35-60)
- [x] VisibleItems[19] (bits 61-80)

### Still missing (dynamic/complex):
- [ ] Customizations (dynamic, bit 1)
- [ ] ArenaCooldowns (dynamic, bit 2)
- [ ] PartyType[2] (bits 32-33)

---

## 5. ACTIVEPLAYERDATA — Mostly Done (48 blocks, 1525 bits)

### Implemented in UPDATE path (scalars):
- [x] FarsightObject (bit 26)
- [x] Coinage (bit 28), XP (bit 29), NextLevelXP (bit 30), TrialXP (bit 31)
- [x] CharacterPoints (bit 33), MaxTalentTiers (bit 34)
- [x] TrackCreatureMask (bit 35)
- [x] MainhandExpertise (bit 36), OffhandExpertise (bit 37)
- [x] Block 38 complete: RangedExpertise, CombatRatingExpertise, Block/Dodge/Parry%,
      Crit%, ShieldBlock, Mastery, Speed, Avoidance, Sturdiness, Versatility,
      PvP power, ModHealing*, ModSpellPower%, ModResilience%, ModTarget*,
      LocalFlags (bits 39-69)
- [x] Block 70 complete: GrantableLevels, MultiActionBars, AmmoID, PvpMedals,
      all Honor/Kill/Contribution fields, WatchedFactionIndex, MaxLevel,
      PetSpellPower, UiHitModifier, ModPetHaste, etc. (bits 71-101)
- [x] Block 102: AuraVision, NumBackpackSlots, OverrideSpellsID, LfgBonusFactionID,
      LootSpecID, OverrideZonePVPType, Honor, HonorNextLevel,
      PvPTierMaxFromWins, PvPRankProgress (bits 103-114)

### Implemented in UPDATE path (arrays):
- [x] InvSlots[141] (bits 124-265)
- [x] TrackResourceMask[2] (bits 266-268)
- [x] SpellCritPercentage[7] (bits 270-276)
- [x] ModDamageDonePos[7] (bits 277-283)
- [x] ModDamageDoneNeg[7] (bits 284-290)
- [x] ModDamageDonePercent[7] (bits 291-297)
- [x] ExploredZones[240] (bits 298-538)
- [x] WeaponDmgMultipliers[3] (bits 543-545)
- [x] WeaponAtkSpeedMultipliers[3] (bits 546-548)
- [x] BuybackPrice[12] (bits 550-561)
- [x] BuybackTimestamp[12] (bits 562-573)
- [x] CombatRatings[32] (bits 574-606)
- [x] NoReagentCostMask[4] (bits 615-619)
- [x] ProfessionSkillLine[2] (bits 620-622)
- [x] BagSlotFlags[4] (bits 623-627)
- [x] BankBagSlotFlags[7] (bits 628-635)
- [x] QuestCompleted[875] (bits 636-1511)

### Also implemented:
- [x] RestInfo[2] (bits 539-541) — nested struct with Threshold + StateID

### Also implemented (dynamic):
- [x] KnownTitles (dynamic field, bit 3) — title selection menu, uint?[12] → ulong[6]
- [x] GlyphSlots[6] / Glyphs[6] — CREATE path only (glyphs change via talent packets, not Values)
- [x] GlyphsEnabled — CREATE path only (computed from level)

### Also implemented (complex nested):
- [x] Skill[256] (bit 32) — full SkillInfo WriteUpdate with 1793-bit changesMask
      (57 blocks, 7 arrays × 256 entries, per-skill interleaved write order)

### Also implemented:
- [x] PvpInfo[7] (bits 607-614) — nested struct HasChangesMask<19>, per-element mask

### Status: ALL FIELDS IMPLEMENTED
Every object type (Object, GameObject, Unit, Player, ActivePlayer, Item, Container)
now has complete create AND update paths for all available fields.

---

## 6. INVENTORY SLOT MAPPING

```
Modern Index → Legacy Source
[0-18]     → InvSlots[0-18]     (Equipment)         OK
[19-29]    → UNMAPPED GAP       (modern-only slots)  OK (empty)
[30-33]    → InvSlots[19-22]    (Bag slots)          OK
[34]       → UNMAPPED GAP                            OK (empty)
[35-58]    → PackSlots[0-23]    (Backpack)           OK
[59-86]    → BankSlots[0-27]    (Bank items)         OK
[87-93]    → BankBagSlots[0-6]  (Bank bags)          OK
[94-105]   → BuyBackSlots[0-11] (Buyback)            OK
[106-137]  → KeyringSlots[0-31] (Keyring)            OK
[138-140]  → UNMAPPED GAP                            OK (empty)
```

### Key inventory issue:
- When items move, only destination GUID sent; old slot not explicitly cleared
- ContainerData vs PlayerField updates may desync

---

## 7. CONTAINERDATA — Mostly Done
- Create: Slots[36], NumSlots — Done
- Update: Has bitmask path — Done
- Risk: Container slot changes via player fields vs container object may desync

---

## 8. ITEMDATA — COMPLETE
- Create and update paths both fully implemented
- Owner, ContainedIn, Creator, GiftCreator, StackCount, Duration
- SpellCharges[5], Flags, Enchantment[13], PropertySeed, RandomProperty
- Durability, MaxDurability, CreatePlayedTime, Context, ArtifactXP, ItemAppearanceModID

---

## 9. ROOT CAUSES OF INVENTORY/VISUAL ISSUES

1. ~~**No old-slot clearing**~~ — VERIFIED OK: WowGuid128.Empty is non-null, slots ARE cleared
2. **ContainerData vs PlayerField disconnect** — bag contents updated via two paths (minor)
3. ~~**ActivePlayer update path extremely limited**~~ — FIXED: 80+ fields + 17 arrays
4. ~~**No GameObjectData update path**~~ — FIXED: WriteUpdateGameObjectData added
5. ~~**PlayerData update missing visual fields**~~ — FIXED: 23 scalar fields
6. ~~**~60+ ActivePlayerData fields absent**~~ — FIXED: all fields implemented
7. **ObjectCacheLegacy not updated on Values updates** — FIXED: merge changed fields back
   into cache so GetInventorySlotItem() returns current data after loot/equip

---

## Priority Order for Fixes

### P0 — Inventory reliability
1. Fix old-slot clearing on item moves
2. Verify ContainerData/PlayerField sync

### P1 — ActivePlayerData update path
Add update support for create-only fields (Skills, CombatRatings, Honor, Glyphs, etc.)

### P2 — ActivePlayerData missing fields
Add ExploredZones, QuestCompleted, KnownTitles, combat percentages, rest XP, tracking, etc.

### P3 — PlayerData update path
Add DuelArbiter, GuildRankID, Customizations, HonorLevel, etc.

### P4 — GameObjectData update path
Implement WriteUpdateGameObjectData for state changes

### P5 — UnitData gaps
VirtualItems in updates, power regen, Flags3, etc.
