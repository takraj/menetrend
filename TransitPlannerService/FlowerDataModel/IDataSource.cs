using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransitPlannerLibrary.FlowerDataModel
{
    public interface IDataSource
    {
        IEnumerable<Dictionary<string, string>> GetAllMetaInfo();
        IEnumerable<Dictionary<string, string>> GetAllRoute();
        IEnumerable<Dictionary<string, string>> GetAllSchedule();
        IEnumerable<Dictionary<string, string>> GetAllSequenceLookupData();
        IEnumerable<Dictionary<string, string>> GetAllSequence();
        IEnumerable<Dictionary<string, string>> GetAllTransferByWalkInfo();
        IEnumerable<Dictionary<string, string>> GetAllStopRoutePair();
        IEnumerable<Dictionary<string, string>> GetAllStop();
        IEnumerable<Dictionary<string, string>> GetAllTrip();
    }
}
