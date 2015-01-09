

# ImagingSDK: 
## Blur params:
- 1-256, kernelSize
- Rect, region
- BlurRegionShape (Exliptical | Rectangular)

Strongly prefer Bitmap over WriteableBitmap. There are memory/garbage collector issues with WriteableBitmap, whereas the memory allocated by Bitmap is managed in native code within the Imaging SDK


# WritableBitmapEx:
## Blur params (Convolute):
- kernel, (KernelGaussianBlur3x3 | KernelGaussianBlur5x5)
- int, kernelFactorSum
- int, kernelOffsetSum