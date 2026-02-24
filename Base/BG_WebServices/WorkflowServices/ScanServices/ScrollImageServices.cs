using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG_Services
{
    public class ScrollImageServices
    {
        public Action<bool> ScrollImageEvent;
        public Action ReflashPreviewImageAction;
        public bool IsScrollImage;
        /// <summary>
        /// 单实例服务
        /// </summary>
        public static ScrollImageServices Service { get; private set; }

        static ScrollImageServices()
        {
            Service = new ScrollImageServices();
        }

        public void Start()
        {
            IsScrollImage = true;
            ScrollImageEvent?.Invoke(true);
        
            DetectorService.GetInstance().ReflashPreviewImage += ReflashPreviewImageEvent;
        }

        public void Stop()
        {
            IsScrollImage = false;
            ScrollImageEvent?.Invoke(false);
            DetectorService.GetInstance().ReflashPreviewImage -= ReflashPreviewImageEvent;
        }
        public void ReflashPreviewImageEvent()
        {
            ReflashPreviewImageAction?.Invoke();
        }
    }
}
