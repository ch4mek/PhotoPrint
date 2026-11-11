import styled from '@emotion/styled';

export const CompoundsFieldsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	width: '100%',
	flexDirection: 'column',
	alignItems: 'flex-start',
	gap: theme.spaces.l,
}));

export const CompoundItemWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	width: '100%',
	alignItems: 'center',
	gap: theme.spaces.s,

	'& > div': {
		minHeight: 40,
		minWidth: 110,
	},
}));

export const CompoundWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	width: '100%',
	alignItems: 'flex-start',
	gap: theme.spaces.l,
}));

export const Divider = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	height: 1,
	background: theme.palette['graphics-divider-1'],
	width: '100%',
	alignItems: 'flex-start',
	gap: theme.spaces.s,
}));

export const CompoundSectionWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	width: '100%',
	flexDirection: 'column',
	alignItems: 'flex-start',
	gap: theme.spaces.s,
}));
