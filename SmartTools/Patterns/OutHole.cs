using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

using SolidWorks.Interop.swconst;

using SmartTools.Lib;
using SolidWorks.Interop.sldworks;

namespace SmartTools.Patterns {
  /// <summary>създава модел на избушен отвор навън</summary>
  [ComVisible(true)]
  public class OutHole: Lib.PatternBase {

    /// <summary>създава помощни повърхнини</summary>
    /// <date>2019-04-09</date>
    protected void CreateSurfaces() {
      PatternBase pBase = this.GetPatternByName(this.pattern);
      if (pBase != null) {
        pBase.CreateSurface(0, false, this.nameInternalSurface,
                                          this.nameInternalSurfaceSketch);
        pBase.CreateSurface(0.01, true, this.nameExternalSurface, 
                                          this.nameExternalSurfaceSketch);
      }
    } // CreateSurfaces

    #region Properties
    #region Global Properties
    /// <summary>тип на модела в среда на SW</summary>
    public override swDocumentTypes_e swDocumentType {
      get { return swDocumentTypes_e.swDocNONE; }
    }
    /// <summary>тип на модела определен от приложението</summary>
    public override Prime.Patterns_e modelType {
      get { return Prime.Patterns_e.OutHole; }
    }
    #endregion Global Properties
    #region Dimension Properties
    /// <summary>текуща стойност на ъгловото отместване</summary>
    public double curAngle { get; protected set; }
    /// <summary>дебелина на стената на тялото върху което се изгражда отвора</summary>
    public double thinBody { get; protected set; }
    /// <summary>диаметър на тялото върху което се изгражда отвора</summary>
    public double diamBody { get; protected set; }
    /// <summary>височина на тялото вурху което се изгражда отвора</summary>
    public double heightBody { get; protected set; }
    #endregion Dimension Properties
    #region Default Values Controls
    /// <summary>стойност по подразбиране за диаметър, в метри</summary>
    public double defDiameter { get { return Prime.TEST ? 0.400 : 0; } }
    /// <summary>стойност по подразбиране за височина на отвора спрямо основата, в метри</summary>
    public double defHeight { get { return Prime.TEST ? 0.450 : 0; } }
    /// <summary>стойност по подразбиране за ъгловото отместване спрямо координатната система на тялото, в радиани</summary>
    public double defAngularOffset { get { return Prime.TEST ? Prime.InRadians(95) : 0; } }
    /// <summary>стойност по подразбиране за разстоянието до центъра, в метри</summary>
    public double defDistance { get { return 0.500; } }
    /// <summary>модел на обекта върху който се прилага инструмента</summary>
    protected string pattern;
    #endregion Default Values Controls
    #region Feature Name
    /// <summary>наименование на равнина в която ще се изгради скицата за отвора</summary>
    //public string namePlane {
    //  get { return "OutHole" + this.holeNumber.ToString(); }
    //}
    // <summary>наименование на скицата на отвора</summary>
    //public string nameSketch {
    //  get { return "OutHoleSketch" + this.holeNumber.ToString(); }
    //}
    /// <summary>наименование на диаметъра на тялото върху което се изгражда отвора</summary>
    public string nameDiamBody {
      get { return "DiamBodyOutHole" + this.holeNumber.ToString(); }
    }
    
    
    
    
    
    /// <summary>наименование на тялото оформящо външните размери на отвора</summary>
    public string nameHoleExternal {
      get { return "outHoleExternal" + this.holeNumber.ToString(); }
    }
    /// <summary>наименование на тялото премахващо материал от оста до вътрешната стена</summary>
    public string nameHoleCut {
      get { return "outHoleCut" + this.holeNumber.ToString(); }
    }
    
    /// <summary>наименование на тялото премхващо материал от стената на тялото</summary>
    public string nameHoleEnd {
      get { return "outHoleEnd" + this.holeNumber.ToString(); }
    }
    #endregion Feature Name
    #endregion Properties

