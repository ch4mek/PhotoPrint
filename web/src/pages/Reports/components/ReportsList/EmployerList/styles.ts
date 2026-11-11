import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

const WRAPPER_BORDER_RADIUS = 6;

export const Wrapper = styled('div')(({ theme }) => ({
	position: 'relative',
	gridColumnStart: 2,

	display: 'flex',
	flexDirection: 'column',
	flexWrap: 'wrap',
	justifyContent: 'center',

	borderRadius: WRAPPER_BORDER_RADIUS,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	backgroundColor: [theme.palette['background-white']],
	height: '100%',
	width: '100%',

	[theme.breakpoints.down(theme.breakpoints.desktopS)]: {
		gridColumnStart: 'auto',
	},

	[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
		justifyContent: 'space-between',
		gap: theme.spaces.xl,
	},
}));

export const TabsWrapper = styled('div')(({ theme }) => ({
	display: 'none',

	[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
		display: 'flex',

		'& > div': {
			width: '100%',
		},
	},
}));

export const SingleChartWrapper = styled('div')(({ theme }) => ({
	alignSelf: 'center',
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'center',
	gap: theme.spaces.s,

	'& > :last-child': {
		top: theme.spaces.xl3,
	},

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		flexGrow: 1,
	},
}));

export const ChartName = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-black'],
	textTransform: 'lowercase',
	marginBottom: theme.spaces.s,
	...theme.typography.headlines.small,

	'&::first-letter': {
		textTransform: 'uppercase',
	},

	[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
		display: 'none',
	},
}));

export const PieChartWithInfoWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'row',
	gap: theme.spaces.l,
	width: '100%',
	justifyContent: 'center',

	[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
		padding: `0px 0px ${theme.spaces.m}px ${theme.spaces.m}px`,
	},

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		flexDirection: 'column',
		padding: 0,
	},
}));
