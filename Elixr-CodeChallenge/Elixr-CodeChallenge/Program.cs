// See https://aka.ms/new-console-template for more information

string securityQuestionPath = string.Format("{0}\\{1}.dat", AppDomain.CurrentDomain.BaseDirectory, "securityQuestions");
string userInfoPath;
Dictionary<int, string>  securityQuestions = GetSecurityQuestionList();
GetUserSecurityInfo();
void GetUserSecurityInfo()
{
    string userName = CheckUserName();
    BuildUserInfoPath(userName);
    FileStream fileStream = CheckForUserData(userName);

    if (fileStream == null)
    {
        GetNewSecurityQuestionAnswers(userName);
    }
    else
    {
        bool answeredCorrectly = ConfirmAnsweredSecurityQuestion(userName, fileStream);
        if (answeredCorrectly)
        {
            Console.WriteLine("Thank you, you may now enter");
        }
        else
        {
            Console.WriteLine("I'm sorry but you have not answered correctly, pelase check your answers and try again");
        }
    }
}

string CheckUserName()
{
    Console.Clear();
    string name = GetuserName();
    if (string.IsNullOrEmpty(name.Trim()))
    {
        Console.WriteLine("We need your name to continue, please hit enter your username or 'X' to exit");
        var input = Console.ReadLine();
        if (input.ToLower() == "x")
        {
            Environment.Exit(0);
        }
        else
        {
            CheckUserName();
        }
    }
    return name;
}

string GetuserName()
{
    Console.WriteLine("Hi, What is your name?");
    return Console.ReadLine();
}

void GetNewSecurityQuestionAnswers(string userName)
{
    Console.Clear();
    Console.WriteLine("Please answer three of the following security questions");
    Console.WriteLine("Press Enter to move to the next question");
    Dictionary<int, string> answeredQuestions = new Dictionary<int, string>();
    int answered = 0;
    foreach(var item in securityQuestions)
    {
        if(answered == 3)
        {
            break;
        }
        Console.WriteLine(item.Value);
        var answer = Console.ReadLine();
        if(answer != "x" && !string.IsNullOrEmpty(answer))
        {
            answeredQuestions.Add(item.Key, answer);
            answered++;
        }
        else { continue; }
    }
    if(answered < 3)
    {
        Console.WriteLine("You must answer three of security questions, press enter to continue");
        Console.ReadLine();
        GetNewSecurityQuestionAnswers(userName);
    }

    else
    {
        var streamWriter = File.CreateText(userInfoPath);
        using(streamWriter)
        {
            foreach (var entry in answeredQuestions)
                streamWriter.WriteLine("{0}, {1}", entry.Key, entry.Value);
        }
    }
}

Dictionary<int, string> GetSecurityQuestionList()
{
    Dictionary<int, string> fileContents;
    FileStream seccurityQuestionStream = File.OpenRead(securityQuestionPath);
    fileContents = GetFileContents(seccurityQuestionStream);
    return fileContents;
}

bool ConfirmAnsweredSecurityQuestion(string userName, FileStream fileStream)
{
    
    Console.WriteLine(string.Format("Hello {0} please answer security question", userName));
    var answeredQuestions = GetFileContents(fileStream);
    foreach(var item in answeredQuestions)
    {
        var question = securityQuestions[item.Key];
        Console.WriteLine(question);
        var answer = Console.ReadLine();
        if(answer != null && answer.Trim() == item.Value.Trim())
        {
            return true;
        }
    }
    return false;
}

FileStream CheckForUserData(string userName)
{
    
    if (File.Exists(userInfoPath))
    {
        return File.OpenRead(userInfoPath);

    }
    else
    {
        return null;
    }
}

Dictionary<int, string> GetFileContents(FileStream fileStream)
{
    Dictionary<int, string> fileContents = new Dictionary<int, string>();
    using (StreamReader reader = new StreamReader(fileStream))
    {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] arr = line.Split(',');
            fileContents.Add(Convert.ToInt32(arr[0]), arr[1]);
        }
    }

    return fileContents;
}

void BuildUserInfoPath(string userName)
{
    userInfoPath = string.Format("{0}\\{1}.dat", AppDomain.CurrentDomain.BaseDirectory, userName);
}
