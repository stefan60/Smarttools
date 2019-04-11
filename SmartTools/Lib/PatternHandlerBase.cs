using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SmartTools.Lib {
  [ComVisible(true)]
  public abstract class PatternHandlerBase: IPropertyManagerPage2Handler9 {

    /// <summary>тип на документ от допустмите в средата на SW</summary>
    public abstract swDocumentTypes_e swDocumentType {
      get;
    }
    /// <summary>тип на модела определен за приложението SmartTools</summary>
    public abstract Prime.Patterns_e modelType {
      get;
    }
    /// <summary>
    /// изглед с облика на SW PropertyManager Page за въвеждане и 
    /// редактиране на параметри на моделите използвани от add-In.
    /// </summary>
    public virtual PropertyManagerPage2 swPMPage {
      get;
      protected set;
    }
    /// <summary>обект за достъп до документа използван от модела</summary>
    public virtual ModelDoc2 swDocument {
      get;
      protected set;
    }
    /// <summary>обект за достъп функциите на модела</summary>
    public virtual ModelDocExtension swExtension {
      get { return this.swDocument.Extension; }
    }
    /// <summary>обект за достъп до функциите за работа със скиците</summary>
    public virtual SketchManager swSketchManager {
      get { return this.swDocument.SketchManager; }
    }
    /// <summary>обект за достъп да функциите за работа с фичърите</summary>
    public virtual FeatureManager swFeatureManager {
      get { return this.swDocument.FeatureManager;  }
    }
    /// <summary>обект за достъп до избраните елемент</summary>
    public virtual SelectionMgr swSelectionMgr {
      get { return this.swDocument.SelectionManager; }
    }
    /// <summary>обект за достъп до описанията на модела</summary>
    public virtual Properties fileProperties {
      get;
      protected set;
    }
    /// <summary>Първоначално изграждане на модела</summary>
    public virtual bool initialCreation {
      get;
      protected set;
    }
    /// <summary>флаг за създаден документ</summary>
    /// <date>2019-04-10</date>
    public virtual bool notCreatedDoc {
      get;
      protected set;
    }

    /// <summary>
    /// Подготвя основните променливи за използване
    /// </summary>
    public PatternHandlerBase() {
      this.swDocument = default(ModelDoc2);
      this.swPMPage = default(PropertyManagerPage2);
      this.fileProperties = default(Lib.Properties);
      this.initialCreation = true;
      this.notCreatedDoc = true;
    } // PatternHandlerBase

    /// <summary>Изпълнява модела.</summary>
    /// <param name="mode">начин за изпълнение на обекта, True създава нов обект, False редактира</param>
    public virtual void Execute(bool mode = true) {
      this.swDocument = Prime.AddIn.SwApp.IActiveDoc2;
      this.initialCreation = true;
      this.swDocument = Prime.AddIn.SwApp.IActiveDoc2;
      if (this.IsEmptyDoc()) {
        // текущия документ е празен
        if (this.swDocumentType == swDocumentTypes_e.swDocNONE) {
          System.Windows.Forms.MessageBox.Show("Избраният инструмент работи върху съществуващ детайл!\n" +
            "Изберете детайл и използвайте инструмента.",
            "Внимание", System.Windows.Forms.MessageBoxButtons.OK, 
            System.Windows.Forms.MessageBoxIcon.Warning);
          return;
        }
        this.NewDocumentSW();
      } else {
        this.FilePropetyTab();
        if (this.swDocumentType != swDocumentTypes_e.swDocNONE) {
          // не е инструмент за отвори
          if (this.fileProperties.Creator().Equals(Prime.ADDIN_NAME) &&
              this.fileProperties.Pattern().Equals(this.modelType.ToString())) {
            // абстрактен метод
            this.EditDocumentSW(); 
          } else {
            System.Windows.Forms.DialogResult res =
              System.Windows.Forms.MessageBox.Show(Prime.ERR_ACTIVE_DOC_NOT_ADDIN, "Внимание",
              System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            if (res == System.Windows.Forms.DialogResult.Yes)
              this.NewDocumentSW();
          }
        } else {
          // инструмент за отвори, абстрактен метод
          if (!this.Relevance()) {
            System.Windows.Forms.MessageBox.Show("Избрания инструмент не е приложим към средата!", "Внимание", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
            return;
          }
          //Feature ft = (Feature)this.swSelectionMgr.GetSelectedObject6(1, -1);
          //if (ft == null) {
            // не е избран елемент за редактиране
            this.initialCreation = true;
            // абстрактен метод
            this.GetCoordinateSystem();
          /*
          } else {
            this.initialCreation = false;
            this.EditDocumentSW();
          }
          */
        }
      }
      this.swPMPage.Show2(0);
    } // Execute

    /// <summary>Изгражда/Възстановява модела</summary>
    public virtual void Rebuild() {
      if (this.initialCreation) {
        // абстрактен метод създаващ обекта
        if (this.Creating())
          this.initialCreation = false;
      } else {
        // абстрактен метод установяващ новите стоиности на размерните елементи на обекта
        this.SetControls();
      }
      // абстрактен метод съхраняващ характеристиките на обекта във File Properties
      this.SetFileProperties();   
      this.swDocument.Extension.EditRebuildAll();
      this.swDocument.ViewZoomtofit2();
    } // Rebuild

    /// <summary>Избира раздел с описанията на модела</summary>
    /// <param name="name">наименование на раздела</param>
    public virtual void FilePropetyTab(string name = Lib.Properties.CUSTOM_TAB) {
      if (this.fileProperties == default(Properties)) {
        this.fileProperties = new Lib.Properties(this.swDocument, name);
      } else {
        this.fileProperties.ChangeConfiguration(this.swDocument, name);
      }
    } // FilePropetyTab

    /// <summary>Създава нова графична област</summary>
    /// <param name="thisGraphicArea">флаг за нова графична област или да се 
    /// използва съществуващата</param>
    public virtual void NewDocumentSW(bool thisGraphicArea = true) {
      if (thisGraphicArea) {
        this.FilePropetyTab();
        string model = this.fileProperties.Pattern();
        if (model == null)
          model = this.modelType.ToString();
        if ((((IModelDoc2)Prime.AddIn.SwApp.ActiveDoc).GetType() == (int)this.swDocumentType) && model.Equals(this.modelType.ToString())) {
          this.swDocument = (ModelDoc2)Prime.AddIn.SwApp.ActiveDoc;
        } else { 
          // текущата графична област не съответства на типа на модела.
          // създавам нова графична област
          this.swDocument = Prime.AddIn.SwApp.NewDocument(this.DeafultName(), (int)swDwgPaperSizes_e.swDwgPaperA4size, 0, 0);
        }
      }
      this.initialCreation = true;
      this.EditDocumentSW(true);
    } // NewDocumentSW

    /// <summary>формира името на графичната област по подразбиране</summary>
    /// <returns>наименование на графична област</returns>
    protected virtual string DeafultName() {
      string defName = default(string);
      switch (this.swDocumentType) {
        case swDocumentTypes_e.swDocASSEMBLY:
          defName = Prime.AddIn.SwApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplateAssembly);
          break;
        case swDocumentTypes_e.swDocDRAWING:
          defName = Prime.AddIn.SwApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplateDrawing);
          break;
        default:
          defName = Prime.AddIn.SwApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
          break;
      }
      return defName;
    } // DeafultName

    /// <summary>проверява активния документ дали е свододен</summary>
    /// <returns>TRUE свободен активен докумен, FALSE не е свободен</returns>
    public virtual bool IsEmptyDoc() {
      // имена на последните фичъри в дървото 
      string[] names = { "Origin", "Mates", "Sheet1" };
      bool empty = false;
      foreach (string feature in names) {
        empty = empty | this.IsLastFeature(feature);
      }
      return empty;
    } // IsEmptyDoc

    #region Abstract Methods
    public abstract bool IsLastFeature(string name); 
    /// <summary>редактира активен документ</summary>
    public abstract void EditDocumentSW(bool defValues = false);
    /// <summary>създава 3D модела</summary>
    /// <returns>TRUE при нормално изграждане, FALSE при проблем</returns>
    public abstract bool Creating();
    public virtual bool CreateDocSW() { return true; }
    public virtual void ChangesDimensions() { }
    /// <summary>
    /// установява стойностите на размерите на модела от въведените
    /// в съответните контроли на swPMPage
    /// </summary>
    public abstract void SetControls();
    /// <summary>
    /// Съхранява характеристиките на модела в описанието му.
    /// </summary>
    public abstract void SetFileProperties();
    /// <summary>Създава папка с построенията на модела</summary>
    public abstract void CreateFolder();
    /// <summary>връща наименованието на използвния материал в модела</summary>
    /// <returns>наименование на материал</returns>
    public abstract string MaterialName();
    /// <summary>обработва въвеждането на Сертификат или Тест</summary>
    /// <param name="control">уникален номер на контролата</param>
    /// <param name="selection">избран елемент в контролата</param>
    public abstract void ChangeCertificateOrTest(int control, int selection);
    /// <summary>
    /// обработва въвеждането на Стандарт или диаметър
    /// </summary>
    /// <param name="control">уникален номер на контролата</param>
    /// <param name="selection">избран елемент в контролата</param>
    public abstract void ChangeStandardOrDiameter(int control, int selection);
    /// <summary>
    /// проверява приложимостта на модела към съществуващата среда на SW
    /// </summary>
    /// <returns>TRUE модела е приложим, FALSE не може да се приложи</returns>
    public abstract bool Relevance();
    /// <summary>
    /// поготвя за визуализация информацията за координатната система
    /// </summary>
    public abstract void GetCoordinateSystem();
    /// <summary>
    /// активира се при избор на радио бутон
    /// </summary>
    public abstract void RadioButtonChek();
    /// <summary>
    /// активира се при промяна на диаметъра и/или дебелината на стената
    /// </summary>
    public abstract void DiameterOrThicknessChange();
    #endregion Abstract Methods

    #region PropertyManager Page Handler
    public void AfterActivation() {
      throw new NotImplementedException();
    }

    /// <summary>изпълнява се при затваряне на  swPMPage</summary>
    /// <param name="Reason">избрам бутон при затваряне на swPMPage</param>
    public void OnClose(Int32 Reason) {
      if ((int)swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay == Reason) {
        this.Rebuild();
        // абстрактен метод
        this.CreateFolder();
        if (this.swDocumentType != swDocumentTypes_e.swDocNONE) {
          this.FilePropetyTab(Properties.CONFIGURATION_SPECIFIC);
          this.swDocument.SetTitle2(this.fileProperties.DrawingNumber());
          ((IPartDoc)this.swDocument).SetMaterialPropertyName2(Properties.CONFIGURATION_SPECIFIC,
            "BM Custom Materials", this.MaterialName());
        }
      }
    }

    public void AfterClose() {
      throw new NotImplementedException();
    }

    public Boolean OnHelp() {
      throw new NotImplementedException();
    }

    public Boolean OnPreviousPage() {
      throw new NotImplementedException();
    }

    public Boolean OnNextPage() {
      throw new NotImplementedException();
    }

    /// <summary>Предварителен преглед на модела</summary>
    /// <returns>TRUE при нормално изпълнение на пгегледа</returns>
    public Boolean OnPreview() {
      this.Rebuild();
      return true;
    } // OnPreview

    public void OnWhatsNew() {
      throw new NotImplementedException();
    }

    public void OnUndo() {
      throw new NotImplementedException();
    }

    public void OnRedo() {
      throw new NotImplementedException();
    }

    public Boolean OnTabClicked(Int32 Id) {
      throw new NotImplementedException();
    }

    public void OnGroupExpand(Int32 Id, Boolean Expanded) {
      throw new NotImplementedException();
    }

    public void OnGroupCheck(Int32 Id, Boolean Checked) {
      throw new NotImplementedException();
    }

    public void OnCheckboxCheck(Int32 Id, Boolean Checked) {
      throw new NotImplementedException();
    }

    public void OnOptionCheck(Int32 Id) {
      this.RadioButtonChek();
    }

    public void OnButtonPress(Int32 Id) {
      throw new NotImplementedException();
    }

    public void OnTextboxChanged(Int32 Id, String Text) {
      throw new NotImplementedException();
    }

    public void OnNumberboxChanged(Int32 Id, Double Value) {
      this.DiameterOrThicknessChange();
    }

    public void OnComboboxEditChanged(Int32 Id, String Text) {
      throw new NotImplementedException();
    }

    public void OnComboboxSelectionChanged(Int32 Id, Int32 Item) {
      if ((Id == (int)Prime.Controls_e.Certificate) ||
          (Id == (int)Prime.Controls_e.Test)) {
        // абстрактен метод
        this.ChangeCertificateOrTest(Id, Item);
      }
      if ((Id == (int)Prime.Controls_e.Standard) ||
          (Id == (int)Prime.Controls_e.Diameter))
        // абстрактен метод
        this.ChangeStandardOrDiameter(Id, Item);
    }

    public void OnListboxSelectionChanged(Int32 Id, Int32 Item) {
      throw new NotImplementedException();
    }

    public void OnSelectionboxFocusChanged(Int32 Id) {
      throw new NotImplementedException();
    }

    public void OnSelectionboxListChanged(Int32 Id, Int32 Count) {
      throw new NotImplementedException();
    }

    public void OnSelectionboxCalloutCreated(Int32 Id) {
      throw new NotImplementedException();
    }

    public void OnSelectionboxCalloutDestroyed(Int32 Id) {
      throw new NotImplementedException();
    }

    public Boolean OnSubmitSelection(Int32 Id, Object Selection, Int32 SelType, ref String ItemText) {
      throw new NotImplementedException();
    }

    public Int32 OnActiveXControlCreated(Int32 Id, Boolean Status) {
      throw new NotImplementedException();
    }

    public void OnSliderPositionChanged(Int32 Id, Double Value) {
      throw new NotImplementedException();
    }

    public void OnSliderTrackingCompleted(Int32 Id, Double Value) {
      throw new NotImplementedException();
    }

    public Boolean OnKeystroke(Int32 Wparam, Int32 Message, Int32 Lparam, Int32 Id) {
      throw new NotImplementedException();
    }

    public void OnPopupMenuItem(Int32 Id) {
      throw new NotImplementedException();
    }

    public void OnPopupMenuItemUpdate(Int32 Id, ref Int32 retval) {
      throw new NotImplementedException();
    }

    public void OnGainedFocus(Int32 Id) {
      throw new NotImplementedException();
    }

    public void OnLostFocus(Int32 Id) {
      throw new NotImplementedException();
    }

    public Int32 OnWindowFromHandleControlCreated(Int32 Id, Boolean Status) {
      throw new NotImplementedException();
    }

    public void OnListboxRMBUp(Int32 Id, Int32 PosX, Int32 PosY) {
      throw new NotImplementedException();
    }

    public void OnNumberBoxTrackingCompleted(Int32 Id, Double Value) {
      throw new NotImplementedException();
    }
    #endregion PropertyManager Page Handler

  } // PatternHandlerBase

}
