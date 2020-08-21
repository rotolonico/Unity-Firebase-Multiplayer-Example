using System;
using UnityEngine;

namespace Serializables
{
    [Serializable]
    public class Move
    {
        public Vector2 direction;

        public Move(Vector2 direction) => this.direction = direction;
    }
}