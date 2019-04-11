using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

using SolidWorks.Interop.swconst;

using SmartTools.Lib;
using SmartTools.XML;
using SolidWorks.Interop.sldworks;

namespace SmartTools.Patterns {
  /// <summary>създава модела на нормален ОТВОР</summary>
  [ComVisible(true)]
  public class Hole: Lib.PatternBase {

    #region Properyes
    #region Global Properties
    /// <summary>тип на модела в среда на SW</summary>
    public override swDocumentTypes_e swDocumentType {
      get { return swDocumentTypes_e.swDocNONE; }
    }
    /// <summary>тип намодела определен от приложението</summary>
    public override Prime.Patterns_e modelType {
      get { return Prime.Patterns_e.Hole; }
    }
    #endregion Global Properties
    #region Dimension Properties
    /// <summary>текуща стойност на ъгловото отместване</summary>
    public double curAngle { get; protected set; }
    #endregion Dimension Properties
    #region Default Values Controls
    /// <summary>стойност по подразбиране за диаметър, в метри</summary>
    public double defDiameter { get { return Prime.TEST ? 0.3 : 0; } }
    /// <summary>стойност по подразбиране за височина на отвора спрямо основата, в метри</summary>
    public double defHeight { get { return Prime.TEST ? 0.450 : 0; } }
    /// <summary>стойност по подразбиране за разстоянието до центъра, в метри</summary>
    public double defDistance { get { return Prime.TEST ? 0.500 : 0; } }
    /// <summary>стойност по подразбиране за ъгловото отместване спрямо координатната система на тялото, в радиани</summary>
    public double defAngularOffset { get { return Prime.TEST ? Prime.InRadians(120) : 0; } }
    /// <summary>модел на обекта върху който се прилага инструмента</summary>
    protected string pattern;
    #endregion Default Values Controls
    #region Feature Names
    /*
    /// <summary>наименование на построението</summary>
    public string name3DModel {
      get { return "HoleCut" + this.holeNumber.ToString(); }
    }*/
    #endregion Feature Names
    #endregion Properyes
    
    #region Override Methods
    /// <summary>проверява приложимостта на модела към съществуващата среда на SW</summary>
    /// <returns>TRUE модела е приложим, FALSE не може да се приложи</returns>
    public override Boolean Relevance() {
      string[] patterns = { Prime.Patterns_e.Mantel.ToString(), Prime.Patterns_e.Cover.ToString(), Prime.Patterns_e.Bottom.ToString() };
      this.FilePropetyTab();
      this.pattern = this.fileProperties.Pattern().ToLower();
      bool rel = false;
      foreach(string pat in patterns) {
        rel = rel | this.pattern.Equals(pat.ToLower());
      }
      return rel;
    } // Relevance

    /// <summary>поготвя за визуализация информацията за координатната система</summary>
    public override void GetCoordinateSystem() {
      this.FilePropetyTab();
      this.SetChecked(Prime.Controls_e.Clockwise, this.fileProperties.Clockwise());
      this.SetChecked(Prime.Controls_e.AntiClockwise, !this.fileProperties.Clockwise());
      string pos = this.fileProperties.Position0().ToLower();
      this.SetChecked(Prime.Controls_e.NortPosition, pos.Equals(Prime.Controls_e.NortPosition.ToString().ToLower()));
      this.SetChecked(Prime.Controls_e.EastPosition, pos.Equals(Prime.Controls_e.EastPosition.ToString().ToLower()));
      this.SetChecked(Prime.Controls_e.SoutPosition, pos.Equals(Prime.Controls_e.SoutPosition.ToString().ToLower()));
      this.SetChecked(Prime.Controls_e.WestPosition, pos.Equals(Prime.Controls_e.WestPosition.ToString().ToLower()));
      //this.pmpNumberbox[Prime.Controls_e.WeldingSeam].Value = this.fileProperties.WeldingSeam();
      // чете от характеристиките на модела номера на последния отвор
      this.FilePropetyTab();
      this.holeNumber = this.fileProperties.HoleNumber() + 1;
      if (this.pattern.Equals(Prime.Patterns_e.Mantel.ToString().ToLower())) {
        ((IPropertyManagerPageControl)this.pmpNumberbox[Prime.Controls_e.Distance]).Visible = false;
        ((IPropertyManagerPageControl)this.pmpLabel[(int)Prime.Controls_e.idLabel + 2]).Visible = false;
        this.pmpNumberbox[Prime.Controls_e.Distance].Value = 0;
        ((IPropertyManagerPageControl)this.pmpNumberbox[Prime.Controls_e.Height]).Visible = true;
        ((IPropertyManagerPageControl)this.pmpLabel[(int)Prime.Controls_e.idLabel + 1]).Visible = true;
      }
      if (this.pattern.Equals(Prime.Patterns_e.Cover.ToString().ToLower()) ||
          this.pattern.Equals(Prime.Patterns_e.Bottom.ToString().ToLower())) {
        ((IPropertyManagerPageControl)this.pmpNumberbox[Prime.Controls_e.Height]).Visible = false;
        ((IPropertyManagerPageControl)this.pmpLabel[(int)Prime.Controls_e.idLabel + 1]).Visible = false;
        this.pmpNumberbox[Prime.Controls_e.Height].Value = 0;
        ((IPropertyManagerPageControl)this.pmpNumberbox[Prime.Controls_e.Distance]).Visible = true;
        ((IPropertyManagerPageControl)this.pmpLabel[(int)Prime.Controls_e.idLabel + 2]).Visible = true;
      }
    } // GetCoordinateSystem

