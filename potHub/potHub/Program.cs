using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RJCP.IO.Ports;
using Newtonsoft.Json;
using System.Net.Http.Headers;

class Program
{
    private SerialPortStream serialPortStream = new SerialPortStream("COM4", 2400);
    private bool statusPompa = false, statusSera = false;
    private int temp = 0, hum = 0;
    private byte type = 0;
    private Pot pot = new Pot();

    static string ToHexString(IEnumerable<byte> bytes) => "0x" + BitConverter.ToString(bytes.ToArray(), 0, bytes.Count());
    static string ToHexByte(Byte number) => "0x" + number.ToString("X2");
    static int ToIntBytes(IEnumerable<byte> bytes) => BitConverter.ToInt16(bytes.ToArray(), 0);
    static byte[] ToArrayBytes(byte b1, byte b2) => new byte[] { b1, b2 };

    static async Task Main()
    {
        Program program = new Program();
        program.StartBackgroundTask();
        await program.ReadFunctionAsync();
    }

    private async Task ReadFunctionAsync()
    {
        serialPortStream.Open();
        bool ok = true;

        while (ok)
        {
            if (Console.KeyAvailable)
            {
                var fromKeyboard = Console.ReadLine();
                if (fromKeyboard == "end")
                {
                    ok = false;
                }
            }

            if (serialPortStream.BytesToRead > 0)
            {
                await Task.Delay(200);
                byte[] data = new byte[serialPortStream.BytesToRead];
                serialPortStream.Read(data, 0, data.Length);
                ProcessData(data);
            }

            await Task.Delay(200);
            HandlePotStatus(pot);
        }

        serialPortStream.Close();
    }

    private void ProcessData(byte[] data)
    {
        if ((data[0] <= 4 || data[0] >= 1) && data.Length >= 2)
        {
            if (data[1] == 15)
            {
                ParseSensorData(data);
            }
            else
            {
                Console.WriteLine("Invalid data header: " + ToHexByte(data[1]));
            }
        }
    }

    private void ParseSensorData(byte[] data)
    {
        switch (data[2])
        {
            case 1 when data.Length == 12:
                Console.WriteLine($"Humidity: {ToHexByte(data[6])} {ToHexByte(data[7])}, Temperature: {ToHexByte(data[8])} {ToHexByte(data[9])}");
                break;
            case 4 when data.Length == 14:
                Console.WriteLine($"NPK - Nitrogen: {ToHexByte(data[6])} {ToHexByte(data[7])}, Phosphor: {ToHexByte(data[8])} {ToHexByte(data[9])}, Potassium: {ToHexByte(data[10])} {ToHexByte(data[11])}");
                break;
            case 3 when data.Length == 10:
                Console.WriteLine($"pH: {ToHexByte(data[6])} {ToHexByte(data[7])}");
                break;
            case 13 when data.Length == 5:
                temp = (data[3] << 8) | data[4];
                Console.WriteLine($"External Temperature: {ToHexByte(data[3])} {ToHexByte(data[4])}");
                _ = PostPotAsync(5, data[0], temp, hum);
                break;
            case 14 when data.Length == 5:
                hum = (data[3] << 8) | data[4];
                Console.WriteLine($"External Humidity: {ToHexByte(data[3])} {ToHexByte(data[4])}");
                _ = PostPotAsync(5, data[0], temp, hum);
                break;
            case 7 when data.Length == 5:
                Console.WriteLine($"Photo Sensor 1: {ToHexByte(data[3])} {ToHexByte(data[4])}");
                break;
            case 8 when data.Length == 5:
                Console.WriteLine($"Photo Sensor 2: {ToHexByte(data[3])} {ToHexByte(data[4])}");
                break;
            case 9 when data.Length == 5:
                Console.WriteLine($"Photo Sensor 3: {ToHexByte(data[3])} {ToHexByte(data[4])}");
                break;
            case 10 when data.Length == 5:
                Console.WriteLine($"Photo Sensor 4: {ToHexByte(data[3])} {ToHexByte(data[4])}");
                break;
            case 5 when data.Length == 5:
                Console.WriteLine($"Potentiometer: {ToHexByte(data[3])} {ToHexByte(data[4])}");
                _ = PostPotAsync(4, 1, (data[3] << 8) | data[4], 0);
                break;
            default:
                Console.WriteLine("Invalid data: " + ToHexString(data));
                break;
        }
    }

