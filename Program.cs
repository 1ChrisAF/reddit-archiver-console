using System.Text.RegularExpressions;

string? username;
Console.Write("Enter a username: ");
username = Console.ReadLine();
if (username != null) {
    HttpClient client = new HttpClient();
    string url = "https://old.reddit.com/u/" + username;
    var response = await client.GetAsync(url);
    var pageContents = await response.Content.ReadAsStringAsync();
    // Console.WriteLine(pageContents);
    string nextButtonPattern = "a href=\"https://old\\.reddit\\.com/user/OrthodoxMemes\\?.*\" rel=\"nofollow next\"";
    Regex regex = new Regex(nextButtonPattern);
    var nextButtonPartial = regex.Matches(pageContents);
    //string beginString = "https://old\\.reddit\\.com/user/OrthodoxMemes\\?";
    nextButtonPattern = @"\b(?:https?://|www.)\S+\b";
    //nextButtonPattern = beginString + nextButtonPattern;
    regex = new Regex(nextButtonPattern);
    nextButtonPartial = regex.Matches(nextButtonPartial[0].Value);
    Console.Write(nextButtonPartial[0].Value);
} else {
    Console.WriteLine("No value entered!");
}
