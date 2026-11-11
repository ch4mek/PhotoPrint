import { Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { Avatar } from '@mms/mms-ui-library/components/Avatar';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import Color from 'color';
import { CSSProperties } from 'react';

import { DeskDirectionRotateDegree, Position } from '../../../../types';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { ThemeMode } from '@/theme';
import { DeskSize } from '@/types/booking';

import { DESK_BORDER_RADUIS, DESK_BORDER_WIDTH } from './constants';
import {
	ColorType,
	DeskTooltipTextWrapperProps,
	WorkplaceProps,
} from './types';

const DESK_WRAPPER_TRANSITION = 'all .2s ease-in-out';
const TINT_FACTOR = 0.05;
const DESK_SEAT_WIDTH = 20;
const DESK_SEAT_HEIGHT = 6;
const DESK_COLOR_OPACITY = 0.8;
const NO_ROTATION_DEGREE = 0;

const cssHelper = (
	func: (param: ColorType) => CSSProperties,
	param: ColorType
) => ({
	...func(param),
	'&:hover': {
		cursor: 'unset',
	},
	'&:active': {},
});

const isHorizontalDesk = (rotationDegree: number) =>
	rotationDegree === 0 || rotationDegree === 180;

const getPlaceColor = (
	isBlocked: boolean,
	isBooked: boolean,
	isBookedByUser: boolean,
	theme: Theme
) => {
	if (!isBlocked && !isBooked && !isBookedByUser) {
		return theme.palette['text-gray'];
	}

	return theme.palette['text-white'];
};

const LIGHTEN_PERSENTAGE = 0.2;

export const DeskTitle = styled(Typography)<{ rotationDegree: number }>(
	({ theme }) => ({
		userSelect: 'none',
		position: 'absolute',
		color: theme.palette['text-gray'],
		zIndex: 1,
	})
);

export const BookedAvatar = styled(Avatar)(({ theme }) => ({
	justifyContent: 'flex-start',
	width: 'fit-content',
	height: 'fit-content',
	...theme.typography.body['m-600'],
	backgroundColor: 'transparent',
	borderRadius: BorderRadiuses.none,
	color: theme.palette['text-white'],
	transition: 'none',
}));

export const DeskTooltipTextWrapper = styled(
	'div'
)<DeskTooltipTextWrapperProps>(({ isMobile, theme }) => ({
	display: 'inline-block',
	whiteSpace: 'pre-wrap',
	maxWidth: isMobile ? '15ch' : '43ch',
	wordWrap: 'break-word',
	...theme.typography.body['s-400'],
}));

const getFlexDirection = (rotationDegree: number) => {
	switch (rotationDegree) {
		case DeskDirectionRotateDegree.Bottom:
			return 'column';
		case DeskDirectionRotateDegree.Left:
			return 'row-reverse';
		case DeskDirectionRotateDegree.Top:
			return 'column-reverse';
		default:
			return 'row';
	}
};

export const WorkplaceContent = styled('div')<
	Position & {
		size: DeskSize;
		rotate?: number;
	}
>(({ left, top }) => ({
	position: 'absolute',
	left,
	top,
}));

export const DeskSeatContentWrapper = styled('div')<{ rotationDegree: number }>(
	({ rotationDegree }) => ({
		display: 'flex',
		alignItems: 'center',
		flexDirection: getFlexDirection(rotationDegree),
	})
);

const getCommonColorStyles = (theme: Theme) => (color: string) => ({
	backgroundColor: `${color}`,
	h6: {
		color: theme.palette['text-white'],
	},
});

const getColorStyles = (
	isBlocked: boolean,
	isBooked: boolean,
	isBookedByUser: boolean,
	selected: boolean,
	isForGuest: boolean,
	theme: Theme
) => ({
	...(isBlocked && {
		...cssHelper(getCommonColorStyles(theme), theme.palette['feedback-info']),
	}),
	...(isBooked && {
		...cssHelper(
			getCommonColorStyles(theme),
			theme.palette['feedback-success']
		),
	}),
	...(isBookedByUser && {
		...cssHelper(getCommonColorStyles(theme), theme.palette['graphics-accent']),
	}),
	...(isForGuest && {
		...cssHelper(getCommonColorStyles(theme), theme.palette['graphics-yellow']),
	}),
	...(selected && {
		borderColor: theme.palette['graphics-accent'],
		'&:hover': {
			cursor: 'pointer',
		},

		h6: {
			color: getPlaceColor(Boolean(isBlocked), isBooked, isBookedByUser, theme),
		},
	}),
	opacity: DESK_COLOR_OPACITY,
});

const getHoverStyles = (
	isBlocked: boolean,
	isBooked: boolean,
	isBookedByUser: boolean,
	isSelected: boolean,
	isForGuest: boolean,
	theme: Theme
) => {
	const hoverText = {
		h6: {
			color: theme.palette['text-gray'],
			'& > div': {
				color: theme.palette['text-gray'],
			},
		},
	};
	const bgColor = getColorStyles(
		isBlocked,
		isBooked,
		isBookedByUser,
		isSelected,
		isForGuest,
		theme
	).backgroundColor;

	if (isBlocked || isBooked || isBookedByUser || isForGuest) {
		return {
			backgroundColor: `${Color(bgColor)
				.lighten(LIGHTEN_PERSENTAGE)
				.toString()}`,
		};
	}

	const colorEffect = theme.mode === ThemeMode.LIGHT ? 'darken' : 'lighten';

	return {
		backgroundColor: `${Color(theme.palette['background-base-3'])
			[colorEffect](TINT_FACTOR)
			.toString()}`,
		...hoverText,
	};
};

export const DeskWrapper = styled('div')<WorkplaceProps>(
	({
		theme,
		isBlocked,
		isBooked,
		isBookedByUser,
		isSelected,
		isDisabled,
		isForGuest,
		height,
		width,
		rotate,
	}) => ({
		height,
		width,
		position: 'relative',
		border: `${DESK_BORDER_WIDTH}px solid ${theme.palette['background-white']}`,
		padding: theme.spaces.s,
		borderRadius: DESK_BORDER_RADUIS,
		cursor: !isDisabled ? 'pointer' : 'default',
		backgroundColor: theme.palette['background-base-3'],
		transition: DESK_WRAPPER_TRANSITION,
		transform: `rotate(${rotate ?? NO_ROTATION_DEGREE}deg)`,
		...getColorStyles(
			isBlocked,
			isBooked,
			isBookedByUser,
			isSelected,
			isForGuest,
			theme
		),

		'&:hover':
			!isDisabled &&
			getHoverStyles(
				isBlocked,
				isBooked,
				isBookedByUser,
				isSelected,
				isForGuest,
				theme
			),
	})
);

const deskSeatBorderRadiusStyles = (rotationDegree: number) => {
	if (rotationDegree === DeskDirectionRotateDegree.Top) {
		return {
			borderBottomLeftRadius: BorderRadiuses.xs2,
			borderBottomRightRadius: BorderRadiuses.xs2,
		};
	}

	if (rotationDegree === DeskDirectionRotateDegree.Bottom) {
		return {
			borderTopLeftRadius: BorderRadiuses.xs2,
			borderTopRightRadius: BorderRadiuses.xs2,
		};
	}

	if (rotationDegree === DeskDirectionRotateDegree.Right) {
		return {
			borderTopLeftRadius: BorderRadiuses.xs2,
			borderBottomLeftRadius: BorderRadiuses.xs2,
		};
	}

	return {
		borderTopRightRadius: BorderRadiuses.xs2,
		borderBottomRightRadius: BorderRadiuses.xs2,
	};
};

export const DeskSeatWrapper = styled('div')<{
	rotationDegree: number;
	isDisabled: boolean;
	rotate?: number;
}>(({ theme, rotationDegree, isDisabled, rotate }) => ({
	height: isHorizontalDesk(rotationDegree) ? DESK_SEAT_WIDTH : DESK_SEAT_HEIGHT,
	width: isHorizontalDesk(rotationDegree) ? DESK_SEAT_HEIGHT : DESK_SEAT_WIDTH,
	overflow: 'hidden',
	display: 'flex',
	flexShrink: 0,
	transform: `rotate(${rotate ?? NO_ROTATION_DEGREE}deg)`,
	backgroundColor: isDisabled
		? 'transparent'
		: theme.palette['background-base-3'],
	...deskSeatBorderRadiusStyles(rotationDegree),
}));
