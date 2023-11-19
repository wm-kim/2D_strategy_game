using System.Collections.Generic;

namespace Utilities
{
    public static class CollectionEx
    {
        /// <summary>
        /// 인덱스가 컬렉션의 범위 내에 있는지 확인합니다.
        /// </summary>
        /// <typeparam name="T">컬렉션의 요소 유형입니다.</typeparam>
        /// <param name="collection">확인할 컬렉션입니다.</param>
        /// <param name="index">확인할 인덱스입니다.</param>
        /// <returns>인덱스가 컬렉션의 범위 내에 있으면 true를 반환하고, 그렇지 않으면 false를 반환합니다.</returns>
        public static bool IsIndexWithinRange<T>(this ICollection<T> collection, int index)
        {
            return index >= 0 && index < collection.Count;
        }

        public static void CheckIndexWithinRange<T>(this ICollection<T> collection, int index)
        {
            if (!IsIndexWithinRange(collection, index))
                throw new System.IndexOutOfRangeException($"Index {index} is out of range of collection {collection}");
        }
    }
}