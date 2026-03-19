using BGDAL;
using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_WorkFlow
{
    public class LocalUploadImageBLL
    {
        private OnlieScanImageDal onlieScanImageDal;
        public Action InsertImageEvent;
        public LocalUploadImageBLL()
        {
            onlieScanImageDal = new OnlieScanImageDal();
            CommonDeleget.SearchStringDataEvent += SelectAll;
            CommonDeleget.DeleteDataEvent += Delete;
        }
        public ObservableCollection<LocalUploadImage> GetLocalUpLoadImage(ParamaterModel<LocalUploadImage> LocalUploadImageCondition)
        {
            ObservableCollection<LocalUploadImage> obList = new ObservableCollection<LocalUploadImage>();
            onlieScanImageDal.GetLocalUploadImageList(LocalUploadImageCondition).ToList().ForEach(q => obList.Add(q));
            return obList;
        }
        public int GetAllConut(LocalUploadImage localUploadImage)
        {
            return onlieScanImageDal.GetAllConut(localUploadImage);
        }
        public string SelectAll()
        {
            return onlieScanImageDal.GetAllConut().ToString();
        }
        public bool Insert(LocalUploadImage localUploadImageObject)
        {
            bool InsertResult = onlieScanImageDal.InsertLocalUploadImage(localUploadImageObject);
            if(InsertResult) InsertImageEvent?.Invoke();
            return InsertResult;
        }
        public bool Insert(List<LocalUploadImage> localUploadImageObjectList)
        {
            bool isSuccess = false;
            foreach (var localUploadImageObject in localUploadImageObjectList)
            {
                isSuccess =  onlieScanImageDal.InsertLocalUploadImage(localUploadImageObject);
            }
            return isSuccess;
        }
        public bool Update(LocalUploadImage localUploadImageObject)
        {
            return onlieScanImageDal.UpdateLocalUploadImage(localUploadImageObject);
        }
        public bool Delete(LocalUploadImage localUploadImageObject)
        {
            return onlieScanImageDal.DeleteLocalUploadImage(localUploadImageObject);
        }
        public bool Delete()
        {
            return onlieScanImageDal.DeleteLocalUploadImage(null);
        }
    }
}
