using System.Threading;
using System;


public sealed class QueuedLock
{
    private static volatile int[] ticketsCount = { 0, 0, 0 };
    private static volatile int[] ticketToRide = { 1, 1, 1 };

    public static void Enter(int parcelLockerId)
    {
        int myTicket = Interlocked.Increment(ref ticketsCount[parcelLockerId]);
        Monitor.Enter(ParcelLockers.SharedResources.ParcelLockers[parcelLockerId]);
        while (true)
        {

            if (myTicket == ticketToRide[parcelLockerId])
                return;
            else
                Monitor.Wait(ParcelLockers.SharedResources.ParcelLockers[parcelLockerId]);
        }
    }

    public static void Exit(int parcelLockerId)
    {
        Interlocked.Increment(ref ticketToRide[parcelLockerId]);
        Monitor.PulseAll(ParcelLockers.SharedResources.ParcelLockers[parcelLockerId]);
        Monitor.Exit(ParcelLockers.SharedResources.ParcelLockers[parcelLockerId]);
    }
}