using System;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Radmars {
    [Serializable]
    [CreateAssetMenu(fileName = "RAD TILE", menuName = "RAD TILE")]
    public class AnimatedTile : TileBase {
        public Sprite[] sprites;
        public Sprite current;
        public float speed = 4;

        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData) {
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;
            tileData.colliderType = Tile.ColliderType.Sprite;
            if(current != null) {
                tileData.sprite = current;
            }
            else {
                tileData.sprite = sprites[0];
            }
        }

/*
        public override bool GetTileAnimationData(Vector3Int location, ITilemap tileMap, ref TileAnimationData tileAnimationData) {
            tileAnimationData.animatedSprites = sprites;
            tileAnimationData.animationSpeed = speed;
            tileAnimationData.animationStartTime = 0;
            return true;
        }
*/
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AnimatedTile))]
    public class AnimatedTileEditor : Editor {
        private AnimatedTile tile { get { return (target as AnimatedTile); } }

        public override void OnInspectorGUI() {
            EditorGUI.BeginChangeCheck();
            float speed = EditorGUILayout.FloatField("Animation Speed", tile.speed);
            if (speed < 0.0f)
                speed = 0.0f;

            tile.speed = speed;

            int count = EditorGUILayout.DelayedIntField("Number of frames", tile.sprites != null ? tile.sprites.Length : 0);
            if (count < 0) {
                count = 0;
            }

            if (tile.sprites == null || tile.sprites.Length != count) {
                Array.Resize<Sprite>(ref tile.sprites, count);
            }

            if (count == 0) {
                return;
            }

            EditorGUILayout.LabelField("Place sprites shown based on the order of animation.");
            EditorGUILayout.Space();

            for (int i = 0; i < count; i++) {
                tile.sprites[i] = (Sprite) EditorGUILayout.ObjectField("Sprite " + (i + 1), tile.sprites[i], typeof(Sprite), false, null);
            }

            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(tile);
            }
        }
    }
#endif
}