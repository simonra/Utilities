import youtube_dl
import argparse

parser = argparse.ArgumentParser()
parser.add_argument("url", help="URL of the webpage with the video you want to save.")
args = parser.parse_args()

print(f'Processing {args.url}')

# link = "https://example.com/video/id"
# yt --format 'bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best'
# --restrict-filenames --prefer-free-formats
# --format 'bestvideo+bestaudio/best'
# youtube-dl --restrict-filenames --prefer-free-formats 'https://example.com/video/id'
# yt "https://example.com/video/id"

units = ['B', 'KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB']
largest_unit = 'YiB'
step = 1024
def bytes_to_human_readable(size):
    for unit in units:
        if size < step:
            # return ('%.' + str(decimals) + 'f %s') % (size, unit)
            return ('%.2f %s') % (size, unit)
        size /= step
    return ('%.2f %s') % (size, largest_unit)

class MyLogger(object):
    def debug(self, msg):
        pass

    def warning(self, msg):
        pass

    def error(self, msg):
        print(msg)

def my_hook(d):
    if d['status'] == 'finished':
        print(f"Download to {d['filename']} completed.")
        print('Done downloading, now converting ...')
    if d['status'] == 'downloading':
        print(f"{d['_percent_str']}\tETA: {d['_eta_str']}\tDownloaded: {bytes_to_human_readable(d['downloaded_bytes'])}\tSpeed: {bytes_to_human_readable(d['speed'])}/s")

        # print(d['filename'], d['_percent_str'], d['_eta_str'][0:5], d['downloaded_bytes'], d['speed'])

ydl_opts = {
    'format': 'bestvideo+bestaudio/best',
    # 'postprocessors': [{
    #     'key': 'FFmpegExtractAudio',
    #     'preferredcodec': 'mp3',
    #     'preferredquality': '192',
    # }],
    'outtmpl': '~/Downloads/%(title)s.%(ext)s',
    'updatetime': False, # Don't trust the sources timestamps, just use now as time. Should be equivalent to the `--no-mtime` option?
    'logger': MyLogger(),
    'progress_hooks': [my_hook],
}
print("Downloading to ~/Downloads/")
with youtube_dl.YoutubeDL(ydl_opts) as ydl:
    ydl.download([f'{args.url}'])
