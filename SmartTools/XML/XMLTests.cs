using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTools.XML {
  public class XMLTests: XMLData {

    public XMLTests(): base("tests.xml") {
    } // XMLTests

    public string[] GetTests() {
      return this.GetItems("code");
    } // GetTests

  } // XMLTests

}
