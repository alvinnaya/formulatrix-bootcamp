using System.Text;

Console.WriteLine("================== string and char ========================");
Console.WriteLine(char.ToUpper('a'));     // C
Console.WriteLine(char.IsWhiteSpace('\t')); // True
Console.WriteLine(char.ToLowerInvariant('B'));

char[] ca = "Hello".ToCharArray();
string s = new string(ca); // s = "Hello"
Console.WriteLine(new string('*', 10));
Console.WriteLine(s);


string empty = "";
Console.WriteLine(empty.Length == 0); 
string nullString = null;
Console.WriteLine(nullString == null);  
// Console.WriteLine(nullString.Length);
Console.WriteLine($"{s[0]}{s[2]}");

Console.WriteLine(string.Equals("foo", "FOO", StringComparison.InvariantCulture)); // True

string composite = "It's {0} degrees in {1} on this {2} morning";
string s2 = string.Format(composite, 35, "Perth", DateTime.Now.DayOfWeek);
Console.WriteLine(s2);



StringBuilder sb = new StringBuilder("test");
Console.WriteLine(sb);
TimeSpan ts = new TimeSpan();
Console.WriteLine($"{TimeSpan.FromTicks(100)}");


Console.WriteLine("============= Date and Time ==============");

Console.WriteLine($"timespan : {new TimeSpan(2,30,0)}");
Console.WriteLine($"TImeSpan from Hours: {TimeSpan.FromDays(2.5)}");

TimeSpan duration = TimeSpan.FromHours(3) + TimeSpan.FromMinutes(20);
TimeSpan days = TimeSpan.FromDays(7) - TimeSpan.FromHours(2);

Console.WriteLine(duration);
Console.WriteLine(days);

DateTime d1 = new DateTime(2026, 1, 22);
DateTime d2 = new DateTime(2026, 1, 22, 14, 30, 0);

DateTime utcTime = new DateTime(
    2026, 1, 22, 7, 0, 0, DateTimeKind.Utc);

DateTime localTime = new DateTime(
    2026, 1, 22, 14, 0, 0, DateTimeKind.Local);

Console.WriteLine(d1);
Console.WriteLine(d2);
Console.WriteLine(utcTime);
Console.WriteLine(localTime);

Console.WriteLine("== date now ==");
DateTime now = DateTime.Now;       // lokal
DateTime utcNow = DateTime.UtcNow; // UTC
DateTime today = DateTime.Today;   // jam 00:00

Console.WriteLine($"{now}");
Console.WriteLine($"{utcNow}");
Console.WriteLine($"{today}");


DateTime deadline = DateTime.Now.AddDays(7);
DateTime reminder = deadline.AddHours(-2);

Console.WriteLine($"deadline: 7 days from nows at {deadline}");
Console.WriteLine($"2 hours before deadline {reminder}");


Console.WriteLine("=============== Formating and Parsing ==================");

bool flag = true;
string s1fp = flag.ToString(); // "True"

DateTime nowfp = DateTime.Now;
string sfp = now.ToString();  // tergantung culture OS

bool success = int.TryParse("123", out int hasil);
int currency = 100000000;

Console.WriteLine($"jumlah uang : {currency.ToString("C")}");



