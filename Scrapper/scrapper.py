from bs4 import BeautifulSoup
import requests
import os
import re
import json

CACHE_PATH = "./cache/"


def get_page_content(url, update_cache=False):
    # Extract the filename from the URL
    filename = url.split("/")[-1]

    # Create the cache directory if it doesn't exist
    os.makedirs(CACHE_PATH, exist_ok=True)

    # Check if the file already exists in the cache
    file_path = os.path.join(CACHE_PATH, filename)

    if not os.path.exists(file_path) or update_cache:
        # File doesn't exist in the cache or update_cache is True, so download it
        response = requests.get(url)

        if response.status_code == 200:
            # Save the content to the cache
            with open(file_path, 'wb') as file:
                file.write(response.content)
            print(f"Downloaded and cached: {url}")
            return response.content.decode('utf-8')
        else:
            print(f"Failed to download: {url}")
            return None
    else:
        # File already exists in the cache, so read and return its content
        with open(file_path, 'r', encoding='utf-8') as file:
            content = file.read()
        print(f"Loaded from cache: {url}")
        return content


def get_distance(orig, dest):
    content = get_page_content(
        f"https://www.airportdistancecalculator.com/flight-{orig}-to-{dest}.html")

    soup = BeautifulSoup(content, "html.parser")

    script = soup.find("script", {"type": "application/ld+json"})

    json_text = script.string.strip()

    data = json.loads(json_text)

    distance_info = data["mainEntity"][0]["acceptedAnswer"]["text"]

    pattern = re.compile(r"\d* kilometers")

    km = re.findall(pattern, distance_info)[0].split(" ")[0]

    return int(km)


def get_data(airports):
    done = []
    distances = {}
    for a1 in airports:
        for a2 in airports:
            if a1 != a2 and ((a1, a2) not in done and (a2, a1) not in done):
                distances[(a1, a2)] = get_distance(a1, a2)
                done.append((a1, a2))

    return distances


def save_data(airports):
    distances = get_data(airports)
    with open("flight_distance.csv", "w") as f:
        f.write("airports,distance\n")
        for d in distances.keys():
            f.write(f"{d[0]}-{d[1]},{distances[d]}\n")


if __name__ == "__main__":
    airports = ["mad", "dxb", "sfo", "cdg", "pvg"]
    save_data(airports)
