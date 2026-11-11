import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const QuickFiltersWrapper = styled('div')(({ theme }) => ({
	position: 'relative',
	display: 'flex',
	gap: theme.spaces.s,
	justifyContent: 'start',
	alignItems: 'center',
	paddingBottom: theme.spaces.m,
}));

export const QuickFiltersLabel = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-gray'],
}));

export const DepartmentsScroller = styled('div')({
	position: 'relative',
	width: '100%',
	maxWidth: '100%',
	overflow: 'hidden',
});

export const DepartmentsFilterButton = styled('button')<{
	isSelected?: boolean;
}>(({ theme, isSelected }) => ({
	backgroundColor: theme.palette['background-base-2'],
	color: theme.palette['text-black'],
	borderRadius: BorderRadiuses.m,
	padding: theme.spaces.s,
	border: 'none',
	cursor: 'pointer',

	...theme.typography.body['m-400'],

	...(isSelected && {
		color: theme.palette['text-white'],
		backgroundColor: theme.palette['graphics-scroll'],
	}),

	'&:hover': !isSelected && {
		backgroundColor: theme.palette['hover-primary-1'],
	},
}));

export const DepartmentsRow = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'start',
	alignItems: 'center',
	gap: theme.spaces.s,
	overflow: 'auto',
	msOverflowStyle: 'none',
	scrollbarWidth: 'none',
	WebkitOverflowScrolling: 'touch',

	'::-webkit-scrollbar': {
		display: 'none',
	},
}));
