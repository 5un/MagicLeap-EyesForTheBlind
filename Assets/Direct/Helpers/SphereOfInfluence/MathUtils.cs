// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicKit
{

    ///<summary>
    /// Contains generic math utilities
    ///</summary>
    public static class MathUtils
    {

        //----------- Public Methods -----------

        public static float StandardDeviation(this IEnumerable<float> values)
        {
            float avg = values.Average();
            return (float)Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }
    }
}