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
  /// <summary>Създава модела КАПАК</summary>
  [ComVisible(true)]
  public class Cover : Lib.PatternBase {

    /*
    /// <summary>Изпълнява модела.</summary>
    /// <date>2019-04-10</date>
    /// <param name="mode">начин на изпълнение, True нов модел, False редактиране на съществуващ модел</param>
    public override void Execute(bool mode = true) {
      this.swDocument = Prime.AddIn.SwApp.IActiveDoc2;
      if (mode) {
        if (!this.IsEmptyDoc())
          this.swDocument = Prime.AddIn.SwApp.INewDocument2(this.DeafultName(),
            (int)swDwgPaperSizes_e.swDwgPaperA3size, 0, 0);
      }
      if (this.swDocument != null)
        this.swPMPage.Show2(0);
    } // Execute
    
    /// <summary>Изгражда/Възстановява модела</summary>
    public override void Rebuild() {
      this.FilePropetyTab();
      string pattern = this.fileProperties.Pattern();
      if (pattern == null) {
        if (!this.CreateDocSW())
          return;
      } else {
        this.ChangesDimensions();
      }
      // съхранява характеристиките във File Properties
      this.SetFileProperties();
      this.swExtension.ForceRebuildAll();
      this.swDocument.ViewZoomtofit2();
    } // Rebuild
    */
    #region Global Properties
    /// <summary>тип на модела определен от SW</summary>
    /// <date>2019-04-08</date>
    public override swDocumentTypes_e swDocumentType {
      get { return swDocumentTypes_e.swDocPART; }
    }
    /// <summary>тип на модела определен от приложението</summary>
    /// <date>2019-04-08</date>
    public override Prime.Patterns_e modelType {
      get { return Prime.Patterns_e.Cover; }
    }
    #endregion Global Properties

    #region Default Values Controls
    /// <summary>стойност по подразбиране за диаметър, в метри</summary>
    /// <date>2019-04-08</date>
    public double defDiameter { get { return Prime.TEST ? 2 : 0; } }
    /// <summary>стойност по подразбиране за ъгъла на наклона, в градуси</summary>
    /// <date>2019-04-08</date>
    public double defAngleIinclination { get { return Prime.TEST ? Prime.InRadians(25) : 0; } }
    /// <summary>стойност по подразбиране за дебелината на стената, в метри</summary>
    /// <date>2019-04-08</date>
    public double defThickness { get { return Prime.TEST ? 0.005 : 0; } }
    /// <summary>стойност по подразбиране за радиуса на бертване, в метри</summary>
    /// <date>2019-04-08</date>
    public double defBendRadius { get { return Prime.TEST ? 0.05 : 0; } }
    /// <summary>стйност по подразбиране за ъгъла на заваръчния шев, в радиани</summary>
    /// <date>2019-04-08</date>
    public double defAngle { get { return Prime.TEST ? Prime.InRadians(135) : 0; } }
    /// <summary>стойност по подразбиране за посоката на нарастване на ъгълав координатната система</summary>
    /// <date>2019-04-10</date>
    public Prime.Controls_e defClockwise { get { return Prime.Controls_e.Clockwise; } }
    /// <summary>стойност по подразбиране за позицията на 0 градуса в координатната система</summary>
    /// <date>2019-04-10</date>
    public Prime.Controls_e defPosition0 { get { return Prime.Controls_e.NortPosition; } }
    #endregion Default Values Controls

    /// <summary>проверява приложимостта на модела към съществуващата среда на SW</summary>
    /// <date>2019-04-08</date>
    /// <returns>TRUE модела е приложим, FALSE не може да се приложи</returns>
    public override Boolean Relevance() {
      return true;
    } // Relevance

    /// <summary>добавя контроли в swPMPage за въвеждане на параметрите на КАПАК </summary>
    /// <date>2019-04-08</date>
    public override void AddControlsOnPMPage() {
      // контроли за избор на координатна система
      this.AddCoordinateSystem(this.defClockwise, this.defPosition0, true);
      // контрола за избор на материал
      XML.XMLData data = new XML.XMLMaterials();
      this.AddCombobox(Prime.Groups_e.Characteristics, 
            Prime.Controls_e.Material, ((XMLMaterials)data).GetMaterials());
      // контроли за въвеждане габаритните размери
      this.AddNumberbox(Prime.Groups_e.Dimension, 
                               Prime.Controls_e.Diameter, this.defDiameter);
      this.AddNumberbox(Prime.Groups_e.Dimension, 
        Prime.Controls_e.AngleInclination, this.defAngleIinclination, 
           swNumberboxUnitType_e.swNumberBox_Angle, 0, Prime.InRadians(80));
      this.AddNumberbox(Prime.Groups_e.Dimension, 
                             Prime.Controls_e.Thickness, this.defThickness);
      this.AddNumberbox(Prime.Groups_e.Dimension, 
                           Prime.Controls_e.BendRadius, this.defBendRadius);
      this.AddNumberbox(Prime.Groups_e.Dimension, 
        Prime.Controls_e.WeldingSeam, this.defAngle, 
          swNumberboxUnitType_e.swNumberBox_Angle, 0, Prime.InRadians(360));
      // контроли за избор на допълнителни характеристики
      data = new XML.XMLCertificates();
      this.AddCombobox(Prime.Groups_e.Complement, 
        Prime.Controls_e.Certificate, 
                                 ((XMLCertificates)data).GetCertificates());
      data = new XML.XMLTests();
      this.AddCombobox(Prime.Groups_e.Complement, Prime.Controls_e.Test, 
                                               ((XMLTests)data).GetTests());
    } // AddControlsOnPMPage

    /// <summary>изгражда 3D модел на КАПАК</summary>
    /// <returns></returns>
    public override Boolean CreateDocSW() {
      // въведени ли са габаритни размери за диаметър, дебелина и радиус на бертване
      if (!(this.diameter > 0 && this.thickness > 0 && 
                        this.bendRadius > 0 && this.angleInclination > 0)) {
        if (!(this.diameter > 0))
          Prime.ShowError(Prime.Controls_e.Diameter, this.modelType, 
                                 System.Windows.Forms.MessageBoxButtons.OK);
        if (!(this.thickness > 0))
          Prime.ShowError(Prime.Controls_e.Thickness, this.modelType, 
                                 System.Windows.Forms.MessageBoxButtons.OK);
        if (!(this.angleInclination > 0))
          Prime.ShowError(Prime.Controls_e.AngleInclination, this.modelType, 
                                 System.Windows.Forms.MessageBoxButtons.OK);
        if (!(this.bendRadius > 0))
          Prime.ShowError(Prime.Controls_e.BendRadius, 
                 this.modelType, System.Windows.Forms.MessageBoxButtons.OK);
        return false;
      }
      // създава оста на обекта
      this.Axis(this.swRightPlane, this.swFrontPlane, this.nameAxis);
      // създава 3D модела на капака
      this.CreateConeBody(true);
      // създава и визуализира заваръчния шев
      this.CreateWeldSeam(this.shoulderWeldingSeam, this.nameBodyWS);
      return true;
    } // CreateDocSW

    /// <summary>
    /// променя стойностите на размерите на модела, получени от контролите в swPMPage
    /// </summary>
    /// <date>2019-04-10</date>
    public override void ChangesDimensions() {
      this.SetDimension(this.FullName(this.nameBodyRadius, this.nameBodySketch), this.diameter / 2);
      this.SetDimension(this.FullName(this.nameAngleInclination, this.nameBodySketch), this.angleInclination);
      this.SetDimension(this.FullName(this.nameThickness, this.nameBody), this.thickness);
      this.SetDimension(this.FullName(this.nameBendRadius, this.nameBodySketch), this.bendRadius);
      this.SetDimension(this.FullName(this.nameExplodeExternal, this.nameBodySketch), Prime.HEIHGT_EXPLODE_HOLE + this.thickness);
      this.SetDimension(this.FullName(this.nameExplodeMidle, this.nameBodySketch), this.thickness);
      this.SetViewWeldSeam(this.shoulderWeldingSeam);
      this.RebuildHoles();
    } // ChangesDimensions

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
      status = this.Selected(this.FullName("Arc1", this.nameBodySketch), this.swExtSketchSegment, true);
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

    

    

    /// <summary>флаг за промяна на координатната система</summary>
    protected bool changeCS = false;
    /// <summary>флаг за промяна на диаметъра и/или дебелината</summary>
    protected bool changeDT = false;

    #endregion Properties

    #region Override Methods
    

    /// <summary>изгражда 3D модела на КАПАК</summary>
    /// <returns>TRUE моделът е изграден, FALSE проблем с параметрите</returns>
    public override Boolean Creating() {
      // проверява въведени ли са важните габаритни размери, диаметър, дебелина и радиус на бертване
      if (this.diameter > 0 &&  this.thickness > 0 && this.bendRadius > 0) {
        // създава оста на тялото
        this.Axis(this.swRightPlane, this.swFrontPlane, this.nameAxis);
        // създава 3D модел на КАПАК
        this.CreateConeBody(true);
        // създава визуализация на заваръчния шев
        this.CreateWeldSeam(this.shoulderWeldingSeam, this.nameBodyWS);
        return true;
      }
      if (!(this.diameter > 0))
        Prime.ShowError(Prime.Controls_e.Diameter, this.modelType, System.Windows.Forms.MessageBoxButtons.OK);
      if (!(this.thickness > 0))
        Prime.ShowError(Prime.Controls_e.Thickness, this.modelType, System.Windows.Forms.MessageBoxButtons.OK);
      if (!(this.angleInclination > 0))
        Prime.ShowError(Prime.Controls_e.AngleInclination, this.modelType, System.Windows.Forms.MessageBoxButtons.OK);
      if (!(this.bendRadius > 0))
        Prime.ShowError(Prime.Controls_e.BendRadius, this.modelType, System.Windows.Forms.MessageBoxButtons.OK);
      return false;
    } // Creating

    /// <summary>установява стойностите на размерите от въведените</summary>
    public override void SetControls() {
      this.SetDimension(Prime.Name(this.nameBodyRadius, this.nameBodySketch), this.diameter / 2);
      this.SetDimension(Prime.Name(this.nameAngleInclination, this.nameBodySketch), this.angleInclination);
      this.SetDimension(Prime.Name(this.nameBendRadius, this.nameBodySketch), this.bendRadius);
      this.SetDimension(Prime.Name(this.nameThickness, this.nameBody), this.thickness);
      this.SetViewWeldSeam(this.shoulderWeldingSeam);
      this.RebuildHoles();
    } // SetControls

    #endregion Override Methods

    #region Protected Methods
    
    /// <summary>съхранява характеристиките на модела във File Properties</summary>
    public override void SetFileProperties() {
      this.FilePropetyTab();
      this.fileProperties.Header(this.modelType);
      this.fileProperties.Characteristics(this.Value(Prime.Controls_e.Material).ToString());
      this.fileProperties.CoordinateSystem((bool)this.Value(Prime.Controls_e.Clockwise),
        (bool)this.Value(Prime.Controls_e.NortPosition), (bool)this.Value(Prime.Controls_e.EastPosition),
        (bool)this.Value(Prime.Controls_e.SoutPosition), (bool)this.Value(Prime.Controls_e.WestPosition));
      this.fileProperties.Dimension(
        diameter: this.diameter,
        thickness: this.thickness,
        angleInclination: this.angleInclination,
        bendRadius: this.bendRadius,
        weldingSeam: this.weldSeamAngle);
      this.fileProperties.Complements(this.Value(Prime.Controls_e.Certificate).ToString(), this.Value(Prime.Controls_e.Test).ToString());
      this.fileProperties.InnerDiameter(true);
      this.FilePropetyTab(Properties.CONFIGURATION_SPECIFIC);
      string info = "Конус ф" +
        ((this.diameter + 2 * this.thickness) * 1000).ToString() + "x" +
        (this.thickness * 1000).ToString() + " а" + Prime.InDegrees(this.angleInclination).ToString();
      this.fileProperties.Description(info);
      this.fileProperties.Complements(this.Value(Prime.Controls_e.Certificate).ToString(), this.Value(Prime.Controls_e.Test).ToString());
    } // SetFileProperties

    /// <summary>Създава папка на построенията</summary>
    public override void CreateFolder() {
      if (!this.RenameFeature("Капак", null)) {
        this.swExtension.SelectByID2(this.nameAxis, this.swAxis, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        Feature feature = this.swDocument.FeatureManager.InsertFeatureTreeFolder2((int)swFeatureTreeFolderType_e.swFeatureTreeFolder_Containing);
        this.swDocument.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "Капак");
        this.swExtension.ReorderFeature(this.nameBody, this.nameAxis, (int)swMoveLocation_e.swMoveAfter);
        this.swExtension.ReorderFeature(this.nameBodyWS, this.nameBody, (int)swMoveLocation_e.swMoveAfter);
        this.swDocument.ClearSelection2(true);
      }
    } // CreateFolder

    

    /// <summary>връща наименованието на избрания материал</summary>
    /// <returns>избран материал</returns>
    public override String MaterialName() {
      return this.Value(Prime.Controls_e.Material).ToString();
    } // MaterialName

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
        this.SetValue(Prime.Controls_e.AngleInclination, this.fileProperties.AngleInclination());
        this.SetValue(Prime.Controls_e.WeldingSeam, this.fileProperties.WeldingSeam());
        this.SetValue(Prime.Controls_e.BendRadius, this.fileProperties.BendRadius());
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
        this.SetValue(Prime.Controls_e.AngleInclination, this.defAngleIinclination);
        this.SetValue(Prime.Controls_e.WeldingSeam, this.defAngle);
        this.SetValue(Prime.Controls_e.BendRadius, this.defBendRadius);
        this.SetSelection(Prime.Controls_e.Certificate, 0);
        this.SetSelection(Prime.Controls_e.Test, 0);
      }
    } // EditDocumentSW

    /// <summary>установява флаг за промяна на координатната система</summary>
    public override void RadioButtonChek() {
      this.changeCS = true;
    } // RadioButtonChek

    /// <summary>установява флаг за промяна на диаметър и/или дебелина</summary>
    public override void DiameterOrThicknessChange() {
      this.changeDT = true;
    } // DiameterOrThicknessChange

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
            hole.SetDimension(Prime.Name(hole.nameAngle, hole.nameBodySketch), this.RealAngle(this.fileProperties.Angle(k.ToString())));
          }
        }
        if (t.modelType == Prime.Patterns_e.OutHole && outHoles > 0) {
          OutHole outHole = (OutHole)t;
          outHole.GetCoordinateSystem();
          for (int h = 1; h <= outHoles; h++) {
            outHole.SetHoleNumber(h);
            double d = this.fileProperties.Diameter("_outHole" + h.ToString());
            // задава новото ъглово отметстване
            outHole.SetDimension(Prime.Name(outHole.nameAngle, outHole.nameBodySketch), this.RealAngle(this.fileProperties.Angle("_outHole" + h.ToString())));
            // задава диаметъра на вътрешната помощна повърхнина
            //outHole.SetDimension(Prime.Name(outHole.nameDiamBody, outHole.nameInternalSurfaceSketch), this.diameter);
            // задава диаметъра на външната помощна повърхнина
            //outHole.SetDimension(Prime.Name(outHole.nameDiamBody, outHole.nameExternalSurfaceSketch), this.diameter + 2 * this.thickness + 0.02);
            // задава външния диаметър на овора
            //outHole.SetDimension(Prime.Name(outHole.nameDiameter, outHole.nameBodySketch), this.fileProperties.Diameter("_outHole" + h.ToString()) + 2 * this.thickness);
          }
        }
        if (t.modelType == Prime.Patterns_e.InHole && inHoles > 0) {
          InHole inHole = (InHole)t;
          inHole.GetCoordinateSystem();
          for (int h = 1; h <= inHoles; h++) {
            inHole.SetHoleNumber(h);
            double d = this.fileProperties.Diameter("_inHole" + h.ToString());
            // задава новото ъглово отметстване
            inHole.SetDimension(Prime.Name(inHole.nameAngle, inHole.nameBodySketch), this.RealAngle(this.fileProperties.Angle("_inHole" + h.ToString())));
            // задава диаметъра на вътрешната помощна повърхнина
            //inHole.SetDimension(Prime.Name(inHole.nameDiamBody, inHole.nameInternalSurfaceSketch), this.diameter - 2 * this.thickness - 0.02);
            // задава диаметъра на външната помощна повърхнина
            //inHole.SetDimension(Prime.Name(inHole.nameDiamBody, inHole.nameExternalSurfaceSketch), this.diameter);
            // задава външния диаметър на овора
            //inHole.SetDimension(Prime.Name(inHole.nameDiameter, inHole.nameBodySketch), this.fileProperties.Diameter("_inHole" + h.ToString()) + 2 * this.thickness);
          }
        }
      }
      if (this.changeCS)
        this.changeCS = false;
      if (this.changeDT)
        this.changeDT = false;
    } // RebuildHoles
    #endregion Protected Methods

  } // Cover

}
