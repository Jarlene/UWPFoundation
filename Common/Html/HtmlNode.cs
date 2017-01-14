using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Common.Html
{
    public abstract class HtmlNode
    {
        public HtmlNode Parent { get; set; }
        public HtmlNode PrevNode { get; set; }
        public HtmlNode NextNode { get; set; }
    }


    public class HtmlText : HtmlNode
    {
        private readonly string _text;

        public HtmlText(string text)
        {
            _text = text ?? string.Empty;
        }

        public override string ToString()
        {
            return _text;
        }
    }

    public class HtmlComment : HtmlText
    {
        public HtmlComment(string comment) : base(comment) { }

        public override string ToString()
        {
            return string.Format("<!--{0}-->", base.ToString());
        }
    }
    public class HtmlInstruction : HtmlText
    {
        public HtmlInstruction(string str) : base(str) { }

        public override string ToString()
        {
            return string.Format("<!{0}>", base.ToString());
        }
    }
    public class HtmlDeclaration : HtmlText
    {
        public HtmlDeclaration(string str) : base(str) { }

        public override string ToString()
        {
            return string.Format("<?{0}>", base.ToString());
        }
    }

    public class HtmlElement : HtmlNode
    {
        private readonly Dictionary<string, HtmlAttribute> _attributes;

        public string Name { get; private set; }
        public bool IsClosed { get; private set; }

        public ReadOnlyCollection<HtmlAttribute> Attributes { get { return new ReadOnlyCollection<HtmlAttribute>(_attributes.Values.ToArray()); } }
        public List<HtmlNode> ChildNodes { get; private set; }

        private HtmlElement()
        {
            _attributes = new Dictionary<string, HtmlAttribute>();
            ChildNodes = new List<HtmlNode>();
        }

        public HtmlElement(string name, bool isClosed)
            : this()
        {
            Name = name.ToLower();
            IsClosed = isClosed;
        }

        public void Add(string name, string value)
        {
            var item = new HtmlAttribute { Name = name.ToLower(), Value = value };
            this._attributes[name.ToLower()] = item;
        }

        public void Add(HtmlNode item)
        {
            item.Parent = this;
            if (this.ChildNodes.Count > 0)
            {
                item.PrevNode = this.ChildNodes[this.ChildNodes.Count - 1];
                this.ChildNodes[this.ChildNodes.Count - 1].NextNode = item;
            }

            this.ChildNodes.Add(item);
        }

        internal void RemoveAll(IEnumerable<HtmlAttribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                if (this._attributes.ContainsKey(attribute.Name.ToLower()))
                    this._attributes.Remove(attribute.Name.ToLower());
            }
        }

        public override string ToString()
        {
            var buf = new StringBuilder("<" + Name);
            foreach (var node in Attributes)
            {
                buf.Append(" " + node.ToString());
            }
            if (ChildNodes.Count == 0)
            {
                buf.Append("/>");
                return buf.ToString();
            }
            buf.Append(">");

            foreach (var node in ChildNodes)
            {
                buf.Append(node.ToString());
            }

            buf.AppendFormat("</{0}>", Name);
            return buf.ToString();
        }


    }

    public class HtmlDocument : HtmlElement
    {
        public HtmlDocument()
            : base("html", false)
        {
        }

        public override string ToString()
        {
            if (ChildNodes.Count == 0)
                return string.Empty;

            var buf = new StringBuilder();
            foreach (var node in ChildNodes)
            {
                buf.Append(node.ToString());
            }
            return buf.ToString();
        }
    }

    public class HtmlAttribute
    {
        public string Name { get; internal set; }
        public string Value { get; internal set; }

        public override string ToString()
        {
            return string.Format("{0}=\"{1}\"", Name, Value);
        }
    }
}
