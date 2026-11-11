import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { BLACK, ThemeMode, WHITE } from '../../theme';

import {
	FilterTagType,
	FilterTagWrapperProps,
} from '@/components/FilterTag/types';
import { BorderRadiuses } from '@/constants/borderRadiuses';
import { HALF_OPACITY } from '@/constants/opacities';
import { createGradient } from '@/helpers/createGradient';

const MIN_MAIN_TAG_WIDTH = 68;
const MAIN_TAG_HEIGHT = 36;
const MAIN_TYPE_TAG_FIRST_PADDING = 9;
const MAIN_TYPE_TAG_SECOND_PADDING = 10;
const GRADIENT_LIGHT_OPACITY = 0.9;
const GRADIENT_DARK_OPACITY = 0.7;

const tagStylesMap = (
	theme: Theme,
	isSelected: boolean,
	disabled: boolean
) => ({
	[FilterTagType.Main]: {
		width: MIN_MAIN_TAG_WIDTH,
		minWidth: MIN_MAIN_TAG_WIDTH,
		height: MAIN_TAG_HEIGHT,
		backgroundColor: theme.palette['background-base-2'],
		color: theme.palette['text-black'],
		borderRadius: BorderRadiuses.m,
		padding: `${MAIN_TYPE_TAG_FIRST_PADDING}px ${MAIN_TYPE_TAG_SECOND_PADDING}px`,
		...(isSelected && {
			color: theme.palette['text-white'],
			backgroundColor: theme.palette['graphics-scroll'],
		}),

		'&:hover': !isSelected &&
			!disabled && {
				backgroundColor: theme.palette['hover-primary-1'],
			},
	},
	[FilterTagType.Secondary]: {
		color: theme.palette['text-gray'],
		borderRadius: BorderRadiuses.xl3,
		padding: `${theme.spaces.xs}px ${theme.spaces.s}px`,

		...(isSelected && {
			color: theme.palette['text-accent'],
			backgroundColor: 'transparent',
			background: createGradient(
				theme.palette['graphics-accent'],
				theme.mode === ThemeMode.LIGHT ? WHITE : BLACK,
				theme.mode === ThemeMode.LIGHT
					? GRADIENT_LIGHT_OPACITY
					: GRADIENT_DARK_OPACITY
			),
		}),

		'&:hover': !isSelected &&
			!disabled && {
				backgroundColor: theme.palette['hover-base-2'],
			},
	},
});

const getTagStyles = (
	type: FilterTagType,
	isSelected: boolean,
	disabled: boolean,
	theme: Theme
) => tagStylesMap(theme, isSelected, disabled)[type] || {};

export const FilterTagWrapper = styled('div')<FilterTagWrapperProps>(
	({ theme, isSelected, type, disabled }) => ({
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'center',
		cursor: 'pointer',
		transition: 'all .2s ease',
		...theme.typography.body['m-400'],
		...getTagStyles(type, isSelected, disabled, theme),
		...(disabled && {
			opacity: HALF_OPACITY,
			cursor: 'not-allowed',
		}),
	})
);
