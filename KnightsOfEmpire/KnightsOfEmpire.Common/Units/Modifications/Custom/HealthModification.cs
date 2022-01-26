namespace KnightsOfEmpire.Common.Units.Modifications.Custom
{
    public class HealthModification : UnitUpgrade
    {
        public HealthModification()
        {
            this.Name = "Light Armor";
            this.Description = "+10 Health, +20 Cost";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.MaxHealth += 10;
            u.Stats.Health += 10;
            u.Stats.TrainCost += 20;
        }
    }

    public class HealthModification2 : UnitUpgrade
    {
        public HealthModification2()
        {
            this.Name = "Heavy Armor";
            this.Description = "Health: +50, Cost: +40, -15% Movement Speed";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.MaxHealth += 50;
            u.Stats.Health += 50;
            u.Stats.TrainCost += 40;
            u.Stats.MovementSpeed -= new UnitStats().MovementSpeed*0.15f;
        }
    }
}
