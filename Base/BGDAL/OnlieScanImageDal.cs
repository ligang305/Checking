using BGModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BGDAL
{
    public class OnlieScanImageDal :RepositoryFactory<LocalUploadImage>
    {
        public OnlieScanImageDal() 
        {
            if(!base.BaseRepository().IsExistIndex<LocalUploadImage>("ImageIndex"))
            base.BaseRepository().CreateIndex<LocalUploadImage>("ImageIndex", "BG_IMAGE", "IMAGE_ID");
        }
        public List<LocalUploadImage> GetLocalUploadImageListFromDataBase(int count,int startIndex)
        {
            try
            {
                string sqlStr = $@"{BaseRepository().SelectAll<LocalUploadImage>()} Order by IMAGESOURCE_NAME desc limit {count} offset {startIndex}";

                return DataTableToObject<LocalUploadImage>(BaseRepository().QueryDataTable(sqlStr));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public int GetAllConut()
        {
            try
            {
                string sqlStr = BaseRepository().SelectAll<LocalUploadImage>() ;

                return BaseRepository().QueryDataTable(sqlStr).Rows.Count;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public int GetAllConut(LocalUploadImage localUploadImage)
        {
            try
            {
                string sqlStr = BaseRepository().SelectAllAccordSearchCondition<LocalUploadImage>(localUploadImage);

                return Convert.ToInt32(BaseRepository().QueryDataTable(sqlStr).Rows[0][0]);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public LocalUploadImage IsExist(LocalUploadImage localUploadImage)
        {
            try
            {
                string sqlStr = string.Format($@"{BaseRepository().SelectAll<LocalUploadImage>()} where Image_Name = '{localUploadImage.IMAGE_NAME}' ");
                return DataTableToObject<LocalUploadImage>(BaseRepository().QueryDataTable(sqlStr)).FirstOrDefault();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public LocalUploadImage IsExist(string localUploadImageFileName)
        {
            try
            {
                string sqlStr = string.Format($@"{BaseRepository().SelectAll<LocalUploadImage>()} where Image_Name = '{localUploadImageFileName}' ");
                var dt = BaseRepository().QueryDataTable(sqlStr);
                return DataTableToObject<LocalUploadImage>(dt).FirstOrDefault();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<LocalUploadImage> GetLocalUploadImageList(ParamaterModel<LocalUploadImage> pageModel)
        {
            //string sqlStr = string.Format("select * from BG_IMAGE ");
            //var dt = base.QueryDataTable(sqlStr);
            //var DataBaseList = CommonFunc.DataTableToObject<LocalUploadImage>(dt);

            if (pageModel == null || pageModel.Model == null)
            {
                pageModel = new ParamaterModel<LocalUploadImage>();
                pageModel.num = 5;
                pageModel.start = 1;
            }
            List<LocalUploadImage> ImageList = GetLocalUploadImageListFromDataBase(pageModel.num,pageModel.start);
            //如果List的数量少于页码起始页
            //分页可能会有问题
            //TotalCount = GetAllConut();
            //if (ImageList.Count < pageModel.start * pageModel.num)
            //{
            //    ImageList = ImageList.Skip((pageModel.start - 1) * pageModel.num).ToList();
            //}
            //else
            //{
            //    ImageList = ImageList.Skip((pageModel.start - 1) * pageModel.num).Take(pageModel.num).ToList();
            //}

            return ImageList;
        }
        public bool InsertLocalUploadImage(LocalUploadImage LocalUploadImage)
        {
            string SqlStr = BaseRepository().InsertSpliteSqlByObject<LocalUploadImage>(LocalUploadImage);
            return BaseRepository().ExecuteNonQuerySQL(SqlStr) > 0;
        }
        public bool DeleteLocalUploadImage(LocalUploadImage LocalUploadImage = null)
        {
            string SqlStr = $@"delete from BG_IMAGE ";
            if(LocalUploadImage != null)
            {
                SqlStr += $@" where Image_Name = '{LocalUploadImage.IMAGE_NAME}' ";
            }
            return BaseRepository().ExecuteNonQuerySQL(SqlStr) > 0;
        }
        public bool UpdateLocalUploadImage(LocalUploadImage localUploadImage)
        {
            string sqlStr = BaseRepository().UpdateSqlByObject<LocalUploadImage>(localUploadImage);
            return BaseRepository().ExecuteNonQuerySQL(sqlStr) > 0;
        }
    }
}
