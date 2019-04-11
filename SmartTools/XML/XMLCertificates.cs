using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTools.XML {
  public class XMLCertificates: XMLData {

    public XMLCertificates(): base("certificate.xml") {
    } // XMLCertificates

    public string[] GetCertificates() {
      return this.GetItems("code");
    } // GetCertificates

  } // XMLCertificates

}
