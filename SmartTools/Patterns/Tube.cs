using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

using SolidWorks.Interop.swconst;

using SmartTools.Lib;
using SmartTools.XML;

namespace SmartTools.Patterns {
  /// <summary>създава модел тръба</summary>
  [ComVisible(true)]
  public class Tube: PatternBase {
    /// <summary>тип на модела</summary>
    public override swDocumentTypes_e swDocumentType {
      get { return swDocumentTypes_e.swDocPART; }
    }
    /// <summary>тип на модела определен от приложението</summary>
    public override Prime.Patterns_e modelType {
      get { return Prime.Patterns_e.Tube; }
    }
    /// <summary>връща стойноста на диаметъра в размер метри</summary>
    public double diameter {
      get { return Convert.ToDouble(this.Value(Prime.Controls_e.Diameter).ToString().TrimEnd('m')) / 1000; }
    }
    /// <summary>връща стойноста на дебелината на стената в размер метри</summary>
    public double thickness {
      get { return Convert.ToDouble(this.Value(Prime.Controls_e.Thickness).ToString().TrimEnd('m')) / 1000; }
    }
    /// <summary>връща стойноста на дължината в размер метри</summary>
    public double length {
      get { return (double)this.Value(Prime.Controls_e.Length); }
    }
    /// <summary>наименование на размера за диаметър</summary>
    public string nameSizeDiameter {
      get { return Prime.GetControlsStringValue(Prime.Controls_e.Diameter, Prime.ControlElement_e.NameSize, this.modelType); }
    }
    /// <summary>наименование на размер за дължина на тръбата</summary>
    public string nameSizeLength {
      get { return Prime.GetControlsStringValue(Prime.Controls_e.Length, Prime.ControlElement_e.NameSize, this.modelType); }
    }
    /// <summary>наименование на размер за дебелина на стената на тръбата</summary>
    public string nameSizeThickness {
      get { return Prime.GetControlsStringValue(Prime.Controls_e.Thickness, Prime.ControlElement_e.NameSize, this.modelType); }
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
      // стандарт
      data = new XML.XMLStandards();
      this.AddCombobox(Prime.Groups_e.Characteristics, Prime.Controls_e.Standard, ((XMLStandards)data).GetStandards());
      // качество
      string[] attr = this.Value(Prime.Controls_e.Standard).ToString().Split(' ');
      data = new XML.XMLQualities();
      this.AddCombobox(Prime.Groups_e.Characteristics, Prime.Controls_e.Quality, ((XMLQualities)data).GetQualities(attr[0]));
      // материал
      data = new XML.XMLMaterials();
      this.AddCombobox(Prime.Groups_e.Characteristics, Prime.Controls_e.Material, ((XMLMaterials)data).GetMaterials());
      // диаметър
      string std = String.Join("", this.Value(Prime.Controls_e.Standard).ToString().Split(' '));
      int i = std.IndexOf('-');
      if (i >= 0)
        std = std.Remove(i, 1);
      data = new XML.XMLTubes();
      this.AddCombobox(Prime.Groups_e.Dimension, Prime.Controls_e.Diameter, ((XMLTubes)data).GetDiameters(std));
      // дебелина
      attr[1] = this.Value(Prime.Controls_e.Diameter).ToString();
      this.AddCombobox(Prime.Groups_e.Dimension, Prime.Controls_e.Thickness, ((XMLTubes)data).GetThickness(std, attr[1]));
      // дължина
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Length, 0);
      // сертификат
      data = new XML.XMLCertificates();
      this.AddCombobox(Prime.Groups_e.Complement, Prime.Controls_e.Certificate, ((XMLCertificates)data).GetCertificates());
      // тест
      data = new XML.XMLTests();
      this.AddCombobox(Prime.Groups_e.Complement, Prime.Controls_e.Test, ((XMLTests)data).GetTests());
      // без чертеж
      this.AddCheckbox(Prime.Groups_e.Complement, Prime.Controls_e.NoDrawing);
    } // AddControlsOnPMPage

