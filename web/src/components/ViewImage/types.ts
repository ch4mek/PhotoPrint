import { CSSProperties } from 'react';

export interface DiscountImageProps {
	title: string;
	type?: 'small' | 'full';
	imageId?: string | null;
	onClick?: VoidFunction;
	styleConfig?: CSSProperties;
	isResized?: boolean;
}
