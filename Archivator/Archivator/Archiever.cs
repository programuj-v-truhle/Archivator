using System;
using System.Collections;
using System.IO;

using System.Text; // Tento namespace je potřeba pro statickou třídu Encoding používanou níže

namespace Archivator
{
    /// <summary>
    /// Tato struktura obsahuje informace o souboru v archívu (jeho název a velikost)
    /// </summary>
    public struct FileHeader
    {
        public string Name;
        public int Size;
    }

    /// <summary>
    /// Hlavní třída
    /// </summary>
    class Archiever : IDisposable
    {
        
        /// <summary>
        /// FilesIn je pole struktur FileHeader - to jsou soubory které již jsou fyzicky v archívu (je jasné že toto pole je prázné u nových archívů kde ještě nic není)
        /// FilesToAdd je pole cest k souborů (string), které mají být po zavolání metody SaveArchieve přidány do archívu
        /// FilesToRemove je pole indexů (int) souborů, které již v archívu jsou (tj. jsou to indexy v poli FilesIn), ale mají být odstraněny - po zavolání SaveArchieve už nebudou zahrnuty
        /// </summary>
        private ArrayList FilesIn,FilesToAdd,FilesToRemove;
        
        private const int NameMaxLength = 150;
        
        private FileStream MyArchieve; // Stream souboru s archívem, otevřen po celou dobu práce s ním
        private long FilesBegin; // Obsahuje pozici na které začínají samotné soubor (tj. hned za hlavičkou)
   
        /// <summary>
        /// Konstruktor třídy
        /// </summary>
        public Archiever()
        {
            // Inicializuj kolekce
            FilesIn = new ArrayList();
            FilesToAdd = new ArrayList();
            FilesToRemove = new ArrayList();

            // Na null lze nastavit všechny třídy, když ještě nemají instanci
            MyArchieve = null;
            
            FilesBegin = -1;
        }

        /// <summary>
        /// Pomocná soukromá metoda která vyhledá soubor v archívu podle jeho názvu a vrátí jeho velikost
        /// </summary>
        /// <param name="Name">Jméno souboru v archívu</param>
        /// <returns>Vrací index souboru v interním poli názvů souborů</returns>
        private int FindFileByName(string Name)
        {
            // Zde projdeme celé pole souborů v otevřeném archívu a zkusíme najít daný soubor - pokud najdeme vrátíme jeho index v poli.
            for (int i = 0; i < FilesIn.Count; i++)
            {
                // Protože jsou prvky v ArrayList typu object, je prvek přetypovat na FileHeader abychom mohli používat jeho vlastnost Name.
                if (((FileHeader)FilesIn[i]).Name == Name)
                    return i;
            }

            // Když jsme prošli celé pole a nic nenašli vyvolej vyjímku
            throw new ArchieverException("Soubor s daným názvem v archívu neexistuje");
        }

        /// <summary>
        /// Veřejná metoda, která zjistí zda je daný soubor v archívu
        /// </summary>
        /// <param name="Name">Jméno souboru</param>
        /// <returns>True když je soubor fyzicky v archívu, False když není.</returns>
        public bool HasFile(string Name)
        {
            // Využijeme již implementované metody FindFileByName
            try
            {
                FindFileByName(Name); // To co metoda vrátí (index v poli FilesIn) nepotřebujeme, zajímá nás pouze to zda hodí vyjímku nebo ne.
            }
            catch (ArchieverException)
            {
                return false; // Metoda FindFileByName vyhodila vyjímku => soubor v archívu neexistuje
            }

            // Všechno prošlo - soubor v archívu je
            return true;
        }

        /// <summary>
        /// Vytvoří nový archív v daném umístění
        /// </summary>
        /// <param name="Where">Cesta k archívu</param>
        public void NewArchieve(string Where)
        {
            // Jestli je něco otevřeno, zavři
            if (IsOpened) Close();
            MyArchieve = File.Create(Where); // Vytvoř nový soubor archívu
        }

