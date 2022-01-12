using UnityEngine;

namespace OperationBlackwell.Puzzles {
	public static class ImageSplicer {
		public static Texture2D[,] SplitImage(Texture2D image, int blocksPerLine) {
			int imageSize = Mathf.Min(image.width, image.height);
			int blockSize = imageSize / blocksPerLine;

			Texture2D[,] blocks = new Texture2D[blocksPerLine, blocksPerLine];

			for(int i = 0; i < blocksPerLine; i++) {
				for(int j = 0; j < blocksPerLine; j++) {
					Texture2D block = new Texture2D(blockSize, blockSize);
					block.wrapMode = TextureWrapMode.Clamp;
					block.SetPixels(image.GetPixels(i * blockSize, j * blockSize, blockSize, blockSize));
					block.Apply();
					blocks[i, j] = block;
				}
			}

			return blocks;
		}
	}
}
