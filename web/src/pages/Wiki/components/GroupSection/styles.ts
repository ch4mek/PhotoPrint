import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';
import { HALF_OPACITY } from '@/constants/opacities';
import { DRAGGABLE_CLASSNAME } from '@/pages/Wiki/components/GroupSection/constants';

const GROUP_SECTION_WIDTH = 500;
const ITEM_PADDING = 2;

export const GroupSectionWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	width: GROUP_SECTION_WIDTH,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: BorderRadiuses.m,
	flex: 1,
	gap: theme.spaces.m,

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		width: 'auto',
		minWidth: `calc(100vw - ${theme.spaces.xl}px)`,
	},
}));

export const GroupWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	height: '100%',
	padding: theme.spaces.m,
}));

export const CollapseGroupWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',

	flex: 1,
	alignItems: 'center',
	padding: `0 ${theme.spaces.m}px ${theme.spaces.m}px`,

	[`& > .${DRAGGABLE_CLASSNAME}`]: {
		padding: `${theme.spaces.xs}px 0`,

		'&:first-of-type': {
			paddingTop: 0,
		},

		'&:last-of-type': {
			paddingBottom: 0,
		},
	},

	...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),
}));

export const ItemContentWrapper = styled('div')({
	flex: 1,
	alignSelf: 'stretch',

	display: 'flex',
	alignItems: 'center',

	wordBreak: 'break-word',
	overflowWrap: 'break-word',
	whiteSpace: 'normal',
});

export const ItemWrapper = styled('div')<{ disabled: boolean }>(
	({ theme, disabled }) => ({
		display: 'flex',
		alignItems: 'start',
		cursor: 'pointer',
		WebkitUserSelect: 'none',
		msUserSelect: 'none',
		userSelect: 'none',
		height: '100%',
		color: theme.palette['text-black'],
		...theme.typography.body['m-400'],
		whiteSpace: 'normal',
		padding: `${ITEM_PADDING}px 0`,

		...(disabled && {
			pointerEvents: 'none',
			opacity: HALF_OPACITY,
		}),
	})
);
