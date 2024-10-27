using UnityEngine;

public class EnemyRevengePoint : MonoBehaviour
{

    [SerializeField]
    GameObject _revengePoint;

    public void DropRevengePoint(int valuePerRevengePoint, int revengePointsQuantity, Transform transform)
    {
        for(int i = 0; i < revengePointsQuantity; i++)
        {
            float posX = Random.Range(-1.5f, 1.5f);
            GameObject revengePoint = Instantiate(_revengePoint, new Vector3(transform.position.x + posX, transform.position.y, transform.position.z), Quaternion.identity);
            revengePoint.GetComponent<RevengePoint>().value = valuePerRevengePoint;
        }
    }

}
