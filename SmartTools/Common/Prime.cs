using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolidWorks.Interop.swconst;

namespace SmartTools {
  /// <summary>Основни характеристики на приложението</summary>
  public static class Prime {

    /// <summary>флаг оределящ режима на работа</summary>
    /// <date>2019-04-08</date>
    public const bool TEST = true;

    /// <summary>височина на избущен отвор</summary>
    /// <date>2019-04-08</date>
    public const double HEIHGT_EXPLODE_HOLE = 0.01;
    /// <summary>радиани за 360 градуса</summary>
    /// <date>2019-04-08</date>
    public const double RADIANS_360DEG = ONE_DEGREE * 360;

    /// <summary>Тип на групите в PropertyManager Page</summary>
    /// <date>2019-04-08</date>
    public enum Groups_e {
      /// <summary>габаритни размери</summary>
      Dimension = 30,
      /// <summary>характеристики на материала</summary>
      Characteristics,
      /// <summary>допълнителна информация</summary>
      Complement,
      /// <summary>координатна система</summary>
      CoordinateSystem
    } // Groups_e

    /// <summary>Моделите използвани от приложението</summary>
    /// <date>2019-04-08</date>
    public enum Patterns_e {
      /// <summary>Модел "планка"</summary>
      Plate = 5,
      /// <summary>Модел "ос"</summary>
      Axis,
      /// <summary>Модел "тръба"</summary>
      Tube,
      /// <summary>Модел "мантел"</summary>
      Mantel,
      /// <summary>Модел "капак"</summary>
      Cover,
      /// <summary>Модел "дъно"</summary>
      Bottom,
      /// <summary>инструмент за нормален отвор</summary>
      Hole,
      /// <summary>инструмент за избушен отвор навън</summary>
      OutHole,
      /// <summary>инструмент за избушен отвор навътре</summary>
      InHole,
      /// <summary>инструмент за редактиране на модел</summary>
      EditPattern
    } // Patterns_e

    /// <summary>контроли използвани в swPMPage</summary>
    /// <date>2019-04-08</date>
    public enum Controls_e {
      /// <summary>контрола за избор на материал</summary>
      Material = 10,
      /// <summary>контрола за избор на сертификат</summary>
      Certificate,
      /// <summary>контрола за избор на тест</summary>
      Test,
      /// <summary>контрола за въвеждане/избор на диаметър</summary>
      Diameter,
      /// <summary>контрола за въвеждане/избор на дебелина</summary>
      Thickness,
      /// <summary>контрола за въвеждане на височина</summary>
      Height,
      /// <summary>контрола за въвеждане ъгъла на заваръчния шев</summary>
      WeldingSeam,
      /// <summary>контрола за въвеждане на дължина</summary>
      Length,
      /// <summary>контрола за въвеждане на ширина</summary>
      Width,
      /// <summary>контрола за избор на стандарт</summary>
      Standard,
      /// <summary>контрола за избор на качество</summary>
      Quality,
      /// <summary>контрола за избор без чертеж на детайл</summary>
      NoDrawing,
      /// <summary>контрола за избор на посока по часовниковата стрелка</summary>
      Clockwise,
      /// <summary>контрола за избор на посока против часовниковата стрелка</summary>
      AntiClockwise,
      /// <summary>контрола за избор на посока север на 0</summary>
      NortPosition,
      /// <summary>контрола за избор на посока изток на 0</summary>
      EastPosition,
      /// <summary>контрола за избор на посока юг на 0</summary>
      SoutPosition,
      /// <summary>контрола за избор на посока запад на 0</summary>
      WestPosition,
      /// <summary>контрола за избор на ъглово отместване на отвор</summary>
      AngularOffset,
      /// <summary>контрола за избор на наклона</summary>
      AngleInclination,
      /// <summary>контрола за избор на рдиуса на бертване</summary>
      BendRadius,
      /// <summary>контрола за избор на разстоянието от центъра на координатната система до центъра на окръзност</summary>
      Distance,
      /// <summary>начален уникален номер на етикетите към контролите</summary>
      idLabel
    } // Controls_e

