namespace Combat
{
    public interface IDamageable
    {
        void TakeDamage(float amount);
    }
    
    public interface IAttackStat
    {
        float GetAttackPower();
    }
}