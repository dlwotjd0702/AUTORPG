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
    
    public interface ISaveable
    {
        void ApplyLoadedData(SaveData data);
        void CollectSaveData(SaveData data);
    }

}