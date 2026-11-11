import { spaces } from '@mms/mms-ui-library/theme';

import { TeamBoardTabsId } from '../types';

import {
	MIN_ABSENCE_CARDS_HEIGHT,
	MIN_ABSENCE_HISTORY_HEIGHT,
	MIN_ABSENCE_REQUESTS_HEIGHT,
	MOBILE_MIN_ABSENCE_HISTORY_HEIGHT,
} from '@/constants/absence';
import {
	getMinCertificateGridHeight,
	MIN_EDUCATION_GRID_HEIGHT,
	MIN_PAYSLIPS_GRID_HEIGHT,
	MIN_SALARY_GRID_HEIGHT,
} from '@/pages/TeamBoardUser/constants';

const TABS_HEIGHT = 40;
const VERTICAL_PADDINGS = spaces.m + spaces.s;

const getMinEducationTabHeight = (hasAddButton?: boolean) =>
	VERTICAL_PADDINGS +
	getMinCertificateGridHeight(hasAddButton) +
	MIN_EDUCATION_GRID_HEIGHT +
	spaces.m +
	TABS_HEIGHT;

const MIN_SALARY_TAB_HEIGHT =
	VERTICAL_PADDINGS +
	MIN_SALARY_GRID_HEIGHT +
	MIN_PAYSLIPS_GRID_HEIGHT +
	spaces.m +
	TABS_HEIGHT;

const DEFAULT_MIN_ABSENCES_TAB_HEIGHT =
	VERTICAL_PADDINGS +
	spaces.s +
	MIN_ABSENCE_CARDS_HEIGHT +
	MIN_ABSENCE_REQUESTS_HEIGHT +
	TABS_HEIGHT +
	spaces.m +
	spaces.l +
	spaces.xs / 2;

const DESKTOP_MIN_ABSENCES_TAB_HEIGHT =
	MIN_ABSENCE_HISTORY_HEIGHT + DEFAULT_MIN_ABSENCES_TAB_HEIGHT;
const MOBILE_MIN_ABSENCES_TAB_HEIGHT =
	MOBILE_MIN_ABSENCE_HISTORY_HEIGHT + DEFAULT_MIN_ABSENCES_TAB_HEIGHT;

export const minHeightTabRestriction = (
	isMobileRestrict: boolean,
	hasAddButton?: boolean
): Partial<Record<TeamBoardTabsId, number>> => ({
	[TeamBoardTabsId.EDUCATION]: getMinEducationTabHeight(hasAddButton),
	[TeamBoardTabsId.SALARY_DETAILS]: MIN_SALARY_TAB_HEIGHT,
	[TeamBoardTabsId.ABSENCES]: isMobileRestrict
		? MOBILE_MIN_ABSENCES_TAB_HEIGHT
		: DESKTOP_MIN_ABSENCES_TAB_HEIGHT,
});
