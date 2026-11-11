import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { IconSize } from '@/constants/iconSizes';

const INFO_ICON_SIZE = 14;

export const generateCircleInfoStylesConfig = (theme: Theme) => ({
	width: IconSize.MS,
	height: IconSize.MS,
	justifyContent: 'center',

	'& > svg': {
		width: INFO_ICON_SIZE,
		height: INFO_ICON_SIZE,
		color: theme.palette['graphics-light-gray'],
	},
});

export const MinAmountSection = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
}));
