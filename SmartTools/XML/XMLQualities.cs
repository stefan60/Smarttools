using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartTools.XML {
  public class XMLQualities: XMLData {

    public XMLQualities(): base("qualities.xml") {
    } // qualities

    public string[] GetQualities(string attribute) {
      return this.GetItems("code", attribute);
    } // GetQualities

  } // XMLQualities

}
