import { ScrollbarProps } from '@mms/mms-ui-library/components/Scrollbar';

import { BAR_MARGIN_TOP } from '../Chart/ChartContent/constants';

import { BarBorders } from './types';

export const HEIGHT_OF_CHART = 224;
export const YAXIS_WIDTH = 53;
export const MIN_BAR_CATEGORY_GAP = 8;

export const CHART_TOP_OFFSET = 0.5; // 50%

export const X_AXIS_MONTHLY_STATS_HEIGHT = 52;
export const X_AXIS_NOT_MONTHLY_STATS_HEIGHT = 35;
export const X_AXIS_TICK_MARGIN = 14;
export const X_AXIS_WIDTH = 20;
export const STACKED_BAR_STRAIGHT: BarBorders = [0, 0, 0, 0];

export const TOTAL_BAR_NAME = 'total';

export const MIN_COLUMN_WIDTHS = {
	mobile: 40,
	desktop: 30,
};
export const BAR_CHART_MARGINS = {
	top: BAR_MARGIN_TOP,
};

export const RESPONSIVE_CONTAINER_INLINE_STYLE = {
	// one pixel should be subtracted to eliminate the appearance
	// of premature horizontal scrolling
	maxWidth: 'calc(100% - 1px)',
};

export const SCROLLBAR_PROPS: ScrollbarProps = {
	scroll: 'horizontal',
	maxWidth: '100%',
	maxHeight: '100%',
	customScrollbarWidth: 3,
};

export const Y_AXIS_DOMAIN_MAX_BASE_SIZE = 1;
export const Y_AXIS_TICKS_STEP = 4;
export const Y_AXIS_TICKS_COUNT = 5;
