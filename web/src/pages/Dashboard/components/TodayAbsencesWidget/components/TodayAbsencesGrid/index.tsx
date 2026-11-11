import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { memo, useCallback, useMemo } from 'react';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { absencesType } from '@/constants/absence';
import { personalInfoClaimAccess } from '@/constants/claims';
import { DATA_GRID_ROW_HEIGHT_WITH_TAG } from '@/constants/commonStyles';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PopUpSearchParams } from '@/constants/searchParams';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { personalInfoPopupAccessRules } from '@/helpers/personalInfoPopupAccessRules';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import { overrideWidgetGridStyles } from '@/pages/Dashboard/utils/overrideGridStyles';

import { TODAY_ABSENCES_COLUMNS_WIDTHS } from './constants';
import { useTodayAbsencesColumns } from './hooks/useTodayAbsencesColumns';
import { TodayAbsenceGridRow, TodayAbsencesGridComponentProps } from './types';

function TodayAbsencesGridComponent({ data }: TodayAbsencesGridComponentProps) {
	const { claims } = useAuth();
	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const hasPersonalInfoAccess = useMemo(
		() => checkIsHasClaims(personalInfoClaimAccess, claims),
		[claims]
	);
	const handleInquiryPageOpen = useOpenPopUpViaUrl(
		PopUpSearchParams.InquiryIdParam
	);

	const handleUserCrossLinkClick = useCallback(
		(row: TodayAbsenceGridRow) => {
			handlePageOpen(row.userId.toString());
		},
		[handlePageOpen]
	);

	const enableCellClick = useCallback(
		(row: TodayAbsenceGridRow) =>
			canOpenProfile(row.userId) &&
			personalInfoPopupAccessRules.canOpenByRole(
				row.userPosition,
				hasPersonalInfoAccess
			),
		[canOpenProfile, hasPersonalInfoAccess]
	);

	const handleAbsenceCrossLinkClick = useCallback(
		(row: TodayAbsenceGridRow) => {
			handleInquiryPageOpen(row.inquiryId.toString());
		},
		[handleInquiryPageOpen]
	);

	const dataGridColumns = useTodayAbsencesColumns({
		onUserCellClick: handleUserCrossLinkClick,
		onAbsenceCellClick: handleAbsenceCrossLinkClick,
		enableCellClick,
	});

	const dataGridRows: TodayAbsenceGridRow[] = useMemo(
		() =>
			data.map(
				({
					userId,
					inquiryId,
					userFullName,
					userPosition,
					inquiryType,
					inquiryTemplateName,
				}) => ({
					userId,
					inquiryId,
					userFullName,
					userPosition,
					inquiryTemplateName,
					inquiryType: absencesType[inquiryType],
				})
			),
		[data]
	);

	const overrideGridStyles = useMemo(
		() => overrideWidgetGridStyles(TODAY_ABSENCES_COLUMNS_WIDTHS),
		[]
	);

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				columns={dataGridColumns}
				rows={dataGridRows}
				overrideDataGridStyles={overrideGridStyles}
				rowHeight={DATA_GRID_ROW_HEIGHT_WITH_TAG}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				enableVirtualization
			/>
		</DataGridHoverRowWrapper>
	);
}

export const TodayAbsencesGrid = memo(TodayAbsencesGridComponent);
