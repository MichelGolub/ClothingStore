using System;
using System.Collections.Generic;

namespace WebApi.Filters
{
    public class PropertyFilter
    {
        public int[] NominalValuesId { get; set; } = Array.Empty<int>();
        public IEnumerable<NumericPropertyRange> NumericPropertyRanges { get; set; } = new List<NumericPropertyRange>();
        public class NumericPropertyRange
        { 
            public int Id { get; set; }
            public float Min { get; set; }
            public float Max { get; set; }
        }
    }
}
