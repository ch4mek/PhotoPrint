import styled from '@emotion/styled';

import { COLLAPSIBLE_TABLE_DEFAULT_ROW_HEIGHT } from '../constants';
import { ListItemButton } from '../styles';
import { getGridTemplateColumnsForCollapsibleTable } from '../utils';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { checkIfForwardProps } from '@/helpers/checkIfForwardProps';

const PROPS_TO_CHECK = [
	'isRowOpen',
	'isDisplayFact',
	'isLastItemBorderRounded',
];

export const TaskListHeaderWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	width: '100%',
	flexDirection: 'column',
	paddingLeft: `${theme.spaces.l}px`,
}));

export const TaskListHeaderContainer = styled('div', {
	shouldForwardProp: checkIfForwardProps(PROPS_TO_CHECK),
})<{ isDisplayFact?: boolean }>(({ theme, isDisplayFact }) => ({
	position: 'relative',
	display: 'grid',
	gridTemplateColumns: getGridTemplateColumnsForCollapsibleTable({
		isDisplayFact,
	}),

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		display: 'flex',
		flexDirection: 'column',
		gridTemplateColumns: 'none',
	},
}));

export const TaskTitleContainer = styled.div`
	display: flex;
	flex-direction: column;
`;

export const ProjectTitleWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		paddingTop: `${theme.spaces.s}px`,
	},
}));

export const ProjectTitleContainer = styled.div`
	display: flex;
	position: relative;
	height: ${COLLAPSIBLE_TABLE_DEFAULT_ROW_HEIGHT}px;
`;

export const ProjectTitle = styled('div')(({ theme }) => ({
	position: 'absolute',
	inset: 0,
	display: 'flex',
	paddingRight: `${theme.spaces.xl2}px`,
}));

export const DurationTitleWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'flex-end',

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		justifyContent: 'start',
		paddingTop: `${theme.spaces.s}px`,
	},
}));

export const DurationTitleContainer = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	marginRight: `${theme.spaces.xl}px`,
	width: '100%',
}));

export const TableListItemButton = styled(ListItemButton, {
	shouldForwardProp: checkIfForwardProps(PROPS_TO_CHECK),
})<{
	isLastItemBorderRounded: boolean;
}>(({ theme, isLastItemBorderRounded }) => ({
	width: '100%',
	padding: `${theme.spaces.s}px`,
	borderRadius: isLastItemBorderRounded
		? `none`
		: `0 0 ${BorderRadiuses.m}px ${BorderRadiuses.m}px`,

	'&:hover': {
		backgroundColor: theme.palette['hover-base-2'],
	},
}));