        /// <summary>
        /// Otevři archív, přečti názvy souborů v něm uložených do interní kolekce FilesIn
        /// </summary>
        /// <param name="Where">Cesta k archívu</param>
        public void OpenArchieve(string Where)
        {
            // Je-li už nějaký archív otevřen, zavři jej
            if (IsOpened) Close();

            // Kdyby se cokoliv stalo, použij try-catch
            try {
               // Otevři soubor s archívem (metoda File.Open vyhodí FileNotFoundException vyjímku když soubor neexistuje)
               MyArchieve = File.Open(Where,FileMode.Open); 

               // Přečti první 4 bajty, v nich je uložený počet souborů v archívu. (Vidíte, že je dobré si pamatovat kolik který datový typ zabírá v paměti bajtů)
               byte[] NumFiles = new byte[4];
               MyArchieve.Read(NumFiles, 0, 4);

               // Statická třída BitConverter slouží k převedení pole bajtů na nějaký primitivní datový typ (v tomto případě Int32 - což je obyčejný int)
               int CountFiles = BitConverter.ToInt32(NumFiles, 0); 
               
               /* Teď budeme číst názvy souborů a jejich velikosti v archívu.
                * Vždycky 150 bajtů obsahuje název souboru a další 4 bajty hned za ním obsahují velikost tohoto souboru, takže budeme číst po 154 bajtových blocích.
                * Je zřejmé že 150 bajtů je maximální velikost názvu souboru - tento limit jsem si zvolil. Záleží na kódování sytému kolik je to znaků 
                * - v systémech s Unicode kódováním to bude 75 znaků, v ASCII kodování to je 150 znaků. 
                * Maximální délku názvu souboru jsem nadefinoval nahoře jako konstantu s názvem NameMaxLegth pro případ, že by někomu 150 bajtů nestačilo.
                */
               byte[] buffer = new byte[NameMaxLength+4]; // Čtecí buffer
               for (int i = 0; i < CountFiles; i++) // Čti přesně tolikrát kolik jsme přečetli že je počet souborů v archívu
               {
                   MyArchieve.Read(buffer, 0, NameMaxLength+4);

                   // Sestavíme strukturu FileHeader z přečtených dat
                   FileHeader Header = new FileHeader();

                   // Metoda Encoding.Default.GetString() zajistí převod pole bajtů na řetězec podle kódování systému na kterém program běží.
                   // Je jasné, že toto není nejvhodnější pokud by program používali v různých částech světa - jím vytvořené archívy by nebyly kompatibilní.
                   // Pak by bylo nutné použít nějaké pevné kódování - nejlépe UTF8 (což je Unicode), takže by se volala metoda Encoding.UTF8.GetString()
                   Header.Name = Encoding.Default.GetString(buffer, 0, buffer.Length - 4).TrimEnd('\0'); 
                   // Metoda TrimEnd() oseká z konce daného řetězce netisknutelné znaky - což jsou znaky \0 v našem případě, ty vznikly při zarovnání jména souboru na NameMaxLength při vytváření archívu.
                   
                   // Opět dekóduj poslední 4 bajty z bloku jako velikost souboru
                   Header.Size = BitConverter.ToInt32(buffer, buffer.Length - 4);
                   
                   // Přidej právě vytvořenou strukturu do kolekce FilesIn
                   FilesIn.Add(Header);
               }
               // Po skončení cyklu bude samozřejmě kurzor uvnitř streamu nastaven na první bajt prvního souboru v archívu. Jako pomůcku tuto hodnotu ulož.
               FilesBegin = MyArchieve.Position;
            }
            catch (Exception e) // Zachytni všechno
            {
                // Něco selhalo, zavři stream <- soubor s archívem se prostě nepodařilo přečíst
                MyArchieve.Close();
                MyArchieve = null;
                
                // ..a vyhoď vyjímku s textem o tom co selhalo
                throw new ArchieverException(e.Message);
            }
        }

        /// <summary>
        /// Vlastnost indikující zda je otevřen nějaký archív (i prázdný)
        /// </summary>
        public bool IsOpened { get { return MyArchieve != null; } }

