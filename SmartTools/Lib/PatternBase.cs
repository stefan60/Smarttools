using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using SolidWorks.Interop.swconst;
using SolidWorks.Interop.sldworks;

namespace SmartTools.Lib {
  [ComVisible(true)]
  public abstract class PatternBase: PatternFeatureBase {

    #region SW Names
    /// <summary>началото на координатната система в SW</summary>
    /// <date>2019-04-08</date>
    public string swStartCS { get { return "Point1@Origin"; } }
    /// <summary>тип равнина</summary>
    /// <date>2019-04-08</date>
    public string swPlane { get { return "PLANE"; } }
    /// <summary>фронталната равнина в SW</summary>
    /// <date>2019-04-08</date>
    public string swFrontPlane { get { return "Front Plane"; } }
    /// <summary>страничната равнина в SW</summary>
    /// <date>2019-04-08</date>
    public string swRightPlane { get { return "Right Plane"; } }
    /// <summary>хоризонтална равнина в SW</summary>
    /// <date>2019-04-08</date>
    public virtual string swTopPlane { get { return "Top Plane"; } }
    /// <summary>тип на ос в SW</summary>
    /// <date>2019-04-08</date>
    public virtual string swAxis { get { return "AXIS"; } }
    /// <summary>тип точка в скицата в SW</summary>
    /// <date>2019-04-08</date>
    public virtual string swSketchPoint { get { return "SKETCHPOINT"; } }
    /// <summary>тип точка извън работната равнина в SW</summary>
    public string swExtSketchPoint { get { return "EXTSKETCHPOINT"; } }
    /// <summary>тип елемент на скица в SW</summary>
    /// <date>2019-04-08</date>
    public string swSketchSegment { get { return "SKETCHSEGMENT"; } }

    /// <summary>ограничение за хоризонталност между две точки</summary>
    /// <date>2019-04-08</date>
    public string sgHorizontalPoints { get { return "sgHORIZONTALPOINTS2D"; } }
    /// <summary>ограничение за тангенциалност на избрани елементи</summary>
    /// <date>2019-04-08</date>
    public string sgTangent { get { return "sgTANGENT"; } }
    #endregion SW Names

    #region Dimension names
    /// <summary>наименование на оста на тялото</summary>
    /// <date>2019-04-08</date>
    public virtual string nameAxis { get { return this.nameModelToLower + "Axis" + ((this.holeNumber != 0) ? this.holeNumber.ToString() : ""); } }
    /// <summary>наименование на тялото на модела</summary>
    /// <date>2019-04-08</date>
    public virtual string nameBody { get { return this.nameModelToLower + "Body"; } }
    /// <summary>наименование на скицата за изграждане на тялото</summary>
    /// <date>2019-04-10</date>
    public virtual string nameBodySketch {
      get { return this.nameModelToLower + "Sketch" + ((this.holeNumber != 0) ? this.holeNumber.ToString() : ""); }
    }
    /// <summary>наименование на размер за дебелина на стена</summary>
    /// <date>2019-04-08</date>
    public virtual string nameThickness { get { return "thin"; } }
    /// <summary>наименование на базовия вътрешния радиус на тялото</summary>
    /// <date>2019-04-08</date>
    public virtual string nameBodyRadius { get { return "bodyRadius"; } }
    /// <summary>наименование на ъгъла на наклона на дъното или капака</summary>
    /// <date>2019-04-08</date>
    public virtual string nameAngleInclination { get { return "angleInclination"; } }
    /// <summary>наименование на радиуса на бертване на дъното или капака</summary>
    /// <date>2019-04-08</date>
    public virtual string nameBendRadius { get { return "bendRadius"; } }
    /// <summary>наименование на тялото на заваръчния шев</summary>
    /// <date>2019-04-08</date>
    public virtual string nameBodyWS { get { return this.nameModelToLower + "BodyWS"; } }
    /// <summary>наименование на размер за вътрешна височина на избушен отвор</summary>
    /// <date>2019-04-10</date>
    public virtual string nameExplodeInternal { get { return "iExplode"; } }
    /// <summary>наименование на размер за вътрешна височина на избушен отвор</summary>
    /// <date>2019-04-10</date>
    public virtual string nameExplodeExternal { get { return "eExplode"; } }
    // <summary>наименование на размер за повърхнина по пъншната страна на тяло</summary>
    /// <date>2019-04-10</date>
    public virtual string nameExplodeMidle { get { return "mExplode"; } }
    /// <summary>наименование на модела като текст</summary>
    /// <date>2019-04-08</date>
    public virtual string nameModel { get { return this.modelType.ToString(); } }
    /// <summary>наименование на модела като текст с малки букви</summary>
    /// <date>2019-04-08</date>
    public virtual string nameModelToLower { get { return this.nameModel.ToLower(); } }
    #endregion Dimension names

    /// <summary>добавя нова скица или обновява съществуващата</summary>
    /// <date>2019-04-08</date>
    protected virtual void InsertSketch() {
      this.swSketchManager.InsertSketch(true);
    } // InsertSketch

    /// <summary>премахва всички избрани обекти</summary>
    /// <date>2019-04-08</date>
    protected void ClearSelection() {
      this.swDocument.ClearSelection2(true);
    } // ClearSelection

    /// <summary>избира обект с определено име и тип</summary>
    /// <date>2019-04-08</date>
    /// <param name="objectName">наименование на обект</param>
    /// <param name="objectType">тип на обекта</param>
    /// <param name="append">начин на добавяне на обекта в списъка</param>
    /// <param name="mark">флаг в зависимост от следващата процедура</param>
    /// <returns>TRUE обекта е избран, FALSES - не е избран</returns>
    protected bool Selected(string objectName, string objectType,
                                        bool append = false, int mark = 0) {
      return this.swExtension.SelectByID2(objectName, objectType, 0, 0, 0,
           append, mark, null, (int)swSelectOption_e.swSelectOptionDefault);
    } // Selected

    /// <summary>избира работна равнина и активира рабитна скица</summary>
    /// <date>2019-04-08</date>
    /// <param name="plane">наименование на варнината</param>
    public virtual void SelectPlane(string plane) {
      this.ClearSelection();
      bool status = this.Selected(plane, this.swPlane);
      this.InsertSketch();
    } // SelectPlane

    /// <summary>връща обект с указано име</summary>
    /// <date>2019-04-2019</date>
    /// <param name="namePattern">наименование на обекта, modelType</param>
    /// <returns>обект от тип PatternBase или null не съществува</returns>
    public PatternBase GetPatternByName(string namePattern) {
      namePattern = namePattern.ToLower();
      foreach (PatternBase pattern in Prime.AddIn.Patterns) {
        if (namePattern.Equals(pattern.modelType.ToString().ToLower())) {
          return pattern;
        }
      }
      return null;
    } // GetPatternByName

