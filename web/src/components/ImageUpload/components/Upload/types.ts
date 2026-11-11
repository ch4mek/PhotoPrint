import { Dispatch, RefAttributes, SetStateAction } from 'react';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export interface UploadProps extends RefAttributes<HTMLDivElement> {
	file: string | null;
	handleUploadClick: VoidFunction;
	setIsHintOpen: Dispatch<SetStateAction<boolean>>;
	isHintOpen: boolean;
	alt: string;
	size: number;
	uploadRadius: number | BorderRadiuses;
	disabled?: boolean;
}

export interface UploadContentRendererProps {
	file: string | null;
	hasImageError: boolean;
	iconSize: number;
	showSkeleton: boolean;
	uploadRadius: number | BorderRadiuses;
	handleImageLoad: VoidFunction;
	handleImageError: VoidFunction;
	imageSource: string;
	alt: string;
}
