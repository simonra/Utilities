import argparse
import os
import numpy as np
import cv2

parser = argparse.ArgumentParser()
parser.add_argument(
	'-ip',
	'--inputPath',
	required=True,
	help='Path to the image file you want to process..')
parser.add_argument(
	'-op',
	'--outputPath',
	required=True,
	help='Path to the file you want the transform written to.')
args = parser.parse_args()

image_file = args.inputPath
assert os.path.exists(image_file), "Could not find anything at " + image_file

destination_path = args.outputPath

# If the folder where we wish to place the output-file doesn't exist, create it:
destination_directory = os.path.abspath(os.path.join(destination_path, os.pardir))
os.makedirs(destination_directory, exist_ok=True)

image = cv2.imread(image_file, 0)
frequency_transform = np.fft.fft2(image)
frequency_transform_shifted = np.fft.fftshift(frequency_transform)
magnitude_spectrum = 20 * np.log(np.abs(frequency_transform_shifted))

cv2.imwrite(destination_path, magnitude_spectrum)

# Code for displaying the image and its magnitude spectrum side by side.
# Superslow compared to just writing the image and viewin it with a regular image viewer.

# from matplotlib import pyplot as pplot
# pplot.subplot(121),pplot.imshow(image, cmap = 'gray')
# pplot.title('Input image'), pplot.xticks([]), pplot.yticks([])
# pplot.subplot(122),pplot.imshow(magnitude_spectrum, cmap= 'gray')
# pplot.title('Magnitude Spectrum'), pplot.xticks([]), pplot.yticks([])
# pplot.show()