    /// <summary>връща указател към feature</summary>
    /// <date>2019-004-08</date>
    /// <param name="name">наименование на feature</param>
    /// <returns>указател към feature с търсеното наименование, 
    /// null ако не е открит feature</returns>
    protected virtual Feature GetFeature(string name, bool fullName = true) {
      Feature feature = default(Feature);
      if (this.swDocument == default(ModelDoc2)) {
        feature = ((ModelDoc2)Prime.AddIn.SwApp.ActiveDoc).IFirstFeature();
      } else {
        feature = this.swDocument.IFirstFeature();
      }
      while (feature != null) {
        if (fullName) {
          if (feature.Name.ToLower().Equals(name.ToLower()))
            return feature;
        } else {
          if (feature.Name.ToLower().IndexOf(name.ToLower()) == 0)
            return feature;
        }
        feature = feature.IGetNextFeature();
      }
      return feature;
    } // GetFeature

    /// <summary>връща контур от скица</summary>
    /// <date>2019-04-10</date>
    /// <param name="nameSketch">наименование на скицата</param>
    /// <param name="contour">номер на контура, 0 базиран индекс</param>
    /// <returns>SketchContour или null ако не съществува</returns>
    public SketchContour Contour(string nameSketch, int contour) {
      Feature feature = (Feature)((PartDoc)this.swDocument).FeatureByName(nameSketch);
      Sketch mySketch = (Sketch)feature.GetSpecificFeature2();
      object[] contours = (object[])mySketch.GetSketchContours();
      if (contours != null && contours.Length > 0 && contour < contours.Length)
        return (SketchContour)contours[contour];
      return null;
    } // Contour

    /// <summary>преименува обект</summary>
    /// <date>2019-04-08</date>
    /// <param name="oldName">старо наименование</param>
    /// <param name="newName">ново наименование</param>
    /// <param name="fullOldName">фалг указващ дали старото наименование е 
    /// пълно, или е част, с която започва старото наименование</param>
    /// <returns>True обекта е преименуван, False не е преименуван</returns>
    protected virtual bool RenameFeature(string oldName, string newName, bool fullOldName = false) {
      Feature feature = this.GetFeature(oldName, fullOldName);
      if (feature != null) {
        if (newName != null)
          feature.Name = newName;
        return true;
      }
      return false;
    } // RenameFeature

    /// <summary>изчислява реалния ъгъл в координатна система, позиция 0 - север, посока на нарастване - по часовниковата стрелка</summary>
    /// <date>2019-04-08</date>
    /// <param name="relativeАngle">стойност на ъгъла в модела</param>
    /// <returns>стойност на реалния ъгъл</returns>
    protected double RealAngle(double relativeАngle) {
      //double realAngle;
      double offsetDegrees;
      if ((bool)this.Value(Prime.Controls_e.NortPosition)) {
        offsetDegrees = 0;
      } else if ((bool)this.Value(Prime.Controls_e.EastPosition)) {
        offsetDegrees = Prime.InRadians(90);
      } else if ((bool)this.Value(Prime.Controls_e.SoutPosition)) {
        offsetDegrees = Prime.InRadians(180);
      } else {
        offsetDegrees = Prime.InRadians(270);
      }
      if ((bool)this.Value(Prime.Controls_e.Clockwise)) {
        relativeАngle = offsetDegrees + relativeАngle;
      } else {
        relativeАngle = offsetDegrees - relativeАngle;
      }
      if (relativeАngle >= Prime.InRadians(360))
        return relativeАngle - Prime.InRadians(360);
      //relativeАngle = relativeАngle - Prime.InRadians(360);
      if (relativeАngle < 0)
        return relativeАngle + Prime.InRadians(360);
      //relativeАngle = relativeАngle + Prime.InRadians(360);
      //realAngle = relativeАngle;
      return relativeАngle;
    } // RealAngle

    /// <summary>създава ос като сечение на две равнини</summary>
    /// <date>2019-04-08</date>
    /// <param name="plane1">наименование на равнина 1</param>
    /// <param name="plane2">наименование на равнина 2</param>
    /// <param name="nameAxis">наименование на оста</param>
    public virtual void Axis(string plane1, string plane2, string nameAxis) {
      this.ClearSelection();
      this.Selected(plane1, this.swPlane);
      this.Selected(plane2, this.swPlane, true);
      this.swDocument.InsertAxis2(true);
      this.RenameFeature("Axis", nameAxis);
    } // Axis

    /// <summary>създава повърхност около модела</summary>
    /// <data>2019-04-09</data>
    /// <param name="offset">отстояние от стената</param>
    /// <param name="location">местоположение на повърхнината, 
    ///        TRUE от външната страна, FALSE от вътрешната страна</param>
    /// <param name="nameSurfaceBody">наименование на повърхнината</param>
    /// <param name="nameSurfaceSketch">хаименование на скицата</param>
    public virtual void CreateSurface(double offset, bool location,
                       string nameSurfaceBody, string nameSurfaceSketch) {
    } // CreateSurface

