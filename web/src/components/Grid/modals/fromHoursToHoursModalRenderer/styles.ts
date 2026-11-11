import styled from '@emotion/styled';

const GRID_TEMPLATE_COLUMNS = 105;
const VERTICAL_DIVIDER_WIDTH = 1;
const VERTICAL_DIVIDER_POSITION = 2;

export const FromHoursToHoursTimeModalWrapper = styled('div')(({ theme }) => ({
	width: '100%',
	display: 'flex',
	flexDirection: 'column',
	padding: theme.spaces.m,
	gap: theme.spaces.m,
}));

export const FromToTimeModalInputsWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: `${GRID_TEMPLATE_COLUMNS}px ${VERTICAL_DIVIDER_WIDTH}px ${GRID_TEMPLATE_COLUMNS}px`,

	'&:hover': {
		[`> div:nth-of-type(${VERTICAL_DIVIDER_POSITION})`]: {
			backgroundColor: theme.palette['graphics-gray'],
		},
	},
	'&:focus-within': {
		[`> div:nth-of-type(${VERTICAL_DIVIDER_POSITION})`]: {
			backgroundColor: theme.palette['graphics-accent'],
		},
	},

	'> div:first-of-type': {
		borderTopRightRadius: 0,
		borderBottomRightRadius: 0,
		background: 'transparent',
		borderRight: 'none',

		'& label:has(+ div>input:not(:focus))': {
			left: 'default',
			transform: 'default',
			whiteSpace: 'nowrap',
			transitionProperty: 'all',
		},
	},
	'> div:last-of-type': {
		borderTopLeftRadius: 0,
		borderBottomLeftRadius: 0,
		background: 'transparent',
		borderLeft: 'none',

		'& label:has(+ div>input:not(:focus))': {
			left: 'default',
			transform: 'default',
			whiteSpace: 'nowrap',
			transitionProperty: 'all',
		},
	},
}));

export const VerticalDivider = styled('div')(({ theme }) => ({
	width: `${VERTICAL_DIVIDER_WIDTH}px`,
	height: '100%',
	backgroundColor: theme.palette['graphics-input-stroke'],
}));
