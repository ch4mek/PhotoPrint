import { CSSProperties } from 'react';

export interface NameWithPhotoItemProps {
	imageId?: string | null;
	onClick?: VoidFunction;
	styleConfig?: CSSProperties;
	isResized?: boolean;
}
