import styled from '@emotion/styled';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';

const DEFAULT_WIDGET_HEIGHT = 400;

export const WidgetWrapper = styled('section')<{
	height?: string | number;
	maxHeight?: string | number;
}>(({ theme, height, maxHeight = DEFAULT_WIDGET_HEIGHT }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
	height,
	maxHeight,
	width: '100%',
	minWidth: 0,
	paddingTop: theme.spaces.m,

	backgroundColor: theme.palette['background-white'],
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: BorderRadiuses.m,
}));

export const WidgetHeader = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'row',
	justifyContent: 'space-between',
	gap: theme.spaces.s,
	padding: `0 ${theme.spaces.m}px`,
}));

export const WidgetTitleWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
	minWidth: 0,
}));

export const WidgetHintWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xs + theme.spaces.xs / 2,
}));

export const WidgetContentWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	overflowY: 'auto',
	flex: 1,
	minHeight: 0,

	...getScrollbarStyles(theme, scrollbarWidths.s, 'vertical'),
}));

export const WidgetScrollableContent = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'center',
	flex: '1 0 auto',
	minWidth: 0,
	padding: `0 ${theme.spaces.m}px ${theme.spaces.m}px`,
}));

export const WidgetActionTitle = styled(ActionButton)(({ theme }) => ({
	width: 'auto',
	height: 'auto',
	color: theme.palette['text-gray'],
}));
