import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { ContainerWrapper } from '@/components/InfoDataContainer/styles';
import { scrollbarWidths } from '@/constants/commonStyles';

const BORDER_WIDTHS = 2;
const MAX_PLAN_SECTION_HEIGHT = 124;

export const PlanContainerWrapper = styled(ContainerWrapper)<{
	isShowLoader: boolean;
}>(({ theme, isShowLoader }) => ({
	paddingTop: theme.spaces.s,
	maxHeight: MAX_PLAN_SECTION_HEIGHT + BORDER_WIDTHS,

	display: 'flex',
	alignItems: 'flex-start',
	justifyContent: 'flex-start',
	wordBreak: 'break-word',
	whiteSpace: 'normal',

	...(!isShowLoader && {
		...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),
	}),
}));
