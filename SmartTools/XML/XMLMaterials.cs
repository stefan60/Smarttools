using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace SmartTools.XML {

  public class XMLMaterials: XMLData {

    public XMLMaterials(): base("materials.xml") {
    } // XMLMaterials

    public string[] GetMaterials() {
      return this.GetItems("code");
    } // XMLMaterials

  } // XMLMaterials

}
