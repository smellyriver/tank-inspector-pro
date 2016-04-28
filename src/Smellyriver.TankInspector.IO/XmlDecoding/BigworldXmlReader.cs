using System.IO;
using System.Xml;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.IO.XmlDecoding
{
    public class BigworldXmlReader : XmlReader
    {
        public override int AttributeCount
        {
            get { return _reader.AttributeCount; }
        }

        public override string BaseURI
        {
            get { return _reader.BaseURI; }
        }

        public override void Close()
        {
            _reader.Close();
        }

        public override int Depth
        {
            get { return _reader.Depth; }
        }

        public override bool EOF
        {
            get { return _reader.EOF; }
        }

        public override string GetAttribute(int i)
        {
            return _reader.GetAttribute(i);
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            return _reader.GetAttribute(name, namespaceURI);
        }

        public override string GetAttribute(string name)
        {
            return _reader.GetAttribute(name);
        }

        public override bool IsEmptyElement
        {
            get { return _reader.IsEmptyElement; }
        }

        public override string LocalName
        {
            get { return _reader.LocalName; }
        }

        public override string LookupNamespace(string prefix)
        {
            return _reader.LookupNamespace(prefix);
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return _reader.MoveToAttribute(name, ns);
        }

        public override bool MoveToAttribute(string name)
        {
            return _reader.MoveToAttribute(name);
        }

        public override bool MoveToElement()
        {
            return _reader.MoveToElement();
        }

        public override bool MoveToFirstAttribute()
        {
            return _reader.MoveToFirstAttribute();
        }

        public override bool MoveToNextAttribute()
        {
            return _reader.MoveToNextAttribute();
        }

        public override XmlNameTable NameTable
        {
            get { return _reader.NameTable; }
        }

        public override string NamespaceURI
        {
            get { return _reader.NamespaceURI; }
        }

        public override XmlNodeType NodeType
        {
            get { return _reader.NodeType; }
        }

        public override string Prefix
        {
            get { return _reader.Prefix; }
        }

        public override bool Read()
        {
            return _reader.Read();
        }

        public override bool ReadAttributeValue()
        {
            return _reader.ReadAttributeValue();
        }

        public override ReadState ReadState
        {
            get { return _reader.ReadState; }
        }

        public override void ResolveEntity()
        {
            _reader.ResolveEntity();
        }

        public override string Value
        {
            get { return _reader.Value; }
        }

        private Stream _contentStream;
        private XmlTextReader _reader;

        private readonly EncodeType _encodeType;
        public EncodeType EncodeType
        {
            get { return _encodeType; }
        }

        public BigworldXmlReader(string path)
        {
            var content = XmlDecoder.Decode(path, out _encodeType);
            _contentStream = content.ToStream();
            _reader = new XmlTextReader(_contentStream);
            _reader.WhitespaceHandling = WhitespaceHandling.None;
        }

        public BigworldXmlReader(Stream contentStream)
        {
            var content = XmlDecoder.Decode(contentStream);
            _contentStream = content.ToStream();
            _reader = new XmlTextReader(_contentStream);
            _reader.WhitespaceHandling = WhitespaceHandling.None;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _contentStream.Dispose();
            }
        }

    }
}
