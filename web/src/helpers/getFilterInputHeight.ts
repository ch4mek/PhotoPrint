import { PAGE_HEADER_CONTROLS_HEIGHT } from '@/constants/commonStyles';

export const getFilterInputHeight = (isMobileView: boolean | undefined) =>
	!isMobileView ? `${PAGE_HEADER_CONTROLS_HEIGHT}px` : undefined;
