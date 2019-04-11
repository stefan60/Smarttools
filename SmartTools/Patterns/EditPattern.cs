using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

using SolidWorks.Interop.swconst;
using SolidWorks.Interop.sldworks;

namespace SmartTools.Patterns {
  /// <summary>Редактира модели на приложението</summary>
  [ComVisible(true)]
  public class EditPattern : Lib.PatternBase {

    /// <summary>Изпълнява модела.</summary>
    /// <date>2019-04-10</date>
    public override void Execute(bool mode = true) {
      System.Windows.Forms.MessageBox.Show("В процес на разработка...");
      /*
      if (this.swDocumentType == swDocumentTypes_e.swDocNONE) {
        // приложимост на инструмента
        if (this.Relevance()) {
          // изпълнение на инструмента
        } else {
          // съобщение за не приложимост
          return;
        }
      } else {
        if (!this.IsEmptyDoc())
          // създава нова графична област
          this.swDocument = Prime.AddIn.SwApp.NewDocument(this.DeafultName(),
                             (int)swDwgPaperSizes_e.swDwgPaperA4size, 0, 0);
      }
      this.swPMPage.Show2(0);
      */
    } // Execute

    #region Global Properties
    /// <summary>тип на модела определен от SW</summary>
    /// <date>2019-04-11</date>
    public override swDocumentTypes_e swDocumentType {
      get { return swDocumentTypes_e.swDocNONE; }
    }
    /// <summary>тип на модела определен от приложението</summary>
    /// <date>2019-04-11</date>
    public override Prime.Patterns_e modelType {
      get { return Prime.Patterns_e.EditPattern; }
    }
    #endregion Global Properties

    /// <summary>проверка за приложимост на инструмента</summary>
    /// <returns>True инструментът се изпълнява, False не се изпълнява</returns>
    public override Boolean Relevance() {
      Feature feature = (Feature)this.swSelectionMgr.GetSelectedObject6(1, -1);
      if (feature == null) {
        System.Windows.Forms.MessageBox.Show("Няма избран елемент от дървото на построението!", "Внимание", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
        return false;
      }
      int pattern;
      while (feature != null) {
        if ((pattern = this.SearchFeature(feature)) == -2)
          return false;
        if (pattern == -1) {
          Feature subFeature = feature.IGetFirstSubFeature();
          while (subFeature != null) {
            pattern = this.SearchFeature(subFeature);
            if (pattern >= 0)
              break;
            subFeature = subFeature.IGetNextFeature();
          }
        }
        if (pattern >= 0) {
          break;
        }
        feature = feature.IGetNextFeature();
      }
      if (feature == null)
        return false;
      System.Windows.Forms.MessageBox.Show(feature.Name);
      Prime.AddIn.Patterns.ElementAt(4).Execute(false);
      return true;
    } // Relevance

    /// <summary>проверява дали еленета подлежи на редакция</summary>
    /// <param name="feature">обект към елемент</param>
    /// <returns>индекс към обектите за работа с моделите, -1 не е открит модел</returns>
    public virtual int SearchFeature(Feature feature) {
      string patternName = "";
      if (feature.Name.IndexOf("__EndTag__") > 0)
        // не е открит елемент пододящ за редактиран
        return -2;
      for (int pattern = 0; pattern < Prime.AddIn.Patterns.Count; pattern++) {
        patternName = Prime.AddIn.Patterns.ElementAt(pattern).modelType.ToString().ToLower() + "sketch";
        if (feature.Name.ToLower().IndexOf(patternName) == 0)
          return pattern;
      }
      return -1;
    } // SearchFeature

    public override void AddControlsOnPMPage() {
      //Prime.AddIn.Patterns.ElementAt(0).AddControlsOnPMPage();
      //throw new NotImplementedException();
    }

    public override void EditDocumentSW(Boolean defValues = false) {
      //throw new NotImplementedException();
    }

    public override Boolean Creating() {
      return true;
    }

    public override void SetControls() {
      //throw new NotImplementedException();
    }

    public override void SetFileProperties() {
      //throw new NotImplementedException();
    }

    public override void CreateFolder() {
      //throw new NotImplementedException();
    }

    public override String MaterialName() {
      //throw new NotImplementedException();
      return "";
    }
  } // EditPattern

}
