using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelLockers
{
    enum UriType
    {
        CELL,
        PARCELLOCKER,
        PERSON,
        COURIER,
        COURIERCAR
    }

    class Resources
    {
        private static Resources instance = new Resources();
        private static List<Uri>  parcelLockerUris = new List<Uri>();
        private static List<Uri>[]  peopleUris = new List<Uri>[6];
        private static List<Uri>  courierUris = new List<Uri>();
        private static List<Uri>  cellsUris = new List<Uri>();
        private static List<Uri> courierCarUris = new List<Uri>();
        private Resources() {}

        public static Resources Instance
        {
            get { return Resources.instance;  }
        }

        public List<Uri> ParcelLockers { get { return parcelLockerUris; } }
        public List<Uri>[] People { get { return peopleUris; } }
        public List<Uri> Couriers { get { return courierUris; } }
        public List<Uri> Cells { get { return cellsUris; } }
        public List<Uri> CourierCars { get { return courierCarUris; } }

        public static void AddUri(UriType uriType, Uri uri, int personId)
        {
            switch(uriType)
            {
                case UriType.CELL:
                    cellsUris.Add(uri);
                    break;
                case UriType.PARCELLOCKER:
                    parcelLockerUris.Add(uri);
                    break;
                case UriType.PERSON:
                    if (peopleUris[personId] == null)
                        peopleUris[personId] = new List<Uri>();
                    peopleUris[personId].Add(uri);
                    break;
                case UriType.COURIER:
                    courierUris.Add(uri);
                    break;
                case UriType.COURIERCAR:
                    courierCarUris.Add(uri);
                    break;
            }
        }

    }
}