    /// <summary>контроли за вида надпис</summary>
    /// <date>2019-04-08</date>
    public enum ControlElement_e {
      /// <summary>надпис на елемент</summary>
      Caption,
      /// <summary>надпис само за радио бутон</summary>
      Caption_rb,
      /// <summary>подсказка за елемент</summary>
      Tooltip,
      /// <summary>съобщение за грешка</summary>
      Error,
      /// <summary>наименование на размер</summary>
      NameSize
    }
    /// <summary>връща размера на катета лежащ срещу ъгъла</summary>
    /// <date>2019-04-08</date>
    /// <param name="angle">стойност на ъгъла</param>
    /// <param name="side">размер на хипотенузата</param>
    /// <returns>размер на катат</returns>
    public static double SinFn(double angle, double side) {
      return Math.Sin(angle) * side;
    } // SinFn

    /// <summary>връща размера на прилежащия катет на ъгъла</summary>
    /// <date>2019-04-08</date>
    /// <param name="angle">стойност на ъгъла</param>
    /// <param name="side">размер на хипотенузата</param>
    /// <returns>размер на катет</returns>
    public static double CosFn(double angle, double side) {
      return Math.Cos(angle) * side;
    } // CosFn

    /// <summary>връща информация за контрола в swPMPage като стринг</summary>
    /// <date>2019-04-08</date>
    /// <param name="control">контрола в swPMPage</param>
    /// <param name="element">елемент от информацията за контролата</param>
    /// <returns></returns>
    public static string GetControlsStringValue(Controls_e control, ControlElement_e element, Patterns_e pattern) {
      string info = default(string);
      string strPattern = default(string);
      switch (pattern) {
        case Patterns_e.Plate:
          strPattern = " на планката";
          break;
        case Patterns_e.Axis:
          strPattern = " на оста";
          break;
        case Patterns_e.Tube:
          strPattern = " на тръбата";
          break;
        case Patterns_e.Mantel:
          strPattern = " на мантела";
          break;
        case Patterns_e.Cover:
          strPattern = " на капака";
          break;
        case Patterns_e.Bottom:
          strPattern = " на дъното";
          break;
        case Patterns_e.Hole:
          strPattern = " на отвора";
          break;
        case Patterns_e.OutHole:
          strPattern = " на избушения отвор навън";
          break;
        case Patterns_e.InHole:
          strPattern = " на избушения отвор навътре";
          break;
      }
      switch (control) {
        case Controls_e.Material:
          switch (element) {
            case ControlElement_e.Caption:
              return "Материал";
            case ControlElement_e.Tooltip:
              return "изберете материала" + strPattern;
          }
          break;
        case Controls_e.Standard:
          switch (element) {
            case ControlElement_e.Caption:
              return "Стандарт";
            case ControlElement_e.Tooltip:
              return "изберете стандарта" + strPattern;
          }
          break;
        case Controls_e.Quality:
          switch (element) {
            case ControlElement_e.Caption:
              return "Качество";
            case ControlElement_e.Tooltip:
              return "изберете качеството" + strPattern;
          }
          break;
        case Controls_e.Thickness:
          switch (element) {
            case ControlElement_e.Caption:
              return "Дебелина, S в mm";
            case ControlElement_e.Tooltip:
              return "въведете дебелината" + strPattern;
            case ControlElement_e.Error:
              return "Не сте въвели дебелината" + strPattern;
            case ControlElement_e.NameSize:
              return "Thickness";
          }
          break;
        case Controls_e.Width:
          switch (element) {
            case ControlElement_e.Caption:
              return "Ширина, A в mm";
            case ControlElement_e.Tooltip:
              return "въведете ширината" + strPattern;
            case ControlElement_e.Error:
              return "Не сте въвели ширинта" + strPattern;
            case ControlElement_e.NameSize:
              return "Width";
          }
          break;
        case Controls_e.Length:
          switch (element) {
            case ControlElement_e.Caption:
              return "Дължина, L в mm";
            case ControlElement_e.Tooltip:
              return "въведете дължината" + strPattern;
            case ControlElement_e.Error:
              return "Не сте въвели дължината" + strPattern;
            case ControlElement_e.NameSize:
              return "Length";
          }
          break;
        case Controls_e.Height:
          switch (element) {
            case ControlElement_e.Caption:
              return "Височина, H в mm";
            case ControlElement_e.Tooltip:
              return "въведете височината" + strPattern;
            case ControlElement_e.Error:
              return "Не сте въвели височината" + strPattern;
            case ControlElement_e.NameSize:
              return "Height";
          }
          break;
        case Controls_e.Diameter:
          switch (element) {
            case ControlElement_e.Caption:
              return "Диаметър, ф в mm";
            case ControlElement_e.Tooltip:
              return "въведете диаметъра" + strPattern;
            case ControlElement_e.Error:
              return "Не сте въвели диаметъра" + strPattern;
            case ControlElement_e.NameSize:
              return "Diameter";
          }
          break;
        case Controls_e.WeldingSeam:
          switch (element) {
            case ControlElement_e.Caption:
              return "Ъгъл на заваръчния шев, в °";
            case ControlElement_e.Tooltip:
              return "въведете ъгъла на заваръчния шев" + strPattern;
            case ControlElement_e.Error:
              return "Не сте въвели ъгъла на заваръчния шев" + strPattern;
            case ControlElement_e.NameSize:
              return "WeldingSeam";
          }
          break;
        case Controls_e.AngularOffset:
          switch (element) {
            case ControlElement_e.Caption:
              return "Ъглово отмстване, в °";
            case ControlElement_e.Tooltip:
              return "въведете ъгловото отместване на отвора" + strPattern;
            case ControlElement_e.Error:
              return "Не сте въвели ъгловото отместване на отвора" + strPattern;
            case ControlElement_e.NameSize:
              return "AngularOoffset";
          }
          break;
        case Controls_e.BendRadius:
          switch (element) {
            case ControlElement_e.Caption:
              return "Радиус на бертване, в mm";
            case ControlElement_e.Tooltip:
              return "въведете радиуса на бертване" + strPattern;
            case ControlElement_e.Error:
              return "Не сте въвели радиуса на бертване" + strPattern;
            case ControlElement_e.NameSize:
              return "BendRadius";
          }
          break;
        case Controls_e.Distance:
          switch (element) {
            case ControlElement_e.Caption:
              return "Разстояние до центъра, в mm";
            case ControlElement_e.Tooltip:
              return "въведете разстоянието до центъра" + strPattern;
            case ControlElement_e.Error:
              return "Не сте въвели разстоянието до центъра" + strPattern;
            case ControlElement_e.NameSize:
              return "Distance";
          }
          break;
        case Controls_e.AngleInclination:
          switch (element) {
            case ControlElement_e.Caption:
              return "Ъгъл на наклона, в °";
            case ControlElement_e.Tooltip:
              return "въведете ъгъла на наклона" + strPattern;
            case ControlElement_e.Error:
              return "Не сте въвели ъгла на наклона" + strPattern;
            case ControlElement_e.NameSize:
              return "AngleIinclination";
          }
          break;
        case Controls_e.Certificate:
          switch (element) {
            case ControlElement_e.Caption:
              return "Сертификат";
            case ControlElement_e.Tooltip:
              return "изберете сертификата";
          }
          break;
        case Controls_e.Test:
          switch (element) {
            case ControlElement_e.Caption:
              return "Тест";
            case ControlElement_e.Tooltip:
              return "изберете теста";
          }
          break;
        case Controls_e.NoDrawing:
          switch (element) {
            case ControlElement_e.Caption:
              return "Без чертеж";
          }
          break;
        case Controls_e.Clockwise:
          switch (element) {
            case ControlElement_e.Caption:
              return "Посока на нарастване на ъгъла";
            case ControlElement_e.Caption_rb:
              return "по часовниковата стрелка";
          }
          break;
        case Controls_e.AntiClockwise:
          switch (element) {
            case ControlElement_e.Caption:
              return "Посока на нарастване на ъгъла";
            case ControlElement_e.Caption_rb:
              return "обратно на часовниковата стрелка";
          }
          break;
        case Controls_e.NortPosition:
          switch (element) {
            case ControlElement_e.Caption:
              return "Позиция на 0°";
            case ControlElement_e.Caption_rb:
              return "север";
          }
          break;
        case Controls_e.EastPosition:
          switch (element) {
            case ControlElement_e.Caption:
              return "Позиция на 0°";
            case ControlElement_e.Caption_rb:
              return "изток";
          }
          break;
        case Controls_e.SoutPosition:
          switch (element) {
            case ControlElement_e.Caption:
              return "Позиция на 0°";
            case ControlElement_e.Caption_rb:
              return "юг";
          }
          break;
        case Controls_e.WestPosition:
          switch (element) {
            case ControlElement_e.Caption:
              return "Позиция на 0°";
            case ControlElement_e.Caption_rb:
              return "запад";
          }
          break;
      }
      return info;
    } // GetControlsStringValue

