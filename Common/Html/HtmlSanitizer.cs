using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
namespace Common.Html
{
    public static class HtmlSanitizer
    {
        private static readonly Collection<string> Unsafe;

        #region - Ctor -
        static HtmlSanitizer()
        {
            var list = new[]
            {
                "onclick", "ondblclick", "onmousedown", "onmouseup", "onmouseover", "onmousemove",
                "onmouseout", "onkeypress", "onkeydown", "onkeyup", "script", "applet", "embed", "frameset",
                "iframe", "frame", "object", "ilayer", "layer"
            };
            Unsafe = new Collection<string>();

            foreach (var item in list)
            {
                Unsafe.Add(item);
            }

        }

        #endregion

        public static string Sanitize(string html)
        {
            var doc = HtmlParser.Parse(html);
            Sanitize(doc);

            if (doc.ChildNodes.Count == 0)
                return null;

            var result = doc.ToString();
            return result;
        }

        public static void Sanitize(HtmlElement doc)
        {
            doc.RemoveUnSafe();
        }

        private static void RemoveUnSafe(this HtmlElement element)
        {
            var attributesToRemove = (from attribute in element.Attributes where attribute.IsUnSafe() select attribute);
            element.RemoveAll(attributesToRemove);

            if (element.Name == "input")
            {
                foreach (var attribute in element.Attributes)
                {
                    if (attribute.Name == "type" && attribute.Value.ToLower() == "submit")
                        attribute.Value = "button";
                }
            }

            var nodesToRemove = new List<int>();
            for (var i = 0; i < element.ChildNodes.Count; i++)
            {
                var node = element.ChildNodes[i];
                if (node is HtmlInstruction || node is HtmlComment || node.IsUnSafe())
                    nodesToRemove.Add(i);
            }
            if (nodesToRemove.Count > 0)
            {
                var index = nodesToRemove.Count - 1;
                while (index > -1)
                {
                    element.ChildNodes.RemoveAt(nodesToRemove[index]);
                    index--;
                }
            }

            foreach (var node in element.ChildNodes)
            {
                if (!(node is HtmlElement))
                    continue;
                (node as HtmlElement).RemoveUnSafe();
            }
        }

        private static bool IsUnSafe(this HtmlAttribute attribute)
        {
            if (Unsafe.Contains(attribute.Name))
                return true;
            return false;
        }

        private static bool IsUnSafe(this HtmlNode node)
        {
            if (!(node is HtmlElement))
                return false;

            return Unsafe.Contains((node as HtmlElement).Name);
        }
    }
}
