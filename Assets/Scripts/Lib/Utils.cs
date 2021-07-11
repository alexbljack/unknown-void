using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Lib
{
    public static class Helpers
    {
        public static void DebugDrawRect(Rect rect, Color color)
        {
            var tl = new Vector3(rect.x, rect.y);
            var tr = new Vector3(rect.xMax, rect.y);
            var bl = new Vector3(rect.x, rect.yMax);
            var br = new Vector3(rect.xMax, rect.yMax);
        
            Debug.DrawLine(tl, tr, color);
            Debug.DrawLine(bl, br, color);
            Debug.DrawLine(tl, bl, color);
            Debug.DrawLine(tr, br, color);
        }

        public static void DebugDrawCircle(Vector2 center, float radius, int sectors, Color color)
        {
            var angle = 0f;
            var step = 360f / sectors;
            Vector2 thisPoint = Vector2.zero;
            Vector2 lastPoint = Vector2.zero;
            
            for (var i = 0; i < sectors + 1; i++)
            {
                thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                
                Debug.DrawLine(center + lastPoint, center + thisPoint, color);
                
                lastPoint = thisPoint;
                angle += step;
            }
        }

        public static Rect GetSpriteBoundsRect(SpriteRenderer renderer)
        {
            var bounds = renderer.bounds;
            var center = bounds.center;
            var extents = bounds.extents;
            var position = new Vector2(center.x - extents.x, center.y - extents.y);
            var size = new Vector2(extents.x * 2, extents.y * 2);
            var rect = new Rect(position, size);
            return rect;
        }

        public static bool DiceRoll(int chance)
        {
            Assert.IsTrue(chance <= 100 && chance >= 0, "Chance must be in [0, 100] interval");
            return Random.value <= chance / 100f;
        }
    
        public static int RandomSigh()
        {
            return Random.Range(0f, 1f) > 0.5f ? 1 : -1;
        }
    
        public static bool ClickedOnScreen()
        {
            return Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject();
        }

        public static (Vector3, GameObject) GetClickedTarget(Camera camera)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(ray, out RaycastHit hit, 100f);
            GameObject hitObject = hit.collider != null ? hit.collider.gameObject : null;

            Vector3 target = MouseWorldPosition(camera);
            target.z = 0;

            return (target, hitObject);
        }

        public static (Vector2, GameObject) GetClickedTarget2D(Camera camera)
        {
            Vector3 position = MouseWorldPosition(camera);
            GameObject obj = null;
            foreach (Collider2D collider in Object.FindObjectsOfType<Collider2D>())
            {
                if (collider.OverlapPoint(new Vector2(position.x, position.y)))
                {
                    obj = collider.gameObject;
                }
            }
            return (new Vector2(position.x, position.y), obj);
        }
    
        public static Vector3 MouseWorldPosition(Camera camera)
        {
            Vector3 target = camera.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;
            return target;
        }

        public static List<Vector2Int> Directions()
        {
            return new List<Vector2Int>
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };
        }

        public static Vector2Int RandomDirection()
        {
            var directions = Directions();
            return directions[Random.Range(0, directions.Count)];
        }

        public static IEnumerator WaitForSceneBecomeActive(string sceneName)
        {
            Scene active, target;
            do
            {
                active = SceneManager.GetActiveScene();
                target = SceneManager.GetSceneByName(sceneName);
                yield return null;
            } while (active != target);
        }

        public static IEnumerator WaitAsyncSceneOperation(AsyncOperation operation)
        {
            while (!operation.isDone) { yield return null; }
            yield return null;  // Wait one more frame for unloading scene.
        }
    }
}