    /// <summary>Изобразява съобщение за грешка</summary>
    /// <date>2019-004-08</date>
    /// <param name="control">тип на контролата за която се отнася грешката</param>
    /// <param name="pattern">тип на модела където е възникнала грешката</param>
    /// <param name="buttons">достъпни бутони в екра за съобщението</param>
    /// <returns></returns>
    public static System.Windows.Forms.DialogResult ShowError(Prime.Controls_e control, Prime.Patterns_e pattern, 
      System.Windows.Forms.MessageBoxButtons buttons = System.Windows.Forms.MessageBoxButtons.YesNo) {
      return System.Windows.Forms.MessageBox.Show(GetControlsStringValue(control, ControlElement_e.Error, pattern),
        "Внимание", buttons, System.Windows.Forms.MessageBoxIcon.Stop);
    } // ShowError

    //==========================================================================
    #region Before 2019-04-08

    /// <summary>Наименование на приложението</summary>
    public const string ADDIN_NAME = "SmartTools";
    /// <summary>
    /// Кратко описание на приложението, записано в регистрите
    /// </summary>
    public const string ADDIN_DESCRIPTION =
          "Add-in allowing rapid and easy development of parts and assemblies.";
    /// <summary>u
    /// Номер на групата съдържаща указателите към моделите на приложението
    /// </summary>
    public const int CMD_GROUP_ID = 5;
    /// <summary>
    /// Наименование на групата съдържаща указателите към моделите на приложението
    /// </summary>
    public const string CMD_GROUP_NAME = "SmartTools group";
    /// <summary>
    /// Наименование на раздела с указатели към моделите
    /// </summary>
    public const string TAB_NAME = "Bio Tools 1.0";
    /// <summary>
    /// Наименование на ключа в регистрите за достъп до папката съдържаща XML файлове
    /// </summary>
    public const string PATH_XML_RKEY = "Path XML";
    /// <summary>Папка съдържаща XML файлове на приложението</summary>
    public const string PATH_XML = Prime.TEST ? "D://Biomashin//SmartTools//XMLData//" : "C://Biomashin//BioTools//XMLData//";

