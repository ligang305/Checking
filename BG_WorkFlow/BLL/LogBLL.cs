using BGDAL;
using BGModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_WorkFlow
{
    public class LogBLL
    {
        private LogDal logDal;

        public LogBLL()
        {
            logDal = new LogDal();
            //CommonDeleget.SearchStringDataEvent += SelectAll;
            //CommonDeleget.DeleteDataEvent += Delete;
        }

        public ObservableCollection<BG_Logs> GetLogs(ParamaterModel<BG_Logs> LogCondition)
        {
            ObservableCollection<BG_Logs> obList = new ObservableCollection<BG_Logs>();
            logDal.GetBGLogs(LogCondition).ToList().ForEach(q => obList.Add(q));
            return obList;
        }

        public int GetAllConut(BG_Logs bGLogs)
        {
            return logDal.GetAllConut(bGLogs);
        }


        public bool Insert(BG_Logs localUploadImageObject)
        {
            return logDal.InsertLog(localUploadImageObject);
        }

        public bool Insert(List<BG_Logs> localUploadImageObjectList)
        {
            bool isSuccess = false;
            foreach (var localUploadImageObject in localUploadImageObjectList)
            {
                isSuccess = logDal.InsertLog(localUploadImageObject);
            }
            return isSuccess;
        }

        public bool Delete(BG_Logs logObject)
        {
            return logDal.DeleteLog(logObject);
        }
        public bool Delete()
        {
            return logDal.DeleteLog(null);
        }
    }
}