    /// <summary>създава скица за 3D модела на КОНУС/капак, дъно/</summary>
    /// <date>2019-04-10</date>
    /// <param name="cover">тип на конуса, TRUE капак, FALSE дъно</param>
    protected void SketchForCone(bool cover) {
      // създава скицата във фронталната равнина
      this.SelectPlane(this.swFrontPlane);
      // координати на центъра на дъгата на берта
      double xCenter = this.diameter / 2 - this.bendRadius;
      double yCenter = 0;
      double zCenter = 0;
      // координати на началната точка на дъгата на берта
      double xStart = this.diameter / 2;
      double yStart = 0;
      double zStart = 0;
      // координати на крайната точка на дъгата на берта
      double xEnd = Prime.SinFn(this.angleInclination, this.bendRadius) + 
                                                                    xCenter;
      double yEnd = Prime.CosFn(this.angleInclination, this.bendRadius) + 
                                                                    yCenter;
      double zEnd = 0;
      // координати на втора точка на правата
      double xLine = this.diameterTopCone / 2;
      double yLine = Math.Tan(this.angleInclination) * 
               (Prime.CosFn(this.angleInclination, this.bendRadius) + xEnd);
      double zLine = 0;
      short dir = 1;
      double yDimInclination = this.bendRadius;
      if (!cover) {
        yEnd = -yEnd;
        yLine = -yLine;
        dir = 0;
        yDimInclination = -yDimInclination;
      }
      // създава дъгата на берта
      this.swSketchManager.CreateArc(xCenter, yCenter, zCenter, 
                             xStart, yStart, zStart, xEnd, yEnd, zEnd, dir);
      // създава правата на формираща конуса
      this.swSketchManager.CreateLine(xEnd, yEnd, zEnd, xLine, yLine, zLine);
      this.ClearSelection();
      this.swDocument.ViewZoomtofit2();
      // създава ъгъла на наклона
      this.ClearSelection();
      bool status = this.Selected("Line1", this.swSketchSegment);
      status = this.Selected("Point2", this.swSketchPoint, true);
      double y = this.bendRadius;
      DisplayDimension dim = 
        (DisplayDimension)this.swExtension.AddDimension(this.diameter / 4, 
          yDimInclination, 0, 
          (int)swSmartDimensionDirection_e.swSmartDimensionDirection_Right);
      dim.GetDimension2(0).Name = this.nameAngleInclination;
      dim.GetDimension2(0).Value = Prime.InDegrees(this.angleInclination);
      this.ClearSelection();
      // поставя центъра на дъгара и началната точка на бертва върху Top Plane
      status = this.Selected(this.swStartCS, this.swExtSketchPoint);
      status = this.Selected("Point3", this.swSketchPoint, true);
      status = this.Selected("Point1", this.swSketchPoint, true);
      this.swDocument.SketchAddConstraints(this.sgHorizontalPoints);
      this.ClearSelection();
      // задава тангенциалност между правата и сегмента на бертване
      status = this.Selected("Point2", this.swSketchPoint);
      this.swDocument.SketchAddConstraints(this.sgTangent);
      this.ClearSelection();
      // задава размера на радиуса на бертване
      this.AddSize("Arc1", this.swSketchSegment, this.nameBendRadius, 
                this.diameter / 2 + this.bendRadius, yEnd, this.bendRadius);
      // задава размера на вътрешния радиус на тялото
      this.AddSize(this.swStartCS, this.swExtSketchPoint, 
        this.nameBodyRadius, this.diameter / 2 + 2 * this.bendRadius, 0, 0, 
        this.diameter / 2, true, "Point1", this.swSketchPoint);
      // задава размера на радиуса при върха на капака
      this.AddSize("Point4", this.swSketchPoint, this.nameRadiusTopBody, 
        0.02, yLine + 0.03, 0, this.diameterTopCone / 2, true, 
        this.nameAxis, this.swAxis);
      this.ClearSelection();
      this.RenameFeature("Sketch", this.nameBodySketch);
      // създава контура за изграждане на повърхнина на разстояние равно на
      // височината на избушен отвор спрямо вътрешната повърхнина на тялото
      // SketchContour == 1
      this.ClearSelection();
      this.Selected("Line1", this.swSketchSegment);
      this.Selected("Arc1", this.swSketchSegment, true);
      this.swSketchManager.SketchOffset2(-Prime.HEIHGT_EXPLODE_HOLE, false,
                                                         false, 0, 0, true);
      this.RenameDimension(this.FullName("D1", this.nameBodySketch),
                                                  this.nameExplodeInternal);
      // създава контура за изграждане на повърхнина на разстояние равно на
      // височината на избушен отвор спрямо външната повърхнина на тялото
      // SketchContour == 2
      this.ClearSelection();
      this.Selected("Line1", this.swSketchSegment);
      this.Selected("Arc1", this.swSketchSegment, true);
      this.swSketchManager.SketchOffset2(this.dimExplodeExternal,
                                                  false, false, 0, 0, true);
      this.RenameDimension(this.FullName("D1", this.nameBodySketch),
                                                  this.nameExplodeExternal);
      // създава контура за изграждане на повърхнина по външната повърхнина 
      // на тялото
      // SketchContour == 3
      this.ClearSelection();
      this.Selected("Line1", this.swSketchSegment);
      this.Selected("Arc1", this.swSketchSegment, true);
      this.swSketchManager.SketchOffset2(this.thickness, false, false, 0,
                                                                   0, true);
      this.RenameDimension(this.FullName("D1", this.nameBodySketch),
                                                     this.nameExplodeMidle);
      this.InsertSketch();
    } // SketchForCone

    /// <summary>създава 3D модел на КОНУС/капак, дъно/</summary>
    /// <date>2019-04-09</date>
    /// <param name="cover">тип на конуса, TRUE капак, FALSE дъно</param>
    protected void CreateConeBody(bool cover) {
      // създава скицата за 3D модела на КАПАК
      this.SketchForCone(cover);
      int dir;
      if (cover) {
        dir = (int)swThinWallType_e.swThinWallOneDirection;
      } else {
        dir = (int)swThinWallType_e.swThinWallOppDirection;
      }
      // създава 3D модела чрез завъртане на скицата около оста на модела
      SketchContour myContour = this.Contour(this.nameBodySketch, 0);
      SelectData swSelData = (SelectData)this.swSelectionMgr.CreateSelectData();
      this.ClearSelection();
      this.swSelectionMgr.EnableContourSelection = true;
      bool status = myContour.Select2(false, swSelData);
      status = this.Selected(this.nameAxis, this.swAxis, true, 16);
      this.swFeatureManager.FeatureRevolve2(true, true, true, false, false, false,
        (int)swEndConditions_e.swEndCondBlind, 0, Prime.RADIANS_360DEG, 0,
        false, false, 0, 0, dir, this.thickness,
        0, true, true, true);
      this.swSelectionMgr.EnableContourSelection = false;
      this.RenameFeature("Revolve-Thin", this.nameBody);
      // преименува стандартното наименование на размера за дебелината на стената
      this.RenameDimension(Prime.Name("D3", this.nameBody), this.nameThickness);
    } // CreateConeBody

    /// <summary>създава скицата за модела визуализиращ заваръчния шев</summary>
    /// <date>2019-04-08</date>
    protected void SketchWeldSeam(double shoulder) {
      this.SelectPlane(this.swTopPlane);
      this.swSketchManager.CreateCenterLine(-shoulder, 0, 0, shoulder, 0, 0);
      this.swSketchManager.CreateCenterLine(0, -shoulder, 0, 0, shoulder, 0);
      // координати на точката управляваща ъгловото отместване на заваръчния шев
      double x = Prime.SinFn(this.RealAngle(this.weldSeamAngle), shoulder);
      double y = Prime.CosFn(this.RealAngle(this.weldSeamAngle), shoulder);
      this.wsPoint = (SketchPoint)this.swSketchManager.CreatePoint(x, y, 0);
      // ъглополовяща на ъгъла визуализиращ заваръчния шев
      this.swSketchManager.CreateLine(0, 0, 0, this.wsPoint.X, this.wsPoint.Y, this.wsPoint.Z);
      this.AddDimensionPoint(this.wsPoint, "Line1", this.swSketchSegment, this.nameYWeldSeam, true);
      this.AddDimensionPoint(this.wsPoint, "Line2", this.swSketchSegment, this.nameXWeldSeam, false);
      this.ClearSelection();
      this.Selected("Line3", this.swSketchSegment);
      this.swSketchManager.CreateConstructionGeometry();
      // първо рамо на ъгъла
      x = Prime.SinFn(this.RealAngle(this.weldSeamAngle + Prime.InRadians(2)), shoulder);
      y = Prime.CosFn(this.RealAngle(this.weldSeamAngle + Prime.InRadians(2)), shoulder);
      this.swSketchManager.CreateLine(0, 0, 0, x, y, 0);
      // второ рамо на ъгъла
      double x1 = Prime.SinFn(this.RealAngle(this.weldSeamAngle - Prime.InRadians(2)), shoulder);
      double y1 = Prime.CosFn(this.RealAngle(this.weldSeamAngle - Prime.InRadians(2)), shoulder);
      this.swSketchManager.CreateLine(0, 0, 0, x1, y1, 0);
      // основа на триъгълника
      this.swSketchManager.CreateLine(x, y, 0, x1, y1, 0);
      // двете рамена на ъгъла са равни
      this.ClearSelection();
      this.Selected("Line3", this.swSketchSegment);
      this.Selected("Line4", this.swSketchSegment, true);
      this.Selected("Line5", this.swSketchSegment, true);
      this.swDocument.SketchAddConstraints(this.sgSameLength);
      this.swDocument.SketchAddConstraints(this.sgSymmetric);
      this.ClearSelection();
      this.Selected("Line6", this.swSketchSegment);
      this.swDocument.SketchConstraintsDel(0, this.sgPerpendicular);
      // създава ъглова връзка между двете рамена на ъгъла
      this.AddSize("Line4", this.swSketchSegment, this.nameAngle, this.wsPoint.X + 0.050, 0, -this.wsPoint.Y + 0.050, this.defAngleView / 2, true, "Line3", this.swSketchSegment);
      this.ClearSelection();
      this.RenameFeature("Sketch", this.nameSketchAngle);
      this.InsertSketch();
    } // SketchWeldSeam

