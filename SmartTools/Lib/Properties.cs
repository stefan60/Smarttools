using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SmartTools.Lib {
  /// <summary>
  /// Клас за работа с описанията на модела
  /// </summary>
  public class Properties {
    /// <summary>наименование на Custom раздела във file Properties</summary>
    public const string CUSTOM_TAB = "";
    /// <summary>наименование на конфигурацията на модела</summary>
    public const string CONFIGURATION_SPECIFIC = "Default";
    /// <summary>ключ към ъгъл използван в модела</summary>
    const string NAME_ANGLE = "Angle";
    /// <summary>ключ към сертификата на модела</summary>
    const string NAME_CERTIFICATE = "Certificates";
    /// <summary>ключ към посока на нарастване на ъгъла в координатната система</summary>
    const string NAME_CLOCKWISE = "Clockwise";
    /// <summary>ключ към създателя на модела</summary>
    const string NAME_CREATOR = "Is Created";
    /// <summary>ключ към описанието на модела</summary>
    const string NAME_DESCRIPTION = "Description";
    /// <summary>ключ към диаметъра на модела</summary>
    const string NAME_DIAMETER = "Diameter Value";
    /// <summary>ключ към черточния номер на модела</summary>
    const string NAME_DRAWING_NUMBER = "Drawing Number";
    /// <summary>ключ към височината на модела</summary>
    const string NAME_HEIGHT = "Height value";
    /// <summary>ключ към флаг посочващ че диаметъра е вътрешен</summary>
    const string NAME_INNER_DIAMETER = "Inner Diameter";
    /// <summary>ключ към дължината на модела</summary>
    const string NAME_LENGTH = "Length Value,Depth Value";
    /// <summary>ключ към катериала на модела</summary>
    const string NAME_MATERIAL = "Material";
    /// <summary>ключ към флага указващ наличие на чертеж към модела</summary>
    const string NAME_NODRAWING = "No Drawing";
    /// <summary>ключ към забележка на модела</summary>
    const string NAME_NOTE = "Note";
    /// <summary>ключ към типа на модела</summary>
    const string NAME_PATTERN = "Pattern";
    /// <summary>позиция на 0 градуса в координатната система</summary>
    const string NAME_POSITION0 = "Position 0";
    /// <summary>ключ към качеството на модела</summary>
    const string NAME_QUALITY = "Quality";
    /// <summary>ключ към стандарт на модел</summary>
    const string NAME_STANDARD = "Standard";
    /// <summary>ключ към теста на модела</summary>
    const string NAME_TEST = "Test";
    /// <summary>ключ към дебелината на модела, "Depth Value" е остарял ключ</summary>
    const string NAME_THICKNESS = "Thickness Value,Depth Value";
    /// <summary>ключ към ъгъл на заваръчен шев</summary>
    const string NAME_WELDINGSEAM = "Welding Seam in deg";
    /// <summary>ключ към ширината на модела</summary>
    const string NAME_WIDTH = "Width Value";
    /// <summary>ключ към брояча на отворите</summary>
    const string NAME_HOLE_NUMBER = "Hole Number";
    /// <summary>ключ към брояча на избушени навън отворите</summary>
    const string NAME_OUTHOLE_NUMBER = "OutHole Number";
    /// <summary>ключ към брояча на избушени навътре отворите</summary>
    const string NAME_INHOLE_NUMBER = "InHole Number";
    /// <summary>ключ към ъгъла на наклона</summary>
    const string NAME_ANGLE_INCLINATION = "Angle Inclination";
    /// <summary>ключ към радиуса на берта</summary>
    const string NAME_BEND_RADIUS = "Bend Radius";
    /// <summary>ключ към отстояние от центъра</summary>
    const string NAME_DISTANCE = "Distance value";
    /// <summary>максимално допустина стойност на разнер</summary>
    const double MAX_VALUE = 999999;
    /// <summary>
    /// обект за достъп до информацията с характеристиките на модела
    /// </summary>
    protected ICustomPropertyManager iCPManager = default(ICustomPropertyManager);

    /// <summary>
    /// създава обекта за обработка на характеристиките на модела
    /// </summary>
    /// <param name="model">обект за достъп до модела</param>
    /// <param name="configName">наименование на конфигурацията</param>
    public Properties(IModelDoc2 model, string configName = Properties.CUSTOM_TAB) {
      this.ChangeConfiguration(model, configName);
    } // Properties

    /// <summary>
    /// получава достъп до конфигурация <paramref name="configName"/> за
    /// модела <paramref name="model"/>
    /// </summary>
    /// <param name="model">обект за достъп до модела</param>
    /// <param name="configName">наименование на конфигурацията</param>
    public void ChangeConfiguration(IModelDoc2 model, string configName) {
      this.iCPManager = model.Extension.get_CustomPropertyManager(configName);
    } // ChangeConfiguration

    /// <summary>
    /// съхранява общата информация за модела
    /// </summary>
    /// <param name="pattern">тип на модела</param>
    /// <param name="creator">създател на модела</param>
    public void Header(Prime.Patterns_e pattern, string creator = Prime.ADDIN_NAME) {
      if (creator != null)
        this.Creator(Prime.ADDIN_NAME);
      this.Pattern(pattern);
    } // Header

    /// <summary>
    /// съхранява информация за характеристиките на модела
    /// </summary>
    /// <param name="material">материал</param>
    /// <param name="standard">стандарт</param>
    /// <param name="quality">качество</param>
    public void Characteristics(string material = null, string standard = null, string quality = null) {
      if (material != null)
        this.Material(material);
      if (standard != null)
        this.Standard(standard);
      if (quality != null)
        this.Quality(quality);
    } // Characteristics

    /// <summary>
    /// съхранява допълнителната информация към модела
    /// </summary>
    /// <param name="certificate">сертификат</param>
    /// <param name="test">тест</param>
    /// <param name="noDrawing">флаг за наличие на чертеж</param>
    public void Complements(string certificate = null, string test = null, bool noDrawing = false) {
      if (certificate != null)
        this.Certificate(certificate);
      if (test != null)
        this.Test(test);
      this.NoDrawing(noDrawing);
    } // Complements

    /// <summary>съхранява информацията за габаритите на модела</summary>
    /// <param name="diameter">диаметър</param>
    /// <param name="thickness">дебелина</param>
    /// <param name="width">ширина</param>
    /// <param name="length">дължина</param>
    /// <param name="height">височина</param>
    /// <param name="weldingSeam">ъгъл на заваръчния шев</param>
    public void Dimension(double diameter = Properties.MAX_VALUE,
      double thickness = Properties.MAX_VALUE, double width = Properties.MAX_VALUE,
      double length = Properties.MAX_VALUE, double height = Properties.MAX_VALUE,
      double weldingSeam = Properties.MAX_VALUE, double angleInclination = Properties.MAX_VALUE,
      double bendRadius = Properties.MAX_VALUE, double distance = Properties.MAX_VALUE) {
      if (thickness < Properties.MAX_VALUE)
        this.Thickness(thickness);
      if (width < Properties.MAX_VALUE)
        this.Width(width);
      if (length < Properties.MAX_VALUE)
        this.Length(length);
      if (diameter < Properties.MAX_VALUE)
        this.Diameter(diameter);
      if (weldingSeam < Properties.MAX_VALUE)
        this.WeldingSeam(weldingSeam);
      if (height < Properties.MAX_VALUE)
        this.Height(height);
      if (angleInclination < Properties.MAX_VALUE)
        this.AngleInclination(angleInclination);
      if (bendRadius < Properties.MAX_VALUE)
        this.BendRadius(bendRadius);
      if (distance < Properties.MAX_VALUE)
        this.Distance(distance);
    } // Dimension

    /// <summary>
    /// съхранява информация за координатната система
    /// </summary>
    /// <param name="clockwise">посока на нарастване на ъгъла</param>
    /// <param name="nort">позиция север 0</param>
    /// <param name="east">позиция изток 0</param>
    /// <param name="sout">позиция юг 0</param>
    /// <param name="west">позиция запад 0</param>
    public void CoordinateSystem(bool clockwise, bool nort, bool east, bool sout, bool west) {
      this.Clockwise(clockwise);
      this.Position0(nort, east, sout, west);
    } // CoordinateSystem

    /// <summary>
    /// връща съдържанието на описание с наименование <paramref name="name"/>
    /// </summary>
    /// <param name="name">наименование на описанието</param>
    /// <returns>съдържание на описанието, NULL ако не съществува описание</returns>
    public string Get(string name) {
      int result = this.iCPManager.Get6(name, false, out string data, 
        out string resolvedVal, out bool wasResolved, out bool link);
      if (result == (int)swCustomInfoGetResult_e.swCustomInfoGetResult_NotPresent)
        return null;
      return data;
    } // Get

    /// <summary>
    /// съхранява съдържанието на описание <paramref name="data"/> с
    /// наименование <paramref name="name"/>
    /// </summary>
    /// <param name="name">наименование на описанието</param>
    /// <param name="data">съдържание на описанието</param>
    public void Set(string name, string data) {
      this.iCPManager.Add3(name, (int)swCustomInfoType_e.swCustomInfoText,
        data, (int)swCustomPropertyAddOption_e.swCustomPropertyDeleteAndAdd);
    } // Set

    /// <summary>връща стрин като реално число</summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public double ToDouble(string data) {
      double dig = 0;
      if (data != null && data.Length > 0) {
        try {
          dig = Convert.ToDouble(data);
        } catch (FormatException) {
          dig = 0;
        } catch (OverflowException) {
          dig = 0;
        }
      }
      return dig;
    } // ToDouble

    /// <summary>връща съдържанието за създателя на модела</summary>
    /// <returns>създател на модела</returns>
    public string Creator() {
      return this.Get(Properties.NAME_CREATOR);
    } // Creator
    
    /// <summary>съхранява създателя на модела</summary>
    /// <param name="data">създател на модела</param>
    public void Creator(string data) {
      this.Set(Properties.NAME_CREATOR, data);
    } // Creator

    /// <summary>връща типа на модела</summary>
    /// <returns>тип на модела</returns>
    public string Pattern() {
      return this.Get(Properties.NAME_PATTERN);
    } // Pattern

    /// <summary>съхранява типа на  модела</summary>
    /// <param name="data">тип на модела</param>
    public void Pattern(Prime.Patterns_e data) {
      this.Set(Properties.NAME_PATTERN, data.ToString());
    } // Pattern

    /// <summary>връща материала на модела</summary>
    /// <returns>материал на модела</returns>
    public string Material() {
      return this.Get(Properties.NAME_MATERIAL);
    } // Material

    /// <summary>съхранява мтериала на модела</summary>
    /// <param name="data">материал на модела</param>
    public void Material(string data) {
      this.Set(Properties.NAME_MATERIAL, data);
    } // Material

    /// <summary>връща сертификата на модела</summary>
    /// <returns>сертифицат на модела</returns>
    public string Certificate() {
      return this.Get(Properties.NAME_CERTIFICATE);
    } // Certificate

    /// <summary>съхранява сертификата на модела</summary>
    /// <param name="data">сертификат на модела</param>
    public void Certificate(string data) {
      this.Set(Properties.NAME_CERTIFICATE, data);
    } // Certificate

    /// <summary>връща теста на модела</summary>
    /// <returns>тест на модела</returns>
    public string Test() {
      return this.Get(Properties.NAME_TEST);
    } // Test

    /// <summary>съхранява теста на модела</summary>
    /// <param name="data">тест на модела</param>
    public void Test(string data) {
      this.Set(Properties.NAME_TEST, data);
    } // Test

    /// <summary>връща флага указващ наличие на чертеж към модела</summary>
    /// <returns>флаг за наличие на чертеж към модела</returns>
    public bool NoDrawing() {
      return Convert.ToBoolean(this.Get(Properties.NAME_NODRAWING));
    } // NoDrawing

    /// <summary>съхранява флага указващ наличие на чертеж към модела</summary>
    /// <param name="data">флаг за наличие на чертеж ъм модела</param>
    public void NoDrawing(bool data) {
      this.Set(Properties.NAME_NODRAWING, data.ToString());
    } // NoDrawing

    /// <summary>връща чертожния номер на модела</summary>
    /// <returns>чертожен номер</returns>
    public string DrawingNumber() {
      return this.Get(Properties.NAME_DRAWING_NUMBER);
    } // DrawingNumber

    /// <summary>съхранява чертожния номер на модела</summary>
    /// <param name="data">чертожен номер</param>
    public void DrawingNumber(string data) {
      this.Set(Properties.NAME_DRAWING_NUMBER, data);
    } // DrawingNumber

    /// <summary>връща дебелината на модела</summary>
    /// <returns>дебелина на модела</returns>
    public double Thickness() {
      string[] keys = Properties.NAME_THICKNESS.Split(',');
      string item = this.Get(keys[0]);
      if (item == null)
        item = this.Get(keys[1]);
      return this.ToDouble(item);
    } // Thickness

    /// <summary>съхранява дебелината на модела</summary>
    /// <param name="data">дебелина на модела</param>
    public void Thickness(double data) {
      this.Set(Properties.NAME_THICKNESS.Split(',')[0], data.ToString());
    } // Thickness

    /// <summary>връща ширината на модела</summary>
    /// <returns>ширина на модела</returns>
    public double Width() {
      return this.ToDouble(this.Get(Properties.NAME_WIDTH));
    } // Width
    
    /// <summary>съхранява ширината на модела</summary>
    /// <param name="data">ширина на модела</param>
    public void Width(double data) {
      this.Set(Properties.NAME_WIDTH, data.ToString());
    } // Width

    /// <summary>връща дължината на модела</summary>
    /// <returns>дължина на модела</returns>
    public double Length() {
      string[] keys = Properties.NAME_LENGTH.Split(',');
      string item = this.Get(keys[0]);
      if (item == null)
        item = this.Get(keys[1]);
      return this.ToDouble(item);
    } // Length

    /// <summary>съхранява дължината на модела</summary>
    /// <param name="data">дължина на модела</param>
    public void Length(double data) {
      this.Set(Properties.NAME_LENGTH.Split(',')[0], data.ToString());
    } // Length

    /// <summary>връща описанието на модела</summary>
    /// <returns>описание</returns>
    public string Description() {
      return this.Get(Properties.NAME_DESCRIPTION);
    } // Description

    /// <summary>съхранява описанието на модела</summary>
    /// <param name="data">описание</param>
    public void Description(string data) {
      this.Set(Properties.NAME_DESCRIPTION, data);
    } // Description

    /// <summary>връща забележката към модела</summary>
    /// <returns>забележка</returns>
    public string Note() {
      return this.Get(Properties.NAME_NOTE);
    } // Note

    /// <summary>съхранява забележка към модела</summary>
    /// <param name="data">забележка</param>
    public void Note(string data) {
      this.Set(Properties.NAME_NOTE, data);
    } // Note

    /// <summary>връща диаметъра на модела</summary>
    /// <param name="noDiameter">номер на диаметър, използва се при отворите</param>
    /// <returns>диаметър</returns>
    public double Diameter(string noDiameter = "") {
      return this.ToDouble(this.Get(Properties.NAME_DIAMETER + noDiameter));
    } // Diameter

    /// <summary>съхранява диаметра на модела</summary>
    /// <param name="noDiameter">номер на диаметър, използва се при отворите</param>
    /// <param name="data">диаметър</param>
    public void Diameter(double data, string noDiameter = "") {
      this.Set(Properties.NAME_DIAMETER + noDiameter, data.ToString());
    } // Diameter

    /// <summary>връща стандарта на модела</summary>
    /// <returns>стандарт</returns>
    public string Standard() {
      return this.Get(Properties.NAME_STANDARD);
    } // Standard

    /// <summary>съхранява стандарта на модела</summary>
    /// <param name="data">стандарт</param>
    public void Standard(string data) {
      this.Set(Properties.NAME_STANDARD, data);
    } // Standard

    /// <summary>връща качеството на модела</summary>
    /// <returns>качество</returns>
    public string Quality() {
      return this.Get(Properties.NAME_QUALITY);
    } // Quality

    /// <summary>съхранява качеството на модела</summary>
    /// <param name="data">качество</param>
    public void Quality(string data) {
      this.Set(Properties.NAME_QUALITY, data);
    } // Quality

    /// <summary>връща ъгъла на заваръчния шев на модела</summary>
    /// <returns>ъгъла на заваръчния шев</returns>
    public double WeldingSeam() {
      return this.ToDouble(this.Get(Properties.NAME_WELDINGSEAM)) * Prime.ONE_DEGREE;
    } // WeldingSeam

    /// <summary>съхранява ъгъла на заваръчния шев на модела</summary>
    /// <param name="data">ъгъла на заваръчния шев</param>
    public void WeldingSeam(double data) {
      this.Set(Properties.NAME_WELDINGSEAM, (data / Prime.ONE_DEGREE).ToString("0.0"));
    } // WeldingSeam

    /// <summary>
    /// връща посоката на нарастване на ъгъла в координатната система
    /// </summary>
    /// <returns>посока на нарастване на ъгъла</returns>
    public bool Clockwise() {
      return Convert.ToBoolean(this.Get(Properties.NAME_CLOCKWISE));
    } // Clockwise

    /// <summary>
    /// съхранява посоката на нарастване на ъгъла в координатната система
    /// </summary>
    /// <param name="data">посока на нарастване на ъгъла</param>
    public void Clockwise(bool data) {
      this.Set(Properties.NAME_CLOCKWISE, data.ToString());
    } // Clockwise

    /// <summary>връща височината на модела</summary>
    /// /// <param name="noHeight">номер на височината, използва се при отворите</param>
    /// <returns>височина</returns>
    public double Height(string noHeight = "") {
      return this.ToDouble(this.Get(Properties.NAME_HEIGHT + noHeight));
    } // Height

    /// <summary>съхранява височината на модела</summary>
    /// <param name="noHeight">номер на височината, използва се при отворите</param>
    /// <param name="data">височина</param>
    public void Height(double data, string noHeight = "") {
      this.Set(Properties.NAME_HEIGHT + noHeight, data.ToString());
    } // Height

    /// <summary>връща отстоянието от центъра</summary>
    /// /// <param name="noHeight">номер на отстоянието, използва се при отворите</param>
    /// <returns>височина</returns>
    public double Distance(string noHeight = "") {
      return this.ToDouble(this.Get(Properties.NAME_DISTANCE + noHeight));
    } // Distance

    /// <summary>съхранява отстоянието до центъра</summary>
    /// <param name="noHeight">номер на височината, използва се при отворите</param>
    /// <param name="data">отстоянието от центъра</param>
    public void Distance(double data, string noHeight = "") {
      this.Set(Properties.NAME_DISTANCE + noHeight, data.ToString());
    } // Distance

    /// <summary>
    /// връща 0 градуса на координатната система
    /// </summary>
    /// <returns>позиция на 0 градуса на координатната система</returns>
    public string Position0() {
      return this.Get(Properties.NAME_POSITION0);
    } // Position0

    /// <summary>
    /// съхранява 0 градуса на координатната система
    /// </summary>
    public void Position0(bool nort, bool east, bool sout, bool west) {
      if (nort)
        this.Set(Properties.NAME_POSITION0, Prime.Controls_e.NortPosition.ToString());
      if (east)
        this.Set(Properties.NAME_POSITION0, Prime.Controls_e.EastPosition.ToString());
      if (sout)
        this.Set(Properties.NAME_POSITION0, Prime.Controls_e.SoutPosition.ToString());
      if (west)
        this.Set(Properties.NAME_POSITION0, Prime.Controls_e.WestPosition.ToString());
    } // Position0

    /// <summary>
    /// връща флага за типа на диаметъра
    /// </summary>
    /// <returns>тип на диаметъра, вътрешен/външен</returns>
    public bool InnerDiameter() {
      return Convert.ToBoolean(this.Get(Properties.NAME_INNER_DIAMETER));
    } // InnerDiameter

    /// <summary>съхранява типа на диаметъра</summary>
    /// <param name="data">тип на диаметъра, вътрешен/външен</param>
    public void InnerDiameter(bool data) {
      this.Set(Properties.NAME_INNER_DIAMETER, data.ToString());
    } // InerDiameter

    /// <summary>връща номера на последния отвор</summary>
    /// <returns>номер на отвора</returns>
    public int HoleNumber() {
      int num;
      try {
        num = Convert.ToInt32(this.Get(Properties.NAME_HOLE_NUMBER));
      } catch (FormatException) {
        num = 0;
      } catch (OverflowException) {
        num = 0;
      }
      return num;
    } // HoleNumber

    /// <summary>съхранява номера на последния отвор</summary>
    /// <param name="hole">номер на отвора</param>
    public void HoleNumber(int hole) {
      this.Set(Properties.NAME_HOLE_NUMBER, hole.ToString());
    } // HoleNumber

    /// <summary>връща ъгъла използван в модела</summary>
    /// <param name="noAngle">номер на ъгъл, използва се при отворите</param>
    /// <returns>ъгъл използван в модела</returns>
    public double Angle(string noAngle = "") {
      return this.ToDouble(this.Get(Properties.NAME_ANGLE + noAngle)) * Prime.ONE_DEGREE;
    } // Angle

    /// <summary>съхранява ъгъла на заваръчния шев на модела</summary>
    /// <param name="noAngle">номер на ъгъл, използва се при отворите</param>
    /// <param name="data">ъгъла на заваръчния шев</param>
    public void Angle(double data, string noAngle = "") {
      this.Set(Properties.NAME_ANGLE + noAngle, (data / Prime.ONE_DEGREE).ToString("0.0"));
    } // Angle

    /// <summary>връща номера на последния избушен навън отвор</summary>
    /// <returns>номер на отвора</returns>
    public int OutHoleNumber() {
      int num;
      try {
        num = Convert.ToInt32(this.Get(Properties.NAME_OUTHOLE_NUMBER));
      } catch (FormatException) {
        num = 0;
      } catch (OverflowException) {
        num = 0;
      }
      return num;
    } // OutHoleNumber

    /// <summary>съхранява номера на последния избушен навън отвор</summary>
    /// <param name="hole">номер на отвора</param>
    public void OutHoleNumber(int hole) {
      this.Set(Properties.NAME_OUTHOLE_NUMBER, hole.ToString());
    } // OutHoleNumber

    /// <summary>връща номера на последния избушен навътре отвор</summary>
    /// <returns>номер на отвора</returns>
    public int InHoleNumber() {
      int num;
      try {
        num = Convert.ToInt32(this.Get(Properties.NAME_INHOLE_NUMBER));
      } catch (FormatException) {
        num = 0;
      } catch (OverflowException) {
        num = 0;
      }
      return num;
    } // InHoleNumber

    /// <summary>съхранява номера на последния избушен навътре отвор</summary>
    /// <param name="hole">номер на отвора</param>
    public void InHoleNumber(int hole) {
      this.Set(Properties.NAME_INHOLE_NUMBER, hole.ToString());
    } // InHoleNumber

    /// <summary>връща ъгъла на наклона на модела</summary>
    /// <returns>ъгъл на наклона на модела</returns>
    public double AngleInclination() {
      return this.ToDouble(this.Get(Properties.NAME_ANGLE_INCLINATION)) * Prime.ONE_DEGREE;
    } // AngleInclination

    /// <summary>съхранява ъгъла на наклона на модела</summary>
    /// <param name="data">дебелина на модела</param>
    public void AngleInclination(double data) {
      this.Set(Properties.NAME_ANGLE_INCLINATION, (data / Prime.ONE_DEGREE).ToString("0.0"));
    } // AngleInclination

    /// <summary>връща радиуса на берта на модела</summary>
    /// <returns>радиус на берта на модела</returns>
    public double BendRadius() {
      return this.ToDouble(this.Get(Properties.NAME_BEND_RADIUS));
    } // BendRadius

    /// <summary>съхранява радиуса на берта на модела</summary>
    /// <param name="data">радиус на берта на модела</param>
    public void BendRadius(double data) {
      this.Set(Properties.NAME_BEND_RADIUS, data.ToString());
    } // BendRadius
  } // Properties

}
