Windows Powered RC Car
===================
---------
Content
-------------

- [Components](#Components)
- [Hooking up Components](#hooking-up-components)
- [Run it!](#run-it)
- [USB HID Controller Code](#usb-hid-controller-code)
- [Winsock UDP](#winsock-udp)

###Components
- Intel Galileo running Windows off an SD Card
- 5V DC Power Supply (either a wall-powered supply, or a battery pack like [this](http://www.adafruit.com/products/1566) one.)
- A L298N Motor Driver Board ( We've used [this](http://www.ebay.com/itm/like/131207525759?lpid=82) one )
- HID Compliant Controller/Gamepad (We've used an old NES Controller converted to be a HID Device, you can get one [here](http://www.thinkgeek.com/product/1b4c/)
- Microusb Male to USB Female Adapter Cable
- Ethernet Cable
- 4 AA Batteries (or as many batteries as your RC Car needs)
- An RC Car (We've used a Maisito Dune Rover) 
- Male-Female Breadboard Wires

###Hooking Up Components
![alt-text](rccar_bb.png "RC Car")
1. Carefully disassemble your purchased RC Car. Specifically, you want to remove the model chassis so you can get at the controller board.
2. You should be able to clearly see 4 wires (two for each motor, assuming a 2 motor RC Car), and 2 more wires from the battery pack, connected to a PCB which contains the controller for the RC Car.
3. Carefully snip away all the leads connected to that PCB. Make sure you know which wire is connected to what part of your RC Car. 
4. Remove the PCB, in it's place, put the L298N Motor Driver Board.
5. Wire up the motor driver board as follows:
>- Connect the +'ve and -'ve leads of one motor to "Output 1" and "Output 2" of the L298N board.
- Similarily, connect the +'ve and -'ve leads of the other motor to "Output 3" and "Output 4" of the L298N Board.
- Connect the Battery Pack ground line to the ground input of the L298N Board.
- Connect the Ground output of the Galileo to the L298N Board Ground input (we want to tie together the grounds of the Galileo and the RC Car)
- Connect the Battery Pack power line to the +12V input of the L298N Board (don't worry if your battery pack puts out less than 12V)
- Connect Galileo GPIO pins 13,12,11,10 to Inputs 1-4 of the L298N Board 

**Note: Depending on how you've wired everything up, you may need to play around with the order that you connect these wires to have the controls inverted/regular. In any case, the first motor will be controlled by the inputs 1 and 2, and the second by 3 and 4. To turn on a motor, one of each pair should be high (+5V) and the other low (0V). The motor's direction may be the opposite of what you expect depending on how you've wired it up.

Finally, using the USB adapter cable, plug the NES Controller into the Galileo's USB Host port, and power on the Galileo (make sure the RC Car is turned off until the Galileo is completely booted up and the controller application is running).

###Run it!
Using Visual Studio or Telnet, deploy and run the eCar application. This application runs two threads, one waiting for commands over UDP from a host machine, and the other waiting for local commands from a HID Controller. When commands are received, it drives the GPIO outputs which should drive the motors through the L298N board. 

Once the application has started running on the Galileo, turn on the RC Car and try driving it with the NES controller. With any luck, it should be working just fine! If not, check your wiring as discussed in the Note above.

###USB HID Controller Code
TODO

###Winsock - UDP
TODO