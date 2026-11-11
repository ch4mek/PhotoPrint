import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { ContainerWrapper } from '@/components/InfoDataContainer/styles';
import { scrollbarWidths } from '@/constants/commonStyles';

import { INTERNSHIP_SECTION_CONTENT_MAX_HEIGHT } from './constants';

export const InternshipContainerWrapper = styled(ContainerWrapper)(
	({ theme }) => ({
		maxHeight: INTERNSHIP_SECTION_CONTENT_MAX_HEIGHT,
		overflowY: 'auto',
		...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),
	})
);
