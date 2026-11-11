import styled from '@emotion/styled';

export const ProgrammerDayGameHeader = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'flex-end',
	gap: theme.spaces.l,
	flexWrap: 'wrap',
	minWidth: 0,
	width: '100%',

	[theme.breakpoints.down(theme.breakpoints.tablet)]: {
		alignItems: 'flex-start',
		flexDirection: 'column',
	},
}));

export const ProgrammerDayGameTitleWrapper = styled('div')(({ theme }) => ({
	minWidth: 0,
	maxWidth: '100%',
	flex: '1 1 0',

	[theme.breakpoints.down(theme.breakpoints.tablet)]: {
		width: '100%',
		flex: '0 1 auto',
	},
}));

export const ProgrammerDayGameTitle = styled('h1')(({ theme }) => ({
	margin: 0,
	maxWidth: '100%',
	color: theme.palette['text-black'],
	fontSize: 'clamp(22px, 2.4vw, 32px)',
	fontWeight: 600,
	lineHeight: 1.1,
	whiteSpace: 'normal',
	overflowWrap: 'break-word',
}));

export const ProgrammerDayGameTitleAccent = styled('span')(({ theme }) => ({
	color: theme.palette['text-accent'],
	fontWeight: 700,
}));

export const ProgrammerDayGameDescription = styled('p')(({ theme }) => ({
	margin: `${theme.spaces.xs}px 0 0`,
	width: '100%',
	maxWidth: '78ch',
	color: theme.palette['text-gray'],
	fontSize: 14,
	lineHeight: 1.5,
	whiteSpace: 'normal',
	overflowWrap: 'break-word',
}));

export const ProgrammerDayGameCallToAction = styled('span')(({ theme }) => ({
	display: 'block',
	marginTop: theme.spaces.xs,
	color: theme.palette['text-black'],
	fontWeight: 500,
}));

export const ProgrammerDayGameNumber = styled('div')(({ theme }) => ({
	minWidth: 0,
	maxWidth: '100%',
	flex: '0 1 auto',
	color: theme.palette['text-gray'],
	fontSize: 'clamp(26px, 3vw, 40px)',
	fontWeight: 300,
	letterSpacing: '0.14em',
	lineHeight: 1,
	whiteSpace: 'normal',
	overflowWrap: 'break-word',

	[theme.breakpoints.down(theme.breakpoints.tablet)]: {
		width: '100%',
	},
}));

export const ProgrammerDayGameNumberAccent = styled('span')(({ theme }) => ({
	color: theme.palette['text-accent'],
	fontWeight: 500,
}));
