//using UnityEngine;
//using TMPro;

//public class FloatingText : MonoBehaviour
//{
//    [SerializeField] private TextMeshProUGUI textMesh;
//    private ObjectPool objectPool;
//    private Transform canvas;

//    public void Initialize(string message, Vector3 worldPosition, ObjectPool pool, Transform parentCanvas)
//    {
//        objectPool = pool;
//        canvas = parentCanvas;

//        textMesh.text = message;

//        transform.SetParent(canvas);
//        transform.position = Camera.main.WorldToScreenPoint(worldPosition);

//        Invoke(nameof(ReturnToPool), 1.5f);
//    }

//    private void ReturnToPool()
//    {
//        if (objectPool != null)
//            objectPool.ReturnObject(gameObject);
//        else
//            Destroy(gameObject);
//    }
//}
