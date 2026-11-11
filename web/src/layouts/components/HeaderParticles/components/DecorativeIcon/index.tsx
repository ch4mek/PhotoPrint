import { useMemo } from 'react';

import { DecorativeIconWrapper } from './styles';
import { DecorativeIconProps } from './types';

export function DecorativeIcon({
	src,
	anchorRef,
	width = 20,
	height = 20,
	offsetX = 0,
	offsetY = 0,
	style,
}: DecorativeIconProps) {
	const anchorPosition = useMemo(() => {
		const anchorElement = anchorRef?.current;

		if (!anchorElement) {
			return { top: 0, left: 0 };
		}

		return {
			top: anchorElement.offsetTop + offsetY,
			left: anchorElement.offsetLeft + offsetX,
		};
	}, [anchorRef, offsetX, offsetY]);

	return (
		<DecorativeIconWrapper
			src={src}
			width={width}
			height={height}
			style={style}
			{...anchorPosition}
		/>
	);
}
