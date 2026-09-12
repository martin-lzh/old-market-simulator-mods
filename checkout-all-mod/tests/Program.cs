using System;
using OldMarket.CheckoutAll;
var gate = new HoldGate();
object first = new object(), second = new object();
int checks = 0;
void Check(bool condition) { checks++; if (!condition) throw new Exception("Failed hold check " + checks); }
Check(!gate.Tick(true, first, .3f, .6f));
Check(gate.Tick(true, first, .3f, .6f));
Check(!gate.Tick(true, first, 5, .6f));
Check(!gate.Tick(true, second, 5, .6f));
Check(!gate.Tick(false, null, 0, .6f));
Check(!gate.Tick(true, first, .4f, .6f));
Check(!gate.Tick(true, second, .3f, .6f));
Check(!gate.Tick(true, null, 1, .6f));
Check(!gate.Tick(true, second, .3f, .6f));
Check(gate.Tick(true, second, .3f, .6f));
Check(!gate.Tick(false, second, 2, .6f));
Check(gate.Tick(true, second, .6f, .6f));
Check(!gate.Tick(true, second, 2, .6f));
Check(!gate.Tick(false, null, 0, .6f));
Check(gate.Tick(true, first, .6f, .6f));
var confirmation = new ConfirmationWait();
// Idle time between customers must never expire the session.
Check(!confirmation.TimedOut(false, 0));
Check(!confirmation.TimedOut(false, 120));
Check(!confirmation.TimedOut(true, 121));
Check(!confirmation.TimedOut(true, 130.9f));
Check(confirmation.TimedOut(true, 131));
// A server acknowledgement clears the deadline for the next item/payment/customer.
Check(!confirmation.TimedOut(false, 132));
Check(!confirmation.TimedOut(true, 150));
Check(!confirmation.TimedOut(false, 151));
Check(!confirmation.TimedOut(false, 500));
Check(!confirmation.TimedOut(true, 501));
Check(!confirmation.TimedOut(true, 510));
Check(confirmation.TimedOut(true, 511));
confirmation.Reset();
Check(!confirmation.TimedOut(true, 700));
Check(!confirmation.TimedOut(false, 701));
var session = new CheckoutSession();
void Step(bool held, bool toggle, object target, float seconds = .1f) => session.Tick(held, toggle, target, seconds, .6f);
Step(false, true, first);
Check(session.Target == first && session.Toggled);
Step(false, false, first, 120);
Check(session.Target == first && session.Toggled); // No E needed between customers.
Step(false, true, first);
Check(session.Target == null && !session.Toggled);
Step(false, false, first, 120);
Check(session.Target == null);
Step(true, false, first, .6f);
Check(session.Target == first && !session.Toggled); // Original hold mode.
Step(true, true, first);
Step(false, false, first);
Check(session.Target == first && session.Toggled); // Switch hold -> toggle without stopping.
Step(true, true, first);
Check(session.Target == null && !session.Toggled);
Step(true, false, first, 5);
Check(session.Target == null); // Toggle off must not restart via the still-held E.
Step(false, false, first);
Step(true, false, first, .6f);
Check(session.Target == first);
Step(false, false, first);
Check(session.Target == null); // Releasing E stops hold mode.
Step(false, true, null);
Step(false, false, first);
Check(session.Target == null); // No arming away from a counter.
Step(false, true, first);
Step(false, false, second);
Check(session.Target == null && !session.Toggled); // Cannot migrate to another counter.
Step(false, false, first);
Check(session.Target == null);
Step(false, true, first);
Step(false, false, null);
Check(session.Target == null && !session.Toggled); // Menu/focus/range cancellation.
Step(false, false, first);
Check(session.Target == null);
Step(false, true, first);
session.Cancel();
Step(false, false, first);
Check(session.Target == null && !session.Toggled); // Error/disconnect/timeout cancellation.
Step(false, true, first);
Check(session.Target == first && session.Toggled); // Explicit restart after cancellation.
Console.WriteLine($"PASS: {checks} hold, toggle and confirmation checks");
