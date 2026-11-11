import { useTheme } from '@emotion/react';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { useMemo } from 'react';

import {
	COMMENT_SECTION_HEIGHT,
	GAPS_COUNT,
	UPCOMING_PERFOMANCE_REVIEW_HEIGHT,
} from '../constants';
import { minHeightTabRestriction } from '../TeamBoardUserTabs/constants';
import { TeamBoardTabsId, UseTabsSectionHeightParams } from '../types';
import { isFullAccess } from '../utils';

import { departmentIdsWithCertificates } from '@/constants/departments';
import { POPUP_HEADER_HEIGHT, VERTICAL_PADDINGS } from '@/constants/popUp';
import { isRegularEmployee } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { useResize } from '@/hooks/useResize';
import { UserClaims } from '@/types/claims';

const { tablet } = breakpoints;

export const useTabsSectionHeight = ({
	activeTab,
	position,
	departmentId,
	accessLevel,
}: UseTabsSectionHeightParams) => {
	const { claims } = useAuth();

	const theme = useTheme();
	const [, windowHeight] = useResize();
	const isMobileView = !useHasScreenWidth(tablet);

	const minHeight = useMemo(() => {
		const isEducationAllowed = Boolean(
			departmentId &&
				position &&
				departmentIdsWithCertificates.includes(departmentId) &&
				isRegularEmployee(position)
		);

		const restrictions: Partial<Record<TeamBoardTabsId, boolean>> = {
			[TeamBoardTabsId.EDUCATION]: isEducationAllowed,
			[TeamBoardTabsId.ABSENCES]: false,
			[TeamBoardTabsId.SALARY_DETAILS]: false,
		};

		const hasAddCertificateButton = isFullAccess(accessLevel);

		if (restrictions[activeTab as TeamBoardTabsId]) {
			return (
				minHeightTabRestriction(isMobileView, hasAddCertificateButton)[
					activeTab as TeamBoardTabsId
				] ?? null
			);
		}

		return null;
	}, [activeTab, departmentId, position, isMobileView]);

	const maxHeight = useMemo(() => {
		const isLimitedPopup =
			checkIsHasClaims([UserClaims.ManageHRModule], claims) &&
			!isFullAccess(accessLevel);

		const isHeightRestricted =
			activeTab !== TeamBoardTabsId.ABSENCES &&
			activeTab !== TeamBoardTabsId.SUMMARY &&
			activeTab !== TeamBoardTabsId.CONTRACTS;

		if (!isHeightRestricted) {
			return null;
		}

		const reviewHeight = isLimitedPopup ? 0 : UPCOMING_PERFOMANCE_REVIEW_HEIGHT;
		const gapsHeight =
			(isLimitedPopup ? GAPS_COUNT - 1 : GAPS_COUNT) * theme.spaces.m;

		return (
			windowHeight -
			VERTICAL_PADDINGS -
			POPUP_HEADER_HEIGHT -
			reviewHeight -
			COMMENT_SECTION_HEIGHT -
			gapsHeight
		);
	}, [activeTab, accessLevel, windowHeight, theme, claims]);

	return { minHeight, maxHeight };
};
