# Images and Waves

Desktop MVC application developed in C# and C++. Folder *MMS* contains Visual Studio source code and *MMSSetup* folder contains Setup for the application installation.

The first use of this application is image processing. This part contains:

  - Loading image, saving image
  - Image filters (safe and unsafe code): RGB channels, Sharpen, Gamma
  - Histograms
  - Changing image to 8bit bitmap (256 colors)
  - Undo and redo actions, as well as resetting
  - Downsampling images by RGB channels
  - Compressing and decompressing images by Shannon-Fano algorithm
  - Special effects: Pixelate, Edge enhance

The second use is processing wave files:

  - Loading wave file
  - Sampling wave file
  - Concatenation of wave files

Details and images of each feature can be found in [user guide][guide] in the Serbian language on Github.

## Application

For code implementation of previously mentioned algorithms, C# and C++ were used. For image filters, safe (C#) and unsafe (C++) codes are written, so that comparison in execution speed can be noticed.

Original image and results after applying *Sharpen* filter with 3x3, 5x5 and 7x7 matrices are shown on the image below:

![alt tag](https://imgur.com/n6Lwu5K.png)

*N-Sampling* of wave file with sample value 5 is shown on the next image:

![alt tag](https://imgur.com/BH1VMHO.png)

### Parameters

Users can enter and chose parameters used in algorithm calculations. Mode (safe and unsafe code) can also be chosen to compare results. For different parameters, different results are obtained.

License
----

© 2019 Dragana Vučić, Faculty of Electronic Engineering, University of Niš

[guide]: <https://github.com/draganavucic/Images-and-Waves/blob/master/ImagesAndWaves%20-%20Korisnicko%20uputstvo.pdf>
