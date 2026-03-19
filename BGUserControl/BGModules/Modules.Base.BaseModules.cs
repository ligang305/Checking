
using BG_Entities;
using BG_Services;
using BGCommunication;
using BGModel;
using BGUserControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shell;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
using BG_WorkFlow;
using CMW.Common.Utilities;

namespace BGUserControl

{
    /// <summary>
    /// 各个操作
    /// </summary>
    public abstract class BaseModules : BaseWindows, IConditionView
    {
        protected ObservableCollection<StatusModel> StatusList = new ObservableCollection<StatusModel>();
        protected bool IsVisible = false;
        protected ControlVersion cv = ControlVersion.Car;
        //你要导入的类型和元数据数组，所有继承IConditionView接口并导出的页面都在这个数组里  
        [ImportMany("ContentPage", typeof(BaseModules), Source = ImportSource.Local, RequiredCreationPolicy = CreationPolicy.NonShared)]
        //保存插件的内存对象
        public Lazy<BaseModules, IMetaData>[] ContentPlugins { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseModules(ControlVersion _cv)
        {
            BoosttingSetting = UpdateStatusNameAction("BoosttingSetting");
            EquipmentResetFair = UpdateStatusNameAction("EquipmentResetFair");
            EquipmentResetSuccess = UpdateStatusNameAction("EquipmentResetSuccess");
            UnConnectionWithPlc = UpdateStatusNameAction("UnConnectionWithPlc");
            Loaded += BaseModules_Loaded;
            Unloaded += BaseModules_Unloaded;
            cv = _cv;
        }

        /// <summary>
        /// 当模块UnLoad的时候 发送命令给PLC说明将手动模式调整为自动模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseModules_Unloaded(object sender, RoutedEventArgs e)
        {
            IsVisible = false;
        }

        protected void BaseModules_Loaded(object sender, RoutedEventArgs e)
        {
            IsVisible = true;
            Base_SwitchLanguage(ConfigServices.GetInstance().localConfigModel.LANGUAGE);
            //初始化状态值
            StatusList.Clear();
            HitChModelBLL.GetInstance().GetHitchModelDataModel
                (SystemDirectoryConfig.GetInstance().GetHittingConfig(cv)).Where(q => q.StatusOwner.Contains("")).ToList().ForEach(q => StatusList.Add(q));

        }

    
        /// <summary>
        /// 完成添加的按钮
        /// </summary>
        protected Border MakeAddButton(string Text)
        {
            Border BtnSendCommand = new Border()
            {
                Width = 50,
                Height = 30,
                Style = (Style)this.FindResource("diyBtnCarHand"),
            };
            Label lblContent = MakeLabel(Text);
            lblContent.Foreground = Brushes.White;
            BtnSendCommand.Child = lblContent;
            return BtnSendCommand;
        }

        /// 生成复位按钮
        /// </summary>
        /// <param name="_csm"></param>
        /// <param name="_dp"></param>
        /// <returns></returns>
        protected Border MakeSettingBorder(CommonSettingModel _csm = null, string LabelName = "Reset")
        {
            Border BtnSendCommand = new Border()
            {
                Width = 50,
                Height = 30,
                Style = (Style)this.FindResource("diyBtnCarHand"),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            //BtnSendCommand.PreviewMouseUp += BtnSendCommand_PreviewMouseUp;
            Label btnInner = new Label()
            {
                Content = UpdateStatusNameAction(LabelName), //"设置",
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#FFFFFF")
            };
            btnInner.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            BtnSendCommand.Child = btnInner;
            if (_csm != null)
            {
                BtnSendCommand.SetBinding(Border.TagProperty,
                    new Binding(".") { Source = _csm, Mode = BindingMode.TwoWay });
            }

            return BtnSendCommand;
        }
        /// <summary>
        /// 生成Label
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        protected Label MakeLabel(string Name)
        {
            Label lblMax = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#1A4F85"),
                Content = UpdateStatusNameAction(Name)
            };
            lblMax.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            return lblMax;
        }
        /// <summary>
        /// 生成Label
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        protected TextBlock MakeTextBlock(string Name)
        {
            TextBlock lblMax = new TextBlock()
            {
                TextTrimming = TextTrimming.CharacterEllipsis,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = StrToBrush("#1A4F85"),
                Text = Name// UpdateStatusNameAction(Name)
            };
            lblMax.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            return lblMax;
        }
        /// <summary>
        /// 因为设置了某个值之后没办法反馈是否设置成功，所以模拟情况下需要去查询是否设置成功
        /// </summary>
        /// <param name="isSendSuccess"></param>
        /// <param name="CommondTag"></param>
        /// <param name="SelectText"></param>
        /// <returns></returns>
        public virtual bool SearchSettingStatus(int index)
        {
            if (index > GlobalRetStatus.Count) return false;
            return GlobalRetStatus[index];
        }

        /// <summary>
        /// 因为设置了某个值之后没办法反馈是否设置成功，所以模拟情况下需要去查询是否设置成功
        /// </summary>
        /// <param name="isSendSuccess"></param>
        /// <param name="CommondTag"></param>
        /// <param name="SelectText"></param>
        /// <returns></returns>
        public virtual bool SearchSettingStatus(bool isSendSuccess, CommonSettingModel CommondTag, string SelectText, ObservableCollection<StatusModel> StatusList = null)
        {
            //Thread.Sleep(50);
            //Inquire();
            List<string> PositionList = CommondTag.CommonSettingIndex.Split(';').ToList();
            Dictionary<string, bool> ByteToResult = new Dictionary<string, bool>();
            isSendSuccess = false;
            foreach (var positionItem in PositionList)
            {
                ByteToResult[positionItem] = PLCControllerManager.GetInstance().ReadPositionValue(positionItem); //GlobalPLCProtocol.GetStatus(positionItem);
                if (ByteToResult[positionItem])
                {
                    if (StatusList == null) break;
                    var SingleObject = StatusList.FirstOrDefault(q => q.BgCommandPosition == positionItem);
                    string DisplayName = UpdateStatusNameAction(SingleObject?.StatusName);
                    if (DisplayName == SelectText)
                    {
                        isSendSuccess = true;
                        break;
                    }
                }
            }

            return isSendSuccess;
        }

        /// <summary>
        /// 因为设置了某个值之后没办法反馈是否设置成功，所以模拟情况下需要去查询是否设置成功
        /// </summary>
        /// <param name="isSendSuccess"></param>
        /// <param name="CommondTag"></param>
        /// <param name="SelectText"></param>
        /// <returns></returns>
        public virtual bool SearchSettingDose(bool isSendSuccess, CommonSettingModel CommondTag, string SelectText)
        {
            //Inquire();
            //这个位子是用来查询剂量 查询该剂量所在的位子 用来对应128个状态的查询位子CommonSettingIndex
            int Position = Convert.ToInt32(CommondTag.CommonSettingIndex);
            if (Position < GlobalDoseStatus.Count())
            {
                if (IsSearchDoseSuccess)
                {
                    ushort Value = GlobalDoseStatus[Position];
                    isSendSuccess = Value == Convert.ToUInt16(SelectText);
                }
            }

            return isSendSuccess;
        }
        /// <summary>
        /// 点击设置按钮通用方法
        /// </summary>
        /// <param name="sender"></param>
        public virtual bool SendCommand(object sender)
        {
            Border bd = sender as Border;
            bool isSendSuccess = false;
            if (bd != null)
            {
                CommonSettingModel CommondTag;
                string SelectText;
                InitPara(bd, out CommondTag, out SelectText);
                PlCSendType type = (PlCSendType)Enum.Parse(typeof(PlCSendType), CommondTag.CommonSettingSendType);
                switch (type)
                {
                    ///三个地址三个值，但是表示的是统一中国状态就是用于这种操作
                    case PlCSendType.OneByteThree:
                        isSendSuccess = SendOneByteThree(CommondTag, SelectText);
                        break;
                    ///一个位置对应0/1 用来开关，的适用于这种状态
                    case PlCSendType.OneByteOneOrTwo:
                        isSendSuccess = OneByteOneOrTwo(CommondTag);
                        break;
                    case PlCSendType.OneByteTrueAndFalse:
                        isSendSuccess = OneByteTrueAndFalse(CommondTag);
                        break;
                    ///一个地址只要传一个值，
                    case PlCSendType.OneByteDefaultOne:
                        //如果发现需要发送的命令是角度,需要将其他三个角度设置为false;
                        isSendSuccess = SendOtherCommand(isSendSuccess, CommondTag);
                        isSendSuccess = OneByteDefaultOne(CommondTag);
                        break;
                    //传入文本值
                    case PlCSendType.StartBytePositionDefaultText:
                        isSendSuccess = StartBytePositionDefaultText(CommondTag, SelectText);
                        break;
                    default:
                        break;
                }
            }
            return isSendSuccess;
        }
        protected static void InitPara( Border bd, out CommonSettingModel CommondTag, out string SelectText)
        {
            CommondTag = bd.Tag as CommonSettingModel;
            Label child = bd.Child as Label;
            BuryingPoint($"Setting {CommondTag.CommonSettingDisplayName.ToString()} Command");
            CommonDeleget.WriteLogAction($"设置{CommondTag.CommonSettingDisplayName.ToString()}命令",LogType.ScanStep,true);
            var fe = child.Tag;
            SelectText = string.Empty;
            if (fe is ComboBox)
            {
                SelectText = (fe as ComboBox).Text;
            }
            else if (fe is TextBox)
            {
                SelectText = (fe as TextBox).Text;
                if (CommondTag.CommonSettingName.Contains("BoostingTO")||
                    CommondTag.CommonSettingName.Contains("EnterTO") ||
                    CommondTag.CommonSettingName.Contains("ExitTO") ||
                    CommondTag.CommonSettingName.Contains("ControlRoomTO"))   
                {
                    SelectText = (Convert.ToDouble(SelectText)).ToString();
                }
            }
        }
        protected bool StartBytePositionDefaultText(CommonSettingModel CommondTag, string SelectText)
        {
            bool isSendSuccess = false;
         
            ushort content = 0;
            switch (CommondTag.CommonSettingValueType)
            {
                case "ushort":
                    content = Convert.ToUInt16(SelectText);
                    if (CommondTag.CommonSettingName.Contains("FrequencySetting"))
                    {
                        if (!string.IsNullOrEmpty(CommondTag.Error)) return false;
                        isSendSuccess = ImageImportDll.SX_SetFrequency(ImageImportDll.intPtr, content) == 0 ? true : false;
                        ConfigServices.GetInstance().localConfigModel.Freeze = content.ToString();
                        UpdateConfigs("Frequency", ConfigServices.GetInstance().localConfigModel.Freeze, Section.SOFT);
                    }
                    else
                    {
                        if(CommondTag.CommonSettingPLCValue.Contains("M") ||
                            CommondTag.CommonSettingPLCValue.Contains("DB")||
                            CommondTag.CommonSettingPLCValue.Contains("I")||
                            CommondTag.CommonSettingPLCValue.Contains("O"))
                        {
                            isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(CommondTag.CommonSettingPLCValue, content);
                        }
                        else
                        {
                            byte StartPosition = Convert.ToByte(CommondTag.CommonSettingPLCValue);
                            isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(StartPosition, content);
                        }
                    }
                    break;
                case "float":
                    var floatContent = Convert.ToSingle(SelectText);
                    if (CommondTag.CommonSettingPLCValue.Contains("M") ||
                         CommondTag.CommonSettingPLCValue.Contains("DB") ||
                         CommondTag.CommonSettingPLCValue.Contains("I") ||
                         CommondTag.CommonSettingPLCValue.Contains("O"))
                    {
                        isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(CommondTag.CommonSettingPLCValue, floatContent);
                    }
                    else
                    {
                        byte _StartPosition = Convert.ToByte(CommondTag.CommonSettingPLCValue);
                        isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(_StartPosition, floatContent);
                    }
                    break;
            }
            
            isSendSuccess = SearchSettingDose(isSendSuccess, CommondTag, SelectText);
            return isSendSuccess;
        }
        protected bool OneByteDefaultOne(CommonSettingModel CommondTag,bool value = true)
        {
            bool isSendSuccess;
            byte StartPosition = Convert.ToByte(CommondTag.CommonSettingValue.Split('?')[0].Split('.')[0]);
            byte StartPositionIndex = Convert.ToByte(CommondTag.CommonSettingValue.Split('?')[0].Split('.')[1]);
            int SearchIndex = Convert.ToInt32(CommondTag.CommonSettingValue.Split('?')[1]);
            isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(CommondTag.CommonSettingPLCValue, value); //GlobalPLCProtocol.Execute(StartPosition, StartPositionIndex, value);
            isSendSuccess = SearchSettingStatus(SearchIndex);
            return isSendSuccess;
        }
        private static bool OneByteOneOrTwo(CommonSettingModel CommondTag)
        {
            bool isSendSuccess;
            //byte StartBytePosition = Convert.ToByte(CommondTag.CommonSettingPLCValue.Split('.')[0]);
            //byte StartBytePositionIndex = Convert.ToByte(CommondTag.CommonSettingPLCValue.Split('.')[1]);
            isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(CommondTag.CommonSettingPLCValue, CommondTag.CommonSettingValue == "1");// (StartBytePosition, StartBytePositionIndex, CommondTag.CommonSettingValue == "1");
            isSendSuccess = true;
            return isSendSuccess;
        }
        private static bool OneByteTrueAndFalse(CommonSettingModel CommondTag)
        {
            bool isSendSuccess;
            //byte StartBytePosition = Convert.ToByte(CommondTag.CommonSettingPLCValue.Split('.')[0]);
            //byte StartBytePositionIndex = Convert.ToByte(CommondTag.CommonSettingPLCValue.Split('.')[1]);
            isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(CommondTag.CommonSettingPLCValue, true);// (StartBytePosition, StartBytePositionIndex, CommondTag.CommonSettingValue == "1");
            Thread.Sleep(500);
            isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(CommondTag.CommonSettingPLCValue, false);// (StartBytePosition, StartBytePositionIndex, CommondTag.CommonSettingValue == "1");
            isSendSuccess = true;
            return isSendSuccess;
        }
        /// <summary>
        /// 发送一个字节代表三种状态的命令
        /// </summary>
        /// <param name="CommondTag"></param>
        /// <param name="SelectText"></param>
        /// <returns></returns>
        private bool SendOneByteThree(CommonSettingModel CommondTag, string SelectText)
        {
            bool isSendSuccess;
            string SelectValue = CommondTag.CommonSettingValue.Trim('[').Trim(']').Split('?')[0];
            string PositionValue = CommondTag.CommonSettingValue.Trim('[').Trim(']').Split('?')[1];
            string StartThreeBytePosition = PositionValue.Split('.')[0];
            string StartThreeBytePositionIndex = PositionValue.Split('.')[1];
            //如果发送的是26.6命令
            if (StartThreeBytePosition.Contains("26") && StartThreeBytePositionIndex == "6")
            {
                //就先发送一个单能命令 然后在发送高低能指令
                isSendSuccess = SetCommand(CommandDic[Command.SettingSingleDouble], false);
            }
            isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(PositionValue, SelectValue == "1"); //GlobalPLCProtocol.Execute(StartThreeBytePosition, StartThreeBytePositionIndex, SelectValue == "1");
            isSendSuccess = SearchSettingStatus(isSendSuccess, CommondTag, SelectText, StatusList);
            return isSendSuccess;
        }
        /// <summary>
        /// 把不相关的命令都设置为false
        /// </summary>
        /// <param name="isSendSuccess"></param>
        /// <param name="CommondTag"></param>
        /// <returns></returns>
        private bool SendOtherCommand(bool isSendSuccess, CommonSettingModel CommondTag)
        {
            List<string> tempAngleList = GetAngleList(cv,CommondTag);
            if (tempAngleList.Contains(CommondTag.CommonSettingValue.Split('?')[0]))
            {
                foreach (var item in tempAngleList)
                {
                    if (item == CommondTag.CommonSettingValue.Split('?')[0])
                    {
                        continue;
                    }
                    byte TempStartPosition = Convert.ToByte(item.Split('.')[0]);
                    byte TempPositionIndex = Convert.ToByte(item.Split('.')[1]);
                    isSendSuccess = PLCControllerManager.GetInstance().WritePositionValue(item, false); //GlobalPLCProtocol.Execute(TempStartPosition, TempPositionIndex, false);
                }
            }

            return isSendSuccess;
        }


        private List<string> GetAngleList(ControlVersion _CV,CommonSettingModel CommondTag)
        {
            switch (_CV)
            {
                case ControlVersion.SelfWorking:
                    if (CommondTag.CommonSettingName == "SelfHandAngle")
                    {
                        return SelfAutoAngleList;
                    }
                    else if (CommondTag.CommonSettingName == "BoostAngle")
                    {
                        return BoostingList;
                    }
                    return angleList;
                case ControlVersion.Car:
                    return angleList;
                default:
                        return angleList;
            }
        }

        /// <summary>
        /// 切换中英文
        /// </summary>
        public override void Base_SwitchLanguage(string Language)
        {
            InitLabelForLanguage();
        }

        /// <summary>
        /// 初始化语言
        /// </summary>
        private void InitLabelForLanguage()
        {
            UnConnectionWithPlc = UpdateStatusNameAction("UnConnectionWithPlc");
            EquipmentResetSuccess = UpdateStatusNameAction("EquipmentResetSuccess");
            BoosttingSetting = UpdateStatusNameAction("BoosttingSetting");
        }
  
        public virtual string GetName()
        {
            return "模块";
        }
        public virtual double GetHeight()
        {
            return 400;
        }
        public virtual double GetWidth()
        {
            return 600;
        }

        public virtual void SetSelectTabName(string TabName)
        {
            
        }
        public virtual bool IsConnectionEquipment()
        {
            return PLCControllerManager.GetInstance().IsConnect();
        }
        public virtual void Show(Window _OwnerWin)
        {

        }
        public virtual void Close()
        {
            CurrentWindow?.Close();
        }

        public virtual void SetCarVersion(ControlVersion _cv)
        {
            cv = _cv;
        }
    }

}
