﻿using UnityEngine;

namespace Svelto.ECS.Example.Survive.OOPLayer
{
    public struct VFXEvent
    {
        internal bool play;
        internal Vector3 position;

        public VFXEvent(Vector3 position)
        {
            this.position = position;
            play = true;
        }
    }
}