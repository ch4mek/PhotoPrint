import styled from '@emotion/styled';

const MIN_AUTOSELECT_WIDTH = 180;
const MAX_AUTOSELECT_WIDTH = 200;

export const AbsenceHistoryControlsWrapper = styled('div')<{
	isHasTotalTitle: boolean;
}>(({ theme, isHasTotalTitle }) => ({
	display: 'flex',
	justifyContent: 'flex-end',
	alignItems: 'center',
	gap: theme.spaces.s,
	flexWrap: 'wrap',
	flexGrow: 1,

	[theme.breakpoints.down(theme.breakpoints.tablet)]: {
		width: '100%',
		justifyContent: isHasTotalTitle ? 'space-between' : 'flex-end',
	},
}));

export const AutoselectsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
	flexDirection: 'row',
	justifyContent: 'flex-end',
	flexWrap: 'wrap',

	[theme.breakpoints.down(theme.breakpoints.tablet)]: {
		width: '100%',
	},
}));

export const FilterWrapper = styled('div')(({ theme }) => ({
	minWidth: MIN_AUTOSELECT_WIDTH,
	maxWidth: MAX_AUTOSELECT_WIDTH,

	[theme.breakpoints.down(theme.breakpoints.tablet)]: {
		width: 'auto',
		maxWidth: 'none',
		flex: 1,
	},
}));
