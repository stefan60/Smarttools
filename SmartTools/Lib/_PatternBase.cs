using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SmartTools.Lib {

  [ComVisible(true)]
  public abstract class _PatternBase: IPropertyManagerPage2Handler9 {

    #region Mandatory Properties
    /// <summary>Тип на модела</summary>
    public abstract Prime.Patterns_e ModelType {
      get;
    }
    /// <summary>Тип на документа</summary>
    public abstract swDocumentTypes_e DocType {
      get;
    }
    /// <summary>Наименование на модела</summary>
    public abstract string Name {
      get;
    }
    /// <summary>
    /// Информация за инструмента, появяваща се в лентата на състояние в SW
    /// </summary>
    public abstract string Hint {
      get;
    }
    /// <summary>
    /// Информация за инструмента, появяваща се, когато показалеца на 
    /// мишкта е върху инструмента
    /// </summary>
    public abstract string ToolTip {
      get;
    }
    /// <summary> Документ използван от модела</summary>
    public IModelDoc2 swModel {
      get;
      protected set;
    }
    public Lib.Properties swProperties {
      get;
      protected set;
    }
    /// <summary>Първо изграждане на модела.</summary>
    protected bool FirstRebuild = true;
    #endregion Mandatory Properties

    #region PropertyManager Page Controls
    /// <summary>
    /// Изглед с облика на SW PropertyManager Page за въвеждане и 
    /// редактиране на параметри на моделите използвани от add-In.
    /// </summary>
    public IPropertyManagerPage2 swPMPage {
      get;
      protected set;
    }
    /// <summary>
    /// Списък с групите използвани от модела в swPMPage
    /// </summary>
    public Dictionary<Prime.Groups_e, IPropertyManagerPageGroup> pmpGroups {
      get;
      protected set;
    }
    /// <summary>Списък с контролите на Labels в swPMPage</summary>
    public Dictionary<int, IPropertyManagerPageLabel> pmpLabels {
      get;
      protected set;
    }
    /// <summary>Списък с контролите на RadioButtons в swPMPage</summary>
    public Dictionary<Prime.Controls_e, IPropertyManagerPageOption> pmpRadioButtons {
      get;
      protected set;
    }
    /// <summary>Списък с контроли на Combobox в swPMPage</summary>
    public Dictionary<Prime.Controls_e, IPropertyManagerPageCombobox> pmpCombobox {
      get;
      protected set;
    }
    /// <summary>Списък с контроли на Numberbox в swPMPage</summary>
    public Dictionary<Prime.Controls_e, IPropertyManagerPageNumberbox> pmpNumberbox {
      get;
      protected set;
    }
    /// <summary>Списък с контроли на Checkbox в swPMPage</summary>
    public Dictionary<Prime.Controls_e, IPropertyManagerPageCheckbox> pmpCheckbox {
      get;
      protected set;
    }
    /// <summary>уникални номера/брояч на етикетите към контролите</summary>
    protected int idLabel = (int)Prime.Controls_e.idLabel;
    /// <summary>
    /// връща/записва номер на избран елемент от контрола Material
    /// </summary>
    public virtual short e_Material {
      get {
        return this.pmpCombobox[Prime.Controls_e.Material].CurrentSelection;
      }
      protected set {
        this.pmpCombobox[Prime.Controls_e.Material].CurrentSelection = value;
      }
    }
    /// <summary>
    /// връща съдържанието на избран елемент от контрола Material
    /// </summary>
    public virtual string e_MaterialItem {
      get {
        return this.pmpCombobox[Prime.Controls_e.Material].ItemText[this.e_Material];
      }
    }
    /// <summary>
    /// връща/записва номер на избран елемент от контрола Certificate
    /// </summary>
    public virtual short e_Certificate {
      get {
        return this.pmpCombobox[Prime.Controls_e.Certificate].CurrentSelection;
      }
      protected set {
        this.pmpCombobox[Prime.Controls_e.Certificate].CurrentSelection = value;
      }
    }
    /// <summary>
    /// връща съдържанието на избран елемент от контрола Certificate
    /// </summary>
    public virtual string e_CertificateItem {
      get {
        return this.pmpCombobox[Prime.Controls_e.Certificate].ItemText[this.e_Certificate];
      }
    }
    /// <summary>
    /// връща/записва номер на избран елемент от контрола Test
    /// </summary>
    public virtual short e_Test {
      get {
        return this.pmpCombobox[Prime.Controls_e.Test].CurrentSelection;
      }
      protected set {
        this.pmpCombobox[Prime.Controls_e.Test].CurrentSelection = value;
      }
    }
    /// <summary>връща съдържанието на избран елемент от контрола Test</summary>
    public virtual string e_TestItem {
      get {
        return this.pmpCombobox[Prime.Controls_e.Test].ItemText[this.e_Test];
      }
    }
    /// <summary>
    /// връща/записва номер на избран елемент от контрола Standard
    /// </summary>
    public virtual short e_Standatd {
      get {
        return this.pmpCombobox[Prime.Controls_e.Standard].CurrentSelection;
      }
      protected set {
        this.pmpCombobox[Prime.Controls_e.Standard].CurrentSelection = value;
      }
    }
    /// <summary>връща съдържанието на избран елемент от контрола Standard</summary>
    public virtual string e_StandardItem {
      get {
        return this.pmpCombobox[Prime.Controls_e.Standard].ItemText[this.e_Standatd];
      }
    }
    /// <summary>
    /// връща/записва номер на избран елемент от контрола Quality
    /// </summary>
    public virtual short e_Quality {
      get {
        return this.pmpCombobox[Prime.Controls_e.Quality].CurrentSelection;
      }
      protected set {
        this.pmpCombobox[Prime.Controls_e.Quality].CurrentSelection = value;
      }
    }
    /// <summary>връща съдържанието на избран елемент от контрола Quality</summary>
    public virtual string e_QualityItem {
      get {
        return this.pmpCombobox[Prime.Controls_e.Quality].ItemText[this.e_Quality];
      }
    }
    /// <summary>връща/записва стойност на контрола NoDrawing</summary>
    public virtual bool e_NoDrawing {
      get {
        return this.pmpCheckbox[Prime.Controls_e.NoDrawing].Checked;
      }
      protected set {
        this.pmpCheckbox[Prime.Controls_e.NoDrawing].Checked = value;
      }
    }
    /// <summary>връща/записва стойност на контрола Length</summary>
    public virtual double e_Length {
      get {
        return this.pmpNumberbox[Prime.Controls_e.Length].Value;
      }
      protected set {
        this.pmpNumberbox[Prime.Controls_e.Length].Value = value;
      }
    }
    /// <summary>връща/записва стойност на контрола Height</summary>
    public virtual double e_Height {
      get {
        return this.pmpNumberbox[Prime.Controls_e.Height].Value;
      }
      protected set {
        this.pmpNumberbox[Prime.Controls_e.Height].Value = value;
      }
    }
    /// <summary>връща/записва стойност на контрола Width</summary>
    public virtual double e_Width {
      get {
        return this.pmpNumberbox[Prime.Controls_e.Width].Value;
      }
      protected set {
        this.pmpNumberbox[Prime.Controls_e.Width].Value = value;
      }
    }
    /// <summary>връща/записва стойност на контрола Thickness</summary>
    public virtual double e_Thickness {
      get {
        return this.pmpNumberbox[Prime.Controls_e.Thickness].Value;
      }
      protected set {
        this.pmpNumberbox[Prime.Controls_e.Thickness].Value = value;
      }
    }
    /// <summary>връща/записва стойност на контрола Diameter</summary>
    public virtual double e_Diameter {
      get {
        return this.pmpNumberbox[Prime.Controls_e.Diameter].Value;
      }
      protected set {
        this.pmpNumberbox[Prime.Controls_e.Diameter].Value = value;
      }
    }
    /// <summary>връща/записва стойност на контрола WeldingSeam</summary>
    public virtual double e_WeldingSeam {
      get {
        return this.pmpNumberbox[Prime.Controls_e.WeldingSeam].Value;
      }
      protected set {
        this.pmpNumberbox[Prime.Controls_e.WeldingSeam].Value = value;
      }
    }
    /// <summary>връща/записва стойност на контрола Clockwise</summary>
    public virtual bool e_Clockwise {
      get {
        return this.pmpRadioButtons[Prime.Controls_e.Clockwise].Checked;
      }
      protected set {
        this.pmpRadioButtons[Prime.Controls_e.Clockwise].Checked = value;
      }
    }
    /// <summary>връща/записва стойност на контрола AntiClockwise</summary>
    public virtual bool e_AntiClockwise {
      get {
        return this.pmpRadioButtons[Prime.Controls_e.AntiClockwise].Checked;
      }
      protected set {
        this.pmpRadioButtons[Prime.Controls_e.AntiClockwise].Checked = value;
      }
    }
    /// <summary>връща/записва стойност на контрола NortPosition</summary>
    public virtual bool e_NortPosition {
      get {
        return this.pmpRadioButtons[Prime.Controls_e.NortPosition].Checked;
      }
      protected set {
        this.pmpRadioButtons[Prime.Controls_e.NortPosition].Checked = value;
      }
    }
    /// <summary>връща/записва стойност на контрола EastPosition</summary>
    public virtual bool e_EastPosition {
      get {
        return this.pmpRadioButtons[Prime.Controls_e.EastPosition].Checked;
      }
      protected set {
        this.pmpRadioButtons[Prime.Controls_e.EastPosition].Checked = value;
      }
    }
    /// <summary>връща/записва стойност на контрола SoutPositionh</summary>
    public virtual bool e_SoutPositionh {
      get {
        return this.pmpRadioButtons[Prime.Controls_e.SoutPosition].Checked;
      }
      protected set {
        this.pmpRadioButtons[Prime.Controls_e.SoutPosition].Checked = value;
      }
    }
    /// <summary>връща/записва стойност на контрола WestPosition</summary>
    public virtual bool e_WestPosition {
      get {
        return this.pmpRadioButtons[Prime.Controls_e.WestPosition].Checked;
      }
      protected set {
        this.pmpRadioButtons[Prime.Controls_e.WestPosition].Checked = value;
      }
    }
    #endregion PropertyManager Page Controls

    #region Abstract Methods
    /// <summary>Добавя контролите в swPMPage</summary>
    public abstract void AddControlsOnPMPage();
    /// <summary>Редактира съществуващия модел/инструмент</summary>
    public abstract void EditPattern();
    /// <summary>
    /// Записва в контролите на swPMPage стойности по подразбиране
    /// </summary>
    public abstract void SetDefaultValue();
    /// <summary>Създава 3D модела</summary>
    public abstract bool BuildModel();
    /// <summary>Установява стойностите на размерите от въведените</summary>
    public abstract void SetValueControls();
    /// <summary>Съхранява характеристиките на модела във File Properties</summary>
    public abstract void SaveFileProperties();
    /// <summary>Създава папка включваща построенията за модела</summary>
    public abstract void CreateFolder();
    #endregion Abstract Methods

    #region PatternBase Methods
    protected int pmpControlsID;
    /// <summary>Създава обект на модела</summary>
    public _PatternBase() {
      this.swPMPage = default(IPropertyManagerPage2);
      this.swProperties = default(Lib.Properties);
      this.pmpGroups = null;
      this.pmpLabels = null;
      this.pmpRadioButtons = null;
      this.pmpCombobox = null;
      this.pmpNumberbox = null;
      this.FirstRebuild = true;
      if (!this.CreatePMPage()) {
        System.Windows.Forms.MessageBox.Show(Prime.ERR_CREATE_PMPAGE,
          "Грешка",
          System.Windows.Forms.MessageBoxButtons.OK,
          System.Windows.Forms.MessageBoxIcon.Stop);
      }
    } // PatternBase
    /// <summary>подготвя за работа обекта с описанията на модела</summary>
    /// <param name="nameConfiguration">наименование на конфигурацията</param>
    protected void SetSWPropertyies(string nameConfiguration = Properties.CUSTOM_TAB) {
      if (this.swProperties == null) {
        this.swProperties = new Lib.Properties(this.swModel, nameConfiguration);
      } else {
        this.swProperties.ChangeConfiguration(this.swModel, nameConfiguration);
      }
    } // SetSWPropertyies
    /// <summary>
    /// Изпълнява модела. Извиква от PushButton() на SmartTools
    /// </summary>
    public virtual void Execute() {
      this.swModel = (IModelDoc2)Prime.AddIn.SwApp.ActiveDoc;
      if (this.IsEmptyModel()) {
        if (this.DocType == swDocumentTypes_e.swDocNONE) {
          System.Windows.Forms.MessageBox.Show("Избрания инструмент не е приложим при липса на основен детайл.",
            "Внимание", System.Windows.Forms.MessageBoxButtons.OK,
            System.Windows.Forms.MessageBoxIcon.Information);
          return;
        }
        this.NewPattern();
      } else {  
        this.SetSWPropertyies();
        // проверява да ли документа е създаден с инструмента на SmasrTools
        if (this.swProperties.Creator().Equals(Prime.ADDIN_NAME) &&
            this.swProperties.Pattern().Equals(this.ModelType.ToString())) {
          // редактира модела, абстрактен метод
          this.EditPattern();
        } else {
          System.Windows.Forms.DialogResult dialogResult =
            System.Windows.Forms.MessageBox.Show(Prime.ERR_ACTIVE_DOC_NOT_ADDIN,
              "Внимание", System.Windows.Forms.MessageBoxButtons.YesNo,
              System.Windows.Forms.MessageBoxIcon.Question);
          if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            this.NewPattern(false);
        }
      }
      this.swPMPage.Show2(0);
    } // Execute
    /// <summary>Създава нов модел/инструмент</summary>
    /// <param name="ThisGraphicArea">
    /// Указва дали да се използва текущия документ или
    /// да се съдаде нов документ
    /// </param>
    public virtual void NewPattern(bool ThisGraphicArea = true) {
      if (!ThisGraphicArea) {
        this.swModel = (IModelDoc2)Prime.AddIn.SwApp.NewDocument(this.PatternDefName(),
          (int)swDwgPaperSizes_e.swDwgPaperA4size, 0, 0);
      } else {
        if (this.swModel.GetType() != (int)this.DocType) {
          // празната графична област не съответства на типа на модела
          // създава нова графична област
          this.swModel = (IModelDoc2)Prime.AddIn.SwApp.NewDocument(this.PatternDefName(),
            (int)swDwgPaperSizes_e.swDwgPaperA4size, 0, 0);
        }
      }
      this.SetDefaultValue(); // абстрактен метод
      this.FirstRebuild = true;
    } // NewPattern
    /// <summary>
    /// Изгражда/Възстановява модела. Извиква се от OnClose() или OnPreview().
    /// </summary>
    public virtual void Rebuild() {
      if (this.FirstRebuild) {
        // изгражда модела за първи път
        if (!this.BuildModel())
          // не са въведени всички параметри на модела
          return;
        this.FirstRebuild = false;
      } else {
        // установява стойностите на параметрите от PropertyManager Page
        this.SetValueControls(); // абстактен метод
      }
      // съхранява стойностите на параметрите в FileProperty
      this.SaveFileProperties(); // абстактен метод
      this.swModel.Extension.EditRebuildAll();
      this.swModel.ViewZoomtofit2();
    } // Rebuild
    #endregion PatternBase Methods

    #region swPMPage Controls
    /// <summary>Създава swPMPage</summary>
    /// <returns>TRUE създадена swPMPage, FALSE възникнал проблем</returns>
    public virtual bool CreatePMPage() {
      int error = -1;
      int options = (int)(swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
                          swPropertyManagerPageOptions_e.swPropertyManagerOptions_PreviewButton |
                          swPropertyManagerPageOptions_e.swPropertyManagerOptions_CanEscapeCancel |
                          swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton |
                          swPropertyManagerPageOptions_e.swPropertyManagerOptions_LockedPage);
      this.swPMPage = (IPropertyManagerPage2)Prime.AddIn.SwApp.ICreatePropertyManagerPage(this.ToolTip,
          options, this, ref error);
      if (error == 0) {
        this.pmpGroups = new Dictionary<Prime.Groups_e, IPropertyManagerPageGroup>();
        // създава контролите в swPMPage, абстрактен метод
        this.AddControlsOnPMPage();
        return true;
      }
      return false;
    } // CreatePMPage
    /// <summary>Връща контролата като уникален номер</summary>
    /// <param name="ctrl">наименование на контролата</param>
    /// <returns></returns>
    public virtual int IDControls(Prime.Controls_e ctrl) {
      return (int)ctrl;
    } // IDControls
    public virtual short GetIndexCombobox(string itemText, IPropertyManagerPageCombobox combobox) {
      short index = -1;
      bool flag = false;
      string item = null;
      while (!flag) {
        index++;
        item = combobox.ItemText[index];
        if (item.Length == 0 || item == null)
          break;
        flag = item.ToLower().Equals(itemText.ToLower());
      }
      if (!flag)
        index = -1;
      return index;
    } // GetIndexCombobox
    /// <summary>Създава група за контроли в swPMPage, ако не съществува</summary>
    /// <param name="group">код на групата</param>
    public virtual void AddGroup(Prime.Groups_e group) {
      if (!this.pmpGroups.ContainsKey(group)) {
        string caption = "";
        switch (group) {
          case Prime.Groups_e.Characteristics:
            caption = "Характеристики";
            break;
          case Prime.Groups_e.Dimension:
            caption = "Габаритни размери";
            break;
          case Prime.Groups_e.Complement:
            caption = "Допълнителна информация";
            break;
          case Prime.Groups_e.CoordinateSystem:
            caption = "Координатна система";
            break;
          default:
            caption = "No names";
            break;
        }
        this.pmpGroups.Add(group, 
          this.swPMPage.IAddGroupBox((int)group,
               caption, (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                        (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible));
      }
    } // AddGroup
    /// <summary>Връща исканите опции за елемент от PM page</summary>
    /// <param name="enabled">разрешава работа с елемента</param>
    /// <param name="visible">показва елемента</param>
    /// <param name="smallGap">по-малко отсстояние от предходния елемент</param>
    /// <returns>комбинация от избраните флагове</returns>
    public virtual int Options(
      bool enabled = true, bool visible = true, bool smallGap = false) {
      int options = 0;
      if (enabled)
        options |= (int)swAddControlOptions_e.swControlOptions_Enabled;
      if (visible)
        options |= (int)swAddControlOptions_e.swControlOptions_Visible;
      if (smallGap)
        options |= (int)swAddControlOptions_e.swControlOptions_SmallGapAbove;
      return options;
    } // Options
    /// <summary>Създава контрола от тип ЕТИКЕТ</summary>
    /// <param name="group">група в която се създава етикета</param>
    /// <param name="id">уникален номер на етикета</param>
    /// <param name="caption">наименование на етикета</param>
    /// <returns>обект от тип IPropertyManagerPageLabel</returns>
    public virtual IPropertyManagerPageLabel AddLabel(Prime.Groups_e group, ref int id, string caption) {
      this.AddGroup(group);
      if (this.pmpLabels == null)
        this.pmpLabels = new Dictionary<int, IPropertyManagerPageLabel>();
      if (!this.pmpLabels.ContainsKey(id)) {
        this.pmpLabels.Add(id, (IPropertyManagerPageLabel)this.pmpGroups[group].AddControl2(id,
          (short)swPropertyManagerPageControlType_e.swControlType_Label, caption,
          (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent, this.Options(), ""));
        id++;
      }
      return this.pmpLabels[id - 1];
    } // AddLabel
    /// <summary>Създава контрола от тип radioButtons</summary>
    /// <param name="group">група в която се създава радио бутона</param>
    /// <param name="id">уникален номер на радио бутона</param>
    /// <param name="caption">наименование на радио бутона</param>
    /// <returns>обект от тип IPropertyManagerPageOption</returns>
    public virtual IPropertyManagerPageOption AddRadioButton(Prime.Groups_e group, Prime.Controls_e id, 
      string caption, bool check, bool firstInGroup, ref int idLabel, string lblCaption = null) {
      this.AddGroup(group);
      if (this.pmpRadioButtons == null)
        this.pmpRadioButtons = new Dictionary<Prime.Controls_e, IPropertyManagerPageOption>();
      if (!this.pmpRadioButtons.ContainsKey(id)) {
        if (lblCaption != null)
          this.AddLabel(group, ref idLabel, lblCaption);
        this.pmpRadioButtons.Add(id, (IPropertyManagerPageOption)this.pmpGroups[group].AddControl2(this.IDControls(id),
          (short)swPropertyManagerPageControlType_e.swControlType_Option, caption,
          (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge,
          this.Options(), ""));
        this.pmpRadioButtons[id].Checked = check;
        if (firstInGroup)
          this.pmpRadioButtons[id].Style = (int)swPropMgrPageOptionStyle_e.swPropMgrPageOptionStyle_FirstInGroup;
      }
      return this.pmpRadioButtons[id];
    } // AddRadioButton
    /// <summary>Създава контрола от тип Combobox</summary>
    /// <param name="group">група в която се създава Combobox</param>
    /// <param name="id">уникален номер на Combobox</param>
    /// <param name="caption">наименование на Combobox</param>
    /// <param name="tip">подсказващата информация Combobox</param>
    /// <param name="items">масив с елементите в Combobox</param>
    /// <param name="idLabel">уникален номер на етикета към контролата</param>
    /// <param name="lblCaption">надпис на етикета</param>
    /// <param name="selection">индекс на избран елемент</param>
    /// <param name="height">размер на областа в която се изобразяват елементите</param>
    /// <returns>обект от тип IPropertyManagerPageCombobox</returns>
    public virtual IPropertyManagerPageCombobox AddCombobox(Prime.Groups_e group, Prime.Controls_e id, 
      string caption, string tip, string[] items, ref int idLabel, string lblCaption, 
      short selection = 0, short height = 50) {
      this.AddGroup(group);
      if (this.pmpCombobox == null)
        this.pmpCombobox = new Dictionary<Prime.Controls_e, IPropertyManagerPageCombobox>();
      if (!this.pmpCombobox.ContainsKey(id)) {
        this.AddLabel(group, ref idLabel, lblCaption);
        this.pmpCombobox.Add(id, (IPropertyManagerPageCombobox)this.pmpGroups[group].AddControl2(this.IDControls(id),
          (short)swPropertyManagerPageControlType_e.swControlType_Combobox, caption,
          (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge,
          this.Options(), tip));
        this.pmpCombobox[id].Height = height;
        this.pmpCombobox[id].AddItems(items);
        this.pmpCombobox[id].CurrentSelection = selection;
      }
      return this.pmpCombobox[id];
    } // AddCombobox
    /// <summary>Създава контрола от тип Numberbox и етикет към нея</summary>
    /// <param name="group">група в която се създава Numberbox</param>
    /// <param name="id">уникален номер на контролата</param>
    /// <param name="caption">наименование на контролата</param>
    /// <param name="tip">подсказваща информация за контролата</param>
    /// <param name="idLabel">уникален номер на етикета към контролата</param>
    /// <param name="lblCaption">надпис на етикета</param>
    /// <param name="val">първоначална стойност</param>
    /// <param name="min">минимално допустима стойност</param>
    /// <param name="max">максимално допустима стойност</param>
    /// <param name="unit">вид на мерната единица</param>
    /// <returns>указател към контролата от тип IPropertyManagerPageNumberbox</returns>
    public virtual IPropertyManagerPageNumberbox AddNumberbox(Prime.Groups_e group, Prime.Controls_e id, 
      string caption, string tip, ref int idLabel, string lblCaption, 
      double val, double min = 0, double max = 3.0, 
      swNumberboxUnitType_e unit = swNumberboxUnitType_e.swNumberBox_Length) {
      this.AddGroup(group);
      if (this.pmpNumberbox == null)
        this.pmpNumberbox = new Dictionary<Prime.Controls_e, IPropertyManagerPageNumberbox>();
      if (!this.pmpNumberbox.ContainsKey(id)) {
        this.AddLabel(group, ref idLabel, lblCaption);
        this.pmpNumberbox.Add(id, (IPropertyManagerPageNumberbox)this.pmpGroups[group].AddControl2(this.IDControls(id),
          (short)swPropertyManagerPageControlType_e.swControlType_Numberbox, caption,
          (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge,
          this.Options(), tip));
        if (max > 0) {
          this.pmpNumberbox[id].SetRange2((int)unit, min, max, true, 0.005, 0.005, 0.002);
        } else {
          this.pmpNumberbox[id].SetRange2((int)unit, 0, 5.0, true, 0.005, 0.005, 0.002);
        }
        this.pmpNumberbox[id].Value = val;
      }
      return this.pmpNumberbox[id];
    } // AddNumberbox
    /// <summary>Създава контрола от тип Checkbox</summary>
    /// <param name="group">група в която се създава Checkbox</param>
    /// <param name="id">уникален номер на контролата</param>
    /// <param name="caption">надпис на контролата</param>
    /// <param name="check">флаг за маркиране на контролата</param>
    /// <returns>обект от тип IPropertyManagerPageCheckbox</returns>
    public virtual IPropertyManagerPageCheckbox AddCheckbox(Prime.Groups_e group, Prime.Controls_e id,
      string caption, bool check = true) {
      this.AddGroup(group);
      if (this.pmpCheckbox == null)
        this.pmpCheckbox = new Dictionary<Prime.Controls_e, IPropertyManagerPageCheckbox>();
      if (!this.pmpCheckbox.ContainsKey(id)) {
        this.pmpCheckbox.Add(id, (IPropertyManagerPageCheckbox)this.pmpGroups[group].AddControl2(this.IDControls(id),
          (short)swPropertyManagerPageControlType_e.swControlType_Checkbox, caption,
          (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent,
          this.Options(), ""));
        this.pmpCheckbox[id].Checked = check;
      }
      return this.pmpCheckbox[id];
    } // AddCheckbox
    #endregion swPMPage Controls

    #region UI Feartures
    /// <summary>
    /// Връща форматиран стринг за параметър използван във фючерите
    /// </summary>
    /// <param name="arg0">аргумент 1</param>
    /// <param name="arg2">аргумент 2</param>
    /// <returns>arg0@arg1</returns>
    public virtual string FormatParameter(string arg0, string arg2) {
      return arg0 + "@" + arg2;
    } // FormatParameter
    /// <summary>
    /// <summary>
    /// <summary>
    /// Проверява празен ли е документа в средата на SW.
    /// Последен fiature в дървото трябва да е Origin
    /// </summary>
    /// <returns>TRUE празен документ</returns>
    public virtual bool IsEmptyModel() {
      IFeature feature = (IFeature)this.swModel.FirstFeature();
      bool flag = false;
      while (feature != null) {
        flag = false;
        if (feature.Name.ToLower().Equals("origin"))
          flag = true;
        feature = (IFeature)feature.GetNextFeature();
      }
      return flag;
    } // IsEmptyModel
    /// <summary> Връща наименованието на новата графична област за модела</summary>
    /// <returns>Наименованието на графичната област</returns>
    public virtual string PatternDefName() {
      int userPreference;
      string name = "SmartTools pattern";
      switch (this.DocType) {
        case swDocumentTypes_e.swDocPART:
          userPreference = (int)swUserPreferenceStringValue_e.swDefaultTemplatePart;
          break;
        case swDocumentTypes_e.swDocDRAWING:
          userPreference = (int)swUserPreferenceStringValue_e.swDefaultTemplateDrawing;
          break;
        case swDocumentTypes_e.swDocASSEMBLY:
          userPreference = (int)swUserPreferenceStringValue_e.swDefaultTemplateAssembly;
          break;
        default:
          userPreference = -1;
          break;
      }
      if (userPreference > 0)
        name = Prime.AddIn.SwApp.GetUserPreferenceStringValue(userPreference);
      return name;
    } // PatternDefName
    /// <summary>
    /// Преименува последно създаден фичер, името на който започва с oldName и
    /// задължително завършва с цифри
    /// </summary>
    /// <param name="oldName">началото на фичера за преименоване</param>
    /// <param name="newName">ново име на фичера</param>
    /// <returns>TRUE при открит и преимениван фичер, FALSE не е открит фичер</returns>
    public virtual bool RenameFeature(string oldName, string newName) {
      IFeature feature = (IFeature)this.swModel.FirstFeature();
      IFeature f1 = null;
      string name = null;
      string tmp = null;
      int idx = -1;
      int max = -1;
      int number = -1;
      bool flag = false;
      bool found = false;
      while (feature != null) {
        name = feature.Name.ToLower();
        idx = name.IndexOf(oldName.ToLower());
        if (idx == 0) {
          found = true;
          tmp = name.Substring(oldName.Length);
          flag = true;
          try {
            number = Int32.Parse(tmp);
          } catch (FormatException) {
            flag = false;
          } catch (OverflowException) {
            flag = false;
          }
          if (flag && number > max) {
            f1 = feature;
            max = number;
          }
        }
        feature = (IFeature)feature.GetNextFeature();
      }
      if (f1 != null) {
        f1.Name = newName;
        return true;
      }
      return found;
    } // RenameFeature
    #endregion UI Feartures

    #region swPMPage Controls Old
    /// <summary>Подтиска въвеждането на стойност на размер от екрана</summary>
    public virtual void SuppressDimInput() {
      Prime.AddIn.SwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swInputDimValOnCreate, false);
    } // SuppressDimInput
    #endregion swPMPage Controls Old

    #region UI 3D Modeling
    /// <summary>Създава размер за обект или между два обекта</summary>
    /// <param name="nameObject1">наименование на основен обект</param>
    /// <param name="typeObject1">тип на основен обект</param>
    /// <param name="nameDim">наименование на размера</param>
    /// <param name="x">X координата на надписа</param>
    /// <param name="y">Y координата на надписа</param>
    /// <param name="z">Z координата на надписа</param>
    /// <param name="valDim">стойност на размера</param>
    /// <param name="nameObject2">наименование на втория обект</param>
    /// <param name="typeObject2">тип на втория обект</param>
    /// <param name="readOnly">размера не се променя</param>
    public virtual void AddDimension(string nameObject1, string typeObject1,
      string nameDim, double x, double y, double z, double valDim = 0,
      string nameObject2 = null, string typeObject2 =null, bool readOnly = true) {
      this.SuppressDimInput();
      this.swModel.ClearSelection2(true);
      bool res = this.swModel.Extension.SelectByID2(nameObject1, typeObject1, 
          0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
      if (nameObject2 != null)
        res = this.swModel.Extension.SelectByID2(nameObject2, typeObject2,
          0, 0, 0, true, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
      IDisplayDimension dim = (IDisplayDimension)this.swModel.AddDimension2(x, y, z);
      dim.GetDimension2(0).Name = nameDim;
      dim.GetDimension2(0).ReadOnly = readOnly;
      if (valDim > 0) {
        dim.GetDimension2(0).SetValue3(valDim * 1000,
         (int)swSetValueInConfiguration_e.swSetValue_InAllConfigurations, null);
      }
    } // AddDimension
    public virtual void SetValueDimension(string nameDimension, double value,
      string nameConfig = null, swSetValueInConfiguration_e 
         config = swSetValueInConfiguration_e.swSetValue_InAllConfigurations ) {
      IDimension dimension = this.swModel.IParameter(nameDimension);
      int res = -99;
      if (value > 0)
        res = dimension.SetSystemValue3(value, (int)config, nameConfig);
        //res = dimension.SetValue3(value, (int)config, nameConfig);
    } // SetValueDimension
    /// <summary>Избира равнина</summary>
    /// <param name="plane">наименование на равнината</param>
    public virtual void SelectPlane(string plane) {
      this.swModel.ClearSelection2(true);
      this.swModel.Extension.SelectByID2(plane, "PLANE", 0, 0, 0, true, 0, null, 0);
      this.swModel.SketchManager.InsertSketch(true);
    } // SelectPlane
    /// <summary>
    /// Създава скица на правоъгълник с център центъра на координатната система
    /// </summary>
    /// <param name="width">ширина</param>
    /// <param name="length">дължина</param>
    /// <param name="nameDim1">наиемнование на дължината</param>
    /// <param name="nameDim2">наименование на ширината</param>
    /// <param name="sketch">ново наименование на скицата</param>
    public virtual void CreateRectangle(double length, double width,
                          string nameDim1, string nameDim2, string nameSketch) {
      this.swModel.SketchManager.CreateCenterRectangle(0, 0, 0, length / 2, 
                                                                  width / 2, 0);
      this.AddDimension("Line1", "SKETCHSEGMENT", nameDim1, 0, 0, 
                                                           (width / 2) + 0.002);
      this.AddDimension("Line2", "SKETCHSEGMENT", nameDim2, 
                                                    (length / 2) + 0.002, 0, 0);
      this.swModel.ClearSelection2(true);
      if (nameSketch != null)
        this.RenameFeature("Sketch", nameSketch);
      this.swModel.SketchManager.InsertSketch(true);
    } // CreateRectangle
    /// <summary>
    /// Създава скица на окръжност по център и диаметър
    /// </summary>
    /// <param name="x">X-координата на центъра</param>
    /// <param name="y">Y-координата на центъра</param>
    /// <param name="z">Z-координата на центъра</param>
    /// <param name="diameter">радиус</param>
    /// <param name="nameDiameter">наименование на диаметъра</param>
    /// <param name="nameSketch">наименование на скицата</param>
    public virtual void CreateCircle(double x, double y, double z,
                        double diameter, string nameDiameter, string nameSketch) {
      this.swModel.SketchManager.CreateCircleByRadius(0, 0, 0, diameter / 2);
      this.AddDimension("Arc1", "SKETCHSEGMENT", nameDiameter, diameter, 0, 0);
      if (nameSketch != null)
        this.RenameFeature("Sketch", nameSketch);
      this.swModel.SketchManager.InsertSketch(true);
    } // CreateCircle
    /// <summary>Създава 3D модела от скица</summary>
    /// <param name="sketch">наименование на скица</param>
    /// <param name="depth">височина на избутване скицата, 3D модел</param>
    /// <param name="nameExtrude">ново наименование на feature</param>
    /// <param name="nameDepth">наименование на размера на височината</param>
    public virtual void Extrude(string sketch, double depth, string nameExtrude,
      string nameDepth, double thin = -1, string nameThickness = null, int thinDir = 1) {
      this.swModel.ClearSelection2(true);
      this.swModel.Extension.SelectByID2(sketch, "SKETCH", 0, 0, 0, false, 0, 
                             null, (int)swSelectOption_e.swSelectOptionDefault);
      if (thin < 0) {
        this.swModel.FeatureManager.FeatureExtrusion3(true, false, false,
          (int)swEndConditions_e.swEndCondBlind, 0, depth, 0, false, false, false,
          false, 0, 0, false, false, false, false, false, false, true,
          (int)swStartConditions_e.swStartSketchPlane, 0, false);
        this.RenameFeature("Boss-Extrude", nameExtrude);
      } else {
        this.swModel.FeatureManager.FeatureExtrusionThin2(true, false, false,
          (int)swEndConditions_e.swEndCondBlind, 0, depth, 0, 
          false, false, false, false, 0, 0, false, false, false, false, true,
          thin, 0, 0, thinDir, 0, false, 0, true, true,
          (int)swStartConditions_e.swStartSketchPlane, 0, false);
        this.RenameFeature("Extrude-Thin", nameExtrude);
        IDimension d2 = this.swModel.IParameter("D5@" + nameExtrude);
        d2.Name = nameThickness;
        d2.ReadOnly = true;
      }
      IDimension d1 = this.swModel.IParameter("D1@" + nameExtrude);
      d1.Name = nameDepth;
      d1.ReadOnly = true;
    } // Extrude
    #endregion UI 3D Modeling

    #region PropertyManager Page Handler
    public void AfterActivation() {
      //throw new NotImplementedException();
    }

    public void OnClose(Int32 Reason) {
      if (Reason == (int)swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay) {
        this.Rebuild();
        this.CreateFolder();
        this.SetSWPropertyies(Properties.CONFIGURATION_SPECIFIC);
        bool res = this.swModel.SetTitle2(this.swProperties.DrawingNumber());
        ((IPartDoc)this.swModel).SetMaterialPropertyName2("default",
          "BM Custom Materials", this.e_MaterialItem);
      }
    }

    public void AfterClose() {
    }

    public Boolean OnHelp() {
      //throw new NotImplementedException();
      return true;
    }

    public Boolean OnPreviousPage() {
      //throw new NotImplementedException();
      return true;
    }

    public Boolean OnNextPage() {
      //throw new NotImplementedException();
      return true;
    }

    public Boolean OnPreview() {
      this.Rebuild();
      return true;
    }

    public void OnWhatsNew() {
      //throw new NotImplementedException();
    }

    public void OnUndo() {
      //throw new NotImplementedException();
    }

    public void OnRedo() {
      //throw new NotImplementedException();
    }

    public Boolean OnTabClicked(Int32 Id) {
      //throw new NotImplementedException();
      return true;
    }

    public void OnGroupExpand(Int32 Id, Boolean Expanded) {
      //throw new NotImplementedException();
    }

    public void OnGroupCheck(Int32 Id, Boolean Checked) {
      //throw new NotImplementedException();
    }

    public void OnCheckboxCheck(Int32 Id, Boolean Checked) {
      //throw new NotImplementedException();
    }

    public void OnOptionCheck(Int32 Id) {
      //throw new NotImplementedException();
    }

    public void OnButtonPress(Int32 Id) {
      //throw new NotImplementedException();
    }

    public void OnTextboxChanged(Int32 Id, String Text) {
      //throw new NotImplementedException();
    }

    public void OnNumberboxChanged(Int32 Id, Double Value) {
      //throw new NotImplementedException();
    }

    public void OnComboboxEditChanged(Int32 Id, String Text) {
      //throw new NotImplementedException();
    }

    public virtual void OnComboboxSelectionChanged(Int32 Id, Int32 Item) {
    } // OnComboboxSelectionChanged

    public void OnListboxSelectionChanged(Int32 Id, Int32 Item) {
      //throw new NotImplementedException();
    }

    public void OnSelectionboxFocusChanged(Int32 Id) {
      //throw new NotImplementedException();
    }

    public void OnSelectionboxListChanged(Int32 Id, Int32 Count) {
      //throw new NotImplementedException();
    }

    public void OnSelectionboxCalloutCreated(Int32 Id) {
      //throw new NotImplementedException();
    }

    public void OnSelectionboxCalloutDestroyed(Int32 Id) {
      //throw new NotImplementedException();
    }

    public Boolean OnSubmitSelection(Int32 Id, Object Selection, Int32 SelType, ref String ItemText) {
      //throw new NotImplementedException();
      return true;
    }

    public Int32 OnActiveXControlCreated(Int32 Id, Boolean Status) {
      //throw new NotImplementedException();
      return Id;
    }

    public void OnSliderPositionChanged(Int32 Id, Double Value) {
      //throw new NotImplementedException();
    }

    public void OnSliderTrackingCompleted(Int32 Id, Double Value) {
      //throw new NotImplementedException();
    }

    public Boolean OnKeystroke(Int32 Wparam, Int32 Message, Int32 Lparam, Int32 Id) {
      //throw new NotImplementedException();
      return true;
    }

    public void OnPopupMenuItem(Int32 Id) {
     //throw new NotImplementedException();
    }

    public void OnPopupMenuItemUpdate(Int32 Id, ref Int32 retval) {
      //throw new NotImplementedException();
    }

    public void OnGainedFocus(Int32 Id) {
      //throw new NotImplementedException();
    }

    public void OnLostFocus(Int32 Id) {
      //throw new NotImplementedException();
    }

    public Int32 OnWindowFromHandleControlCreated(Int32 Id, Boolean Status) {
      //throw new NotImplementedException();
      return Id;
    }

    public void OnListboxRMBUp(Int32 Id, Int32 PosX, Int32 PosY) {
      //throw new NotImplementedException();
    }

    public void OnNumberBoxTrackingCompleted(Int32 Id, Double Value) {
      //throw new NotImplementedException();
    }
    #endregion PropertyManager Page Handler

  } // PatternBase

}