    /// <summary>
    /// Форматиращ стринг за параметрите
    /// </summary>
    public const string FORMAT_PARAMETER = "{0}@{1}";
    // <summary>
    /// Съобщение при опит да се използва модел/инструмент върху документ,
    /// който не е създаден от приложението или не с избрания инструмент
    /// </summary>
    public const string ERR_ACTIVE_DOC_NOT_ADDIN =
      "Активният документ не е създаден със " + ADDIN_NAME +
      ".\nА може би се опитвате да редктирате документ с инструмент различен" +
      " от използвания при създаването му.\n\nДа се създаде ли нов?";
    /// <summary>
    /// Съобщение, че не е възможно създаване на PropertyManager Page
    /// </summary>
    public const string ERR_CREATE_PMPAGE =
      "Не е възможно да се създаде Property Page!" +
      "\nОбърнете се към администратора на системата";

    /// <summary>Указател към приложението</summary>
    public static SmartTools AddIn;

    /// <summary>
    /// Масив с допустимите типове работни документи за SmartTools
    /// </summary>
    public static int[] PattensTypes {
      get {
        return new int[] { (int)swDocumentTypes_e.swDocPART,
                           (int)swDocumentTypes_e.swDocASSEMBLY,
                           (int)swDocumentTypes_e.swDocDRAWING };
      }
    } // docTypes
    /// <summary>Папка съдържаща XML файлове на приложението</summary>
    public static string PathXML;

    /// <summary>
    /// един градус в радиани
    /// </summary>
    public const double ONE_DEGREE = Math.PI / 180;
    
    /// <summary>точност на числата с плаваща запетая</summary>
    public const double DECIMAL_PLACE = 0.00001;


    

    

    

    /// <summary>елементи на мадела</summary>
    public enum PatternElement_e {
      /// <summary>наименование на модела</summary>
      Name,
      /// <summary>
      /// информация за модела появяваща се в лентата на състояние в SW
      /// </summary>
      Hint
    }


