import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import {
	DEFAULT_DISABLE_OPACITY,
	DEFAULT_OPACITY,
} from '@/constants/opacities';
import { TimeSheetBreakpoints } from '@/pages/TimeSheet/constants';

import { FilterDropDownWithArrowsWrapperAttrs } from './types';

const FILTER_WRAPPER_SPAN_MAX_WIDTH = '90%';
const ARROWS_WRAPPER_HEIGHT = '95%';
const TEXT_DISTANCE = 72;
const ARROW_WRAPPER_POSITIONS = '1px';
const ARROW_WRAPPER_WIDTH = 32;

const getHoverStyles = (theme: Theme) => ({
	backgroundColor: theme.palette['hover-base-2'],
});

const WIDTH_VARIANTS = {
	small: 142,
	large: 280,
};

const { MOBILE_LG } = TimeSheetBreakpoints;

export const FilterWrapper = styled(
	'div'
)<FilterDropDownWithArrowsWrapperAttrs>(
	({ theme, width = 'large', fillAvailableWidthOnMobile }) => ({
		background: theme.palette['background-white'],
		borderRadius: BorderRadiuses.m,
		width: fillAvailableWidthOnMobile ? 'auto' : WIDTH_VARIANTS[width],
		...(fillAvailableWidthOnMobile && {
			flex: 1,
			minWidth: 0,
			[theme.breakpoints.up(MOBILE_LG)]: {
				maxWidth: WIDTH_VARIANTS.large,
			},
		}),
		position: 'relative',
		div: {
			padding: 0,
			gap: 0,

			'& button': {
				marginRight: theme.spaces.s,
			},

			'& span': {
				overflow: 'visible',
			},
		},
		span: {
			maxWidth: FILTER_WRAPPER_SPAN_MAX_WIDTH,
			paddingLeft: TEXT_DISTANCE,
		},
	})
);

export const ArrowsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'space-between',
	position: 'absolute',
	left: ARROW_WRAPPER_POSITIONS,
	top: ARROW_WRAPPER_POSITIONS,
	height: ARROWS_WRAPPER_HEIGHT,
	background: theme.palette['background-white'],
	borderTopLeftRadius: BorderRadiuses.m,
	borderBottomLeftRadius: BorderRadiuses.m,
	'& > div:first-of-type': {
		borderTopLeftRadius: BorderRadiuses.m,
		borderBottomLeftRadius: BorderRadiuses.m,
	},
}));

export const ArrowWrapper = styled('div')<{ disabled: boolean }>(
	({ theme, disabled }) => ({
		width: ARROW_WRAPPER_WIDTH,
		height: '100%',
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'center',
		borderRight: `1px solid ${theme.palette['graphics-input-stroke']}`,
		color: theme.palette['graphics-primary-2'],
		cursor: disabled ? 'default' : 'pointer',
		opacity: disabled ? DEFAULT_DISABLE_OPACITY : DEFAULT_OPACITY,

		'&:hover': disabled ? undefined : getHoverStyles(theme),
	})
);