    /// <summary>създава визуализация на заваръчния шев</summary>
    /// <date>2019-04-08</date>
    protected void CreateWeldSeam(double shoulder, string nameFeature, bool cover = true) {
      // създава скицата за модела визуализиращ заваръчния шев
      this.SketchWeldSeam(shoulder);
      // визуализиране на заваръчния шев
      this.ClearSelection();
      this.Selected(this.nameSketchAngle, this.swSketch);
      this.swDocument.FeatureManager.FeatureCut4(true, false, cover,
          (int)swEndConditions_e.swEndCondThroughAll, 0, 0, 0, false,
          false, false, false, 0, 0, false, false, false, false, false,
          true, true, true, true, false,
          (int)swStartConditions_e.swStartSketchPlane, 0, false, false);
      this.RenameFeature("Cut-Extrude", nameFeature);
      this.ClearSelection();
    } // CreateWeldSeam

    /// <summary>визуализира заваръчния шев с нови параметри </summary>
    /// <date>2019-04-08</date>
    protected void SetViewWeldSeam(double shoulder) {
      double x = Prime.SinFn(this.RealAngle(this.weldSeamAngle), shoulder);
      double y = Prime.CosFn(this.RealAngle(this.weldSeamAngle), shoulder);
      // проверка новата водеща точка съвпада ли със старата
      if (Math.Abs(this.wsPoint.X - x) < 0.003 && Math.Abs(this.wsPoint.Y - y) < 0.003)
        return;
      //редактира скицата за визуализаця на заваръчния шев
      this.ClearSelection();
      this.Selected(this.nameSketchAngle, this.swSketch);
      this.swDocument.EditSketch();
      this.ClearSelection();
      //изтрива водещата точка в скицата
      SelectData swSelData = (SelectData)this.swSelectionMgr.CreateSelectData();
      bool status = this.wsPoint.Select4(false, swSelData);
      this.swDocument.EditDelete();
      // създава новата водеща точла
      this.ClearSelection();
      this.wsPoint = this.swSketchManager.CreatePoint(x, y, 0);
      this.ClearSelection();
      status = this.wsPoint.Select4(false, swSelData);
      this.swDocument.SketchConstraintsDel(0, this.sgCoincident);
      // задава новите координати на водещата точка
      this.AddDimensionPoint(this.wsPoint, "Line1", this.swSketchSegment, this.nameYWeldSeam, true);
      this.AddDimensionPoint(this.wsPoint, "Line2", this.swSketchSegment, this.nameXWeldSeam, false);
      this.ClearSelection();
      status = this.Selected("Line3", this.swSketchSegment);
      SketchLine line3 = (SketchLine)this.swSelectionMgr.GetSelectedObject6(1, -1);
      SketchPoint p = (SketchPoint)line3.GetEndPoint2();
      p.X = this.wsPoint.X;
      p.Y = this.wsPoint.Y;
      p.Z = this.wsPoint.Z;
      status = this.wsPoint.Select4(false, swSelData);
      status = ((SketchPoint)line3.GetEndPoint2()).Select4(true, swSelData);
      this.swDocument.SketchAddConstraints(this.sgCoincident);
      this.InsertSketch();
    } // SetViewWeldSeam 

    /// <summary>
    /// създава размер на обект или размер между два обекта</summary>
    /// <date>201-04-08</date>
    /// <param name="nameObject">наименование на обекта</param>
    /// <param name="typeObject">тип на обекта</param>
    /// <param name="nameSize">наименование на размера</param>
    /// <param name="x">X координата на текстовото описание на размера</param>
    /// <param name="y">Y координата на текстовото описание на размера</param>
    /// <param name="z">Z координата на текстовото описание на размера</param>
    /// <param name="val">стойност на размера</param>
    /// <param name="readOnly">размера не може да се променя</param>
    /// <param name="addNameObject">наименование на втори обект</param>
    /// <param name="addTypeObject">тип на втория обект</param>
    protected virtual void AddSize(string nameObject, string typeObject,
                                   string nameSize, double x, double y,
                                   double z, double val = 0,
                                   bool readOnly = true,
                                   string addNameObject = null,
                                   string addTypeObject = null) {
      this.SuppressSizeInput();
      this.ClearSelection();
      bool status = this.Selected(nameObject, typeObject);
      if (addNameObject != null)
        status = this.Selected(addNameObject, addTypeObject, true);
      DisplayDimension size = this.swDocument.IAddDimension2(x, y, z);
      size.GetDimension2(0).Name = nameSize;
      size.GetDimension2(0).ReadOnly = readOnly;
      //size.SupplementaryAngle();
      if (val > 0)
        size.GetDimension2(0).SetSystemValue3(val,
          (int)swSetValueInConfiguration_e.swSetValue_InAllConfigurations, null);
    } // AddSize

    /// <summary>установява стойност на рамер</summary>
    /// <date>2019-04-08</date>
    /// <param name="name">наименование на размера</param>
    /// <param name="value">стойност на размера</param>
    /// <param name="config">конфигурация за която се отнася размера</param>
    /// <param name="nameConfig">наименование на конфигурацията</param>
    public virtual void SetDimension(string name, double value,
      swSetValueInConfiguration_e config = swSetValueInConfiguration_e.swSetValue_InAllConfigurations,
      string nameConfig = null) {
      Dimension dim = this.swDocument.IParameter(name);
      if (dim != null)
        dim.SetSystemValue3(value, (int)config, nameConfig);
    } // SetDimension
    /// <summary>поддиска въвеждането на стойност на размера от екрана</summary>
    /// <date>2019-08-04</date>
    protected virtual void SuppressSizeInput() {
      Prime.AddIn.SwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swInputDimValOnCreate, false);
    } // SuppressSizeInput

    //======================================================================
    #region Before 2019-04-08






    // променливи за имената на елементи на модела
    #region Feature Names
    /// <summary>наименование на размер за диаметър</summary>
    public virtual string nameDiameter { get { return "diameter"; } }
    