        /// <summary>
        /// Přidá nějaký existující soubor do seznamu k přidání do archívu
        /// </summary>
        /// <param name="PathToFile">Cesta k souboru</param>
        public void AddFile(string PathToFile)
        {
            if (!File.Exists(PathToFile)) throw new ArchieverException("Soubor neexistuje!");
            
            // Nedovol duplicitu názvů souborů
            if (FilesIn.IndexOf(Path.GetFileName(PathToFile)) >= 0 || FilesToAdd.IndexOf(Path.GetFileName(PathToFile)) >= 0)
                throw new ArchieverException("Soubor s takovým jménem už v archívu nebo v seznamu souborů pro přidání existuje!");

            // Tady neděláme nic jiného že přidáváme do kolekce FilesToAdd. Samotné fyzické přidání proběhne až při zavolání SaveArchieve.
            FilesToAdd.Add(PathToFile);
        }

        /// <summary>
        /// Odstraní soubor - rozliší jestli je odstraňovaný soubor už fyzicky v archívu nebo má být teprve přidán po uložení
        /// </summary>
        /// <param name="Name">Název souboru k odstranění</param>
        public void RemoveFile(string Name)
        {
            // Zkus soubor prvně najít a odstranit ze seznamu nových souborů k přidání do archívu
            for (int i = 0; i < FilesToAdd.Count; i++)
            {
                // V kolekci FilesToAdd jsou absolutní cesty k souborů, my chceme však jen jejich názvy s příponou k porovnání
                if (Path.GetFileName(FilesToAdd[i].ToString()) == Name)
                {
                    FilesToAdd.RemoveAt(i); // Odstraň a skonči metodu
                    return;
                }
            }

            // Když tam nebyl, zkus najít a označit ke smazání takový soubor v seznamu souborů v archívu - když tam nebude metoda skončí vyjímkou (protože pomocní metoda FindFileByName skončí vyjímkou)
            FilesToRemove.Add(FindFileByName(Name));
        }

        /// <summary>
        /// Vrátí pole názvů souborů v archívu.
        /// </summary>
        public string[] Names
        {
            get
            {
                // Vytáhni z pole FilesIn pouze jména souborů
                string[] SNames = new string[FilesIn.Count];
                for (int i = 0; i < FilesIn.Count; i++)
                {
                    SNames[i] = ((FileHeader)FilesIn[i]).Name;
                }
                return SNames;
            }
        }

        /// <summary>
        /// Indexer najde soubor s daným názvem v archívu (pokud existuje, jinak vyjímka) a jeho obsah v MemoryStreamu (ten je nutný po skončení práce s ním zavřít)
        /// </summary>
        /// <param name="index">Název souboru</param>
        /// <returns>MemoryStream s obsahem daného souboru</returns>
        public MemoryStream this[string index]
        {
            get {
                // Volání metody FindFileByName zajistí to, že pokud soubor v archívu neexistuje bude vyvolána vyjímka a běh se přeruší
                int FileIndex = FindFileByName(index);
                                
                // Teď musíme spočít o kolik bajtů ještě musíme ve streamu skočit od začátku, abysme se dostali k souboru který chceme            
                long ToJump = FilesBegin;
                for (int i = 0; i < FileIndex; i++)
                    ToJump += (long)((FileHeader)FilesIn[i]).Size; // Možná pro někoho zmatené přetypování, ale toto je zcela běžné

                // ... teď už máme údaj o tom kolik je třeba skočit, tak přeskočíme.
                MyArchieve.Seek(ToJump, SeekOrigin.Begin);

                // Víme i celou velikost souboru, takže nemusíme číst po blocích ale přečteme prostě všechno.
                // Toto obecně není moc preferovaný způsob, protože tak můžou vzniknout obrovská pole bajtů v paměti v případě velkých souborů.
                // Tuto možnost jsem ale zvolil proto, abysme se vyhnuli zbytečné komplikovanosti kódu - chci však upozornit, že toto rozhodně není správný přístup pro jeho neuniverzálnost.
                int TotalFileSize = ((FileHeader)FilesIn[FileIndex]).Size;
                byte[] buffer = new byte[TotalFileSize];
                MyArchieve.Read(buffer, 0, TotalFileSize);

                // Přetížení konstruktoru MemoryStreamu nám umožňuje jej také vytvořit již s nějakými počátečními daty - využijeme toho.
                return new MemoryStream(buffer); // V MemoryStreamu který vrátíme je tedy celý obsah požadovaného souboru
                
                // Nutno podotknout, že ten kdo se rozhodne používat tento indexer, musí MemoryStream z něj získaný i zavřít pomocí Close nebo jej použít v using() bloku
                // - přesně tak jak je to učiněno v metodě ExtractFile
            }
        }

