using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartTools.XML {
  public abstract class XMLData {

    public XElement data { get; protected set; }

    public XMLData(string FileName) {
      this.data = XElement.Load(Prime.PATH_XML + FileName);
    } // XMLData

    public virtual string[] GetItems(string ItemName, string attribute = null) {
      IEnumerable<XElement> elements;
      List<string> tmp = new List<string>();
      if (attribute == null) {
        elements = from el in this.data.Elements()
                   select el;
        foreach (XElement el in elements)
          tmp.Add(el.Element(ItemName).Value);
      } else {
        foreach (XElement el in data.Elements()) {
          if (el.Attribute("standard").Value.ToString().Equals(attribute)) {
            XNode node = el.FirstNode;
            while (node != null) {
              tmp.Add(((XElement)node).Value.ToString());
              node = node.NextNode;
            }
          }
        }
      }
      return tmp.ToArray();
    } // GetItems

    // public string[] GetMaterials() { return this.GetItems("code"); }
    // public string[] GetCertificates() { return this.GetItems("code"); }
    // public string[] GetTests() { return this.GetItems("code"); }
    // public string[] GetStandards() { return this.GetItems("code"); }
    // public string[] GetQualities(string attribute) { return this.GetItems("code", attribute); }

} // XMLData
}
