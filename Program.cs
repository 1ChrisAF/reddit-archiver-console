using System.Text.RegularExpressions;
using System.Text.Json;

main();

void main() {
    string username = GetUsername();
    while (username == "") {
        username = GetUsername();
    }
    List<Listing> listingList = parseThroughProfile(username);
    string listingJSON = JsonSerializer.Serialize(listingList);
    Console.WriteLine(listingJSON);

}

string GetUsername() {
    string username;
    // Retrieve username from user
    Console.Write("Enter a username: ");
    username = Console.ReadLine();
    if (username != "") {
        return username.ToLower();
    } else {
        Console.WriteLine("No username entered! Trying again...");
        return "";
    }
}

List<Listing> parseThroughProfile(string username) {
    // List to be returned
    List<Listing> listingList = new List<Listing>();
    // Temp list for each profile page
    List<Listing> newListings = new List<Listing>();
    // For url of page of user profile, initially just username
    string url = "https://old.reddit.com/user/" + username;
    Task<String> htmlTask;
    string pageContents;
    MatchCollection listings;
    do {
        // Call getProfileHTML and pull HTML from relevant
        // user profile page
        htmlTask = getProfileHTML(url);
        // Read Task into string
        pageContents = htmlTask.Result;
        // Collect all user posts and comments on page
        listings = getListings(pageContents);
        // Parse info from collected posts/comments into List of
        // Listing objs from this page
        newListings = parseListings(listings);
        foreach (Listing listing in newListings) {
            // Push Listing obs from THIS page into list of Listing
            // objs for ALL pages
            listingList.Add(listing);
        }
        // Assign the url from the 'next button' and loop through the next page
        url = getNextButtonLink(pageContents);
        // If url is empty, we are on the last page and may break
    } while(url != "");
    // Return all Listing objs from parse
    return listingList;
}

async Task<String> getProfileHTML(string url) {
    HttpClient client = new HttpClient();
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

string getNextButtonLink(string pageContents) {
    string link = "";
    // Grab span tag with URL
    Regex regex = new Regex("<span class=\"[^\"]*?next-button[^\"]*?\">(.*?)<\\/span>");
    link = regex.Match(pageContents).Value;
    link = link.Replace("&amp;", "&");
    // Grab URL from span tag
    regex = new Regex("(?<=href=\")\\S*(?=\")");
    link = regex.Match(link).Value;
    link = link.Replace("&amp;", "&");
    return link;
}