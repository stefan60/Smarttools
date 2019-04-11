using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

using SolidWorks.Interop.swconst;

using SmartTools.Lib;
using SmartTools.XML;

namespace SmartTools.Patterns {
  /// <summary>Създава модел ОС</summary>
  [ComVisible(true)]
  public class Axis: PatternBase {
    /// <summary>тип на модела</summary>
    public override swDocumentTypes_e swDocumentType {
      get { return swDocumentTypes_e.swDocPART; }
    }
    /// <summary>тип на модела определен от приложението</summary>
    public override Prime.Patterns_e modelType {
      get { return Prime.Patterns_e.Axis; }
    }
    /// <summary>наименование на размера за диаметър</summary>
    public string nameSizeDiameter {
      get { return Prime.GetControlsStringValue(Prime.Controls_e.Diameter, Prime.ControlElement_e.NameSize, this.modelType); }
    }
    /// <summary>наименование на размер за дължина на оста</summary>
    public string nameSizeLength {
      get { return Prime.GetControlsStringValue(Prime.Controls_e.Length, Prime.ControlElement_e.NameSize, this.modelType); }
    }
    /// <summary>наименование на скицата</summary>
    public string nameSketch {
      get { return this.modelType.ToString() + "Sketch"; }
    }
    /// <summary>наименование на 3D построението</summary>
    public string nameExtrude {
      get { return "BossExtrude" + this.modelType.ToString(); }
    }

