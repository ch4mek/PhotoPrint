import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { useMemo } from 'react';

import {
	MIN_COLUMN_WIDTHS,
	MIN_BAR_CATEGORY_GAP,
	YAXIS_WIDTH,
} from '../constants';

// columnContainerPadding is the default value of Rechartjs; cannot be changed (in pixels)
const columnContainerPadding = 10;
const { desktopXS } = breakpoints;

export function useChartSizes(dataLenght: number) {
	const isDesktopXSWidth = useHasScreenWidth(desktopXS);

	const minColumnWidthWithoutGap = isDesktopXSWidth
		? MIN_COLUMN_WIDTHS.desktop
		: MIN_COLUMN_WIDTHS.mobile;

	const minColumnWidthWithGap = minColumnWidthWithoutGap + MIN_BAR_CATEGORY_GAP;

	// necessary to limit the minimum width of one column (minColumnWidthWithoutGap = 40 pixels)
	// and the appearance of horizontal scrolling when the width of one column is less than 40 pixels.
	// Rechartjs does not provide this functionality
	const responsiveContainerMinWidth = useMemo(() => {
		if (dataLenght) {
			const minResponsiveContainerWidth =
				minColumnWidthWithGap * dataLenght +
				columnContainerPadding +
				YAXIS_WIDTH;

			return minResponsiveContainerWidth;
		}

		return 0;
	}, [dataLenght, isDesktopXSWidth]);

	const barCategoryGapInPercents = useMemo(() => {
		if (dataLenght) {
			const gap = ((MIN_BAR_CATEGORY_GAP / 2) * 100) / minColumnWidthWithGap;

			return `${gap}%`;
		}

		return '0%';
	}, [dataLenght, isDesktopXSWidth]);

	return { responsiveContainerMinWidth, barCategoryGapInPercents };
}