    /// <summary>наименование на размер за височина</summary>
    public virtual string nameHeight { get { return "height"; } }
    /// <summary>наименование на размера за отстояние</summary>
    public virtual string nameDistance {  get { return "distance"; } }
    /// <summary>наименование на размер за ъгъл</summary>
    public virtual string nameAngle { get { return "angle"; } }
    /// <summary>наименование за размера на рамото на ъгъл</summary>
    public virtual string nameAngleSide { get { return "angleSide"; } }
    
    
    /// <summary>наименование на равнината на скицата за отвор</summary>
    public virtual string namePlaneHole {
      get {
        return this.nameModelToLower + "Plane" + ((this.holeNumber != 0) ? this.holeNumber.ToString() : "");
      }
    }
    /// <summary>наименование на 3D модела на отвора</summary>
    public virtual string nameHole3D {
      get {
        return this.nameModelToLower + "Cut" + ((this.holeNumber != 0) ? this.holeNumber.ToString() : "");
      }
    }
    /// <summary>наименование на скицата за изобразяване на заваръчния шев</summary>
    public string nameSketchAngle { get { return this.nameModelToLower + "AngleSketch"; } }
    
    
    /// <summary>наименование на ъгъла визуализиращ заваръчния шев</summary>
    public virtual string nameAngleView { get { return "angleView"; } }
    
    
    
    /// <summary>наименование на страната на равнобедрения триъгълник за заваръчния шев</summary>
    public virtual string nameSideTriangle { get { return "sideTriangle"; } }
    /// <summary>наименование на радиуса при върха на дъното или капака</summary>
    public virtual string nameRadiusTopBody { get { return "bodyRadiusTop"; } }
    /// <summary>наименование на X-координатата на точката управляваща ъгловото отместване на заваръчния шев</summary>
    public virtual string nameXWeldSeam { get { return "xWeldSeam"; } }
    /// <summary>наименование на Y-координатата на точката управляваща ъгловото отместване на заваръчния шев</summary>
    public virtual string nameYWeldSeam { get { return "yWeldSeam"; } }
    /// <summary>наименование на скицата за вътрешната помощна повърхнина</summary>
    public string nameInternalSurfaceSketch {
      get { return "intSurfaceSketch" + this.nameModel + this.holeNumber.ToString(); }
    }
    /// <summary>наименование на скицата за външната помощна повърхнина</summary>
    public string nameExternalSurfaceSketch {
      get { return "extSurfaceSketch" + this.nameModel + this.holeNumber.ToString(); }
    }
    /// <summary>наименование на скицата оформяща вътрешния диаметър на отвора</summary>
    public string nameSketchEnd {
      get { return "endSketch" + this.nameModel + this.holeNumber.ToString(); }
    }
    /// <summary>наименование на вътрешната помощната повърхнина</summary>
    public string nameInternalSurface {
      get { return "inSurface" + this.nameModel + this.holeNumber.ToString(); }
    }
    /// <summary>наименование на външната помощната повърхнина</summary>
    public string nameExternalSurface {
      get { return "extSurface" + this.nameModel + this.holeNumber.ToString(); }
    }

    
    #endregion Feature Names

    // наименования дефинирани от SW API
    #region SW Names
    
    
    /// <summary>наименование на тип за скица в SW</summary>
    public virtual string swSketch { get { return "SKETCH"; } }
    
    
    /// <summary>наименование на тип елемент извън работната равнина в SW</summary>
    public string swExtSketchSegment { get { return "EXTSKETCHSEGMENT"; } }
    /// <summary>наименование на тип за помощна равнина</summary>
    public virtual string swSurfaceBody { get { return "SURFACEBODY"; } }
    /// <summary>наименование на тип за помощно тяло</summary>
    public virtual string swRefSurface { get { return "REFSURFACE"; } }

    
    

    /// <summary>ограничение за вертикалност на елемент в SW</summary>
    public string sgVertical { get { return "sgVERTICAL2D"; } }
    
    /// <summary>ограничение за симетричност между избрани елементи</summary>
    public string sgSymmetric { get { return "sgSYMMETRIC"; } }
    /// <summary>ограничение за еднаква дължина за избрани елементи</summary>
    public string sgSameLength { get { return "sgSAMELENGTH"; } }
    /// <summary>ограничение за съвпадение на два избрани елемента</summary>
    public string sgCoincident { get { return "sgCOINCIDENT"; } }
    /// <summary>ограничение за перпендикплярност на избрани елементи</summary>
    public string sgPerpendicular { get { return "sgPERPENDICULAR"; } }
    
    #endregion SW Names

    public SketchLine trSide1 { get; protected set; }
    public SketchLine trSide2 { get; protected set; }

    

    // помощни методи за работа с елементите в среда на SW
    #region Auxiliary Methods

    /// <summary>връща пълното име на обект</summary>
    /// <param name="feature">наименование на обекта</param>
    /// <param name="typeFeature">тип на обекта</param>
    /// <returns>пълното име на обекта</returns>
    public string FullName(string feature, string typeFeature) {
      return feature + "@" + typeFeature;
    } // FullName

    

    

    

    

    /// <summary>създава размер към точка и елемент от скица</summary>
    /// <param name="point">обект към точката</param>
    /// <param name="nameElement">наименование на еленет от скицата</param>
    /// <param name="typeElement">тип на елемент от скицата</param>
    /// <param name="nameDimension">наименование на размера</param>
    /// <param name="vertical">вертикален размер - TRUE, хоризонтален размер -FALSE</param>
    protected virtual void AddDimensionPoint(SketchPoint point, string nameElement, string typeElement, string nameDimension, bool vertical = true) {
      SelectData swSelData = (SelectData)this.swSelectionMgr.CreateSelectData();
      this.ClearSelection();
      if ((vertical && Math.Abs(point.Y) < Prime.DECIMAL_PLACE) || (!vertical && Math.Abs(point.X) < Prime.DECIMAL_PLACE))
        return;
      double x;
      double y;
      int sign = 1;
      if (vertical) {
        if (point.X < 0)
          sign = -1;
        x = point.X + 0.02 * sign;
        y = -point.Y / 2;
      } else {
        x = point.X / 2;
        if (point.Y < 0)
          sign = -1;
        y = -(point.Y + 0.02 * sign);
      }
      /* = vertical ? point.X : point.X / 2; = vertical ? point.Y / Math.Abs(point.Y) * Math.Abs(point.Y) / 2 : point.Y / 2;
      /*double x = vertical ? -(point.X / Math.Abs(point.X)) * 0.09 : point.X / 2;*/
      /*double y = vertical ? -point.Y / 2 : point.Y + 0.1;*/
      bool status = point.Select4(false, swSelData);
      status = this.Selected(nameElement, typeElement, true);
      DisplayDimension size = (DisplayDimension)this.swDocument.AddDimension2(x, 0, y);
      size.GetDimension2(0).Name = nameDimension;
      size.GetDimension2(0).ReadOnly = true;
      this.ClearSelection();
    } // AddDimensionPoint

    

    /// <summary>преименура размер</summary>
    /// <param name="oldName">старо наименование на размера</param>
    /// <param name="newName">ново наименование на размера</param>
    /// <param name="read">размера е само за четене</param>
    public virtual void RenameDimension(string oldName, string newName, bool read = true) {
      Dimension dim = this.swDocument.IParameter(oldName);
      if (dim != default(Dimension)) {
        dim.Name = newName;
        dim.ReadOnly = read;
      }
    } // RenameDimension
    #endregion Auxiliary Methods

    #region Creating Feature

    


    

    

    

    

    

    





