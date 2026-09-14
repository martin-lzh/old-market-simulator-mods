global using Object = UnityEngine.Object;
using OldMarket.StackAll;
using TMPro;
using Unity.Netcode;
using UnityEngine;

// Exercise the real display update with state-only stand-ins; no Unity methods run.
internal static class ContainerViewChecks
{
    internal static void Run(Action<bool, string> check)
    {
        var inventory = new PlayerInventory { maxSlots = 8 };
        var slot = new ItemSlot();
        slot.transform.parent = inventory.itemSlots.transform;
        var native = slot.textAmount;
        native.transform.parent = slot.transform;
        native.fontSize = 22;
        native.fontSizeMin = 18;
        native.fontSizeMax = 72;
        native.alignment = TextAlignmentOptions.BottomRight;
        native.rectTransform.anchorMin = new Vector2(.1f, .2f);
        native.rectTransform.anchorMax = new Vector2(.9f, .8f);
        NetworkManager.Singleton = new NetworkManager
        {
            LocalClient = new NetworkClient { PlayerObject = new GameObject { Component = inventory } }
        };
        GameManager.Instance = new GameManager();

        void Update(ItemSO item, int count, int contents)
        {
            GameManager.Instance.Item = item;
            inventory.slots = Enumerable.Range(0, ContainerPlan.StorageSize(8))
                .Select(_ => ContainerPlan.Empty()).ToList();
            for (int p = 0; p < count; p++)
                inventory.slots[ContainerPlan.Index(8, 0, p)] = new InventorySlot { itemId = 1, amount = contents };
            // ItemSlot.UpdateSlot resets visibility and text before the postfix.
            native.gameObject.SetActive(count > 0);
            native.text = contents.ToString();
            ContainerView.Update(slot, inventory.slots[0], 0);
        }
        TextMeshProUGUI Right() => UnityEngine.Object.LastClone;
        var fish = new ProductSO { amount = 1, destroyWhenEmpty = true };
        foreach (int count in new[] { 1, 9, 10, 64 })
        {
            Update(fish, count, 1);
            check(!native.gameObject.activeSelf && Right().gameObject.activeSelf && Right().text == count.ToString(),
                $"{count} whole fish have exactly one right-side quantity");
            check(Right().fontSize == 22 && !Right().enableAutoSizing &&
                Right().rectTransform.anchorMin.x == 0 && Right().rectTransform.anchorMax.x == 1,
                $"{count} fish retain native 22-point size and full slot width");
        }
        Update(new ProductSO { amount = 24 }, 12, 24);
        check(native.gameObject.activeSelf && native.text == "288" && Right().text == "12",
            "fish-to-basket transition restores goods and container labels");
        check(native.enableAutoSizing && native.rectTransform.anchorMax.x == .5f &&
            Right().fontSize == 22 && !Right().enableAutoSizing,
            "three-digit goods may fit the left half without shrinking the container count");
        Update(new ProductSO { amount = 85 }, 64, 85);
        check(native.text == "5440" && native.enableAutoSizing && native.rectTransform.anchorMax.x == .5f &&
            Right().text == "64" && Right().fontSize == 22 && !Right().enableAutoSizing,
            "maximum goods total reserves space for a native-size two-digit container count");
        Update(new ProductSO { amount = 24 }, 12, 0);
        check(native.gameObject.activeSelf && native.text == "0" && Right().text == "12", "empty baskets retain both quantities");
        Update(new ProductSO { amount = 1, destroyWhenEmpty = false }, 2, 0);
        check(native.gameObject.activeSelf && native.text == "0" && Right().text == "2", "one-unit reusable containers retain content quantity");
        Update(new ProductSO { amount = 6, destroyWhenEmpty = true }, 10, 6);
        check(native.gameObject.activeSelf && native.text == "60" && Right().text == "10", "boxed fish cuts retain both quantities");
        check(native.fontSize == 22 && Right().fontSize == 22 && !native.enableAutoSizing && !Right().enableAutoSizing,
            "both two-digit labels return to native size after a large goods total");
        Update(new SeedSO { amount = 10 }, 3, 4);
        check(native.gameObject.activeSelf && native.text == "12" && Right().text == "3", "seed contents and packet count are distinct");
        Update(fish, 10, 1);
        Update(new ItemSO(), 1, 10);
        check(native.gameObject.activeSelf && native.text == "10" && !Right().gameObject.activeSelf,
            "fish-to-non-product transition restores native quantity");
        check(native.fontSize == 22 && native.fontSizeMin == 18 && native.fontSizeMax == 72 &&
            !native.enableAutoSizing && native.rectTransform.anchorMin.x == .1f && native.rectTransform.anchorMax.x == .9f,
            "non-product transition restores native formatting");
        Update(fish, 10, 1);
        Update(null, 0, 0);
        check(!native.gameObject.activeSelf && !Right().gameObject.activeSelf, "empty slot hides both quantities");
        ContainerView.Clear();
    }
}

public class ItemSO { public int amount; public int stackSize = 1; }
public class SeedSO : ItemSO { }
public class ToolSO : ItemSO { }
public class ProductSO : ItemSO { public bool destroyWhenEmpty; public int maxDays = -1; }
public class ItemSlot
{
    public Transform transform = new Transform();
    public TextMeshProUGUI textAmount = new TextMeshProUGUI();
}
public class PlayerInventory
{
    public int maxSlots;
    public GameObject itemSlots = new GameObject();
    public List<InventorySlot> slots = new List<InventorySlot>();
}
public class GameManager
{
    public static GameManager Instance;
    public ItemSO Item;
    public ItemSO GetItemById(long id) => Item;
}

namespace Unity.Netcode
{
    public class NetworkManager { public static NetworkManager Singleton; public NetworkClient LocalClient; }
    public class NetworkClient { public GameObject PlayerObject; }
}
namespace UnityEngine
{
    public class Object
    {
        public static TextMeshProUGUI LastClone;
        public static TextMeshProUGUI Instantiate(TextMeshProUGUI source, Transform parent)
        {
            LastClone = new TextMeshProUGUI();
            LastClone.transform.parent = parent;
            return LastClone;
        }
        public static void Destroy(GameObject value) { value.SetActive(false); }
    }
    public class Transform
    {
        public Transform parent;
        public bool IsChildOf(Transform ancestor) => this == ancestor || (parent?.IsChildOf(ancestor) ?? false);
    }
    public class GameObject
    {
        public Transform transform = new Transform();
        public object Component;
        public bool activeSelf = true;
        public void SetActive(bool active) { activeSelf = active; }
        public T GetComponent<T>() where T : class => Component as T;
    }
    public struct Vector2
    {
        public float x, y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
    }
    public class RectTransform : Transform { public Vector2 anchorMin, anchorMax, offsetMin, offsetMax, pivot; }
    public static class Mathf
    {
        public static float Max(float a, float b) => Math.Max(a, b);
        public static float Min(float a, float b) => Math.Min(a, b);
    }
}
namespace TMPro
{
    public enum TextAlignmentOptions { BottomLeft, BottomRight }
    public class TextMeshProUGUI
    {
        public string name, text;
        public object font, fontSharedMaterial;
        public bool raycastTarget, enableAutoSizing;
        public float fontSize, fontSizeMin, fontSizeMax;
        public TextAlignmentOptions alignment;
        public GameObject gameObject = new GameObject();
        public RectTransform rectTransform = new RectTransform();
        public Transform transform => rectTransform;
    }
}
