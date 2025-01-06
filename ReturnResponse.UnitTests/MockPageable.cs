using Azure;
using Azure.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MockPageable<T> : Pageable<T>, IEnumerable<T>
{

    private readonly IEnumerable<T> _items;

    public MockPageable(IEnumerable<T> items)
    {
        _items = items;
    }

    public override IEnumerable<Page<T>> AsPages(string continuationToken = null, int? pageSizeHint = null)
    {
        yield return Page<T>.FromValues(_items.ToList(), null, new MockResponse(200, "OK"));
    }

    public override IEnumerator<T> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    private class MockResponse : Response
    {
        private readonly int _status;
        private readonly string _reasonPhrase;

        public MockResponse(int status, string reasonPhrase)
        {
            _status = status;
            _reasonPhrase = reasonPhrase;
        }

        public override int Status => _status;

        public override string ReasonPhrase => _reasonPhrase;

        public override Stream ContentStream { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override string ClientRequestId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        protected override bool TryGetHeader(string name, out string value) { value = null; return false; }

        protected override bool TryGetHeaderValues(string name, out IEnumerable<string> values) { values = null; return false; }

        protected override bool ContainsHeader(string name) => false;

        protected override IEnumerable<HttpHeader> EnumerateHeaders() => Enumerable.Empty<HttpHeader>();

        public override void Dispose() { }
    }

}
