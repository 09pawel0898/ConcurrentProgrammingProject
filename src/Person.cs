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
    class Person
    {
        private static int ID = 0;
        private Thread m_personThread;
        private Canvas m_Context;
        private Animator m_Animator;
        private int m_Id;
        public Image Img { get; set; }

        public Person(Canvas context)
        {
            m_Id = ID++;
            m_Context = context;
            m_Animator = new Animator(2, Resources.Instance.People);

            Img = new Image
            {
                Width = 90,
                Height = 200,
                Name = "P"+m_Id,
                Source = new BitmapImage(Resources.Instance.People[0])
            };

            m_Context.Children.Add(Img);
            Canvas.SetTop(Img, 360);
            Canvas.SetLeft(Img, 185 );
            m_personThread = new Thread(Simulate);
            m_personThread.Start();
        }

        private void Simulate()
        {
            while(true)
            {
                Thread.Sleep(200);

                SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Img.Source = m_Animator.updateImg();
                }));
                
            }
        }
    }
}
