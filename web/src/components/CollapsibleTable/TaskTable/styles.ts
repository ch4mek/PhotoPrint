import styled from '@emotion/styled';

import { getGridTemplateColumnsForCollapsibleTable } from '../utils';

import { checkIfForwardProps } from '@/helpers/checkIfForwardProps';

const TOP_BOTTOM_TABLE_ROW_PADING = 2;

const PROPS_TO_CHECK = ['isDisplayFact', 'isTaskRowHeader'];

export const ListItemButtonWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	width: '100%',
	padding: `${TOP_BOTTOM_TABLE_ROW_PADING}px ${theme.spaces.s}px ${TOP_BOTTOM_TABLE_ROW_PADING}px ${theme.spaces.xl3}px`,

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		display: 'none',
	},
}));

export const MobileTableItem = styled('div')(({ theme }) => ({
	display: 'none',

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		display: 'flex',
		flexDirection: 'column',
		width: '100%',
	},
}));

export const TaskDescription = styled('span')(({ theme }) => ({
	...theme.typography.body['m-400'],
	textWrap: 'wrap',
	width: '90%',
}));

export const TaskTableRowWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	width: '100%',
	flexDirection: 'column',
	padding: `${theme.spaces.s}px ${theme.spaces.s}px ${theme.spaces.xs}px ${theme.spaces.xl3}px`,

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		display: 'none',
	},
}));

export const TaskTableRowContainer = styled('div', {
	shouldForwardProp: checkIfForwardProps(PROPS_TO_CHECK),
})<{ isDisplayFact?: boolean; isTaskRowHeader?: boolean }>(
	({ theme, isDisplayFact, isTaskRowHeader }) => ({
		display: 'grid',
		gridTemplateColumns: getGridTemplateColumnsForCollapsibleTable({
			isDisplayFact,
			isTaskRow: isTaskRowHeader,
		}),
		color: theme.palette['text-gray'],
	})
);

export const TaskTableRowDurationTitle = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'flex-end',
	marginRight: `${theme.spaces.xl}px`,
}));

export const TaskTableList = styled('ul')(({ theme }) => ({
	padding: 0,
	paddingBottom: `${theme.spaces.s}px`,
	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		padding: 0,
	},
}));
