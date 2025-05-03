import asyncio
import socket
from bleak import BleakScanner, BleakClient
from bleakheart import HeartRate

# UDP Setup
# Create a UDP socket to send data to Unity
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
unity_ip = "127.0.0.1" # IP address of Unity app (localhost)
unity_port = 5055 # Must match the port Unity listens to

async def connect_to_sensor():
    """ Try to find and connect to the Polar H10 """
    print("Scanning for Polar H10...")
    device = await BleakScanner.find_device_by_filter(
        lambda dev, adv: dev.name and "polar" in dev.name.lower()
    )
    if device is None:
        print("No Polar device found. Retrying in 5 seconds...")
        await asyncio.sleep(5)
        return None

    client = BleakClient(device)
    try:
        await client.connect()
        if client.is_connected:
            print(f"Connected to {device.name}")
            return client
        else:
            print("Failed to connect. Retrying...")
            return None
    except Exception as e:
        print(f"Connection error: {e}")
        return None

async def main():
    """ Main loop that connects to the sensor, listens for HR data, and sends it to Unity.
    Automatically handles reconnection if sensor disconnects """
    client = None

    while True:
        if client is None or not client.is_connected:
            client = await connect_to_sensor()
            if client is None:
                continue  # Keep retrying

        try:
            hr = HeartRate(client, unpack=True)
            await hr.start_notify()

            print("Receiving heart rate data...")

            while client.is_connected:
                frame = await hr.queue.get()

                if frame[0] != "HR":
                    continue # Skip non-HR frames

                _, _, (bpm, rr), _ = frame
                msg = f"{bpm},{rr}"
                sock.sendto(msg.encode(), (unity_ip, unity_port))
                print(f"Sent HR={bpm}, RR={rr}")

        except Exception as e:
            print(f"Error in data stream: {e}")
            await asyncio.sleep(2)  # Wait before trying to reconnect

asyncio.run(main())
