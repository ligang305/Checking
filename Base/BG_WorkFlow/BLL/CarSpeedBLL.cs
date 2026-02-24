using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BG_Entities;
using BGDAL;
using BGModel;
using CMW.Common.Utilities;

namespace BG_WorkFlow
{
    public class CarSpeedBLL : BaseInstance<CarSpeedBLL>
    {

        private List<Bg_Carspeed> carSpeedList = new List<Bg_Carspeed>();
        private CarSpeedFreezeDal carSpeedFreezeDal;

        public CarSpeedBLL()
        {
            carSpeedFreezeDal = new CarSpeedFreezeDal();
            carSpeedList = GetCarspeedModel();
        }

        public List<Bg_Carspeed> GetCarSpeedList()
        {
            carSpeedList = GetCarspeedModel();
            return carSpeedList;
        }


        /// <summary>
        /// 通过当前车速获取需要设置的频率值
        /// </summary>
        /// <returns></returns>
        public int GetFreez(ushort CarSpeed)
        {
            int DefaultValue  = 200;
            decimal CarSpeedValue = CarSpeed / 100.00M;
            carSpeedList = GetCarSpeedList().Where(q=>q.BayNumber == ConfigServices.GetInstance().localConfigModel.CMW_FastCheckBayNumber).ToList();
            bool isHasFreez = false;
            foreach (var csItem in carSpeedList)
            {
                decimal Min = Convert.ToDecimal(csItem.SpeedMin);
                decimal Max = Convert.ToDecimal(csItem.SpeedMax);
                if (CarSpeedValue>= Max || CarSpeedValue< Min) continue;
                isHasFreez = true;
                return Convert.ToInt32(Math.Truncate(Convert.ToDecimal(csItem.Freez)));
            }

            if (!isHasFreez)
            {
                DefaultValue = 400;
            }
            return DefaultValue;
        }

        /// <summary>
        /// 通过当前车速获取需要设置的频率值
        /// </summary>
        /// <returns></returns>
        public int GetFreezUseExpression(ushort CarSpeed)
        {
            int DefaultValue = 200;
            decimal CarSpeedValue = CarSpeed / 100.00M;
            string tempBarNum = ConfigServices.GetInstance().localConfigModel.CMW_FastCheckBayNumber;
            int BayNumber = Convert.ToInt32(tempBarNum);
            DefaultValue = Convert.ToInt32(Math.Truncate(
                            ((5000 * CarSpeedValue) 
                 / (9 * ConfigServices.GetInstance().localConfigModel.VerticalSpatialResolution * BayNumber)) 
                 - ConfigServices.GetInstance().localConfigModel.AdjustableParameters));
        
            return DefaultValue;
        }


        /// <summary>
        /// 获取默认车速
        /// </summary>
        /// <returns></returns>
        public int GetCarSpeedUseExpression(ushort CarSpeed)
        {
            int DefaultValue = 200;
            decimal CarSpeedValue = CarSpeed / 100.00M;
            int BayNumber = Convert.ToInt32(ConfigServices.GetInstance().localConfigModel.CMW_FastCheckBayNumber);
            DefaultValue = Convert.ToInt32(Math.Truncate(
                            ((5000 * CarSpeedValue)
                 / (9 * ConfigServices.GetInstance().localConfigModel.VerticalSpatialResolution * BayNumber))
                 - ConfigServices.GetInstance().localConfigModel.AdjustableParameters));

            return DefaultValue;
        }
        protected List<Bg_Carspeed> GetCarspeedModel()
        {
            //将xml路径先创立 创建节点
            return carSpeedFreezeDal.GetBg_CarspeedFromDataBase();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="_TCarspeedModel"></param>
        /// <param name="cv"></param>
        /// <returns></returns>
        public bool DeleteCarSpeedModel(Bg_Carspeed _TCarspeedModel, ControlVersion cv)
        {
            return carSpeedFreezeDal.DeleteCarSpeed(_TCarspeedModel);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="_TCarspeedModel"></param>
        /// <param name="cv"></param>
        /// <returns></returns>
        public bool AddCarSpeedModel(Bg_Carspeed _TCarspeedModel)
        {
            return carSpeedFreezeDal.InsertCarSpeed(_TCarspeedModel);
        }
        protected bool SaveCarspeedModel(Bg_Carspeed _TCarspeedModel, ControlVersion cv)
        {
            return carSpeedFreezeDal.UpdateCarSpeed(_TCarspeedModel);
        }

        public bool SaveDoseModel(IEnumerable<Bg_Carspeed> _Tarspeeds, ControlVersion cv)
        {
            bool isSuccess = false;
            foreach (var item in _Tarspeeds)
            {
                isSuccess = SaveCarspeedModel(item, cv);
                if (!isSuccess) break;
            }
            return isSuccess;
        }

    }
}
