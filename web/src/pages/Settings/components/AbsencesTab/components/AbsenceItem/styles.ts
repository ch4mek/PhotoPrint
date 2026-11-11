import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { IconSize } from '@/constants/iconSizes';

const COLLAPSIBLE_ANIMATION_DURATION = '0.2s';

export const AbsenceItemWrapper = styled('li')(({ theme }) => ({
	width: '100%',
	padding: theme.spaces.s,

	borderRadius: BorderRadiuses.m,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	backgroundColor: theme.palette['background-white'],
}));

export const AbsenceItemHeader = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'row',
	justifyContent: 'space-between',
	alignItems: 'center',
	width: '100%',
	gap: theme.spaces.s,
}));

export const ItemWrapper = styled('div')<{ clickable?: boolean }>(
	({ theme, clickable }) => ({
		display: 'flex',
		flexDirection: 'row',
		alignItems: 'center',
		gap: theme.spaces.s,

		flex: 1,
		minWidth: 0,

		cursor: clickable ? 'pointer' : 'default',
	})
);

export const ControlsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
}));

export const ContentWrapper = styled('div')<{ height: number }>(
	({ height, theme }) => ({
		marginTop: height <= 0 ? 0 : theme.spaces.s,
		height,
		transition: `all ${COLLAPSIBLE_ANIMATION_DURATION} ease-in-out`,
		overflow: 'hidden',
	})
);

export const Content = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'center',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

export const IconWrapper = styled('div')<{ backgroundColor?: string }>(
	({ backgroundColor }) => ({
		width: IconSize.L,
		height: IconSize.L,
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'center',
		borderRadius: BorderRadiuses.xs2,
		background: backgroundColor,
	})
);
