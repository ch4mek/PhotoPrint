import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';

const MIN_PLATE_HEIGHT = 400;

export const EditorSectionWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
	width: '100%',
	flex: 1,
}));

export const PlateWrapper = styled('div')<{ isReadMode: boolean }>(
	({ theme, isReadMode }) => ({
		flexGrow: 1,
		overflowY: 'auto',

		backgroundColor: 'transparent',
		borderRadius: BorderRadiuses.m,
		border: isReadMode
			? 'none'
			: `1px solid ${theme.palette['graphics-divider-1']}`,

		...(!isReadMode && {
			height: MIN_PLATE_HEIGHT,
			padding: theme.spaces.m,
		}),

		whiteSpace: 'normal',
		wordBreak: 'break-word',

		...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),
	})
);
