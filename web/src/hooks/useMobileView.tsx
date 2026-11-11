import { useMemo } from 'react';

import { useResize } from '@/hooks/useResize';
import { TimeSheetBreakpoints } from '@/pages/TimeSheet/constants';

export const useMobileView = (breakpoint = TimeSheetBreakpoints.TABLET_LG) => {
	const [viewportWidth] = useResize();

	const isMobileView = useMemo(
		() => viewportWidth <= breakpoint - 1,
		[viewportWidth, breakpoint]
	);

	return isMobileView;
};
