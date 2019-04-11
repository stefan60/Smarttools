using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SmartTools.XML {

  public class XMLTubes: XMLData {

    public XMLTubes(): base("tubes.xml") {
    } // XMLTubes

    /// <summary>
    /// Връща допустимите диаметри за избрания стандарт
    /// </summary>
    /// <param name="standard">наименование на стандарта</param>
    /// <returns></returns>
    public virtual string[] GetDiameters(string standard) {
      List<string> tmp = new List<string>();
      IEnumerable<XElement> xmlStandard = this.data.Elements(standard).Elements("item");
      foreach (XElement item in xmlStandard) {
        XNode node = item.FirstNode;
        while (node != null) {
          if (((XElement)node).Name.ToString().ToLower().Equals("diameter")) {
            tmp.Add(Convert.ToDouble(((XElement)node).Value).ToString("0.00mm"));
          }
          node = node.NextNode;
        }
      }
      return tmp.ToArray();
    } // GetDiameters

    /// <summary>
    /// Връща допустимите дебелини за съответно избран стандарт и диаметър на тръба
    /// </summary>
    /// <param name="standard">наименование на стандарт</param>
    /// <param name="diameter">стойност на диаметъра</param>
    /// <returns></returns>
    public virtual string[] GetThickness(string standard, string diameter) {
      List<string> tmp = new List<string>();
      IEnumerable<XElement> xmlStandard = this.data.Elements(standard).Elements("item");
      string xmlDiameter;
      foreach (XElement item in xmlStandard) {
        XNode node = item.FirstNode;
        while (node != null) {
          xmlDiameter = null;
          if (((XElement)node).Name.ToString().ToLower().Equals("diameter")) {
            xmlDiameter = Convert.ToDouble(((XElement)node).Value).ToString("0.00mm");
            if (xmlDiameter.Equals(diameter)) {
              node = node.NextNode;
              while (node != null && ((XElement)node).Name.ToString().ToLower().Equals("thickness")) {
                tmp.Add(Convert.ToDouble(((XElement)node).Value).ToString("0.000mm"));
                node = node.NextNode;
              }
              return tmp.ToArray();
            }
          }
          node = node.NextNode;
        }
      }
      return tmp.ToArray();
    } // GetThickness

  } // XMLTubes
}
