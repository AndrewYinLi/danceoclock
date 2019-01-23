# Dance O'clock: movement-deactivated alarm clock using the Microsoft Kinect

## Introduction
College students are perennially exhausted, and getting out of bed is often a painful ordeal involving several sound-based alarms that are frequently snoozed or slept through. To alleviate the struggles faced by students in waking up in the morning, we introduce Dance O’clock, a motion-deactivated alarm clock program that utilizes the motion-capture capabilities of the Microsoft Kinect. Dance O'clock's capabilities include: recording a physical movement routine using the Kinect camera, setting an alarm with a series of user-customized parameters, and deactivation of a set alarm by performing the previously recorded movements. 

Dance O'clock was designed and developed from the holistic perspective of human-computer interaction; the original design was built upon an initial questionnaire of 26 college students around the world, which gauged their needs and feelings towards a movement-deactivated alarm system, and throughout the development process, the design components and functionalities were constantly adjusted accoring to real-time feedback from potential users. 

Dance O'clock was programmed by Andrew Li and Shana Li in C# using Visual Studio and the Microsoft Kinect SDK, along with the Microsoft Kinect 2.0 sensor. This project would not be possible without [Vangos Pterneas' tutorials for developing applications with the Kinect SDK](https://pterneas.com/), or professor [Cynthia Taylor's](https://cs.oberlin.edu/~ctaylor/) unwavering support and guidance.

## How to Use Dance O'clock
Using the Dance O'clock application, you can do the following, after connecting a Microsoft Kinect 2.0 sensor:
1. Open and read the help menu, which has helpful tips and instructions for using the program.
2. Record an action using the Kinect camera, and specify the length of recording, frame sample rate (these are the frames that are saved and matched with the frames that are performed during alarm deactivation), and the file saving options. The saved files contain the angles between limbs at each important body joint, which is captured at the specified sample rate by the Kinect sensor. The Dance O'clock package includes five preset movements.
3. Create and set up a new alarm with a selected recorded action, along with any valid .mp3 file to play as the alarm sound. The Dance O'clock package includes three preset alarm sounds.
4. Modify any of the alarm’s attributes.
5. Wait for the alarm to go off, and then perform the required actions to deactivate the alarm with the visual guidance of a colored skeleton, as well as a different skeleton displaying user movement and indicating incorrect joint angles.

## Future Work
- Waking up the computer from sleep when the alarm activates.
- Spotify integration.
- High-velocity freestyle movement recognition.
- Smoother action recording and detection.
- Improved file management.
- Support for other operating systems.

Feel free to contact us with your suggestions!
