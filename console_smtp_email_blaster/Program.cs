using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Mail;
using System.IO;

namespace console_smtp_email_blaster
{
    static class Globals
    {
        public static string[] emails_listing; //store emails from text list
        public static string auth_smtp;         
        public static string auth_user;
        public static string auth_pwd;
        public static string auth_port;
        public static string auth_email;
        public static string auth_name;
        public static string auth_subject;
        public static string last_session_email;
        public static string last_email;
        public static string server_profile_path;
        public static string pointer_path; //store the path of the last email sent from a list that needs continuation
        public static string html_path;
        public static string current_list;
        public static string txt_log;
        public static string html_body; //store HTML code
        public static int email_hour; 
        public static int count_attemp;
        public static int sleep_miliseconds;
        public static int sending_mode; //Store sending mode, blast = 1 or Progressive = 2
        public static bool limit_time_ok;  //This var it's useless for this version. For Future  purpose as theFunction where its used.
        public static bool check_pointer; //Check if pointer_path exist. This bool is used multiple times instead File.Exists
        public static DateTime start_time;
        public static DateTime last_process_time;
        public static List<string> list_folder_txts; //Store differents text lists
        public static List<string> list_log_sent; //store sent emails and save this log in a file when the process is complete.
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 142;
            Console.Title = "SMTP Email Blaster";
            MenuLogo();

            //Main paths
            Globals.current_list = string.Empty;
            Globals.server_profile_path = "./Resources/server_config.txt";
            Globals.html_path = "./Resources/email-template.html";
            Globals.txt_log = "./Log/log.txt";
            Globals.limit_time_ok = true;
            Globals.list_log_sent = new List<string>();
            Globals.html_body = File.ReadAllText(Globals.html_path);
            bool runMainMenu = true;

            Console.WriteLine(" This is a Console Mass Email Sender, Welcome !\n");
            MenuOptions();
            do
            {
                Int32.TryParse(Console.ReadLine(), out int menuOptionNumber);
                switch (menuOptionNumber)
                {
                    case 1:
                        Option1();
                        break;
                    case 2:
                        Option2();
                        break;
                    case 3:
                        Option3();
                        break;
                    case 4:
                        ImportNewEmailList();
                        break;
                    case 5:
                        try
                        {
                            System.Diagnostics.Process.Start(Path.GetFullPath(Globals.html_path));
                            MenuOptions();
                        }
                        catch
                        {
                            Environment.Exit(0);
                        }
                        break;
                    case 6:
                        ConfigSMTP();
                        break;
                    case 7:
                        Console.WriteLine("\n This is a Mass Email Sender, please edit the Email HTML Template at your will, fill the\n" +
                            " SMTP Profile fields and for last copy a list of emails (.txt document) to 'Lists' folder.\n");
                        Console.WriteLine(" [-] Email HTML Template: " + @"/Resources/email-template.html");
                        Console.WriteLine(" [-] SMTP Profile Config: " + @"/Resources/server_config.txt");
                        Console.WriteLine(" [-] Lists Folder: " + "/Lists/\n");
                        MenuOptions();
                        break;
                    case 8:
                        runMainMenu = false;
                        break;
                    default:
                        Console.WriteLine(" Incorrect option, please insert a valid option.");
                        break;
                }
            } while (runMainMenu);
        }

