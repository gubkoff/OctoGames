using System;
using UnityEngine;

namespace Samples
{
    public class Character: MonoBehaviour
    {
        public float Value { get; private set; }

        private void Awake()
        {
            Value = 1;
        }
    }
}