    private async Task PostPotAsync(int index, int type, int temp, int hum)
    {
        var pot = new Pot { index = index, type = type, temp = (double)temp / 100, humidity = (float)hum / 100 };

        using (var httpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true }))
        {
            httpClient.BaseAddress = new Uri("http://192.168.201.1:3000/api/Pot");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.PostAsJsonAsync("/pots/", pot);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Pot successfully updated!");
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
    }


    private async Task GetPotAsync(int index, Pot pot)
    {
        using (var httpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true }))
        {
            httpClient.BaseAddress = new Uri("http://192.168.137.185:3000");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.GetAsync($"/pots/{index}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var updatedPot = JsonConvert.DeserializeObject<Pot>(content);
                UpdatePot(pot, updatedPot);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
    }

    private void UpdatePot(Pot original, Pot updated)
    {
        original.index = updated.index;///////////////
        original.type = updated.type;/////////////////////
        original.pompa = updated.pompa;//////////////////
        original.sera = updated.sera;////////////////////
        original.temp = updated.temp;///////////////////        CHANGE THE LOGIC HERE!!!!
        original.humidity = updated.humidity;///////////        TYHE POT WONT CHANGE ITS TYPE PER GET REQ OR THE POT PARAMS ETC.
        original.potassium = updated.potassium;/////////
        original.phosphor = updated.phosphor;///////////
        original.nitrogen = updated.nitrogen;///////////
    }

    private void HandlePotStatus(Pot pot)
    {
        type = (byte)pot.type;

        if (pot.type == 1 || pot.type == 2 || pot.type == 3)
        {
            HandlePumpStatus(pot.pompa);
        }
        else if (pot.type == 4)
        {
            HandleSeraStatus(pot.sera);
        }
    }

    private void HandlePumpStatus(bool activate)
    {
        if (activate && !statusPompa)
        {
            Console.WriteLine("Pump started");
            serialPortStream.WriteByte(3);
            serialPortStream.WriteByte(15);
            serialPortStream.WriteByte(11);
            serialPortStream.WriteByte(1);
            serialPortStream.Write("\n");
            statusPompa = true;
        }
        else if (!activate && statusPompa)
        {
            serialPortStream.WriteByte(3);
            serialPortStream.WriteByte(15);
            serialPortStream.WriteByte(11);
            serialPortStream.WriteByte(0);
            serialPortStream.Write("\n");
            statusPompa = false;
        }
    }

    private void HandleSeraStatus(bool activate)
    {
        if (activate && !statusSera)
        {
            Console.WriteLine("Sera started");
            serialPortStream.WriteByte(4);
            serialPortStream.WriteByte(15);
            serialPortStream.WriteByte(12);
            serialPortStream.WriteByte(1);
            serialPortStream.Write("\n");
            statusSera = true;
        }
        else if (!activate && statusSera)
        {
            serialPortStream.WriteByte(4);
            serialPortStream.WriteByte(15);
            serialPortStream.WriteByte(12);
            serialPortStream.WriteByte(0);
            serialPortStream.Write("\n");
            statusSera = false;
        }
    }

    private void StartBackgroundTask()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                await GetPotAsync(5, pot);
                await Task.Delay(5000); // Refresh every 5 seconds
            }
        });
    }
}

public class Pot
{
    public int index { get; set; }
    public int type { get; set; }
    public bool pompa { get; set; }
    public bool sera { get; set; }
    public double temp { get; set; }
    public double humidity { get; set; }
    public double potassium { get; set; }
    public double phosphor { get; set; }
    public double nitrogen { get; set; }
}