    /// <summary>добавя контроли в swPMPage</summary>
    public override void AddControlsOnPMPage() {
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Diameter, this.defDiameter);
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Height, this.defHeight);
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Distance, this.defDistance); ;
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.AngularOffset, this.defAngularOffset, swNumberboxUnitType_e.swNumberBox_Angle, 0, Prime.InRadians(360));
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.Clockwise, true, true, true, false);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.AntiClockwise, false, false, false, false);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.NortPosition, true, true, true, false);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.EastPosition, false, false, false, false);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.SoutPosition, false, false, false, false);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.WestPosition, false, false, false, false);
      // TODO: да се добави информация за заваръчния шев на обекта върху който се прилага инструмента
      //this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.WeldingSeam, 0, swNumberboxUnitType_e.swNumberBox_Angle);
    } // AddControlsOnPMPage

    /// <summary>изгражда 3D модела</summary>
    /// <returns>TRUE модела е изграден, FALSE проблем с параметрите</returns>
    public override Boolean Creating() {
      bool isOk = true;
      if (this.pattern.Equals(Prime.Patterns_e.Mantel.ToString().ToLower()))
        isOk = this.height > 0;
      if (this.diameter > 0 && isOk) {
        // създава 3D модела на отвора
        this.CreateHole();
        return true;
      }
      if (!(this.diameter > 0))
        Prime.ShowError(Prime.Controls_e.Diameter, this.modelType, System.Windows.Forms.MessageBoxButtons.OK);
      if (!(this.height > 0))
        Prime.ShowError(Prime.Controls_e.Height, this.modelType, System.Windows.Forms.MessageBoxButtons.OK);
      return false;
    } // Creating

    /// <summary>създава 3D модела на отвора</summary>
    protected void CreateHole() {
      if (this.pattern.ToLower().Equals(Prime.Patterns_e.Mantel.ToString().ToLower())) {
        // за мантела създаваме ранина завъртяна на 90 + ъгловото отместване на отвора, прямо RightPlane
        this.CreatePlane(this.swRightPlane, this.RealAngle(this.angularOffset + Prime.InRadians(90)), this.namePlaneHole, this.nameAngle);
        // създава вертикална скица за отвора
        this.SketchingVertical();
      } else {
        this.SketchingHorizontal();
      }
      // създава отвора с отнемане на материал
      this.CutExtrusion();
      // скрива работната равнина
      if (this.pattern.ToLower().Equals(Prime.Patterns_e.Mantel.ToString().ToLower())) {
        this.Selected(this.namePlaneHole, this.swPlane);
        this.swDocument.BlankRefGeom();
      }
      // преименува 3D модела
      this.RenameFeature(this.swCutExtrude, this.nameHole3D);
    } // CreateHole

    /// <summary>създава скицата за отвора във верикала</summary>
    protected void SketchingVertical() {
      // избира работната равнина
      this.SelectPlane(this.namePlaneHole);
      this.Circle(-this.height, 0, 0, this.diameter, this.nameDiameter, this.nameBodySketch);
      // привързва центъра на окръжноста към центъра на координатната система
      this.ClearSelection();
      this.Selected("Point2", this.swSketchPoint);
      this.Selected(this.swStartCS, this.swExtSketchPoint, true);
      this.swDocument.SketchAddConstraints(this.sgHorizontalPoints);
      // създава размер от центъра на координатната система до центъра на окръжноста
      this.ClearSelection();
      this.AddSize(this.swStartCS, this.swExtSketchPoint, this.nameHeight, this.diameter / 2, this.diameter / 2, 0, this.height, true, "Point2", this.swSketchPoint);
      this.InsertSketch();
    } // Sketching

    /// <summary>създава скица за отвор в Top Plane</summary>
    protected void SketchingHorizontal() {
      this.SelectPlane(this.swTopPlane);
      SelectData swSelData = (SelectData)this.swSelectionMgr.CreateSelectData();
      this.FilePropetyTab();
      double shoulder = this.fileProperties.Diameter() / 2 + this.fileProperties.Thickness() + 0.03;
      //double shoulder = this.diameter / 2 + this.thickness + 0.03;
      this.swSketchManager.CreateCenterLine(0, shoulder, 0, 0, -shoulder, 0);
      // координати на точката управляваща ъгловото отместване на отвора
      double x = Prime.SinFn(this.RealAngle(this.angularOffset), this.distance);
      double y = Prime.CosFn(this.RealAngle(this.angularOffset), this.distance);
      this.holePoint = this.swSketchManager.CreatePoint(x, y, 0);
      // отсточние на точката от центъра е this.distance
      this.AddDimensionPoint(this.holePoint, this.swStartCS, this.swExtSketchPoint, this.nameDistance);
      // създава помощна права за визуализация на ъгловото отместване
      this.ClearSelection();
      this.swSketchManager.CreateLine(0, 0, 0, x, y, 0);
      this.Selected("Line2", this.swSketchSegment);
      this.swSketchManager.CreateConstructionGeometry();
      this.ClearSelection();
      this.Selected("Line2", this.swSketchSegment);
      SketchLine l2 = (SketchLine)this.swSelectionMgr.GetSelectedObject6(1, -1);
      // крайната точка на правата и точката за управление на ъгловото отместване трябва да съвпадат
      this.ClearSelection();
      this.holePoint.Select4(false, swSelData);
      ((SketchPoint)l2.GetEndPoint2()).Select4(true, swSelData);
      this.swDocument.SketchAddConstraints(this.sgCoincident);
      // визуализира стойността на ъгловото отместване
      this.ClearSelection();
      this.Selected("Line2", this.swSketchSegment);
      this.Selected("Line1", this.swSketchSegment, true);
      DisplayDimension dim = this.swDocument.IAddDimension2(0.285, 0, -0.565);
      dim.GetDimension2(0).Name = this.nameAngle;
      // създава окръжност за изграждане на отвора
      this.ClearSelection();
      this.Circle(x, y, 0, this.diameter, this.nameDiameter, this.nameBodySketch);
      this.ClearSelection();
      bool status = this.Selected("Arc1", this.swSketchSegment);
      SketchArc arc = (SketchArc)this.swSelectionMgr.GetSelectedObject6(1, -1);
      this.ClearSelection();
      // центъра на окръжността и точката за управление на ъгловото отместване трябва да съвпадат
      this.holePoint.Select4(true, swSelData);
      ((SketchPoint)arc.GetCenterPoint2()).Select4(true, swSelData);
      this.swDocument.SketchAddConstraints(this.sgCoincident);
      this.InsertSketch();
      this.ClearSelection();
      this.holePoint.Select4(false, swSelData);
      this.Selected(this.swTopPlane, this.swPlane, true);
      this.swDocument.InsertAxis2(false);
      this.RenameFeature("Axis", this.nameAxis);
      this.Selected(this.nameAxis, this.swAxis);
      this.swDocument.BlankRefGeom();
    } // SketchingHorizontal





    /// <summary>съхранява характеристиките на отвора във File Properties</summary>
    public override void SetFileProperties() {
      this.FilePropetyTab();
      this.fileProperties.HoleNumber(this.holeNumber);
      this.fileProperties.Diameter(this.diameter, this.holeNumber.ToString());
      if (this.height > 0) {
        this.fileProperties.Height(this.height, this.holeNumber.ToString());
      } else {
        this.fileProperties.Distance(this.distance, this.holeNumber.ToString());
      }
      this.fileProperties.Angle(this.angularOffset, this.holeNumber.ToString());
    } // SetFileProperties

    /// <summary>създава папка с постоенията</summary>
    public override void CreateFolder() {
      if (this.GetFeature("Отвор" + this.holeNumber.ToString()) == null) {
        if (this.pattern.ToLower().Equals(Prime.Patterns_e.Mantel.ToString().ToLower())) {
          this.swDocument.Extension.SelectByID2(this.namePlaneHole, this.swPlane, 0, 0, 0, false, 0, null, 0);
          this.swDocument.FeatureManager.InsertFeatureTreeFolder2((int)swFeatureTreeFolderType_e.swFeatureTreeFolder_Containing);
          this.swDocument.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "Отвор" + this.holeNumber.ToString());
          this.swDocument.Extension.SelectByID2(this.nameHole3D, this.swBody, 0, 0, 0, false, 0, null, 0);
          this.swDocument.Extension.ReorderFeature(this.nameHole3D, this.namePlaneHole, (int)swMoveLocation_e.swMoveAfter);
        } else {
          this.Selected(this.nameHole3D, this.swBody);
          this.swFeatureManager.InsertFeatureTreeFolder2((int)swFeatureTreeFolderType_e.swFeatureTreeFolder_Containing);
          this.swDocument.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "Отвор" + this.holeNumber.ToString());
          this.swExtension.ReorderFeature(this.nameAxis, this.nameHole3D, (int)swMoveLocation_e.swMoveAfter);
        }
        this.ClearSelection();
      }
    } // CreateFolder

    /// <summary>установява стойностите на размерите от въведените</summary>
    public override void SetControls() {
      if (this.pattern.ToLower().Equals(Prime.Patterns_e.Mantel.ToString().ToLower())) {
        this.SetDimension(Prime.Name(this.nameDiameter, this.nameBodySketch), this.diameter);
        if (this.height > 0)
          this.SetDimension(Prime.Name(this.nameHeight, this.nameBodySketch), this.height);
        this.SetDimension(Prime.Name(this.nameAngle, this.namePlaneHole), this.RealAngle(this.angularOffset + Prime.InRadians(90)));
      }
      if (this.pattern.ToLower().Equals(Prime.Patterns_e.Cover.ToString().ToLower()) ||
        this.pattern.ToLower().Equals(Prime.Patterns_e.Bottom.ToString().ToLower())) {
        this.SetDimension(Prime.Name(this.nameDiameter, this.nameBodySketch), this.diameter);
        this.SetDimension(Prime.Name(this.nameAngle, this.nameBodySketch), this.RealAngle(this.angularOffset));
        this.SetDimension(Prime.Name(this.nameDistance, this.nameBodySketch), this.distance);
      }
    } // SetControls

    public override void EditDocumentSW(bool defValues = false) {
      //throw new NotImplementedException();
    } // EditDocumentSW

    public override String MaterialName() {
      throw new NotImplementedException();
    } // MaterialName
    #endregion Override Methods

    #region Protected methods

    /// <summary>създава отвора с отнемане на материал</summary>
    protected void CutExtrusion() {
      // избира скицата за отвора
      this.swExtension.SelectByID2(this.nameBodySketch, this.swSketch, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
      bool dir = true;
      if (this.pattern.ToLower().Equals(Prime.Patterns_e.Bottom.ToString().ToLower()) ||
        (this.pattern.ToLower().Equals(Prime.Patterns_e.Mantel.ToString().ToLower())))
        dir = false;
      // изгражда отвора с отнемане на материал
      this.swFeatureManager.FeatureCut4(true, false, dir,
        (int)swEndConditions_e.swEndCondThroughAll, 0, 0, 0,
        false, false, false, false, 0, 0, false, false, false, false, false,
        true, true, true, true, false,
        (int)swStartConditions_e.swStartSketchPlane, 0, false, false);
      this.ClearSelection();
    } // CutExtrusion
    #endregion Protected methods

    public void SetHoleNumber(int number) {
      this.holeNumber = number;
    } // SetHoleNumber

  } // Hole

}
