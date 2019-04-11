using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

using SmartTools.Lib;
using SmartTools.XML;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.sldworks;

namespace SmartTools.Patterns {
  /// <summary>Създава модела МАНТЕЛ</summary>
  [ComVisible(true)]
  public class Mantel: Lib.PatternBase {

    /// <summary>създава повърхност около модела</summary>
    /// <data>2019-04-09</data>
    /// <param name="offset">отстояние от стената</param>
    /// <param name="location">местоположение на повърхнината, TRUE от външната страна, FALSE от вътрешната страна</param>
    /// <param name="nameSurfaceBody">наименование на повърхнината</param>
    /// <param name="nameSurfaceSketch">хаименование на скицата</param>
    public override void CreateSurface(double offset, bool location, string nameSurfaceBody, string nameSurfaceSketch) {
      double distance;
      // определя отстоянието до повърхнината
      if (location) {
        // от външната страна добавя дебелината на стената
        distance = this.thickness + offset;
      } else {
        distance = offset;
      }
      this.SelectPlane(this.swFrontPlane);
      // създава скицата на повърхнината
      bool status = this.Selected(this.FullName("Line1", this.nameBodySketch), this.swExtSketchSegment);
      status = this.swDocument.SketchOffsetEntities2(distance, false, true);
      this.InsertSketch();
      this.ClearSelection();
      this.RenameFeature("Sketch", nameSurfaceSketch);
      // създава повърхнината
      status = this.Selected(nameSurfaceSketch, this.swSketch);
      status = this.Selected(this.nameAxis, this.swAxis, true, 16);
      this.swFeatureManager.FeatureRevolve2(true, false, false, false, false,
        false, (int)swEndConditions_e.swEndCondBlind, 0, Prime.RADIANS_360DEG,
        0, false, false, 0, 0, 0, 0, 0, true, true, true);
      this.RenameFeature("Surface-Revolve", nameSurfaceBody);
    } // CreateSurface


    #region Properties
    #region Global Properties
    /// <summary>тип на модела в SW</summary>
    public override swDocumentTypes_e swDocumentType {
      get { return swDocumentTypes_e.swDocPART; }
    }
    /// <summary>тип на модела в приложението</summary>
    public override Prime.Patterns_e modelType {
      get { return Prime.Patterns_e.Mantel; }
    }
    #endregion Global Properties
    #region Default Values Controls
    /// <summary>стойност по подразбиране за диаметър, в метри</summary>
    public double defDiameter { get { return Prime.TEST ? 2 : 0; } }
    /// <summary>стойност по подразбиране за дебелината на стената, в метри</summary>
    public double defThickness { get { return Prime.TEST ? 0.005 : 0; } }
    /// <summary>стойност по подразбиране за височината, в метри</summary>
    public double defHeight { get { return Prime.TEST ? 1.2 : 0; } }
    /// <summary>стйност по подразбиране за ъгъла на заваръчния шев, в радиани</summary>
    public double defAngle { get { return Prime.TEST ? Prime.InRadians(135) : 0; } }
    /// <summary>стойност на ъгъла визуализиращ заваръчния щев</summary>
    //public double defAngleView { get { return Prime.InRadians(0.2); } }
    /// <summary>флаг за промяна на координатната система</summary>
    protected bool changeCS = false;
    /// <summary>флаг за промяна на диаметъра и/или дебелината</summary>
    protected bool changeDT = false;
    #endregion Default Values Controls
    #endregion Properties

