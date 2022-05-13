using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace EzHomework
{
    //Klasa argumenata, usko povezana sa CHAT-om zato ide u isti fajl
    class chatArgs : EventArgs {
        public int SentId;
        public string SendType;
        public string FName;
        public string LName;
        public float Payment;
    }
    class CHAT
    {
        public delegate void UpdateEventHandler(object source, chatArgs args);
        public delegate void SenderMassageHandler(object source, MessageArgs args);
        //Event update. U mojim ocima bolje se cuva enkapsulacija ako pustim same komponente (programere) da "updat-uju" sami sebe, da li je uopste to potrebno?
        public event UpdateEventHandler updateEvent;
        public event SenderMassageHandler SendMassage;
        //Svi Radnici na platformi
        public List<Worker> OnPlatform = new List<Worker>();
        //aplikacija
        public void Engine()
        {
            while (true)
            {
            Console.Clear();
            Console.WriteLine("Welcome to chat please select one of the following to interact with our application:\n1-Unos, izmena i brisanje podataka o programerima\n2-Slanje poruka\n0-gasenje programa\n");
                String[] PossibleInputs = { "0", "1", "2" };
                String UI = MainDisplay(PossibleInputs);//Mozda bi trebalo da se odradi kao kasnije u posebnoj promenljivoj
                switch (UI)
                {
                    case "1":
                        {
                            InteractionOne();
                        }
                        break;
                    case "2": 
                        {
                            SecondPart();
                        }
                        break;
                    case "0":
                        {
                            return;
                        }
                        break;
                }
            }
        }
        //prva interakcija (1) cela
        public void InteractionOne()
        {
            
            string[] PossibleInputs = { "0", "1", "2", "3", "4", "5", "6" };//Razlog je sto bismo kasnije mogli da imamo vise opcije koje mozemo direktno da menjamo ovde kako dodajemo funkcije
            //postavljen message radi citljivosti koda, a mogu i da kazem lakse da se promeni u buducnosti ako zatreba :D
            string IntroMessage = "1 - unos novog Developera\n2 - izmena Developera\n3 - brisanje Developera\n4 - unos novog QA\n5 - izmena QA\n6 - brisanje QA\n0 - nazad";
            while (true)
            {
                Console.Clear();
                Console.WriteLine(IntroMessage);
                string UI = MainDisplay(PossibleInputs);
                switch (UI)
                {
                    case "1":
                        {
                            AddWorker(1);
                        }
                        break;
                    case "2":
                        {
                            OnWorkerUpdate("feature");
                        }
                        break;
                    case "3":
                        {
                            DeleteWorker("feature");
                        }
                        break;
                    case "4":
                        {
                            AddWorker(2);
                        }
                        break;
                    case "5":
                        {
                            OnWorkerUpdate("testing");
                        }
                        break;
                    case "6":
                        {
                            DeleteWorker("testing");
                        }
                        break;
                    case "0":
                        {
                            return;
                        }
                        break;
                }
            }
        }
        //Druga komponenta cela
        void SecondPart()
        {
            Console.Clear();
            bool exists = false;
            int id;
            Developer toSend;
            while (true) { 
            Console.WriteLine("Enter the id of the profile from which you want to send messages or press '0' to go back");
            String Ui = Console.ReadLine();
            while(!int.TryParse(Ui, out id))
            {
                Console.WriteLine("Please check for errors and try again");
                Ui = Console.ReadLine();
            }
                if (Ui == "0")
                {
                    return;
                }
                else
                {
                    foreach (Worker i in OnPlatform)
                    {
                        if (i.IdentifyMe(id) & i.ReturnTitle() == "feature")
                        {
                            Console.WriteLine("Enter the massage you want to send...");
                            (i as Developer).OnMessageSend(Console.ReadLine());
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        Console.WriteLine("Developer with thta id doesnt exist");
                    }
                }
            }
        }
        //Kontrola povratne vrednost. Unos moze da bude samo jedan od vrednosti datih u nizu "_POSSIBLE_COUTNS", ime lose (ne intuitivno) ali ne mogu da trazim gde sam je sve koristio da bih je menjao
        string MainDisplay(string[] _POSSIBLE_COUNTS)
        {
         string UI="default";
         while (!_POSSIBLE_COUNTS.Contains(UI))
         {
           Console.WriteLine("Please insert one of the above methods");
           UI = Console.ReadLine();
            }
         return UI;
         }
        //Dodavanje kao i potrebne provere(1)
        bool InputErrorWorkerVar(string[] _TO_CHECK)//funkcija je mozda mogla da ide direktno u main loop zbog boljeg iskoriscavanja "out float-a", ali ovako je dosta citljivije za pola funkcije performansi
        {
            if ((_TO_CHECK.Length != 3) || !(Single.TryParse(_TO_CHECK[2], out float muka)))
            {
                return true;
            }
           
            return false;
        }
        //Ako se setim da popravim problem za vise tipova radnika bez promene ove funkcije, interfaci ili delegati su mozda kljucni
        //Dodavanje radnika prosto(1)
        void AddWorker(int SimpleCheck)
        {
            Console.Clear();
            Console.WriteLine("Please enter first name, second name and payment of new manager seperated by ' ' (spaces)\nExample: Mile Strugar 6000\nOr enter '0' to go back");
            string UI = Console.ReadLine();
            string[] ForUse = UI.Split(" ");
            while (InputErrorWorkerVar(ForUse))
            {
                if (UI =="0") { return; }
                Console.WriteLine("Input incorrect, please look at above mentioned requirements");
                UI = Console.ReadLine();
                ForUse = UI.Split(" ");
            }
            if (SimpleCheck == 1)
            {
                Developer Temp = new Developer(float.Parse(ForUse[2]), ForUse[0], ForUse[1]);
                updateEvent += Temp.onUpdateEvent;//subskrajbanje na event update
                Temp.MessageSend += OnMessageRecieved;//jer samo developeri mogu da salju message
                SendMassage += Temp.OnMessageRecieved;
                OnPlatform.Add(Temp);
            }
            else
            {
                QA Temp = new QA(float.Parse(ForUse[2]), ForUse[0], ForUse[1]);
                updateEvent += Temp.onUpdateEvent;
                SendMassage += Temp.OnMessageRecieved;
                OnPlatform.Add(Temp);
            }
        }
        //pokretanje eventa radi updata radnika(1)
        protected virtual void OnWorkerUpdate(string Title)
        {
            Console.Clear();
            bool exists = false;
            chatArgs args = new chatArgs();
            while (true)
            {
                Console.WriteLine("Enter id of the worker you are looking for, or enter '0' to go back");
                string Ui = Console.ReadLine();
                while (!int.TryParse(Ui, out args.SentId))
                {
                    Console.WriteLine("Enter valid value for of id (integer number) or '0' to go back");
                    Ui = Console.ReadLine();
                }
                if(Ui=="0") { return; }
                foreach (Worker i in OnPlatform)
                {
                    if (i.IdentifyMe(args.SentId) & Title == i.ReturnTitle())
                    {
                        exists = true;
                        break;
                    }
                }
                if (exists == true)
                {
                    Console.WriteLine("Please enter first name, second name and payment of new programer seperated by ' ' (spaces)\nExample: Mile Strugar 6000 or '0' to go back");
                    Ui = Console.ReadLine();
                    while (InputErrorWorkerVar(Ui.Split(" ")))
                    {
                        if (Ui == "0")
                        {
                            return;
                        }
                        Console.WriteLine("Input incorrect, please look at above mentioned requirements");
                        Ui = Console.ReadLine();
                    }
                    args.FName = Ui.Split(" ")[0];
                    args.LName = Ui.Split(" ")[1];
                    args.Payment = float.Parse(Ui.Split(" ")[2]);
                    args.SendType = Title;
                    if (updateEvent != null)
                    {
                        updateEvent(this, args);
                    }
                }
                else
                {
                    Console.WriteLine($"The programer that id doesn't exist");
                }
                exists = false;
            }
        }
        //Prosto brisanje(1)
        void DeleteWorker(String _TITLE)
        {
            int lookFor;
            Console.Clear();
            while (true)
            {
                Console.WriteLine("Enter the ID of dev you want to delete or '0' to go back");
                string Ui = Console.ReadLine();
                while (!int.TryParse(Ui, out lookFor))
                {
                    Console.WriteLine("Enter valid value for of id (integer number) or '0' to go back");
                    Ui = Console.ReadLine();
                }
                if (Ui == "0")
                {
                    return;
                }

                foreach(Worker i in OnPlatform)
                {
                    if(i.IdentifyMe(lookFor) & _TITLE==i.ReturnTitle())
                    {
                        OnPlatform.Remove(i);
                        break;
                    }
                }
            }
        }
        //Prvo chat dobija poruku od neke komponente a zatim salje ostalim komponentama tu poruku
        void OnMessageRecieved(object source, MessageArgs args)
        {
            if (SendMassage != null)
            {
                SendMassage(source, args);
            }
        }
        
        
    }
}
