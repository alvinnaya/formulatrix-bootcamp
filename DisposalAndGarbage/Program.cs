using System;
using System.Diagnostics;



int x = 10;

Console.WriteLine("hello");

Debug.WriteLine("DEBUG: Program dimulai");
Trace.WriteLine("TRACE: Program dimulai");

Debug.WriteIf(x > 5, "DEBUG: x lebih besar dari 5");
Trace.WriteIf(x > 5, "TRACE: x lebih besar dari 5");


