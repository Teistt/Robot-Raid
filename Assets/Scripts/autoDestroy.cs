/*
 *   Copyright (c) 2021 MARY Corp
 *   All rights reserved.
 */

using UnityEngine;

public class autoDestroy : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1.5f;

    void Awake()
    {
        Destroy(gameObject, lifeTime);
    }
}
