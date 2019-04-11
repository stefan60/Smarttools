using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartTools.XML {
  public class XMLStandards: XMLData {

    public XMLStandards(): base("standards.xml") {
    } // XMLStandards

    public string[] GetStandards() {
      return this.GetItems("code");
    } // GetStandards

  } // XMLStandards
}
