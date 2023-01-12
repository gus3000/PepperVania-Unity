using System;
using DefaultNamespace.Model;
using UnityEngine;

namespace Attribute
{
    public class PopulateMeshAnimationAttribute: EnumDataAttribute
    {
        public PopulateMeshAnimationAttribute() : base(typeof(AnimationPhase))
        {
        }
    }
}