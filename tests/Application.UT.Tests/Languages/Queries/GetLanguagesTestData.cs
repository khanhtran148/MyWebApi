using System.Collections;
using System.Collections.Generic;

namespace Application.UT.Tests.Languages.Queries
{
    public sealed class GetLanguagesTestsGoodData : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new()
        {
            new object[] { new List<string>() { "vi" }, "", true },
            new object[] { new List<string>() {  }, "Po", true },
            new object[] { new List<string>() {  }, "Po", false },
            new object[] { new List<string>() {  }, "", true },
            new object[] { new List<string>() {  }, "", false }
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public sealed class GetLanguagesTestsBadData : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new()
        {
           new object[] { new List<string>() { "un"  }, "", true },
           new object[] { new List<string>() {  }, "unknown", true }
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
