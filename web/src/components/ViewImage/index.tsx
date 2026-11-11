import { NoImageIcon } from '@mms/mms-ui-library/icons/base/NoImageIcon';
import { useMemo, useState } from 'react';

import {
	DiscountImageSkeleton,
	DiscountImageWrapper,
	ImageWrapper,
	NoImageWrapper,
} from '@/components/ViewImage/styles';
import { DiscountImageProps } from '@/components/ViewImage/types';
import { generateImageDataUrl } from '@/helpers/generateImageDataUrl';
import { useGetFile } from '@/queries/useGetFile';
import { ImageFileSize } from '@/types/file';

export function ViewImage({
	imageId,
	title,
	type = 'small',
	onClick,
	styleConfig,
	isResized,
}: DiscountImageProps) {
	const [showSkeleton, setShowSkeleton] = useState(true);
	const [hasImageError, setHasImageError] = useState(false);

	const { data, isLoading } = useGetFile(
		{
			fileId: imageId!,
			imageSize: isResized ? ImageFileSize.Thumbnail : undefined,
		},
		{
			enabled: !!imageId,
		}
	);

	const isSkeletonVisible = isLoading || showSkeleton;

	const handleImageError = () => {
		setShowSkeleton(false);
		setHasImageError(true);
	};

	const imageSource = useMemo(() => {
		if (data && hasImageError) {
			setShowSkeleton(true);
			setHasImageError(false);
		}

		if (data) {
			return generateImageDataUrl(data);
		}
		handleImageError();
	}, [data, hasImageError, isResized]);

	const handleImageLoad = () => {
		setShowSkeleton(false);
	};

	return (
		<ImageWrapper
			isClickable={Boolean(onClick)}
			onClick={onClick}
			styleConfig={styleConfig}
		>
			{hasImageError && !isSkeletonVisible ? (
				<NoImageWrapper styleConfig={styleConfig} type={type}>
					<NoImageIcon />
				</NoImageWrapper>
			) : (
				<DiscountImageSkeleton
					styleConfig={styleConfig}
					visible={isSkeletonVisible}
				>
					<DiscountImageWrapper
						styleConfig={styleConfig}
						src={imageSource}
						onLoad={handleImageLoad}
						onError={handleImageError}
						alt={`${title} discount`}
						type={type}
					/>
				</DiscountImageSkeleton>
			)}
		</ImageWrapper>
	);
}
