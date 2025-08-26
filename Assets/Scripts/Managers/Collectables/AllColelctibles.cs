using UnityEngine;

public class AllCollectibles : MonoBehaviour
{
    public static AllCollectibles Instance;
    public int totalCollectibles = 0;
    public bool BiscuitSpawned = false;
    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (totalCollectibles <= 0)
        {
            if (!BiscuitSpawned)
            {
                BiscuitSpawned = true;
                Debug.Log("All collectibles gathered!");
                GridRoomGenerator.Instance.InstantiateCollectibles(true);
                return;
            }
        }
    }
}