        /// <summary>
        /// Extrahuje z archívu soubor s daným názvem na dané umístění
        /// </summary>
        /// <param name="Name">Název souboru k extrakci</param>
        /// <param name="SaveTo">Cesta kam soubor uložit</param>
        public void ExtractFile(string Name, string SaveTo)
        {
            // Výhodně využijeme našeho již implementovaného indexeru, což naši tuto metodu zjednoduší
            using (MemoryStream MyFile = this[Name])
            {
                // Funkce WriteAllBytes prostě vytvoří daný soubor a vypíše do něj pole bajtů 
                // (to získáme metodou ToArray() která je specificky implementována ve třídě MemoryStream a prostě převede obsah MemoryStreamu do pole bajtů)
                File.WriteAllBytes(SaveTo, MyFile.ToArray());
            }
        }

        /// <summary>
        /// Nejsložitější metoda v celé třídě. Má na starost uložení změn provedených v archívu.
        /// </summary>
        public void SaveArchieve()
        {
            // Metoda nemá smysl, pokud není otevřen nějaký soubor
            if (!IsOpened) throw new ArchieverException("Nepracujete s žádným souborem!");
            
            // Do tohoto MemoryStreamu načteme všechny staré věci které mají v archívu zůstat (nebyly odstraněny) a zároveň do něj přidáme nové věci.
            using (MemoryStream MyMemory = new MemoryStream())
            {

                // Musíme tedy projít všechny soubory které chceme zachovat a "přeuložit" je do streamu MyMemory
                int ToJump = 0, CurrentSize = 0;
                for (int i = 0; i < FilesIn.Count; i++)
                {
                    // CurrentSize používám jen jako pomocnou proměnnou abych pořád nemusel dělat toto nepěkné přetypování
                    CurrentSize = ((FileHeader)FilesIn[i]).Size;

                    // Jesliže je momentální index v poli indexů pro odstranění z archívu, vynecháme jej, ale musíme s ním počítat i při přeskoku pomocí Seek
                    if (FilesToRemove.IndexOf(i) > -1)
                    {
                        ToJump += CurrentSize;
                        continue; // Jak jistě víte, příkaz continue ukončí momentální iteraci, ale ne samotný cyklus.
                    }

                    // Přeskoč na daný soubor
                    MyArchieve.Seek((long)(FilesBegin + ToJump), SeekOrigin.Begin);

                    // Opět se zde vědomě dopouštím stejné chyby jako v indexeru, viz. komentáře v kódu indexeru
                    byte[] buffer = new byte[CurrentSize];
                    MyArchieve.Read(buffer, 0, CurrentSize);
                    MyMemory.Write(buffer, 0, CurrentSize);

                    ToJump += CurrentSize;
                }

                // Teď už můžeme z FilesIn skutečně bezpečně odstranit všechny ty indexy, které jsou v poli FilesToRemove
                foreach (int index in FilesToRemove)
                    FilesIn.RemoveAt(index);

                // Další krok bude přidání nových souborů do archívu...

                byte[] buf = new byte[1024];
                // V BytesRead bude počet bajtů který se přečtl při jedné blokové operaci.V TotalSize nasčítáme počet všech přečtěných bajtů, což je totéž jako velikost souboru
                int BytesRead = 0, TotalSize = 0;
                foreach (string PathToFile in FilesToAdd)
                {
                    try // Zde by mohla nastat spousta špatností, protože pracujeme s filesystémem
                    {
                        // Toto je již velmi známá konstrukce čtení dat po pevných blocích - netřeba dál komentovat.
                        using (FileStream MyNewFile = File.OpenRead(PathToFile))
                        {
                            TotalSize = 0;
                            while ((BytesRead = MyNewFile.Read(buf, 0, 1024)) > 0)
                            {
                                MyMemory.Write(buf, 0, BytesRead);
                                TotalSize += BytesRead;
                            }
                        }
                    }
                    catch (Exception e) // Chytej jakoukoliv vyjímku
                    {
                        // Vyhoď vyjímku ArchieverException s textem původní vyjímky která nastala
                        throw new ArchieverException(e.Message);
                    } // Všimněte si, že díky bloku using() nemusíme používat finally a v něm stream zavírat!

                    // Když všechno prošlo bez chyby, tak vytvoříme pro nový soubor hlavičku a přidáme ji do pole FilesIn
                    FileHeader MyNewHeader = new FileHeader();
                    MyNewHeader.Name = Path.GetFileName(PathToFile);
                    MyNewHeader.Size = TotalSize;
                    FilesIn.Add(MyNewHeader);
                }

                // Vyčistíme kolekce FilesToAdd a FilesToRemove
                FilesToAdd.Clear();
                FilesToRemove.Clear();

                // V posledním kroku prostě přepíšeme původní stream MyArchieve novou hlavičkou a tím co máme v MyMemory streamu
                MyArchieve.Seek(0, SeekOrigin.Begin);
                MyArchieve.SetLength(0); // Toto zničí všechny data ve streamu (zkrátí délku souboru na 0)

                // První čtyři bajty archívu je počet souborů v něm (opět používáme BitConverter)
                MyArchieve.Write(BitConverter.GetBytes(FilesIn.Count), 0, 4);

                // Teď zapíšeme názvy souborů (s ohledem na 150 bajtový limit jeho délky) a hned za něj jeho velikost, celkem tedy 154 bajtů (v případě že někdo nezmění NameMaxLength na jinou hodnotu než 150)
                foreach (FileHeader item in FilesIn)
                {
                    // Toto pole bajtů má přesně takovou velikost jako je délka názvu souboru v kódování systému
                    byte[] bytesOfName = Encoding.Default.GetBytes(item.Name);

                    // Musíme velikost pole bytesOfName zarovnat na NameMaxLength, což je v našem případě 150
                    byte[] normalizedName = new byte[NameMaxLength];
                    
                    // Pozorně prosím prostudujte jak pracuje tento cyklus (kopíruje z pole s názvem souboru do normalizovaného pole od délce NameMaxLength, když je delší - oseká se)
                    for (int i = 0; i < (bytesOfName.Length > NameMaxLength ? NameMaxLength : bytesOfName.Length);i++)
                      normalizedName[i] = bytesOfName[i];

                    MyArchieve.Write(normalizedName, 0,NameMaxLength);
                    MyArchieve.Write(BitConverter.GetBytes(item.Size), 0, 4);
                }

                // ... a úplně nakonec zapíšeme všechny soubory, které již máme připraveny v MyMemory - tam jsou ve stejném pořadí tak jako v hlavičce.
                MyArchieve.Write(MyMemory.ToArray(), 0, (int)MyMemory.Length);

            } // A HOTOVO! (Všimněte si, že celý kód metody byl v postatě "zabalen" v bloku od using(MemoryStream MyMemory ...) takže ať se stalo cokoliv bude vždycky zavřen!
        }

        /// <summary>
        /// Odvozená metoda Dispose od typu IDisposable. Zavře archív. Odvození od IDisposable umožní použít třídu i v bloku using()
        /// </summary>
        public void Dispose()
        {
            // Zavři stream a nastav jej na null (pokud je otevřen)
            if (MyArchieve != null)
            {
                MyArchieve.Close();
                MyArchieve = null;
            }
        }


        /// <summary>
        /// Metoda Close, jen pro lepší přehlednost - prostě zavolá Dispose
        /// </summary>
        public void Close()
        {
            Dispose();
        }

    }

    
    /// <summary>
    /// Takto vypadá formálně správná deklarace vlastní vyjímky. Teoreticky však stačí jen: public class MojeVyjimka : Exception { public MojeVyjimka() { } }
    /// </summary>
    [Serializable]
    public class ArchieverException : Exception
    {
      public ArchieverException() { }
      public ArchieverException( string message ) : base( message ) { }
      public ArchieverException( string message, Exception inner ) : base( message, inner ) { }
      protected ArchieverException( 
	    System.Runtime.Serialization.SerializationInfo info, 
	    System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
    }
}
