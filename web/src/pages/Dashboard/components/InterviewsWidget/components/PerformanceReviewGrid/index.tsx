import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { memo, useCallback, useMemo } from 'react';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { personalInfoClaimAccess } from '@/constants/claims';
import { DATA_GRID_ROW_HEIGHT_WITH_TAG } from '@/constants/commonStyles';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PopUpSearchParams } from '@/constants/searchParams';
import { PASCAL_TO_KEBAB_REGEX } from '@/constants/staffing';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { formatUtcTime } from '@/helpers/date';
import { personalInfoPopupAccessRules } from '@/helpers/personalInfoPopupAccessRules';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import {
	INTERVIEW_ROW_CLASSES,
	STATUS_CLASS_PREFIX,
} from '@/pages/Staffing/components/InterviewTab/constants';
import { getOverrideInterviewGridStyles } from '@/pages/Staffing/components/InterviewTab/utils/getOverridedInterviewGridStyles';

import { interviewColumnWidth } from './constants';
import { useGetGridColumns } from './hooks/useGetGridColumns';
import { InterviewsGridComponentProps, InterviewsGridRow } from './types';

function InterviewsGridComponent({ data }: InterviewsGridComponentProps) {
	const { claims } = useAuth();
	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const hasPersonalInfoAccess = useMemo(
		() => checkIsHasClaims(personalInfoClaimAccess, claims),
		[claims]
	);

	const handleCrossLinkClick = useCallback(
		(row: InterviewsGridRow) => {
			handlePageOpen(row.userId.toString());
		},
		[handlePageOpen]
	);

	const enableCellClick = useCallback(
		(row: InterviewsGridRow) =>
			canOpenProfile(row.userId) &&
			personalInfoPopupAccessRules.canOpenByRole(
				row.userPosition,
				hasPersonalInfoAccess
			),
		[canOpenProfile, hasPersonalInfoAccess]
	);

	const columns = useGetGridColumns(
		interviewColumnWidth,
		handleCrossLinkClick,
		enableCellClick
	);

	const rows = useMemo(
		() =>
			data.map((item) => ({
				...item,
				time: formatUtcTime(item.userInterviewFromUtc) || '',
			})),
		[data]
	);

	const rowClass = useMemo(
		() => (row: InterviewsGridRow) => {
			const classes: string[] = [];

			if (row.status) {
				const statusClass = row.status
					.replace(PASCAL_TO_KEBAB_REGEX, '$1-$2')
					.toLowerCase();
				classes.push(`${STATUS_CLASS_PREFIX}${statusClass}`);
			} else {
				classes.push(INTERVIEW_ROW_CLASSES.statusWaitingForInterview);
			}

			return classes.join(' ');
		},
		[]
	);

	const overrideDataGridStyles = useMemo(
		() =>
			getOverrideInterviewGridStyles(
				interviewColumnWidth,
				columns.length,
				rows.length,
				true
			),
		[columns.length, rows.length]
	);

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				{...{ rows, columns, overrideDataGridStyles, rowClass }}
				rowHeight={DATA_GRID_ROW_HEIGHT_WITH_TAG}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				enableVirtualization
			/>
		</DataGridHoverRowWrapper>
	);
}

export const InterviewsGrid = memo(InterviewsGridComponent);