    /// <summary>създава скица за заваръчния шев на мантел, дъно и капак</summary>
    public virtual void CreatingWeldingSeamSketch() {
      double size = this.diameter / 2 + this.thickness + 0.05;
      double x = Prime.CosFn(Prime.InRadians(2), size);
      double y = Prime.SinFn(Prime.InRadians(2), size);
      // избира работната равнина
      this.SelectPlane(this.swTopPlane);
      // създава помощна права по Y-координата
      this.swSketchManager.CreateCenterLine(0, 0, 0, 0, size + 0.03, 0);
      // създава равнобедрен триъгълник с ъглополовяща при върха
      this.ClearSelection();
      // Line2 ъглополовяща по оста X
      SketchSegment line = this.swSketchManager.CreateLine(0, 0, 0, size, 0, 0);
      line.ConstructionGeometry = true;
      this.DelConstraints("Line2", "sgHORIZONTAL2D");
      // Line3 и Line4 страните на равнобедрения триъгълник
      this.swSketchManager.CreateLine(0, 0, 0, x, y, 0);
      this.swSketchManager.CreateLine(0, 0, 0, x, -y, 0);
      // Line5 основа на триъгълника
      this.swSketchManager.CreateLine(x, y, 0, x, -y, 0);
      this.DelConstraints("Line5", "sgVERTICAL2D");
      // задава дължината на страната Line 4
      this.AddSize("Line4", this.swSketchSegment, this.nameSideTriangle, 0, 0, 0, size);
      this.swDocument.ClearSelection2(true);
      // Line2 е ъглополовяща на ъгъла между страните Line3 и Line4
      this.Selected("Line2", this.swSketchSegment);
      this.Selected("Line3", this.swSketchSegment, true);
      this.Selected("Line4", this.swSketchSegment, true);
      this.swDocument.SketchAddConstraints("sgSYMMETRIC");
      this.ClearSelection();
      // двете страни на триъгълника са еднакви
      this.Selected("Line3", this.swSketchSegment);
      this.Selected("Line4", this.swSketchSegment, true);
      this.swDocument.SketchAddConstraints("sgSAMELENGTH");
      this.ClearSelection();
      // създава измерване на ъгъла при върха, между Line3 и Line4
      this.AddSize("Line3", this.swSketchSegment, this.nameAngleView, size + 0.03, 0, 0, this.defAngleView, true, "Line4", this.swSketchSegment);
      // създава измерване на ъгъла на отместване на заваръчния шев
      this.AddSize("Line1", this.swSketchSegment, this.nameAngle, 2 * size / 3, 2 * size / 3, 0, 0, true, "Line2", this.swSketchSegment);
      this.RenameFeature("Sketch", this.nameSketchAngle);
      this.InsertSketch();
    } // CreatingWeldingSeamSketch

    /// <summary>завърта на заваръчния шев на исканото ъглово отместване</summary>
    public virtual void RotateWeldSeam() {
      double size = this.diameter / 2 + this.thickness + 0.05;
      // започва редактиране на скицата на ъгъла на заваръчния шев
      if (!this.Selected(this.nameSketchAngle, this.swSketch))
        return;
      this.swDocument.EditSketch();
      // изтрива текущия ъгъл на заваръчния шев
      this.ClearSelection();
      bool status = this.Selected(this.FullName(this.nameAngle, this.nameSketchAngle), this.swDimension);
      this.swDocument.EditDelete();
      if (this.currentAngle != 0) {
        // завърта скицата на 0 градус на заваръчния шев
        this.ClearSelection();
        this.Selected("Line2", this.swSketchSegment);
        this.swExtension.RotateOrCopy(false, 1, true, 0, 0, 0, 0, 0, 1, -this.currentAngle);
        this.ClearSelection();
      }
      // завърта скицата за ъгъла на заваръчния шев
      this.ClearSelection();
      this.Selected("Line2", this.swSketchSegment);
      this.swExtension.RotateOrCopy(false, 1, true, 0, 0, 0, 0, 0, 1, this.RealAngle(this.weldSeamAngle));
      this.ClearSelection();
      this.currentAngle = this.RealAngle(this.weldSeamAngle);
      // създава измерване на ъгъла на отместване на заваръчния шев
      this.AddSize("Line1", this.swSketchSegment, this.nameAngle, 2 * size / 3, 2 * size / 3, 0, 0, true, "Line2", this.swSketchSegment);
      this.ClearSelection();
      this.InsertSketch();
    } // RotateWeldSeam

    /// <summary>изгражда изобразяването на заваръчния шев с отнемане на материал</summary>
    public void CreatingWeldingSeam() {
      // скица на завъръчния шев
      this.CreatingWeldingSeamSketch();
      // ъглово отместване на заваръчния шев
      this.RotateWeldSeam();
      this.Selected(this.nameSketchAngle, this.swSketch);
      this.swDocument.FeatureManager.FeatureCut4(true, false, false,
          1, 0, 0, 0, false, false, false, false, 0, 0,
          false, false, false, false, false, true, true, true, true,
          false, 0, 0, false, false);
      this.RenameFeature("Cut-Extrude", this.nameBodyWS);
      this.swDocument.ClearSelection2(true);
    } // CreatingWeldingSeam

    /// <summary>създава скица на ъгъл</summary>
    /// <param name="plane">наименование на равнината, където се създава скицата</param>
    /// <param name="nameSketch">наименование на скицата</param>
    /// <param name="angle">стойност на ъгъла за първоначално изграждане на скицата</param>
    /// <param name="nameAngle">наименование на ъгъла на завъртане</param>
    /// <param name="angleView">стойност на "видимия" ъгъл</param>
    /// <param name="nameAngleView">наименование на "видимия" ъгъл</param>
    /// <param name="side">размер на рамото на ъгъла</param>
    /// <param name="nameSide">наименование на размера на рамото на ъгъла</param>
    public virtual void AngleSketch(string plane, 
                                    string nameSketch,
                                    double angle, 
                                    string nameAngle, 
                                    double angleView,
                                    string nameAngleView, 
                                    double side,
                                    string nameSide) {
      // крайна точка на рамото, което носи ъгъла
      double x0 = Prime.CosFn(Prime.InRadians(45), side);
      double y0 = Prime.SinFn(Prime.InRadians(45), side);
      // крайна точка на второто рамо на равнобедрения триъгулник
      double x1 = Prime.CosFn(Prime.InRadians(40), side);
      double y1 = Prime.SinFn(Prime.InRadians(40), side);
      // създава скица на равностранен провоъгълник с помощна права по Y оста
      this.SelectPlane(plane);
      this.swSketchManager.CreateCenterLine(0, 0, 0, side, 0, 0);
      this.trSide1 = (SketchLine)this.swSketchManager.CreateLine(0, 0, 0, x0, y0, 0);
      this.trSide2 = (SketchLine)this.swSketchManager.CreateLine(0, 0, 0, x1, y1, 0);
      this.swSketchManager.CreateLine(x0, y0, 0, x1, y1, 0);
      // създава рамер на едно от рамената на триъгълнилка
      this.AddSize("Line2", this.swSketchSegment, nameSide, 0, 0, 0, side);
      //създава ограничение за еднаква дължина на двете рамена на триъгълника
      this.ClearSelection();
      this.Selected("Line2", this.swSketchSegment);
      this.Selected("Line3", this.swSketchSegment, false);
      this.swDocument.SketchAddConstraints(this.sgSameLength);
      // създава ъгъла между двете рамена на триъгулника
      this.AddSize("Line3", this.swSketchSegment, nameAngleView, 0.760, 0.825, 0, Prime.InRadians(10), true, "Line2", this.swSketchSegment);
      // създава ъгълово измерение между Line1 и X оста на координатната система
      this.AddSize("Line1", this.swSketchSegment, nameAngle, 3 * x0 / 2, 3 * y0 / 2, 0, 0, true, "Line2", this.swSketchSegment);
      this.ClearSelection();
      this.InsertSketch();
      this.RenameFeature("Sketch", nameSketch);
      this.RotateAngle(this.trSide2, Prime.InRadians(135), side, nameSketch, nameSide, nameAngle);
    } // AngleSketch

