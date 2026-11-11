import { ViewImage } from '@/components/ViewImage';

import { NameWithPhotoItemProps } from './types';

export function PhotoItem({
	imageId,
	onClick,
	styleConfig,
	isResized,
}: NameWithPhotoItemProps) {
	return (
		<ViewImage
			styleConfig={styleConfig}
			onClick={onClick}
			imageId={imageId}
			title={String(imageId)}
			isResized={isResized}
		/>
	);
}
