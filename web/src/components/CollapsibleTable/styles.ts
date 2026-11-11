import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { ChevronButton } from '@/components/ChevronButton';
import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';

const PAGINATION_HEIGHT = 52;

const MIN_COLLAPSIBLE_TABLE_HEIGHT_WITH_PAGINATION = `calc(100% - ${PAGINATION_HEIGHT}px)`;

const DEFAULT_MIN_COLLAPSIBLE_TABLE_BACKGROUND = 324;
const EMPLOYEE_MIN_COLLAPSIBLE_TABLE_BACKGROUND = 292;
const CONTENT_HEIGHT_WITHOUT_COLLAPSIBLE_TABLE = 885;

const LIST_ITEM_DISABLED_OPACITY = 0.38;

export const CollapsibleTableBackground = styled('div')<{
	isEmployee: boolean;
}>(({ theme, isEmployee }) => ({
	backgroundColor: theme.palette['background-white'],
	borderRadius: BorderRadiuses.m,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	width: '100%',
	minHeight: isEmployee
		? EMPLOYEE_MIN_COLLAPSIBLE_TABLE_BACKGROUND
		: DEFAULT_MIN_COLLAPSIBLE_TABLE_BACKGROUND,
}));

export const CollapsibleTableWrapper = styled.div`
	flex-direction: column;
	align-items: flex-start;
	width: 100%;
	height: 100%;
	display: flex;
	white-space: nowrap;
`;

export const PaginationWrapper = styled('div')(({ theme }) => ({
	borderTop: `1px solid ${theme.palette['graphics-divider-1']}`,
	padding: `${theme.spaces.s}px ${theme.spaces.m}px`,
	width: '100%',
	height: PAGINATION_HEIGHT,
}));

export const CollapsibleTableList = styled('ul')<{
	isEmployee: boolean;
}>(({ theme, isEmployee }) => ({
	width: '100%',
	scrollbarGutter: 'stable',
	gap: theme.spaces.s,
	listStyle: 'none',
	display: 'flex',
	flexDirection: 'column',
	padding: `${theme.spaces.m}px ${theme.spaces.s}px ${theme.spaces.m}px ${theme.spaces.m}px`,
	margin: 0,

	maxHeight: `calc(100vh - ${CONTENT_HEIGHT_WITHOUT_COLLAPSIBLE_TABLE}px)`,
	minHeight: isEmployee ? '100%' : MIN_COLLAPSIBLE_TABLE_HEIGHT_WITH_PAGINATION,

	...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),
}));

export const TableTitle = styled(Typography)(({ theme }) => ({
	marginBottom: theme.spaces.xs,
	color: theme.palette['text-gray'],
}));

export const DurationTitleLabel = styled('span')(({ theme }) => ({
	color: theme.palette['text-gray'],
	...theme.typography.body['s-400'],
}));

export const TableContent = styled(Typography)(
	() => css`
		width: 90%;
		word-break: break-all;
		max-width: 100%;
		overflow: hidden;
		text-overflow: ellipsis;
		white-space: nowrap;
	`
);

export const ChevronButtonWrapper = styled(ChevronButton)(
	({ theme, disabled }) => ({
		position: 'absolute',
		top: theme.spaces.s,
		right: 0,
		color: theme.palette['graphics-primary-2'],

		...(!disabled && {
			'&:hover': {
				color: theme.palette['graphics-accent'],
			},
		}),
	})
);

export const ListItemButton = styled('button')(({ theme }) => ({
	padding: `${theme.spaces.s}px`,
	color: theme.palette.text.primary,
	background: 'transparent',
	border: 'none',
	cursor: 'pointer',
	outline: 'none',
	display: 'flex',
	alignItems: 'center',
	width: '100%',
	justifyContent: 'flex-start',
	transition: 'background 0.3s ease',

	'&:hover': {
		background: theme.palette['hover-base-2'],
		'&:disabled': {
			background: 'transparent',
		},
	},

	'&:disabled': {
		userSelect: 'none',
		pointerEvents: 'none',
		cursor: 'default',
		opacity: LIST_ITEM_DISABLED_OPACITY,

		button: {
			cursor: 'default',
		},
	},
}));
