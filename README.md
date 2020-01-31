# SERIAL 2 JPEG

SERIAL 2 JPEG is a command line tool that saves incoming COM port jpg data to a file.

## Notes

This tool is case sensitive, therefore, None must have a capital N.etc.

## Usage

With the SERIAL_2_JPEG.exe in any folder.
./SERIAL_2_JPEG.exe -output=FILENAME.JPG

## Available Arguments

|	Argument Name	|					Valid Input							|	Optional?	|	Description													|
|-------------------|-------------------------------------------------------|---------------|---------------------------------------------------------------|
|	-output			|														|	YES			|	Set the file name to name the output file.					|
|	-baud_rate		|														|	NO			|	Set the baud rate for the serial connection.				|
|	-port_name		|														|	NO			|	Set the serial port object's target port's name.			|                                      
|	-parity			|	None,Odd,Even,Mark,Space							|	NO			|	Set the parity of the serial connection.					|                                 
|	-data_bits		|														|	YES			|	Set the data bits											|
|	-stop_bits		|	None,One,OnePointFive,Two							|	YES			|	Set the stop bits											|
|	-handshake		|	None,XOnXOff,RequestToSend,RequestToSendXOnXOff		|	YES			|	Set the handshake											|
|	-read_timeout	|	Default: 500										|	YES			|	Sets the read timeout (ms)									|
|	-write_timeout	|	Default: 500										|	YES			|	Sets the write timeout (ms)									|
