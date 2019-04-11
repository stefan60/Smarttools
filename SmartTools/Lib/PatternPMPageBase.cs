using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.sldworks;

namespace SmartTools.Lib {

   /// <summary>Създава PropertyManager Page по подобие на SW</summary>
  [ComVisible(true)]
  public abstract class PatternPMPageBase: PatternHandlerBase {

    #region Dictionary Controls
    /// <summary>библиотека с използваните групи с swPMPage</summary>
    /// <date>2019-04-08</date>
    public Dictionary<Prime.Groups_e, PropertyManagerPageGroup> pmpGroups {
      get;
      protected set;
    }
    /// <summary>библиотека с използваните етикети в swPMPage</summary>
    /// <date>2019-04-08</date>
    public Dictionary<int, PropertyManagerPageLabel> pmpLabel {
      get;
      protected set;
    }
    /// <summary>библиотека с използваните контроли от тип Radio Button</summary>
    /// <date>2019-04-08</date>
    public Dictionary<Prime.Controls_e, PropertyManagerPageOption> pmpRadioButton {
      get;
      protected set;
    }
    /// <summary>уникален номер на етикетите в swPMPage</summary>
    /// <date>2019-04-08</date>
    protected int idPMPLabel = (int)Prime.Controls_e.idLabel;
    /// <summary>бояч на отворите</summary>
    /// <date>2019-04-08</date>
    public int holeNumber { get; protected set; }
    #endregion Dictionary Controls

    #region Dimension Properties
    /// <summary>диаметър на обект</summary>
    /// <date>2019-04-08</date>
    public virtual double diameter {
      get { return this.dblValue(Prime.Controls_e.Diameter); }
    }
    /// <summary>дебелина на стената на обект</summary>
    /// <date>2019-04-08</date>
    public virtual double thickness {
      get { return this.dblValue(Prime.Controls_e.Thickness); }
    }
    /// <summary>радиус на бертване на дъното или капака</summary>
    /// <date>2019-04-08</date>
    public virtual double bendRadius {
      get { return this.dblValue(Prime.Controls_e.BendRadius); }
    }
    /// <summary>ъгъл на наклона на дъното или капака</summary>
    /// <date>2019-04-08</date>
    public virtual double angleInclination {
      get { return this.dblValue(Prime.Controls_e.AngleInclination); }
    }
    /// <summary>добакка към линеен размер</summary>
    /// <date>2019-04-08</date>
    public virtual double addOnSize { get { return 0.03; } }
    /// <summary>стойност на диаметъра на отвора при въха на конуса</summary>
    /// <date>2019-04-08</date>
    public virtual double diameterTopCone { get { return 0.022; } }
    /// <summary>размер на рамото на ъгъла за визуализация на заваръчния шев</summary>
    /// <date>2019-04-10</date>
    public double shoulderWeldingSeam {
      get { return this.diameter / 2 + this.thickness + this.addOnSize; }
    }
    /// <summary>отстояние от формообразуващата на модела до височината на избушен отвор навън</summary>
    /// <date>2019-04-11</date>
    public double dimExplodeExternal {
      get { return Prime.HEIHGT_EXPLODE_HOLE + this.thickness; }
    }
    #endregion Dimension Properties

    /// <summary>връща комбинация от исканите опции за контрола в swPMPage</summary>
    /// <date>2019-04-08</date>
    /// <param name="enabled">разрешава работа с елемента</param>
    /// <param name="visible">визуализира елемента</param>
    /// <param name="smallGap">по-малко отстояние от предходната контрола</param>
    /// <returns>комбинация от опциите</returns>
    public virtual int Options(bool enabled = true, bool visible = true, bool smallGap = false) {
      int options = 0;
      if (enabled)
        options |= (int)swAddControlOptions_e.swControlOptions_Enabled;
      if (visible)
        options |= (int)swAddControlOptions_e.swControlOptions_Visible;
      if (smallGap)
        options |= (int)swAddControlOptions_e.swControlOptions_SmallGapAbove;
      return options;
    } // Options

