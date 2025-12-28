using UnityEngine;

namespace PrecursorBaseMeshFix
{
    internal class MeshManager : MonoBehaviour
    {
        private const float yHighThreshold = -90f;
        private const float yLowThreshold = -124f;

        private Vector2 corner1 = new Vector2(493.27f, 1237.84f);
        private Vector2 corner2 = new Vector2(464.7f, 1161.6f);
        private Vector2 corner3 = new Vector2(430.7f, 1174.0f);
        private Vector2 corner4 = new Vector2(459.27f, 1250.24f);

        private void Update()
        {
            Transform exteriorMesh = transform.Find("precursor_base/Instances/precursor_base_15/precursor_base_15_LOD3");
            if (exteriorMesh == null) return;
            Vector3 pPos = Player.main.transform.position; // use maincamera position instead?
            Vector2 pFlatPosition = new Vector2(pPos.x, pPos.z);
            if (PointInQuad(pFlatPosition, corner1, corner2, corner3, corner4) && pPos.y > yLowThreshold && pPos.y < yHighThreshold)
            {
                exteriorMesh.gameObject.SetActive(false);
            }
            else
            {
                exteriorMesh.gameObject.SetActive(true);
            }
        }

        bool PointInQuad(Vector2 p, Vector2 c1, Vector2 c2, Vector2 c3, Vector2 c4)
        {
            return PointInTriangle(p, c1, c2, c3) || PointInTriangle(p, c1, c3, c4);
        }

        bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float Sign(Vector2 v1, Vector2 v2, Vector2 v3)
            {
                return (v1.x - v3.x) * (v2.y - v3.y) - (v2.x - v3.x) * (v1.y - v3.y);
            }

            bool b1 = Sign(p, a, b) < 0.0f;
            bool b2 = Sign(p, b, c) < 0.0f;
            bool b3 = Sign(p, c, a) < 0.0f;

            return (b1 == b2) && (b2 == b3);
        }
    }
}
