import asyncio
import socket
import sys
from bleak import BleakScanner, BleakClient
from bleakheart import HeartRate

# UDP Setup
# Create a UDP socket to send data to Unity
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
unity_ip = "127.0.0.1" # IP address of Unity app (localhost)
unity_port = 5055 # Must match the port Unity listens to

# Global stop flag
stop_flag = False

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
    
async def listen_for_exit():
    """Listen for 'q' input to gracefully quit the script."""
    global stop_flag
    print("Press 'q' then Enter at any time to quit.")
    loop = asyncio.get_event_loop()
    while True:
        user_input = await loop.run_in_executor(None, sys.stdin.readline)
        if user_input.strip().lower() == 'q':
            print("Exiting script...")
            stop_flag = True
            break

async def main():
    """ Main loop that connects to the sensor, listens for HR data, and sends it to Unity.
    Automatically handles reconnection if sensor disconnects """
    global stop_flag
    client = None
    queue = asyncio.Queue()

    # Run exit listener in the background
    asyncio.create_task(listen_for_exit())

    while not stop_flag:
        if client is None or not client.is_connected:
            client = await connect_to_sensor()
            if client is None:
                continue  # Keep retrying

        try:
            hr = HeartRate(client, queue=queue, unpack=True)

            await hr.start_notify()

            print("Receiving heart rate data...")

            while client.is_connected and not stop_flag:
                try:
                    frame = await asyncio.wait_for(queue.get(), timeout=5.0)
                except asyncio.TimeoutError:
                    print("No data received in 5 seconds, assuming disconnect.")
                    break

                if frame[0] != "HR":
                    continue # Skip non-HR frames

                _, _, (bpm, rr), _ = frame
                msg = f"{bpm},{rr}"
                sock.sendto(msg.encode(), (unity_ip, unity_port))
                print(f"Sent HR={bpm}, RR={rr}")

        except Exception as e:
            print(f"Error in data stream: {e}")

        finally:
            try:
                await hr.stop_notify()
            except Exception:
                pass
            await client.disconnect()
            print("Reconnecting in 2 seconds...")
            await asyncio.sleep(2)  # Wait before trying to reconnect

    print("Script ended.")

asyncio.run(main())
