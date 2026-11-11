import styled from '@emotion/styled';
import { Skeleton } from '@mms/mms-ui-library/components/Skeleton';
import { CSSProperties } from 'react';

import { IMAGE_HEIGHT } from '@/components/ViewImage/constants';
import { BorderRadiuses } from '@/constants/borderRadiuses';
import { ThemeMode } from '@/theme';

const LIGHT_COLOR_NO_IMAGE_ICON = '#DADFE6';

export const ImageWrapper = styled('div')<{
	isClickable?: boolean;
	styleConfig?: CSSProperties;
}>(({ isClickable, styleConfig }) => ({
	height: styleConfig?.height ?? IMAGE_HEIGHT,
	width: '100%',
	cursor: isClickable ? 'pointer' : 'auto',
	maxWidth: styleConfig?.width,

	'& > div': {
		height: styleConfig?.width ?? IMAGE_HEIGHT,
		minWidth: styleConfig?.width,
	},
}));

export const DiscountImageWrapper = styled('img')<{
	type: 'small' | 'full';
	styleConfig?: CSSProperties;
}>(({ type, styleConfig }) => ({
	height: '100%',
	objectFit: 'cover',
	width: '100%',
	border: styleConfig?.border,
	borderRadius:
		styleConfig?.borderRadius ??
		(type === 'small'
			? BorderRadiuses.m
			: `${BorderRadiuses.m}px ${BorderRadiuses.m}px 0 0`),
}));

export const NoImageWrapper = styled('div')<{
	type: 'small' | 'full';
	styleConfig?: CSSProperties;
}>(({ theme, type, styleConfig }) => ({
	width: '100%',
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'center',
	border: styleConfig?.border,
	height: styleConfig?.height,

	backgroundColor:
		styleConfig?.backgroundColor ??
		(theme.mode === ThemeMode.LIGHT
			? theme.palette['background-base-2']
			: theme.palette['graphics-divider-2']),
	color:
		theme.mode === ThemeMode.LIGHT
			? LIGHT_COLOR_NO_IMAGE_ICON
			: theme.palette['graphics-primary-2'],

	borderRadius:
		type === 'small'
			? BorderRadiuses.m
			: `${BorderRadiuses.m}px ${BorderRadiuses.m}px 0 0`,
}));

export const DiscountImageSkeleton = styled(Skeleton)<{
	styleConfig?: CSSProperties;
}>(({ styleConfig }) => ({
	display: 'block',
	height: styleConfig?.height,
}));
