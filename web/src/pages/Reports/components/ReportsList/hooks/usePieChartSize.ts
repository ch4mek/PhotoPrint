import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { useMemo } from 'react';

import { PieChartSize } from '@/components/PieChart/types';

const { desktopXS, desktopL, desktop, desktopS } = breakpoints;

export const usePieChartSize = () => {
	const isDesktopLWidth = useHasScreenWidth(desktopL);
	const isDesktopWidth = useHasScreenWidth(desktop);
	const isDesktopSWidth = useHasScreenWidth(desktopS);
	const isDesktopXSWidth = useHasScreenWidth(desktopXS);

	const size = useMemo<PieChartSize>(() => {
		if (isDesktopLWidth) {
			return 'large';
		}

		if (isDesktopWidth) {
			return 'medium';
		}

		if (isDesktopSWidth) {
			return 'extraSmall';
		}

		if (isDesktopXSWidth) {
			return 'small';
		}

		return 'extraLarge';
	}, [isDesktopWidth, isDesktopXSWidth, isDesktopLWidth, isDesktopSWidth]);

	return size;
};