    /// <summary>
    /// обработва ситуция при промяна избора на Стандарт или Диаметър
    /// </summary>
    /// <param name="control">тип на контролата</param>
    /// <param name="selection">избран елемент</param>
    public override void ChangeStandardOrDiameter(Int32 control, Int32 selection) {
      if (control == (int)Prime.Controls_e.Standard) {
        // избран нов стандарт
        this.pmpCombobox[Prime.Controls_e.Quality].Clear();
        string[] st = this.pmpCombobox[Prime.Controls_e.Standard].ItemText[(short)selection].Split(' ');
        XML.XMLData data = new XML.XMLQualities();
        this.pmpCombobox[Prime.Controls_e.Quality].AddItems(((XML.XMLQualities)data).GetQualities(st[0]));
        this.SetSelection(Prime.Controls_e.Quality, 0);
        string std = String.Join("", st);
        int i = std.IndexOf('-');
        if (i >= 0)
          std = std.Remove(i, 1);
        this.pmpCombobox[Prime.Controls_e.Diameter].Clear();
        data = new XML.XMLTubes();
        this.pmpCombobox[Prime.Controls_e.Diameter].AddItems(((XML.XMLTubes)data).GetDiameters(std));
        this.SetSelection(Prime.Controls_e.Diameter, 0);
        st[0] = this.Value(Prime.Controls_e.Diameter).ToString();
        this.pmpCombobox[Prime.Controls_e.Thickness].Clear();
        data = new XML.XMLTubes();
        this.pmpCombobox[Prime.Controls_e.Thickness].AddItems(((XML.XMLTubes)data).GetThickness(std, st[0]));
        this.SetSelection(Prime.Controls_e.Thickness, 0);
      }
      if (control == (int)Prime.Controls_e.Diameter) {
        // избран нов диаметър
        string std = String.Join("", this.Value(Prime.Controls_e.Standard).ToString().Split(' '));
        int i = std.IndexOf('-');
        if (i >= 0)
          std = std.Remove(i, 1);
        string diameter = this.pmpCombobox[Prime.Controls_e.Diameter].ItemText[(short)selection];
        this.pmpCombobox[Prime.Controls_e.Thickness].Clear();
        XML.XMLData data = new XML.XMLTubes();
        this.pmpCombobox[Prime.Controls_e.Thickness].AddItems(((XML.XMLTubes)data).GetThickness(std, diameter));
        this.SetSelection(Prime.Controls_e.Thickness, 0);
      }
    } // ChangeStandardOrDiameter

    /// <summary>изгражда 3D модела</summary>
    /// <returns>TRUE - модела е изграден, FALSE - проблем с параметрите</returns>
    public override Boolean Creating() {
      if (this.length > 0) {
        this.SelectPlane("Top Plane");
        this.Circle(0, 0, 0, this.diameter, this.nameSizeDiameter, this.nameSketch);
        this.Extrusion(this.nameSketch, this.length, this.nameSizeLength, this.nameExtrude,
          this.thickness, this.nameSizeThickness);
        return true;
      }
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
      this.fileProperties.Characteristics(this.Value(Prime.Controls_e.Material).ToString(),
        this.Value(Prime.Controls_e.Standard).ToString(), this.Value(Prime.Controls_e.Quality).ToString());
      this.fileProperties.Complements(this.Value(Prime.Controls_e.Certificate).ToString(),
        this.Value(Prime.Controls_e.Test).ToString(), (bool)this.Value(Prime.Controls_e.NoDrawing));
      this.fileProperties.Dimension(diameter: this.diameter, length: this.length, thickness: this.thickness);

      this.FilePropetyTab(Properties.CONFIGURATION_SPECIFIC);
      string info = "Тр";
      info += (this.Selection(Prime.Controls_e.Material) + 1).ToString() + " " +
              (this.diameter * 1000).ToString() + " " +
              (this.thickness * 1000).ToString() + " " +
              (this.length * 1000).ToString() + " " +
              this.Value(Prime.Controls_e.Quality).ToString();
      if ((bool)this.Value(Prime.Controls_e.NoDrawing)) {
        info += " БЧ";
      } else {
        info = "";
      }
      this.fileProperties.DrawingNumber(info);
      info = "Тръба ф" + (this.diameter * 1000).ToString() + "x" +
        (this.thickness * 1000).ToString() + " L" +
        (this.length * 1000).ToString() + "; " +
        this.Value(Prime.Controls_e.Standard).ToString();
      this.fileProperties.Description(info);
      this.fileProperties.Complements(this.Value(Prime.Controls_e.Certificate).ToString(),
        this.Value(Prime.Controls_e.Test).ToString(), (bool)this.Value(Prime.Controls_e.NoDrawing));
      if ((bool)this.Value(Prime.Controls_e.NoDrawing)) {
        this.fileProperties.Note("БЧ");
      } else {
        this.fileProperties.Note("");
      }
    } // SetFileProperties

    /// <summary>създава папка с постоенията за модела</summary>
    public override void CreateFolder() {
      // за модел ТРЪБА не е необходимо да се създава пака с построенията
    }

