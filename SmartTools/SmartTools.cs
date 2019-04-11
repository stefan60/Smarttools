using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

using SolidWorksTools;
using SolidWorksTools.File;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SmartTools {

  [Guid("81E4572F-3F5A-448A-8B0A-0F269AD01B1D"), ComVisible(true)]
  [SwAddin(Description = Prime.ADDIN_DESCRIPTION,
           Title = Prime.ADDIN_NAME,
           LoadAtStartup = true)]
  public class SmartTools: ISwAddin {

    #region Local Variables
    /// <summary>указателя на SW API върнат от SW</summary>
    private ISldWorks iSldApp = default(ISldWorks);
    /// <summary>Указател към Command Manager на SW</summary>
    private ICommandManager iCmdMgr = default(ICommandManager);
    /// <summary>
    /// Указател към групата обекти на модели в CommandManager на SW
    /// </summary>
    private ICommandGroup iCmdGroup = default(ICommandGroup);
    /// <summary>
    /// Обект за обработка на иконите на приложението
    /// </summary>
    private BitmapHandler iBitmap = default(BitmapHandler);
    /// <summary>
    /// Списък с обекти на моделите използвани от приложението
    /// </summary>
    private List<Lib.PatternBase> iPatterns;
    /// <summary>Номер на приложението в средата на SW</summary>
    private int addinID = 0;
    #endregion Local Variables

    #region Public Variables
    /// <summary>указател към SW APP</summary>
    public ISldWorks SwApp {
      get { return this.iSldApp; }
    }
    public List<Lib.PatternBase> Patterns {
      get { return this.iPatterns; }
    }
    #endregion Public Variables

    #region Connect and Disconnect To SW
    /// <summary>Инстанцира приложението в средата на SW</summary>
    public SmartTools() { 
    } // SmartTools

    /// <summary> Осъществява връзката на приложението със SW</summary>
    /// <param name="ThisSW">указател на SW API върнат от SW</param>
    /// <param name="Cookie">номер на приложението в средата на SW</param>
    /// <returns>TRUE ако приложението е осъществило връзката със SW</returns>
    public Boolean ConnectToSW(Object ThisSW, Int32 Cookie) {
      this.iSldApp = (ISldWorks)ThisSW;
      this.addinID = Cookie;
      this.iSldApp.SetAddinCallbackInfo2(0, this, this.addinID);
      this.iCmdMgr = this.iSldApp.GetCommandManager(this.addinID);

      Microsoft.Win32.RegistryKey hkcu = Microsoft.Win32.Registry.CurrentUser;
      string keyName = "Software\\" + Prime.ADDIN_NAME;

      Microsoft.Win32.RegistryKey key = hkcu.OpenSubKey(keyName);
      Prime.PathXML = (string)key.GetValue(Prime.PATH_XML_RKEY);


      Prime.AddIn = this;

      this.AddPatternsMgr();

      return true;
    } // ConnectToSW

    public Boolean DisconnectFromSW() {
      this.RemovePatternsMgr();

      System.Runtime.InteropServices.Marshal.ReleaseComObject(this.iCmdMgr);
      this.iCmdMgr = null;
      System.Runtime.InteropServices.Marshal.ReleaseComObject(this.iSldApp);
      this.iSldApp = null;
      System.GC.Collect();
      System.GC.WaitForPendingFinalizers();
      System.GC.Collect();
      System.GC.WaitForPendingFinalizers();
      return true;
    } // DisconnectFromSW
    #endregion Connect and Disconnect To SW

    #region User Interface Methods
    /// <summary>
    /// Добавя иконите на моделите в раздел/tab на CommandManager на SW
    /// </summary>
    protected void AddPatternsMgr() {
      this.AddPatterns();
      bool ignorePrevious = this.CreateCommandGroup(out bool getDataResult);
      this.SetIcons();
      int options = (int)swCommandItemType_e.swMenuItem | (int)swCommandItemType_e.swToolbarItem;
      int[] cmdIndex = new int[this.iPatterns.Count];
      // добавя моделите в групата на CommandManager
      for (int j = 0; j < this.iPatterns.Count; j++) {
        cmdIndex[j] = this.iCmdGroup.AddCommandItem2(this.iPatterns.ElementAt(j).modelType.ToString(),
          -1, Prime.GetPatternStringValue(this.iPatterns.ElementAt(j).modelType, Prime.PatternElement_e.Hint),
          //this.iPatterns.ElementAt(j).Hint,
          Prime.GetPatternStringValue(this.iPatterns.ElementAt(j).modelType, Prime.PatternElement_e.Name),
          //this.iPatterns.ElementAt(j).ToolTip,
          0, "PushButton(" + ((int)this.iPatterns.ElementAt(j).modelType).ToString() + ")",
          "EnablePushButton(" + this.iPatterns.ElementAt(j).modelType.ToString(),
          (int)this.iPatterns.ElementAt(j).modelType, options);
      }
      this.iCmdGroup.HasToolbar = true;
      this.iCmdGroup.HasMenu = true;
      this.iCmdGroup.Activate();

      foreach (int t in Prime.PattensTypes) {
        CommandTab cmdTab = this.iCmdMgr.GetCommandTab(t, Prime.TAB_NAME);
        if (cmdTab != null & !getDataResult | ignorePrevious) {
          this.iCmdMgr.RemoveCommandTab(cmdTab);
          cmdTab = null;
        }
        if (cmdTab == null) {
          cmdTab = this.iCmdMgr.AddCommandTab(t, Prime.TAB_NAME);
          CommandTabBox tabBox1 = cmdTab.AddCommandTabBox();
          int count = cmdIndex.Length;
          int[] cmdIDs = new int[3];
          int[] textType = new int[3];
          // детайли без чертежи
          for (int j = 0; j < 3; j++) {
            cmdIDs[j] = this.iCmdGroup.get_CommandID(j);
            textType[j] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow;
            count--;
            if (count < 1)
              break;
          }
          tabBox1.AddCommands(cmdIDs, textType);
          // детайли с чертежи
          if (cmdIndex.Length > 3) {
            cmdIDs = new int[3];
            textType = new int[3];
            CommandTabBox tabBox2 = cmdTab.AddCommandTabBox();
            for (int j = 0; j < 3; j++) {
              cmdIDs[j] = this.iCmdGroup.get_CommandID(j + 3);
              textType[j] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow;
              count--;
              if (count < 1)
                break;
            }
            tabBox2.AddCommands(cmdIDs, textType);
            cmdTab.AddSeparator(tabBox2, cmdIDs[0]);
          }
          // отвори
          if (cmdIndex.Length > 6) {
            cmdIDs = new int[3];
            textType = new int[3];
            CommandTabBox tabBox3 = cmdTab.AddCommandTabBox();
            for (int j = 0; j < 3; j++) {
              cmdIDs[j] = this.iCmdGroup.get_CommandID(j + 6);
              textType[j] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow;
              count--;
              if (count < 1)
                break;
            }
            tabBox3.AddCommands(cmdIDs, textType);
            cmdTab.AddSeparator(tabBox3, cmdIDs[0]);
          }
          // други инструменти - редактиране, чертеж
          if (cmdIndex.Length > 9) {
            cmdIDs = new int[3];
            textType = new int[3];
            CommandTabBox tabBox4 = cmdTab.AddCommandTabBox();
            for (int j = 0; j < 3; j++) {
              cmdIDs[j] = this.iCmdGroup.get_CommandID(j + 9);
              textType[j] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow;
              count--;
              if (count < 1)
                break;
            }
            tabBox4.AddCommands(cmdIDs, textType);
            cmdTab.AddSeparator(tabBox4, cmdIDs[0]);
          }
        }
      }
    } // AddPatternsMgr

    protected void RemovePatternsMgr() {
      this.iBitmap.Dispose();
      this.iCmdMgr.RemoveCommandGroup2(Prime.CMD_GROUP_ID, false);
    } // RemovePatternsMgr

    /// <summary>
    /// Създава групата за обектите на моделите на приложението
    /// </summary>
    /// <param name="getDataResult">изходящ флаг за налична информация в
    /// регистрите за обекти на моделите</param>
    /// <returns>TRUE окрита е разлика в броя на обектите</returns>
    protected bool CreateCommandGroup(out bool getDataResult) {
      bool ignorePrev = false;
      getDataResult = this.iCmdMgr.GetGroupDataFromRegistry(Prime.CMD_GROUP_ID,
        out object registryIDs);
      if (getDataResult) {
        List<int> storedList = new List<int>((int[])registryIDs);
        storedList.Sort();
        List<int> addinList = new List<int>();
        foreach (var pattern in this.iPatterns) {
          addinList.Add((int)pattern.modelType);
        }
        addinList.Sort();
        if (addinList.Count != storedList.Count) {
          ignorePrev = true;
        } else {
          for (int j = 0; j < addinList.Count; j++) {
            if (addinList[j] != storedList[j]) {
              ignorePrev = true;
              break;
            }
          }
        }
      }
      int error = 0;
      this.iCmdGroup = this.iCmdMgr.CreateCommandGroup2(Prime.CMD_GROUP_ID,
        Prime.CMD_GROUP_NAME, "", "", -1, ignorePrev, ref error);
      return ignorePrev;
    } // CreateCommandGroup

    /// <summary>
    /// Подготвя файловте с икони използвани от приложението
    /// </summary>
    protected void SetIcons() {
      string[] icons = new string[6];
      string[] mainIcons = new string[6];
      if (this.iBitmap == null)
        this.iBitmap = new BitmapHandler();
      System.Reflection.Assembly asm = System.Reflection.Assembly.GetAssembly(this.GetType());
      icons[0] = this.iBitmap.CreateFileFromResourceBitmap("SmartTools.Icons.toolbar20x.png", asm);
      icons[1] = this.iBitmap.CreateFileFromResourceBitmap("SmartTools.Icons.toolbar32x.png", asm);
      icons[2] = this.iBitmap.CreateFileFromResourceBitmap("SmartTools.Icons.toolbar40x.png", asm);
      icons[3] = this.iBitmap.CreateFileFromResourceBitmap("SmartTools.Icons.toolbar64x.png", asm);
      icons[4] = this.iBitmap.CreateFileFromResourceBitmap("SmartTools.Icons.toolbar96x.png", asm);
      icons[5] = this.iBitmap.CreateFileFromResourceBitmap("SmartTools.Icons.toolbar128x.png", asm);
      this.iCmdGroup.IconList = icons;
      mainIcons[0] = this.iBitmap.CreateFileFromResourceBitmap("SmartTools.Icons.mainicon_20.png", asm);
      mainIcons[1] = this.iBitmap.CreateFileFromResourceBitmap("SmartTools.Icons.mainicon_32.png", asm);
      mainIcons[2] = this.iBitmap.CreateFileFromResourceBitmap("SmartTools.Icons.mainicon_40.png", asm);
      mainIcons[3] = this.iBitmap.CreateFileFromResourceBitmap("SmartTools.Icons.mainicon_64.png", asm);
      mainIcons[4] = this.iBitmap.CreateFileFromResourceBitmap("SmartTools.Icons.mainicon_96.png", asm);
      mainIcons[5] = this.iBitmap.CreateFileFromResourceBitmap("SmartTools.Icons.mainicon_128.png", asm);
      this.iCmdGroup.MainIconList = mainIcons;
      asm = null;
    } // SetIcons
    #endregion User Interface Methods

    /// <summary>
    /// Добавя в списък обектите на моделите използвани от приложението
    /// </summary>
    protected void AddPatterns() {
      if (this.iPatterns == null)
        this.iPatterns = new List<Lib.PatternBase>();
      this.iPatterns.Add(new Patterns.Plate());
      this.iPatterns.Add(new Patterns.Axis());
      this.iPatterns.Add(new Patterns.Tube());
      this.iPatterns.Add(new Patterns.Mantel());
      this.iPatterns.Add(new Patterns.Cover());
      this.iPatterns.Add(new Patterns.Bottom());
      this.iPatterns.Add(new Patterns.Hole());
      this.iPatterns.Add(new Patterns.OutHole());
      this.iPatterns.Add(new Patterns.InHole());
      this.iPatterns.Add(new Patterns.EditPattern());
    } // AddPatterns

    // <summary> Функция управляваща стартирането на функцията Run</summary>
    /// <param name="strID">Номер на инструмента</param>
    /// <returns>int</returns>
    public int EnablePushButton(string strID) {
      return 1;
    } // EnablePushButton

    /// <summary>
    /// Започва изпълнението на модел/инструмент
    /// </summary>
    /// <param name="strID">номер на модел/инструмент</param>
    public void PushButton(string strID) {
      int id = Int32.Parse(strID);
      foreach (Lib.PatternBase pattern in this.iPatterns) {
        if ((int)pattern.modelType == id) {
          pattern.Execute();
          break;
        }
      }
     } // PushButton

    #region Registration AddIn
    [ComRegisterFunction]
    public static void RegisterFunction(Type t) {
      SwAddinAttribute SWattr = null;
      Type type = typeof(SmartTools);
      foreach (System.Attribute attr in type.GetCustomAttributes(false)) {
        if (attr is SwAddinAttribute) {
          SWattr = attr as SwAddinAttribute;
          break;
        }
      }
      try {
        Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
        Microsoft.Win32.RegistryKey hkcu = Microsoft.Win32.Registry.CurrentUser;
        string keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID.ToString() + "}";
        Microsoft.Win32.RegistryKey addinkey = hklm.CreateSubKey(keyname);
        addinkey.SetValue(null, 0);
        addinkey.SetValue("Description", SWattr.Description);
        addinkey.SetValue("Title", SWattr.Title);
        keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID.ToString() + "}";
        addinkey = hkcu.CreateSubKey(keyname);
        addinkey.SetValue(null, Convert.ToInt32(SWattr.LoadAtStartup), Microsoft.Win32.RegistryValueKind.DWord);
        keyname = "Software\\" + Prime.ADDIN_NAME;
        addinkey = hkcu.CreateSubKey(keyname);
        addinkey.SetValue(Prime.PATH_XML_RKEY, Prime.PATH_XML);
      } catch (System.NullReferenceException nl) {
        Console.WriteLine("There was a problem registering this dll: SWattr is null. \n\"" + nl.Message + "\"");
        System.Windows.Forms.MessageBox.Show("There was a problem registering this dll: SWattr is null.\n\"" + nl.Message + "\"");
      } catch (System.Exception e) {
        Console.WriteLine(e.Message);System.Windows.Forms.MessageBox.Show("There was a problem registering the function: \n\"" + e.Message + "\"");
      }
    } // RegisterFunction

    [ComUnregisterFunctionAttribute]
    public static void UnregisterFunction(Type t) {
      try {
        Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
        Microsoft.Win32.RegistryKey hkcu = Microsoft.Win32.Registry.CurrentUser;
        string keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID.ToString() + "}";
        hklm.DeleteSubKey(keyname);
        keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID.ToString() + "}";
        hkcu.DeleteSubKey(keyname);
        keyname = "Software\\" + Prime.ADDIN_NAME;
        hkcu.DeleteSubKey(keyname);
      } catch (System.NullReferenceException nl) {
        Console.WriteLine("There was a problem unregistering this dll: " + nl.Message);
        System.Windows.Forms.MessageBox.Show("There was a problem unregistering this dll: \n\"" + nl.Message + "\"");
      } catch (System.Exception e) {
        Console.WriteLine("There was a problem unregistering this dll: " + e.Message);
        System.Windows.Forms.MessageBox.Show("There was a problem unregistering this dll: \n\"" + e.Message + "\"");
      }
    }
    #endregion Registration AddIn
  } // SmartTools

}
