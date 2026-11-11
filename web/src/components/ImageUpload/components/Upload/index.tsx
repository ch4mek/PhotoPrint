import { Hint } from '@mms/mms-ui-library/components/Hint';
import { ImageIcon } from '@mms/mms-ui-library/icons/editor/ImageIcon';
import { MouseEvent, useMemo, useState } from 'react';

import { uploadContentRenderer } from '@/components/ImageUpload/components/Upload/components/renderers/uploadContentRenderer';
import { UPLOAD_HINT_BREAKPOINT } from '@/components/ImageUpload/components/Upload/constants';
import {
	NoImageWrapper,
	UploadWrapper,
} from '@/components/ImageUpload/components/Upload/styles';
import { UploadProps } from '@/components/ImageUpload/components/Upload/types';
import { ProfileNotification } from '@/components/ProfileAvatar/components/ProfileNotification';
import { DEFAULT_MAXIMUM_ALLOWED_IMAGE_SIZE_KB } from '@/constants/allowedFileSizes';
import { imageValidExtensions } from '@/constants/fileExtensions';
import { convertKBToMB } from '@/helpers/convertFileSize';
import { formatAllowedFileTypesToShow } from '@/helpers/formatAllowedFileTypes';
import { useResize } from '@/hooks/useResize';

export function Upload({
	file,
	alt,
	size,
	uploadRadius,
	handleUploadClick,
	setIsHintOpen,
	isHintOpen,
	disabled,
	ref,
}: UploadProps) {
	const [showSkeleton, setShowSkeleton] = useState(true);
	const [hasImageError, setHasImageError] = useState(false);
	const [innerWidth] = useResize();

	const noImageIconSize = size / 2;
	// eslint-disable-next-line react/destructuring-assignment
	const anchorElement = typeof ref === 'function' || !ref ? null : ref.current;

	const handleUploadHover = ({ type }: MouseEvent) => {
		setIsHintOpen(type === 'mouseenter');
	};

	const hintPosition = useMemo(() => {
		if (innerWidth < UPLOAD_HINT_BREAKPOINT) {
			return 'bottom-start';
		}

		return 'right';
	}, [innerWidth]);

	const imageSource = useMemo(() => {
		if (file && hasImageError) {
			setShowSkeleton(true);
			setHasImageError(false);
		}

		return file || '';
	}, [file, hasImageError]);

	const handleImageLoad = () => {
		setShowSkeleton(false);
	};

	const handleImageError = () => {
		setShowSkeleton(false);
		setHasImageError(true);
	};

	const imageContent = useMemo(
		() =>
			uploadContentRenderer({
				file,
				hasImageError,
				iconSize: noImageIconSize,
				showSkeleton,
				uploadRadius,
				handleImageError,
				handleImageLoad,
				imageSource,
				alt,
			}),
		[
			file,
			hasImageError,
			noImageIconSize,
			showSkeleton,
			uploadRadius,
			imageSource,
			alt,
		]
	);

	return (
		<UploadWrapper size={size} uploadRadius={uploadRadius} ref={ref}>
			{imageContent || (
				<NoImageWrapper
					onClick={handleUploadClick}
					onMouseEnter={handleUploadHover}
					onMouseLeave={handleUploadHover}
					{...{ disabled }}
				>
					<ImageIcon width={noImageIconSize} height={noImageIconSize} />
				</NoImageWrapper>
			)}

			<Hint
				open={isHintOpen}
				anchorElement={anchorElement}
				position={hintPosition}
				text={
					<ProfileNotification
						allowedFormats={formatAllowedFileTypesToShow(imageValidExtensions)}
						maximumFileSizeInMb={convertKBToMB(
							DEFAULT_MAXIMUM_ALLOWED_IMAGE_SIZE_KB
						)}
					/>
				}
			/>
		</UploadWrapper>
	);
}
