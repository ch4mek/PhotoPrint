import styled from '@emotion/styled';

import { CONTROLLERS_BREAKPOINT } from '../../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const FORM_WIDTH = 232;
const FORM_PADDING = 15;
const FILTER_WIDTH = 200;

const CONTROLLER_MD_BREAKPOINT = 1280;

export const ControlsContainer = styled('div')(({ theme }) => ({
	marginTop: theme.spaces.m,
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,

	[theme.breakpoints.up(CONTROLLERS_BREAKPOINT)]: {
		flexDirection: 'row',
		gap: theme.spaces.s,
		marginTop: 0,
	},
}));

export const Form = styled('form')(({ theme }) => ({
	display: 'block',
	width: FORM_WIDTH,
	padding: FORM_PADDING,
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	backgroundColor: theme.palette['background-white'],
	borderRadius: BorderRadiuses.m,
	boxShadow: theme.namedShadows.dropDownListWrapper,

	[theme.breakpoints.up(CONTROLLERS_BREAKPOINT)]: {
		display: 'flex',
		gap: theme.spaces.s,
		width: 'auto',
		padding: 0,
		border: 'none',
		borderRadius: 0,
		boxShadow: 'none',
		backgroundColor: 'transparent',
	},
}));

export const FormFields = styled('div')<{ countOfFiltersByRole: number }>(
	({ theme, countOfFiltersByRole }) => ({
		display: 'grid',
		gap: theme.spaces.s,
		justifyContent: 'start',
		gridAutoRows: 'min-content',

		gridTemplateColumns: '1fr',

		[theme.breakpoints.up(CONTROLLER_MD_BREAKPOINT)]: {
			gridTemplateColumns: `repeat(${Math.min(
				4,
				countOfFiltersByRole
			)}, ${FILTER_WIDTH}px)`,
		},

		[theme.breakpoints.up(theme.breakpoints.desktopS)]: {
			gridTemplateColumns: `repeat(${Math.min(
				5,
				countOfFiltersByRole
			)}, ${FILTER_WIDTH}px)`,
		},

		[theme.breakpoints.up(theme.breakpoints.desktop)]: {
			gridTemplateColumns: `repeat(${Math.min(
				6,
				countOfFiltersByRole
			)}, ${FILTER_WIDTH}px)`,
		},
	})
);
