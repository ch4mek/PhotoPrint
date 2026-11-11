import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths, SMALL_ICON_SIZE } from '@/constants/commonStyles';

const WIDTH_EDITOR_WRAPPER = 850;

export const DragButton = styled.button`
	overflow: hidden;
	padding: 0;
	background-color: transparent;
	background-repeat: no-repeat;
	border-style: none;
	cursor: pointer;
	outline: 0;
	width: ${SMALL_ICON_SIZE}px;
	height: ${SMALL_ICON_SIZE}px;
`;

export const ScrollbarWrapper = styled('div')<{ isEditor: boolean }>(
	({ isEditor, theme }) => ({
		flexGrow: isEditor ? 1 : 0,
		padding: isEditor ? theme.spaces.m : `0 ${theme.spaces.l}px`,
		display: 'flex',
		width: '100%',
		justifyContent: 'center',
		overflowY: 'auto',

		...(!isEditor && {
			display: 'flow-root',
			...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),
		}),
	})
);

export const EditorWrapper = styled('div')<{ isEditor: boolean }>(
	({ isEditor, theme }) => ({
		backgroundColor: isEditor
			? theme.palette['background-white']
			: 'transparent',
		border: isEditor
			? `1px solid ${theme.palette['graphics-divider-1']}`
			: 'none',
		borderRadius: isEditor ? `${BorderRadiuses.m}px` : 'none',
		maxWidth: WIDTH_EDITOR_WRAPPER,
		width: '100%',
		flexGrow: 1,

		...(isEditor && {
			padding: theme.spaces.l,

			whiteSpace: 'normal',
			wordBreak: 'break-word',

			overflowY: 'auto',
			...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),
		}),
	})
);

export const ArticleTitle = styled(Typography)(({ theme }) => ({
	marginBottom: theme.spaces.m,
	color: theme.palette['text-black'],
}));
