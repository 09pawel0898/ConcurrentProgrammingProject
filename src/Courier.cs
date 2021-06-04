using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelLockers
{
    class Courier : Human
    {
        public Courier(Canvas context)
        {
            m_Animator = new Animator(2, Resources.Instance.People);
            m_Context = context;

            SharedResources.SafeSharedResourceOperation.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                Img = new Image
                {
                    Width = 80,
                    Height = 200,
                    Name = "P" + m_Id,
                    Source = new BitmapImage(Resources.Instance.People[4])
                };
                m_Context.Children.Add(Img);
                Canvas.SetLeft(Img, Defines.sidewalkSpawnPos.x);
                Canvas.SetTop(Img, Defines.sidewalkSpawnPos.y);
            }));
            SharedResources.SafeSharedResourceOperation.ReleaseMutex();

            m_currentPos = new Coord(Defines.sidewalkSpawnPos.x, Defines.sidewalkSpawnPos.y);
            m_Thread = new Thread(Simulate);
            m_Thread.Start();
        }

        private void Simulate()
        {
            Random rand = new Random();
            PersonAction action;

            while (true)
            {
                // Decide what to do in the current loop
                int randomNum = rand.Next(0, 100);
                action = (randomNum < 100) ? PersonAction.SENDING : PersonAction.PICKINGUP;

                // two action types (sending or picking up a parcel)
                switch (action)
                {
                    case PersonAction.SENDING: SimulateSendingParcel(); break;
                    case PersonAction.PICKINGUP: SimulatePickingUpParcel(); break;
                }
                FadeOut();
                // Sleep for some time
                Thread.Sleep(rand.Next(1000, 4000));
            }
        }
    }
}