    /// <summary>
    /// завърта скица на зададен ъгъл
    /// </summary>
    /// <param name="line">базова линия в скицата</param>
    /// <param name="angle">ъгъл на завъртане</param>
    /// <param name="side">дължина на рамото</param>
    /// <param name="nameSketch">наименование на скицата</param>
    /// <param name="nameSide">наименование на размера на рамота</param>
    /// <param name="nameAngle">наименование на размера на ъгъла</param>
    /// <param name="nameLine">наименование на базоват линия</param>
    protected void RotateAngle(SketchLine line, double angle, double side, string nameSketch, string nameSide, string nameAngle, string nameLine = "Line2", string nameLinex = "Line3") {
      angle = this.RealAngle(angle);
      if (!this.Selected(nameSketch, this.swSketch))
        return;
      this.swDocument.EditSketch();
      this.ClearSelection();
      this.Selected(this.FullName(nameSide, nameSketch), this.swDimension);
      this.swDocument.EditDelete();
      this.ClearSelection();
      this.Selected(this.FullName(nameAngle, nameSketch), this.swDimension);
      this.swDocument.EditDelete();
      this.ClearSelection();
      double x0 = Prime.CosFn(angle, side);
      double y0 = Prime.SinFn(angle, side);
      double x1 = Prime.CosFn(angle - Prime.InRadians(5), side);
      double y1 = Prime.SinFn(angle - Prime.InRadians(5), side);
      SketchPoint sp = this.trSide1.GetEndPoint2();
      sp.X = 0.005;
      sp.Y = 0.005;
      sp.X = x0;
      sp.Y = y0;
      sp = this.trSide2.GetEndPoint2();
      sp.X = 0.005;
      sp.Y = 0.005;
      sp.X = x1;
      sp.Y = y1;
      this.ClearSelection();
      this.AddSize(nameLine, this.swSketchSegment, nameSide, 0, 0, 0, side);
      this.ClearSelection();
      this.Selected(nameLine, this.swSketchSegment);
      this.Selected(nameLinex, this.swSketchSegment, false);
      this.swDocument.SketchAddConstraints(this.sgSameLength);
      this.AddSize("Line1", this.swSketchSegment, nameAngle, 0, 0, 0, 0, true, nameLine, this.swSketchSegment);
      this.InsertSketch();
    }
    /// <summary>завърта скица на ъгъл спрямо 0 позиция в координатната система</summary>
    /// <param name="nameAngle">наименование на ъгъла за завъртане</param>
    /// <param name="nameSketch">наименование на скицата</param>
    /// <param name="angle">стойност на новия ъгъл</param>
    /// <returns>абсолютна стойност на ъгъла, на който е завъртането</returns>
    protected double RotateAngle_(string nameAngle, string nameSketch, double angle, double side) {
      double rAngle = this.RealAngle(angle);
      // започва редактиране на скицата на ъгъла на заваръчния шев
      if (!this.Selected(nameSketch, this.swSketch))
        return -1;
      this.swDocument.EditSketch();
      // чете текущия ъгъл на скицата
      Dimension dim = this.swDocument.IParameter(this.FullName(nameAngle, nameSketch));
      double[] tmp = (double[])dim.GetSystemValue3((int)swInConfigurationOpts_e.swThisConfiguration, (this.swDocument.GetConfigurationNames()));
      double currentAngle = rAngle - tmp[0];
      // изтрива текущия ъгъл на заваръчния шев
      this.ClearSelection();
      bool status = this.Selected(this.FullName(nameAngle, nameSketch), this.swDimension);
      this.swDocument.EditDelete();
      
      if (tmp[0] != 0) {
        this.ClearSelection();
        this.Selected("Line2", this.swSketchSegment, true);
        this.Selected("Line3", this.swSketchSegment, true);
        this.Selected("Line4", this.swSketchSegment, true);
        this.Selected("Line5", this.swSketchSegment, true);
        this.swExtension.RotateOrCopy(false, 1, true, 0, 0, 0, 0, 0, 1, -tmp[0]);
      }
      // завърта скицата за ъгъла на заваръчния шев
      currentAngle = this.RealAngle(angle);
      
      this.ClearSelection();
      this.Selected("Line2", this.swSketchSegment, true);
      this.Selected("Line3", this.swSketchSegment, true);
      this.Selected("Line4", this.swSketchSegment, true);
      this.Selected("Line5", this.swSketchSegment, true);
      this.swExtension.RotateOrCopy(false, 1, true, 0, 0, 0, 0, 0, 1, rAngle);
      // създава размер между ъглополовящата и X-оста на координатната система
      if (rAngle > Prime.InRadians(180))
        rAngle = Prime.InRadians(360) - rAngle;
      this.AddSize("Line1", this.swSketchSegment, nameAngle, side / 2, side / 2, 0, rAngle, true, "Line2", this.swSketchSegment);
      this.InsertSketch();
      return rAngle;
    } // RotateAngle

    

    #endregion Creating Feature










    #region Old properties and methods







    /// <summary>наименование на тип за равнина в SW</summary>
    //public virtual string swPlane { get { return "PLANE"; } }
    /// <summary>наименование на тип за линия в SW</summary>
    public virtual string swLine { get { return "LINE"; } }
    /// <summary>наименование на тип за тяло в SW</summary>
    public virtual string swBody { get { return "BODYFEATURE"; } }
    /// <summary>наименование на тип за размер в SW</summary>
    public virtual string swDimension { get { return "DIMENSION"; } }
    
    /// <summary>стандартно наименование на изтегляне на материал с дебелина на стена</summary>
    public virtual string swExtrudeThin { get { return "Extrude-Thin"; } }
    /// <summary>стандартно наименование на изтегляне с отнемане на материал</summary>
    public virtual string swCutExtrude { get { return "Cut-Extrude"; } }

    

    

    

    /// <summary>изтрива ограничение за елемент от скица</summary>
    /// <param name="nameSketchSegment">наименование на елемента</param>
    /// <param name="nameConstraint">наименование на ограничението</param>
    protected virtual void DelConstraints(string nameSketchSegment, string nameConstraint) {
      Sketch sketch = this.swSketchManager.ActiveSketch;
      object[] segments = (object[])sketch.GetSketchSegments();
      foreach (SketchSegment segment in segments) {
        string name = segment.GetName().ToLower();
        if (name.Equals(nameSketchSegment.ToLower())) {
          string[] constraints = (string[])segment.GetConstraints();
          for (int j = 0; j < constraints.Length; j++) {
            if (nameConstraint.Equals(constraints[j])) {
              this.swDocument.SketchConstraintsDel(j, nameConstraint);
              break;
            }
          }
          break;
        }
      }
    } // DelConstraints

    

    

    

    

    

