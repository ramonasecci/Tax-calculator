
using System.Text.RegularExpressions;

namespace TaxCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Benvenut* su TaxCalculator");
            string res = "";
            bool end = false;
            while (!end)
            {
                //RICHIEDO I DATI IN INPUT(VARI CONTROLLI SUI SET DELLE PROPRIETà)
                Console.WriteLine("");
                Console.WriteLine("Inserisci il tuo nome");
                string nome = Console.ReadLine();

                Console.WriteLine("Inserisci il tuo cognome");
                string cognome = Console.ReadLine();

                Console.WriteLine("Inserisci la data di nascita(formato GG/MM/AAAA):");
                string dataDiNascita = Console.ReadLine();

                Console.WriteLine("Inserisci il tuo codice fiscale:");
                string codiceFiscale = Console.ReadLine();

                Console.WriteLine("Genere (M/F/*):");
                string genere = Console.ReadLine();

                Console.WriteLine("Inserisci il tuo comune di residenza:");
                string residenza = Console.ReadLine();

                Console.WriteLine("Inserisci il tuo reddito annuo:");
                double reddito = double.Parse(Console.ReadLine());

                //ISTANZIO OGGETTO 

                User contribuente1 = new User(nome, cognome, dataDiNascita, codiceFiscale, genere, residenza, reddito);

                //VERIFICA RANGE REDDITO --->RICHIAMO PROCEDURA PER CALCOLO IMPOSTA
                if (!contribuente1.Error)
                {
                    if (contribuente1.Reddito > 0 && contribuente1.Reddito <= 15000)
                    {
                        contribuente1.TaxCalc(0, 23, 0);
                    }
                    else if (contribuente1.Reddito >= 15001 && contribuente1.Reddito <= 28000)
                    {
                        contribuente1.TaxCalc(15001, 27, 3450);
                    }
                    else if (contribuente1.Reddito >= 28001 && contribuente1.Reddito <= 55000)
                    {
                        contribuente1.TaxCalc(28001, 38, 6960);
                    }
                    else if (contribuente1.Reddito >= 55001 && contribuente1.Reddito <= 75000)
                    {
                        contribuente1.TaxCalc(55001, 41, 17220);
                    }
                    else
                    {
                        contribuente1.TaxCalc(75001, 43, 25420);
                    }
                    contribuente1.ShowInfo();
                    Console.WriteLine("Desideri calcolare l'imposta per un altro utente? Risondi digitanto: SI/NO");
                    res = Console.ReadLine();
                    if (res.ToUpper() == "NO")
                    {
                        end = true;
                    }
                }
                else
                {
                    //MOSTRA EVENTUALI ERRORI NEL SET
                    Console.Clear();
                    Console.WriteLine("***OPERAZIONE FALLITA***");
                    Console.WriteLine("");
                    foreach (var text in contribuente1.textsError)
                    {
                        Console.WriteLine(text);
                    }
                    Console.WriteLine("");
                    Console.WriteLine("Vuoi proseguire inserendo nuovamente i dati? risondi digitanto: SI/NO");
                    res = Console.ReadLine();
                    if (res.ToUpper() == "NO")
                    {
                        end = true;
                    }
                    Console.Clear();
                }

            }
        }

    }


    class User
    {
        //CAMPI
        string _nome;
        string _cognome;
        string _dataNascita;
        string _codFiscale;
        string _genere;
        string _residenza;
        double _reddito;

        //CATTURA ERRORI 
        public bool Error = false;


        //LISTA ELENCO ERRORI
        public List<string> textsError = new List<string>();

        public string DataNascita
        {
            get { return _dataNascita; }
            set
            {
                string regexPattern = @"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[0-2])/\d{4}$";
                if (!Regex.IsMatch(value, regexPattern))
                {
                    Error = true;
                    textsError.Add("La data di nascita inserita non è valida. Utilizza il formato GG/MM/AAAA.");
                }
                else
                {
                    if (DateTime.TryParseExact(value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                    {
                        if (parsedDate > DateTime.Now)
                        {
                            Error = true;
                            textsError.Add("La data di nascita non può essere successiva alla data odierna.");
                        }
                        else
                        {
                            _dataNascita = value;
                        }
                    }
                }
            }
        }

        public string CodiceFiscale
        {
            get { return _codFiscale; }
            set
            {
                if (value.Length == 16)
                {
                    _codFiscale = value;
                }
                else
                {
                    Error = true;
                    textsError.Add("Il codice fiscale inserito non è corretto");
                }
            }

        }

        public string Genere
        {
            get { return _genere; }
            set
            {
                if (value.ToUpper() == "M" || value.ToUpper() == "F" || value.ToUpper() == "*")
                {
                    _genere = value;
                }
                else
                {
                    Error = true;
                    textsError.Add("Il genere inserito non è corretto, segui l'esempio");
                }
            }
        }

        public double Reddito
        {
            get { return _reddito; }
            set
            {
                if (value > 0)
                {
                    _reddito = value;
                }
                else
                {
                    Error = true;
                    textsError.Add("Devi inserire un importo maggiore di 0");
                }
            }
        }

        public double TotTax { get; set; }

        //costruttore

        public User(string nome, string cognome, string dataNascita, string codFiscale, string genere, string residenza, double reddito)
        {
            _nome = nome;
            _cognome = cognome;
            DataNascita = dataNascita;
            CodiceFiscale = codFiscale;
            Genere = genere;
            _residenza = residenza;
            Reddito = reddito;
        }

        //procedura per il calcono imposta (rangeMin-->range minimo x calcolo eccedenza, taxExec--->imposta già calcolata al reddito al netto del range minimo )
        public void TaxCalc(int rangeMin, int aliquota, int taxExec)
        {
            double eccedenza = Reddito - rangeMin;
            double tax = eccedenza * aliquota / 100;
            TotTax = tax + taxExec;
        }

        //procedura per mostrare tutti i dati + imposte
        public void ShowInfo()
        {
            Console.WriteLine($"Contribuente: {char.ToUpper(_nome[0]) + _nome.Substring(1)} {char.ToUpper(_cognome[0]) + _cognome.Substring(1)}");
            Console.WriteLine($"Nat* il {DataNascita},({Genere})");
            Console.WriteLine($"Residente in {char.ToUpper(_residenza[0]) + _residenza.Substring(1)}");
            Console.WriteLine($"codice fiscale: {CodiceFiscale}");
            Console.WriteLine($"Reddito dichiarato: {Reddito}");
            Console.WriteLine($"IMPOSTA DA VERSARE: {TotTax}");

        }

    }

}
