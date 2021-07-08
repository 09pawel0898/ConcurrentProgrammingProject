# Concurrent Programming end of term assignment

My task was to program a simulation of the network of three parcel lockers with the following assumptions :
- There are 3 parcel lockers in the system and customers who use them to send and to pick up parcels.
- Each package has an addressee and a sender
- The customer who wants to send the package specifies the recipient
- There is a courier in the system that transports parcels between parcel lockers
- There may be queues at parcel lockers(for collection or dispatch)
- The screen is a shared resource

The intention of the task was to correctly identify the shared resources and synchronize
access to the critical section using the best-suited synchronization mechanism.
The program must meet the safety property and the lifetime property.

### Idea for solution

Implementation language : C#
As there are queues to parcel lockers, i used Monitors to synchronize access to the parcel lockers by customers and the courier.
The monitors themselves implement FIFO access acquiring, but there are exceptions from this behaviour, so i used fixed class QueuedLock, which is based
on monitors and provides FIFO.
The screen is being accessed asynchronously so i used mutex for it.
Some other shared memory operations are protected by mutex too.


![ScreenShot](screenshot1.png)
