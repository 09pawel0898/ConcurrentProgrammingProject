using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace ParcelLockers
{
    class SharedResources
    {
        private static Window m_Window;
        public static Window Window { get { return m_Window; } set { m_Window = value; } }
    }
}
