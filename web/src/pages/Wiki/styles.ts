import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { scrollbarWidths } from '@/constants/commonStyles';

export const Wrapper = styled.div(({ theme }) => ({
	position: 'relative',
	overflowY: 'hidden',
	display: 'flex',
	height: '100%',

	...getScrollbarStyles(theme, scrollbarWidths.m, 'horizontal'),
}));

export const EditorWrapper = styled.div`
	display: flex;
	position: relative;
	flex: 1;
	flex-direction: column;
	height: 100%;
	width: 100%;
	max-width: 100%;
`;