    /// <summary>
    /// проверява дали фичър с <paramref name="name"/> е последен в дървото
    /// </summary>
    /// <param name="name">наименование на фичъра</param>
    /// <returns>TRUE последен фичър</returns>
    public override bool IsLastFeature(string name) {
      Feature feature = this.GetFeature(name);
      if (feature != null) {
        feature = feature.IGetNextFeature();
      } else {
        return false;
      }
      return (feature == null);
    } // IsLastFeature

    

    

    /// <summary>
    /// създава правоъгълник с пресечна точка на диагоналите в началото
    /// на координатната система
    /// </summary>
    /// <param name="width">ширина в метри</param>
    /// <param name="length">дължина в метри</param>
    /// <param name="nameWidth">наименование на размера по ширината</param>
    /// <param name="nameLength">наименование на размера по дължината</param>
    /// <param name="nameSketch">наименование на скицата</param>
    public virtual void Rectangle(double width, double length, string nameWidth, string nameLength, string nameSketch) {
      this.swDocument.SketchManager.CreateCenterRectangle(0, 0, 0, length / 2, width / 2, 0);
      this.AddSize("Line1", "SKETCHSEGMENT", nameLength, 0, 0, width / 2 + 0.01);
      this.AddSize("Line2", "SKETCHSEGMENT", nameWidth, length / 2 + 0.01, 0, 0);
      this.ClearSelection();
      this.RenameFeature("Sketch", nameSketch);
      this.InsertSketch();
    } // Rectangle

    /// <summary>създава окръжност по център и радиус</summary>
    /// <param name="x">X координата на центъра</param>
    /// <param name="y">Y координата на центъра</param>
    /// <param name="z">Z координата на центъра</param>
    /// <param name="diameter">диаметър</param>
    /// <param name="nameDiameter">наименование на размера на диаметъра</param>
    /// <param name="nameSketch">наименование на скицата</param>
    public virtual void Circle(double x, double y, double z, double diameter, string nameDiameter, string nameSketch) {
      double radius = diameter / 2;
      if (radius < 0.003)
        radius = 0.003;
      this.swDocument.SketchManager.CreateCircleByRadius(x, y, z, radius);
      this.AddSize("Arc1", "SKETCHSEGMENT", nameDiameter, radius, 0, 0, diameter);
      this.ClearSelection();
      this.RenameFeature("Sketch", nameSketch);
    } // Circle

    /// <summary>изгражда 3D модела от избрана скица </summary>
    /// <param name="sketch">наименование на скицата</param>
    /// <param name="height">височина на 3D модела</param>
    /// <param name="nameHeight">наименование на размера за височина</param>
    /// <param name="nameExtrude">наименование на 3D модела</param>
    /// <param name="thin">дебелина на стената</param>
    /// <param name="nameThin">наименование на размера на дебелината на стената</param>
    /// <param name="thinDir">посока на стената, 1 навътре от диаметъра, 0 - навън</param>
    public virtual void Extrusion(string sketch, double height, string nameHeight, 
      string nameExtrude, double thin = -1, string nameThin = null, int thinDir = 1) {
      this.ClearSelection();
      this.swDocument.Extension.SelectByID2(sketch, "SKETCH", 0, 0, 0,
        false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
      if (thin < 0) {
        this.swDocument.FeatureManager.FeatureExtrusion3(true, false, false,
          (int)swEndConditions_e.swEndCondBlind, 0, height, 0,
          false, false, false, false, 0, 0, false, false, false, false, false,
          false, true, (int)swStartConditions_e.swStartSketchPlane, 0, false);
        this.RenameFeature("Boss-Extrude", nameExtrude);
      } else {
        this.swDocument.FeatureManager.FeatureExtrusionThin2(true, false, false,
          (int)swEndConditions_e.swEndCondBlind, 0, height, 0, 
          false, false, false, false, 0, 0, false, false, false, false, true,
          thin, 0, 0, thinDir, 0, false, 0, true, true,
          (int)swStartConditions_e.swStartSketchPlane, 0, false);
        this.RenameFeature("Extrude-Thin", nameExtrude);
        Dimension d1 = this.swDocument.IParameter("D5@" + nameExtrude);
        d1.Name = nameThin;
        d1.ReadOnly = true;
      }
      Dimension d = this.swDocument.IParameter("D1@" + nameExtrude);
      d.Name = nameHeight;
      d.ReadOnly = true;
    } // Extrusion

    /// <summary>
    /// Създава работна повърхнина
    /// </summary>
    /// <param name="sketch">скица за повърхнината</param>
    /// <param name="height">височина на повърхнината</param>
    /// <param name="name">наименование на повърхнината</param>
    public virtual void ExtrudeSurface(string sketch, double height, string name) {
      this.ClearSelection();
      this.swDocument.Extension.SelectByID2(sketch, "SKETCH", 0, 0, 0, false, 0, null, 0);
      this.swDocument.FeatureManager.FeatureExtruRefSurface3(true, false,
        (int)swStartConditions_e.swStartSketchPlane, 0,
        (int)swEndConditions_e.swEndCondBlind,
        (int)swEndConditions_e.swEndCondBlind, height, height,
        false, false, false, false, 0, 0, false, false, false, false, false,
        false, false, false);
      this.RenameFeature("Surface-Extrude", name);
      this.ClearSelection();
    } // ExtrudeSurface

    /// <summary>
    /// Създава работна равнина спрямо basePlane завъртяна на ъгъл angle 
    /// </summary>
    /// <param name="basePlane">наименование на базовата равнина</param>
    /// <param name="angle">ъгъл на завъртане</param>
    /// <param name="namePlane">наименование на новата равнина</param>
    /// <param name="nameAngle">наименование на размера на ъгъла</param>
    public virtual void CreatePlane(string basePlane, double angle, string namePlane, string nameAngle) {
      this.ClearSelection();
      this.swExtension.SelectByID2(basePlane, this.swPlane, 0, 0, 0, true, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
      bool status = this.swDocument.Extension.SelectByID2("", this.swAxis, 0, 0, 0, true, 1, null, (int)swSelectOption_e.swSelectOptionDefault);
      this.swFeatureManager.InsertRefPlane((int)swRefPlaneReferenceConstraints_e.swRefPlaneReferenceConstraint_Angle, angle, (int)swRefPlaneReferenceConstraints_e.swRefPlaneReferenceConstraint_Coincident, 0, 0, 0);
      this.RenameFeature("Plane", namePlane);
      Dimension d = this.swDocument.IParameter(Prime.Name("D1", namePlane));
      d.Name = nameAngle;
      d.ReadOnly = true;
    } // CreatePlane



    #endregion Old properties and methods
    #endregion Before 2019-04-08
  } // PatternBase

}