    /// <summary>
    /// подготвя характеристиките на модела за редактиране
    /// </summary>
    public override void EditDocumentSW(bool defValues = false) {
      if (!defValues) {
        this.FilePropetyTab();
        short sel = this.ElementSelection(this.fileProperties.Standard(), Prime.Controls_e.Standard);
        this.SetSelection(Prime.Controls_e.Standard, sel);

        this.pmpCombobox[Prime.Controls_e.Quality].Clear();
        string[] st = this.pmpCombobox[Prime.Controls_e.Standard].ItemText[sel].Split(' ');
        XML.XMLData data = new XML.XMLQualities();
        this.pmpCombobox[Prime.Controls_e.Quality].AddItems(((XML.XMLQualities)data).GetQualities(st[0]));
        this.SetSelection(Prime.Controls_e.Quality,
          this.ElementSelection(this.fileProperties.Quality(), Prime.Controls_e.Quality));
        string std = String.Join("", st);
        int i = std.IndexOf('-');
        if (i >= 0)
          std = std.Remove(i, 1);
        this.pmpCombobox[Prime.Controls_e.Diameter].Clear();
        data = new XML.XMLTubes();
        this.pmpCombobox[Prime.Controls_e.Diameter].AddItems(((XML.XMLTubes)data).GetDiameters(std));
        sel = this.ElementSelection((Convert.ToDouble(this.fileProperties.Diameter()) * 1000).ToString("0.00mm"), Prime.Controls_e.Diameter);
        this.SetSelection(Prime.Controls_e.Diameter, sel);
        st[0] = this.Value(Prime.Controls_e.Diameter).ToString();
        this.pmpCombobox[Prime.Controls_e.Thickness].Clear();
        data = new XML.XMLTubes();
        this.pmpCombobox[Prime.Controls_e.Thickness].AddItems(((XML.XMLTubes)data).GetThickness(std, st[0]));
        this.SetSelection(Prime.Controls_e.Material,
          this.ElementSelection(this.fileProperties.Material(), Prime.Controls_e.Material));
        std = String.Join("", this.Value(Prime.Controls_e.Standard).ToString().Split(' '));
        i = std.IndexOf('-');
        if (i >= 0)
          std = std.Remove(i, 1);
        string diameter = this.pmpCombobox[Prime.Controls_e.Diameter].ItemText[sel];
        this.pmpCombobox[Prime.Controls_e.Thickness].Clear();
        data = new XML.XMLTubes();
        this.pmpCombobox[Prime.Controls_e.Thickness].AddItems(((XML.XMLTubes)data).GetThickness(std, diameter));
        this.SetSelection(Prime.Controls_e.Thickness,
          this.ElementSelection((Convert.ToDouble(this.fileProperties.Thickness()) * 1000).ToString("0.000mm"), Prime.Controls_e.Thickness));
        this.SetValue(Prime.Controls_e.Length, this.fileProperties.Length());
        this.SetSelection(Prime.Controls_e.Certificate,
          this.ElementSelection(this.fileProperties.Certificate(), Prime.Controls_e.Certificate));
        this.SetSelection(Prime.Controls_e.Test,
          this.ElementSelection(this.fileProperties.Test(), Prime.Controls_e.Test));
        this.SetChecked(Prime.Controls_e.NoDrawing, this.fileProperties.NoDrawing());
        this.initialCreation = false;
      } else {
        this.SetSelection(Prime.Controls_e.Standard, 0);
        this.SetSelection(Prime.Controls_e.Quality, 0);
        this.SetSelection(Prime.Controls_e.Diameter, 0);
        this.SetSelection(Prime.Controls_e.Material, 0);
        this.SetSelection(Prime.Controls_e.Thickness, 0);
        this.SetValue(Prime.Controls_e.Length, 0);
        this.SetSelection(Prime.Controls_e.Certificate, 0);
        this.SetSelection(Prime.Controls_e.Test, 0);
        this.SetChecked(Prime.Controls_e.NoDrawing, true);
      }
    } // EditDocumentSW

    /// <summary>
    /// връща наименованието на избрания материал
    /// </summary>
    /// <returns>наименование на материала</returns>
    public override String MaterialName() {
      return this.Value(Prime.Controls_e.Material).ToString();
    } // MaterialName

    /// <summary>
    /// установява стойностите на размерите от въведените
    /// </summary>
    public override void SetControls() {
      this.SetDimension(this.nameSizeDiameter + "@" + this.nameSketch, this.diameter);
      this.SetDimension(this.nameSizeLength + "@" + this.nameExtrude, this.length);
      this.SetDimension(this.nameSizeThickness + "@" + this.nameExtrude, this.thickness);
    } // SetControls

    /// <summary>
    /// проверява приложимостта на модела към съществуващата среда на SW
    /// </summary>
    /// <returns>TRUE модела е приложим, FALSE не може да се приложи</returns>
    public override Boolean Relevance() {
      return true;
    } // Relevance

  } // Tube

}
