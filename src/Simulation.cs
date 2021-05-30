using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;

namespace paczkomaty
{
    class Simulation
    {
        Canvas m_Context;
        List<ParcelLocker> m_ParcelLockers = null;
        //Thread m_Thread;

        public Simulation(Canvas context)
        {
            m_Context = context;
            InitResources();
            InitParcelLockers();
               
        }

        private void InitResources()
        {
            Resources.AddUri(UriType.PARCELLOCKER, new Uri("/Resources/parcelLocker.png",UriKind.Relative));
            Resources.AddUri(UriType.CELL, new Uri("/Resources/cellfree.png", UriKind.Relative));
            Resources.AddUri(UriType.CELL, new Uri("/Resources/celltaken.png", UriKind.Relative));
        }

        private void InitParcelLockers()
        {
            m_ParcelLockers = new List<ParcelLocker>();
            m_ParcelLockers.Add(new ParcelLocker(new Coord(0,0),m_Context));
            m_ParcelLockers.Add(new ParcelLocker(new Coord(400,0),m_Context));
            m_ParcelLockers.Add(new ParcelLocker(new Coord(800,0),m_Context));
        }
    }
}
