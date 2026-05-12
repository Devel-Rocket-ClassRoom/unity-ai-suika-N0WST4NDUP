using UnityEngine;

namespace Watermelon.Gameplay
{
    [RequireComponent(typeof(LineRenderer))]
    public class GameOverLine : MonoBehaviour
    {
        [SerializeField] private float boardHalfWidth = 2.4f;

        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            SetupLine();
        }

        private void SetupLine()
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.SetPosition(0, new Vector3(-boardHalfWidth, transform.position.y, 0f));
            _lineRenderer.SetPosition(1, new Vector3(boardHalfWidth, transform.position.y, 0f));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
            Gizmos.DrawLine(
                new Vector3(-boardHalfWidth, transform.position.y, 0f),
                new Vector3(boardHalfWidth, transform.position.y, 0f)
            );
        }
    }
}
