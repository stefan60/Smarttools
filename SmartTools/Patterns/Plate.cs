using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

using SolidWorks.Interop.swconst;

using SmartTools.Lib;
using SmartTools.XML;

namespace SmartTools.Patterns {

  /// <summary>
  /// Създава модел ПЛАНКА
  /// </summary>
  [ComVisible(true)]
  public class Plate: PatternBase  {
    /// <summary>тип на модела</summary>
    public override swDocumentTypes_e swDocumentType {
      get { return swDocumentTypes_e.swDocPART; }
    }
    /// <summary>тип на модела определен от приложението</summary>
    public override Prime.Patterns_e modelType {
      get { return Prime.Patterns_e.Plate; }
    }
    /// <summary>наименование на размера за дебелина</summary>
    public string nameSizeThickness {
      get { return Prime.GetControlsStringValue(Prime.Controls_e.Thickness, Prime.ControlElement_e.NameSize, this.modelType); }
    }
    /// <summary>наименование на размера за ширина</summary>
    public string nameSizeWidth {
      get { return Prime.GetControlsStringValue(Prime.Controls_e.Width, Prime.ControlElement_e.NameSize, this.modelType); }
    }
    /// <summary>наименование на размера за дължина</summary>
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
    /// <summary>добавя контролите в swPMPage</summary>
    public override void AddControlsOnPMPage() {
      XMLData data = default(XMLData);
      // материал
      data = new XMLMaterials();
      this.AddCombobox(Prime.Groups_e.Characteristics, Prime.Controls_e.Material, ((XMLMaterials)data).GetMaterials());
      // дебелина
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Thickness, 0);
      // ширина
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Width, 0);
      // дължина
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Length, 0);
      // сертификат
      data = new XMLCertificates();
      this.AddCombobox(Prime.Groups_e.Complement, Prime.Controls_e.Certificate, ((XMLCertificates)data).GetCertificates());
      // качество
      data = new XMLTests();
      this.AddCombobox(Prime.Groups_e.Complement, Prime.Controls_e.Test, ((XMLTests)data).GetTests());
      // без чертеж
      this.AddCheckbox(Prime.Groups_e.Complement, Prime.Controls_e.NoDrawing);
    } // AddControlsOnPMPage

    /// <summary>изграажда 3D модела</summary>
    /// <returns>състояние TRUE модела е изграден, FALSE проблем с параметрите на модела</returns>
    public override bool Creating() {
      if ((double)this.Value(Prime.Controls_e.Thickness) > 0 &&
          (double)this.Value(Prime.Controls_e.Width) > 0 &&
          (double)this.Value(Prime.Controls_e.Length) > 0) {
        this.SelectPlane("Top Plane");
        this.Rectangle((double)this.Value(Prime.Controls_e.Width),
                       (double)this.Value(Prime.Controls_e.Length),
                       this.nameSizeWidth, this.nameSizeLength, this.nameSketch);
        this.Extrusion(this.nameSketch, (double)this.Value(Prime.Controls_e.Thickness),
          this.nameSizeThickness, this.nameExtrude);
        return true;
      }
      if ((double)this.Value(Prime.Controls_e.Thickness) <= 0) {
        System.Windows.Forms.MessageBox.Show(Prime.GetControlsStringValue(Prime.Controls_e.Thickness, Prime.ControlElement_e.Error, this.modelType),
          "Внимание",
          System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
      }
      if ((double)this.Value(Prime.Controls_e.Width) <= 0)
        System.Windows.Forms.MessageBox.Show(Prime.GetControlsStringValue(Prime.Controls_e.Width, Prime.ControlElement_e.Error, this.modelType),
          "Внимание",
          System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
      if ((double)this.Value(Prime.Controls_e.Length) <= 0)
        System.Windows.Forms.MessageBox.Show(Prime.GetControlsStringValue(Prime.Controls_e.Length, Prime.ControlElement_e.Error, this.modelType),
          "Внимание",
          System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
      return false;
    } // Creating

    /// <summary>
    /// съхранява характеристиките на модела във File Properties
    /// </summary>
    public override void SetFileProperties() {
      this.FilePropetyTab();
      this.fileProperties.Header(this.modelType);
      this.fileProperties.Characteristics(this.Value(Prime.Controls_e.Material).ToString());
      this.fileProperties.Complements(this.Value(Prime.Controls_e.Certificate).ToString(),
        this.Value(Prime.Controls_e.Test).ToString(), (bool)this.Value(Prime.Controls_e.NoDrawing));
      this.fileProperties.Dimension(thickness: (double)this.Value(Prime.Controls_e.Thickness),
        width: (double)this.Value(Prime.Controls_e.Width), length: (double)this.Value(Prime.Controls_e.Length));

      this.FilePropetyTab(Properties.CONFIGURATION_SPECIFIC);
      string info = "Пл";
      info += (this.Selection(Prime.Controls_e.Material) + 1).ToString() + " " +
              ((double)this.Value(Prime.Controls_e.Thickness) * 1000).ToString() + " " +
              ((double)this.Value(Prime.Controls_e.Width) * 1000).ToString() + " " +
              ((double)this.Value(Prime.Controls_e.Length) * 1000).ToString();
      if ((bool)this.Value(Prime.Controls_e.NoDrawing)) {
        info += " БЧ";
      } else {
        info = "";
      }
      this.fileProperties.DrawingNumber(info);

      info = "Планка " + 
        ((double)this.Value(Prime.Controls_e.Thickness) * 1000).ToString() + "x" +
        ((double)this.Value(Prime.Controls_e.Width) * 1000).ToString() + "x" +
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
    /// създава папка с построенията за модела
    /// </summary>
    public override void CreateFolder() {
      // за модела Планка не е необходимо да се създава папка с построенията
    } // CreateFolder

    /// <summary>
    /// подготвя характеристиките на модела за редактиране
    /// </summary>
    public override void EditDocumentSW(bool defValues = false) {
      if (!defValues) {
        this.FilePropetyTab();
        this.SetSelection(Prime.Controls_e.Material,
          this.ElementSelection(this.fileProperties.Material(), Prime.Controls_e.Material));
        this.SetValue(Prime.Controls_e.Thickness, this.fileProperties.Thickness());
        this.SetValue(Prime.Controls_e.Width, this.fileProperties.Width());
        this.SetValue(Prime.Controls_e.Length, this.fileProperties.Length());
        this.SetSelection(Prime.Controls_e.Certificate,
          this.ElementSelection(this.fileProperties.Certificate(), Prime.Controls_e.Certificate));
        this.SetSelection(Prime.Controls_e.Test,
          this.ElementSelection(this.fileProperties.Test(), Prime.Controls_e.Test));
        this.SetChecked(Prime.Controls_e.NoDrawing, this.fileProperties.NoDrawing());
        this.initialCreation = false;
      } else {
        this.SetSelection(Prime.Controls_e.Material, 0);
        this.SetValue(Prime.Controls_e.Thickness, 0);
        this.SetValue(Prime.Controls_e.Width, 0);
        this.SetValue(Prime.Controls_e.Length, 0);
        this.SetSelection(Prime.Controls_e.Certificate, 0);
        this.SetSelection(Prime.Controls_e.Test, 0);
        this.SetChecked(Prime.Controls_e.NoDrawing, true);
      }
      //this.initialCreation = false;
    } // EditDocumentSW

    /// <summary>
    /// връща наименованието на избрания материал
    /// </summary>
    /// <returns>избран материал</returns>
    public override String MaterialName() {
      return this.Value(Prime.Controls_e.Material).ToString();
    } // MaterialName

    /// <summary>установява стойностите на размерите от въведените</summary>
    public override void SetControls() {
      this.SetDimension(this.nameSizeWidth + "@" + this.nameSketch, (double)this.Value(Prime.Controls_e.Width));
      this.SetDimension(this.nameSizeLength + "@" + this.nameSketch, (double)this.Value(Prime.Controls_e.Length));
      this.SetDimension(this.nameSizeThickness + "@" + this.nameExtrude, (double)this.Value(Prime.Controls_e.Thickness));
    } // SetControls

    /// <summary>
    /// проверява приложимостта на модела към съществуващата среда на SW
    /// </summary>
    /// <returns>TRUE модела е приложим, FALSE не може да се приложи</returns>
    public override Boolean Relevance() {
      return true;
    } // Relevance

  } // Plate

}
