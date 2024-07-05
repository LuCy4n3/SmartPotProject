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
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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
        program.StartUpFucntion();
        program.StartBackgroundTask();
        await program.ReadFunctionAsync();
    }
    private async void StartUpFucntion()
    {
        serialPortStream.Open();
        bool ok = false;
        Console.WriteLine("Looking for data.");
        while (!ok)
        {

            if (serialPortStream.BytesToRead > 0)
            {
                Console.WriteLine("Got data.");
                byte[] data = new byte[serialPortStream.BytesToRead];
                serialPortStream.Read(data, 0, data.Length);
                ok = GetIndex(data);
            }
        }
        serialPortStream.Close();
        await GetPotAsync(pot.PotId, pot);
    }
    private bool GetIndex(byte[] data)
    {
        if (data[0] == 15)
        {
            pot.PotId = data[1];
            pot.PotType = data[2];
            Console.WriteLine("Got the index.");
            return true;
        }
        else
        {
            Console.WriteLine("Bad data. Retrying to get index...");
            return false;
        }
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
        if ((data[0] == 15) && data.Length >= 3)
        {
            if (data[1] >= 1 && data[1] <= 2 )
                ParseSensorData(data);
            else
                Console.WriteLine("Invalid type header: " + ToHexByte(data[1]));
        }
        else
        {
            Console.WriteLine("Invalid data header: " + ToHexByte(data[0]));
        }
    }

    private void ParseSensorData(byte[] data)
    {
        switch (data[3])
        {
            case 10 when data.Length == 12:
                Console.WriteLine($"Humidity: {ToHexByte(data[6])} {ToHexByte(data[7])}, Temperature: {ToHexByte(data[8])} {ToHexByte(data[9])}");
                break;
            case 40 when data.Length == 14:
                Console.WriteLine($"NPK - Nitrogen: {ToHexByte(data[6])} {ToHexByte(data[7])}, Phosphor: {ToHexByte(data[8])} {ToHexByte(data[9])}, Potassium: {ToHexByte(data[10])} {ToHexByte(data[11])}");
                break;
            case 3 when data.Length == 10:
                Console.WriteLine($"pH: {ToHexByte(data[6])} {ToHexByte(data[7])}");
                break;
            case 1 when data.Length == 6:
                temp = (data[5]<<8) | data[4];
                Console.WriteLine($"External Temperature: {ToHexByte(data[4])} {ToHexByte(data[5])} . And real temp is {temp}");
                _ = PostPutAsync(temp, hum, pot);
                break;
            case 2 when data.Length == 6:
                hum = (data[5] << 8) | data[4];
                Console.WriteLine($"External Humidity: {ToHexByte(data[4])} {ToHexByte(data[5])}. And real hum is {hum}");
                _ = PostPutAsync( temp, hum, pot);
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
                _ = PostPutAsync((data[3] << 8) | data[4], 0,pot);
                break;
            default:
                Console.WriteLine("Invalid data: " + ToHexString(data));
                break;
        }
    }

    private async Task PostPutAsync(int temp, int hum, Pot potMain)
    {
        Console.WriteLine("Trying to POST ....");
        var pot = new Pot { PotId = potMain.PotId, UserId = 1, PlantName = potMain.PlantName, PotName = potMain.PlantName, PotType = potMain.PotType, GreenHouseTemperature = (double)temp / 100, GreenHouseHumidity = (double)hum / 100 };

        using (var httpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true }))
        {
            httpClient.BaseAddress = new Uri("http://192.168.201.1:3000");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.PutAsJsonAsync("/api/Pot/1/1", pot);

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
        Console.WriteLine("Trying to GET ...");
        using (var httpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true }))
        {
            httpClient.BaseAddress = new Uri("http://192.168.201.1:3000");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.GetAsync($"/api/Pot/1/{index}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Succesful GET req, updating Pot!");
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    var updatedPot = JsonConvert.DeserializeObject<Pot>(content);
                    UpdatePot(pot, updatedPot);
                }
                catch
                {
                    Console.WriteLine("Error on updating pot. this is the response: "+content);
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
    }

    private void UpdatePot(Pot original, Pot updated)
    {
        original.PotName = updated.PotName; // Update pot name
        original.PlantName = updated.PlantName; // Update plant name
        original.PumpStatus = updated.PumpStatus; // Update pump status
        original.GreenHouseStatus = updated.GreenHouseStatus; // Update greenhouse status
        //original.GreenHouseTemperature = updated.GreenHouseTemperature; // Update greenhouse temperature
       // original.GreenHouseHumidity = updated.GreenHouseHumidity; // Update greenhouse humidity
       // original.GreenHousePressure = updated.GreenHousePressure; // Update greenhouse pressure
        //original.PotPotassium = updated.PotPotassium; // Update potassium level
      //  original.PotPhospor = updated.PotPhospor; // Update phosphor level
       // original.PotNitrogen = updated.PotNitrogen; // Update nitrogen level
    }

    private void HandlePotStatus(Pot pot)
    {
        type = (byte)pot.PotType;

        if (pot.PotType == 1 || pot.PotType == 2 || pot.PotType == 3)
        {
            HandlePumpStatus(pot.PumpStatus);
        }
        else if (pot.PotType == 4)
        {
            HandleSeraStatus(pot.GreenHouseStatus);
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
                await GetPotAsync(pot.PotId, pot);
                await Task.Delay(5000); // Refresh every 5 seconds
            }
        });
    }
}

public class Pot
{
    public int PotId { get; set; }
    public string PotName { get; set; }
    public int PotType { get; set; }
    public string PlantName { get; set; }
    public int UserId { get; set; }

    public bool PumpStatus { get; set; }
    public bool GreenHouseStatus { get; set; }
    public double GreenHouseTemperature { get; set; }
    public double GreenHouseHumidity { get; set; }
    public double GreenHousePressure { get; set; }
    public double PotPotassium { get; set; }
    public double PotPhospor { get; set; }
    public double PotNitrogen { get; set; }
}
