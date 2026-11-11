import styled from '@emotion/styled';

import { COLLAPSIBLE_TABLE_DEFAULT_ROW_HEIGHT } from '../constants';
import { ListItemButton } from '../styles';
import { getGridTemplateColumnsForCollapsibleTable } from '../utils';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { checkIfForwardProps } from '@/helpers/checkIfForwardProps';

import { EMPLOYEE_NAME_LEFT_INDENTATION } from './constants';

const TABLE_LIST_ITEM_BUTTON_PROPS_TO_CHECK = ['isRowOpen', 'isDisplayFact'];

export const TableListItemButton = styled(ListItemButton, {
	shouldForwardProp: checkIfForwardProps(TABLE_LIST_ITEM_BUTTON_PROPS_TO_CHECK),
})<{
	isRowOpen: boolean;
}>(({ theme, isRowOpen }) => ({
	width: '100%',
	padding: `${theme.spaces.s}px`,
	borderRadius: isRowOpen
		? `${BorderRadiuses.m}px ${BorderRadiuses.m}px 0 0`
		: BorderRadiuses.m,

	'&:hover': {
		backgroundColor: theme.palette['hover-base-2'],
	},
}));

export const MobileUserRowInfoWrapper = styled('div')(({ theme }) => ({
	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		display: 'flex',
		flexDirection: 'column',
		marginTop: theme.spaces.s,
		'&>*:nth-of-type(2n):not(:last-child)': {
			marginBottom: theme.spaces.s,
		},
	},

	[theme.breakpoints.up(theme.breakpoints.tabletXS)]: {
		display: 'none',
	},
}));

export const UserTableRowHeaderContainer = styled('div', {
	shouldForwardProp: checkIfForwardProps(TABLE_LIST_ITEM_BUTTON_PROPS_TO_CHECK),
})<{ isDisplayFact?: boolean }>(({ theme, isDisplayFact }) => ({
	width: '100%',
	display: 'grid',
	position: 'relative',
	gridTemplateColumns: getGridTemplateColumnsForCollapsibleTable({
		isDisplayFact,
	}),

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		gridTemplateColumns: '100%',
	},
}));

export const UserTableRowHeaderWrapper = styled.div`
	display: flex;
	flex-direction: column;
	width: 100%;
`;

export const UserTableRowHeaderCountContainer = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		display: 'none',
	},
}));

export const UserTableRowHeaderEmployeeContainer = styled.div`
	display: flex;
	flex-direction: column;
`;

export const UserTableEmployeeRowContainer = styled.div`
	display: flex;
	position: relative;
`;

export const UserTableEmployeeRowBullet = styled.div`
	display: flex;
	align-items: center;
	height: ${COLLAPSIBLE_TABLE_DEFAULT_ROW_HEIGHT}px;
	margin-right: 0.5px;
`;

export const UserTableEmployeeNameContainer = styled('div')(({ theme }) => ({
	position: 'absolute',
	top: 0,
	bottom: 0,
	left: EMPLOYEE_NAME_LEFT_INDENTATION,
	right: 0,
	display: 'flex',
	height: `${COLLAPSIBLE_TABLE_DEFAULT_ROW_HEIGHT}px`,
	paddingRight: `${theme.spaces.xl2 + EMPLOYEE_NAME_LEFT_INDENTATION}px`,
}));

export const UserTableRowHeaderDurationContainer = styled('div')(
	({ theme }) => ({
		display: 'flex',
		justifyContent: 'flex-end',
		[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
			justifyContent: 'left',
		},
	})
);

export const UserTableRowHeaderDuration = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	marginRight: `${theme.spaces.xl}px`,
	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		display: 'none',
	},
}));
