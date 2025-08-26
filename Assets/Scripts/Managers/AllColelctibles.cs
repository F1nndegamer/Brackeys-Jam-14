using UnityEngine;

public class AllCollectibles : MonoBehaviour
{
    public static AllCollectibles Instance;
    public int totalCollectibles = 0;
    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if(totalCollectibles <= 0)
        {
            Debug.Log("All collectibles gathered!");
            GridRoomGenerator.Instance.InstantiateCollectibles(true);
        }
    }
}