    #region Override Methods
    /// <summary>добавя контроли в swPMPage</summary>
    public override void AddControlsOnPMPage() {
      // контрола за избор на посоката на нарастване на ъгъла в координатната система
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.Clockwise, true, true, true);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.AntiClockwise);
      // контрола за избор на позизция на 0 градуса в координатната система
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.NortPosition, true, true, true);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.EastPosition);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.SoutPosition);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.WestPosition);
      // контрола за избор на материал
      XML.XMLData data = new XML.XMLMaterials();
      this.AddCombobox(Prime.Groups_e.Characteristics, Prime.Controls_e.Material, ((XMLMaterials)data).GetMaterials());
      // контроли за габаритните размери - диаметър, дебелина на стената, височина и ълово отместване на заваръчния шев
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Diameter, this.defDiameter);
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Thickness, this.defThickness);
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Height, this.defHeight);
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.WeldingSeam, this.defAngle, swNumberboxUnitType_e.swNumberBox_Angle, 0, Prime.InRadians(360));
      // контроли за избор на допълнителни характеристики - сертификат и тест
      data = new XML.XMLCertificates();
      this.AddCombobox(Prime.Groups_e.Complement, Prime.Controls_e.Certificate, ((XMLCertificates)data).GetCertificates());
      data = new XML.XMLTests();
      this.AddCombobox(Prime.Groups_e.Complement, Prime.Controls_e.Test, ((XMLTests)data).GetTests());
    } // AddControlsOnPMPage

    /// <summary>подготвя характеристиките на модела за редактиране</summary>
    public override void EditDocumentSW(bool defValues = false) {
      if (!defValues) {
        this.FilePropetyTab();
        this.SetChecked(Prime.Controls_e.Clockwise, this.fileProperties.Clockwise());
        this.SetChecked(Prime.Controls_e.AntiClockwise, !this.fileProperties.Clockwise());
        string pos = this.fileProperties.Position0().ToLower();
        this.SetChecked(Prime.Controls_e.NortPosition, pos.Equals(Prime.Controls_e.NortPosition.ToString().ToLower()));
        this.SetChecked(Prime.Controls_e.EastPosition, pos.Equals(Prime.Controls_e.EastPosition.ToString().ToLower()));
        this.SetChecked(Prime.Controls_e.SoutPosition, pos.Equals(Prime.Controls_e.SoutPosition.ToString().ToLower()));
        this.SetChecked(Prime.Controls_e.WestPosition, pos.Equals(Prime.Controls_e.WestPosition.ToString().ToLower()));
        this.SetSelection(Prime.Controls_e.Material, this.ElementSelection(this.fileProperties.Material(), Prime.Controls_e.Material));
        this.SetValue(Prime.Controls_e.Diameter, this.fileProperties.Diameter());
        this.SetValue(Prime.Controls_e.Thickness, this.fileProperties.Thickness());
        this.SetValue(Prime.Controls_e.Height, this.fileProperties.Height());
        this.SetValue(Prime.Controls_e.WeldingSeam, this.fileProperties.WeldingSeam());
        this.SetSelection(Prime.Controls_e.Certificate, this.ElementSelection(this.fileProperties.Certificate(), Prime.Controls_e.Certificate));
        this.SetSelection(Prime.Controls_e.Test, this.ElementSelection(this.fileProperties.Test(), Prime.Controls_e.Test));
        this.initialCreation = false;
      } else {
        this.SetChecked(Prime.Controls_e.Clockwise, true);
        this.SetChecked(Prime.Controls_e.AntiClockwise, false);
        this.SetChecked(Prime.Controls_e.NortPosition, true);
        this.SetChecked(Prime.Controls_e.EastPosition, false);
        this.SetChecked(Prime.Controls_e.SoutPosition, false);
        this.SetChecked(Prime.Controls_e.WestPosition, false);
        this.SetSelection(Prime.Controls_e.Material, 0);
        this.SetValue(Prime.Controls_e.Diameter, this.defDiameter);
        this.SetValue(Prime.Controls_e.Thickness, this.defThickness);
        this.SetValue(Prime.Controls_e.Height, this.defHeight);
        this.SetValue(Prime.Controls_e.WeldingSeam, this.defAngle);
        this.SetSelection(Prime.Controls_e.Certificate, 0);
        this.SetSelection(Prime.Controls_e.Test, 0);
      }
    } // EditDocumentSW

    /// <summary>изгражда 3D модела</summary>
    /// <returns>TRUE моделът е изграден, FALSE проблем с параметрите</returns>
    public override bool Creating() {
      if (this.diameter > 0 && this.thickness > 0 && this.height > 0) {
        // създава оста на тялото
        this.Axis(this.swFrontPlane, this.swRightPlane, this.nameAxis);
        // създава тялото
        this.CreatingBody();
        // създава визуализация на заваръчния шев
        this.CreateWeldSeam(this.shoulderWeldingSeam, this.nameBodyWS);
        //this.CreateWeldSeam(this.diameter / 2 + this.thickness + this.addOnSize, this.nameBodyWS);
        return true;
      }
      if (!(this.diameter > 0))
        Prime.ShowError(Prime.Controls_e.Diameter, this.modelType, System.Windows.Forms.MessageBoxButtons.OK);
      if (!(this.thickness > 0))
        Prime.ShowError(Prime.Controls_e.Thickness, this.modelType, System.Windows.Forms.MessageBoxButtons.OK);
      if (!(this.height > 0))
        Prime.ShowError(Prime.Controls_e.Height, this.modelType, System.Windows.Forms.MessageBoxButtons.OK);
      return false;
    } // Creating
    #endregion Override Methods

    

    #region Protected Methods
    /// <summary>създава тялото на мантела</summary>
    protected void CreatingBody() {
      // координати на началната точка на права
      double x1 = this.diameter / 2;
      double y1 = 0;
      // координати на крайната точка на права
      double x2 = x1;
      double y2 = this.height;
      this.SelectPlane(this.swFrontPlane);
      // създава правата
      this.swSketchManager.CreateLine(x1, y1, 0, x2, y2, 0);
      this.ClearSelection();
      this.Selected("Line1", this.swSketchSegment);
      // правата е вертикална
      this.swDocument.SketchAddConstraints(this.sgVertical);
      // размер за височината на тялото
      this.AddSize("Line1", this.swSketchSegment, this.nameHeight, x1 + 0.05, y2 / 2, 0, this.height);
      this.ClearSelection();
      // центъра на координатната система и началото на правата лежат в една равнина
      this.Selected(this.swStartCS, this.swExtSketchPoint);
      this.Selected("Point1", this.swSketchPoint, true);
      this.swDocument.SketchAddConstraints(this.sgHorizontalPoints);
      // размер за вътрешния радиус на тялото
      this.AddSize(this.swStartCS, this.swExtSketchPoint, this.nameDiameter, 
        x1 / 2, 0.02, 0, this.diameter / 2, true, "Point1", this.swSketchPoint);
      this.ClearSelection();
      // преименува скицата
      this.RenameFeature("Sketch", this.nameBodySketch);
      this.InsertSketch();
      this.ClearSelection();
      this.Selected(this.nameBodySketch, this.swSketch);
      this.Selected(this.nameAxis, this.swAxis, true, 16);
      this.swFeatureManager.FeatureRevolve2(true, true, true, false, false, false,
        (int)swEndConditions_e.swEndCondBlind, 0, Prime.RADIANS_360DEG,
        0, false, false, 0, 0, (int)swThinWallType_e.swThinWallOneDirection,
        this.thickness, 0, true, true, true);
      // преименува тялото
      this.RenameFeature("Revolve-Thin", this.nameBody);
      // преименува размера за дебелина
      this.RenameDimension(this.FullName("D3", this.nameBody), this.nameThickness);
      // преименува размера за завъртане
      this.RenameDimension(this.FullName("D1", this.nameBody), "notUsed");
    } // CreatingBody

    #endregion Protected Methods


    #region Old Methods and Properties

    #region Override Methods
    

    

    /// <summary>установява стойностите на размерите от въведените</summary>
    public override void SetControls() {
      this.SetDimension(Prime.Name(this.nameDiameter, this.nameBodySketch), this.diameter / 2);
      this.SetDimension(Prime.Name(this.nameHeight, this.nameBodySketch), this.height);
      this.SetDimension(Prime.Name(this.nameThickness, this.nameBody), this.thickness);
      this.SetViewWeldSeam(this.shoulderWeldingSeam);
      this.RebuildHoles();
    } // SetControls

    /// <summary>
    /// съхранява характеристиките на модела във File Properties
    /// </summary>
    public override void SetFileProperties() {
      this.FilePropetyTab();
      this.fileProperties.Header(this.modelType);
      this.fileProperties.Characteristics(this.Value(Prime.Controls_e.Material).ToString());
      this.fileProperties.CoordinateSystem((bool)this.Value(Prime.Controls_e.Clockwise),
        (bool)this.Value(Prime.Controls_e.NortPosition), (bool)this.Value(Prime.Controls_e.EastPosition),
        (bool)this.Value(Prime.Controls_e.SoutPosition), (bool)this.Value(Prime.Controls_e.WestPosition));
      this.fileProperties.Dimension(diameter: this.diameter, thickness: this.thickness, height: this.height, weldingSeam: this.weldSeamAngle);
      this.fileProperties.Complements(this.Value(Prime.Controls_e.Certificate).ToString(), this.Value(Prime.Controls_e.Test).ToString());
      this.fileProperties.InnerDiameter(true);
      this.FilePropetyTab(Properties.CONFIGURATION_SPECIFIC);
      string info = "Мантел ф" +
        ((this.diameter + 2 * this.thickness) * 1000).ToString() + "x" +
        (this.thickness * 1000).ToString() + " H" + (this.height * 1000).ToString();
      this.fileProperties.Description(info);
      this.fileProperties.Complements(this.Value(Prime.Controls_e.Certificate).ToString(), this.Value(Prime.Controls_e.Test).ToString());
    } // SetFileProperties

    /// <summary>Създава папка на построенията</summary>
    public override void CreateFolder() {
      if (!this.RenameFeature("Мантел", null)) {
        this.swDocument.Extension.SelectByID2(this.nameBody, this.swBody, 0, 0, 0, false, 0, null, 0);
        Feature feature = this.swDocument.FeatureManager.InsertFeatureTreeFolder2((int)swFeatureTreeFolderType_e.swFeatureTreeFolder_Containing);
        this.swDocument.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "Мантел");
        this.swDocument.Extension.SelectByID2(this.nameBodyWS, this.swBody, 0, 0, 0, false, 0, null, 0);
        this.swDocument.Extension.ReorderFeature(this.nameBodyWS, this.nameBody, (int)swMoveLocation_e.swMoveAfter);
        this.swDocument.Extension.ReorderFeature(this.nameAxis, this.nameBody, (int)swMoveLocation_e.swMoveAfter);
        this.swDocument.ClearSelection2(true);
      }
    } // CreateFolder

    /// <summary>
    /// проверява приложимостта на модела към съществуващата среда на SW
    /// </summary>
    /// <returns>TRUE модела е приложим, FALSE не може да се приложи</returns>
    public override Boolean Relevance() {
      return true;
    } // Relevance

    /// <summary>връща наименованието на избрания материал</summary>
    /// <returns>избран материал</returns>
    public override String MaterialName() {
      return this.Value(Prime.Controls_e.Material).ToString();
    } // MaterialName

    

    /// <summary>установява флаг за промяна на координатната система</summary>
    public override void RadioButtonChek() {
      this.changeCS = true;
    } // RadioButtonChek

    /// <summary>установява флаг за промяна на диаметър и/или дебелина</summary>
    public override void DiameterOrThicknessChange() {
      this.changeDT = true;
    }
    #endregion Overiide Methods

    protected void RebuildHoles() {
      this.FilePropetyTab();
      int holes = this.fileProperties.HoleNumber();
      int outHoles = this.fileProperties.OutHoleNumber();
      int inHoles = this.fileProperties.InHoleNumber();
      if ((holes == 0 && outHoles == 0 && inHoles == 0) || !(this.changeCS || this.changeDT)) {
        this.changeCS = false;
        this.changeDT = false;
        return;
      }
      for (int j = 0; j < Prime.AddIn.Patterns.Count; j++) {
        PatternBase t = Prime.AddIn.Patterns.ElementAt(j);
        if (t.modelType == Prime.Patterns_e.Hole && holes > 0) {
          Hole hole = (Hole)t;
          hole.GetCoordinateSystem();
          for (int k = 1; k <= holes; k++) {
            hole.SetHoleNumber(k);
            string name = this.FullName(hole.nameAngle, hole.namePlaneHole);
            double angle = this.fileProperties.Angle(k.ToString());
            hole.SetDimension(this.FullName(hole.nameAngle, hole.namePlaneHole), this.RealAngle(this.fileProperties.Angle(k.ToString()) + Prime.InRadians(90)));
          }
        }
        if (t.modelType == Prime.Patterns_e.OutHole && outHoles > 0) {
          OutHole outHole = (OutHole)t;
          outHole.GetCoordinateSystem();
          for (int h = 1; h <= outHoles; h++) {
            outHole.SetHoleNumber(h);
            double d = this.fileProperties.Diameter("_outHole" + h.ToString());
            // задава новото ъглово отметстване
            outHole.SetDimension(Prime.Name(outHole.nameAngle, outHole.namePlaneHole), this.RealAngle(this.fileProperties.Angle("_outHole" + h.ToString()) + Prime.InRadians(90)));
            // задава диаметъра на вътрешната помощна повърхнина
            //outHole.SetDimension(Prime.Name(outHole.nameDiamBody, outHole.nameInternalSurfaceSketch), this.diameter);
            // задава диаметъра на външната помощна повърхнина
            //outHole.SetDimension(Prime.Name(outHole.nameDiamBody, outHole.nameExternalSurfaceSketch), this.diameter + 2 * this.thickness + 0.02);
            // задава външния диаметър на овора
            outHole.SetDimension(Prime.Name(outHole.nameDiameter, outHole.nameBodySketch), this.fileProperties.Diameter("_outHole" + h.ToString()) + 2 * this.thickness);
          }
        }
        if (t.modelType == Prime.Patterns_e.InHole && inHoles > 0) {
          InHole inHole = (InHole)t;
          inHole.GetCoordinateSystem();
          for (int h = 1; h <= inHoles; h++) {
            inHole.SetHoleNumber(h);
            double d = this.fileProperties.Diameter("_inHole" + h.ToString());
            // задава новото ъглово отметстване
            inHole.SetDimension(Prime.Name(inHole.nameAngle, inHole.namePlaneHole), this.RealAngle(this.fileProperties.Angle("_inHole" + h.ToString()) + Prime.InRadians(90)));
            // задава диаметъра на вътрешната помощна повърхнина
            //inHole.SetDimension(Prime.Name(inHole.nameDiamBody, inHole.nameInternalSurfaceSketch), this.diameter - 2 * this.thickness - 0.02);
            // задава диаметъра на външната помощна повърхнина
            //inHole.SetDimension(Prime.Name(inHole.nameDiamBody, inHole.nameExternalSurfaceSketch), this.diameter);
            // задава външния диаметър на овора
            inHole.SetDimension(Prime.Name(inHole.nameDiameter, inHole.nameBodySketch), this.fileProperties.Diameter("_inHole" + h.ToString()) + 2 * this.thickness);
          }
        }
      }
      if (this.changeCS)
        this.changeCS = false;
      if (this.changeDT)
        this.changeDT = false;
    } // RebuildHoles
    #endregion Old Methods and Properties

  } // Mantel

}
