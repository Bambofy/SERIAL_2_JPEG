using System;
using System.IO.Ports;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

/// <summary>
/// # SERIAL 2 JPEG
/// 
/// SERIAL 2 JPEG is a command line tool that saves incoming COM port jpg data to a file.
/// 
/// ## Notes
/// 
/// This tool is case sensitive, therefore, None must have a capital N.etc.
/// 
/// ## Usage
/// 
/// With the SERIAL_2_JPEG.exe in any folder.
/// ./SERIAL_2_JPEG.exe -output=FILENAME.JPG
/// 
/// ## Available Arguments
/// 
///	|	Argument Name	|					Valid Input							|	Optional?	|	Description													|
///	|-------------------|-------------------------------------------------------|---------------|---------------------------------------------------------------|
///	|	-output			|														|	YES			|	Set the file name to name the output file.					|
///	|	-baud_rate		|														|	NO			|	Set the baud rate for the serial connection.				|
///	|	-port_name		|														|	NO			|	Set the serial port object's target port's name.			|                                      
///	|	-parity			|	None,Odd,Even,Mark,Space							|	NO			|	Set the parity of the serial connection.					|                                 
///	|	-data_bits		|														|	YES			|	Set the data bits											|
///	|	-stop_bits		|	None,One,OnePointFive,Two							|	YES			|	Set the stop bits											|
///	|	-handshake		|	None,XOnXOff,RequestToSend,RequestToSendXOnXOff		|	YES			|	Set the handshake											|
/// |	-read_timeout	|	Default: 500										|	YES			|	Sets the read timeout (ms)									|
///	|	-write_timeout	|	Default: 500										|	YES			|	Sets the write timeout (ms)									|
///	
/// </summary>
/// 

namespace SERIAL_2_JPG
{
	class Program
	{
		static void Main(string[] args)
		{
			// the default file name is the date time as a unix timestamp
			string defaultFileName = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() + ".jpg";

			// default settings.
			Dictionary<String, String> configurationSettings = new Dictionary<string, string>();
			configurationSettings.Add("OUTPUT", defaultFileName);


			// find each command in the list of argument given.
			foreach (string arg in args)
			{
				// ensure each argument has a value.
				if (!arg.Contains("="))
				{
					Console.WriteLine("Argument given that does not use the format -ARG=VALUE" + arg);
					Console.ReadLine();
					return;
				}

				// split the argument into name and value.
				string[] argumentParts = arg.Split('=');
				if (argumentParts.Length > 2)
				{
					Console.WriteLine("Argument given that has too many VALUEs: " + arg);
					Console.ReadLine();
					return;
				}
				string argumentName = argumentParts[0];
				string argumentValue = argumentParts[1];



				// convert this input and compare it to the default configuration.
				string uppercaseArgName = argumentName.ToUpper();

				// if it exists in the settings dictionary, overwrite the key's value.
				if (configurationSettings.ContainsKey(uppercaseArgName))
				{
					configurationSettings[uppercaseArgName] = argumentValue;
				}
				else
				{
					Console.WriteLine("Argument given that does not exist: " + arg);
					Console.ReadLine();
					return;
				}
			}

			// start the port reading system.
			SerialPortReader portReader = new SerialPortReader(configurationSettings);

			portReader.Listen();
		}
	}

	class SerialPortReader
	{
		private SerialPort _serialPort;
		private Dictionary<string, string> _configurationSettings;

		public SerialPortReader(Dictionary<string, string> inConfigurationSettings)
		{
			_configurationSettings = inConfigurationSettings;

			Console.WriteLine("Configuring serial port...");

			_serialPort = new SerialPort();

			_serialPort.PortName = inConfigurationSettings["PORT_NAME"];
			_serialPort.BaudRate = Convert.ToInt32(inConfigurationSettings["BAUD_RATE"]);

			Parity parity;
			Enum.TryParse<Parity>(inConfigurationSettings["PARITY"], out parity);
			_serialPort.Parity = parity;

			_serialPort.DataBits = Convert.ToInt32(inConfigurationSettings["DATA_BITS"]);

			StopBits stopBits;
			Enum.TryParse<StopBits>(inConfigurationSettings["STOP_BITS"], out stopBits);
			_serialPort.StopBits = stopBits;

			Handshake handshake;
			Enum.TryParse<Handshake>(inConfigurationSettings["HANDSHAKE"], out handshake);
			_serialPort.Handshake = handshake;

			_serialPort.ReadTimeout = Convert.ToInt32(inConfigurationSettings["READ_TIMEOUT"]);
			_serialPort.WriteTimeout = Convert.ToInt32(inConfigurationSettings["WRITE_TIMEOUT"]);

			Console.WriteLine("Done!");
		}

		public void Listen()
		{
			_serialPort.DataReceived += DataReceived;

			// open serial port.
			_serialPort.Open();

			// we are using events so we can block this thread? https://docs.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.datareceived?view=netframework-4.8
			Console.WriteLine("Waiting to receive data... (press Enter to stop)");
			string userInput = Console.ReadLine();

			_serialPort.Close();
		}

		private void DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			SerialPort serialPort = (SerialPort)sender;

			int bytesToRead = serialPort.BytesToRead;

			byte[] readBuffer = new byte[bytesToRead];

			serialPort.Read(readBuffer, 0, bytesToRead);

			// append it to the file we are creating.
			using (FileStream outputStream = File.Open(_configurationSettings["OUTPUT"], FileMode.Append)) // append mode.
			{
				using (BinaryWriter br = new BinaryWriter(outputStream))
				{
					br.Write(readBuffer, 0, bytesToRead);
				}
			}
		}
	}
}

