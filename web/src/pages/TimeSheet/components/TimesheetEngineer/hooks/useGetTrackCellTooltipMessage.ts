import { useContext, useMemo } from 'react';
import { CalculatedColumn } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { TimeSheetContext } from '..';
import { TimeSheetRow } from '../types';
import {
	getTimeCellHoverMessage,
	isDayBlocked,
	isDayDisabledByProjectAssignDates,
} from '../utils';

import { OUTSIDE_USER_ASSIGN_DATE } from '@/constants/errors';

export const useGetTrackCellTooltipMessage = (
	row: TimeSheetRow,
	column: CalculatedColumn<TimeSheetRow, unknown>,
	isEngineer: boolean
) => {
	const { t } = useTranslation('common', { keyPrefix: 'tooltips' });
	const {
		userHasAccessToProject,
		isProjectActive,
		disabledDates,
		projectId,
		entireRowHoverMessage,
	} = row;
	const { key } = column;

	const { blockedDays } = useContext(TimeSheetContext);

	const dayBlockError = useMemo(
		() =>
			getTimeCellHoverMessage(
				isEngineer,
				key,
				userHasAccessToProject,
				isProjectActive,
				isDayBlocked(blockedDays, key),
				t,
				entireRowHoverMessage
			),
		[
			isEngineer,
			key,
			userHasAccessToProject,
			isProjectActive,
			blockedDays,
			entireRowHoverMessage,
		]
	);

	const isDisabledByAssignDateError = useMemo(
		() =>
			isDayDisabledByProjectAssignDates(key, disabledDates)
				? OUTSIDE_USER_ASSIGN_DATE
				: undefined,
		[disabledDates, key]
	);

	const projectIsNotSelectedError = useMemo(
		() => (!projectId ? t('projectFirst') : undefined),
		[projectId]
	);

	const tooltipMessage =
		dayBlockError || isDisabledByAssignDateError || projectIsNotSelectedError;

	return tooltipMessage;
};
