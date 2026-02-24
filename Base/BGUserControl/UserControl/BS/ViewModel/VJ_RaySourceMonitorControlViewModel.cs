using BG_Entities;
using BG_Services;
using BG_WorkFlow;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BGUserControl
{
    public class VJ_RaySourceMonitorControlViewModel : BaseMvvm
    {
        public ObservableCollection<DoseModel> DataSource = new ObservableCollection<DoseModel>();
        public ObservableCollection<ParamaterLabel> BSParamaterLabelList = new ObservableCollection<ParamaterLabel>();

        CarDoseBLL _carDoseBll = new CarDoseBLL();
        protected override void InquirePlcStandardStatus(PLCDBStatus PlcDbStatus)
        {
            if (Common.controlVersion != ControlVersion.BS)
            {
                return;
            }
            ReflashUI(PlcDbStatus);
        }
        protected override void BSInquirePlcStandardStatus(PLCDBStatus PlcDbStatus)
        {
            if (Common.controlVersion == ControlVersion.BS)
            {
                return;
            }
            ReflashUI(PlcDbStatus);
        }
        private Dispatcher UIDispatcher => Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        /// <summary>
        /// UI
        /// </summary>
        /// <param name="PlcDbStatus"></param>
        private async void ReflashUI(PLCDBStatus PlcDbStatus)
        {
            await UIDispatcher.InvokeAsync(() =>
            {
                try
                {
                    foreach (var item in DataSource)
                    {
                        int Index = DataSource.IndexOf(item);
                        if (item == null) return;

                        if (item.BG_ValueType.ToLower() == "float" || string.IsNullOrEmpty(item.BG_ValueType.ToLower()))
                        {
                            if (Convert.ToInt32(item.BgDoseIndex) < 25
                            && Convert.ToInt32(item.BgDoseIndex) < PlcDbStatus.FloatArray.Count
                            && BSParamaterLabelList.Count > Index)
                            {
                                double Value = PlcDbStatus.FloatArray[Convert.ToInt32(item.BgDoseIndex)];
                                BSParamaterLabelList[Index].ParamaterValue = $@"{Value.ToString("F2")} {item.BgUnit}";
                                double.TryParse(item.BS_ThorValue, out double thorValue);
                                if (Value < thorValue)
                                {
                                    BSParamaterLabelList[Index].ParamaterForeColor = "#FF0000";
                                }
                                else
                                {
                                    BSParamaterLabelList[Index].ParamaterForeColor = "#014087";
                                }
                            }
                        }
                        else if (item.BG_ValueType.ToLower() == "ushort")
                        {
                            if (Convert.ToInt32(item.BgDoseIndex) < 25
                          && Convert.ToInt32(item.BgDoseIndex) < PlcDbStatus.IntArray.Count
                          && BSParamaterLabelList.Count > Index)
                            {
                                short Value = PlcDbStatus.IntArray[Convert.ToInt32(item.BgDoseIndex)];
                                BSParamaterLabelList[Index].ParamaterValue = $@"{Value.ToString("F2")} {item.BgUnit}";
                                double.TryParse(item.BS_ThorValue, out double thorValue);
                                if (Value < thorValue)
                                {
                                    BSParamaterLabelList[Index].ParamaterForeColor = "#FF0000";
                                }
                                else
                                {
                                    BSParamaterLabelList[Index].ParamaterForeColor = "#014087";
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    BGLogs.Log.GetDistance().WriteInfoLogs(ex.StackTrace);
                }
            });
        }

        public void InitDoseList(string FilePath)
        {
            DataSource.Clear();
            _carDoseBll.GetDoseModelDataModel(FilePath).Where(q => q.BgIsShow == "1")
            .ToList().ForEach(q => { DataSource.Add(q); });
        }
    }
}
