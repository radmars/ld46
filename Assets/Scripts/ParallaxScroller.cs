﻿using System.Linq;
using UnityEngine;

public class ParallaxScroller : MonoBehaviour {
    private float layerRatio = .05f;
    public Sprite[] sprites;
    private SpriteRenderer[] children;

    private new Camera camera;

    private float yOffset = 4;

    void Start() {
        camera = GetComponent<Camera>();
        var index = 0;
        children = sprites.Select(sprite => {
            var renderer = new GameObject(sprite.name + " renderer")
                .AddComponent<SpriteRenderer>();
            var y = index == sprites.Length - 1 ? 0 : index * yOffset;
            renderer.transform.position = new Vector3(0, y, 10 + index++);
            renderer.sprite = sprite;
            renderer.sortingLayerID = SortingLayer.NameToID("BG");
            return renderer;
        }).ToArray();
    }

    void MoveCamera(object position) {
        camera.transform.position = (Vector3) position;
        var count = children.Length;
        var index = 0;
        foreach (var sprite in children) {
            var y = Mathf.Max((camera.transform.position.y * ((count - index - 1)) * layerRatio), 0);
            sprite.transform.position = new Vector3(
                camera.transform.position.x,
                camera.transform.position.y - y + (index == sprites.Length - 1 ? 0 : index * yOffset),
                sprite.transform.position.z
            );
            index++;
        }
    }
}