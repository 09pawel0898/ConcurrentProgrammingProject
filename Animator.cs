using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;

namespace ParcelLockers
{
    class Animator
    {
        private static int DT;
        private int m_elapsedTime;
        private int m_currentFrameId = 0;
        private int m_numFrames;
        private List<Uri> m_imageSources;
        public Animator(int numFrames,List<Uri> imageUris)
        {
            m_numFrames = numFrames;
            m_imageSources = new List<Uri>(numFrames);
            m_imageSources = imageUris;
        }

        public BitmapImage updateImg()
        {
            
            if (m_currentFrameId > m_numFrames-1)
                m_currentFrameId = 0;

            BitmapImage newFrameImage = new BitmapImage(m_imageSources[m_currentFrameId]);
            m_currentFrameId++; 
            return newFrameImage;
        }
    }
}
