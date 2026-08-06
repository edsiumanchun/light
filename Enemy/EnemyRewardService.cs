public static class EnemyRewardService
{
    public static void Grant(int assetIndex)
    {
        if (assetIndex < 1000) return;
        int[] rewards = {
            assetIndex / 1000 - 1,
            (assetIndex / 100) % 10 - 1,
            (assetIndex / 10) % 10 - 1,
            assetIndex % 10
        };
        foreach (int reward in rewards) GrantGem(reward);
    }

    private static void GrantGem(int index)
    {
        Cache cache;
        switch (index)
        {
            case 0: cache = Cache.Greed; break;
            case 1: cache = Cache.Hatred; break;
            case 2: cache = Cache.Delusion; break;
            case 3: cache = Cache.Ethics; break;
            case 4: cache = Cache.Concentration; break;
            case 5: cache = Cache.Delusion; break;
            default: return;
        }
        CacheManager.ChangeCache(cache.ToString(), 1, 99999);
    }
}
