using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    public partial class RGTFSCompacter
    {
        void CalculateTransferDistances(ref TransitDB tdb, ref OriginalMaps originalMaps)
        {
            ulong iteration = 0;
            var lck = new Object();

            foreach (var stop_collector in tdb.stops)
            {
                if ((++iteration) % 100 == 0)
                {
                    Console.Write('.');
                }

                tdb.stops.AsParallel().ForAll(stop_subject =>
                {
                    if (stop_collector == stop_subject)
                    {
                        return;
                    }

                    lock (lck)
                    {
                        if (stop_collector.transfers.Exists(tr => tr.toStopIndex == stop_subject.idx))
                        {
                            return;
                        }
                    }

                    var coordA = new GeoCoordinate(stop_collector.position.latitude, stop_collector.position.longitude);
                    var coordB = new GeoCoordinate(stop_subject.position.latitude, stop_subject.position.longitude);

                    var transfer = new Transfer
                    {
                        toStopIndex = stop_subject.idx,
                        distance = (float) coordA.GetDistanceTo(coordB)
                    };

                    lock (lck)
                    {
                        stop_collector.transfers.Add(transfer);
                    }

                    transfer.toStopIndex = stop_collector.idx;
                    stop_subject.transfers.Add(transfer);
                });
            }
        }
    }
}
