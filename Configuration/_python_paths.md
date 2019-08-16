
## PYTHON paths
# this relates to: PythonConnector.cs, BRefLeagueBattingController.cs, BRefLeagueBattingController.py




public void AddPythonPaths(ICollection<string> paths)
{
    paths.Add("./miniconda2/lib/python2.7/site-packages");
    paths.Add("./DanBinder/miniconda2/lib/python2.7/site-packages");

    paths.Add("/DanBinder/miniconda2/lib/python2.7");
    paths.Add("/DanBinder/miniconda2/lib/python2.7/site-packages");

    paths.Add("/Library/Frameworks/Python.framework/Versions/2.7/bin/python2.7");
    paths.Add("/Library/Frameworks/Python.framework/Versions/2.7/lib/python2.7/site-packages");

    // python -m site --user-base
    paths.Add("/Users/DanBinder/.local");

    // python -m site --user-site
    paths.Add("/Users/DanBinder/.local/lib/python2.7/site-packages");
    paths.Add("/Users/DanBinder/miniconda2/bin/python");
    paths.Add("/Users/DanBinder/miniconda2/lib/python2.7");
    paths.Add("/Users/DanBinder/miniconda2/lib/python2.7/site.py");
    paths.Add("/Users/DanBinder/miniconda2/lib/python2.7/site-packages");

    paths.Add("/usr/local/lib/python2.7/site-packages");
    paths.Add("/usr/local/lib/python3.7/site-packages");

    paths.Add("/Users/DanBinder/Google_Drive/Coding/Projects/BaseballScraper/Controllers/BaseballReferenceControllers/BRefLeagueBattingController.py");

    paths.Add(Environment.CurrentDirectory);

    // PrintPaths(paths);
    // Console.WriteLine($"paths: {paths}");
}



Python Locations of Things:

    Miniconda location:
        /Users/DanBinder/miniconda2
        /Users/DanBinder/.bash_profile
        A Backup: /Users/DanBinder/.bash_profile-miniconda2.bak

    Miniconda Info
        https://conda.io/miniconda.html


    Possible Locations of Python Things:
        /Users/DanBinder/Google_Drive/Coding/Projects/BaseballScraper/bin/Debug/netcoreapp2.1/Lib
        /Users/DanBinder/Google_Drive/Coding/Projects/BaseballScraper/bin/Debug/netcoreapp2.1/DLLs

    Potential File Paths:
        string s1 = "/Library/Frameworks/Python.framework/Versions/2.7/Resources/Python.app/Contents/MacOS/Python";
        string s2 = "Library/Frameworks/Python.framework/Versions/2.7/Resources/Python.app/Contents/MacOS/Python";
        pathList.Add(s1);
        pathList.Add(s2);

        string s3 = "/Library/Frameworks/Python.framework/Versions/3.6/lib/python3.6/site-packages";
        string s4 = "Library/Frameworks/Python.framework/Versions/3.6/lib/python3.6/site-packages";
        pathList.Add(s3);
        pathList.Add(s4);

        string s5 = "/Library/Frameworks/Python.framework/Versions/2.7/bin/python";
        string s6 = "Library/Frameworks/Python.framework/Versions/2.7/bin/python";
        pathList.Add(s5);
        pathList.Add(s6);

        string s7 = "/usr/local/bin/python";
        string s8 = "usr/local/bin/python";
        pathList.Add(s7);
        pathList.Add(s8);

        string s9  = "/usr/local/bin";
        string s10 = "usr/local/bin";
        pathList.Add(s9);
        pathList.Add(s10);

        string s11 = "/usr/bin/python";
        string s12 = "usr/bin/python";
        pathList.Add(s11);
        pathList.Add(s12);

        string s13 = "/Library/Frameworks/Python.framework/Versions/2.7/lib/python2.7/site-packages";
        string s14 = "Library/Frameworks/Python.framework/Versions/2.7/lib/python2.7/site-packages";
        pathList.Add(s13);
        pathList.Add(s14);

        string s15 = "/usr/local/lib/python3.7/site-packages";
        string s16 = "usr/local/lib/python3.7/site-packages";
        pathList.Add(s15);
        pathList.Add(s16);

        string s17 = "/usr/local/Cellar/python/2.7.14_2/Frameworks/Python.framework/Versions/2.7/lib/python2.7/site-packages";
        string s18 = "usr/local/Cellar/python/2.7.14_2/Frameworks/Python.framework/Versions/2.7/lib/python2.7/site-packages";
        pathList.Add(s17);
        pathList.Add(s18);

        string s19 = "/usr/local/lib/python2.7/site-packages";
        string s20 = "usr/local/lib/python2.7/site-packages";
        pathList.Add(s19);
        pathList.Add(s20);

        string s21 = "/Users/DanBinder/Library/Python/2.7";
        string s22 = "Users/DanBinder/Library/Python/2.7";
        pathList.Add(s21);
        pathList.Add(s22);

        string s23 = "/Users/DanBinder/Library/Python/2.7/bin";
        string s24 = "Users/DanBinder/Library/Python/2.7/bin";
        pathList.Add(s23);
        pathList.Add(s24);

        string s25 = "/Library/Frameworks/Python.framework/Versions/2.7/lib/python2.7/site.py";
        string s26 = "Library/Frameworks/Python.framework/Versions/2.7/lib/python2.7/site.py";
        pathList.Add(s25);
        pathList.Add(s26);

        string s27 = "/Users/DanBinder/miniconda2/lib/python2.7/site-packages";
        string s28 = "Users/DanBinder/miniconda2/lib/python2.7/site-packages";
        pathList.Add(s27);
        pathList.Add(s28);
