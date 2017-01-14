using Common.Html;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public static class HtmlUtils
    {

        public static HtmlElement FindNode(this HtmlElement element, string name)
        {
            foreach (var node in element.ChildNodes)
            {
                var el = node as HtmlElement;
                if (el != null && el.Name == name)
                    return el;
            }
            return null;
        }


        public static List<HtmlElement> FindNodes(this HtmlElement element, string name)
        {
            var result = new List<HtmlElement>();
            foreach (var node in element.ChildNodes)
            {
                var sub = node as HtmlElement;
                if (sub == null)
                {
                    continue;
                }
                if ( sub.ChildNodes.Count <= 0)
                {
                    if (sub.Name == name)
                    {
                        result.Add(sub);
                    }
                } 
                else
                {
                    FindNodes(sub, name);
                }
            }
            return result;
        }

        public static IEnumerable<HtmlElement> FindDescendants(this HtmlElement element, string name)
        {
            foreach (var node in element.ChildNodes)
            {
                var el = node as HtmlElement;
                if (el == null)
                    continue;

                if (el.Name == name)
                    yield return el;

                foreach (var child in el.FindDescendants(name))
                {
                    yield return child;
                }
            }
        }

        public static HtmlElement FindAncestor(this HtmlNode element, string name)
        {
            if (element == null || element.Parent == null || !(element.Parent is HtmlElement))
                return null;

            var parent = (element.Parent as HtmlElement);
            if (parent.Name == name)
                return parent;
            return parent.FindAncestor(name);
        }

        public static string GetAttributeValue(this HtmlElement element, string name)
        {
            if (element == null || element.Attributes == null)
                return null;
            foreach (var attribute in element.Attributes)
            {
                if (attribute.Name == name)
                    return attribute.Value;
            }
            return null;
        }

        public static string GetStyleValue(this HtmlElement element, string name, string attribute = null)
        {
            var style = element.GetAttributeValue("style");
            if (string.IsNullOrEmpty(style) && !string.IsNullOrEmpty(attribute))
            {
                var value = element.GetAttributeValue(attribute);
                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            if (string.IsNullOrEmpty(style))
                return null;
            if (style.StartsWith(name))
                return style.SubstringAfter(name + ":").SubstringBefore(";").Trim();

            var result = style.SubstringAfter(";" + name + ":").SubstringBefore(";").Trim();
            return result;
        }

        private static string SubstringBefore(this string str, string separator)
        {
            if (string.IsNullOrEmpty(str) || separator == null)
                return str;
            if (separator == string.Empty)
                return string.Empty;

            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            int index = compareInfo.IndexOf(str, separator, CompareOptions.Ordinal);

            if (index < 0)
            {
                //No such substring
                return str;
            }
            return str.Substring(0, index);
        }

        private static string SubstringAfter(this string str, string separator)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (separator == null)
                return string.Empty;

            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            int index = compareInfo.IndexOf(str, separator, CompareOptions.Ordinal);
            if (index < 0)
            {
                //No such substring
                return string.Empty;
            }
            return str.Substring(index + separator.Length);
        }
        


    }
}
