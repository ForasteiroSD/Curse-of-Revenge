using UnityEngine;
using Cainos.LucidEditor;

namespace Cainos.PixelArtPlatformer_VillageProps
{
    public class Chest : MonoBehaviour
    {
        public GameObject _revengePoint;
        public int _valuePerRevengePoint;
        public float _revengePointsQuantity;


        [FoldoutGroup("Reference")]
        public Animator animator;

        [FoldoutGroup("Runtime"), ShowInInspector, DisableInEditMode]
        public bool IsOpened
        {
            get { return isOpened; }
            set
            {
                isOpened = value;
                animator.SetBool("IsOpened", isOpened);
                DropRevengePoints();
            }
        }

        private bool isOpened;

        [FoldoutGroup("Runtime"),Button("Open"), HorizontalGroup("Runtime/Button")]
        public void Open()
        {
            IsOpened = true;
        }

        [FoldoutGroup("Runtime"), Button("Close"), HorizontalGroup("Runtime/Button")]
        public void Close()
        {
            IsOpened = false;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            GetComponent<CapsuleCollider2D>().enabled = false;
            IsOpened = true;
        }

        //called by open animation
        void DropRevengePoints()
        {
            for (int i = 0; i < _revengePointsQuantity; i++)
            {
                GameObject revengePoint = Instantiate(_revengePoint, new Vector2(transform.position.x, transform.position.y + 0.7f), Quaternion.identity);
                revengePoint.GetComponent<RevengePoint>().value = _valuePerRevengePoint;
            }
        }
    }
}
