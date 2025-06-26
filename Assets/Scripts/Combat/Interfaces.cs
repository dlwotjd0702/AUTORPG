namespace Combat
{
    public interface IDamageable
    {
        void TakeDamage(float amount, bool isCritical = false);
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