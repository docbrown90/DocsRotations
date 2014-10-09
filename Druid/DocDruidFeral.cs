using System;
using System.Linq;
using ReBot.API;

namespace ReBot
{
	[Rotation("Doc's Druid Feral", "docbrown", WoWClass.Druid, Specialization.DruidFeral, 5, 35)]
	public class DocDruidFeral : CombatRotation
	{
		// This is important, some mobs can't get the rake debuff. If this is missing the bot would always try rake...
		AutoResetDelay rakeDelay = new AutoResetDelay(7000);

		public DocDruidFeral()
		{
			GroupBuffs = new string[]
			{
				"Mark of the Wild"
			};
			PullSpells = new string[]
			{

			};
		}

		public override bool OutOfCombat()
		{
			if (CastSelf("Rejuvenation", () => Me.HealthFraction <= 0.75 && !HasAura("Rejuvenation"))) return true;
			if (CastSelf("Innervate", () => Me.GetPower(WoWPowerType.Mana) <= 0.6 && !HasAura("Innervate"))) return true;
			if (CastSelfPreventDouble("Healing Touch", () => Me.HealthFraction <= 0.5)) return true;
			if (CastSelf("Remove Corruption", () => Me.Auras.Any(x => x.IsDebuff && "Curse,Poison".Contains(x.DebuffType)))) return true;

			if (CastSelf("Mark of the Wild", () => !HasAura("Mark of the Wild") && !HasAura("Blessing of Kings"))) return true;
			if (CastSelf("Cat Form", () => Me.MovementSpeed != 0 && !Me.IsSwimming && Me.DisplayId == Me.NativeDisplayId)) return true;

			return false;
		}

		public override void Combat()
		{
			CastSelf("Barkskin", () => Me.HealthFraction < 0.6);

			if (HasSpell("Cat Form") && HasAura("Cat Form"))
			{

				if(HasAura("Prowl")) {
					if (Cast("Savage Roar", () => !Me.HasAura("Savage Roar"))) return;
					if (Cast("Pounce", () => HasAura("Prowl"))) return;
				}

				else {

					if (!Target.IsInCombatRangeAndLoS) {
						if (Cast("Savage Roar", () => !Me.HasAura("Savage Roar"))) return;
						if (CastSelfPreventDouble("Prowl", () => !Me.InCombat && Target.CombatRange <= RotationInfo.DismountRange && !HasAura("Prowl") )) return;
					}

					Cast("Skull Bash", () => Target.IsCastingAndInterruptible());
					if (CastSelf("Nature's Swiftness", () => Me.HealthFraction <= 0.5))
					{
						CastSelf("Healing Touch");
						return;
					}

                    if (Cast("Healing Touch", () => HasAura("Predatory Swiftness"))) return;

					if (CastSelf("Rejuvenation", () => Me.HealthFraction <= 0.5 && !HasAura("Rejuvenation"))) return;
					if (Cast("Faerie Fire", () => !Target.HasAura("Faerie Fire") && !HasAura("Prowl"))) return;

					if (CastSelf("Tiger's Fury", () => !HasAura("Berserk")))
						CastSelf("Berserk", () => Me.HpLessThanOrElite(0.6));

					if (Cast("Typhoon", () => Target.IsInCombatRange && Me.HealthFraction < 0.5)) return;

					if (Cast("Savage Roar", () => Me.GetPower(WoWPowerType.Energy) >= 25 && !HasAura("Savage Roar") && (SpellCost("Savage Roar") == 0 || Me.ComboPoints >= 3))) return;
					if (Cast("Rip", () => Me.GetPower(WoWPowerType.Energy) >= 30 && !Target.HasAura("Rip", true) && Me.ComboPoints >= 3)) return;
					if (Cast("Ferocious Bite", () => Me.GetPower(WoWPowerType.Energy) >= 25 && Me.ComboPoints >= 3)) return;

					if (Cast("Thrash", () => HasAura("Omen of Clarity") && !Target.HasAura("Thrash", true))) return;

					if (Cast("Rake", () => Me.GetPower(WoWPowerType.Energy) >= 35 && !Target.HasAura("Rake", true) && rakeDelay.IsReady)) return;

					if (Adds.Count > 1 && Adds.Count(x => x.DistanceSquared < 8 * 8) > 1)
					{
						if (Cast("Swipe", () => Me.GetPower(WoWPowerType.Energy) >= 45 || HasAura("Omen of Clarity"))) return;
						if (Cast("Mangle", () => Me.GetPower(WoWPowerType.Energy) >= 45 || HasAura("Omen of Clarity"))) return;
					}
					else
					{
						//if (Cast("Shred",     () => Me.GetPower(WoWPowerType.Energy) >= 40 && VectorTools.GetRotationDiff())) return; //if behind
						if (Cast("Mangle", () => Me.GetPower(WoWPowerType.Energy) >= 35 || HasAura("Omen of Clarity"))) return;
					}
				}

			}
			else
			{
				if (CastSelf("Rejuvenation", () => Me.HealthFraction <= 0.75 && !HasAura("Rejuvenation"))) return;
				//if (Cast("Moonfire", () => Target.HealthFraction <= 0.1 && !Target.IsElite())) return;
				if (CastSelf("Cat Form", () => Target.IsInCombatRangeAndLoS || Target.CombatRange <= 25)) return;
			}
		}
	}
}