    /// <summary>добавя група в swPMPage, ако не съществува</summary>
    /// <date>2019-04-08</date>
    /// <param name="group">код на групата</param>
    protected virtual void AddGroup(Prime.Groups_e group) {
      if (this.pmpGroups == default(Dictionary<Prime.Groups_e, PropertyManagerPageGroup>))
        this.pmpGroups = new Dictionary<Prime.Groups_e, PropertyManagerPageGroup>();
      if (!this.pmpGroups.ContainsKey(group)) {
        string caption = default(string);
        switch (group) {
          case Prime.Groups_e.Characteristics:
            caption = "Характеристики";
            break;
          case Prime.Groups_e.Complement:
            caption = "Допълнителна информация";
            break;
          case Prime.Groups_e.CoordinateSystem:
            caption = "Координатна система";
            break;
          case Prime.Groups_e.Dimension:
            caption = "Габаритни размери";
            break;
        }
        this.pmpGroups.Add(group, this.swPMPage.IAddGroupBox((int)group, caption,
          (int)(swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded | swAddGroupBoxOptions_e.swGroupBoxOptions_Visible)));
      }
    } // AddGroup

    /// <summary>добавя етикет към библиотеката от етикети</summary>
    /// <date>2019-04-08</date>
    /// <param name="group">група в която се добавя</param>
    /// <param name="control">контрола кък която се свързва</param>
    protected virtual void AddLabel(Prime.Groups_e group, Prime.Controls_e control) {
      this.AddGroup(group);
      if (this.pmpLabel == default(Dictionary<int, PropertyManagerPageLabel>))
        this.pmpLabel = new Dictionary<int, PropertyManagerPageLabel>();
      if (!this.pmpLabel.ContainsKey(this.idPMPLabel)) {
        this.pmpLabel.Add(this.idPMPLabel, this.pmpGroups[group].AddControl2(this.idPMPLabel,
          (short)swPropertyManagerPageControlType_e.swControlType_Label,
          Prime.GetControlsStringValue(control, Prime.ControlElement_e.Caption, this.modelType),
          (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent,
          this.Options(), ""));
        this.idPMPLabel++;
      }
    } // AddLabel

    /// <summary>добавя радио бутон в библиотеката</summary>
    /// <date>2019-04-08</date>
    /// <param name="group">група в която се дибавя</param>
    /// <param name="control">уникален номер на радио бутона</param>
    /// <param name="check">флаг Избран/Не избран, True/False</param>
    /// <param name="firstInGroup">флаг за указване начало на група</param>
    /// <param name="isLabel">флаг указващ наличие на етикет </param>
    /// <param name="enable">флаг Разрешен/Забранен, True/False</param>
    protected virtual void AddRadioButton(Prime.Groups_e group, 
      Prime.Controls_e control, bool check = false, 
      bool firstInGroup = false, bool isLabel = false, bool enable = true) {
      this.AddGroup(group);
      if (this.pmpRadioButton == default(Dictionary<Prime.Controls_e, PropertyManagerPageOption>))
        this.pmpRadioButton = new Dictionary<Prime.Controls_e, PropertyManagerPageOption>();
      if (!this.pmpRadioButton.ContainsKey(control)) {
        if (isLabel)
          this.AddLabel(group, control);
        this.pmpRadioButton.Add(control, this.pmpGroups[group].AddControl2((int)control,
          (short)swPropertyManagerPageControlType_e.swControlType_Option,
          Prime.GetControlsStringValue(control, Prime.ControlElement_e.Caption_rb, this.modelType),
          (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge,
          this.Options(enable), ""));
        this.pmpRadioButton[control].Checked = check;
        if (firstInGroup)
          this.pmpRadioButton[control].Style = (int)swPropMgrPageOptionStyle_e.swPropMgrPageOptionStyle_FirstInGroup;
      }
    } // AddRadioButton

    public virtual void AddCoordinateSystem(Prime.Controls_e clockwise, Prime.Controls_e position0, bool enabled = true) {
      // контроли за избор на посоката на координатна система
      bool sel = clockwise == Prime.Controls_e.Clockwise;  
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem,
                      Prime.Controls_e.Clockwise, sel, true, true, enabled);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem,
               Prime.Controls_e.AntiClockwise, !sel, false, false, enabled);
      // контроли за избор на посицията на 0 градуса
      sel = (position0 == Prime.Controls_e.NortPosition);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem,
                   Prime.Controls_e.NortPosition, sel, true, true, enabled);
      sel = (position0 == Prime.Controls_e.EastPosition);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem,
                 Prime.Controls_e.EastPosition, sel, false, false, enabled);
      sel = (position0 == Prime.Controls_e.SoutPosition);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem,
                 Prime.Controls_e.SoutPosition, sel, false, false, enabled);
      sel = (position0 == Prime.Controls_e.WestPosition);
      this.AddRadioButton(Prime.Groups_e.CoordinateSystem,
                 Prime.Controls_e.WestPosition, sel, false, false, enabled);
    } // AddCoordinateSystem

    #region Controls Value
    /// <summary>връща въведена/избрана стойност от контрола в swPMPage</summary>
    /// <date>2019-04-08</date>
    /// <param name="control">тип на контролата</param>
    /// <returns>стойност на контролата</returns>
    public virtual object Value(Prime.Controls_e control) {
      if (this.pmpCombobox != default(Dictionary<Prime.Controls_e, PropertyManagerPageCombobox>))
        if (this.pmpCombobox.ContainsKey(control))
          return this.pmpCombobox[control].ItemText[this.Selection(control)];
      if (this.pmpNumberbox != default(Dictionary<Prime.Controls_e, PropertyManagerPageNumberbox>))
        if (this.pmpNumberbox.ContainsKey(control))
          return this.pmpNumberbox[control].Value;
      if (this.pmpCheckbox != default(Dictionary<Prime.Controls_e, PropertyManagerPageCheckbox>))
        if (this.pmpCheckbox.ContainsKey(control))
          return this.pmpCheckbox[control].Checked;
      if (this.pmpRadioButton != default(Dictionary<Prime.Controls_e, PropertyManagerPageOption>))
        if (this.pmpRadioButton.ContainsKey(control))
          return this.pmpRadioButton[control].Checked;
      return null;
    } // Value

    /// <summary>
    /// връща стойноста на контрола, като реално чиало. Ако контролата не 
    /// е от тип релано чиало се връща най-малкото реално чиало в C#
    /// </summary>
    /// <date>2019-04-08</date>
    /// <param name="control">тип на контролата</param>
    /// <returns>стойноста на контролата</returns>
    public virtual double dblValue(Prime.Controls_e control) {
      Type type = this.Value(control).GetType();
      if (type == typeof(double))
        return (double)this.Value(control);
      return Double.MinValue;
    } // dblValue
    #endregion Controls Value




    //==========================================================================
    #region before 2019-04-08
    // променливи за габаритните размери на модела
    #region Dimension Properties

    /// <summary>височина на обект</summary>
    public virtual double height {
      get { return this.dblValue(Prime.Controls_e.Height); }
    }
    /// <summary>отстояние на обект от центъра</summary>
    public virtual double distance {
      get { return this.dblValue(Prime.Controls_e.Distance); }
    }
    /// <summary>
    /// ъглово отместване по координатанат система на обект
    /// </summary>
    public virtual double angularOffset {
      get { return this.dblValue(Prime.Controls_e.AngularOffset); }
    }
    /// <summary>ъглово отместване на заваръчния шев</summary>
    public virtual double weldSeamAngle {
      get { return this.dblValue(Prime.Controls_e.WeldingSeam); }
    }
    
    
    /// <summary>
    /// текуща стойност на ъгловото отместване на елемет от модела
    /// </summary>
    public virtual double currentAngle { get; protected set; }
    /// <summary>стойност на ъгъла визуализиращ заваръчния щев</summary>
    public virtual double defAngleView { get { return Prime.TEST ? Prime.InRadians(0.3) : Prime.InRadians(0.1); } }
    
    
    /// <summary>точка за управление на ъгловото отместване на заваръчния шев</summary>
    public SketchPoint wsPoint { get; protected set; }
    /// <summary>точка за управление на ъгловото отместване на отвор</summary>
    public SketchPoint holePoint { get; protected set; }
    /// <summary>ъглополовяща на ъгъла за визуализация на заваръчния шев</summary>
    public SketchLine wsBisector { get; protected set; }
    /// <summary>окръжност за отвор</summary>
    public SketchArc holeArc { get; protected set; }
   
    #endregion Dimension Properties

    
    
    /// <summary>библиотека с използваните контроли от тип Combobox</summary>
    public Dictionary<Prime.Controls_e, PropertyManagerPageCombobox> pmpCombobox {
      get;
      protected set;
    }
    /// <summary>библиотека с използваните контроли от тип Numberbox</summary>
    public Dictionary<Prime.Controls_e, PropertyManagerPageNumberbox> pmpNumberbox {
      get;
      protected set;
    }
    /// <summary>библиотека с използваните контроли от тип Checkbox</summary>
    public Dictionary<Prime.Controls_e, PropertyManagerPageCheckbox> pmpCheckbox {
      get;
      protected set;
    }
    
    


    public PatternPMPageBase() : base() {
      this.pmpGroups = default(Dictionary<Prime.Groups_e, PropertyManagerPageGroup>);
      this.pmpLabel = default(Dictionary<int, PropertyManagerPageLabel>);
      this.pmpCombobox = default(Dictionary<Prime.Controls_e, PropertyManagerPageCombobox>);
      this.pmpNumberbox = default(Dictionary<Prime.Controls_e, PropertyManagerPageNumberbox>);
      this.pmpCheckbox = default(Dictionary<Prime.Controls_e, PropertyManagerPageCheckbox>);
      this.pmpRadioButton = default(Dictionary<Prime.Controls_e, PropertyManagerPageOption>);
      this.holeNumber = 0;
      int error = -1;
      int options = (int)(swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
        swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton |
        swPropertyManagerPageOptions_e.swPropertyManagerOptions_CanEscapeCancel |
        swPropertyManagerPageOptions_e.swPropertyManagerOptions_PreviewButton |
        swPropertyManagerPageOptions_e.swPropertyManagerOptions_LockedPage);
      this.swPMPage = Prime.AddIn.SwApp.ICreatePropertyManagerPage(Prime.GetPatternStringValue(this.modelType, Prime.PatternElement_e.Name), options, this, ref error);
      if (error == 0) {
        // абстрактен метод добавящ необходимите контроли в swPMPage за инструмента
        this.AddControlsOnPMPage();
      } else {
        System.Windows.Forms.MessageBox.Show(Prime.ERR_CREATE_PMPAGE, "Грешка",
          System.Windows.Forms.MessageBoxButtons.OK,
          System.Windows.Forms.MessageBoxIcon.Stop);
      }
    } // PatternCtrlPMPageBase

    #region Add Controls
    /// <summary>добавя контрола от тип Combobox</summary>
    /// <param name="group">група в която се добавя</param>
    /// <param name="control">уникален номер на контролата</param>
    /// <param name="items">масив с елементрите</param>
    /// <param name="height">размер на областта в която се жизуализират елементите</param>
    /// <param name="selection">индекс на избран елемент</param>
    protected virtual void AddCombobox(Prime.Groups_e group,
                                       Prime.Controls_e control,
                                       string[] items, short height = 50, 
                                       short selection = 0) {
      this.AddGroup(group);
      if (this.pmpCombobox == default(Dictionary<Prime.Controls_e, PropertyManagerPageCombobox>))
        this.pmpCombobox = new Dictionary<Prime.Controls_e, PropertyManagerPageCombobox>();
      if (!this.pmpCombobox.ContainsKey(control)) {
        this.AddLabel(group, control);
        this.pmpCombobox.Add(control, this.pmpGroups[group].AddControl2((int)control,
          (short)swPropertyManagerPageControlType_e.swControlType_Combobox,
          control.ToString(), 
          (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge,
          this.Options(), 
          Prime.GetControlsStringValue(control, Prime.ControlElement_e.Tooltip, this.modelType)));
        this.pmpCombobox[control].AddItems(items);
        this.pmpCombobox[control].Height = height;
        this.pmpCombobox[control].CurrentSelection = selection;
      }
    } // AddCombobox

    /// <summary>добавя контрола от тип Numberbox</summary>
    /// <param name="group">група в която се добавя</param>
    /// <param name="control">уникален номер на контролата</param>
    /// <param name="val">стойност по подразбиране</param>
    /// <param name="unit">мерна единица</param>
    /// <param name="min">минимална стойност</param>
    /// <param name="max">максимална стойност</param>
    protected virtual void AddNumberbox(Prime.Groups_e group, Prime.Controls_e control,
      double val, swNumberboxUnitType_e unit = swNumberboxUnitType_e.swNumberBox_Length,
      double min = 0, double max = 10) {
      this.AddGroup(group);
      if (this.pmpNumberbox == default(Dictionary<Prime.Controls_e, PropertyManagerPageNumberbox>))
        this.pmpNumberbox = new Dictionary<Prime.Controls_e, PropertyManagerPageNumberbox>();
      if (!this.pmpNumberbox.ContainsKey(control)) {
        this.AddLabel(group, control);
        this.pmpNumberbox.Add(control, this.pmpGroups[group].AddControl2((int)control,
          (short)swPropertyManagerPageControlType_e.swControlType_Numberbox,
          control.ToString(),
          (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge,
          this.Options(),
          Prime.GetControlsStringValue(control, Prime.ControlElement_e.Tooltip, this.modelType)));
        this.pmpNumberbox[control].SetRange2((int)unit, min, max, true, 0.005, 0.005, 0.005);
        this.pmpNumberbox[control].Value = val;
      }
    } // AddNumberbox

    /// <summary>
    /// добавя контрола от тип Checkbox
    /// </summary>
    /// <param name="group">група в която се добавя</param>
    /// <param name="control">уникален номер на контролата</param>
    /// <param name="check">флаг за маркирн/ не маркиран</param>
    protected virtual void AddCheckbox(Prime.Groups_e group, Prime.Controls_e control, bool check = true) {
      this.AddGroup(group);
      if (this.pmpCheckbox == default(Dictionary<Prime.Controls_e, PropertyManagerPageCheckbox>))
        this.pmpCheckbox = new Dictionary<Prime.Controls_e, PropertyManagerPageCheckbox>();
      if (!this.pmpCheckbox.ContainsKey(control)) {
        this.pmpCheckbox.Add(control, this.pmpGroups[group].AddControl2((int)control,
          (short)swPropertyManagerPageControlType_e.swControlType_Checkbox,
          Prime.GetControlsStringValue(control, Prime.ControlElement_e.Caption, this.modelType),
          (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent,
          this.Options(), ""));
        this.pmpCheckbox[control].Checked = check;
      }
    } // AddCheckbox

    
   
    

    

    
    #endregion Add Controls

   

    

    /// <summary>връща индекс на избран елемент в Combobox</summary>
    /// <param name="control">тип на контролата</param>
    /// <returns>индекс на избрания елемент</returns>
    public virtual short Selection(Prime.Controls_e control) {
      if (this.pmpCombobox != default(Dictionary<Prime.Controls_e, PropertyManagerPageCombobox>))
        if (this.pmpCombobox.ContainsKey(control))
          return this.pmpCombobox[control].CurrentSelection;
      return -1;
    } // Selection

    /// <summary>
    /// връща индекса на елемент от списък на контрола в swPMPage
    /// </summary>
    /// <param name="element">стойност на търсения елемент</param>
    /// <param name="control">контрола</param>
    /// <returns></returns>
    public virtual short ElementSelection(string element, Prime.Controls_e control) {
      bool found = false;
      short index = -1;
      if (this.pmpCombobox == default(Dictionary<Prime.Controls_e, PropertyManagerPageCombobox>))
        return index;
      while (!found) {
        index++;
        if (this.pmpCombobox[control].ItemText[index].Length <= 0)
          break;
        found = this.pmpCombobox[control].ItemText[index].Equals(element);
      }
      if (!found)
        index = -1;
      return index;
    } // ElementSelection

    /// <summary>
    /// установява индекс на избран елемент в Combobox
    /// </summary>
    /// <param name="control">тип на контролата</param>
    /// <param name="selection">индекс на избран елемент</param>
    public virtual void SetSelection(Prime.Controls_e control, short selection) {
      if (this.pmpCombobox != default(Dictionary<Prime.Controls_e, PropertyManagerPageCombobox>))
        if (this.pmpCombobox.ContainsKey(control))
          this.pmpCombobox[control].CurrentSelection = selection;
    } // SetSelection

    /// <summary>
    /// установява стойност на контрола от тип Numberbox
    /// </summary>
    /// <param name="control">тип на контролата</param>
    /// <param name="value">стойност</param>
    public virtual void SetValue(Prime.Controls_e control, double value) {
      if (this.pmpNumberbox != default(Dictionary<Prime.Controls_e, PropertyManagerPageNumberbox>))
        if (this.pmpNumberbox.ContainsKey(control))
          this.pmpNumberbox[control].Value = value;
    } // SetValue

    /// <summary>
    /// установява избран/не избран на контроли Checkbox/Radio button
    /// </summary>
    /// <param name="control">тип на контролата</param>
    /// <param name="check">избран/не избран</param>
    public virtual void SetChecked(Prime.Controls_e control, bool check) {
      if (this.pmpCheckbox != default(Dictionary<Prime.Controls_e, PropertyManagerPageCheckbox>))
        if (this.pmpCheckbox.ContainsKey(control))
          this.pmpCheckbox[control].Checked = check;
      if (this.pmpRadioButton != default(Dictionary<Prime.Controls_e, PropertyManagerPageOption>))
        if (this.pmpRadioButton.ContainsKey(control))
          this.pmpRadioButton[control].Checked = check;
    } // SetChecked

    /// <summary>
    /// управлява избора на Сертификат и Тест. Ако едната контрола е "-",
    /// задължително и втората трябва да бъде "-"
    /// </summary>
    /// <param name="control">тип на контролата</param>
    /// <param name="selection">индекс на избран елемент</param>
    public override void ChangeCertificateOrTest(Int32 control, Int32 selection) {
      if (control == (int)Prime.Controls_e.Certificate)
        if (((string)this.Value(Prime.Controls_e.Certificate)).Equals("-"))
          this.SetSelection(Prime.Controls_e.Test, (short)selection);
      if (control == (int)Prime.Controls_e.Test)
        if (((string)this.Value(Prime.Controls_e.Test)).Equals("-"))
          this.SetSelection(Prime.Controls_e.Certificate, (short)selection);
    } // ChangeCertificateOrTest    #endregion Controls Value

    #region Override Methods
    public override void ChangeStandardOrDiameter(Int32 control, Int32 selection) {
      //throw new NotImplementedException();
    }

    /// <summary>
    /// поготвя за визуализация информацията за координатната система
    /// </summary>
    public override void GetCoordinateSystem() {
      //throw new NotImplementedException();
    }
    public override void RadioButtonChek() {
      throw new NotImplementedException();
    }

    public override void DiameterOrThicknessChange() {
      throw new NotImplementedException();
    }
    #endregion Override Methods

    #region Abstract Methods
    /// <summary>добавя контролите в swPMPage свързани с модела</summary>
    public abstract void AddControlsOnPMPage();
    #endregion Abstract Methods

    #endregion before 2019-04-08
  } // PatternCtrlPMPageBase

}
