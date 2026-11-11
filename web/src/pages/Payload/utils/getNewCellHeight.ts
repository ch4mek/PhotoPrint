import { spaces } from '@mms/mms-ui-library/theme';

import {
	FIRST_PROJECTS_ROW_HEIGHT,
	HEIGHT_CELL,
	GRID_SUBROW_HEIGHT,
	HEIGHT_TIME_CELL,
	MAX_PROJECTS_FITS,
	PROJECTS_ROW_HEIGHT,
} from '@/pages/Payload/contants';

export function getNewCellHeight(
	isCellExpanded: boolean,
	projectsCount: number,
	activeProjectsCount: number,
	showTime?: boolean
) {
	const currentNonExpandedHeight =
		activeProjectsCount > MAX_PROJECTS_FITS
			? FIRST_PROJECTS_ROW_HEIGHT +
			  (activeProjectsCount - 1) * PROJECTS_ROW_HEIGHT +
			  spaces.s
			: HEIGHT_CELL;

	const additionalHeight = showTime ? HEIGHT_TIME_CELL : 0;

	return (
		(isCellExpanded
			? (GRID_SUBROW_HEIGHT + additionalHeight) * projectsCount +
			  currentNonExpandedHeight
			: currentNonExpandedHeight) + additionalHeight
	);
}
