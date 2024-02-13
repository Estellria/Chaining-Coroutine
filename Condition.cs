using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace StellaFox
{
    public class Condition //풀링할 수 있게 만들기
    {
        private int _count;


        public bool To(int value, int target, int increase = 1, bool repeat = false)
        {
            return CalculateToTarget(value, target, increase, repeat) < target;
        }

        public bool To(int value, int target, int increase, ref int copyValue, bool repeat = false)
        {
            copyValue = CalculateToTarget(value, target, increase, repeat);
            return copyValue < target;
        }

        private int CalculateToTarget(int value, int target, int increase, bool repeat)
        {
            _count = repeat && (value + _count * increase > target) ? 0 : _count;
            return value + _count++ * increase;
        }
    }
}
