using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using RJCP.IO.Ports;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
class Program
{
    SerialPortStream serialPortStream = new SerialPortStream("COM4", 2400);
    static string ToHexString(IEnumerable<byte> bytes) => "0x" + BitConverter.ToString(bytes.ToArray(), 0, bytes.ToArray().Length);
    static string ToHexByte(Byte number) => "0x" + number.ToString("X2");
    static int ToIntBytes(IEnumerable<byte> bytes) => BitConverter.ToInt16(bytes.ToArray(), 0);
    byte type = 0;
    static byte[] ToArrayBytes(byte b1, byte b2)
    {
        byte[] aux = { 0, 0 };
        aux[0] = b1;
        aux[1] = b2;
        return aux;
    }
    static byte[] deleteFirst(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length - 1; i++)
        {
            bytes[i] = bytes[i + 1];
        }
        return bytes;
    }
    async Task readFunctionAsync()
    {
        serialPortStream.Open();
        bool ok = true,statusPompa=false,statusSera=false;
        int temp=0, hum=0;
        var byteItems = new byte[10];
        Pot pot=new Pot();
        while (ok)
        {
            //string line = serialPortStream.ReadLine();
            // Console.WriteLine(line);
            // var fromKeyboard = Console.ReadLine();
            // if (fromKeyboard == null)
            //   serialPortStream.Write(fromKeyboard);
            // await Task.Delay(1000);
            // while ( Console.ReadKey().Key != ConsoleKey.A &&serialPortStream.ReadLine() != null )
            //{
            //  Console.WriteLine(serialPortStream.ReadLine());
            //await Task.Delay(1000);
            //}
            //Console.WriteLine("am iesit");
            int i = 0;
            var array = new int[10];
            if (Console.KeyAvailable == true)
            {
                var fromKeyboard = Console.ReadLine();
                if (fromKeyboard == "end")
                { ok = false; }
                if (fromKeyboard == "openp")
                {
                    serialPortStream.WriteByte(15);
                    serialPortStream.WriteByte(11);
                    serialPortStream.WriteByte(1);
                }
                if (fromKeyboard == "closep")
                {
                    serialPortStream.WriteByte(15);
                    serialPortStream.WriteByte(11);
                    serialPortStream.WriteByte(0);
                }
                if (fromKeyboard == "openg")
                {
                    serialPortStream.WriteByte(15);
                    serialPortStream.WriteByte(12);
                    serialPortStream.WriteByte(1);
                    serialPortStream.WriteByte(10);
                }
                if (fromKeyboard == "closeg")
                {
                    serialPortStream.WriteByte(15);
                    serialPortStream.WriteByte(12);
                    serialPortStream.WriteByte(0);
                    serialPortStream.WriteByte(10);
                }
                if (fromKeyboard == "1")
                {
                    serialPortStream.WriteByte(15);
                    //  Thread.Sleep(1);
                    serialPortStream.WriteByte(12);

                    //Thread.Sleep(1);
                    serialPortStream.WriteByte(1);
                    // Thread.Sleep(1);
                    serialPortStream.WriteByte(10);
                }
                else if (fromKeyboard == "0")
                {
                    serialPortStream.WriteByte(15);
                    serialPortStream.WriteByte(12);
                    serialPortStream.WriteByte(0);
                    serialPortStream.WriteByte(10);
                }
                // serialPortStream.Write(fromKeyboard);

                // serialPortStream.WriteByte(10);

            }
            if (serialPortStream.BytesToRead > 0)
            {
                await Task.Delay(200);
                byte[] data = new byte[serialPortStream.BytesToRead];
                //int aux = serialPortStream.ReadByte();
                serialPortStream.Read(data, 0, data.Length);
                if ((data[0] <= 4 || data[0] >= 1)&&data.Length>=2)
                {
                    if (data[1] == 15)
                    {
                        //data=deleteFirst(data);
                        Console.WriteLine("got data ");
                        if (data[2] == 1 && data.Length == 12)
                        {
                            //data= deleteFirst(data);
                            //  Console.WriteLine(ToHexString(data));
                            Console.WriteLine("hum " + ToHexByte(data[6]) + " " + ToHexByte(data[7]) + " temp " + ToHexByte(data[8]) + " " + ToHexByte(data[9]));
                            // Console.WriteLine(ToHexString(data));
                        }
                        else if (data[2] == 4 && data.Length == 14)
                        { Console.Write("npk "); Console.WriteLine("nitrogen " + ToHexByte(data[6]) + " " + ToHexByte(data[7]) + " phosophor " + ToHexByte(data[8]) + " " + ToHexByte(data[9]) + " potassium " + ToHexByte(data[10]) + " " + ToHexByte(data[11])); }
                        else if (data[2] == 3 && data.Length == 10)
                        { Console.Write("pH "); Console.WriteLine(ToHexByte(data[6]) + " " + ToHexByte(data[7])); }
                        else if (data[2] == 13 && data.Length == 5)
                        {
                            Console.Write("temp ext "); Console.WriteLine(ToHexByte(data[3]) + " " + ToHexByte(data[4]));
                            temp = (data[3] << 8) | data[4];
                            await PutPotAsync(5, data[0], (data[3] << 8) | data[4], hum);//poate se deregleaza timpurile de citire!!

                        }
                        else if (data[2] == 14 && data.Length == 5)
                        {
                            Console.Write("hum ext "); Console.WriteLine(ToHexByte(data[3]) + " " + ToHexByte(data[4]));
                            hum = (data[3] << 8) | data[4];
                            await PutPotAsync(5, data[0],  temp,(data[3] << 8) | data[4]);//poate se deregleaza timpurile de citire!!
                        }
                        else if (data[2] == 7 && data.Length == 5)
                        {
                            Console.Write("photo sensor 1 "); Console.WriteLine(ToHexByte(data[3]) + " " + ToHexByte(data[4]));
                        }
                        else if (data[2] == 8 && data.Length == 5)
                        {
                            Console.Write("photo sensor 2 "); Console.WriteLine(ToHexByte(data[3]) + " " + ToHexByte(data[4]));
                        }
                        else if (data[2] == 9 && data.Length == 5)
                        {
                            Console.Write("photo sensor 3 "); Console.WriteLine(ToHexByte(data[3]) + " " + ToHexByte(data[4]));
                        }
                        else if (data[2] == 10 && data.Length == 5)
                        {
                            Console.Write("photo sensor 4 "); Console.WriteLine(ToHexByte(data[3]) + " " + ToHexByte(data[4]));
                        }
                        else if (data[2] == 5 && data.Length == 5)
                        {
                            Console.Write("potentiometru "); Console.WriteLine(ToHexByte(data[3]) + " " + ToHexByte(data[4]));
                            await PutPotAsync(4, 1, (data[3] << 8) | data[4],0);//poate se deregleaza timpurile de citire!!
                        }
                        else
                        {
                            Console.WriteLine("bad data");
                            Console.WriteLine(ToHexString(data) + " " + data.Length + " " + data[1]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("bad data");
                        Console.WriteLine(ToHexByte(data[1]));
                        // Console.WriteLine(ToHexString(data)+" "+data.Length+" " + data[1]);
                    }
                }

            }
            await Task.Delay(200);

            await GetPotAsync(5, pot);
             type = (byte)pot.type;
            if (pot.type == 1||pot.type==2||pot.type==3)
            {
                //Console.WriteLine("am intrat in type =1");
                if (pot.pompa)
                {
                    if (!statusPompa)
                    {
                        
                        Console.WriteLine("pornit");
                        serialPortStream.WriteByte(3);
                        serialPortStream.WriteByte(15);
                        serialPortStream.WriteByte(11);
                        serialPortStream.WriteByte(1);
                        serialPortStream.Write("\n");
                        statusPompa = true;

                    }

                }
                else
                {
                    if(statusPompa)
                    {
                        serialPortStream.WriteByte(3);
                        serialPortStream.WriteByte(15);
                        serialPortStream.WriteByte(11);
                        serialPortStream.WriteByte(0);
                        serialPortStream.Write("\n");
                        statusPompa = false;
                    }
                    
                }
            }
            else if(pot.type == 4) {
                if (pot.sera)
                {
                    if (!statusSera)
                    {

                        Console.WriteLine("pornit");
                        serialPortStream.WriteByte(4);
                        serialPortStream.WriteByte(15);
                        serialPortStream.WriteByte(12);
                        serialPortStream.WriteByte(1);
                        serialPortStream.Write("\n");
                        statusSera = true;

                    }

                }
                else
                {
                    if (statusSera)
                    {
                        serialPortStream.WriteByte(4);
                        serialPortStream.WriteByte(15);
                        serialPortStream.WriteByte(12);
                        serialPortStream.WriteByte(0);
                        serialPortStream.Write("\n");
                        statusSera = false;
                    }

                }
            }


        }
        serialPortStream.Close();
    }
     static async Task Main()
    {
        Program program = new Program();
        await program.readFunctionAsync();

    }

    static async Task PutPotAsync(int index,int type,int temp,int hum)
    {
        var pot = new Pot { index = index, type = type ,temp =(double)temp/100,hum=(float)hum/100 };

        using (var httpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true }))
        {
            // Set the base address of your API
            httpClient.BaseAddress = new Uri("http://192.168.137.185:3000");

            // Set the content type to JSON
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // Send a POST request with the pot data in the request body
            var response = await httpClient.PutAsJsonAsync("/pots/", pot);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Pot successfully added!");
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
    }
    static async Task GetPotAsync(int index,Pot pot)
    {
        // pot = new Pot ();

        using (var httpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true }))
        {
            // Set the base address of your API
            httpClient.BaseAddress = new Uri("http://192.168.137.185:3000");

            // Set the content type to JSON
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // Send a POST request with the pot data in the request body
            var response = await httpClient.GetAsync($"/pots/{index}");

            if (response.IsSuccessStatusCode)
            {
                // Read the content and convert it to Pot object
                var content = await response.Content.ReadAsStringAsync();
                var updatedPot = JsonConvert.DeserializeObject<Pot>(content);

                // Update the properties of the 'pot' object
                pot.index = updatedPot.index;
                pot.type = updatedPot.type;
                pot.pompa = updatedPot.pompa;
                pot.sera = updatedPot.sera;
                pot.temp = updatedPot.temp;
                pot.humidity = updatedPot.humidity;
                pot.potassium = updatedPot.potassium;
                pot.phosphor = updatedPot.phosphor;
                pot.nitrogen = updatedPot.nitrogen;
                //Console.WriteLine($"Received Pot: Index={pot.index}, Type={pot.type}, Temp={pot.temp},pompa={pot.pompa}");
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
    }
}
int index, int type,bool pompa,bool sera, double temp, double humidity, double potassium, double phosphor, double nitrogen
public class Pot
{
    public int index { get; set; }
    public int type { get; set; }
    
    public bool pompa {  get; set; }
    public bool sera { get; set; }
    public double temp { get; set; }
    public double humidity { get; set; }
    public double potassium { get; set; }
    public double phosphor { get; set; }
    public double nitrogen { get; set; }

}