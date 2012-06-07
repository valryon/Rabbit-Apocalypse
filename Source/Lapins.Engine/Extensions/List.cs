using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lapins.Engine.Core;

namespace Lapins
{
    public static class ListExtension
    {
        public static T GetRandomElement<T>(this List<T> list)
        {
            return list[Application.Random.GetRandomInt(list.Count)];
        }
    }
}
