import { spaces } from '@mms/mms-ui-library/theme';

import { DocViewerHeaderPosition } from '../types';

import { usePosition } from '@/hooks/usePosition';

export const calcHeaderPosition = (
	windowWidth: number,
	containerPosition: ReturnType<typeof usePosition>
): DocViewerHeaderPosition => ({
	left: containerPosition?.left,
	right: containerPosition ? windowWidth - containerPosition.right : undefined,
	top: containerPosition ? containerPosition.top + spaces.m : undefined,
});