    #region Override Methods
    /// <summary>проверява приложимостта на модела към съществуващата среда на SW</summary>
    /// <returns>TRUE модела е приложим, FALSE не е приложим</returns>
    public override bool Relevance() {
     string[] patterns = { Prime.Patterns_e.Mantel.ToString(), Prime.Patterns_e.Cover.ToString(), Prime.Patterns_e.Bottom.ToString() };
      this.FilePropetyTab();
      this.pattern = this.fileProperties.Pattern().ToLower();
      bool rel = false;
      foreach (string pat in patterns) {
        rel = rel | this.pattern.Equals(pat.ToLower());
      }
      return rel;
    } // Relevance

    /// <summary>добавя контролите в swPMPage</summary>
    public override void AddControlsOnPMPage() {
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Diameter, this.defDiameter);
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Height, this.defHeight);
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.Distance, this.defDistance);
      this.AddNumberbox(Prime.Groups_e.Dimension, Prime.Controls_e.AngularOffset, this.defAngularOffset, swNumberboxUnitType_e.swNumberBox_Angle, 0, Prime.InRadians(360));
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.Clockwise, true, true, true, false);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.AntiClockwise, false, false, false, false);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.NortPosition, true, true, true, false);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.EastPosition, false, false, false, false);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.SoutPosition, false, false, false, false);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem, Prime.Controls_e.WestPosition, false, false, false, false);
    } // AddControlsOnPMPage

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
      // чете от характеристиките на модела номера на последния отвор
      this.FilePropetyTab();
      this.holeNumber = this.fileProperties.OutHoleNumber() + 1;
      this.thinBody = this.fileProperties.Thickness();
      this.diamBody = this.fileProperties.Diameter();
      this.heightBody = this.fileProperties.Height();
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

    /// <summary>изгражда 3D модела</summary>
    /// <returns>TRUE модела е изграден, FALSE проблем с параметрите</returns>
    public override bool Creating() {
      bool isOk = true;
      if (this.pattern.Equals(Prime.Patterns_e.Mantel.ToString().ToLower()))
        isOk = this.height > 0;
      if (this.diameter > 0 && isOk) {
        // създава работни повърхнини около тялото върху което се изгражда отвора
        this.CreateSurfaces();
        this.CreateHole();
        // скрива работните повърхнини и елементи
        this.ClearSelection();
        if (this.pattern.Equals(Prime.Patterns_e.Mantel.ToString().ToLower())) {
          this.ClearSelection();
          this.swExtension.SelectByID2(this.namePlaneHole, this.swPlane, 0, 0, 0, false, 1, null, (int)swSelectOption_e.swSelectOptionDefault);
          this.swDocument.BlankRefGeom();
        }
        this.Selected(this.nameInternalSurface, this.swRefSurface);
        this.swFeatureManager.HideBodies();
        this.ClearSelection();
        this.Selected(this.nameExternalSurface, this.swRefSurface);
        this.swFeatureManager.HideBodies();
        return true;
      }
      if (!(this.diameter > 0))
        Prime.ShowError(Prime.Controls_e.Diameter, this.modelType, System.Windows.Forms.MessageBoxButtons.OK);
      if (!(this.height > 0))
        Prime.ShowError(Prime.Controls_e.Height, this.modelType, System.Windows.Forms.MessageBoxButtons.OK);
      return false;
    } // Creating

    /// <summary>съхранява характеристиките на отвора във File Propertyes</summary>
    public override void SetFileProperties() {
      this.FilePropetyTab();
      this.fileProperties.OutHoleNumber(this.holeNumber);
      this.fileProperties.Diameter(this.diameter, "_outHole" + this.holeNumber.ToString());
      this.fileProperties.Height(this.height, "_outHole" + this.holeNumber.ToString());
      this.fileProperties.Angle(this.angularOffset, "_outHole" + this.holeNumber.ToString());
    } // SetFileProperties

    /// <summary>създава папка с постоенията</summary>
    public override void CreateFolder() {
      if (this.GetFeature("ИзВнОтвор" + this.holeNumber.ToString()) == null) {
        bool status = this.swExtension.SelectByID2(this.nameInternalSurface, this.swBody, 0, 0, 0, false, 0, null, 0);
        if (!status)
          status = this.swExtension.SelectByID2(this.nameInternalSurface, "REFSURFACE", 0, 0, 0, false, 0, null, 0);
        Feature feature = this.swFeatureManager.InsertFeatureTreeFolder2((int)swFeatureTreeFolderType_e.swFeatureTreeFolder_Containing);
        this.swDocument.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "ИзВнОтвор" + this.holeNumber.ToString());
        this.swExtension.SelectByID2(this.nameExternalSurface, this.swBody, 0, 0, 0, false, 0, null, 0);
        this.swExtension.ReorderFeature(this.nameExternalSurface, this.nameInternalSurface, (int)swMoveLocation_e.swMoveAfter);
        status = this.swExtension.SelectByID2(this.namePlaneHole, this.swPlane, 0, 0, 0, false, 0, null, 0);
        if (status) {
          this.swExtension.ReorderFeature(this.namePlaneHole, this.nameExternalSurface, (int)swMoveLocation_e.swMoveAfter);
        }
        this.swExtension.SelectByID2(this.nameHoleExternal, this.swBody, 0, 0, 0, false, 0, null, 0);
        if (status) {
          this.swExtension.ReorderFeature(this.nameHoleExternal, this.namePlaneHole, (int)swMoveLocation_e.swMoveAfter);
        } else {
          this.swExtension.ReorderFeature(this.nameHoleExternal, this.nameExternalSurface, (int)swMoveLocation_e.swMoveAfter);
        }
        this.swExtension.SelectByID2(this.nameHoleCut, this.swBody, 0, 0, 0, false, 0, null, 0);
        this.swExtension.ReorderFeature(this.nameHoleCut, this.nameHoleExternal, (int)swMoveLocation_e.swMoveAfter);
        this.swExtension.SelectByID2(this.nameHoleEnd, this.swBody, 0, 0, 0, false, 0, null, 0);
        this.swExtension.ReorderFeature(this.nameHoleEnd, this.nameHoleCut, (int)swMoveLocation_e.swMoveAfter);
      }
    } // CreateFolder

    /// <summary>установява стойностите на размерите от въведените</summary>
    public override void SetControls() {
      if (this.pattern.ToLower().Equals(Prime.Patterns_e.Mantel.ToString().ToLower())) {
        this.SetDimension(this.FullName(this.nameDiameter, this.nameBodySketch), this.diameter + 2 * this.thinBody);
        this.SetDimension(this.FullName(this.nameDiameter, this.nameHoleEnd), this.defDiameter);
        this.SetDimension(this.FullName(this.nameHeight, this.nameBodySketch), this.height);
        this.SetDimension(this.FullName(this.nameAngle, this.namePlaneHole), this.RealAngle(this.angularOffset) + Prime.InRadians(90));
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
    }

    public override String MaterialName() {
      //throw new NotImplementedException();
      return "";
    }
    #endregion Override Methods

    #region Protected Methods
    /*
    protected void CreateSurfaces() {
      this.CreateInternalSurface();
      this.CreateExternalSurface();
    } // CreateSurfaces
    */
    

    /// <summary>създава помощна повърхнина на разтояние 10mm от външната стена</summary>
    protected void CreateExternalSurface() {
      if (this.pattern.Equals(Prime.Patterns_e.Mantel.ToString().ToLower())) {
        Mantel mantel = (Mantel)this.GetPatternByName(this.pattern);
        mantel.CreateSurface(0.01, true, this.nameExternalSurface, this.nameExternalSurfaceSketch);
      } else {
        PatternBase pBase = this.GetPatternByName(this.pattern);
        if (this.pattern.Equals(Prime.Patterns_e.Cover.ToString().ToLower())) {
          ((Cover)pBase).CreateSurface(0.01, true, this.nameExternalSurface, this.nameExternalSurfaceSketch);
        } else {
          ((Bottom)pBase).CreateSurface(0.01, true, this.nameExternalSurface, this.nameExternalSurfaceSketch);
        }
      }
    } // CreateExternalSurface

    protected void CreateInternalSurface() {
      if (this.pattern.Equals(Prime.Patterns_e.Mantel.ToString().ToLower())) {
        Mantel mantel = (Mantel)this.GetPatternByName(this.pattern);
        mantel.CreateSurface(0, true, this.nameInternalSurface, this.nameInternalSurfaceSketch);
      } else {
        PatternBase pBase = this.GetPatternByName(this.pattern);
        if (this.pattern.Equals(Prime.Patterns_e.Cover.ToString().ToLower())) {
          ((Cover)pBase).CreateSurface(0, true, this.nameInternalSurface, this.nameInternalSurfaceSketch);
        } else {
          ((Bottom)pBase).CreateSurface(0, true, this.nameInternalSurface, this.nameInternalSurfaceSketch);
        }
      }
    } // CreateExternalSurface

    /// <summary>създава 3D модела на отвора</summary>
    protected void CreateHole() {
      if (this.pattern.Equals(Prime.Patterns_e.Mantel.ToString().ToLower())) {
        this.CreatePlane(this.swRightPlane, this.RealAngle(this.angularOffset + Prime.InRadians(90)), this.namePlaneHole, this.nameAngle);
        this.SelectPlane(this.namePlaneHole);
        this.Circle(-this.height, 0, 0, this.diameter + 2 * this.thinBody, this.nameDiameter, this.nameBodySketch);
        this.ClearSelection();
        this.Selected("Point2", this.swSketchPoint);
        this.Selected(this.swStartCS, this.swExtSketchPoint, true);
        this.swDocument.SketchAddConstraints(this.sgHorizontalPoints);
        // създава размер от центъра на координатната система до центъра на окръжноста
        this.ClearSelection();
        this.AddSize(this.swStartCS, this.swExtSketchPoint, this.nameHeight, this.diameter / 2, this.diameter / 2, 0, this.height, true, "Point2", this.swSketchPoint);
        this.InsertSketch();
      } else {
        this.SelectPlane(this.swTopPlane);
        SelectData swSelData = (SelectData)this.swSelectionMgr.CreateSelectData();
        double shoulder = this.diamBody / 2 + this.thinBody + 0.03;
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
        this.Circle(x, y, 0, this.diameter + 2 * this.thinBody, this.nameDiameter, this.nameBodySketch);
        this.ClearSelection();
        bool status = this.Selected("Arc1", this.swSketchSegment);
        SketchArc arc = (SketchArc)this.swSelectionMgr.GetSelectedObject6(1, -1);
        this.ClearSelection();
        // центъра на окръжността и точката за управление на ъгловото отместване трябва да съвпадат
        this.holePoint.Select4(true, swSelData);
        ((SketchPoint)arc.GetCenterPoint2()).Select4(true, swSelData);
        this.swDocument.SketchAddConstraints(this.sgCoincident);
        this.InsertSketch();
      }
      this.CutExtrusion();
      this.EndHole();
    } // CreateHold

    /// <summary>създава повърхнини около тялото върху което ще се изгражда отвора</summary>
    protected void CreatingSurfaces() {
      if (this.pattern.Equals(Prime.Patterns_e.Mantel.ToString().ToLower())) {

        this.SelectPlane(this.swTopPlane);
        // изгражда помощна повърхнина по вътрешната стена на тялото
        this.Circle(0, 0, 0, this.diamBody, this.nameDiamBody, this.nameInternalSurfaceSketch);
        this.swSketchManager.InsertSketch(true);
        this.ClearSelection();
        this.swExtension.SelectByID2(this.nameInternalSurfaceSketch, this.swSketch, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        this.swFeatureManager.FeatureExtruRefSurface3(true, false,
          (int)swStartConditions_e.swStartSketchPlane, 0,
          (int)swEndConditions_e.swEndCondBlind, 0, this.heightBody, 0,
          false, false, false, false, 0, 0, false, false, false, false, false, false, false, false);
        this.RenameFeature("Surface-Extrude", this.nameInternalSurface);
        // изгражда момощна повърхнина на отстояние 10mm от външната стена
        this.ClearSelection();
        this.SelectPlane(this.swTopPlane);
        this.Circle(0, 0, 0, this.diamBody + 2 * this.thinBody + 0.02, this.nameDiamBody, this.nameExternalSurfaceSketch);
        this.swSketchManager.InsertSketch(true);
        this.ClearSelection();
        this.swExtension.SelectByID2(this.nameExternalSurfaceSketch, this.swSketch, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        this.swFeatureManager.FeatureExtruRefSurface3(true, false,
          (int)swStartConditions_e.swStartSketchPlane, 0,
          (int)swEndConditions_e.swEndCondBlind, 0, this.heightBody, 0,
          false, false, false, false, 0, 0, false, false, false, false, false, false, false, false);
        this.RenameFeature("Surface-Extrude", this.nameExternalSurface);
      } else {
        double thin = this.thinBody + 0.01;
        string namePattern = Prime.Patterns_e.Bottom.ToString().ToLower();
        if (this.pattern.Equals(Prime.Patterns_e.Cover.ToString().ToLower())) {
          namePattern = Prime.Patterns_e.Cover.ToString().ToLower();
        } else {
          namePattern = Prime.Patterns_e.Bottom.ToString().ToLower();
          thin *= -1;
        }
        bool status = this.swExtension.SelectByID2(namePattern + "Sketch", this.swSketch, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        this.swDocument.UnblankSketch();
        // изгражда момощна повърхнина на отстояние 10mm от външната стена
        this.SelectPlane(this.swFrontPlane);
        status = this.swExtension.SelectByID2("Line1@" + namePattern + "Sketch", "EXTSKETCHSEGMENT", 0, 0, 0, true, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        status = this.swExtension.SelectByID2("Arc1@" + namePattern + "Sketch", "EXTSKETCHSEGMENT", 0, 0, 0, true, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        status = this.swDocument.SketchOffsetEntities2(thin, false, true);
        this.swSketchManager.InsertSketch(true);
        this.ClearSelection();
        status = this.swExtension.SelectByID2(namePattern + "Sketch", this.swSketch, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        this.swDocument.BlankSketch();
        this.ClearSelection();
        this.RenameFeature("Sketch", this.nameExternalSurfaceSketch);
        this.ClearSelection();
        status = this.swExtension.SelectByID2(this.nameExternalSurfaceSketch, "SKETCH", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        status = this.swExtension.SelectByID2(namePattern + "Axis", "AXIS", 0, 0, 0, true, 16, null, (int)swSelectOption_e.swSelectOptionDefault);
        Feature fet = this.swFeatureManager.FeatureRevolve2(true, false, false, false, false, false, 0, 0, 6.2831853071796, 0, false, false, 0.01, 0.01, 0, 0, 0, true, true, true);
        this.RenameFeature("Surface-Revolve", this.nameExternalSurface);
        // изгражда помощна повърхнина по вътрешната стена на тялото
        this.SelectPlane(this.swFrontPlane);
        status = this.swExtension.SelectByID2("Line1@" + namePattern + "Sketch", "EXTSKETCHSEGMENT", 0, 0, 0, true, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        status = this.swExtension.SelectByID2("Arc1@" + namePattern + "Sketch", "EXTSKETCHSEGMENT", 0, 0, 0, true, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        status = this.swDocument.SketchOffsetEntities2(0, false, true);
        this.swSketchManager.InsertSketch(true);
        this.ClearSelection();
        this.RenameFeature("Sketch", this.nameInternalSurfaceSketch);
        this.ClearSelection();
        status = this.swExtension.SelectByID2(this.nameInternalSurfaceSketch, "SKETCH", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        status = this.swExtension.SelectByID2(namePattern + "Axis", "AXIS", 0, 0, 0, true, 16, null, (int)swSelectOption_e.swSelectOptionDefault);
        this.swFeatureManager.FeatureRevolve2(true, false, false, false, false, false, 0, 0, 6.2831853071796, 0, false, false, 0.01, 0.01, 0, 0, 0, true, true, true);
        this.RenameFeature("Surface-Revolve", this.nameInternalSurface);
      }
    } // CreatingSurfaces

    /// <summary>създава скицата за отвора</summary>
    protected void Sketching() {
      if (this.pattern.Equals(Prime.Patterns_e.Mantel.ToString().ToLower())) {
        // избира работната равнина
        this.SelectPlane(this.namePlaneHole);
        this.Circle(this.height, 0, 0, this.diameter + 2 * this.thinBody, this.nameDiameter, this.nameBodySketch);
        // привързва центъра на окръжноста към центъра на координатната система
        this.ClearSelection();
        this.swExtension.SelectByID2("Point2", this.swSketchSegment, 0, 0, 0, true, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        this.swExtension.SelectByID2("Point1@Origin", this.swExtSketchPoint, 0, 0, 0, true, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        this.swDocument.SketchAddConstraints("sgHORIZONTALPOINTS2D");
        // създава размер от центъра на координатната система до центъра на окръжноста
        this.ClearSelection();
        this.AddSize("Point1@Origin", this.swExtSketchPoint, this.nameHeight, this.diameter / 2, this.diameter / 2, 0, this.height, true, "Point2", this.swSketchPoint);
        this.InsertSketch();
      } else {
        this.ClearSelection();
        this.SelectPlane(this.swTopPlane);
        this.curAngle = Prime.InRadians(45);
        double x = Prime.SinFn(this.curAngle, this.distance);
        double y = x;
        // създава окръжност по диаметъра наотвора
        this.Circle(x, y, 0, this.diameter + 2 * this.thinBody, this.nameDiameter, this.nameBodySketch);
        this.swSketchManager.CreateLine(0, 0, 0, x, y, 0);
        this.ClearSelection();
        // създава конструктивна линия от центъра на окръжността до центъра на коорднинатната система
        this.swExtension.SelectByID2("Line1", this.swSketchSegment, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        this.swSketchManager.CreateConstructionGeometry();
        this.AddSize("Line1", this.swSketchSegment, this.nameDistance, 0, 0, 0, this.distance);
        this.ClearSelection();
        // създава ъгъл между конструктивната права и X - оста на координатната система
        bool res = this.swExtension.SelectByID2("Line1", this.swSketchSegment, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        res = this.swExtension.SelectByID2("", this.swSketchPoint, 0, 0, 0, true, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        DisplayDimension dim = (DisplayDimension)this.swExtension.AddDimension(0, 0, 0, (int)swSmartDimensionDirection_e.swSmartDimensionDirection_Right);
        dim.GetDimension2(0).Name = this.nameAngle;
        this.ClearSelection();
        this.InsertSketch();
        // завърта скицата за ъгловото отместване на отвора
        this.RotateAngleSketch();
      }
    } // Sketching

    /// <summary>създава отвора с изтегляне на материал и след това с отнемане на материал</summary>
    protected void CutExtrusion() {
      bool dir = true;
      if (this.pattern.Equals(Prime.Patterns_e.Cover.ToString().ToLower()))
        dir = false;
      // избира скицата за отвора и външната помощна повърхнина
      this.Selected(this.nameBodySketch, this.swSketch);
      this.Selected(this.nameExternalSurface, this.swSurfaceBody, true, 1);
      this.swDocument.FeatureManager.FeatureExtrusionThin2(true, false, dir,
          (int)swEndConditions_e.swEndCondUpToSurface, 0, 0, 0,
          false, false, false, false, 0, 0, false, false, false, false, true,
          this.thinBody, 0, 0, 
          1, 
          0, false, 0, true, true,
          (int)swStartConditions_e.swStartSketchPlane, 0, false);
      // преименува стандарното си наименование на тялото
      this.RenameFeature(this.swExtrudeThin, this.nameHoleExternal);
      // премахва тялото отоста до вътрешната помощна повърхнина
      this.ClearSelection();
      this.Selected(this.nameBodySketch, this.swSketch);
      this.Selected(this.nameInternalSurface, this.swSurfaceBody, true, 1);
      this.swFeatureManager.FeatureCut4(true, false, !dir,
        (int)swEndConditions_e.swEndCondUpToSurface, 0, 0, 0,
        false, false, false, false, 0, 0, false, false, false, false, false,
        true, true, true, true, false,
        (int)swStartConditions_e.swStartSketchPlane, 0, false, false);
      this.RenameFeature("Cut-Extrude", this.nameHoleCut);
    } // CutExtrusion

    /// <summary>довършване на отвора, като прмахва материала от тялото върху което се изгражда отвора</summary>
    protected void EndHole() {
      if (this.pattern.Equals(Prime.Patterns_e.Mantel.ToString().ToLower())) {
        // избира работната равнина
        this.SelectPlane(this.namePlaneHole);
        this.Circle(-this.height, 0, 0, this.diameter, this.nameDiameter, this.nameSketchEnd);
        // привързва центъра на окръжноста към центъра на координатната система
        this.ClearSelection();
        this.Selected("Point2", this.swSketchSegment);
        this.Selected(this.swStartCS, this.swExtSketchPoint, true);
        this.swDocument.SketchAddConstraints(this.sgHorizontalPoints);
        // създава размер от центъра на координатната система до центъра на окръжноста
        this.ClearSelection();
        this.AddSize(this.swStartCS, this.swExtSketchPoint, this.nameHeight, this.diameter / 2, this.diameter / 2, 0, this.height, true, "Point2", this.swSketchPoint);
        this.InsertSketch();
      } else {
        this.ClearSelection();
        bool status = this.swExtension.SelectByID2(this.nameBodySketch, this.swSketch, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        this.swDocument.UnblankSketch();
        this.ClearSelection();
        this.SelectPlane(this.swTopPlane);
        status = this.swExtension.SelectByID2("Arc1@" + this.nameBodySketch, "EXTSKETCHSEGMENT", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        status = this.swDocument.SketchOffsetEntities2(-this.thinBody, false, true);
        this.ClearSelection();
        this.swSketchManager.InsertSketch(true);
        this.RenameFeature("Sketch", this.nameSketchEnd);
        this.ClearSelection();
        status = this.swExtension.SelectByID2(this.nameBodySketch, this.swSketch, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        this.swDocument.BlankSketch();
      }
      bool dir = false;
      if (this.pattern.Equals(Prime.Patterns_e.Cover.ToString().ToLower()))
        dir = true;
      this.ClearSelection();
      this.swExtension.SelectByID2(this.nameSketchEnd, this.swSketch, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
      this.swFeatureManager.FeatureCut4(true, false, dir,
        (int)swEndConditions_e.swEndCondThroughAll, 0, 0, 0,
        false, false, false, false, 0, 0, false, false, false, false, false,
        true, true, true, true, false,
        (int)swStartConditions_e.swStartSketchPlane, 0, false, false);
      this.RenameFeature("Cut-Extrude", this.nameHoleEnd);
    } // EndHole

    // <summary>завърта скицата за изобразяване на заваръчния шев на искания ъгъл</summary>
    protected void RotateAngleSketch() {
      // започва редактиране на хоризонталната скица за отвора
      if (!this.swExtension.SelectByID2(this.nameBodySketch, this.swSketch, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault))
        return;
      this.swDocument.EditSketch();
      // изтрива текущия ъгъл на заваръчния шев
      this.ClearSelection();
      bool res = this.swExtension.SelectByID2(Prime.Name(this.nameAngle, this.nameBodySketch), this.swDimension, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
      this.swDocument.EditDelete();
      if (this.curAngle != 0) {
        // завърта скицата на 0 градус на заваръчния шев
        this.ClearSelection();
        this.swExtension.SelectByID2("Arc1", this.swSketchSegment, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        this.swExtension.SelectByID2("Line1", this.swSketchSegment, 0, 0, 0, true, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        this.swExtension.RotateOrCopy(false, 1, true, 0, 0, 0, 0, 0, 1, -this.curAngle);
        this.ClearSelection();
      }
      // завърта скицата за ъгъла на заваръчния шев
      this.curAngle = this.RealAngle(this.angularOffset);
      this.ClearSelection();
      this.swExtension.SelectByID2("Arc1", this.swSketchSegment, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
      this.swExtension.SelectByID2("Line1", this.swSketchSegment, 0, 0, 0, true, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
      this.swExtension.RotateOrCopy(false, 1, true, 0, 0, 0, 0, 0, 1, this.curAngle);
      this.ClearSelection();
      // създава измерване на ъгъла на отместване на заваръчния шев
      // създава ъгъл между конструктивната права и X - оста на координатната система
      res = this.swExtension.SelectByID2("Line1", this.swSketchSegment, 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
      res = this.swExtension.SelectByID2("", this.swSketchPoint, 0, 0, 0, true, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
      DisplayDimension dim = (DisplayDimension)this.swExtension.AddDimension(0, 0, 0, (int)swSmartDimensionDirection_e.swSmartDimensionDirection_Right);
      dim.GetDimension2(0).Name = this.nameAngle;
      this.ClearSelection();
      this.InsertSketch();
    } // RotateAngleSketch
    #endregion Protected Methods

    public void SetHoleNumber(int number) {
      this.holeNumber = number;
    }
  } // OutHole

}
