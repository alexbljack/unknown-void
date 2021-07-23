using UnityEngine;

public class SquadInfo : MonoBehaviour
{
    [SerializeField] GameObject unitInfoPrefab;
    
    void Start()
    {
        foreach (UnitInfo obj in transform.GetComponentsInChildren<UnitInfo>())
        {
            Destroy(obj.gameObject);
        }
        
        foreach (Marine player in LevelManager.Instance.players)
        {
            GameObject obj = Instantiate(unitInfoPrefab, transform);
            obj.GetComponent<UnitInfo>().Init(player);
        }
    }
}
