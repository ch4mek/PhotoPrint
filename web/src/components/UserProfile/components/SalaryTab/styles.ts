import styled from '@emotion/styled';

export const SalaryTabWrapper = styled('div')(({ theme }) => ({
	flexGrow: 1,
	minHeight: 0,

	display: 'grid',
	gap: theme.spaces.m,

	gridTemplateColumns: 'minmax(0px, 1fr)',

	gridTemplateRows: 'minmax(0, 1fr) minmax(0, 1fr)',

	'& > *': {
		minHeight: 0,
	},
}));

export const PayslipsSectionWrapper = styled('div')({
	display: 'flex',
	flexDirection: 'column',
	height: '100%',
	minHeight: 0,
});
