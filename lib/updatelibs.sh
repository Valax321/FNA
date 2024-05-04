#!/bin/sh

# Clones or pulls the latest fnalibs.
# Intended for use with FNA on macOS, iOS, and tvOS.
# Requires git, cmake, and python3 to be installed.
# Written by Caleb Cornett.
# Usage: ./updatelibs.sh

# Clone repos if needed
if [ ! -d "./SDL2/" ]; then
	echo "SDL2 folder not found. Cloning now..."
	git clone --depth 1 --branch release-2.30.3 https://github.com/libsdl-org/SDL.git SDL2

	echo ""
fi

if [ ! -d "./MoltenVK" ]; then
	echo "MoltenVK folder not found. Cloning now..."
	git clone --recursive https://github.com/KhronosGroup/MoltenVK
fi

if [ ! -d "./Vulkan-Headers" ]; then
	echo "Vulkan-Headers folder not found. Cloning now..."
	git clone https://github.com/KhronosGroup/Vulkan-Headers
fi

if [ ! -d "./Vulkan-Loader" ]; then
	echo "Vulkan-Loader folder not found. Cloning now..."
	git clone https://github.com/KhronosGroup/Vulkan-Loader
fi
