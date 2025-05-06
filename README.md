# **Heart Rate Platformer**

This Unity project is a 2D platformer that dynamically adjusts gameplay based on a player's heart rate (HR) in real-time or simulated RR interval (time between heart rates) data. It offers an engaging gameplay experience by making the difficulty responsive to the player's physiological state.

**Features**
- **Real-Time Heart Rate Integration**
  
  Uses a Polar H10 heart rate monitor and the bleakheart Python driver to receive live HR and RR data via Bluetooth and send it to Unity over UDP
- **Simulated Mode for Testing**
  
  Allows players without a heart rate sensor to select from predefined RR interval datasets from healthy individuals
- **Adaptive Platform Generatiom**
  
  Platform height, spacing, and width adjust dynamically based on heart rate and RR intervals
- **Live Heart Rate UI**
  
  Displays the player's current heart rate during gameplay
- **Responsive Time Scaling**
  
  The game speed increases slightly with higher heart rates
- **Level Completion System**
  
  For simulated mode, the game detects when the RR dataset ends and shows a level completion screen
- **Score Tracking**
  
  Records and stores the high score locally. Score increases based on survival time. This is only applicable for sensor mode
- **Polished UX**
  
  Has a pause menu, game over screen, and main menu.

---

## **Known Limitations**

- Not all levels are currently able to be completed, some files with extra high heart rates make platforms spawn too high currently which the player cannot jump
- Level length is also currently too long, the Level complete panel shows when a level is completed but the files are very long and it may get boring
- Level progression is yet to be set up, like completing one level will unlock the next for example
- Player animation sometimes bugs when player lands on a platform
- Has only been tested with the Polar H10 heart rate monitor

---

## **How It Works**

**Python Sensor Script**

The Python script:
- Uses `bleak` and `bleakheart` to connect to the Polar H10 Heart Rate Sensor
- Extracts and decodes HR and RR data
- Sends the data to Unity over a UDP socket

**Unity Game**
- Listens on port 5055 for HR and RR data
- Updates gameplay elements based on incoming data
- Supports two modes: real sensor and simulated data

---

## **Requirements**

**Software**
- Unity (Recommended Unity 2021.3 or later)
- Python 3.8+
  - `bleak`
  - `bleakheart`

**Hardware**
- Polar H10 Heart Rate Sensor
- Windows, Mac, or Linux

---

## **Getting Started**

1. **Clone the repository**
   
   `git clone https://github.com/gabrielleGadjakaeva/FYPHeartRatePlatformer.git`

2. **Install Python 3.8** by going on [Python.org](https://www.python.org/downloads/release/python-3810/)

3. **Install Python Dependencies**
   
   Install dependencies using pip:
   
   `pip install bleak bleakheart`

5. **Run Python Script found in PythonScript folder (can be skipped if no sensor)**

   `python send_hr.py`

6. **Open Builds folder and navigate to your operating system**
   
   - Download the folder corresponding to your operating system (either MacOS or Windows)
   - Open executable file inside to run the game

   Alternatively make your own build (Linux and other operating systems):

   - Open Unity project in Unity Hub
   - Navigate to File -> Build settings
   - Choose your operating system and press Build
   - Then open exe file saved on your computer

---

## **References**

- Python script made using the bleakheart driver from `https://github.com/fsmeraldi/bleakheart.git`
- RR interval data for simulated mode taken from [PhysioNet](https://physionet.org/content/rr-interval-healthy-subjects/1.0.0/)
- Player Sprite animation made by Grace `grace03ma@gmail.com`

---

Made by Gabrielle Gadjakaeva

For any inquiries contact gabrielle.gadjakaeva@gmail.com
