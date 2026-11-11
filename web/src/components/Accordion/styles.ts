import styled from '@emotion/styled';

import { ChevronButton } from '../ChevronButton';

import { DEFAULT_ICON_SIZE } from '@/constants/commonStyles';

const COLLAPSIBLE_ANIMATION_DURATION = '0.2s';

export const CollapsibleWrapper = styled('div')(
	() =>
		({
			display: 'flex',
			flexDirection: 'column',
		} as const)
);

export const CollapsibleButton = styled('div')<{ allowToOpen: boolean }>(
	({ theme, allowToOpen }) => ({
		display: 'flex',
		gap: theme.spaces.s,
		color: theme.palette['text-black'],
		justifyContent: 'space-between',
		cursor: allowToOpen ? 'pointer' : undefined,

		span: {
			wordBreak: 'break-word',
		},
	})
);

export const ContentWrapper = styled('div')<{
	height: number;
	spacingType?: 'margin' | 'padding';
	isOpened?: boolean;
}>(({ height, theme, spacingType = 'margin', isOpened = false }) => {
	const spacingValue = theme.spaces.m;

	return {
		[`${spacingType}Top`]: height <= 0 ? 0 : spacingValue,
		height:
			spacingType === 'padding' && isOpened
				? height + spacingValue + theme.spaces.xs
				: height,
		transition: `all ${COLLAPSIBLE_ANIMATION_DURATION} ease-in-out`,
		overflow: 'hidden',
	};
});

export const Content = styled('div')({
	display: 'flex',
	justifyContent: 'center',
});

export const ButtonsContainer = styled('div')(() => ({
	display: 'flex',
	alignItems: 'flex-start',
}));

export const ErrorIconContainer = styled('div')(() => ({
	width: DEFAULT_ICON_SIZE,
	display: 'flex',
	justifyContent: 'center',

	svg: {
		width: '100%',
		height: '100%',
	},
}));

export const ExpandButton = styled(ChevronButton)(({ theme }) => ({
	'&, &:hover': {
		color: theme.palette['text-black'],
	},
}));
