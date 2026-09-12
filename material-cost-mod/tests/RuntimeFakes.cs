// Minimal test doubles for the game's read-only API, not copied game implementations.
// The tests compile the production RuntimeRules.cs against these instead of loading Unity.
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine
{
    public class MonoBehaviour { }
    public static class Object
    {
        public static int SceneScans;
        public static T[] FindObjectsOfType<T>(bool includeInactive)
        {
            SceneScans++;
            return System.Array.Empty<T>();
        }
    }
}

public class TestPrefab
{
    public int Scans;
    public UnityEngine.MonoBehaviour[] Components = System.Array.Empty<UnityEngine.MonoBehaviour>();
    public T[] GetComponentsInChildren<T>(bool includeInactive)
    {
        Scans++;
        return Components.OfType<T>().ToArray();
    }
}

public class ItemSO { public long id; public int basePrice, amount = 1; public TestPrefab prefab; }
public class ProductSO : ItemSO { }
public class SeedSO : ItemSO { public ProductSO productSO; public int harvestAmount = 1; }
public class TreeSO : ItemSO { public ProductSO productSO; }
public class AnimalSO : ItemSO { public ProductSO productSO; public List<ProductSO> meats = new(); }
public class Ingredient { public ItemSO itemSO; public int amount = 1; }
public class RecipeSO { public ItemSO output; public int amount = 1; public List<Ingredient> ingredients = new(); }
public class BlockBeeHive : UnityEngine.MonoBehaviour { public ProductSO output; }
public class BlockFishTrap : UnityEngine.MonoBehaviour { public List<ProductSO> commonFishes = new(); }
public class FishingRod : UnityEngine.MonoBehaviour { public List<ProductSO> commonFishes = new(); }
public class FishingZone { public List<Ingredient> fishes = new(); }
public class TestRecipeMachine : UnityEngine.MonoBehaviour { public List<RecipeSO> recipes = new(); }

public class GameManager
{
    public List<ItemSO> itemDatabase = new();
    public int Day = 1, PriceMultiplier = 1, Lookups, WholesaleCalls, RecommendedCalls;
    public int GetCurrentDayRaw() => Day;
    public ItemSO GetItemById(long id) { Lookups++; return itemDatabase.FirstOrDefault(item => item?.id == id); }
    public int GetWholesalePrice(ProductSO product) { WholesaleCalls++; return product.basePrice * PriceMultiplier; }
    public int GetRecommendedPrice(ProductSO product) { RecommendedCalls++; return product.basePrice * PriceMultiplier * 3; }
}
