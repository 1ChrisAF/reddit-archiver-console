using System.Text.RegularExpressions;

main();

void main() {
    string username = GetUsername();
    while (username == "") {
        username = GetUsername();
    }
    Task<String> htmlTask = getProfileHTML(username);
    string pageContents = htmlTask.Result;
    MatchCollection listings = getListings(pageContents);
    List<Listing> listingList = parseListings(listings);
    foreach (Listing listing in listingList) {
        Console.WriteLine(listing.toString());
    }
    MatchCollection linkCandidates = getLinksFromPage(pageContents);
    string nextButtonLink = "";
    if (linkCandidates.Count > 1) {
        nextButtonLink = getNextButtonLink(linkCandidates);
    }
    //Console.WriteLine(nextButtonLink);
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

MatchCollection getListings(string pageContents) {
    // Regex to pull all user comments and posts from profile page
    Regex regex = new Regex("(class=\"[^\"]*?thing[^\"]*?\")[\\s\\S]*?(class=\"child\")");
    MatchCollection listings = regex.Matches(pageContents);
    return listings;
}

List<Listing> parseListings(MatchCollection listings) {
    string data_type;
    string data_subreddit;
    string data_author;
    string data_permalink;
    DateTime datetime;
    List<Listing> listingList = new List<Listing>();
    Regex regex;
    foreach (Match listing in listings) {
        string? listingValue = listing.Value;
        // Regex for data_type
        regex = new Regex("(?<=data-type=\")\\S*(?=\")");
        data_type = regex.Match(listingValue).Value;
        // Regex for data_subreddit
        regex = new Regex("(?<=data-subreddit=\")\\S*(?=\")");
        data_subreddit = regex.Match(listingValue).Value;
        // Regex for data_author
        regex = new Regex("(?<=data-author=\")\\S*(?=\")");
        data_author = regex.Match(listingValue).Value;
        // Regex for data_permalink
        regex = new Regex("(?<=data-permalink=\")\\S*(?=\")");
        data_permalink = regex.Match(listingValue).Value;
        // Regex for datetime
        regex = new Regex("(?<=datetime=\")\\S*(?=\")");
        datetime = DateTime.Parse(regex.Match(listingValue).Value);
        Listing newListing = new Listing(data_type, data_subreddit, data_author, data_permalink, datetime);
        listingList.Add(newListing);
    }
    return listingList;
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