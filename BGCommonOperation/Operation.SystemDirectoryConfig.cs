
using BG_Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace CMW.Common.Utilities
{
    public class SystemDirectoryConfig: BaseInstance<SystemDirectoryConfig>
    {
        public static string AppDir;
        public static string ServerSaveLocalPath;
        
        /// <summary>
        /// 获取本地数据库文件
        /// </summary>
        /// <returns></returns>
        public string GetSystemDataBase()
        {
            if (!Directory.Exists($@"{AppDir}\Data"))
            {
                Directory.CreateDirectory($@"{AppDir}\Data");
            }
            return string.Format($@"Data Source = {AppDir}\Data\BGAUTODATABASE.db") ;
        }
        public string GetConfigIni()
        {
            return AppDir + @"CONFIG\SysConfig.ini";
        }
        /// <summary>
        /// 获取探测器DLL日志路径
        /// </summary>
        /// <returns></returns>
        public string GetScanLogFile()
        {
            return AppDir + $@"Logs\ScanIog_{DateTime.Now.ToString("yyyy-MM-dd")}.txt";
        }
        /// <summary>
        /// 获取日志文件夹
        /// </summary>
        /// <returns></returns>
        public string GetLogDir()
        {
            return AppDir + @"Logs";
        }
        /// <summary>
        /// 设置扫描模块配置文件
        /// </summary>
        /// <returns></returns>
        public string GetScanConfigFile()
        {
            return AppDir + @"Config\ScanX.xml";
        }
        /// <summary>
        /// 设置扫描模块配置文件
        /// </summary>
        /// <returns></returns>
        public string GetScanConfigFile(string IpAddress)
        {
            return AppDir +$@"Config\{IpAddress}\ScanX.xml";
        }
        /// <summary>
        /// 获取探测器存储图片的路径
        /// </summary>
        /// <returns></returns>
        public string GetScanImageFile()
        {
            string DirPath = AppDir + @"ScanImage\";
            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(DirPath);
            }
            return DirPath;
        }
        /// <summary>
        /// 获取背散设备扫描图片路径（合并后）
        /// </summary>
        /// <returns></returns>
        public string GetBSScanImageFile()
        {
            string DirPath = AppDir + @"BSScanImage\";
            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(DirPath);
            }
            return DirPath;
        }
        /// <summary>
        /// 获取物质颜色表
        /// </summary>
        /// <returns></returns>
        public string GetMatexColorFile()
        {
            string DirPath = AppDir + @"Matex";
            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(DirPath);
            }
            string FilePath = DirPath + @"\Matex.color";
            return FilePath;
        }
        /// <summary>
        /// 获取线性表
        /// </summary>
        /// <returns></returns>
        public string GetMatexLineFile()
        {
            string DirPath = AppDir + @"Matex";
            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(DirPath);
            }
            string FilePath = DirPath + @"\Matex.line";
            return FilePath;
        }
        /// <summary>
        /// 获取版本插件配置文件
        /// </summary>
        /// <returns></returns>
        public string GetControlVersionsConfig()
        {
            return AppDir + @"CONFIG\CONTROLVERSION_MODULECONFIG.xml";
        }
        /// <summary>
        /// 获取设置的配置文件
        /// </summary>
        /// <returns></returns>
        public string GetDirection()
        {
            return AppDir + @"CONFIG\CONFIG.xml";
        }
        /// <summary>
        /// 获取树状配置文件
        /// </summary>
        /// <param name="CV"></param>
        /// <returns></returns>
        public string GetTreeConfig(ControlVersion CV)
        {
            switch (CV)
            {
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\Tree\CAR_CENTERCONTROLTREE_CONFIG.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\Tree\CAR_CENTERCONTROLTREE_CONFIG.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\Tree\CAR_CENTERCONTROLTREE_CONFIG.xml";
                default:
                    return AppDir + @"CONFIG\Tree\CAR_CENTERCONTROLTREE_CONFIG.xml";
            }
        }
        /// <summary>
        /// 获取树状配置文件
        /// </summary>
        /// <param name="CV"></param>
        /// <returns></returns>
        public string GetZDModeTreeConfig(ControlVersion CV)
        {
            switch (CV)
            {
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\Tree\CAR_ZDMODE_CENTERCONTROLTREE.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\Tree\CAR_ZDMODE_CENTERCONTROLTREE.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\Tree\CAR_ZDMODE_CENTERCONTROLTREE.xml";
                default:
                    return AppDir + @"CONFIG\Tree\CAR_CENTERCONTROLTREE_CONFIG.xml";
            }
        }
        /// <summary>
        /// 获取树状配置文件
        /// </summary>
        /// <param name="CV"></param>
        /// <returns></returns>
        public string GetHandRayModeTreeConfig(ControlVersion CV)
        {
            switch (CV)
            {
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\Tree\CAR_HANDCENTERCONTROLTREE.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\Tree\CAR_HANDCENTERCONTROLTREE.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\Tree\CAR_HANDCENTERCONTROLTREE.xml";
                default:
                    return AppDir + @"CONFIG\Tree\CAR_HANDCENTERCONTROLTREE.xml";
            }
        }
        /// <summary>
        /// 获取臂架展开动作条件树配置文件
        /// </summary>
        /// <param name="CV"></param>
        /// <returns></returns>
        public string GetHandOpenActionConditionTreeConfig(ControlVersion CV)
        {
            switch (CV)
            {
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\Tree\CAR_HAND_OPEN_CONDITIONTREE.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\Tree\CAR_HAND_OPEN_CONDITIONTREE.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\Tree\CAR_HAND_OPEN_CONDITIONTREE.xml";
                default:
                    return AppDir + @"CONFIG\Tree\CAR_HAND_OPEN_CONDITIONTREE.xml";
            }
        }
        /// <summary>
        /// 获取臂架展开动作条件树配置文件
        /// </summary>
        /// <param name="CV"></param>
        /// <returns></returns>
        public string GetTreeConfigForZD(ControlVersion CV)
        {
            switch (CV)
            {
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\Tree\CAR_CENTERCONTROLTREE_CONFIGForZD.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\Tree\CAR_CENTERCONTROLTREE_CONFIGForZD.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\Tree\CAR_CENTERCONTROLTREE_CONFIGForZD.xml";
                default:
                    return AppDir + @"CONFIG\Tree\CAR_CENTERCONTROLTREE_CONFIGForZD.xml";
            }
        }
        /// <summary>
        /// 获取臂架展开动作条件树配置文件
        /// </summary>
        /// <param name="CV"></param>
        /// <returns></returns>
        public string GetHandCloseActionConditionTreeConfig(ControlVersion CV)
        {
            switch (CV)
            {
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\Tree\CAR_HAND_CLOSE_CONDITIONTREE.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\Tree\CAR_HAND_CLOSE_CONDITIONTREE.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\Tree\CAR_HAND_CLOSE_CONDITIONTREE.xml";
                default:
                    return AppDir + @"CONFIG\Tree\CAR_HAND_CLOSE_CONDITIONTREE.xml";
            }
        }
        /// <summary>
        /// 获取主系统就绪树
        /// </summary>
        /// <param name="CV"></param>
        /// <returns></returns>
        public string GetMainReadyTree(ControlVersion CV)
        {
            switch (CV)
            {
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\Tree\CAR_MAINSYSTEMREADY.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\Tree\FASTCHECK_MAINSYSTEMREADY.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\Tree\CAR_MAINSYSTEMREADY.xml";
                case ControlVersion.PassengerCar:
                    return AppDir + @"CONFIG\Tree\PassengerCar_MainSystemready.xml";
                default:
                    return AppDir + @"CONFIG\Tree\CAR_MAINSYSTEMREADY.xml";
            }
        }
        /// <summary>
        /// 获取车体控制的配置文件
        /// </summary>
        /// <returns></returns>
        public string GetCarBodyDirection(ControlVersion CV)
        {
            switch (CV)
            {
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\CAR_BODY_CONFIG.xml";
                case ControlVersion.FastCheck:
                    return AppDir +@"CONFIG\CAR_BODY_CONFIG.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\SELFAUTOWORKING_BODY_CONFIG.xml";
                default:
                    return AppDir +@"CONFIG\CAR_BODY_CONFIG.xml";
            }
          
        }
        /// <summary>
        /// 获取命令地址的配置文件
        /// 车载
        /// </summary>
        /// <returns></returns>
        public string GetCommandConfig(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.BGV7000:
                    return AppDir + @"CONFIG\BGV7000\BGV7000_COMMAND_CONFIG.xml";
                case ControlVersion.BGV7700:
                    return AppDir + @"CONFIG\BGV7700\BGV7700_COMMAND_CONFIG.xml";
                case ControlVersion.BGV8000:
                    return AppDir + @"CONFIG\BGV8000\BGV8000_COMMAND_CONFIG.xml";
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\CAR_COMMAND_CONFIG.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\FASTCHECK_COMMAND_CONFIG.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\SELFAUTOWORKING_COMMAND_CONFIG.xml";
                case ControlVersion.CombinedMovement:
                    //组合移动
                    return AppDir + @"CONFIG\COMBINEDMOVEMENT_COMMAND_CONFIG.xml";
                case ControlVersion.CombinedMovementBetatron:
                    //组合移动
                    return AppDir + @"CONFIG\COMBINEDMOVEMENTBETRATRON_COMMAND_CONFIG.xml";
                case ControlVersion.PassengerCar:
                    //乘用车
                    return AppDir + @"CONFIG\PASSENGERCAR_COMMAND_CONFIG.xml";
                case ControlVersion.BS:
                    //背散设备
                    return AppDir + @"CONFIG\BS2000\BS2000_COMMAND_CONFIG.xml";
                case ControlVersion.BGV5100:
                    //绿通设备（锐新）
                    return AppDir + @"CONFIG\BGV5100\BGV5100_COMMAND_CONFIG.xml";
                case ControlVersion.BGV5100FH:
                    //绿通设备（泛华）
                    return AppDir + @"CONFIG\BGV5100FH\BGV5100FH_COMMAND_CONFIG.xml";
                //case ControlVersion.BGV6000BS:
                    //广西项目
                    //return AppDir + @"CONFIG\BGV6000BS\BGV6000BS_COMMAND_CONFIG.xml";
                default:
                    return AppDir + @"CONFIG\CAR_COMMAND_CONFIG.xml";
            }
        }

        /// <summary>
        /// 获取Flow配置文件
        /// </summary>
        /// <returns></returns>
        public string GetFlowConfig(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.BS:
                    return AppDir + @"CONFIG\BS2000\BS2000_FLOWCONTROL_CONFIG.xml";
                case ControlVersion.BGV7000:
                    return AppDir + @"CONFIG\BGV7000\BGV7000_FLOWCONTROL_CONFIG.xml";
                case ControlVersion.BGV7700:
                    return AppDir + @"CONFIG\BGV7700\BGV7700_FLOWCONTROL_CONFIG.xml";
                case ControlVersion.BGV8000:
                    return AppDir + @"CONFIG\BGV8000\BGV8000_FLOWCONTROL_CONFIG.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\FASTCHECK_FLOWCONTROL_CONFIG.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\SELFAUTOWORKING_FLOWCONTROL_CONFIG.xml";
                case ControlVersion.PassengerCar:
                    return AppDir + @"CONFIG\PassengerCar_FLOWCONTROL_CONFIG.xml";
                //case ControlVersion.BGV6000BS:
                    //return AppDir + @"CONFIG\BGV6000BS\BGV6000BS_FLOWCONTROL_CONFIG.xml";
                default:
                    return AppDir + @"CONFIG\FASTCHECK_FLOWCONTROL_CONFIG.xml";
            }
        }
        /// <summary>
        /// 获取Repair配置文件
        /// </summary>
        /// <returns></returns>
        public string GetRepairConfig(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.BGV7000:
                    return AppDir + @"CONFIG\BGV7000\BGV7000_REPAIR_CONFIG.xml";
                case ControlVersion.BGV7700:
                    return AppDir + @"CONFIG\BGV7700\BGV7700_REPAIR_CONFIG.xml";
                case ControlVersion.BGV8000:
                    return AppDir + @"CONFIG\BGV8000\BGV8000_REPAIR_CONFIG.xml";
                case ControlVersion.BS:
                    return AppDir + @"CONFIG\BS2000\BS2000_REPAIR_CONFIG.xml";
                //case ControlVersion.BGV6000BS:
                    //return AppDir + @"CONFIG\BGV6000BS\BGV6000BS_REPAIR_CONFIG.xml";
                default:
                    return AppDir + @"CONFIG\BGV7000_REPAIR_CONFIG.xml";
            }
        }



        /// <summary>
        /// 获取语言配置文件
        /// </summary>
        /// <returns></returns>
        public string GetLanguageConfig()
        {
            return AppDir + @"CONFIG\Language\Language.xml";
        }
        /// <summary>
        /// 获取字体配置文件
        /// </summary>
        /// <returns></returns>
        public string GetFontSizeConfig()
        {
            return AppDir + @"CONFIG\FontSize\Fontsize.xml";
        }
        /// <summary>
        /// 获取语言文件
        /// </summary>
        /// <returns></returns>
        public string GetLanguage()
        {
            return AppDir + @"CONFIG\Language\";
        }
        /// <summary>
        /// 获取所有图片
        /// </summary>
        /// <returns></returns>
        public string GetImage()
        {
            return AppDir + @"CONFIG\Image\";
        }
        /// <summary>
        /// 获取语言文件
        /// </summary>
        /// <returns></returns>
        public string GetFontsize()
        {
            return AppDir + @"CONFIG\FontSize\";
        }
        /// <summary>
        /// 获取臂架前进后退的配置文件
        /// 解释【但是暂时只有自行走的使用了这份配置文件】
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        public string GetHAND_CONFIG(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\SELFAUTOWORKING_HAND_CONFIG.xml";
                case ControlVersion.CombinedMovement:
                    return AppDir + @"CONFIG\COMBINEDMOVEMENT_HAND_CONFIG.xml";
                case ControlVersion.CombinedMovementBetatron:
                    return AppDir + @"CONFIG\COMBINEDMOVEMENT_BETRATRON_HAND_CONFIG.xml";
                case ControlVersion.PassengerCar:
                    return AppDir + @"CONFIG\PASSENGERCAR_HAND_CONFIG.xml";
                case ControlVersion.BGV7700:
                    return AppDir + @"CONFIG\BGV7700\BGV7700_HAND_CONFIG.xml";
                default:
                    return AppDir + @"CONFIG\SELFAUTOWORKING_HAND_CONFIG.xml";
            }
        }

        /// <summary>
        /// 获取臂架前进后退的配置文件
        /// 解释【但是暂时只有自行走的使用了这份配置文件】
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        public string GetComponentConfig(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\SELFAUTOWORKING_COMPONENT_HAND_CONFIG.xml";
                case ControlVersion.CombinedMovement:
                    return AppDir + @"CONFIG\COMBINEDMOVEMENT_COMPONENT_HAND_CONFIG.xml";
                case ControlVersion.CombinedMovementBetatron:
                    return AppDir + @"CONFIG\COMBINEDMOVEMENT_COMPONENT_HAND_CONFIG.xml";
                default:
                    return AppDir + @"CONFIG\SELFAUTOWORKING_COMPONENT_HAND_CONFIG.xml";
            }
        }
        /// <summary>
        /// 获取状态配置文件
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        public string GetHittingConfig(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.BGV7000:
                    return AppDir + @"CONFIG\BGV7000\BGV7000_HITCH_CONFIG.xml";
                case ControlVersion.BGV7700:
                    return AppDir + @"CONFIG\BGV7700\BGV7700_HITCH_CONFIG.xml";
                case ControlVersion.BGV8000:
                    return AppDir + @"CONFIG\BGV8000\BGV8000_HITCH_CONFIG.xml";
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\CAR_HITCH_CONFIG.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\FASTCHECK_HITCH_CONFIG.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\SEFTAUTOWORKING_HITCH_CONFIG.xml";
                case ControlVersion.CombinedMovement:
                    return AppDir + @"CONFIG\COMBINEDMOVEMENT_HITCH_CONFIG.xml";
                case ControlVersion.CombinedMovementBetatron:
                    return AppDir + @"CONFIG\COMBINEDMOVEMENTBETRATRON_HITCH_CONFIG.xml";
                case ControlVersion.PassengerCar:
                    return AppDir + @"CONFIG\PassengerCar.xml";
                case ControlVersion.BS:
                    return AppDir + @"CONFIG\BS2000\BS2000_HITCH_CONFIG.xml";
                case ControlVersion.BSTopCabin:
                    return AppDir + @"CONFIG\BS2000\TopCabin\BS2000_HITCH_CONFIG.xml";
                case ControlVersion.BGV5100:
                    return AppDir + @"CONFIG\BGV5100\BGV5100_HITCH_CONFIG.xml";
                case ControlVersion.BGV5100FH:
                    return AppDir + @"CONFIG\BGV5100FH\BGV5100FH_HITCH_CONFIG.xml";
                //case ControlVersion.BGV6000BS:
                    //return AppDir + @"CONFIG\BGV6000BS\BGV6000BS_HITCH_CONFIG.xml";
                default:
                    return AppDir + @"CONFIG\CAR_HITCH_CONFIG.xml";
            }
        }
        /// <summary>
        /// Betatron加速器状态配置
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        public string GetBetatronBoosting(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\SELFAUTO_BOOSETING_CONFIG.xml";
                case ControlVersion.CombinedMovementBetatron:
                    return AppDir + @"CONFIG\COMBINEDMOVEMENT_BETRATRONBOOSTING_STATUS_CONFIG.xml";
                default:
                    return AppDir + @"CONFIG\SELFAUTO_BOOSETING_CONFIG.xml";
            }
        }

        /// <summary>
        /// VJ射线源文件夹下所有配置文件
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        public string GetVJ_RaySourceDir(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.BS:
                    return AppDir + @"CONFIG\BS2000\BS2000RaySource";
                //case ControlVersion.BGV6000BS:
                    //return AppDir + @"CONFIG\BGV6000BS\BGV6000BSRaySource";
                default:
                    return AppDir + @"CONFIG\BS2000\BS2000RaySource";
            }
        }
        /// <summary>
        /// VJ射线源文件夹读取电压电流配置文件
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        public string GetVJ_RaySourceDoseValueDir(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.BS:
                    return AppDir + @"CONFIG\BS2000\BS2000RaySourceDoseValue";
                //case ControlVersion.BGV6000BS:
                    //return AppDir + @"CONFIG\BGV6000BS\BGV6000BSRaySourceDoseValue";
                default:
                    return AppDir + @"CONFIG\BS2000\BS2000RaySourceDoseValue";
            }
        }
        /// <summary>
        /// 获取加速器模块配置文件
        /// </summary>
        /// <returns></returns>
        public string GetBOOSTCONFIG(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.BGV7000:
                    return AppDir + @"CONFIG\BGV7000\BGV7000_BOOSTCONFIG.xml";
                case ControlVersion.BGV7700:
                    return AppDir + @"CONFIG\BGV7700\BGV7700_BOOSTCONFIG.xml";
                case ControlVersion.BGV8000:
                    return AppDir + @"CONFIG\BGV8000\BGV8000_BOOSTCONFIG.xml";
                case ControlVersion.BS:
                    return AppDir + @"CONFIG\BS2000\BS2000_BOOSTCONFIG.xml";
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\CARBOOSTCONFIG.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\FASTCHECKBOOSTCONFIG.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\SELFWORKBOOSTING.xml";
                case ControlVersion.CombinedMovement:
                    return AppDir + @"CONFIG\COMBUNEDMOVEMENTBOOSTCONFIG.xml";
                case ControlVersion.CombinedMovementBetatron:
                    return AppDir + @"CONFIG\COMBINEDMOVEMENT_BETRATRONBOOSTING_CONFIG.xml";
                //case ControlVersion.BGV5100Russia:
                //return AppDir + @"CONFIG\BGV5100\BGV5100Russia_BOOSTCONFIG.xml";
                case ControlVersion.BGV5100FH:
                    return AppDir + @"CONFIG\BGV5100FH\BGV5100FH_BOOSTCONFIG.xml";
                case ControlVersion.BGV5100:
                    return AppDir + @"CONFIG\BGV5100\BGV5100_BOOSTCONFIG.xml";
                //case ControlVersion.BGV6000BS:
                    //return AppDir + @"CONFIG\BGV6000BS\BGV6000BS_BOOSTCONFIG.xml";
                default:
                    return AppDir + @"CONFIG\FASTCHECKBOOSTCONFIG.xml";
            }
        }
        /// <summary>
        /// 获取Betratron_FHBoosting加速器的状态配置文件
        /// </summary>
        /// <returns></returns>
        public string GetBetratron_FHBoostingStatus(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.CombinedMovementBetatron:
                    return AppDir + @"CONFIG\COMBINEDMOVEMENTBETRATRON_STATUS_CONFIG.xml";
                default:
                    return AppDir + @"CONFIG\COMBINEDMOVEMENTBETRATRON_STATUS_CONFIG.xml";
            }
        }
 
        /// <summary>
        /// 获取车载剂量的配置文件
        /// </summary>
        /// <returns></returns>
        public string GetDoseConfig(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.BGV7000:
                    return AppDir + @"CONFIG\BGV7000\BGV7000_DOSE_CONFIG.xml";
                case ControlVersion.BGV7700:
                    return AppDir + @"CONFIG\BGV7700\BGV7700_DOSE_CONFIG.xml";
                case ControlVersion.BGV8000:
                    return AppDir + @"CONFIG\BGV8000\BGV8000_DOSE_CONFIG.xml";
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\CAR_DOSE_CONFIG.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\FASTCHECK_DOSE_CONFIG.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\SELFAUTO_DOSE_CONFIG.xml";
                case ControlVersion.CombinedMovement:
                    return AppDir + @"CONFIG\COMBINEDMOVEMENT_DOSE_CONFIG.xml";
                case ControlVersion.CombinedMovementBetatron:
                    return AppDir + @"CONFIG\COMBINEDMOVEMENTBETRATRON_DOSE_CONFIG.xml";
                case ControlVersion.PassengerCar:
                    return AppDir + @"CONFIG\PASSENGERCAR_DOSE_CONFIG.xml";
                case ControlVersion.BS:
                    return AppDir + @"CONFIG\BS2000\BS2000_DOSE_CONFIG.xml";
                case ControlVersion.BGV5100:
                    return AppDir + @"CONFIG\BGV5100\BGV5100_DOSE_CONFIG.xml";
                //case ControlVersion.BGV5100Russia:
                //return AppDir + @"CONFIG\BGV5100Russia\BGV5100Russia_DOSE_CONFIG.xml";
                case ControlVersion.BGV5100FH:
                    return AppDir + @"CONFIG\BGV5100FH\BGV5100FH_DOSE_CONFIG.xml";
                //case ControlVersion.BGV6000BS:
                    //return AppDir + @"CONFIG\BGV6000BS\BGV6000BS_DOSE_CONFIG.xml";
                default:
                    return AppDir + @"CONFIG\CAR_DOSE_CONFIG.xml";
            }
        }
        /// <summary>
        /// 获取登录的角色配置文件
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        public string GetRoleConfig(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.BS:
                    return AppDir + @"CONFIG\BS2000\Role\Role\ROLECONFIG.xml";
                case ControlVersion.BGV7000:
                    return AppDir + @"CONFIG\BGV7000\Role\Role\ROLECONFIG.xml";
                case ControlVersion.BGV7700:
                    return AppDir + @"CONFIG\BGV7700\Role\Role\ROLECONFIG.xml";
                case ControlVersion.BGV8000:
                    return AppDir + @"CONFIG\BGV8000\Role\Role\ROLECONFIG.xml";
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\Car\Role\ROLECONFIG.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\FastCheck\Role\ROLECONFIG.xml";
                case ControlVersion.SelfWorking:
                    return AppDir + @"CONFIG\SelfWork\Role\ROLECONFIG.xml";
                case ControlVersion.CombinedMovement:
                case ControlVersion.CombinedMovementBetatron:
                    return AppDir + @"CONFIG\CombinedMovement\Role\ROLECONFIG.xml";
                case ControlVersion.PassengerCar:
                    return AppDir + @"CONFIG\FastCheck\Role\ROLECONFIG.xml";
                case ControlVersion.BGV5100:
                    return AppDir + @"CONFIG\BGV5100\Role\Role\ROLECONFIG.xml";
                case ControlVersion.BGV5100FH:
                    return AppDir + @"CONFIG\BGV5100FH\Role\Role\ROLECONFIG.xml";
                //case ControlVersion.BGV6000BS:
                    //return AppDir + @"CONFIG\BGV6000BS\Role\Role\ROLECONFIG.xml";
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// 获取用户配置文件
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        public string GetUserConfig(ControlVersion cv)
        {
            switch (cv)
            {
                case ControlVersion.Car:
                    return AppDir + @"CONFIG\Car\User\USER_CONFIG.xml";
                case ControlVersion.FastCheck:
                    return AppDir + @"CONFIG\FastCheck\User\USER_CONFIG.xml";
                default:
                    return string.Empty;
            }
        }
        public string GetUserConfig()
        {
            return AppDir + @"CONFIG\USER\UserConfig.xml";
        }
        /// <summary>
        /// 获取车载车体配置文件
        /// </summary>
        /// <returns></returns>
        public string GetCarCantilever(ControlVersion cv = ControlVersion.Car)
        {
            return AppDir + @"CONFIG\CAR_CONFIG.xml";
        }

        /// <summary>
        /// 获取警报铃声
        /// </summary>
        /// <returns></returns>
        public string GetAlarmMusic()
        {
            return AppDir + @"ALARM1.WAV";
        }
        /// <summary>
        /// 获取本地功能性插件路径
        /// </summary>
        /// <returns></returns>
        public string GetPlugins()
        {
            return AppDir + @"\Plugins";
        }

        /// <summary>
        /// 获取O点配置文件路径
        /// </summary>
        /// <returns></returns>
        public string GetOPosition(ControlVersion cv = ControlVersion.BGV7000)
        {
            switch (cv)
            {
                case ControlVersion.BGV7000:
                    return AppDir + @"CONFIG\BGV7000\IO\OPositionConfig.xml";
                case ControlVersion.BGV7700:
                    return AppDir + @"CONFIG\BGV7700\IO\OPositionConfig.xml";
                case ControlVersion.BGV8000:
                    return AppDir + @"CONFIG\BGV8000\IO\OPositionConfig.xml";
                case ControlVersion.Car:
                    break;
                case ControlVersion.FastCheck:
                    break;
                case ControlVersion.SelfWorking:
                    break;
                case ControlVersion.CombinedMovement:
                    break;
                case ControlVersion.CombinedMovementBetatron:
                    break;
                case ControlVersion.PassengerCar:
                    break;
                default:
                    return AppDir + @"CONFIG\BGV7000\IO\OPositionConfig.xml";
            }
            return AppDir + @"CONFIG\BGV7000\IO\OPositionConfig.xml";
        }

        /// <summary>
        /// 获取I点配置文件路径
        /// </summary>
        /// <returns></returns>
        public string GetIPosition(ControlVersion cv = ControlVersion.BGV7000)
        {
            switch (cv)
            {
                case ControlVersion.BGV7000:
                    return AppDir + @"CONFIG\BGV7000\IO\IPositionConfig.xml";
                case ControlVersion.BGV7700:
                    return AppDir + @"CONFIG\BGV7700\IO\IPositionConfig.xml";
                case ControlVersion.Car:
                    break;
                case ControlVersion.BS:
                    return AppDir + @"CONFIG\BS2000\IO\IPositionConfig.xml";
                case ControlVersion.FastCheck:
                    break;
                case ControlVersion.SelfWorking:
                    break;
                case ControlVersion.CombinedMovement:
                    break;
                case ControlVersion.CombinedMovementBetatron:
                    break;
                case ControlVersion.PassengerCar:
                    break;
                //case ControlVersion.BGV6000BS:
                    //return AppDir + @"CONFIG\BGV6000BS\IO\IPositionConfig.xml";
                default:
                    return AppDir + @"CONFIG\BGV7000\IO\IPositionConfig.xml";
            }
            return AppDir + @"CONFIG\BGV7000\IO\IPositionConfig.xml";
        }

        /// <summary>
        /// 获取DInt配置文件路径
        /// </summary>
        /// <returns></returns>
        public string GetDIntPosition(ControlVersion cv = ControlVersion.BGV7000)
        {
            switch (cv)
            {
                case ControlVersion.BGV7000:
                    return AppDir + @"CONFIG\BGV7000\IO\DIntPositionConfig.xml";
                case ControlVersion.BGV7700:
                    return AppDir + @"CONFIG\BGV7700\IO\DIntPositionConfig.xml";
                case ControlVersion.BS:
                    return AppDir + @"CONFIG\BS2000\IO\DIntPositionConfig.xml";
                case ControlVersion.Car:
                    break;
                case ControlVersion.FastCheck:
                    break;
                case ControlVersion.SelfWorking:
                    break;
                case ControlVersion.CombinedMovement:
                    break;
                case ControlVersion.CombinedMovementBetatron:
                    break;
                case ControlVersion.PassengerCar:
                    break;
                //case ControlVersion.BGV6000BS:
                    //return AppDir + @"CONFIG\BGV6000BS\IO\DIntPositionConfig.xml";
                default:
                    return AppDir + @"CONFIG\BGV7000\IO\DIntPositionConfig.xml";
            }
            return AppDir + @"CONFIG\BGV7000\IO\DIntPositionConfig.xml";
        }
        /// <summary>
        /// 获取Int配置文件路径
        /// </summary>
        /// <returns></returns>
        public string GetIntPosition(ControlVersion cv = ControlVersion.BGV7000)
        {
            switch (cv)
            {
                case ControlVersion.BGV7000:
                    return AppDir + @"CONFIG\BGV7000\IO\IntPositionConfig.xml";
                case ControlVersion.BGV7700:
                    return AppDir + @"CONFIG\BGV7700\IO\IntPositionConfig.xml";
                case ControlVersion.BS:
                    return AppDir + @"CONFIG\BS2000\IO\IntPositionConfig.xml";
                case ControlVersion.Car: 
                    break;
                case ControlVersion.FastCheck:
                    break;
                case ControlVersion.SelfWorking:
                    break;
                case ControlVersion.CombinedMovement:
                    break;
                case ControlVersion.CombinedMovementBetatron:
                    break;
                case ControlVersion.PassengerCar:
                    break;
                //case ControlVersion.BGV6000BS:
                    //return AppDir + @"CONFIG\BGV6000BS\IO\IntPositionConfig.xml";
                default:
                    return AppDir + @"CONFIG\BGV7000\IO\IntPositionConfig.xml";
            }
            return AppDir + @"CONFIG\BGV7000\IO\IntPositionConfig.xml";
        }
        /// <summary>
        /// 获取Float配置文件路径
        /// </summary>
        /// <returns></returns>
        public string GetFloatPosition(ControlVersion cv = ControlVersion.BGV7000)
        {
            switch (cv)
            {
                case ControlVersion.BGV7000:
                    return AppDir + @"CONFIG\BGV7000\IO\FloatPositionConfig.xml";
                case ControlVersion.BGV7700:
                    return AppDir + @"CONFIG\BGV7700\IO\FloatPositionConfig.xml";
                case ControlVersion.BS:
                    return AppDir + @"CONFIG\BS2000\IO\FloatPositionConfig.xml";
                case ControlVersion.Car:
                    break;
                case ControlVersion.FastCheck:
                    break;
                case ControlVersion.SelfWorking:
                    break;
                case ControlVersion.CombinedMovement:
                    break;
                case ControlVersion.CombinedMovementBetatron:
                    break;
                case ControlVersion.PassengerCar:
                    break;
                //case ControlVersion.BGV6000BS:
                    //return AppDir + @"CONFIG\BGV6000BS\IO\FloatPositionConfig.xml";
                default:
                    return AppDir + @"CONFIG\BGV7000\IO\FloatPositionConfig.xml";
            }
            return AppDir + @"CONFIG\BGV7000\IO\FloatPositionConfig.xml";
        }


        public String GetModuleMainPage(ControlVersion control = ControlVersion.FastCheck)
        {
            switch (control)
            {
                case ControlVersion.BGV7000:
                    return AppDir + $@"Config\Image\BGV7000\MainCar.png";
                case ControlVersion.BGV7700:
                    return AppDir + $@"Config\Image\BGV7700\MainCar.jpg";
                case ControlVersion.BGV8000:
                    return AppDir + $@"Config\Image\BGV8000\MainCar.png";
                case ControlVersion.Car:
                    return AppDir + $@"Config\Image\BGV7000\MainCar.png";
                case ControlVersion.FastCheck:
                    return AppDir + $@"Config\Image\BGV6000\MainCar.png";
                case ControlVersion.SelfWorking:
                    return AppDir + $@"Config\Image\BGV7600\MainCar.png";
                case ControlVersion.CombinedMovement:
                case ControlVersion.CombinedMovementBetatron:
                    return AppDir + $@"Config\Image\BGV6100\MainCar.jpg";
                case ControlVersion.PassengerCar:
                    return AppDir + $@"Config\Image\BGV3200\MainCar.jpg";
                case ControlVersion.BS:
                    return AppDir + $@"Config\Image\BS2030\MainCar.png";
                case ControlVersion.BGV5100: 
                    return AppDir + $@"Config\Image\BGV5100\MainCar.png";
                //case ControlVersion.BGV5100Russia:
                //return AppDir + $@"Config\Image\BGV5100Russia\MainCar.png";
                case ControlVersion.BGV5100FH:
                    return AppDir + $@"Config\Image\BGV5100FH\MainCar.png";
                default:
                    return String.Empty;
            }
        }

        public String GetModuleSmallRayPage(ControlVersion control = ControlVersion.FastCheck)
        {
            switch (control)
            {
                case ControlVersion.FastCheck:
                    return AppDir + $@"Config\Image\BGV6000\MainCar.png";
                case ControlVersion.BS:
                    return AppDir + $@"Config\Image\BS2000\MainCar.png";
                case ControlVersion.CombinedMovement:
                    return AppDir + $@"Config\Image\BGV6100\MainCar.png";
                case ControlVersion.BGV5100:
                    return AppDir + $@"Config\Image\BGV5100\MainCar.png";
                //case ControlVersion.BGV5100Russia:
                //return AppDir + $@"Config\Image\BGV5100Russia\MainCar.png";
                case ControlVersion.BGV5100FH:
                    return AppDir + $@"Config\Image\BGV5100FH\MainCar.png";
                //case ControlVersion.BGV6000BS:
                    //return AppDir + $@"Config\Image\BGV6000BS\MainCar.png";
                default:
                    return String.Empty;
            }
        }
    }
}
