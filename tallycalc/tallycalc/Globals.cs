using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tallycalc
{
    public static class Globals
    {
        #region Variable Declarations
        private static TallyGroup _currentTally;
        private static List<TallyGroup> _currentTallies;
        private static TallyItem _currentTallyItem;
        #endregion
        #region Variable Definitions

        public static TallyGroup CurrentTally
        {
            get { return _currentTally; }
            set
            {
                _currentTally = value;
            }
        }

        public static List<TallyGroup> CurrentTallies
        {
            get { return _currentTallies; }
            set{ _currentTallies = value; }
        }

        public static TallyItem CurrentTallyItem
        {
            get { return _currentTallyItem; }
            set
            {
                _currentTallyItem = value;
            }
        }
        #endregion
        #region Isolate Storage Handling
        public static void SaveStorageData()
        {
            using (var filesystem = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var fs = new IsolatedStorageFileStream(
                  "tallies.dat", FileMode.Create, filesystem))
                {
                    var serializer = new System.Runtime.Serialization
                      .Json.DataContractJsonSerializer(typeof(List<TallyGroup>));
                    serializer.WriteObject(fs, _currentTallies);
                }
            }
        }
        #endregion
        #region Numeral Handlers
        public static int GetNumeralIndexByName(string a)
        {
            foreach (TallyGroup item in _currentTallies)
            {
                if (item.name.CompareTo(a) == 0)
                    return _currentTallies.IndexOf(item);
            }
            return -1;
        }

        public static int GetNumeralItemIndexByName(string a)
        {
            foreach (TallyItem item in _currentTally.tallyItems)
            {
                if (item.name.CompareTo(a) == 0)
                    return _currentTally.tallyItems.IndexOf(item);
            }
            return -1;
        }
        #endregion


    }
}
