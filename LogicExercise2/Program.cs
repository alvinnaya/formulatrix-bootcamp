// See https://aka.ms/new-console-template for more information

using NumPrinter;


NumPrinterClass printer = new NumPrinterClass();

printer.AddRule(3, "foo");
printer.AddRule(4, "baz");
printer.AddRule(5, "bar");
printer.AddRule(7, "jazz");
printer.AddRule(9, "huzz");


printer.Printing(300);
