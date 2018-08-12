# from bottle import route, run, template, request, static_file
# from Adafruit_MotorHAT import Adafruit_MotorHAT, Adafruit_DCMotor
# import Adafruit_PCA9685
#from Adafruit_BNO055 import BNO055
# import atexit
import socket
import logging
import sys
import time

"""
    Instances of the TCP_CarControl class accept a TCP connection
    and use the Adafruit_PCA9685, Adafruit_MotorHAT, BNO055 to provide
    the connected machine with a means to control the car and receive
    heading information.
"""
class TCP_CarControl:
    """
        The init function for the TCP_CarControl class initializes
        IP address and port number variables for the socket used by
        the class and calls additional initialization functions for 
        the class.
        @param myIP The IP address that the socket for the class will 
        be bound to.
        @param connectionPort The port number that the socket for the class
        will be bound to.
    """
    def __init__(self, myIP = None, connectionPort = None):
        print("Instantiating new CarControlTest")
        # NOTE: Wheel rotations per second and wheel circumference are temporary values
        #       that need to updated when actual values are known
        # self.WHEEL_ROT_S = Number of Wheel Rotations per Second = 1
        self.WHEEL_ROT_S = 4
        # self.WHEEL_CIRC = Circumference of the Car's Wheels = 
        # 2 * PI * Wheel Radius (2.75") = (2.75/12) * 3.14 =  0.72 ft   
        self.WHEEL_CIRC = 0.72
        # Conversion factor for feet per world coordinate unit, 100ft per WC unit
        self.WORLD_FT = 5
        self.sock = None
        # self.bno = None
        # self.pwm = None
        # self.mh = None
        # self.myMotor1 = None
        # self.myMotor2 = None
        self.dcRotSpd = 150
        self.dcTranSpd = 255
        self.dcOneTime = 0
        self.dcTwoTime = 0
        self.heading = 0
        self.isMoving = False
        self.backward = False
        self.connectionOpen = False

        if connectionPort is None:
            self.TCP_PORT = 1030
        else:
            self.TCP_PORT = connectionPort
        if myIP is None:
            # self.TCP_IP = "172.24.1.1"
             self.TCP_IP = "127.0.0.1"
        else:
            self.TCP_IP = myIP
        #self.initHardware()
        self.acceptConnections()
        
    """
        The initHardware function initializes an instance of the BNO55 class, 
        prints status and diagnostic information, initializes an instance
        of the Adafruit_PCA9685 class, and initializes an instance of the 
        Adafruit_MotorHAT class.
    """
    def initHardware(self):
        print("Initializing the BNO")
        self.bno = BNO055.BNO055(serial_port='/dev/serial0')

        # Enable verbose debug logging if -v is passed as a parameter.
        if len(sys.argv) == 2 and sys.argv[1].lower() == '-v':
           logging.basicConfig(level=logging.DEBUG)

        # Initialize the BNO055 and stop if something went wrong.
        if not self.bno.begin():
            raise RuntimeError('Failed to initialize BNO055! Is the sensor connected?')

        # Print system status and self test result.
        status, self_test, error = self.bno.get_system_status()
        print('System status: {0}'.format(status))
        print('Self test result (0x0F is normal): 0x{0:02X}'.format(self_test))
        # Print out an error if system status is in error mode.
        if status == 0x01:
            print('System error: {0}'.format(error))
            print('See datasheet section 4.3.59 for the meaning.')

        # Print BNO055 software revision and other diagnostic data.
        sw, bl, accel, mag, gyro = self.bno.get_revision()
        print('Software version:   {0}'.format(sw))
        print('Bootloader version: {0}'.format(bl))
        print('Accelerometer ID:   0x{0:02X}'.format(accel))
        print('Magnetometer ID:    0x{0:02X}'.format(mag))
        print('Gyroscope ID:       0x{0:02X}\n'.format(gyro))
        
        print("Initializing the PCA9685")
        # Initialize the PCA9685 using the default address (0x40).
        self.pwm = Adafruit_PCA9685.PCA9685()
        self.mh = Adafruit_MotorHAT(addr=0x60)
        self.pwm.set_pwm_freq(60)
        self.myMotor1 = self.mh.getMotor(1)
        self.myMotor2 = self.mh.getMotor(2)
        self.mh.getMotor(1).run(Adafruit_MotorHAT.RELEASE)
        self.mh.getMotor(2).run(Adafruit_MotorHAT.RELEASE)

    """
        The acceptConnections function initializes the socket, binds it to
        the port used for communication, and sets the socket in a listening state.
    """
    def acceptConnections(self):
        print("Initializing the socket with IP: localhost and Port: " + str(self.TCP_PORT))
        if self.sock is None:
            self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.sock.bind(('localhost', self.TCP_PORT))
            self.sock.listen(1)
        self.connectionOpen = True
        while self.connectionOpen:
            print("Listening for connections")
            (clientSock, cAddress) = self.sock.accept()
            self.handleConnections(clientSock, cAddress)

    """
        The handleConnections function iteratively calls the receiveCommand
        and updateHeading functions to service the current connection until 
        an exit command is received and the socket is closed.
        @param cSock The socket used for receiving commands and sending the
        heading.
        @param cAddr The address of the machine providing commands and receiving
        heading data through a TCP connection
    """
    def handleConnections(self, cSock, cAddr):
        # handle the connection by sending heading data and recieving commands
        # Currently the heading is only sent if a command is recieved and the 
        # car has moved, requiring a heading update
        print("Handling the New Connection")
        while self.connectionOpen:
            if(self.isMoving):
                d = self.calculateDistance()
                self.sendDistance(cSock, d)
            if self.receiveCommand(cSock) :
                self.sendHeading(cSock)
        cSock.close()

    """
        The receiveCommand function blocks until a command can be received
        from the socket. When a command is received the appropriate action 
        is taken to control the motors. The function returns a dirty status
        for the car's heading.
        @param cSock The socket used to receive the next command for the car
        @return bool Returns True if a command was received that changes the 
        car's heading.
    """
    def receiveCommand(self, cSock) : 
        #print("Waiting for a Command")
        data = cSock.recv(16) # buffer size is 1024 bytes
        data = data.decode('utf-8')
        
        if data == "F":
            self.setDCMotors(1)
            return True
        elif data == "B":
            self.setDCMotors(2)
            return True
        elif data == "L":
            self.setDCMotors(3)
            self.heading += 2
            return True
        elif data == "R":
            self.setDCMotors(4)
            self.heading -= 2
            return True
        elif data == "S":
            self.setDCMotors(0)
            return False
        elif data == "E":
            print("Closing the connection")
            self.connectionOpen = False
            return True
        # elif data == 'camLeft':
        #     self.pwm.set_pwm(0, 0, 390)
        # elif data == 'camRight': 
        #     self.pwm.set_pwm(0, 0, 570)
        # elif data == 'camHor': 
        #     self.pwm.set_pwm(0, 0, 0)   
        # elif data == 'camDown':
        #     self.pwm.set_pwm(1, 0, 390)
        # elif data == 'camVer': 
        #     self.pwm.set_pwm(1, 0, 0)
        # elif data == 'camUp': 
        #     self.pwm.set_pwm(1, 0, 570)
            
    """
        The setDCMotors function sends appropriate commands to the 
        DC motors according to the parameter. If the parameter is
        less than 0 or greater than 4, then no action is taken.
        @param dcSet A numeric value between 0 and 4 inclusive that
        signals to set the motors to stop, forward, backward, Left,
        and right. 
    """
    def setDCMotors(self, dcSet) :
        if dcSet == 0 :
            pass
            # Stop - Release the motors
            # self.myMotor1.run(Adafruit_MotorHAT.RELEASE)
            # self.myMotor2.run(Adafruit_MotorHAT.RELEASE)
            # self.pwm.set_pwm(1, 0, 0) 
            # self.pwm.set_pwm(0, 0, 0)
            # print("Stop")
        elif dcSet == 1 :
            # Starting motors get the current system time
            self.isMoving = True
            self.dcOneTime = time.time()
            self.dcTwoTime = time.time()
            # self.myMotor1.setSpeed(self.dcTranSpd)
            # self.myMotor1.run(Adafruit_MotorHAT.BACKWARD)
            # self.myMotor2.setSpeed(self.dcTranSpd)
            # self.myMotor2.run(Adafruit_MotorHAT.BACKWARD)
            # print("Forward")
        elif dcSet == 2 :
            self.isMoving = True
            self.backward = True
            self.dcOneTime = time.time()
            self.dcTwoTime = time.time()
            # self.myMotor1.setSpeed(self.dcTranSpd)
            # self.myMotor2.run(Adafruit_MotorHAT.FORWARD)
            # self.myMotor2.setSpeed(self.dcTranSpd)
            # self.myMotor1.run(Adafruit_MotorHAT.FORWARD)
            # print("Backward")
        elif dcSet == 3 :
            pass
            # self.myMotor1.setSpeed(self.dcRotSpd)
            # self.myMotor1.run(Adafruit_MotorHAT.FORWARD)
            # self.myMotor2.setSpeed(self.dcRotSpd)
            # self.myMotor2.run(Adafruit_MotorHAT.BACKWARD)
            # print("Left")
        elif dcSet == 4 :
            pass
             # self.myMotor1.setSpeed(self.dcRotSpd)
            # self.myMotor2.run(Adafruit_MotorHAT.FORWARD)
            # self.myMotor2.setSpeed(self.dcRotSpd)
            # self.myMotor1.run(Adafruit_MotorHAT.BACKWARD) 
            # print("Right")
        else :
            print("setDCMotors: No action taken," +
            " because the parameter is not a valid command")

    """
        The sendHeading function reads the current heading value from the BNO
        and sends the value to the socket.
        @param cSock The socket used for sending the car's current heading as
        reported by the BNO orientation sensor.
    """
    def sendHeading(self, cSock):
        #heading, roll, pitch = self.bno.read_euler()
        # Read the calibration status, 0=uncalibrated and 3=fully calibrated.
        #sys, gyro, accel, mag = self.bno.get_calibration_status()
        # Print everything out.
        # print('Heading={0:0.2F} Roll={1:0.2F} Pitch={2:0.2F}\tSys_cal={3} Gyro_cal={4} Accel_cal={5} Mag_cal={6}'.format(
        #        heading, roll, pitch, sys, gyro, accel, mag))
        # Send data over the socket
        toSend = str(self.heading)
        if len(toSend) > 16 :
            toSend = toSend[:16]
        msHead = str(len(toSend)) + "l"
        message = msHead + toSend
        cSock.sendall(message.encode('utf-8'))

    """
        The calculateDistance function determines the time that the dcMotors
        have been running, calculates the distance traveled, and returns the
        calculated distance
    """
    def calculateDistance(self) :
        # Update the time each motor has been running
        self.dcOneTime = time.time() - self.dcOneTime
        self.dcTwoTime = time.time() - self.dcTwoTime
        self.isMoving = False
        # Calculate the distance traveled
        dist = min(self.dcOneTime, self.dcTwoTime)
        # For Debug
        # print("Motor Time: " + str(dist))
        # End Debug
        dist = dist * self.WHEEL_ROT_S * self.WHEEL_CIRC * self.WORLD_FT
        if(self.backward) :
            dist = dist * -1
            self.backward = False
        # Return the distance
        # print("Calculated Distance: " + str(dist))
        return dist
    

    """
        The sendDistance function sends the given numeric distance
        to the given socket using sendall. A string representation
        of the numberic distance is sent with the length of the string
        preceding it. If the number is more than 15 digits, then it is
        truncated to 15 digits to ensure that is may be parsed as a single
        precision float
                @param cSock The socket that the given data is written to
        @param distance The distance to be written to the given socket
    """
    def sendDistance(self, cSock, distance) :
        # Send out the distance traveled
        toSend = str(distance)
        # print("Sending Distance: " + toSend)
        if len(toSend) > 16 :
            toSend = toSend[:16]
        # print("Post Trim: " + toSend)
        msHead = str(len(toSend)) + "l"
        message = msHead + toSend
        # print("Distance Message: " + message)
        cSock.sendall(message.encode('utf-8'))
    
# Instantiate an instance of the TCP_CarControl class
car = TCP_CarControl()
