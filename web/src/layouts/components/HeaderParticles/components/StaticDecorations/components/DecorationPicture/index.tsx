import { useTheme } from '@emotion/react';

import { getResponsivePictureSources } from '../../utils';

import { DecorationImage } from './styles';
import { DecorationPictureProps } from './types';

export function DecorationPicture({
	images,
	alt,
	hideBelow,
}: DecorationPictureProps) {
	const theme = useTheme();
	const sources = getResponsivePictureSources(theme, images);

	return (
		<DecorationImage hideBelow={hideBelow}>
			{sources.map(({ maxWidth, srcSet }) => (
				<source
					key={maxWidth}
					media={`(max-width: ${maxWidth}px)`}
					srcSet={srcSet}
				/>
			))}
			{images.desktop && <img src={images.desktop} alt={alt ?? ''} />}
		</DecorationImage>
	);
}
