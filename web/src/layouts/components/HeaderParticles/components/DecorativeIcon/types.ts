import { CSSProperties, RefObject } from 'react';

export interface DecorativeIconConfig {
	src: string;
	width?: number;
	height?: number;
	offsetX?: number;
	offsetY?: number;
	style?: CSSProperties;
}

export interface DecorativeIconProps extends DecorativeIconConfig {
	anchorRef?: RefObject<HTMLElement | null>;
}