    /// <summary>добавя контроли в swPMPage</summary>
    public override void AddControlsOnPMPage() {
      XMLData data = default(XMLData);
      data = new XMLMaterials();
      this.AddCombobox(Prime.Groups_e.Characteristics, Prime.Controls_e.Material, ((XMLMaterials)data).GetMaterials());
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Diameter, 0);
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Length, 0);
      data = new XMLCertificates();
      this.AddCombobox(Prime.Groups_e.Complement, Prime.Controls_e.Certificate, ((XMLCertificates)data).GetCertificates());
      data = new XMLTests();
      this.AddCombobox(Prime.Groups_e.Complement, Prime.Controls_e.Test, ((XMLTests)data).GetTests());
      this.AddCheckbox(Prime.Groups_e.Complement, Prime.Controls_e.NoDrawing);
    } // AddControlsOnPMPage

    /// <summary>изгражда 3D модела</summary>
    /// <returns>TRUE модела е изграден, FALSE проблем с параметрита</returns>
    public override Boolean Creating() {
      if ((double)this.Value(Prime.Controls_e.Diameter) > 0 &&
          (double)this.Value(Prime.Controls_e.Length) > 0) {
        this.SelectPlane("Top Plane");
        this.Circle(0, 0, 0, (double)this.Value(Prime.Controls_e.Diameter),this.nameSizeDiameter, nameSketch);
        this.Extrusion(this.nameSketch, (double)this.Value(Prime.Controls_e.Length), this.nameSizeLength, this.nameExtrude);
        return true;
      }
      if ((double)this.Value(Prime.Controls_e.Diameter) <= 0)
        System.Windows.Forms.MessageBox.Show(Prime.GetControlsStringValue(Prime.Controls_e.Diameter, Prime.ControlElement_e.Error, this.modelType),
          "Внимание",
          System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
      if ((double)this.Value(Prime.Controls_e.Length) <= 0)
        System.Windows.Forms.MessageBox.Show(Prime.GetControlsStringValue(Prime.Controls_e.Length, Prime.ControlElement_e.Error, this.modelType),
          "Внимание",
          System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
      return false;
    } // Creating

    /// <summary>
    /// съхранява характеристиките на модела във Филе Пропертиес
    /// </summary>
    public override void SetFileProperties() {
      this.FilePropetyTab();
      this.fileProperties.Header(this.modelType);
      this.fileProperties.Characteristics(this.Value(Prime.Controls_e.Material).ToString());
      this.fileProperties.Complements(this.Value(Prime.Controls_e.Certificate).ToString(),
        this.Value(Prime.Controls_e.Test).ToString(), (bool)this.Value(Prime.Controls_e.NoDrawing));
      this.fileProperties.Dimension(diameter: (double)this.Value(Prime.Controls_e.Diameter),
        length: (double)this.Value(Prime.Controls_e.Length));

      this.FilePropetyTab(Properties.CONFIGURATION_SPECIFIC);
      string info = "О";
      info += (this.Selection(Prime.Controls_e.Material) + 1).ToString() + " " +
              ((double)this.Value(Prime.Controls_e.Diameter) * 1000).ToString() + " " +
              ((double)this.Value(Prime.Controls_e.Length) * 1000).ToString();
      if ((bool)this.Value(Prime.Controls_e.NoDrawing)) {
        info += " БЧ";
      } else {
        info = "";
      }
      this.fileProperties.DrawingNumber(info);
      info = "Ос ф" + ((double)this.Value(Prime.Controls_e.Diameter) * 1000).ToString() + " L" +
        ((double)this.Value(Prime.Controls_e.Length) * 1000).ToString();
      this.fileProperties.Description(info);
      this.fileProperties.Complements(this.Value(Prime.Controls_e.Certificate).ToString(), 
        this.Value(Prime.Controls_e.Test).ToString(), (bool)this.Value(Prime.Controls_e.NoDrawing));
      if ((bool)this.Value(Prime.Controls_e.NoDrawing)) {
        this.fileProperties.Note("БЧ");
      } else {
        this.fileProperties.Note("");
      }
    } // SetFileProperties

    /// <summary>
    /// създава папка с постоенията за модела
    /// </summary>
    public override void CreateFolder() {
      // за модела ОС не е необходимо да се създава папка с построенията
    } // CreateFolder

    /// <summary>
    /// подготвя характеристиките на модела за редактиране
    /// </summary>
    public override void EditDocumentSW(bool defValues = false) {
      if (!defValues) {
        this.FilePropetyTab();
        this.SetSelection(Prime.Controls_e.Material,
          this.ElementSelection(this.fileProperties.Material(), Prime.Controls_e.Material));
        this.SetValue(Prime.Controls_e.Diameter, this.fileProperties.Diameter());
        this.SetValue(Prime.Controls_e.Length, this.fileProperties.Length());
        this.SetSelection(Prime.Controls_e.Certificate,
          this.ElementSelection(this.fileProperties.Certificate(), Prime.Controls_e.Certificate));
        this.SetSelection(Prime.Controls_e.Test,
          this.ElementSelection(this.fileProperties.Test(), Prime.Controls_e.Test));
        this.SetChecked(Prime.Controls_e.NoDrawing, this.fileProperties.NoDrawing());
        this.initialCreation = false;
      } else {
        this.SetSelection(Prime.Controls_e.Material, 0);
        this.SetValue(Prime.Controls_e.Diameter, 0);
        this.SetValue(Prime.Controls_e.Length, 0);
        this.SetSelection(Prime.Controls_e.Certificate, 0);
        this.SetSelection(Prime.Controls_e.Test, 0);
        this.SetChecked(Prime.Controls_e.NoDrawing, true);
      }
    } // EditDocumentSW

    /// <summary>
    /// връща наименованието на избрания материал
    /// </summary>
    /// <returns></returns>
    public override String MaterialName() {
      return this.Value(Prime.Controls_e.Material).ToString();
    } // MaterialName

    /// <summary>
    /// установява стойностите на размерите от въведениет
    /// </summary>
    public override void SetControls() {
      this.SetDimension(this.nameSizeDiameter + "@" + this.nameSketch, (double)this.Value(Prime.Controls_e.Diameter));
      this.SetDimension(this.nameSizeLength + "@" + this.nameExtrude, (double)this.Value(Prime.Controls_e.Length));
    } // SetControls

    /// <summary>
    /// проверява приложимостта на модела към съществуващата среда на SW
    /// </summary>
    /// <returns>TRUE модела е приложим, FALSE не може да се приложи</returns>
    public override Boolean Relevance() {
      return true;
    } // Relevance

  } // Axis
}