    /// <summary>връща информация за модела като стринг</summary>
    /// <param name="pattern">модел</param>
    /// <param name="element">елемент от информацията за модела</param>
    /// <returns>текстова информация</returns>
    public static string GetPatternStringValue(Patterns_e pattern, PatternElement_e element) {
      string info = default(string);
      switch (pattern) {
        case Patterns_e.Plate:
          switch (element) {
            case PatternElement_e.Name:
              return "Планка";
            case PatternElement_e.Hint:
              return "Създава/Редактира планка";
          }
          break;
        case Patterns_e.Axis:
          switch (element) {
            case PatternElement_e.Name:
              return "Ос";
            case PatternElement_e.Hint:
              return "Създава/Редактира ос";
          }
          break;
        case Patterns_e.Tube:
          switch (element) {
            case PatternElement_e.Name:
              return "Тръба";
            case PatternElement_e.Hint:
              return "Създава/Редактира тръба";
          }
          break;
        case Patterns_e.Mantel:
          switch (element) {
            case PatternElement_e.Name:
              return "Мантел";
            case PatternElement_e.Hint:
              return "Създава/Редактира мантел";
          }
          break;
        case Patterns_e.Cover:
          switch (element) {
            case PatternElement_e.Name:
              return "Капак";
            case PatternElement_e.Hint:
              return "Създава/Редактира капак";
          }
          break;
        case Patterns_e.Bottom:
          switch (element) {
            case PatternElement_e.Name:
              return "Дъно";
            case PatternElement_e.Hint:
              return "Създава/Редактира дъно";
          }
          break;
        case Patterns_e.Hole:
          switch (element) {
            case PatternElement_e.Name:
              return "Отвор";
            case PatternElement_e.Hint:
              return "Създава/Редактира отвор";
          }
          break;
        case Patterns_e.OutHole:
          switch (element) {
            case PatternElement_e.Name:
              return "Избушен отвор навън";
            case PatternElement_e.Hint:
              return "Създава/Редактира избушен отвор навън";
          }
          break;
        case Patterns_e.InHole:
          switch (element) {
            case PatternElement_e.Name:
              return "Избушен отвор навътре";
            case PatternElement_e.Hint:
              return "Създава/Редактира избушен отвор навътре";
          }
          break;
        case Patterns_e.EditPattern:
          switch (element) {
            case PatternElement_e.Name:
              return "Редактира избран елемент";
            case PatternElement_e.Hint:
              return "Редактира избран елемент от дървото на построението";
          }
          break;
      }
      return info;
    } // GetInfoPattern

    

    

    /// <summary>
    /// Проверява дали <paramref name="val"/> е по-голяма от <paramref name="check"/>.
    /// </summary>
    /// <param name="val">проверяване стойност</param>
    /// <param name="check">контролна стойност</param>
    /// <param name="msg">съобщение при <paramref name="val"/> е по-малка от <paramref name="check"/></param>
    /// <returns>TRUE при <paramref name="val"/> е по-голямо от <paramref name="check"/></returns>
    public static bool CheckDimension(double val, double check, string msg) {
      if (val < check) {
        System.Windows.Forms.MessageBox.Show(msg, "Внимание",
          System.Windows.Forms.MessageBoxButtons.OK,
          System.Windows.Forms.MessageBoxIcon.Stop);
        return false;
      }
      return true;
    } // CheckDimension

    /// <summary>Преобразува ъглови градуси в радиани</summary>
    /// <param name="degrees">стойност на ъгловия градус</param>
    /// <returns>стойност на радианите</returns>
    public static double InRadians(double degrees) {
      return degrees * Prime.ONE_DEGREE;
    } // InRadians

    /// <summary>Преобразува радиани в ъглови градуси</summary>
    /// <param name="radians">стойнонст на радианите</param>
    /// <returns>стойност на ъгловите градуси</returns>
    public static double InDegrees(double radians) {
      return radians / Prime.ONE_DEGREE;
    } // InDegrees

    

    /// <summary>връща форматирано наименование на елемент</summary>
    /// <param name="arg0">водещ аргумент</param>
    /// <param name="arg1">основен аргумент</param>
    /// <returns><paramref name="arg0"/>@<paramref name="arg1"/></returns>
    public static string Name(string arg0, string arg1) {
      return arg0 + "@" + arg1;
    } // Name
    #endregion Before 2019-04-08
  } // Prime

}
