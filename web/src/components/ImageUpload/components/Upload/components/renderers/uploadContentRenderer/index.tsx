import { Skeleton } from '@mms/mms-ui-library/components/Skeleton';
import { NoImageIcon } from '@mms/mms-ui-library/icons/base/NoImageIcon';

import {
	ImageWrapper,
	NoImageWrapper,
} from '@/components/ImageUpload/components/Upload/styles';
import { UploadContentRendererProps } from '@/components/ImageUpload/components/Upload/types';

export const uploadContentRenderer = ({
	file,
	hasImageError,
	iconSize,
	showSkeleton,
	uploadRadius,
	handleImageLoad,
	handleImageError,
	imageSource,
	alt,
}: UploadContentRendererProps) => {
	if (file) {
		if (hasImageError) {
			return (
				<NoImageWrapper>
					<NoImageIcon width={iconSize} height={iconSize} />
				</NoImageWrapper>
			);
		}

		return (
			<Skeleton visible={showSkeleton}>
				<ImageWrapper
					src={imageSource}
					alt={alt}
					onLoad={handleImageLoad}
					onError={handleImageError}
					uploadRadius={uploadRadius}
				/>
			</Skeleton>
		);
	}

	return null;
};