        static void MenuOptions()
        {
            string condit = "Add";
            if (File.Exists(Globals.server_profile_path))
                condit = "Edit";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n SMTP Email Blaster MENU");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  1. Send Test Email\n" +
                "  2. Send Bulk Emails\n" +
                "  3. Schedule\n" +
                "  4. Import List\n" +
                "  5. View Email Template\n" +
                "  6. " + condit + " SMTP Profile\n" +
                "  7. About\n" +
                "  8. Exit\n");
        }

        static void Option1() //Send test Email
        {
            CheckSMTP();
            string test_email;
            bool confirm_email = true;

            do
            {
                Console.WriteLine("\n Please introduce an email address to send a message: ");
                test_email = Console.ReadLine();

                if (test_email == string.Empty)
                    break;

                Console.WriteLine("\n Do you confirm " + test_email + " ? (y/n)");
                if (Console.ReadLine() == "y")
                    confirm_email = false;

            } while (confirm_email);

            if (test_email == string.Empty)
                MenuOptions();
            else
            {

                Console.WriteLine("\n");
                Console.ForegroundColor = ConsoleColor.Green;
                SendEmail(test_email);
                MenuOptions();
            }
        }

        static void Option2() //Send Bulk Emails
        {
            CheckSMTP();
            if (CheckLimitTime())
            {
                Globals.list_folder_txts = Directory.GetFiles(@"Lists", "*.txt", SearchOption.TopDirectoryOnly).ToList();

                Console.WriteLine("\n Select List of Emails:");
                int mini_counter = 1; //var reutilizada para numerar menu e também receber resposta do utilizador.
                foreach (string list in Globals.list_folder_txts)
                {
                    if (File.Exists("./Resources/" + Path.GetFileName(list)))
                        Console.WriteLine(" " + mini_counter.ToString() + ". " + Path.GetFileName(list) + " (continue)");
                    else
                        Console.WriteLine(" " + mini_counter.ToString() + ". " + Path.GetFileName(list));
                    mini_counter++;
                }
                Int32.TryParse(Console.ReadLine(), out mini_counter);
                if(mini_counter == 0)
                {
                    Console.WriteLine("Incorrect Number!");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                else
                {
                    Globals.current_list = Globals.list_folder_txts[mini_counter - 1];
                    Globals.pointer_path = "./Resources/" + Path.GetFileName(Globals.current_list);
                    Globals.txt_log = "./Log/" + Path.GetFileNameWithoutExtension(Globals.current_list) + "_log.txt";
                    if (File.Exists(Globals.pointer_path))
                    {
                        
                        Globals.check_pointer = true;
                        Globals.last_session_email = File.ReadAllText("./Resources/" + Path.GetFileName(Globals.current_list));
                    }

                    if (!File.Exists(Globals.current_list))
                    {
                        File.Create(Globals.current_list);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" Please create a text file with a list of email and copy to folders Lists");
                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                    else
                    {
                        Globals.emails_listing = File.ReadAllLines(Globals.current_list);
                        if (Globals.emails_listing.Length == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(" Please fill " + Globals.current_list + " and restart this app.");
                            Console.ReadKey();
                            Environment.Exit(0);

                        }
                        else
                        {
                            Console.WriteLine("\n Your list have " + Globals.emails_listing.Length.ToString() +
                                " emails. Do you like start sending now ? (y/n)");
                            string answer = Console.ReadLine();

                            if (answer == "n")
                            {
                                Console.WriteLine("\n");
                                MenuOptions();
                            }
                            else
                            {
                                if (Globals.emails_listing.Length > 100)
                                {
                                    Console.WriteLine("\n How many email/hour you like to send ? (max 1000)");
                                    Int32.TryParse(Console.ReadLine(), out Globals.email_hour);

                                    if ((Globals.email_hour > 1000) || (Globals.email_hour == 0))
                                        Globals.email_hour = 1000;



                                    Console.WriteLine("\n Please select the sending mode:\n" +
                                        "  1. Blast Mode (Default)\n" +
                                        "  2. Progressive Mode");
                                    Int32.TryParse(Console.ReadLine(), out Globals.sending_mode);

                                    if ((Globals.sending_mode != 2))
                                    {
                                        Console.WriteLine("\n Blaste Mode selected.");
                                        Globals.sending_mode = 1;
                                    }
                                    else
                                    {
                                        Console.WriteLine("\n Progressive Mode selected.");
                                    }
                                }
                                else
                                {
                                    Globals.sending_mode = 1;
                                    Globals.email_hour = 3600;
                                }

                                double email_per_second = 3600 / Globals.email_hour;
                                Globals.sleep_miliseconds = 100;

                                if(Globals.sending_mode == 2) //progressive mode
                                {
                                    Globals.sleep_miliseconds = (int)Math.Round(email_per_second * 1000, 1);
                                    Console.WriteLine(" Waiting time between emails: " + (Globals.sleep_miliseconds / 1000).ToString() + " Seconds.");
                                }
                                
                                //dev line (remove on release version)
                                //Console.WriteLine("\n\n Sleep Time:" + Globals.sleep_miliseconds.ToString() + "\n\n");
                                
                                Console.ForegroundColor = ConsoleColor.Green;

                                Thread thr1 = new Thread(new ThreadStart(MainThread));
                                thr1.Start();

                                while (true)
                                {
                                    Thread.Sleep(1000);
                                }

                            }
                        }
                    }
                }
            }
            else
            {
                TimeSpan tspan = DateTime.Now - Globals.last_process_time;
                Console.WriteLine(" You already exceeded the server hour limit rate, please wait " + TimeConverter(tspan));
            }
        }

        static void Option3()//Schedule
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" The Schedule function is not avaiable yet. Please wait until next version ;)\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void MainThread()
        {
            Globals.start_time = DateTime.Now;
            Globals.count_attemp = 0;

            if (Globals.check_pointer)
            {
                Console.Title = "SMTP Loading Emails...";
                Console.WriteLine("\nLoading emails...\n");
            }

            bool starting_pointer = false;
            int line_line_counter = 1;
            int list_size = Globals.emails_listing.Length;

            //starting email cycle
            foreach (string e in Globals.emails_listing) 
            {
                if(Globals.check_pointer == true)
                {
                    if (e == Globals.last_session_email)
                        starting_pointer = true;

                    if (starting_pointer == true)
                    {
                        if (Globals.count_attemp < Globals.email_hour)
                        {
                            SendEmail(e);
                            Globals.count_attemp++;
                            Console.Title = "SMTP Sending: " + line_line_counter.ToString() + "/" + list_size.ToString();
                            Globals.last_email = e;
                            Globals.list_log_sent.Add(e);
                            if (Globals.sending_mode == 2)
                            {
                                if((Globals.sleep_miliseconds / 1000) >= 10)
                                    Console.WriteLine("Waiting " + (Globals.sleep_miliseconds / 1000).ToString() + " Seconds...");
                                Thread.Sleep(Globals.sleep_miliseconds);
                            }
                        }
                        else
                        {
                            File.WriteAllText(Globals.pointer_path, e);
                            break;
                        }
                    }
                }else
                {
                    if (Globals.count_attemp < Globals.email_hour)
                    {
                        SendEmail(e);
                        Globals.count_attemp++;
                        Console.Title = "SMTP Sending " + line_line_counter.ToString() + "/" + list_size.ToString();
                        Globals.last_email = e;
                        Globals.list_log_sent.Add(e);
                        if (Globals.sending_mode == 2)
                            Thread.Sleep(Globals.sleep_miliseconds);
                    }
                    else
                    {
                        File.WriteAllText(Globals.pointer_path, e);
                        break;
                    }
                }
                line_line_counter++;
            }   //end of email cycle.

            File.AppendAllLines(Globals.txt_log, Globals.list_log_sent); //write log
            Globals.last_process_time = DateTime.Now;
            TimeSpan total = Globals.last_process_time.Subtract(Globals.start_time);
            Console.ForegroundColor = ConsoleColor.Yellow;

            if (Globals.last_email == Globals.emails_listing[Globals.emails_listing.Length - 1])
            {
                Console.WriteLine("\n List successfully processed :)");
                if (File.Exists(Globals.pointer_path))
                    File.Delete(Globals.pointer_path);
            }
            else
                Console.WriteLine("\n Emails/hour limit reached. Please come back for more at " + Globals.start_time);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Start Time: " + Globals.start_time +
                "\n End Time:   " + Globals.last_process_time +
                "\n Total: " + TimeConverter(total)+"\n");
            Globals.limit_time_ok = false;

            Console.ReadKey();
            Environment.Exit(0);
        }

        static void ImportNewEmailList()
        {
            bool mRunning = true;
            bool validated = false;

            Console.WriteLine(" Import a list of emails. Please introduce the path:");
            Console.Write(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+@"\");
            string new_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + Console.ReadLine();

            if (!File.Exists(new_path))
            {

                Console.WriteLine(" The path introduced is incorret! Do you like re-write the path? (y/n)");
                string answr = Console.ReadLine();
                if (answr == "y" || answr == "yes")
                {

                    do
                    {
                        Console.Write(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\");
                        new_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + Console.ReadLine();
                        if (File.Exists(new_path))
                        {
                            mRunning = false;
                            validated = true;
                        }
                        else
                        {
                            Console.WriteLine("The written path is incorret !");
                        }
                    } while (mRunning);
                }
            }else
            {
                validated = true;
            }

            if (validated)
            {
                try
                {
                    File.Copy(new_path, "./Lists/" + Path.GetFileName(new_path));
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" List " + Path.GetFileName(new_path) + " successfully copied.\n");
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Upss something went wrong\n\n");
                }
            }

            MenuOptions();
        }

        static void CheckSMTP()
        {
            //check auth information
            if (File.Exists(Globals.server_profile_path))
            {
                try
                {
                    string[] lines = File.ReadAllLines(Globals.server_profile_path);
                    List<string> credentials = new List<string>();
                    foreach (string line in lines)
                    {
                        string[] result = line.Split(':');
                        credentials.Add(result[1]);
                    }

                    if (lines.Length == 7)
                    {
                        Globals.auth_smtp = credentials[0];
                        Globals.auth_port = credentials[1];
                        Globals.auth_user = credentials[2];
                        Globals.auth_pwd = credentials[3];
                        Globals.auth_email = credentials[4];
                        Globals.auth_name = credentials[5];
                        Globals.auth_subject = credentials[6];
                    }
                    else
                        HandleError();

                }
                catch { HandleError(); }
            }
            else
            {
                Console.WriteLine(" You need config your SMTP Details");
                ConfigSMTP();
            }

            string passw_aster = string.Empty; //replace password for *******
            for (int i = 0; i < Globals.auth_pwd.Length; i++)
                passw_aster += "*";

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n SMTP PROFILE");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" SMTP Server: " + Globals.auth_smtp);
            Console.WriteLine(" Server Port: " + Globals.auth_port);
            Console.WriteLine("        User: " + Globals.auth_user);
            Console.WriteLine("    password: " + passw_aster);
            Console.WriteLine("  Email From: " + Globals.auth_email);
            Console.WriteLine("  Email Name: " + Globals.auth_name + "\n");
        }

        static void ConfigSMTP()
        {
            Console.WriteLine("\n 1. Write your SMTP Server details now \n 2. fill the fields in server_config.txt");
            if (Console.ReadLine() == "1")
            {
                Console.Write(" SMTP Server: ");
                Globals.auth_smtp = Console.ReadLine();
                Console.Write(" Server Port (Default 587): ");
                Globals.auth_port = Console.ReadLine();
                Console.Write(" Server User: ");
                Globals.auth_user = Console.ReadLine();
                Console.Write(" Password: ");
                Globals.auth_pwd = Console.ReadLine();
                Console.Write(" Email: ");
                Globals.auth_email = Console.ReadLine();
                Console.Write(" Emails Name: ");
                Globals.auth_name = Console.ReadLine();
                Console.Write(" Email Subject: ");
                Globals.auth_subject = Console.ReadLine();

                bool run_this = true;
                do
                {
                    int counter = 1;
                    Console.WriteLine("\nDo you confirm the following data ?");

                    string[] data_to_write = new string[] { "SMTP Server:"+Globals.auth_smtp, "Server Port:"+Globals.auth_port,
                        "User:"+Globals.auth_user, "Password:"+Globals.auth_pwd, "Email:"+Globals.auth_email, "Name:"+Globals.auth_name,
                        "Subject:"+Globals.auth_subject};

                    foreach (string text in data_to_write)
                    {
                        Console.WriteLine(" " + counter.ToString() + ". " + text);
                        counter++;
                    }
                    Console.Write("\n Yes (y) / No (n): ");
                    if (Console.ReadLine() == "n")
                    {
                        Console.WriteLine(" Which field is wrong? (number)");
                        Int32.TryParse(Console.ReadLine(), out int _result);
                        SmptServerChange(_result);
                    }
                    else
                    {
                        File.WriteAllLines(Globals.server_profile_path, data_to_write);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n SMTP Profile saved successfully.\n\n");
                        MenuOptions();
                        run_this = false;
                    }

                } while (run_this);

            }
            else
                Environment.Exit(0);
        }

        static void SmptServerChange(int field)
        {
            if (field == 0)
                field = 1;

            switch (field)
            {
                case 1:
                    Console.Write(" SMTP Server: ");
                    Globals.auth_smtp = Console.ReadLine();
                    break;
                case 2:
                    Console.Write(" Server Port (Default 587): ");
                    Globals.auth_port = Console.ReadLine();
                    break;
                case 3:
                    Console.Write(" Server User: ");
                    Globals.auth_user = Console.ReadLine();
                    break;
                case 4:
                    Console.Write(" Password: ");
                     Globals.auth_pwd = Console.ReadLine();
                    break;
                case 5:
                    Console.Write(" Email: ");
                    Globals.auth_email = Console.ReadLine();
                    break;
                case 6:
                    Console.Write(" Emails Name: ");
                    Globals.auth_name = Console.ReadLine();
                    break;
                case 7:
                    Console.Write(" Email Subject: ");
                    Globals.auth_subject = Console.ReadLine();
                    break;
                default:
                    break;
            }
        }

        static bool CheckLimitTime()
        {
            if (Globals.limit_time_ok)
                return true;
            else if(Globals.email_hour == 0)
            {
                return true;
            }
            else
            {
                if (Globals.last_process_time != DateTime.MinValue)
                {
                    TimeSpan ts = DateTime.Now - Globals.last_process_time;
                    if (ts.TotalMinutes > 60)
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }
        }

        static string TimeConverter(TimeSpan ts)
        {
            string minutesOrSeconds;

            if (ts.TotalSeconds > 60)
            {
                minutesOrSeconds = Math.Round((double)ts.TotalMinutes, MidpointRounding.ToEven).ToString() + " minutes.";
            }else
            {
                minutesOrSeconds = Math.Round((double)ts.TotalSeconds, MidpointRounding.ToEven).ToString() + " seconds.";
            }
            return minutesOrSeconds;
        }

        static void SendEmail(string _email)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(Globals.auth_smtp);

                mail.From = new MailAddress(Globals.auth_email, Globals.auth_name);
                mail.To.Add(_email);
                mail.Subject = Globals.auth_subject;

                mail.IsBodyHtml = true;
                string htmlBody;

                htmlBody = Globals.html_body;

                mail.Body = htmlBody;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Globals.auth_user, Globals.auth_pwd);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

                Console.WriteLine("Sent to " + _email);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" sending mail to " + _email+ " failed!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void HandleError()
        {
            Console.WriteLine("an error occurred while reading the smtp server file, Please check " +
                    Globals.server_profile_path + " and restart this app.");
            Console.ReadKey();
            Environment.Exit(0);
        }

        static void MenuLogo()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("\n\n");
            Console.WriteLine(@" ███████╗███╗   ███╗████████╗██████╗     ███████╗███╗   ███╗ █████╗ ██╗██╗         ██████╗ ██╗      █████╗ ███████╗████████╗███████╗██████╗ ");
            Console.WriteLine(@" ██╔════╝████╗ ████║╚══██╔══╝██╔══██╗    ██╔════╝████╗ ████║██╔══██╗██║██║         ██╔══██╗██║     ██╔══██╗██╔════╝╚══██╔══╝██╔════╝██╔══██╗");
            Console.WriteLine(@" ███████╗██╔████╔██║   ██║   ██████╔╝    █████╗  ██╔████╔██║███████║██║██║         ██████╔╝██║     ███████║███████╗   ██║   █████╗  ██████╔╝");
            Console.WriteLine(@" ╚════██║██║╚██╔╝██║   ██║   ██╔═══╝     ██╔══╝  ██║╚██╔╝██║██╔══██║██║██║         ██╔══██╗██║     ██╔══██║╚════██║   ██║   ██╔══╝  ██╔══██╗");
            Console.WriteLine(@" ███████║██║ ╚═╝ ██║   ██║   ██║         ███████╗██║ ╚═╝ ██║██║  ██║██║███████╗    ██████╔╝███████╗██║  ██║███████║   ██║   ███████╗██║  ██║");
            Console.WriteLine(@" ╚══════╝╚═╝     ╚═╝   ╚═╝   ╚═╝         ╚══════╝╚═╝     ╚═╝╚═╝  ╚═╝╚═╝╚══════╝    ╚═════╝ ╚══════╝╚═╝  ╚═╝╚══════╝   ╚═╝   ╚══════╝╚═╝  ╚═╝");
            Console.WriteLine(" Version 1.0. by Bernardo Ascensão\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
