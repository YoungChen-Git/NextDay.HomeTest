// See https://aka.ms/new-console-template for more information

using NextDay.HomeTest;

var nextDateHandler = new NextDateHandler();
var dateOnly = nextDateHandler.Next(new DateOnly(0001, 1, 1));
Console.WriteLine(dateOnly);