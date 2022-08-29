using System.Text.RegularExpressions;

main();

void main() {
    string username = GetUsername();
    while (username == "") {
        username = GetUsername();
    }
    Task<String> htmlTask = getProfileHTML(username);
    string pageContents = htmlTask.Result;
    MatchCollection linkCandidates = getLinksFromPage(pageContents);
    string nextButtonLink = "";
    if (linkCandidates.Count > 1) {
        nextButtonLink = getNextButtonLink(linkCandidates);
    }
    Console.WriteLine(nextButtonLink);
}

string GetUsername() {
    string username;
    // Retrieve username from user
    Console.Write("Enter a username: ");
    username = Console.ReadLine();
    if (username != "") {
        return username;
    } else {
        Console.WriteLine("No username entered! Trying again...");
        return "";
    }
}

async Task<String> getProfileHTML(string username) {
    HttpClient client = new HttpClient();
    string url = "https://old.reddit.com/u/" + username;
    Console.WriteLine("Checking {0}...", url);
    var response = await client.GetAsync(url);
    var pageContents = await response.Content.ReadAsStringAsync();
    return pageContents;
}

MatchCollection getLinksFromPage(string pageContents) {
    // Regex to parse page for all links
    Regex regex = new Regex(@"\b(?:https?://|www.)\S+\b");
    var linkCandidates = regex.Matches(pageContents);
    return linkCandidates;
}

string getNextButtonLink(MatchCollection linkCandidates) {
    Regex regex = new Regex("OrthodoxMemes\\?count");
    string nextButton = "";
    foreach (var link in linkCandidates) {
        if (regex.IsMatch(link.ToString())) {
            nextButton = link.ToString();
        }
    }
    return nextButton;
}