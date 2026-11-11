import styled from '@emotion/styled';

import { IconSize } from '@/constants/iconSizes';

const DISABLE_ICON_OPACITY = 0.3;

export const StatusCellWrapper = styled('div')(({ theme }) => ({
	height: '100%',
	width: '100%',
	padding: `${theme.spaces.xs}px 0`,
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
}));

export const StatusesWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
}));

export const IconWrapper = styled('div')<{
	color: string;
	isDisabled: boolean;
}>(({ color, isDisabled }) => ({
	width: IconSize.M,
	height: IconSize.M,
	color,

	'& > svg': {
		width: IconSize.M,
		height: IconSize.M,
	},

	...(isDisabled && {
		opacity: DISABLE_ICON_OPACITY,
	}),
}));
