using System.Linq;
using ReBot.API;

namespace ReBot
{
    [Rotation("Doc's Druid Guardian", "docbrown", WoWClass.Druid, Specialization.DruidGuardian, 5, 25)]
    public class DocDruidGuardian : CombatRotation
    {
        public DocDruidGuardian()
        {
            GroupBuffs = new string[]
			{
				"Mark of the Wild"
			};
            PullSpells = new string[]
            {
                "Enrage"
            };
        }

        public override bool OutOfCombat()
        {
            if (CastSelf("Rejuvenation", () => Me.HealthFraction <= 0.75 && !HasAura("Rejuvenation"))) return true;
            if (CastSelfPreventDouble("Healing Touch", () => Me.HealthFraction <= 0.5)) return true;
            if (CastSelf("Mark of the Wild", () => !HasAura("Mark of the Wild") && !HasAura("Blessing of Kings"))) return true;
            if (CastSelf("Aquatic Form", () => Me.MovementSpeed != 0 && Me.IsSwimming && !HasAura("Aquatic Form"))) return true;
            if (CastSelf("Bear Form", () => Me.MovementSpeed != 0 && !Me.IsSwimming && !HasAura("Bear Form") && !HasAura("Dash"))) return true;
            return false;
        }

        public override void Combat()
        {
            // Bear Form!!
            if (HasAura("Bear Form"))
            {
                // Survival, off GCD
                Cast("Survival Instincts", () => Me.HealthFraction <= 0.2);
                Cast("Might of Ursoc", () => Me.HealthFraction <= 0.3);
                Cast("Frenzied Regeneration", () => Me.HealthFraction <= 0.5);
                Cast("Barkskin", () => Me.HealthFraction <= 0.6);
                Cast("Savage Defense", () => !HasAura("Savage Defense"));

                // Survival, on GCD
                if (CastSelf("Cenarion Ward")) return;

                // Interrupts
                Cast("Skull Bash", () => Target.IsCastingAndInterruptible());

                // Debuffs
                if (Cast("Faerie Fire", () => !Target.HasAura("Weakened Armor"))) return;

                // 3+ Targets, all off GCD
                if (Adds.Count >= 3 && Adds.Count(x => x.DistanceSquared <= 8 * 8) >= 3)
                {
                    Cast("Disorienting Roar");
                    Cast("Mass Entanglement");
                    Cast("Berserk");
                }

                // 2+ Targets
                if (Adds.Count >= 2 && Adds.Count(x => x.DistanceSquared <= 8 * 8) >= 2)
                {
                    if (Cast("Swipe")) return;
                    if (Cast("Thrash")) return;
                }

                // 1+ Targets
                if (Cast("Maul", () => 
                    HasAura("Tooth and Claw")
                    && HasAura("Savage Defense")
                    && Me.GetPower(WoWPowerType.Rage) > 80
                )) return;
                if (Cast("Mangle")) return;
                if (Cast("Lacerate", () => !Target.HasAura("Lacerate", false, 3))) return;
                if (Cast("Thrash", () => !Target.HasAura("Thrash"))) return;

            }
            else
            {
                if (CastSelf("Bear Form", () => !HasAura("Dash") && !Me.InCombat)) return;
            }
        }
    }
}